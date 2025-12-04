using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    //public class PaymentService(IConfiguration config, ICartService cartService, 
    //    IGenericRepository<Core.Entities.Product> productRepo,
    //    IGenericRepository<DeliveryMethod> dmRepo) : IPaymentService
    public class PaymentService(IConfiguration config, ICartService cartService,
        IUnitOfWork unit) : IPaymentService
    {
        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

            var cart = await cartService.GetCartAsync(cartId);

            if (cart == null) return null;

            var shippingPrice = 0m;

            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);

                if (deliveryMethod == null) return null;

                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);
                if (productItem == null) return null;
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(cart.Items.Sum(x => x.Quantity * (x.Price * 100))) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)(shippingPrice * 100),
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }

            //// compute amount in cents safely
            //long ComputeAmountInCents()
            //{
            //    decimal itemsTotal = cart.Items.Sum(x => x.Quantity * x.Price);
            //    decimal totalCents = Math.Round(itemsTotal * 100M) + Math.Round(shippingPrice * 100M);
            //    return (long)totalCents;
            //}

            //var amount = ComputeAmountInCents();

            //var updatableStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            //{
            //    "requires_payment_method",
            //    "requires_confirmation",
            //    "requires_action"
            //};

            //// If there's no stored PaymentIntent, create a new one
            //if (string.IsNullOrEmpty(cart.PaymentIntentId))
            //{
            //    var options = new PaymentIntentCreateOptions
            //    {
            //        Amount = amount,
            //        Currency = "usd",
            //        PaymentMethodTypes = new List<string> { "card" }
            //    };
            //    intent = await service.CreateAsync(options);
            //    cart.PaymentIntentId = intent.Id;
            //    cart.ClientSecret = intent.ClientSecret;
            //}
            //else
            //{
            //    // Try to fetch current intent to determine if it's safe to update
            //    PaymentIntent? current = null;
            //    try
            //    {
            //        current = await service.GetAsync(cart.PaymentIntentId);
            //    }
            //    catch (StripeException)
            //    {
            //        // If we can't fetch (e.g., not found), we'll create a new intent below
            //        current = null;
            //    }

            //    // If we couldn't fetch or the current intent is in a non-updatable status, create a new intent
            //    if (current == null || !updatableStatuses.Contains(current.Status ?? string.Empty))
            //    {
            //        var createOptions = new PaymentIntentCreateOptions
            //        {
            //            Amount = amount,
            //            Currency = "usd",
            //            PaymentMethodTypes = new List<string> { "card" }
            //        };
            //        intent = await service.CreateAsync(createOptions);
            //        cart.PaymentIntentId = intent.Id;
            //        cart.ClientSecret = intent.ClientSecret;
            //    }
            //    else
            //    {
            //        // Safe to update
            //        var updateOptions = new PaymentIntentUpdateOptions
            //        {
            //            Amount = amount
            //        };

            //        try
            //        {
            //            intent = await service.UpdateAsync(cart.PaymentIntentId, updateOptions);
            //        }
            //        catch (StripeException ex)
            //        {
            //            // Fallback: if Stripe says it cannot be updated (race or status changed),
            //            // create a new PaymentIntent so the client can continue
            //            // (this prevents the exception from bubbling and leaves cart in usable state)
            //            if (ex.Message?.Contains("could not be updated", StringComparison.OrdinalIgnoreCase) == true
            //                || ex.StripeError?.Code == "payment_intent_unexpected_state")
            //            {
            //                var createOptions = new PaymentIntentCreateOptions
            //                {
            //                    Amount = amount,
            //                    Currency = "usd",
            //                    PaymentMethodTypes = new List<string> { "card" }
            //                };
            //                intent = await service.CreateAsync(createOptions);
            //                cart.PaymentIntentId = intent.Id;
            //                cart.ClientSecret = intent.ClientSecret;
            //            }
            //            else
            //            {
            //                throw;
            //            }
            //        }
            //    }
            //}

            await cartService.SetCartAsync(cart);
            return cart;

        }
    }
}
