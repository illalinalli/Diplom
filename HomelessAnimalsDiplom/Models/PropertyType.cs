using MongoDB.Bson;

namespace HomelessAnimalsDiplom.Models
{
    public class PropertyType
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; } // окрас, пол
    }
}
