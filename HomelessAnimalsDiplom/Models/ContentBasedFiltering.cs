using HomelessAnimalsDiplom.Models;
using Microsoft.AspNet.Identity;
using System.Numerics;
using static HomelessAnimalsDiplom.Controllers.HomeController;
using NumSharp;
using MongoDB.Bson;

namespace HomelessAnimalsDiplom.Models
{
    public class ContentBasedFiltering
    {
        public List<Item> items;
        public List<User> users;
        public List<Breed> breeds;
        public List<PropertyValue> properties_values;

        public List<ObjectId> Recommend()
        {
            // Получаем предпочтения текущего пользователя.
            var currentFavorites = this.items.Where(item => CurUser.Favorites.Contains(item.Id)); ;

            //var items = ApplicationContext.GetInstance().Arts;
            var favoriteItems = currentFavorites.Select(favorite =>
                items.Find(art => art.Id == favorite.Id)).ToList();

            // Исключаем избранные виды искусства текущего пользователя.
            items = items.Where(art =>
                    currentFavorites.All(favorite => favorite.Id != art.Id))
                .ToList();

            // Создаём матрицу схожести видов искусства (строки - избранное
            // текущего пользователя, столбцы - все виды искусства, исключая избранное).
            var similarityMatrix =
                new double[favoriteItems.Count, items.Count];
            for (var i = 0; i < similarityMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < similarityMatrix.GetLength(1); j++)
                {
                    var pairs = BuildValuePairs(favoriteItems[i], items[j]);

                    var euclideanDistance = SimilarityMeasureCalculator
                        .CalcEuclideanDistance(pairs);
                    double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(
                            favoriteItems[i].Parents, items[j].Parents);

                    // Используем евклидово расстояние (составляет 70% в итоговом значении)
                    // и близость по дереву (30%) для получения итогового сходства видов искусства.
                    similarityMatrix[i, j] =
                        (1 - euclideanDistance) * 0.7 +
                        (1 - treeProximity / ItemHelper.MAX_TREE_PROXIMITY) * 0.3;
                }
            }

            // Для каждого вида искусства получаем общее сходство
            // со всеми избранными видами искусства
            // (key - id вида искусства, value - среднее арифметическое
            // всех значений схожести по столбцу).
            var artsSimilarity = new Dictionary<ObjectId, double>();
            for (var j = 0; j < similarityMatrix.GetLength(1); j++)
            {
                double similarity = 0;
                for (var i = 0; i < similarityMatrix.GetLength(0); i++)
                {
                    similarity += similarityMatrix[i, j];
                }

                similarity /= similarityMatrix.GetLength(0);

                artsSimilarity.Add(items[j].Id, similarity);
            }

            // Отбираем те виды искусства, сходство которых больше 50%, и сортируем их по убыванию.
            var recommendedArtIds = artsSimilarity
                .Where(artSimilarity => artSimilarity.Value > 0.5)
                .OrderByDescending(artSimilarity =>
                    artSimilarity.Value)
                .Select(artSimilarity => artSimilarity.Key);

            return recommendedArtIds.ToList();
        }

        private ValuePair[] BuildValuePairs(Item item1, Item item2)
        {
            return new[]
            {
                    new ValuePair(
                        ItemHelper.GetPeriodNumber(item1.Date),
                        ItemHelper.GetPeriodNumber(item2.Date),
                        ItemHelper.MAX_PERIOD_DIFFERENCE),

                    new ValuePair(item1.MuseumNumber, item2.MuseumNumber,
                        ItemHelper.MAX_MUSEUM_NUMBER_DIFFERENCE),

                    new ValuePair(item1.AreMasterClassesHeld ? 1 : 0,
                        item2.AreMasterClassesHeld ? 1 : 0,
                        ItemHelper.MAX_MASTER_CLASSES_DIFFERENCE),

                    new ValuePair((double) item1.Popularity,
                        (double) item2.Popularity,
                        ItemHelper.MAX_POPULARITY_DIFFERENCE),

                    new ValuePair(1,
                        ItemHelper.GetGenresSimilarity(item1.Genres.ToArray(),
                            item2.Genres.ToArray()),
                        ItemHelper.MAX_BREED_DIFFERENCE)
            };
        }

        static List<List<double>> PadMatrix(List<List<double>> matrix, int targetLength)
        {
            List<List<double>> paddedMatrix = new List<List<double>>();

            foreach (var vector in matrix)
            {
                paddedMatrix.Add(PadVector(vector, targetLength));
            }

            return paddedMatrix;
        }

        static List<double> PadVector(List<double> vector, int targetLength)
        {
            List<double> paddedVector = new List<double>(vector);
            paddedVector.AddRange(Enumerable.Repeat(0.0, targetLength - vector.Count));
            return paddedVector;
        }
        static List<double> TrimVector(List<double> vector, int targetLength)
        {
            if (vector.Count > targetLength)
            {
                return vector.Take(targetLength).ToList();
            }

            return vector;
        }
        static double[] linear_kernel(List<List<double>> tfidfMatrixUser, List<List<double>> tfidfMatrixItems)
        {
            int maxLength = Math.Max(tfidfMatrixUser.Count, tfidfMatrixItems.Count);
            int maxVectorLength = Math.Max(tfidfMatrixUser.Count, tfidfMatrixItems.Select(v => v.Count).DefaultIfEmpty(0).Max());

            // Дополнение векторов нулями до максимальной длины
            List<List<double>> paddedUserMatrix = PadMatrix(tfidfMatrixUser, maxVectorLength);
            List<List<double>> paddedItemMatrix = tfidfMatrixItems.Select(v => PadVector(v, maxVectorLength)).ToList();

            double[] similarities = new double[maxLength]; // paddedItemMatrix.Count

            for (int i = 0; i < maxLength; i++)
            {
                similarities[i] = CosineSimilarity(paddedUserMatrix[i], paddedItemMatrix[i]);
            }

            return similarities;
        }

