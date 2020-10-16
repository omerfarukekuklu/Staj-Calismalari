using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ViewModels;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        CategoryManager categoryManager = new CategoryManager();
        EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        // GET: Home
        public ActionResult Index()
        {
            //object o = 0;
            //int a = 1;
            //int c = a / (int)o;

            return View(noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(nm.GetNotesQueryable().OrderByDescending(x => x.ModifiedOn));
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category cat = categoryManager.Find(x => x.Id == id.Value);

            if (cat == null)
            {
                return HttpNotFound();
                //return RedirectToAction("Index", "Home");
            }

            IQueryable<Note> notes = noteManager.ListQueryable().Where(x => x.CategoryId == cat.Id && x.IsDraft == false);

            string referredUrl = Request.UrlReferrer.AbsoluteUri;

            if (referredUrl.Contains("MostLiked"))
            {
                return View("Index", notes.OrderByDescending(x => x.LikeCount).ToList());
            }

            return View("Index", notes.OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.GetUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }

        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
                (ProfileImage.ContentType == "image/jpeg" ||
                ProfileImage.ContentType == "image/jpg" ||
                ProfileImage.ContentType == "image/png"))
                {
                    string fileName = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/images/{fileName}"));
                    model.ProfileImageFilename = fileName;
                }

                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);

                    ////Bildirim ekranı ile hata gösterimi
                    //ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    //{
                    //    Items = res.Errors,
                    //    Title = "Profil Güncellenemedi",
                    //    RedirectUrl = "/Home/EditProfile"
                    //};

                    //return View("Error", errorNotifyObj);
                }

                CurrentSession.Set("login", res.Result); // Profil güncellendiği için session güncellendi.                

                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.RemoveUserById(CurrentSession.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi",
                    RedirectUrl = "/Home/ShowProfile"
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            //string isSent = TempData["isSent"] as string;
            //TempData["isSent"] = isSent;

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> result = evernoteUserManager.LoginUser(model);

                if (result.Errors.Count > 0)
                {
                    if (result.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActive) != null)
                    {
                        ViewBag.Activate = "/Home/SendActivationMail/";
                        ViewBag.ActivationButtonText = "Aktivasyon maili gönder";
                        TempData["user"] = model;
                    }

                    result.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                CurrentSession.Set("login", result.Result); // Session'a kullanıcı bilgisini saklama

                return RedirectToAction("Index"); // yönlendirme
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> result = evernoteUserManager.RegisterUser(model);

                if (result.Errors.Count > 0)
                {
                    result.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                OKViewModel notifyObj = new OKViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectUrl = "/Home/Login",
                };

                notifyObj.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı" +
                    " aktive ediniz. Hesabınızı aktive etmeden not oluşturamaz ve beğeni yapamazsınız.");

                return View("OK", notifyObj);
            }

            return View(model);
        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = evernoteUserManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };

                return View("Error", errorNotifyObj);
            }

            OKViewModel okNotifyObject = new OKViewModel()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectUrl = "/Home/Login",
            };

            okNotifyObject.Items.Add("Hesabınız başarıyla aktifleştirildi. Artık notlarınızı oluşturabilir " +
                "ve notları beğenebilirsiniz.");

            return View("OK", okNotifyObject);
        }

        public ActionResult Logout()
        {
            CurrentSession.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }

        public ActionResult SendActivationMail()
        {
            LoginViewModel model = TempData["user"] as LoginViewModel;

            EvernoteUser user = evernoteUserManager.Find(x => x.Username == model.Username);

            TempData["isSent"] = "true";

            //evernoteUserManager.SendActivationMail(user);

            return RedirectToAction("Login");
        }
    }
}