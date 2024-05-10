using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using static HomelessAnimalsDiplom.Models.Database;
namespace HomelessAnimalsDiplom.Models
{
    
    public class Breed
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public ObjectId SizeRef { get; set; }
        //public List<Breed> SubBreeds { get; set; } = new();
        public ObjectId AnimalTypeRef { get; set; }

        [BsonIgnore]
        public static Dictionary<ObjectId, int> BreedsNums { get; set; } = new();

        public static List<Breed> GetAllBreeds()
        {
            return BreedCollection.Find(new BsonDocument()).ToList();
        }
        public static void SetBreedsNum()
        {
            var allBreeds = GetAllBreeds();
            int num = 0;
            foreach (var b in allBreeds)
            {
                BreedsNums.TryAdd(b.Id, num);
                num++;
            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
