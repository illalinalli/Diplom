using MongoDB.Bson;

namespace MonkeyShop.Models
{
    public class Item
    {
        public ObjectId Id { get; set; }
        public string? Title { get; set; }
        public bool IsPublished { get; set; }
        public List<byte[]>? Images { get; set; }
        public ObjectId BreedRef { get; set; }

        public List<ObjectId>? Properties { get; set; }
        public string? LongDescription { get; set; }
        public ObjectId UserRef { get; set; }
        public DateTimeOffset? CreationDate { get; set; }

        public Item()
        {
            Id = ObjectId.GenerateNewId();
        }

    }
}
