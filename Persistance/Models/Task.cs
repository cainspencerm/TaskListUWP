using Persistance.DTOs;
using System;

namespace Persistance.Models
{
    public class Task : Item
    {
        public DateTime Deadline { get; set; }
        public bool IsComplete { get; set; }

        public Task() : base()
        {
            Deadline = new DateTime(1970, 1, 1);
            IsComplete = false;
        }

        public override ItemDTO DTO()
        {
            var dto = new TaskDTO
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Priority = Priority,
                Deadline = Deadline,
                IsComplete = IsComplete
            };

            return dto;
        }
    }
}