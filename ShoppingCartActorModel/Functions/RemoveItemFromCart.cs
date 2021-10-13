using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ShoppingCartActorModel.Actors;

namespace ShoppingCartActorModel.Functions
{
    public static class RemoveItemFromCart
    {
        [FunctionName(nameof(RemoveItemFromCart))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var id = req.Query["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult("The ID is required");
            }

            var entityId = new EntityId(nameof(ShoppingCartEntity), id.ToString());
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<CartItemEntity>(requestBody);

            await client.SignalEntityAsync<IShoppingCartEntity>(entityId, proxy => proxy.Remove(data));

            return new AcceptedResult();
        }
    }
}
