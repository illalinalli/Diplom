using HomelessAnimalsDiplom.Models;
using Microsoft.AspNet.Identity;

namespace HomelessAnimalsDiplom.Models
{
    public class ContentBasedRecomSystem
    {
        public List<Item> items;
        public List<User> users;
        public List<Breed> breeds;
        public List<PropertyValue> properties_values;
        static double[] linear_kernel(List<List<double>> tfidfMatrixUser, List<List<double>> tfidfMatrixItems)
        {
            double[] similarities = new double[tfidfMatrixItems.Count];

            for (int i = 0; i < tfidfMatrixItems.Count; i++)
            {
                similarities[i] = CosineSimilarity(tfidfMatrixUser[i], tfidfMatrixItems[i]);
            }

            return similarities;
        }

        static double CosineSimilarity(List<double> vector1, List<double> vector2)
        {
            double dotProduct = DotProduct(vector1, vector2);
            double magnitude1 = Magnitude(vector1);
            double magnitude2 = Magnitude(vector2);

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            return dotProduct / (magnitude1 * magnitude2);
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
        void GetRecommendations()
        {
            // Предобработка данных
            List<string> itemsFeatures = new List<string>();
            foreach (var item in items)
            {
                string features = string.Join(" ", breeds.Where(breed => breed.Id == item.BreedRef).Select(breed => breed.Name));
                List<string> properties = properties_values.Where(prop_value => item.Properties.Contains(prop_value.PropTypeRef)).Select(prop_value => prop_value.Name).ToList();
                features += " " + string.Join(" ", properties);
                itemsFeatures.Add(features);
            }

            List<string> usersFeatures = new List<string>();
            foreach (var user in users)
            {
                var userProperties = items.Where(item => user.Favorites.Contains(item.Id));
                var userViewingHistory = items.Where(item => user.ViewingHistory.Any(historyItem => historyItem.ItemId == item.Id));
                string features = string.Join(" ", userProperties.Concat(userViewingHistory).Select(item => item.Title));
                usersFeatures.Add(features);
            }

            // Вычисление косинусного сходства
            TfidfVectorizer vectorizer = new TfidfVectorizer();
            var tfidfMatrixItems = vectorizer.FitTransform(itemsFeatures);
            var tfidfMatrixUsers = vectorizer.Transform(usersFeatures);

            for (int i = 0; i < users.Count; i++)
            {
                var cosineSimilarities = linear_kernel(tfidfMatrixUsers, tfidfMatrixItems).ToList(); // .Flatten()

                // Получаем индексы наиболее похожих публикаций
                var similarItemsIndices = cosineSimilarities.Select((value, index) => new { Value = value, Index = index }).OrderByDescending(pair => pair.Value).Select(pair => pair.Index).ToArray();

                // Рекомендации для пользователя
                Console.WriteLine($"Рекомендации для пользователя {users[i].Name}:");
                foreach (var idx in similarItemsIndices)
                {
                    Console.WriteLine(items[idx].Title);
                }
            }
        }
       
    }
}
