using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public enum Role
    {
        User, Tutor, Admin
    }
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string Name { get; set; }
        public Role? Role { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public char Gender { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePic { get; set; }
        public string Status { get; set; }

        [Display(Name = "Class ID")]
        public string ClassID { get; set; }
        public Class Class { get; set; }
        
        public ICollection<Enrollment> Enrollments { get; set; }



    }
}
