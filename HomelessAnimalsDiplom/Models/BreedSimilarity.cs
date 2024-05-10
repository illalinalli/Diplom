using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using static HomelessAnimalsDiplom.Models.Database;

namespace HomelessAnimalsDiplom.Models
{
    public class BreedSimilarity
    {
        public ObjectId Id { get; set; }
        public Breed AnimalBreed { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<Breed, double> SimilarityValues { get; set; } = new();

        public static List<BreedSimilarity> GetAllBreedsSimilarity()
        {
            return BreedSimilarityCollection.Find(new BsonDocument()).ToList();
        }
        //public void Fill()
        //{
        //    var allBreedsSimilarity = GetAllBreedsSimilarity();
        //    List<Breed> breeds = Item.GetAllBreeds();

        //    foreach (var breed in breeds)
        //    {
        //        var existingSimilarity = allBreedsSimilarity.FirstOrDefault(bs => bs.AnimalBreed.Id == breed.Id);
        //        var res = existingSimilarity.SimilarityValues[breed];
        //        SimilarityValues[breed] = existingSimilarity != null ? existingSimilarity.SimilarityValues[breed] : 0;
        //    }
        //}
        public static List<BreedSimilarity> Fill()
        {
            var allBreedsSimilarity = GetAllBreedsSimilarity();
            List<Breed> breeds = Item.GetAllBreeds();
            List<BreedSimilarity> res = new();
            foreach (var breed in breeds)
            {
                var existingSimilarity = allBreedsSimilarity.FirstOrDefault(bs => bs.AnimalBreed.Id == breed.Id);
                if (existingSimilarity != null)
                {
                    res.Add(existingSimilarity); 
                    //if (existingSimilarity.SimilarityValues.TryGetValue(breed, out double similarity))
                    //{
                    //    // Значение similarity теперь доступно, если ключ существует
                    //    SimilarityValues[breed] = similarity;
                    //}
                    //else
                    //{
                    //    // Ключ не найден в словаре, установите значение по умолчанию или обработайте этот случай
                    //    existingSimilarity.SimilarityValues[breed] = 0;
                    //}
                }
                else
                {
                    BreedSimilarity bs = new()
                    {
                        AnimalBreed = breed,
                        SimilarityValues = new()
                    };

                    // Обработка случая, когда нет соответствующего элемента BreedSimilarity для данного breed
                    // Например, можно создать новый экземпляр BreedSimilarity и добавить его в список
                }
            }
            return res;
        }
        public BreedSimilarity()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
