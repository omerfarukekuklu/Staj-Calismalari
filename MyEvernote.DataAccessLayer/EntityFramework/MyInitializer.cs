using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public static class MyInitializer //:CreateDatabaseIfNotExists<DatabaseContext>
    {
        //protected override void Seed(DatabaseContext context)

        public static void SeedData(DatabaseContext context)
        {
            //Adding admin user...
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Omer",
                Surname = "Ekuklu",
                Email = "omerfarukekuklu@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                ProfileImageFilename = "profilepicture.png",
                Username = "omerekuklu",
                Password = "123456",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "omerekuklu"
            };

            //Adding standard user...
            EvernoteUser standardUser = new EvernoteUser()
            {
                Name = "Faruk",
                Surname = "Ekuklu",
                Email = "farukekuklu@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                ProfileImageFilename = "profilepicture.png",
                Username = "farukekuklu",
                Password = "654321",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "omerekuklu"
            };

            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standardUser);

            //Adding fake users...
            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    ProfileImageFilename = "profilepicture.png",
                    Username = $"user{i}",
                    Password = "1234",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"
                };

                context.EvernoteUsers.Add(user);
            }
      
            context.SaveChanges();

            //User list
            List<EvernoteUser> userlist = context.EvernoteUsers.ToList();

            // Adding fake categories...
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername = "omerekuklu"
                };

                // Adding  fake notes...
                for (int k = 0; k < FakeData.NumberData.GetNumber(5, 9); k++)
                {
                    EvernoteUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count)];
                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                        Category = cat,
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username
                    };                    

                    //Adding fake comments...
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                    {
                        EvernoteUser comment_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count)];
                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Note = note,
                            Owner = comment_owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = comment_owner.Username,                           
                        };

                        note.Comments.Add(comment);
                    }
                   
                    //Adding fake likes
                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {                           
                            LikedUser = userlist[m],
                            Note = note
                        };
                        note.Likes.Add(liked);
                    }

                    cat.Notes.Add(note);
                }

                context.NoteCategories.Add(cat);
                context.SaveChanges();
            }
        }
    }
}
