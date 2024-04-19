using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;

namespace HomelessAnimalsDiplom.Models
{
    public class Item
    {
        public ObjectId Id { get; set; }
        public string? Title { get; set; }
        public bool IsPublished { get; set; }
        public List<byte[]>? Images { get; set; }
        public ObjectId BreedRef { get; set; }

        public List<ObjectId>? Properties { get; set; }
        public string? LongDescription { get; set; }
        public ObjectId UserRef { get; set; }
        public DateTimeOffset? CreationDate { get; set; }

        
        [BsonIgnore]
        public string[] Parents { get; set; }

        protected Item()
        {
            //Id = ObjectId.GenerateNewId();
            Title = "";
            Parents = new string[0];
        }

    }
}
