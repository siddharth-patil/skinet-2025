using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderMappingExtentions
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                DeliveryMethod = order.DeliveryMethod.Description,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems = order.OrderItems.Select(x => x.ToDto()).ToList(),
                BuyerEmail = order.BuyerEmail,
                PaymentSummary = order.PaymentSummary,
                ShippingAddress = order.ShippingAddress,
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                Status = order.Status.ToString(),
                PaymentIntentId = order.PaymentIntentId
            };
        }

        public static OrderItemDto ToDto(this OrderItem item)
        {
            return new OrderItemDto
            {
                ProductId = item.ItemOrdered.ProductId,
                ProductName = item.ItemOrdered.ProductName,
                PictureUrl = item.ItemOrdered.PictureUrl,
                Price = item.Price,
                Quantity = item.Quantity
            };
        }
    }
}
