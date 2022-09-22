using TaruChat.Models;
using System;
using System.Linq;


namespace TaruChat.Data
{
    public class DbInitializer
    {
        public static void Initialize(ChatContext context)
        {
            context.Database.EnsureCreated();

            //Look for any Classes
            if (context.Classes.Any())
            {
                return;   // DB has been seeded
            }
            

            //Generate DB Class
            var classes = new Class[]
            {
                new Class { ID = "RITG1", Title = "Internet Technology" },
                new Class { ID = "RITG2", Title = "Internet Technology" },
                new Class { ID = "RITG3", Title = "Internet Technology" },
                new Class { ID = "RITG4", Title = "Internet Technology" },
                new Class { ID = "RSDG1", Title = "Software Engineering" },
                new Class { ID = "RSDG2", Title = "Software Engineering" }
            };
            foreach (Class c in classes)
            {
                context.Classes.Add(c);
            }
            context.SaveChanges();

            //Generate DB Subject
            var subjects = new Subject[]
            {
                new Subject{ID="BMIT3094", Title="Advanced Computer Network", StartDate=DateTime.Parse("2021-06-01"), EndDate=DateTime.Parse("2021-10-01")},
                new Subject{ID="BAIT2073", Title="Mobile Application", StartDate=DateTime.Parse("2021-11-01"), EndDate=DateTime.Parse("2022-01-01")},
                new Subject{ID="BACS3033", Title="Social and Professional Issues", StartDate=DateTime.Parse("2021-06-01"), EndDate=DateTime.Parse("2021-10-01")},
                new Subject{ID="BAIT2023", Title="Internet Security", StartDate=DateTime.Parse("2021-11-01"), EndDate=DateTime.Parse("2022-01-01")},
                new Subject{ID="BAIT2203", Title="Human Compter Interaction", StartDate=DateTime.Parse("2021-06-01"), EndDate=DateTime.Parse("2021-10-01")},
                new Subject{ID="BMCS2003", Title="Artificial Intelligence", StartDate=DateTime.Parse("2021-06-01"), EndDate=DateTime.Parse("2021-10-01")}
            };
            foreach (Subject s in subjects)
            {
                context.Subjects.Add(s);
            }
            context.SaveChanges();

            //Generate DB Assign (Many - to - Many)
            var assigns = new Assign[]
            {
                new Assign{ClassID="RITG1", SubjectID="BMIT3094"},
                new Assign{ClassID="RITG1", SubjectID="BAIT2073"},
                new Assign{ClassID="RITG1", SubjectID="BACS3033"},
                new Assign{ClassID="RITG1", SubjectID="BAIT2023"},
                new Assign{ClassID="RITG1", SubjectID="BAIT2203"},
                new Assign{ClassID="RITG1", SubjectID="BMCS2003"},
                new Assign{ClassID="RITG2", SubjectID="BMIT3094"},
                new Assign{ClassID="RITG2", SubjectID="BAIT2073"},
                new Assign{ClassID="RITG3", SubjectID="BACS3033"},
                new Assign{ClassID="RITG3", SubjectID="BAIT2023"},
                new Assign{ClassID="RITG4", SubjectID="BAIT2203"},
                new Assign{ClassID="RITG4", SubjectID="BMCS2003"},
            };
            foreach (Assign a in assigns)
            {
                context.Assigns.Add(a);
            }
            context.SaveChanges();

            //Generate DB User
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User { ID = "20WMR09001", Name = "Kris Wu", Role = Role.User,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==", 
                           Email = "Apple@gmail.com", Gender = 'M' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG1"},
                new User { ID = "20WMR09002", Name = "Cris Ang", Role = Role.User,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==",
                           Email = "Apple@gmail.com", Gender = 'F' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG1"},
                new User { ID = "20WMR09003", Name = "Jimmy Ong", Role = Role.User,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==",
                           Email = "Apple@gmail.com", Gender = 'F' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG2"},
                new User { ID = "20WMR09004", Name = "Jason Leong", Role = Role.User,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==",
                           Email = "Apple@gmail.com", Gender = 'M' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG2"},
                new User { ID = "Tutor1", Name = "Anson Liaw", Role = Role.Tutor,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==",
                           Email = "Apple@gmail.com", Gender = 'M' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG1"},
                new User { ID = "Admin", Name = "Admin 1", Role = Role.Admin,  Password = "AOD6hGRMSazTN8948NMpiGz8wL/7EEu3BjAjiwJbBW/J+FCKnZP0iy6XqnMNLSJtRg==",
                           Email = "Apple@gmail.com", Gender = 'M' , ProfilePic = "../../ProfilePic/Profile.png" ,
                           Status = "Active" , ClassID = "RITG1"},
            };
            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            //Generate DB Chat
            var chats = new Chat[]
            {
                new Chat { ID = "C001", Title = "BMIT3094 Advanced Computer Network (RITG1)" , CreatedAt = DateTime.Parse("2021-11-01"),
                           UpdatedAt = DateTime.Parse("2021-11-01"), DeletedAt = DateTime.Parse("2022-01-01"), ProfilePic = "../../ProfilePic/Profile.png"},
                new Chat { ID = "C002", Title = "BAIT2073 Mobile Application (RITG1)" , CreatedAt = DateTime.Parse("2021-11-01"),
                           UpdatedAt = DateTime.Parse("2021-11-01"), DeletedAt = DateTime.Parse("2022-01-01"), ProfilePic = "../../ProfilePic/Profile.png"},
                new Chat { ID = "C003", Title = "BACS3033 Social and Professional Issues (RITG1)" , CreatedAt = DateTime.Parse("2021-11-01"),
                           UpdatedAt = DateTime.Parse("2021-11-01"), DeletedAt = DateTime.Parse("2022-01-01"), ProfilePic = "../../ProfilePic/Profile.png"},
                new Chat { ID = "C004", Title = "BMIT3094 Advanced Computer Network (RITG2)" , CreatedAt = DateTime.Parse("2021-11-01"),
                           UpdatedAt = DateTime.Parse("2021-11-01"), DeletedAt = DateTime.Parse("2022-01-01"), ProfilePic = "../../ProfilePic/Profile.png"},
            };
            foreach (Chat c in chats)
            {
                context.Chats.Add(c);
            }
            context.SaveChanges();

            //Generate DB Enrollment (Many - to - Many)
            var enrollments = new Enrollment[]
            {
                new Enrollment{ChatID="C001", UserID="20WMR09001"},
                new Enrollment{ChatID="C001", UserID="20WMR09002"},
                new Enrollment{ChatID="C001", UserID="Tutor1"},
                new Enrollment{ChatID="C002", UserID="20WMR09001"},
                new Enrollment{ChatID="C002", UserID="20WMR09002"},
                new Enrollment{ChatID="C003", UserID="20WMR09001"},
                new Enrollment{ChatID="C003", UserID="20WMR09002"},
                new Enrollment{ChatID="C004", UserID="20WMR09003"},
                new Enrollment{ChatID="C004", UserID="20WMR09004"},
            };
            foreach (Enrollment e in enrollments)
            {
                context.Enrollments.Add(e);
            }
            context.SaveChanges();

        }
    }
}
