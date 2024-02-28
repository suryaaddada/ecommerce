using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class OrderedItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public int? InventoryId { get; set; }
        public int? ShippingId { get; set; }

        public virtual Inventory? Inventory { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Shipping? Shipping { get; set; }
    }
}
