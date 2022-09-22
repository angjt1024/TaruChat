using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models.ChatViewModels
{
    public class UserDetailModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public char Gender { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePic { get; set; }
        public IFormFile Photo { get; set; }
        public string Img64base { get; set; }


    }
}
