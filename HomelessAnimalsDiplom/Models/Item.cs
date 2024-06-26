﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Xml.Linq;
using static HomelessAnimalsDiplom.Models.Breed;
using static HomelessAnimalsDiplom.Models.Database;

namespace HomelessAnimalsDiplom.Models
{
    public class Item
    {
        public ObjectId Id { get; set; }
        public string? Title { get; set; }
        public bool IsPublished { get; set; }
        public List<byte[]>? Images { get; set; }
        public ObjectId BreedRef { get; set; }
        public List<ObjectId>? Properties { get; set; } = new();
        public List<PropertyValue> Colors { get; set; } = new();
        public PropertyValue Sex { get; set; }
        public string? LongDescription { get; set; }
        public ObjectId UserRef { get; set; }
        public DateTimeOffset? CreationDate { get; set; }
        //public PropertyValue Size { get; set; } // крупный, мелкий, средний
        public string[]? Parents { get; set; }

        [BsonIgnore]
        public static Dictionary<ObjectId, int> ColorsNums { get; set; } = new();

        public static Item GetById(ObjectId id)
        {
            return GetAllItems().FirstOrDefault(x => x.Id == id);
        }
        public static List<Item> GetNewestItems()
        {
            List<Item> res = new();
            var allItems = GetAllItems();

            if (allItems.Count == 0) return res; // Если нет публикаций, возвращаем пустой список

            // Находим максимальную дату создания
            var maxCreationDate = allItems.Max(item => item.CreationDate);

            // Добавляем все публикации с максимальной датой создания
            res = allItems.Where(item => item.CreationDate == maxCreationDate).ToList();

            return res;
        }
        public static List<Item> GetMostPopularItems()
        {
            List<Item> result = new();
            var allItems = GetAllItems();
            var allUsers = User.GetAllUsers();

            Dictionary<ObjectId, int> itemLikes = new();

            foreach (var user in allUsers)
            {
                foreach (var itemId in user.Favorites)
                {
                    if (itemLikes.ContainsKey(itemId))
                    {
                        itemLikes[itemId]++;
                    }
                    else
                    {
                        itemLikes[itemId] = 1;
                    }
                }
            }

            var sortedItems = itemLikes.OrderByDescending(x => x.Value).ToList();

            int maxLikes = sortedItems[0].Value;

            foreach (var item in sortedItems)
            {
                if (item.Value == maxLikes)
                {
                    result.Add(GetById(item.Key));
                }
                else
                {
                    break;
                }
            }

            return result;
        }
        public bool HaveColor(PropertyValue propertyValue)
        {
            // this.Colors
            foreach (var c in Colors)
            {
                if (c.Id == propertyValue.Id)
                {
                    return true;
                }
            }
            return false;
        }
        public AnimalType GetAnimalType()
        {
            var curBreed = GetBreed();
            var allTypes = AnimalType.GetAnimalTypes();
            var res = allTypes.FirstOrDefault(x => x.Id == curBreed.AnimalTypeRef);
            return res;
        }
        
        public List<Item> GetAllCats()
        {
            var allItems = ItemCollection.Find(new BsonDocument()).ToList();
            var allBreeds = GetAllBreeds();
            ObjectId catId = ObjectId.Parse("654c9b86b060976eb8527e54");
            var onlyCatsBreeds = allBreeds.FindAll(x => x.AnimalTypeRef == catId);
            List<Item> res = new();
            allItems.ForEach(x => 
            {
                onlyCatsBreeds.ForEach(y => 
                {
                    if (x.BreedRef == y.Id)
                    {
                        res.Add(x);
                    }
                   
                });
            });
            return res;
        }

        public int GetBreedNumber()
        {
            return BreedsNums[BreedRef];
        }
        public int GetSizeNum(Breed breed)
        {
            return SizesNums[breed.SizeRef];
        }
        public Breed GetBreed()
        {
            var a = GetAllBreeds();
            return a.FirstOrDefault(x => x.Id == BreedRef);
        }

        public PropertyValue GetBreedSize()
        {
            var allBreeds = GetAllBreeds();
            var allProps = GetAllPropValues();
            var curBreed = allBreeds.FirstOrDefault(x => x.Id == BreedRef);
            var size = allProps.FirstOrDefault(x => x.Id == curBreed.SizeRef);
            if (size == null) return null;
            return size;
        }

        public static List<Item> GetAllItems()
        {
            return ItemCollection.Find(new BsonDocument()).ToList();
        }
        
        public static List<Breed> GetAllBreeds()
        {
            return BreedCollection.Find(new BsonDocument()).ToList();
        }
        public static List<PropertyValue> GetAllPropValues()
        {
            return PropertyValueCollection.Find(new BsonDocument()).ToList();
        }
        public static List<PropertyValue> GetAllSexes()
        {
            var sex = PropertyTypeCollection.Find(s => s.Name == "Пол животного").FirstOrDefault();
            if (sex == null) return null;
            return PropertyValueCollection.Find(new BsonDocument()).ToList().FindAll(x => x.PropTypeRef == sex.Id);
        }
        public static List<PropertyValue> GetAllColors()
        {
            var color = PropertyTypeCollection.Find(c => c.Name == "Окрас животного").FirstOrDefault();
            if (color == null) return null;
            return PropertyValueCollection.Find(new BsonDocument()).ToList().FindAll(x => x.PropTypeRef == color.Id);
        }
        public static void SetColorsNum()
        {
            var allPropertyValues = GetAllColors();
            int num = 0;
            foreach (var pt in allPropertyValues)
            {
                ColorsNums.TryAdd(pt.Id, num);
                num++;
            }
        }

        public static void SetSizesNum()
        {
            var allPropertyValues = PropertyValue.GetAllSizes();
            int num = 0;
            foreach (var pt in allPropertyValues)
            {
                SizesNums.TryAdd(pt.Id, num);
                num++;
            }
        }
        public Item()
        {
            //Id = ObjectId.GenerateNewId();
            
            Parents = new string[0];
        }

    }
}
