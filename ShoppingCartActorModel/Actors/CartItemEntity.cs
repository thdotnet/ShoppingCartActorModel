using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartActorModel.Actors
{
    public class CartItemEntity
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
    }
}
