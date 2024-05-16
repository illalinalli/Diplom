using static HomelessAnimalsDiplom.Models.Database;
using static HomelessAnimalsDiplom.Controllers.HomeController;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HomelessAnimalsDiplom.Models
{
    public class ContentBasedFiltering
    {
        public List<Item> items;
      
        //List<PropertyValue> GetAllProperties()
        //{
        //    return PropertyValueCollection.Find(new BsonDocument()).ToList();
        //}
        // Метод для получения родителей элемента из дерева
        private List<string> GetParentsFromNode(Node node, Item item)
        {
            // Найдем узел в дереве, соответствующий породе элемента
            Node breedNode = FindNodeInTree(node, item.GetBreed().Name, item);

            // Получим список родителей элемента из дерева
            List<string> parents = new List<string>();
            while (breedNode != null && breedNode.Parent != null)
            {
                parents.Add(breedNode.Parent.Name);
                breedNode = breedNode.Parent;
            }

            return parents;
        }

        // Метод для поиска узла в дереве по item
        private Node FindNodeInTree(Node node, string name, Item item)
        {
            if (node.Item != null && node.Item.Id == item.Id)
            {
                return node;
            }

            foreach (var child in node.Children)
            {
                var foundNode = FindNodeInTree(child, name, item);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        /// <summary>
        /// Только близость по дереву
        /// </summary>
        /// <returns></returns>
        public List<Item> TreeProximityRecommend()
        {
            var allCoefficients = CoefficientAdjuster.GetAllCoefficients();
            double itemsSimilarityCoeff = allCoefficients
                 .FirstOrDefault(x => x.IsItemsSimilarityValue
                 && !x.IsTree && !x.IsCommonResult && !x.IsSize && !x.IsColor).CoefficientValue;
            var tree = new TreeBuilder();
            // Получаем предпочтения текущего пользователя.
            var currentFavorites = items.Where(item => CurUser.Favorites.Contains(item.Id));

            var favoriteItems = currentFavorites.Select(favorite =>
                items.Find(item => item.Id == favorite.Id)).ToList();

            // Исключаем избранные публикации текущего пользователя.
            items = items.Where(item =>
                    currentFavorites.All(favorite => favorite.Id != item.Id))
                .ToList();
           var node = tree.BuildTree(ItemCollection.Find(new BsonDocument()).ToList());
            // Создаём матрицу схожести публикаций (по породам) (строки - избранное
            // текущего пользователя, столбцы - все публикации, исключая избранное).
            var similarityMatrix =
                new double[favoriteItems.Count, items.Count];
            for (var i = 0; i < similarityMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < similarityMatrix.GetLength(1); j++)
                {
                    var pairs = BuildValuePairs(favoriteItems[i], items[j]);
                    var parentsItem1 = GetParentsFromNode(node, favoriteItems[i]);
                    var parentsItem2 = GetParentsFromNode(node, items[j]);
                    double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(parentsItem1.ToArray(), parentsItem2.ToArray(), favoriteItems[i], items[j]);

                    // близость по дереву для получения итогового сходства пород.
                    similarityMatrix[i, j] = treeProximity; //    (1 - treeProximity / ItemHelper.MAX_TREE_PROXIMITY)
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

            // Отбираем те породы, сходство которых больше 40%, и сортируем их по убыванию.
            var recommendedIds = itemsSimilarity
                .Where(itemSimilarity => itemSimilarity.Value > itemsSimilarityCoeff)
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

        /// <summary>
        /// Только евклидово расстояние
        /// </summary>
        /// <returns></returns>
        public List<Item> EuclideanDistanceRecommend()
        {
            var allCoefficients = CoefficientAdjuster.GetAllCoefficients();
            double itemsSimilarityCoeff = allCoefficients
                .FirstOrDefault(x => x.IsItemsSimilarityValue 
                && !x.IsTree && !x.IsCommonResult && !x.IsSize && !x.IsColor).CoefficientValue;

            // Получаем предпочтения текущего пользователя.
            var currentFavorites = items.Where(item => CurUser.Favorites.Contains(item.Id));

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

                    //double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(
                            //favoriteItems[i].Parents, items[j].Parents);

                    // Используем евклидово расстояние (составляет 70% в итоговом значении)
                    // и близость по дереву (30%) для получения итогового сходства пород.
                    similarityMatrix[i, j] = 1 - euclideanDistance; // + (1 - treeProximity / ItemHelper.MAX_TREE_PROXIMITY) * 0.3
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

            // Отбираем те породы, сходство которых больше 40%, и сортируем их по убыванию.
            var recommendedIds = itemsSimilarity
                .Where(itemSimilarity => itemSimilarity.Value > itemsSimilarityCoeff)
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

        public List<Item> Recommend()
        {
            var tree = new TreeBuilder();
            // Подгружаем коэффициенты из базы.
            var allCoefficients = CoefficientAdjuster.GetAllCoefficients();
            double euclideanCoeff = allCoefficients.FirstOrDefault(x => x.IsCommonResult && !x.IsTree).CoefficientValue;
            double treeCoeff = allCoefficients.FirstOrDefault(x => x.IsCommonResult && x.IsTree).CoefficientValue;
            double itemsSimilarityCoeff = allCoefficients.FirstOrDefault(x => x.IsItemsSimilarityValue && !x.IsTree
            && !x.IsCommonResult && !x.IsSize && !x.IsColor).CoefficientValue;
            if (euclideanCoeff == null || treeCoeff == null) return null;
            // Получаем предпочтения текущего пользователя.
            var currentFavorites = items.Where(item => CurUser.Favorites.Contains(item.Id));

            var favoriteItems = currentFavorites.Select(favorite =>
                items.Find(item => item.Id == favorite.Id)).ToList();

            // Исключаем избранные публикации текущего пользователя.
            items = items.Where(item => currentFavorites.All(favorite => favorite.Id != item.Id) 
                    && item.UserRef != CurUser.Id).ToList();
            var node = tree.BuildTree(ItemCollection.Find(new BsonDocument()).ToList());
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

                    var parentsItem1 = GetParentsFromNode(node, favoriteItems[i]);
                    var parentsItem2 = GetParentsFromNode(node, items[j]);
                    double treeProximity = SimilarityMeasureCalculator.CalcTreeProximity(parentsItem1.ToArray(), parentsItem2.ToArray(), favoriteItems[i], items[j]);


                    // Используем евклидово расстояние (составляет 70% в итоговом значении)
                    // и близость по дереву (30%) для получения итогового сходства пород.
                    similarityMatrix[i, j] =
                        (1 - euclideanDistance) * euclideanCoeff + treeProximity * treeCoeff;
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

            // Отбираем те породы, сходство которых больше 40%, и сортируем их по убыванию.
            var recommendedIds = itemsSimilarity
                .Where(itemSimilarity => itemSimilarity.Value > itemsSimilarityCoeff)
                .OrderByDescending(itemSimilarity =>
                    itemSimilarity.Value)
                .Select(itemSimilarity => itemSimilarity.Key).Take(5);
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

            List<int> colorsNum1 = new();
            List<int> colorsNum2 = new();

            foreach (var c1 in item1.Colors)
            {
                colorsNum1.Add(c1.GetColorNumber());
            }
            
            foreach (var c2 in item2.Colors)
            {
                colorsNum2.Add(c2.GetColorNumber());
            }

            var a = item1.GetSizeNum(item1.GetBreed());
            var b = item2.GetSizeNum(item2.GetBreed());

            return new[]
            {
                new ValuePair(1,
                    ItemHelper.GetBreedsSimilarity(numBreedItem1.ToArray(), numBreedItem2.ToArray()),
                    ItemHelper.MAX_BREED_DIFFERENCE),
                new ValuePair(1,
                        ItemHelper.GetColorsSimilarity(colorsNum1.ToArray(), colorsNum2.ToArray()),
                        ItemHelper.MAX_COLOR_DIFFERENCE),
                new ValuePair(1,
                        ItemHelper.GetSizesSimilarity(item1.GetSizeNum(item1.GetBreed()), item2.GetSizeNum(item2.GetBreed())),
                        ItemHelper.MAX_SIZE_DIFFERENCE)
         
            };
        }
    }
}
