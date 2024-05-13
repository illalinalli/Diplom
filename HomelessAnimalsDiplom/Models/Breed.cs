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

        public Breed()
        {
            Id = ObjectId.GenerateNewId();
        }

        [BsonIgnore]
        public static Dictionary<ObjectId, int> SizesNums { get; set; } = new();

        [BsonIgnore]
        public static Dictionary<ObjectId, int> BreedsNums { get; set; } = new();

        // Переопределяем метод Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Breed other = (Breed)obj;
            return Id.Equals(other.Id);
        }

        // Переопределяем метод GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    
        public static void SaveNewBreed(Breed newBreed)
        {
            if (newBreed.Name == null && newBreed.AnimalTypeRef == ObjectId.Empty
                && newBreed.SizeRef == ObjectId.Empty && newBreed.Id == ObjectId.Empty) return;
            // создание фильтра для поиска существующей записи
            var filter = Builders<Breed>.Filter.Eq("_id", newBreed.Id);

            // выполнение операции upsert
            BreedCollection?.ReplaceOneAsync(filter, newBreed, ReplaceOptionsUpsert);
            //return newBreed;
        }

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
