
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using static HomelessAnimalsDiplom.Models.Database;
namespace HomelessAnimalsDiplom.Models
{
    [Serializable]
    public class Similarity
    {
        public ObjectId Id { get; set; }
        // Добавляем атрибут для указания на то, что ключи словаря будут сериализоваться как ObjectId
        public Dictionary<ObjectId, Dictionary<ObjectId, double>> BreedIdSimilarity { get; set; } = new();
       
        [BsonIgnore]
        public Dictionary<Breed, Dictionary<Breed, double>> BreedSimilarity { get; set; } = new();
        [BsonIgnore]
        public Dictionary<string, Dictionary<string, double>> BreedStringSimilarity { get; set; } = new();
        //[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        //public Dictionary<Breed, Dictionary<Breed, double>> BreedSimilarity { get; set; } = new();


        //[BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        //public Dictionary<Item, Dictionary<Item, double>> ColorSimilarity { get; set; } = new();

        public Similarity()
        {
            Id = ObjectId.GenerateNewId();

        }
      
        public void Fill()
        {
            List<Breed> breeds = Item.GetAllBreeds();
            foreach (var breed1 in breeds)
            {
                BreedIdSimilarity[breed1.Id] = new Dictionary<ObjectId, double>();

                foreach (var breed2 in breeds)
                {
                    BreedIdSimilarity[breed1.Id][breed2.Id] = 0.0;
                }
            }
        }

        public void ConvertObjectIdsToString()
        {
            Dictionary<string, Dictionary<string, double>> newBreedIdSimilarity = new();

            foreach (var outerKey in BreedIdSimilarity.Keys)
            {
                var innerDictionary = BreedIdSimilarity[outerKey];
                Dictionary<string, double> newInnerDictionary = innerDictionary.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
                newBreedIdSimilarity[outerKey.ToString()] = newInnerDictionary;
            }

            BreedStringSimilarity = newBreedIdSimilarity;
        }
    }
}
