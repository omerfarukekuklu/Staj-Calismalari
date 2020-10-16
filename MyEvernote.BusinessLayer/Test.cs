using MyEvernote.DataAccessLayer;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class Test
    {
        private static Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        private static Repository<Category> repo_category = new Repository<Category>();
        private static Repository<Comment> repo_comment = new Repository<Comment>();
        private static Repository<Note> repo_note = new Repository<Note>();
     
        public static void ListTest()
        {            
            List<Category> categories = repo_category.List();
            List<Category> categories_filtered = repo_category.List(x => x.Id > 5);
        }

        public static void InsertTest()
        {          
            int result = repo_user.Insert(new EvernoteUser()
            {
                Name = "aaa",
                Surname = "bbb",
                Email = "aaabbb@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "aaabbb",
                Password = "123",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "aaabbb"
            });
        }

        public static void UpdateTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Name == "aaa");

            if(user != null)
            {
                user.Name = "xxx";
            }

            int result = repo_user.Update(user);
        }

        public static void DeleteTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Name == "xxx");

            if(user!= null)
            {
                int result = repo_user.Delete(user);
            }
        }

        public static void CommentInsertTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Id == 1);
            Note note = repo_note.Find(x => x.Id == 3);

            Comment comment = new Comment()
            {
                Text = "Bu bir Test Comment",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "omerekuklu",
                Note = note,
                Owner = user
            };

            int result = repo_comment.Insert(comment);
        }
    }
}
