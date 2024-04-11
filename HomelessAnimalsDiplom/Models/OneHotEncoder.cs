using MongoDB.Bson;

namespace HomelessAnimalsDiplom.Models
{
    /// <summary>
    /// Класс, позволяющий конвертировать категориальные признаки в числовые.
    /// </summary>
    public class OneHotEncoder
    {
        private Dictionary<ObjectId, int> _mapping;
        private int _numCategories;

        public OneHotEncoder()
        {
            _mapping = new Dictionary<ObjectId, int>();
            _numCategories = 0;
        }

        public void Fit(ObjectId[] categories)
        {
            foreach (var category in categories)
            {
                if (!_mapping.ContainsKey(category))
                {
                    _mapping.Add(category, _numCategories);
                    _numCategories++;
                }
            }
        }

        public double[] Transform(ObjectId category)
        {
            double[] encoded = new double[_numCategories];

            if (_mapping.ContainsKey(category))
            {
                int index = _mapping[category];
                encoded[index] = 1.0;
            }

            return encoded;
        }
    }
}
