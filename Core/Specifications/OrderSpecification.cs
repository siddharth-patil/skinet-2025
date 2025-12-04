using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string buyerEmail) : base(o => o.BuyerEmail == buyerEmail)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            AddOrderByDescinding(o => o.OrderDate);
        }

        public OrderSpecification(string buyerEmail, int id) : base(o => o.Id == id && o.BuyerEmail == buyerEmail)
        {
            //AddInclude(o => o.OrderItems);
            //AddInclude(o => o.DeliveryMethod);
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(string paymentIntentId, bool isPaymentIntent): base(o => o.PaymentIntentId == paymentIntentId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            //AddOrderByDescinding(o => o.OrderDate);
        }
    }
}
