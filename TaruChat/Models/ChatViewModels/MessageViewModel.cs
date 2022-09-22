using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models.ChatViewModels
{
    public class MessageViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public Message Message { get; set; }
        public User User { get; set; }
        public Chat Chat{ get; set; }

    }
}
