using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI
{
    public static class PointsCover
    {
        public static IReadOnlySet<int> MostCovered(IReadOnlyList<int> prices, int budget)
        {
            var reached = new Dictionary<int, (int From, int Item)> { [0] = (0, -1) };

            for (var item = 0; item < prices.Count; item++)
            {
                var price = prices[item];
                if (price <= 0 || price > budget)
                    continue;

                foreach (var sum in reached.Keys.ToList())
                {
                    var next = sum + price;

                    if (next <= budget && !reached.ContainsKey(next))
                        reached[next] = (sum, item);
                }
            }

            var chosen = new HashSet<int>();

            for (var sum = reached.Keys.Max(); sum > 0; sum = reached[sum].From)
                chosen.Add(reached[sum].Item);

            return chosen;
        }
    }
}
