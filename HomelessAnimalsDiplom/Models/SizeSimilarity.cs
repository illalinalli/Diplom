using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;
using static HomelessAnimalsDiplom.Models.Database;
using MongoDB.Driver;

namespace HomelessAnimalsDiplom.Models
{
    public class SizeSimilarity
    {
        public ObjectId Id { get; set; }
        public PropertyValue Size { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<PropertyValue, double> SimilarityValues { get; set; } = new();
        public SizeSimilarity()
        {
            Id = ObjectId.GenerateNewId();
        }
        public static List<SizeSimilarity> GetAllSizesSimilarity()
        {
            return SizeSimilarityCollection.Find(new BsonDocument()).ToList();
        }
        public static List<SizeSimilarity> FillSizeMatrix()
        {
            var allSizesSimilarity = GetAllSizesSimilarity();
            List<PropertyValue> sizes = PropertyValue.GetAllSizes();
            List<SizeSimilarity> res = new();

            // Находим новые породы, для которых не существует записей схожести
            var newSizes = sizes.Where(c => allSizesSimilarity.All(cs => cs.Size.Id != c.Id)).ToList();
            if (newSizes != null || newSizes.Count() > 0)
            {
                // Добавляем новые породы в матрицу
                foreach (var newColor in newSizes)
                {
                    SizeSimilarity newSizeSimilarity = new SizeSimilarity
                    {
                        Size = newColor
                    };
                    allSizesSimilarity.Add(newSizeSimilarity);

                    // Добавляем значения 0 для всех имеющихся пород в SimilarityValues
                    foreach (var cs in allSizesSimilarity)
                    {
                        if (cs.Size != newColor)
                        {
                            cs.SimilarityValues.TryAdd(newColor, 0);

                        }
                        else
                        {
                            if (cs.SimilarityValues.Count() == 0)
                            {
                                sizes.ForEach(b => cs.SimilarityValues.Add(b, 0));
                            }
                        }
                    }
                }
                //
            }
            // Добавляем все записи схожести в результат
            res.AddRange(allSizesSimilarity);


            return res;
        }
    }
}
