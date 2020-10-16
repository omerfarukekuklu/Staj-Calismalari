using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using MyEvernote.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEvernote.Entities.Messages;
using MyEvernote.Common.Helper;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.BusinessLayer.Abstract;

namespace MyEvernote.BusinessLayer
{
    public class EvernoteUserManager : ManagerBase<EvernoteUser>
    {
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterViewModel data)
        {
            EvernoteUser username_check = Find(x => x.Username == data.Username);
            EvernoteUser email_check = Find(x => x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (username_check != null || email_check != null)
            {
                if (username_check != null)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (email_check != null)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExit, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            int dbResult = base.Insert(new EvernoteUser()
            {
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                ProfileImageFilename = "profilepicture.png",
                ActivateGuid = Guid.NewGuid(),
                IsActive = false,
                IsAdmin = false
            });

            if (dbResult > 0)
            {
                res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                #region Aktivasyon Maili Gönderme
                SendActivationMail(res.Result);
                #endregion
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            res.Result = Find(x => x.Id == id);

            if (res == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel data)
        {
            EvernoteUser user = Find(x => x.Username == data.Username && x.Password == data.Password);
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            res.Result = user;

            if (user != null)
            {
                if (!user.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı ya da şifre yanlış.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateProfile(EvernoteUser data)
        {
            EvernoteUser username_check = Find(x => x.Id != data.Id && x.Username == data.Username);
            EvernoteUser email_check = Find(x => x.Id != data.Id && x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (username_check != null || email_check != null)
            {
                if (username_check != null)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (email_check != null)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExit, "E-posta adresi kullanılıyor.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);

            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Username = data.Username;
            res.Result.Password = data.Password;
            res.Result.Email = data.Email;

            if (!string.IsNullOrWhiteSpace(data.ProfileImageFilename))
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }

            if (Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> RemoveUserById(int id)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.ActivateGuid == activateId);

            if (user != null)
            {
                if (user.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }

                user.IsActive = true;
                Update(user);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdNotFound, "Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> InsertUser(EvernoteUser data)
        {
            EvernoteUser username_check = Find(x => x.Username == data.Username);
            EvernoteUser email_check = Find(x => x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (username_check != null || email_check != null)
            {
                if (username_check != null)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (email_check != null)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExit, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            int dbResult = base.Insert(new EvernoteUser()
            {
                Name = data.Name,
                Surname = data.Surname,
                Username = data.Username,
                Email = data.Email,
                Password = data.Password,
                ProfileImageFilename = "profilepicture.png",
                ActivateGuid = Guid.NewGuid(),
                IsActive = data.IsActive,
                IsAdmin = data.IsAdmin
            });

            if (dbResult > 0)
            {
                res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<EvernoteUser> UpdateUser(EvernoteUser data)
        {
            EvernoteUser username_check = Find(x => x.Id != data.Id && x.Username == data.Username);
            EvernoteUser email_check = Find(x => x.Id != data.Id && x.Email == data.Email);

            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();

            if (username_check != null || email_check != null)
            {
                if (username_check != null)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (email_check != null)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExit, "E-posta adresi kullanılıyor.");
                }

                return res;
            }

            res.Result = Find(x => x.Id == data.Id);

            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Username = data.Username;
            res.Result.Password = data.Password;
            res.Result.Email = data.Email;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

            if (Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }

        public override int Delete(EvernoteUser user)
        {
            if(user.Notes.Count > 0)
            {
                NoteManager noteManager = new NoteManager();

                //Kullanıcının notlarının silinmesi
                foreach (Note note in user.Notes.ToList())
                {
                    noteManager.Delete(note);
                }                  
            }
            
            if(user.Comments.Count > 0)
            {
                CommentManager commentManager = new CommentManager();

                //Kullanıcının yorumlarının silinmesi
                foreach(Comment comment in user.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }
            }

            if(user.Likes.Count > 0)
            {
                LikedManager likedManager = new LikedManager();

                //Kullanıcının beğenilerinin silinmesi
                foreach(Liked like in user.Likes.ToList())
                {
                    likedManager.Delete(like);
                }
            }

            return base.Delete(user);
        }

        public void SendActivationMail(EvernoteUser user)
        {
            string siteUri = ConfigHelper.Get<string>("SiteRootUri");
            string activateUri = $"{siteUri}/Home/UserActivate/{user.ActivateGuid}";
            string body = $"Merhaba {user.Username};<br><br>Hesabınızı aktifleştirmek için " +
                $"<a href='{activateUri}' target='_blank'> tıklayınız</a>.";

            MailHelper.SendMail(body, user.Email, "MyEvernote Hesap Aktivasyonu");
        }
    }
}
