using Amazon.Runtime.Internal.Transform;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using NuGet.Packaging;
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

        public static List<BreedSimilarity> Fill()
        {
            var allBreedsSimilarity = GetAllBreedsSimilarity();
            List<Breed> breeds = Item.GetAllBreeds();
            List<BreedSimilarity> res = new();

            if (breeds.Count != allBreedsSimilarity.Count)
            {
                // Находим новые породы, для которых не существует записей схожести
                var newBreeds = breeds.Where(b => allBreedsSimilarity.All(bs => bs.AnimalBreed.Id != b.Id)).ToList();

                // Добавляем новые породы в матрицу
                foreach (var newBreed in newBreeds)
                {
                    BreedSimilarity newBreedSimilarity = new BreedSimilarity
                    {
                        AnimalBreed = newBreed
                    };
                    allBreedsSimilarity.Add(newBreedSimilarity);

                    // Добавляем значения 0 для всех имеющихся пород в SimilarityValues
                    foreach (var bs in allBreedsSimilarity)
                    {
                        if (bs.AnimalBreed != newBreed)
                        {
                            bs.SimilarityValues.Add(newBreed, 0);

                        }
                        else
                        {
                            if (bs.SimilarityValues.Count() == 0)
                            {
                                breeds.ForEach(b => bs.SimilarityValues.Add(b, 0));
                            }
                        }
                    }
                }
            }

            // Добавляем все записи схожести в результат
            res.AddRange(allBreedsSimilarity);

            return res;
        }
        public BreedSimilarity()
        {
            Id = ObjectId.GenerateNewId();
        }

        public static double[,] ConvertToDoubleMatrix(List<BreedSimilarity> breedSimilarities, List<Breed> allBreeds)
        {
            var doubleMatrix = new double[allBreeds.Count, allBreeds.Count];

            // Initialize the matrix with default values
            for (int i = 0; i < allBreeds.Count; i++)
            {
                for (int j = 0; j < allBreeds.Count; j++)
                {
                    doubleMatrix[i, j] = 0.0;
                }
            }

            // Fill the matrix with similarity values
            foreach (var breedSimilarity in breedSimilarities)
            {
                int row = allBreeds.IndexOf(breedSimilarity.AnimalBreed);

                if (row != -1)
                {
                    foreach (var similarityEntry in breedSimilarity.SimilarityValues)
                    {
                        int column = allBreeds.IndexOf(similarityEntry.Key);
                        if (column != -1)
                        {
                            doubleMatrix[row, column] = similarityEntry.Value;
                        }
                    }
                }
            }

            return doubleMatrix;
        }
    }
}
