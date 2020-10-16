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
using MyEvernote.Entities.Messages;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;

namespace MyEvernote.WebApp.Controllers
{
    [Auth, AuthAdmin, Exc]
    public class CategoryController : Controller
    {
        private CategoryManager categoryManager = new CategoryManager();

        public ActionResult Index()
        {
            return View(categoryManager.ListQueryable().OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryManager.Find(x => x.Id == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                Category cat = categoryManager.Find(x => x.Title == category.Title);

                if (cat != null)
                {
                    ModelState.AddModelError("", "Bu isimli bir kategori zaten bulunmakta.");
                    return View(category);
                }

                categoryManager.Insert(category);
                CacheHelper.RemoveCategoriesFromCache();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryManager.Find(x => x.Id == id.Value);

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            ModelState.Remove("ModifiedUserName");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");

            if (ModelState.IsValid)
            {
                Category cat = categoryManager.Find(x => x.Id != category.Id && x.Title == category.Title);

                if (cat != null)
                {
                    ModelState.AddModelError("", "Bu isimli bir kategori zaten bulunmakta.");
                    return View(category);
                }

                cat = categoryManager.Find(x => x.Id == category.Id);

                cat.Title = category.Title;
                cat.Description = category.Description;

                int result = categoryManager.Update(cat);
                CacheHelper.RemoveCategoriesFromCache();

                if (result == 0)
                {
                    ErrorViewModel err = new ErrorViewModel()
                    {
                        RedirectUrl = "/Category/Index",
                        Title = "Kategori eklenemedi.",
                        Header = "İşlem Başarısız",
                        IsRedirecting = true,
                    };

                    err.Items.Add(new ErrorMessageObj()
                    {
                        Code = ErrorMessageCode.CategoryCouldNotUpdated,
                        Message = "Kategoriyi ekleme işlemi başarısız oldu."
                    });

                    return View("Error", err);
                }

                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = categoryManager.Find(x => x.Id == id);
            categoryManager.Delete(category);
            CacheHelper.RemoveCategoriesFromCache();

            return RedirectToAction("Index");
        }
    }
}
