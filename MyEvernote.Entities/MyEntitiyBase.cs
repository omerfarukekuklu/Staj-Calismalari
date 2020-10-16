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
    public class MyEntitiyBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Oluşturma Tarihi"), Required]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Değiştirme Tarihi"), Required]
        public DateTime ModifiedOn { get; set; }

        [DisplayName("Son Düzenleyen"), Required, StringLength(18)]
        public string ModifiedUsername { get; set; }
    }
}
