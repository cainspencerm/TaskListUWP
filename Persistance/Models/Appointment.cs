using Persistance.DTOs;
using Persistance.Converters;
using System;
using System.Collections.Generic;

namespace Persistance.Models
{
    public class Appointment : Item
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<AttendeeDB> Attendees { get; set; }

        public Appointment() : base()
        {
            Start = DateTime.Now;
            Stop = DateTime.Now.AddHours(1);
            Attendees = new List<AttendeeDB>();
        }

        public override ItemDTO DTO()
        {
            var dto = new AppointmentDTO
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Priority = Priority,
                Start = Start,
                Stop = Stop,
                Attendees = Attendees
            };

            return dto;
        }
    }
}
