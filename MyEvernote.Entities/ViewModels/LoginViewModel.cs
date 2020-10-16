using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(18, ErrorMessage = "{0} maksimum {1} karakter olabilir.")]
        public string Username { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = " {0} alanı boş geçilemez."), 
            StringLength(25, ErrorMessage = "{0} maksimum {1} karakter olabilir.")]
        public string Password { get; set; }

    }
}