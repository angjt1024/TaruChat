using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public class Chat
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string Title { get; set; }

        [Display(Name = "Create At")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Updated At")]
        [DataType(DataType.Date)]
        public DateTime UpdatedAt { get; set; }
        [Display(Name = "Deleted At")]
        [DataType(DataType.Date)]
        public DateTime DeletedAt { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePic { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }

    }
}
