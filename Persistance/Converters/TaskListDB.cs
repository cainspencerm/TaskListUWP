using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistance.Converters
{
    [Table("TaskList")]
    public class TaskListDB
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public TaskListDB()
        {
            Id = 0;
            Name = "";
            Description = "";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
