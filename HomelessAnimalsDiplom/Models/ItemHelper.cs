namespace HomelessAnimalsDiplom.Models
{
    public static class ItemHelper
    {
        //public static readonly Period[] Periods;
        public static double[,] BreedsSimilarity;
        public static double[,] ColorsSimilarity;
        public static double[,] SizesSimilarity;
        public const int MAX_BREED_DIFFERENCE = 16;
        public const int MAX_TREE_PROXIMITY = 3; // уровни
        public static double MAX_COLOR_DIFFERENCE { get; } = 6; // 2 0.5 0.5 07. 0.3 0.6
        public static double MAX_SIZE_DIFFERENCE { get; } = 2; // 2
        static ItemHelper()
        {
            BreedsSimilarity = BreedSimilarity.ConvertToDoubleMatrix(BreedSimilarity.GetAllBreedsSimilarity(), Breed.GetAllBreeds());
            ColorsSimilarity = ColorSimilarity.ConvertToDoubleMatrix(ColorSimilarity.GetAllColorsSimilarity());
            SizesSimilarity = SizeSimilarity.ConvertToDoubleMatrix(SizeSimilarity.GetAllSizesSimilarity());
        }
        public static double GetSizesSimilarity(int size1, int size2)
        {
            return SizesSimilarity[size1, size2];
        }
        public static double GetColorsSimilarity(int[] colors1, int[] colors2)
        {
            double similarity = 0;

            // Создаём и заполняем матрицу схожести пород
            // (строки - цвета первого списка, столбцы - второго).
            var similarityMatrix = new double[colors1.Length, colors2.Length];
            var commonColors = new List<int[]>();
            for (var i = 0; i < colors1.Length; i++)
            {
                for (var j = 0; j < colors2.Length; j++)
                {
                    similarityMatrix[i, j] =
                        ColorsSimilarity[colors1[i], colors2[j]];
                    // Суммируем общее значение схожести.
                    similarity += similarityMatrix[i, j];

                    // Сохраняем координаты общих пород (схожесть = 1).
                    if (Math.Abs(similarityMatrix[i, j] - 1) < 0.01)
                    {
                        commonColors.Add(new[] { i, j });
                    }
                }
            }

            // Дополняем до 1 те значения схожести, которые стоят
            // на пересечении строк и столбцов общих цветов,
            // увеличивая таким образом общую схожесть.
            // То есть в таблице схожести должно быть
            // (commonBreeds.Count * commonBreeds.Count) единиц.
            for (var i = 0; i < commonColors.Count; i++)
            {
                for (var j = 0; j < commonColors.Count; j++)
                {
                    if (i != j)
                    {
                        similarity += 1 - similarityMatrix[commonColors[i][0],
                                          commonColors[j][1]];
                    }
                }
            }

            // Получаем схожесть списков цветов как среднее арифметическое всех ячеек.
            similarity /= colors1.Length * colors1.Length;

            return similarity;

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
                        BreedsSimilarity[list1[i], list2[j]];
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
