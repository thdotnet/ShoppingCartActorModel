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
    public static class GetCartForSession
    {
        [FunctionName(nameof(GetCartForSession))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var id = req.Query["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult("The ID is required");
            }
            var entityId = new EntityId(nameof(ShoppingCartEntity), id.ToString());

            var stateResponse = await client.ReadEntityStateAsync<ShoppingCartEntity>(entityId);

            if (!stateResponse.EntityExists)
            {
                return (ActionResult)new NotFoundObjectResult("No cart with this id");
            }

            var response = stateResponse.EntityState.GetCartItems();

            return (ActionResult)new OkObjectResult(response.Result);
        }
    }
}
