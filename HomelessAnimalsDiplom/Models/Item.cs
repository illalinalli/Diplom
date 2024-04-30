using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;
using static HomelessAnimalsDiplom.Models.Breed;
using static HomelessAnimalsDiplom.Models.Database;

namespace HomelessAnimalsDiplom.Models
{
    public class Item
    {
        public ObjectId Id { get; set; }
        public string? Title { get; set; }
        public bool IsPublished { get; set; }
        public List<byte[]>? Images { get; set; }
        public ObjectId BreedRef { get; set; }

        public List<ObjectId>? Properties { get; set; } = new();
        public List<PropertyValue> Colors { get; set; } = new();
        public PropertyValue Sex { get; set; }
        public string? LongDescription { get; set; }
        public ObjectId UserRef { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        //public PropertyValue Size { get; set; } // крупный, мелкий, средний
        public string[]? Parents { get; set; }

        [BsonIgnore]
        public static Dictionary<ObjectId, int> ColorsNums { get; set; } = new();

        public int GetBreedNumber()
        {
            return BreedsNums[BreedRef];
        }

        public PropertyValue GetBreedSize()
        {
            var allBreeds = GetAllBreeds();
            var allProps = GetAllPropValues();
            var curBreed = allBreeds.FirstOrDefault(x => x.Id == BreedRef);
            var size = allProps.FirstOrDefault(x => x.Id == curBreed.SizeRef);
            if (size == null) return null;
            return size;
        }
        List<Breed> GetAllBreeds()
        {
            return BreedCollection.Find(new BsonDocument()).ToList();
        }
        public static List<PropertyValue> GetAllPropValues()
        {
            return PropertyValueCollection.Find(new BsonDocument()).ToList();
        }
        public static List<PropertyValue> GetColors()
        {
            return PropertyValueCollection.Find(new BsonDocument()).ToList().FindAll(x => x.PropTypeRef == ObjectId.Parse("658ab0a38a4dcfd166a80346"));
        }
        public static void SetColorsNum()
        {
            var allPropertyValues = GetColors();
            int num = 0;
            foreach (var pt in allPropertyValues)
            {
                ColorsNums.TryAdd(pt.Id, num);
                num++;
            }
        }

      
        public Item()
        {
            //Id = ObjectId.GenerateNewId();
            
            Parents = new string[0];
        }

    }
}
