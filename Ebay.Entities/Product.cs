using System;

namespace Ebay.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDesc { get; set; }

        public decimal Price { get; set; }
    }
}
