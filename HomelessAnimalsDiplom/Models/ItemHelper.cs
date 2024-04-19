namespace HomelessAnimalsDiplom.Models
{
    public static class ItemHelper
    {
        //public static readonly Period[] Periods;
        public static double[,] BreedsSimilarity;

        public const int MAX_BREED_DIFFERENCE = 4;
       
        public const int MAX_TREE_PROXIMITY = 3; // уровни

        static ItemHelper()
        {
            BreedsSimilarity = new[,]
            {
                { 1, 0.2, 0.1, 0, 0, 0 }, // Мейн-Кун
                { 0.2, 1, 0.3, 0, 0, 0 }, // Британская
                { 0.1, 0.3, 1, 0, 0, 0 }, // Другая (кот)
                { 0, 0, 0, 1, 0.1, 0.2 }, // Мопс
                { 0, 0, 0, 0.1, 1, 0.6 }, // Нем овчарка
                { 0, 0, 0, 0.2, 0.6, 1 }, // Другая (собака)
            };
        }

        public static double GetBreedsSimilarity(int[] list1, int[] list2)
        {
            double similarity = 0;

            // Создаём и заполняем матрицу схожести пород
            // (строки - породы первого списка, столбцы - второго).
            var similarityMatrix = new double[list1.Length, list2.Length];
            var commonBreeds = new List<int[]>();
            for (var i = 0; i < list1.Length; i++)
            {
                for (var j = 0; j < list2.Length; j++)
                {
                    similarityMatrix[i, j] =
                        BreedsSimilarity[(int)list1[i], (int)list2[j]];
                    // Суммируем общее значение схожести.
                    similarity += similarityMatrix[i, j];

                    // Сохраняем координаты общих пород (схожесть = 1).
                    if (Math.Abs(similarityMatrix[i, j] - 1) < 0.01)
                    {
                        commonBreeds.Add(new[] { i, j });
                    }
                }
            }

            // Дополняем до 1 те значения схожести, которые стоят
            // на пересечении строк и столбцов общих пород,
            // увеличивая таким образом общую схожесть.
            // То есть в таблице схожести должно быть
            // (commonBreeds.Count * commonBreeds.Count) единиц.
            for (var i = 0; i < commonBreeds.Count; i++)
            {
                for (var j = 0; j < commonBreeds.Count; j++)
                {
                    if (i != j)
                    {
                        similarity += 1 - similarityMatrix[commonBreeds[i][0],
                                          commonBreeds[j][1]];
                    }
                }
            }

            // Получаем схожесть списков пород как среднее арифметическое всех ячеек.
            similarity /= list1.Length * list2.Length;

            return similarity;
        }
    }
}
