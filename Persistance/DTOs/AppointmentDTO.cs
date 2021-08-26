using Persistance.Models;
using Persistance.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Persistance.DTOs
{
    [JsonConverter(typeof(ProductJsonConverter))]
    public class AppointmentDTO : ItemDTO
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public List<AttendeeDB> Attendees { get; set; }

        public AppointmentDTO() : base()
        {
            Start = DateTime.Now;
            Stop = DateTime.Now.AddHours(1);
            Attendees = new List<AttendeeDB>();
        }

        public override Item Item()
        {
            var appt = new Appointment
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Priority = Priority,
                Start = Start,
                Stop = Stop,
                Attendees = Attendees
            };

            return appt;
        }
    }
}
