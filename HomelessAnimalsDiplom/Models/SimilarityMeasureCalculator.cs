

namespace HomelessAnimalsDiplom.Models
{
    public static class SimilarityMeasureCalculator
    {
        /// <summary>
        /// Нормализованное Евклидово расстояние
        /// </summary>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static double CalcEuclideanDistance(ValuePair[] valuePairs)
        {
            double result = 0;
            foreach (var pair in valuePairs)
            {
                result += Math.Pow((pair.First - pair.Second) / pair.MaxDifference, 2);
            }

            result = Math.Sqrt(result / valuePairs.Length);
            return result;
        }

        public static double CalcManhattanDistance(ValuePair[] valuePairs)
        {
            double result = 0;
            foreach (var pair in valuePairs)
            {
                result += Math.Abs(pair.First - pair.Second) / pair.MaxDifference;
            }

            result = Math.Sqrt(result / valuePairs.Length);
            return result;
        }

        static double GetSizesSimilarity(Item item1, Item item2)
        {
            var simSize = ItemHelper.GetSizesSimilarity(item1.GetSizeNum(item1.GetBreed()), item2.GetSizeNum(item2.GetBreed()));
            var size1 = item1.GetBreedSize();
            var size2 = item2.GetBreedSize();

            return simSize; // size1 == size2 ? 1 : 0
        }
        static double GetColorsSimilarity(Item item1, Item item2)
        {

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
            var simColors = ItemHelper.GetColorsSimilarity(colorsNum1.ToArray(), colorsNum2.ToArray());
            var commonColors = item1.Colors.Intersect(item2.Colors).Count();
            var totalColors = item1.Colors.Count() + item2.Colors.Count();
            var a = (double)commonColors / totalColors;
            return a; // simColors > 0.5? 1 : 0
        }
        public static double CalcTreeProximity(string[] parentList1, string[] parentList2,
            Item item1 = null, Item item2 = null)
        {
            if (parentList1 == null)
            {
                throw new ArgumentNullException(nameof(parentList1));
            }

            if (parentList2 == null)
            {
                throw new ArgumentNullException(nameof(parentList2));
            }

            double result = 0.0;
            var commonDepth = Math.Min(parentList1.Length, parentList2.Length);
           
            if (item1 != null && item2 != null)
            {
                var sizeSimilarity = GetSizesSimilarity(item1, item2);
                double colorsSimilarity = GetColorsSimilarity(item1, item2);
                result = 0.6 * sizeSimilarity + 0.4 * colorsSimilarity; // 0.5 * в файлик с настройками в конфиги
            }

            var commonParents = parentList1.Zip(parentList2, (p1, p2) => p1 == p2).Count(c => c);
            if (commonParents > 1) commonParents--;
            result += commonParents;
            result /= commonDepth;

            return result;
        }
        
        public static double CalcCorrelation(ValuePair[] valuePairs)
        {
            double firstMedium = valuePairs.Sum(pair => pair.First) / valuePairs.Length;
            double secondMedium = valuePairs.Sum(pair => pair.Second) / valuePairs.Length;

            double nominator = 0;
            double sumFirst = 0;
            double sumSecond = 0;

            foreach (var pair in valuePairs)
            {
                nominator += (pair.First - firstMedium) *
                             (pair.Second - secondMedium);

                sumFirst += Math.Pow(pair.First - firstMedium, 2);
                sumSecond += Math.Pow(pair.Second - secondMedium, 2);
            }

            double denominator = Math.Sqrt(sumFirst * sumSecond);
            return nominator / denominator;
        }
    }
}
