using MongoDB.Bson;
using MongoDB.Driver;
using static HomelessAnimalsDiplom.Models.Database;

namespace HomelessAnimalsDiplom.Models
{
    public class CoefficientAdjuster
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public double CoefficientValue { get; set; }
        public bool IsTree { get; set; }
        public bool IsCommonResult { get; set; }
        public bool IsColor { get; set; }
        public bool IsSize { get; set; }
        public bool IsItemsSimilarityValue { get; set; }


        public static List<CoefficientAdjuster> GetAllCoefficients()
        {
            return CoefficientAdjusterCollection.Find(new BsonDocument()).ToList();
        }
        public void Save()
        {
            var filter = Builders<CoefficientAdjuster>.Filter.Eq("_id", Id);
            CoefficientAdjusterCollection?.ReplaceOneAsync(filter, this, ReplaceOptionsUpsert).Wait();
        }
    }
}
