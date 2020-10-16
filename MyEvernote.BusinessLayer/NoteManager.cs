using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class NoteManager : ManagerBase<Note>
    {
        public override int Delete(Note note)
        {
            if (note.LikeCount > 0)
            {
                LikedManager likedManager = new LikedManager();

                //Not ile ilişkili beğenilerin silinmesi
                foreach (Liked like in note.Likes.ToList())
                {
                    likedManager.Delete(like);
                }
            }

            if (note.Comments.Count > 0)
            {
                //Not ile ilişkili yorumların silinmesi
                CommentManager commentManager = new CommentManager();

                foreach (Comment comment in note.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }
            }

            return base.Delete(note);
        }
    }
}
