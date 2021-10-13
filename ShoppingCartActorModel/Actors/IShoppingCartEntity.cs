using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ShoppingCartActorModel.Actors
{
    public interface IShoppingCartEntity
    {
        void Add(CartItemEntity item);
        Task<ReadOnlyCollection<CartItemEntity>> GetCartItems();
        void Remove(CartItemEntity item);
    }
}