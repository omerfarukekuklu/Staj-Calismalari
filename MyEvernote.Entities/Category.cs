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
    [Table("NoteCategories")]
    public class Category : MyEntitiyBase
    {
        [DisplayName("Kategori"), Required(ErrorMessage = "{0} alanı boş bırakılamaz."), 
            StringLength(50, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Title { get; set; }

        [DisplayName("Açıklama"), StringLength(150, ErrorMessage = "{0} alanı en fazla {1} karakter olabilir.")]
        public string Description { get; set; }

        public virtual List<Note> Notes { get; set; }

        public Category()
        {
            Notes = new List<Note>();
        }
    }
}
