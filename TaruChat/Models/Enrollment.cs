using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public class Enrollment
    {
        public string ChatID { get; set; }
        public string UserID { get; set; }
        public Chat Chat { get; set; }
        public User User { get; set; }


    }
}
