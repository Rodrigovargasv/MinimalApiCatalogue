﻿using System.Text.Json.Serialization;

namespace ApiCatalogue.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public DateTime DatePurchase { get; set; }
        public int? Stock { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set;}

    }
}
