using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistance.Converters
{
    [Table("Attendee")]
    public class AttendeeDB
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AttendeeDB()
        {
            Id = 0;
            Email = "";
            FirstName = "";
            LastName = "";
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
