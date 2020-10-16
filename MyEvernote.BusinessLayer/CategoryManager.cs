using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.Entities;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MyEvernote.BusinessLayer
{
    public class CategoryManager : ManagerBase<Category>
    {
        public override int Delete(Category category)
        {
            if (category.Notes.Count > 0)
            {
                NoteManager noteManager = new NoteManager();

                // Kategori ile ilişkili notların silinmesi
                foreach (Note note in category.Notes.ToList())
                {
                    noteManager.Delete(note);
                }
            }

            return base.Delete(category);
        }

    }
}
