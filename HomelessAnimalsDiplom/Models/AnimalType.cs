using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace HomelessAnimalsDiplom.Models
{
    public class AnimalType
    {
        public ObjectId Id { get; set; }
        [BsonIgnore]
        public string IdAsString { 
            get {
                return Id.ToString();
            }
            set
            {
                Id.ToString();
            }
        }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static List<AnimalType> GetAnimalTypes()
        {
            return Database.AnimalTypeCollection.Find(new BsonDocument()).ToList();
        }
    }
}
