using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("EvernoteUsers")]
    public class EvernoteUser : MyEntitiyBase
    {
        [DisplayName("Ad"), StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Name { get; set; }

        [DisplayName("Soyad"), StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez"), 
            StringLength(18, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Username { get; set; }

        [DisplayName("E-posta"), Required(ErrorMessage = "{0} alanı boş geçilemez"), 
            StringLength(70, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir."),
            RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez"), 
            StringLength(25, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir."),
            MinLength(4, ErrorMessage = "Şifre {1} haneden kısa olacamaz.")]
        public string Password { get; set; }

        [StringLength(30), ScaffoldColumn(false)] //images/username.jpg
        public string ProfileImageFilename { get; set; }

        [DisplayName("Aktif mi?")]
        public bool IsActive { get; set; }

        [DisplayName("Admin mi?")]
        public bool IsAdmin { get; set; }

        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }

        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }
    }
}
