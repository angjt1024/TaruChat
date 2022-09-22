using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models.ChatViewModels
{
    public class ChangePwVM
    {
        [Display(Name = "Current Password")]
        [Required]
        public string Old { get; set; }

        [Display(Name = "New Password")]
        [Required]
        public string New { get; set; }
        [Display(Name = "Confirm Password")]
        [Required]
        [Compare("New")]
        public string Confirm { get; set; }

    }
}
