using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class NoteController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();
        EvernoteUser user = CurrentSession.User;

        [Auth]
        public ActionResult Index()
        {
            var notes = noteManager.ListQueryable().Include("Category").Include("Owner").Where(
                x => x.Owner.Id == user.Id).OrderByDescending(x => x.ModifiedOn);

            return View(notes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                note.Owner = user;
                noteManager.Insert(note);

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                Note db_note = noteManager.Find(x => x.Id == note.Id);

                db_note.IsDraft = note.IsDraft;
                db_note.CategoryId = note.CategoryId;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                noteManager.Update(db_note);

                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);

            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            return View(note);
        }

        [HttpPost, ActionName("Delete"), Auth]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = noteManager.Find(x => x.Id == id);
            noteManager.Delete(note);

            return RedirectToAction("Index");
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(
                x => x.LikedUser.Id == user.Id).Select(
                x => x.Note).Include("Category").OrderByDescending(
                x => x.ModifiedOn);

            int i = notes.ToList().Count;
            return View("Index", notes.ToList());
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {            
            List<int> likedNoteIds = new List<int>();

            if (user != null)
            {
                likedNoteIds = likedManager.List(
                    x => x.LikedUser.Id == user.Id &&
                    ids.Contains(x.Note.Id)).Select(x => x.Note.Id).ToList();
                return Json(new { result = likedNoteIds });
            }

            return null;
        }

        [HttpPost]
        public ActionResult SetLikeState(int? noteid, bool liked)
        {
            int res = 0;

            if(user == null)
            {
                return Json(new { hasError = true, errorMessage = "Notları beğenebilmek için lütfen giriş yapın." });
            }
            else
            {
                Liked like = likedManager.Find(x => x.Note.Id == noteid && x.LikedUser.Id == user.Id);

                Note note = noteManager.Find(x => x.Id == noteid);

                if (like != null && liked == false)
                {
                    res = likedManager.Delete(like);
                }
                else if (like == null && liked == true)
                {
                    like = new Liked()
                    {
                        LikedUser = user,
                        Note = note
                    };

                    res = likedManager.Insert(like);
                }

                if (res > 0)
                {
                    if (liked)
                    {
                        note.LikeCount++;
                    }
                    else
                    {
                        note.LikeCount--;
                    }

                    res = noteManager.Update(note);

                    return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
                }

                return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = note.LikeCount });
            }
            
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(x => x.Id == id.Value);

            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNoteText", note);
        }
    }
}
