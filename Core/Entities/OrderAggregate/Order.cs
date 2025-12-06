using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class Order : BaseEntity, IDtoConvertible
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DeliveryMethod DeliveryMethod { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = [];
        public required string BuyerEmail { get; set; }
        public PaymentSummary PaymentSummary { get; set; } = null!;
        public ShippingAddress ShippingAddress { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public required string PaymentIntentId { get; set; }

        public decimal GetTotal()
        {
            return Subtotal + DeliveryMethod.Price;
        }
    }
}
