using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Drawing;
using static HomelessAnimalsDiplom.Models.Item;
using static HomelessAnimalsDiplom.Models.Database;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace HomelessAnimalsDiplom.Models
{
    public class PropertyValue
    {
        public ObjectId Id { get; set; }
        [BsonIgnore]
        public string IdAsString
        {
            get
            {
                return Id.ToString();
            }
        }
        public string Name { get; set; }
        public ObjectId PropTypeRef { get; set; }

        public PropertyValue()
        {
            Id = ObjectId.GenerateNewId();
        }
        public static void SaveNewColor(PropertyValue newColor)
        {
            if (newColor.Name == null && newColor.PropTypeRef == ObjectId.Empty && newColor.Id == ObjectId.Empty) return;
            // создание фильтра для поиска существующей записи
            var filter = Builders<PropertyValue>.Filter.Eq("_id", newColor.Id);

            // выполнение операции upsert
            PropertyValueCollection?.ReplaceOneAsync(filter, newColor, ReplaceOptionsUpsert);
            //return newBreed;
        }
        public int GetColorNumber()
        {
            return ColorsNums[Id];
        }

        //public static List<PropertyValue> GetAllColors()
        //{
        //    return 
        //}
        public static List<PropertyValue> GetAllPropertyValues()
        {
            return PropertyValueCollection.Find(new BsonDocument()).ToList();
        }
        public static List<PropertyValue> GetAllSizes()
        {
            var propType = PropertyTypeCollection.Find(x => x.Name == "Размер животного").FirstOrDefault();
            //if (propType?.Id == null) return null;
            return GetAllPropertyValues().Where(x => x.PropTypeRef == propType.Id).ToList();
        }
    }
}
