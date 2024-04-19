using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Xml.Linq;
using static HomelessAnimalsDiplom.Models.Breed;

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
        public string[]? Parents { get; set; }

        public int GetBreedNumber()
        {
            return BreedsNums[BreedRef];
        }

        public Item()
        {
            //Id = ObjectId.GenerateNewId();
            
            Parents = new string[0];
        }

    }
}
