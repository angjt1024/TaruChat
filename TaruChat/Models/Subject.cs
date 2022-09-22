using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaruChat.Models
{
    public class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string Title { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate {get; set;}
        [Display(Name = "End  Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public ICollection<Assign> Assigns { get; set; }
    }
}
