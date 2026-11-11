using System.Collections.Generic;
using System.Linq;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI
{
    public static class CartPrices
    {
        public static readonly IReadOnlyDictionary<int, decimal> None = new Dictionary<int, decimal>();

        public static IReadOnlyDictionary<int, decimal> Of(IEnumerable<UserCartProductViewState> lines)
        {
            var prices = new Dictionary<int, decimal>();

            foreach (var line in lines)
            {
                if (line is { ProductId: int productId, Quantity: > 0, PayType: OrderLinePayType.Cash, TotalPrice: > 0 })
                    prices.TryAdd(productId, line.TotalPrice / line.Quantity);
            }

            return prices;
        }

        public static bool Same(IReadOnlyDictionary<int, decimal> left, IReadOnlyDictionary<int, decimal> right) =>
            left.Count == right.Count && left.All(a => right.TryGetValue(a.Key, out var price) && price == a.Value);

        public static decimal? Own(UserProductViewState product, IReadOnlyDictionary<int, decimal> prices) =>
            product is not null && prices is not null && prices.TryGetValue(product.Id, out var own) && own < product.UnitPrice
                ? own
                : null;
    }
}
