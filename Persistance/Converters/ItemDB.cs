using Persistance.DTOs;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistance.Converters
{
    [Table("Item")]
    public class ItemDB
    {
        // Item
        [Key]
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        
        // Appointment
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }

        // Task
        public DateTime Deadline { get; set; }
        public int IsComplete { get; set; }

        public string Discriminator { get; set; }
        
        public ItemDB()
        {
        }

        public ItemDB(ItemDTO dto)
        {
            Id = dto.Id;
            ListId = dto.ListId;
            Name = dto.Name;
            Description = dto.Description;
            Priority = dto.Priority;

            if (dto is TaskDTO)
            {
                Deadline = (dto as TaskDTO).Deadline;
                IsComplete = (dto as TaskDTO).IsComplete ? 1 : 0;
            } 
            else if (dto is AppointmentDTO)
            {
                Start = (dto as AppointmentDTO).Start;
                Stop = (dto as AppointmentDTO).Stop;
            }
        }

        public ItemDTO DTO()
        {
            ItemDTO dto = null;

            if (Discriminator == "Task")
            {
                dto = new TaskDTO()
                {
                    Id = Id,
                    ListId = ListId,
                    Name = Name,
                    Description = Description,
                    Priority = Priority,
                    Deadline = Deadline,
                    IsComplete = IsComplete == 1
                };
            } else if (Discriminator == "Appointment")
            {
                dto = new AppointmentDTO()
                {
                    Id = Id,
                    ListId = ListId,
                    Name = Name,
                    Description = Description,
                    Priority = Priority,
                    Start = Start,
                    Stop = Stop
                };
            }

            return dto;
        }
    }
}
