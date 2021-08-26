using Newtonsoft.Json.Linq;
using System;
using Persistance.DTOs;

namespace Persistance
{
    public class ProductJsonConverter : JsonCreationConverter<ItemDTO>
    {
        protected override ItemDTO Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException(nameof(jObject));

            if (jObject["deadline"] != null || jObject["Deadline"] != null)
            {
                return new TaskDTO();
            }
            else if (jObject["attendees"] != null || jObject["Attendees"] != null)
            {
                return new AppointmentDTO();
            }
            else
            {
                return null;
            }
        }
    }
}
