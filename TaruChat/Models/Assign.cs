
namespace TaruChat.Models
{
    public class Assign
    {
        public string ClassID { get; set; }
        public string SubjectID { get; set; }

        public Class Class { get; set; }
        public Subject Subject { get; set; }

    }
}
