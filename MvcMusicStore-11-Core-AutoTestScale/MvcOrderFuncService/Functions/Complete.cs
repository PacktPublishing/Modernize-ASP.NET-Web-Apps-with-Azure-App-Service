using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MvcMusicStore.Models;
using System.Linq;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MvcOrderFuncService.Helpers;

namespace MvcOrderService
{
    public static class Complete
    {

        [FunctionName("Complete")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req, 
            TraceWriter log)
        {
            var queryParams = req.GetQueryNameValuePairs();
            var id = int.Parse(queryParams.FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value);
            var username = queryParams.FirstOrDefault(q => string.Compare(q.Key, "username", true) == 0).Value;


            var storeDB = new DbContextHelper().GetContext();

            bool isValid = storeDB.Orders.Any(
                         o => o.OrderId == id &&
                         o.Username == username);

            return req.CreateResponse(HttpStatusCode.OK, isValid);
            
        }
    }
}
