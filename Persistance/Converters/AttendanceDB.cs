using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistance.Converters
{
    [Table("Attendance")]
    public class AttendanceDB
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int AttendeeId { get; set; }

        public AttendanceDB()
        {
            Id = 0;
            ItemId = 0;
            AttendeeId = 0;
        }
    }
}
