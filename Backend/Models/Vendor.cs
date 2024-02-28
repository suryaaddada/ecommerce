using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public long? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? Pincode { get; set; }
        public bool? Isapproved { get; set; }
        public string? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
