using MongoDB.Bson;

namespace HomelessAnimalsDiplom.Models
{
    public class PropertyValue
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public ObjectId PropTypeRef { get; set; }
    }
}
