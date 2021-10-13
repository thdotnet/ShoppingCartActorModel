using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartActorModel.Actors
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShoppingCartEntity : IShoppingCartEntity
    {
        [JsonProperty("cartItems")]
        private List<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();

        public void Add(CartItemEntity item)
        {
            if (this.CartItems.Any() == false)
            {
                this.CartItems.Add(item);
            }
            else
            {
                var existingItem = this.CartItems.FirstOrDefault(i => i.ProductId == item.ProductId);
                existingItem.Count += item.Count;
            }
        }

        public void Remove(CartItemEntity item)
        {
            var existingItem = this.CartItems.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (existingItem == null)
            {
                return;
            }

            if (existingItem.Count > item.Count)
            {
                existingItem.Count -= item.Count;
            }
            else
            {
                this.CartItems.Remove(existingItem);
            }
        }

        public Task<ReadOnlyCollection<CartItemEntity>> GetCartItems()
        {
            return Task.FromResult(this.CartItems.AsReadOnly());
        }

        [FunctionName(nameof(ShoppingCartEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
               => ctx.DispatchAsync<ShoppingCartEntity>();
    }
}
