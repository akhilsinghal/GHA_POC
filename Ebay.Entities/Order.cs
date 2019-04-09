using System;
using System.Collections.Generic;
using System.Text;

namespace Ebay.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime CreationDate { get; set; }

        public List<Product> Products { get; set; }
    }
}