        static double CosineSimilarity(List<double> vector1, List<double> vector2)
        {
            //double dotProduct = DotProduct(vector1, vector2);
            //double magnitude1 = Magnitude(vector1);
            //double magnitude2 = Magnitude(vector2);

            //if (magnitude1 == 0 || magnitude2 == 0)
            //    return 0;
            //var res = (double)dotProduct / (magnitude1 * magnitude2);
            //return (double)dotProduct / (magnitude1 * magnitude2);

            int maxLength = Math.Max(vector1.Count, vector2.Count);

            double dotProduct = 0;
            double magnitude1 = 0;
            double magnitude2 = 0;

            for (int i = 0; i < maxLength; i++)
            {
                double value1 = i < vector1.Count ? vector1[i] : 0;
                double value2 = i < vector2.Count ? vector2[i] : 0;

                dotProduct += value1 * value2;
                magnitude1 += value1 * value1;
                magnitude2 += value2 * value2;
            }

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
        }

        static double DotProduct(List<double> vector1, List<double> vector2)
        {
            double product = 0;

            for (int i = 0; i < vector1.Count; i++)
            {
                product += vector1[i] * vector2[i];
            }

            return product;
        }

        static double Magnitude(List<double> vector)
        {
            double sumOfSquares = 0;

            foreach (var value in vector)
            {
                sumOfSquares += value * value;
            }

            return Math.Sqrt(sumOfSquares);
        }

        //public void GetRecommendations()
        //{
        //    // Предобработка данных
        //    List<string> itemsFeatures = new List<string>();
        //    foreach (var item in items)
        //    {
        //        string features = string.Join(" ", breeds.Where(breed => breed.Id == item.BreedRef).Select(breed => breed.Name));
        //        List<string> properties = properties_values.Where(prop_value => item.Properties.Contains(prop_value.Id)).Select(prop_value => prop_value.Name).ToList();
        //        features += " " + string.Join(" ", properties);
        //        itemsFeatures.Add(features);
        //    }

        //    List<string> usersFeatures = new List<string>();
        //    //foreach (var user in users)
        //    //{
        //    //    if (user.ViewingHistory == null) user.ViewingHistory = new();
        //    //    var userProperties = items.Where(item => user.Favorites.Contains(item.Id));
        //    //    var userViewingHistory = items.Where(item => user.ViewingHistory.Any(historyItem => historyItem.ItemId == item.Id));
        //    //    string features = string.Join(" ", userProperties.Concat(userViewingHistory).Select(item => item.Title));
        //    //    usersFeatures.Add(features);
        //    //}

        //    if (CurUser.ViewingHistory == null) CurUser.ViewingHistory = new();
        //    var userFavs = items.Where(item => CurUser.Favorites.Contains(item.Id));
        //    var userNotViewingItems = items.Where(item => !userFavs.Contains(item) && item.UserRef != CurUser.Id); //  CurUser.ViewingHistory.Any(historyItem => historyItem.ItemId == item.Id &&
        //    //string resProp = string.Empty;
        //    foreach (var item in userNotViewingItems)
        //    {
        //        string resProp = string.Join(" ", breeds.Where(breed => breed.Id == item.BreedRef).Select(breed => breed.Name));
        //        List<string> properties = properties_values.Where(prop_value => item.Properties.Contains(prop_value.Id)).Select(prop_value => prop_value.Name).ToList();
        //        resProp += " " + string.Join(" ", properties);
        //        usersFeatures.Add(resProp);
        //    }
        //    //string featuresUser = string.Join(" ", userFavs.Concat(userNotViewingItems).Select(item => item.Title));
        //    //string featuresUser = string.Join(" ", userFavs.Select(item => item.Title));

        //    // Вычисление косинусного сходства
        //    TfidfVectorizer vectorizer = new TfidfVectorizer();
        //    var tfidfMatrixItems = vectorizer.FitTransform(itemsFeatures);

        //    var tfidfMatrixUsers = vectorizer.Transform(usersFeatures);

        //    for (int i = 0; i < users.Count; i++)
        //    {
        //        var cosineSimilarities = linear_kernel(tfidfMatrixUsers, tfidfMatrixItems).ToList(); // .Flatten()

        //        // Получаем индексы наиболее похожих публикаций
        //        var similarItemsIndices = cosineSimilarities.Select((value, index) => new { Value = value, Index = index }).OrderByDescending(pair => pair.Value).Select(pair => pair.Index).ToArray();

        //        // Рекомендации для пользователя
        //        Console.WriteLine($"Рекомендации для пользователя {users[i].Name}:");
        //        foreach (var idx in similarItemsIndices)
        //        {
        //            Console.WriteLine(items[idx].Title);
        //        }
        //    }
        //}

    }
}
