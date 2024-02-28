using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            Shippings = new HashSet<Shipping>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Gender { get; set; }
        public long? Mobile { get; set; }
        public string? Role { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Shipping> Shippings { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
