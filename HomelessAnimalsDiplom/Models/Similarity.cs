namespace HomelessAnimalsDiplom.Models
{
    public class Similarity
    {
        public Dictionary<Breed, Dictionary<Breed, double>> SizeSimilarity { get; set; } = new();
        public Dictionary<Breed, Dictionary<Breed, double>> BreedSimilarity { get; set; } = new();
        public Dictionary<Item, Dictionary<Item, double>> ColorSimilarity { get; set; } = new();

        public void SizesSimilarityUpdate()
        {

        }
    }
}
