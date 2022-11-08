using Gizmo.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Host.WPF
{
    public class TestClient : IGizmoClient
    {
        public TestClient()
        {
            Random random = new Random();

            _productGroups = new List<ProductGroup>();
            _productGroups.Add(new ProductGroup() { Id = 1, Name = "Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 2, Name = "Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 3, Name = "Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 4, Name = "Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 5, Name = "Time offers" });

            _productGroups.Add(new ProductGroup() { Id = 6, Name = "Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 7, Name = "Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 8, Name = "Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 9, Name = "Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 10, Name = "Time offers" });
            _productGroups.Add(new ProductGroup() { Id = 11, Name = "Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 12, Name = "Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 13, Name = "Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 14, Name = "Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 15, Name = "Time offers" });
            _productGroups.Add(new ProductGroup() { Id = 16, Name = "Coffee" });
            _productGroups.Add(new ProductGroup() { Id = 17, Name = "Beverages" });
            _productGroups.Add(new ProductGroup() { Id = 18, Name = "Sandwiches" });
            _productGroups.Add(new ProductGroup() { Id = 19, Name = "Snacks" });
            _productGroups.Add(new ProductGroup() { Id = 20, Name = "Time offers" });

            _products = Enumerable.Range(0, 18).Select(i => new Product()
            {
                Id = i + 1,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola {i + 1} 500ml",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                ProductType = (ProductType)random.Next(0, 3),
                PurchaseOptions = (PurchaseOptionType)random.Next(0, 2),
            }).ToList();
        }

        private List<ProductGroup> _productGroups;
        private List<Product> _products;

        public async Task<PagedList<ProductGroup>> GetProductGroupsAsync(ProductGroupsFilter filter)
        {
            var pagedList = new PagedList<ProductGroup>(_productGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<Product>> GetProductsAsync(ProductsFilter filter)
        {
            var pagedList = new PagedList<Product>(_products, new PaginationMetadata());

            return pagedList;
        }

        public async Task<Product> GetProductByIdAsync(int id, GetOptions? options = null)
        {
            return _products.Where(a => a.Id == id).FirstOrDefault();
        }

        public async Task<PagedList<BundledProduct>> GetBundledProducts(int id)
        {
            Random random = new Random();

            var bundledProducts = Enumerable.Range(0, 5).Select(i => new BundledProduct()
            {
                Id = i + 1,
                ProductId = random.Next(1, 5),
                Quantity = random.Next(1, 5),
                UnitPrice = random.Next(1, 5)
            }).ToList();

            var pagedList = new PagedList<BundledProduct>(bundledProducts, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationGroup>> GetApplicationGroupsAsync(ApplicationGroupsFilter filter)
        {
            List<ApplicationGroup> applicationGroups = Enumerable.Range(0, 4).Select(i => new ApplicationGroup()
            {
                Id = i + 1,
                Name = $"Category ({i + 1})"
            }).ToList();

            var pagedList = new PagedList<ApplicationGroup>(applicationGroups, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<Application>> GetApplicationsAsync(ApplicationsFilter filter)
        {
            Random random = new Random();

            List<Application> applications = Enumerable.Range(0, 1000).Select(i => new Application()
            {
                Id = i + 1,
                ApplicationCategoryId = random.Next(1, 5),
                Title = $"Fortnite ({i + 1})",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                ReleaseDate = DateTime.Now
            }).ToList();

            var pagedList = new PagedList<Application>(applications, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<ApplicationExecutable>> GetApplicationExecutablesAsync(ApplicationExecutablesFilter filter)
        {
            List<ApplicationExecutable> executables = new List<ApplicationExecutable>();
            executables.Add(new ApplicationExecutable() { Id = 1, Caption = "battlenet.exe" });
            executables.Add(new ApplicationExecutable() { Id = 2, Caption = "DOTA" });
            executables.Add(new ApplicationExecutable() { Id = 3, Caption = "Spotify" });
            executables.Add(new ApplicationExecutable() { Id = 4, Caption = "valve_steamclient.exe" });

            var pagedList = new PagedList<ApplicationExecutable>(executables, new PaginationMetadata());

            return pagedList;
        }

        public async Task<PagedList<PaymentMethod>> GetPaymentMethodsAsync(PaymentMethodsFilter filter)
        {
            List<PaymentMethod> paymentMethods = Enumerable.Range(0, 4).Select(i => new PaymentMethod()
            {
                Id = i + 1,
                Name = $"Payment method {i + 1}"
            }).ToList();

            var pagedList = new PagedList<PaymentMethod>(paymentMethods, new PaginationMetadata());

            return pagedList;
        }

        public Task<ApplicationImage> GetApplicationImageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationExecutableImage> GetApplicationExecutableImageAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}