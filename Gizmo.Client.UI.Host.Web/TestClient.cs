using Gizmo.Shared.Client.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Host.Web
{
    public class TestClient : IGizmoClient
    {
        public IEnumerable<ProductGroupViewModel> GetProductGroups()
        {
            List<ProductGroupViewModel> productGroups = new List<ProductGroupViewModel>();
            productGroups.Add(new ProductGroupViewModel() { Id = 1, Name = "Coffee" });
            productGroups.Add(new ProductGroupViewModel() { Id = 2, Name = "Beverages" });
            productGroups.Add(new ProductGroupViewModel() { Id = 3, Name = "Sandwiches" });
            productGroups.Add(new ProductGroupViewModel() { Id = 4, Name = "Snacks" });
            productGroups.Add(new ProductGroupViewModel() { Id = 5, Name = "Time offers" });

            productGroups.Add(new ProductGroupViewModel() { Id = 6, Name = "Coffee" });
            productGroups.Add(new ProductGroupViewModel() { Id = 7, Name = "Beverages" });
            productGroups.Add(new ProductGroupViewModel() { Id = 8, Name = "Sandwiches" });
            productGroups.Add(new ProductGroupViewModel() { Id = 9, Name = "Snacks" });
            productGroups.Add(new ProductGroupViewModel() { Id = 10, Name = "Time offers" });
            productGroups.Add(new ProductGroupViewModel() { Id = 11, Name = "Coffee" });
            productGroups.Add(new ProductGroupViewModel() { Id = 12, Name = "Beverages" });
            productGroups.Add(new ProductGroupViewModel() { Id = 13, Name = "Sandwiches" });
            productGroups.Add(new ProductGroupViewModel() { Id = 14, Name = "Snacks" });
            productGroups.Add(new ProductGroupViewModel() { Id = 15, Name = "Time offers" });
            productGroups.Add(new ProductGroupViewModel() { Id = 16, Name = "Coffee" });
            productGroups.Add(new ProductGroupViewModel() { Id = 17, Name = "Beverages" });
            productGroups.Add(new ProductGroupViewModel() { Id = 18, Name = "Sandwiches" });
            productGroups.Add(new ProductGroupViewModel() { Id = 19, Name = "Snacks" });
            productGroups.Add(new ProductGroupViewModel() { Id = 20, Name = "Time offers" });

            return productGroups;
        }

        public IEnumerable<ProductViewModel> GetProducts()
        {
            Random random = new Random();

            List<ProductViewModel> products = Enumerable.Range(0, 18).Select(i => new ProductViewModel()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            }).ToList();

            products.Add(new ProductViewModel()
            {
                ProductGroupId = 100,
                Name = "Freddo Espresso Coffee",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            });

            return products;
        }

        public IEnumerable<ApplicationViewModel> GetApplications()
        {
            List<ApplicationViewModel> applications = Enumerable.Range(0, 15).Select(i => new ApplicationViewModel()
            {
                Id = i + 1,
                Title = $"Fortnite ({ i + 1 })",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                ReleaseDate = DateTime.Now
            }).ToList();

            return applications;
        }

        public IEnumerable<ExecutableViewModel> GetExecutables()
        {
            List<ExecutableViewModel> executables = new List<ExecutableViewModel>();
            executables.Add(new ExecutableViewModel() { Id = 1, Caption = "battlenet.exe" });
            executables.Add(new ExecutableViewModel() { Id = 2, Caption = "DOTA"});
            executables.Add(new ExecutableViewModel() { Id = 3, Caption = "Spotify"});
            executables.Add(new ExecutableViewModel() { Id = 4, Caption = "valve_steamclient.exe" });

            return executables;
        }

    }
}
