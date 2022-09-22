using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string MessageType { get; set; }
        public string Word { get; set; }
        public string AttachmentURL { get; set; }
        public DateTime CreatedAt{ get; set; }
        public string ChatID { get; set; }
        public string UserID { get; set; }
        public Chat Chat { get; set; }

    }
}
