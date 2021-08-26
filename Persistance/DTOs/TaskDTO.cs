using Persistance.Models;
using Newtonsoft.Json;
using System;

namespace Persistance.DTOs
{
    [JsonConverter(typeof(ProductJsonConverter))]
    public class TaskDTO : ItemDTO
    {
        public DateTime Deadline { get; set; }
        public bool IsComplete { get; set; }

        public TaskDTO() : base()
        {
            Deadline = DateTime.Now;
            IsComplete = false;
        }

        public override Item Item()
        {
            var task = new Task
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Priority = Priority,
                Deadline = Deadline,
                IsComplete = IsComplete
            };

            return task;
        }
    }
}
