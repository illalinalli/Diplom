using HomelessAnimalsDiplom.Models;
using Microsoft.AspNet.Identity;
using System.Numerics;
using static HomelessAnimalsDiplom.Controllers.HomeController;
using NumSharp;
using MongoDB.Bson;
using Accord.MachineLearning;
using Accord.MachineLearning.Clustering;
using Accord.Math;
using BaseLibS.Num.Cluster;

namespace HomelessAnimalsDiplom.Models
{
    class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; }

        public Node(string name)
        {
            Name = name;
            Children = new List<Node>();
        }
    }
    public class ContentBasedFiltering
    {
        public List<Item> items;
        public List<User> users;
        public List<Breed> breeds;
        public List<PropertyValue> properties_values;

        public List<Item> Recommend()
        {
            // Получаем предпочтения текущего пользователя.
            var currentFavorites = this.items.Where(item => CurUser.Favorites.Contains(item.Id));

            var favoriteItems = currentFavorites.Select(favorite =>
                items.Find(item => item.Id == favorite.Id)).ToList();

            // Исключаем избранные публикации текущего пользователя.
            items = items.Where(item =>
                    currentFavorites.All(favorite => favorite.Id != item.Id))
                .ToList();

            // Создаём матрицу схожести публикаций (по породам) (строки - избранное
            // текущего пользователя, столбцы - все публикации, исключая избранное).
            var similarityMatrix =
                new double[favoriteItems.Count, items.Count];
            for (var i = 0; i < similarityMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < similarityMatrix.GetLength(1); j++)
                {
                    var pairs = BuildValuePairs(favoriteItems[i], items[j]);

                    var euclideanDistance = SimilarityMeasureCalculator.CalcEuclideanDistance(pairs); // для построения дерева близостей
                         
                    double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(
                            favoriteItems[i].Parents, items[j].Parents);

                    // Используем евклидово расстояние (составляет 70% в итоговом значении)
                    // и близость по дереву (30%) для получения итогового сходства пород.
                    similarityMatrix[i, j] =
                        (1 - euclideanDistance) * 0.7 + (1 - treeProximity / ItemHelper.MAX_TREE_PROXIMITY) * 0.3;
                }
            }

            // Для каждой породы получаем общее сходство
            // со всеми избранными публикациями
            // (key - публикация, value - среднее арифметическое
            // всех значений схожести по столбцу).
            var itemsSimilarity = new Dictionary<ObjectId, double>();
            for (var j = 0; j < similarityMatrix.GetLength(1); j++)
            {
                double similarity = 0;
                for (var i = 0; i < similarityMatrix.GetLength(0); i++)
                {
                    similarity += similarityMatrix[i, j];
                }

                similarity /= similarityMatrix.GetLength(0);

                itemsSimilarity.Add(items[j].Id, similarity);
            }

            // Отбираем те породы, сходство которых больше 60%, и сортируем их по убыванию.
            var recommendedIds = itemsSimilarity
                .Where(itemSimilarity => itemSimilarity.Value > 0.6)
                .OrderByDescending(itemSimilarity =>
                    itemSimilarity.Value)
                .Select(itemSimilarity => itemSimilarity.Key);
            List<Item> res = new();
            foreach (var recId in recommendedIds)
            {
                var item = items.FirstOrDefault(x => x.Id == recId);
                if (item == null) continue;
                res.Add(item);
            }
            
            return res;
        }

        private ValuePair[] BuildValuePairs(Item item1, Item item2)
        {
            List<int> numBreedItem1 = new();
            List<int> numBreedItem2 = new();

            numBreedItem1.Add(item1.GetBreedNumber());
            numBreedItem2.Add(item2.GetBreedNumber());

            return new[]
            {
                new ValuePair(1,
                    ItemHelper.GetBreedsSimilarity(numBreedItem1.ToArray(), numBreedItem2.ToArray()), 
                    ItemHelper.MAX_BREED_DIFFERENCE)
            };
        }

    }
}
