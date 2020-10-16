using MyEvernote.DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class InitializeDb
    {
        public static bool Initialize()
        {
            bool state, result = true;

            using (DatabaseContext db = new DatabaseContext())
            {
                state = db.Database.Exists();

                //try
                //{
                    if (state)
                    {
                        db.NoteCategories.ToList();
                        if (db.NoteCategories.Count() == 0)
                        {
                            MyInitializer.SeedData(db);
                        }
                    }
                    else
                    {
                        db.Database.CreateIfNotExists();
                        db.NoteCategories.ToList();
                        if (db.NoteCategories.Count() == 0)
                        {
                            MyInitializer.SeedData(db);
                        }
                    }
                //}
                //catch (Exception)
                //{
                //    result = false;
                //}
            }
            return result;
        }
    }
}
