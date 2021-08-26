using Persistance.Models;
using Newtonsoft.Json;

namespace Persistance.DTOs
{
    [JsonConverter(typeof(ProductJsonConverter))]
    public class ItemDTO
    {
        public int Id { get; set; }
        public int ListId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }

        public ItemDTO()
        {
            Id = 0;
            Name = null;
            Description = null;
            Priority = 0;
        }

        public virtual Item Item()
        {
            return null;
        }
    }
}
