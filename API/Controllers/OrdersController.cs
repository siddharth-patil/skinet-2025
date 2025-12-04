using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();
            var cart = await cartService.GetCartAsync(orderDto.CartId);
            if (cart == null) return BadRequest("Problem retrieving cart/Cart not found");

            if (cart.PaymentIntentId == null) return BadRequest("No Payment Intent for this order");

            var items = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);

                if (productItem == null)
                {
                    return BadRequest($"Product with id {item.ProductId} not found");
                }

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);

            }

            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod == null) return BadRequest("Delivery method not found");

            var order = new Order
            {
                BuyerEmail = email,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = orderDto.ShippingAddress,
                DeliveryMethod = deliveryMethod,
                PaymentSummary = orderDto.PaymentSummary,
                OrderItems = items,
                Subtotal = items.Sum(item => item.Price * item.Quantity),
                PaymentIntentId = cart.PaymentIntentId
            };

            unit.Repository<Order>().Add(order);

            if (await unit.Complete()) {
                //await cartService.DeleteCartAsync(orderDto.CartId);
                return order;
            }
            return BadRequest("Problem creating order");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var spec = new OrderSpecification(User.GetEmail());
            var orders = await unit.Repository<Order>().ListAsync(spec);
            var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(User.GetEmail(), id);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return NotFound();
            return order.ToDto();
        }
    }
}
