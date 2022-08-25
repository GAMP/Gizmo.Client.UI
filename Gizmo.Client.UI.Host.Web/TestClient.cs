using Gizmo.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Host.Web
{
    public class TestClient : IGizmoClient
    {
        public IEnumerable<ProductGroup> GetProductGroups(ProductGroupsFilter filter)
        {
            List<ProductGroup> productGroups = new List<ProductGroup>();
            productGroups.Add(new ProductGroup() { Id = 1, Name = "Coffee" });
            productGroups.Add(new ProductGroup() { Id = 2, Name = "Beverages" });
            productGroups.Add(new ProductGroup() { Id = 3, Name = "Sandwiches" });
            productGroups.Add(new ProductGroup() { Id = 4, Name = "Snacks" });
            productGroups.Add(new ProductGroup() { Id = 5, Name = "Time offers" });

            productGroups.Add(new ProductGroup() { Id = 6, Name = "Coffee" });
            productGroups.Add(new ProductGroup() { Id = 7, Name = "Beverages" });
            productGroups.Add(new ProductGroup() { Id = 8, Name = "Sandwiches" });
            productGroups.Add(new ProductGroup() { Id = 9, Name = "Snacks" });
            productGroups.Add(new ProductGroup() { Id = 10, Name = "Time offers" });
            productGroups.Add(new ProductGroup() { Id = 11, Name = "Coffee" });
            productGroups.Add(new ProductGroup() { Id = 12, Name = "Beverages" });
            productGroups.Add(new ProductGroup() { Id = 13, Name = "Sandwiches" });
            productGroups.Add(new ProductGroup() { Id = 14, Name = "Snacks" });
            productGroups.Add(new ProductGroup() { Id = 15, Name = "Time offers" });
            productGroups.Add(new ProductGroup() { Id = 16, Name = "Coffee" });
            productGroups.Add(new ProductGroup() { Id = 17, Name = "Beverages" });
            productGroups.Add(new ProductGroup() { Id = 18, Name = "Sandwiches" });
            productGroups.Add(new ProductGroup() { Id = 19, Name = "Snacks" });
            productGroups.Add(new ProductGroup() { Id = 20, Name = "Time offers" });

            return productGroups;
        }

        public IEnumerable<Product> GetProducts(ProductsFilter filter)
        {
            Random random = new Random();

            List<Product> products = Enumerable.Range(0, 18).Select(i => new Product()
            {
                Id = i + 1,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                ProductType = (ProductType)random.Next(0, 3),
            }).ToList();

            return products;
        }

        public IEnumerable<Application> GetApplications(ApplicationsFilter filter)
        {
            List<Application> applications = Enumerable.Range(0, 15).Select(i => new Application()
            {
                Id = i + 1,
                Title = $"Fortnite ({ i + 1 })",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                ReleaseDate = DateTime.Now
            }).ToList();

            return applications;
        }

        public IEnumerable<ApplicationExecutable> GetApplicationExecutables(ApplicationExecutablesFilter filter)
        {
            List<ApplicationExecutable> executables = new List<ApplicationExecutable>();
            executables.Add(new ApplicationExecutable() { Id = 1, Caption = "battlenet.exe" });
            executables.Add(new ApplicationExecutable() { Id = 2, Caption = "DOTA"});
            executables.Add(new ApplicationExecutable() { Id = 3, Caption = "Spotify"});
            executables.Add(new ApplicationExecutable() { Id = 4, Caption = "valve_steamclient.exe" });

            return executables;
        }

    }
}
