namespace HomelessAnimalsDiplom.Models
{
    public static class ItemHelper
    {
        //public static readonly Period[] Periods;
        public static double[,] BreedsSimilarity;
        public static double[,] ColorsSimilarity;
        public const int MAX_BREED_DIFFERENCE = 4;
        public const int MAX_TREE_PROXIMITY = 5; // уровни
        public static double MAX_COLOR_DIFFERENCE { get; } = 2;
        public static double MAX_SIZE_DIFFERENCE { get; } = 2;
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

            ColorsSimilarity = new[,]
            {
                { 1, 0, 0.1, 0, 0, 0, 0 }, // white
                { 0, 1, 0.3, 0, 0, 0, 0 }, // black
                { 0.1, 0.3, 1, 0, 0, 0, 0.6 }, // gray
                { 0, 0, 0, 1, 0.6, 0.5, 0 }, // рыжий
                { 0, 0, 0, 0.6, 1, 0.3, 0 }, // brown
                { 0, 0, 0, 0.5, 0.3, 1, 0 }, // бежевый
                { 0, 0, 0.6, 0, 0, 0, 1 }, // blue
            };
        }
        public static double GetSizesSimilarity(string size1, string size2)
        {
            // Приведем размеры к нижнему регистру для удобства сравнения
            size1 = size1.ToLower();
            size2 = size2.ToLower();

            // Проверяем сходство размеров
            if (size1 == size2)
            {
                // Если размеры полностью совпадают
                return 1.0;
            }
            else if ((size1 == "мелкий" && size2 == "средний") || (size1 == "средний" && size2 == "мелкий"))
            {
                // Если размеры отличаются на один уровень (мелкий и средний)
                return 0.5;
            }
            else if ((size1 == "мелкий" && size2 == "крупный") || (size1 == "крупный" && size2 == "мелкий"))
            {
                // Если размеры отличаются на два уровня (мелкий и крупный)
                return 0.2;
            }
            else if ((size1 == "средний" && size2 == "крупный") || (size1 == "крупный" && size2 == "средний"))
            {
                // Если размеры отличаются на один уровень (средний и крупный)
                return 0.7;
            }
            else
            {
                // Если размеры полностью различны
                return 0.0;
            }
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
                        ColorsSimilarity[(int)colors1[i], colors2[j]];
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
