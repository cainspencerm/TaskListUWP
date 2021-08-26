using Persistance.DTOs;
using System;

namespace Persistance.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private int _Priority;
        public int Priority
        {
            get => _Priority;
            set
            {
                if (value > 3)
                {
                    value = 0;
                }
                _Priority = value;
            }
        }

        public DateTime DateTime { get
            {
                return this is Task ? (this as Task).Deadline : (this as Appointment).Start;
            } 
        }

        public Item()
        {
            Id = 0;
            Name = "";
            Description = "";
            Priority = 0;
        }

        public virtual ItemDTO DTO()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}