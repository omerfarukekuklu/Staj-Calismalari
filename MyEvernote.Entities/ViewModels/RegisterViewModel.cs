using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(18, ErrorMessage = "{0} maksimum {1} karakter olabilir.")]
        public string Username { get; set; }

        [DisplayName("E-posta"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(70, ErrorMessage = "{0} maksimum {1} karakter olabilir."), 
            RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", 
            ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(25, ErrorMessage = "{0} maksimum {1} karakter olabilir."), 
            MinLength(4, ErrorMessage = "Şifre {1} karakterden kısa olamaz.")]
        public string Password { get; set; }

        [DisplayName("Şifre Tekrar"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(25, ErrorMessage = "{0} maksimum {1} karakter olabilir."),
            Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
        public string RePassword { get; set; }

    }
}