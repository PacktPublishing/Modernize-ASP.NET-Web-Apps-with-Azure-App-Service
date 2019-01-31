using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using MusicStore.Models;
using Microsoft.Extensions.DependencyInjection;
using MvcMusicStore.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using MvcOrderFuncService.Helpers;

namespace MvcOrderService
{
    public static class AddressAndPayment
    {
        
        [FunctionName("AddressAndPayment")]
        public static async Task<HttpResponseMessage>  Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, 
            TraceWriter log)
        {
            OrderCreation creation = await req.Content.ReadAsAsync<OrderCreation>();

            var storeDB = new DbContextHelper().GetContext();
            
            var order = creation.OrderToCreate;
            //Save Order
            storeDB.Orders.Add(order);
            storeDB.SaveChanges();

            //Process the order
            decimal orderTotal = 0;


            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in creation.CartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Album.Price);

                storeDB.OrderDetails.Add(orderDetail);

            }

            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            storeDB.SaveChanges();

            return req.CreateResponse(HttpStatusCode.OK, order.OrderId);
        }
    }
}
