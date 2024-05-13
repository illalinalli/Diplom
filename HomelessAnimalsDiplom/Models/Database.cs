using MongoDB.Bson;
using MongoDB.Driver;
using NuGet.Packaging;
using Radzen;
using System.Drawing;
using System.Text;
using static HomelessAnimalsDiplom.Models.Breed;
using static HomelessAnimalsDiplom.Models.Item;
namespace HomelessAnimalsDiplom.Models
{
    public class Database
    {
        public static IMongoDatabase? DB;
        public static IMongoCollection<User>? UserCollection;
        public static IMongoCollection<Item>? ItemCollection;
        public static IMongoCollection<Breed>? BreedCollection;
        public static IMongoCollection<AnimalType>? AnimalTypeCollection;
        public static IMongoCollection<PropertyType>? PropertyTypeCollection;
        public static IMongoCollection<PropertyValue>? PropertyValueCollection;
        public static IMongoCollection<BreedSimilarity>? BreedSimilarityCollection;
        public static IMongoCollection<ColorSimilarity>? ColorSimilarityCollection;
        public static IMongoCollection<SizeSimilarity>? SizeSimilarityCollection;
        public static IMongoCollection<CoefficientAdjuster>? CoefficientAdjusterCollection;
       
        public static string GetHash(string password)
        {
            var sha = System.Security.Cryptography.SHA1.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return hash.Aggregate("", (string s, byte b) => s + b.ToString("x2"));
        }
        public static void HashPassword()
        {
            var users = UserCollection.Find(new BsonDocument()).ToList();
            foreach (var user in users)
            {
                var hash = GetHash(user.Password);
                user.Password = hash;
                var filter = Builders<User>.Filter.Eq("_id", user.Id);
                UserCollection?.ReplaceOne(filter, user, ReplaceOptionsUpsert);
            }
        }

        //void AddProps()
        //{
        //    var allItems = ItemCollection.Find(new BsonDocument()).ToList();
        //    var allPropValues = PropertyValueCollection.Find(new BsonDocument()).ToList();
        //    var sex = ObjectId.Parse("658ab0418a4dcfd166a80344"); // db
        //    var color = ObjectId.Parse("658ab0a38a4dcfd166a80346"); // db
        //    foreach (var it in allItems)
        //    {
        //        foreach (var prop in it.Properties)
        //        {
        //            var res = allPropValues.FirstOrDefault(x => x.Id == prop);
        //            var propVal = allPropValues.FirstOrDefault(x => x.Id == prop);
        //            if (propVal.PropTypeRef == sex)
        //            {
        //                it.Sex = res;
        //            }
        //            if (propVal.PropTypeRef == color)
        //            {
        //                it.Colors.Add(res);
        //            }
        //        }
        //        // создание фильтра для поиска существующей записи
        //        var filter = Builders<Item>.Filter.Eq("_id", it.Id);

        //        // выполнение операции upsert
        //        ItemCollection?.ReplaceOneAsync(filter, it, ReplaceOptionsUpsert);
        //    }
         
        //}
        public Database()
        {
            var connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            DB = client.GetDatabase("HomelessAnimals");
            UserCollection = DB.GetCollection<User>("User");
            ItemCollection = DB.GetCollection<Item>("Item");
            BreedCollection = DB.GetCollection<Breed>("Breed");
            AnimalTypeCollection = DB.GetCollection<AnimalType>("AnimalType");
            PropertyTypeCollection = DB.GetCollection<PropertyType>("PropertyType");
            PropertyValueCollection = DB.GetCollection<PropertyValue>("PropertyValue");
            BreedSimilarityCollection = DB.GetCollection<BreedSimilarity>("BreedSimilarity");
            ColorSimilarityCollection = DB.GetCollection<ColorSimilarity>("ColorSimilarity");
            SizeSimilarityCollection = DB.GetCollection<SizeSimilarity>("SizeSimilarity");
            CoefficientAdjusterCollection = DB.GetCollection<CoefficientAdjuster>("CoefficientAdjuster");
            SetBreedsNum();
            SetColorsNum();
            SetSizesNum();

            //AddProps();
            //HashPassword();
        }


        public static readonly ReplaceOptions ReplaceOptionsUpsert = new() { IsUpsert = true };
    }
}
