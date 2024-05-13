using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson;
using static HomelessAnimalsDiplom.Models.Database;
using MongoDB.Driver;

namespace HomelessAnimalsDiplom.Models
{
    public class ColorSimilarity
    {
        public ObjectId Id { get; set; }
        public PropertyValue Color { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<PropertyValue, double> SimilarityValues { get; set; } = new();

        public ColorSimilarity()
        {
            Id = ObjectId.GenerateNewId();
        }
        public static List<ColorSimilarity> GetAllColorsSimilarity()
        {
            return ColorSimilarityCollection.Find(new BsonDocument()).ToList();
        }
        public static List<ColorSimilarity> FillColorMatrix()
        {
            var allColorsSimilarity = GetAllColorsSimilarity();
            List<PropertyValue> colors = Item.GetAllColors();
            List<ColorSimilarity> res = new();

            // Находим новые породы, для которых не существует записей схожести
            var newColors = colors.Where(c => allColorsSimilarity.All(cs => cs.Color.Id != c.Id)).ToList();
            if (newColors != null || newColors.Count() > 0)
            {
                // Добавляем новые породы в матрицу
                foreach (var newColor in newColors)
                {
                    ColorSimilarity newColorSimilarity = new ColorSimilarity
                    {
                        Color = newColor
                    };
                    allColorsSimilarity.Add(newColorSimilarity);

                    // Добавляем значения 0 для всех имеющихся пород в SimilarityValues
                    foreach (var cs in allColorsSimilarity)
                    {
                        if (cs.Color != newColor)
                        {
                            cs.SimilarityValues.TryAdd(newColor, 0);

                        }
                        else
                        {
                            if (cs.SimilarityValues.Count() == 0)
                            {
                                colors.ForEach(b => cs.SimilarityValues.Add(b, 0));
                            }
                        }
                    }
                }
                //
            }
            // Добавляем все записи схожести в результат
            res.AddRange(allColorsSimilarity);
            return res;
        }

        public static double[,] ConvertToDoubleMatrix(List<ColorSimilarity> colorSimilarities)
        {
            List<PropertyValue> colors = Item.GetAllColors();
            double[,] result = new double[colors.Count, colors.Count];

            // Заполняем массив нулями
            for (int i = 0; i < colors.Count; i++)
            {
                for (int j = 0; j < colors.Count; j++)
                {
                    result[i, j] = 0.0;
                }
            }

            // Заполняем массив значениями из SimilarityValues
            foreach (var colorSimilarity in colorSimilarities)
            {
                int row = colors.FindIndex(c => c.Id == colorSimilarity.Color.Id);

                foreach (var similarityValue in colorSimilarity.SimilarityValues)
                {
                    int col = colors.FindIndex(c => c.Id == similarityValue.Key.Id);
                    result[row, col] = similarityValue.Value;
                }
            }

            return result;
        }
    }
}
