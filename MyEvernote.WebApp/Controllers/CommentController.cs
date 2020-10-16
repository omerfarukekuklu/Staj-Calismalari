using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class CommentController : Controller
    {      
        public ActionResult ShowNoteComments(int? id)
        {          
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }

            NoteManager noteManager = new NoteManager();

            //Note note = noteManager.Find(x => x.Id == id);
            Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);

            if(note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments", note.Comments);
        }

        [Auth]
        [HttpPost]
        public ActionResult Create(Comment comment, int? noteid)
        {
            /* Ajax istediğinden gönderilen text değeri ismi aynı olduğu için comment nesnesinin
             text propertisine yazılır.*/
            
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                CommentManager commentManager = new CommentManager();
                NoteManager noteManager = new NoteManager();

                if (noteid == null)
                {
                    return HttpNotFound();
                }

                Note note = noteManager.Find(x => x.Id == noteid);

                if (note == null)
                {
                    return HttpNotFound();
                }

                comment.Note = note;
                comment.Owner = CurrentSession.User;

                if (commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        } 

        /* Actionun ilk parametre ismi id olmalı, diğer parametreler ise javascriptte
                gönderirken yazdığımız değişken isimleriyle aynı olmalı. */
        [Auth]
        [HttpPost]
        public ActionResult Edit(int? id, Comment cmnt)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                CommentManager commentManager = new CommentManager();

                if (id == null)
                {
                    return HttpNotFound();
                }

                Comment comment = commentManager.Find(x => x.Id == id);

                if (comment == null)
                {
                    return HttpNotFound();
                }

                comment.Text = WebUtility.HtmlDecode(cmnt.Text); 

                if (commentManager.Update(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        // Actionun ilk parametre ismi id olmalı
        [Auth]
        public ActionResult Delete(int? id)
        {
            CommentManager commentManager = new CommentManager();

            if (id == null)
            {
                return HttpNotFound();
            }

            Comment comment = commentManager.Find(x => x.Id == id);

            if (comment == null)
            {
                return HttpNotFound();
            }

            if (commentManager.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}