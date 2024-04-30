using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Drawing;
using static HomelessAnimalsDiplom.Models.Item;
namespace HomelessAnimalsDiplom.Models
{
    public class PropertyValue
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public ObjectId PropTypeRef { get; set; }

        public int GetColorNumber()
        {
            return ColorsNums[Id];
        }
    }
}
