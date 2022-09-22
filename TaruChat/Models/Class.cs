using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public class Class
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string Title { get; set; }
        public ICollection<Assign> Assigns { get; set; }
    }
}
