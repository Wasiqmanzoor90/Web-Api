using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WebApplication1.Models
{
    public class Product
    {
        public ObjectId Id{get; set; }
        public required string Name { get; set; }
        public required string Size { get; set; }

        public int Quantity { get; set; }
    }
}