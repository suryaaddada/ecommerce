using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Shipping
    {
        public Shipping()
        {
            OrderedItems = new HashSet<OrderedItem>();
        }

        public int Id { get; set; }
        public int? Userid { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? Pincode { get; set; }
        public string? Country { get; set; }
        public long? Mobile { get; set; }
        public string? State { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
    }
}
