using MongoDB.Bson;

namespace HomelessAnimalsDiplom.Models
{
    public class HistoryItem
    {
        public ObjectId ItemId { get; set; }
        public DateTimeOffset ViewedAt { get; set; }
    }

    public class User
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public List<ObjectId> Favorites { get; set; } = new();
        public List<HistoryItem>? ViewingHistory { get; set; } = new();

        public bool IsAdmin { get; set; }
    }

}
