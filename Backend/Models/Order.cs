using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderedItems = new HashSet<OrderedItem>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PaymentId { get; set; }
        public decimal? Amount { get; set; }
        public string? Status { get; set; }
        public string? PaymentType { get; set; }
        public DateTime? Date { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<OrderedItem> OrderedItems { get; set; }
    }
}
