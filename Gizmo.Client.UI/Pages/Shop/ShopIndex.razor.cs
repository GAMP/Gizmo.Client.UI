using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(2)]
    [Route("/shop")]
    public partial class ShopIndex : ComponentBase
    {
        public ShopIndex()
        {
        }

        #region FIELDS
        private List<string> _sortOptions;
        private int? _selectedProductGroupId;
        #endregion

        #region PROPERTIES

        [Inject]
        ShopPageService ShopService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        public string SelectedSortOption { get; set; } = "Name";

        public List<string> SortOptions
        {
            get
            {
                if (_sortOptions == null)
                {
                    _sortOptions = new List<string>();
                    _sortOptions.Add("Name");
                    _sortOptions.Add("Price");
                }

                return _sortOptions;
            }
            set
            {
                _sortOptions = value;
            }
        }

        public bool PaymentMethodSelectorIsOpen { get; set; }

        #endregion

        private Task PlaceOrder()
        {
            PaymentMethodSelectorIsOpen = true;

            StateHasChanged();

            return Task.CompletedTask;
        }

        private Task SelectProductGroup(int? productGroupId)
        {
            _selectedProductGroupId = productGroupId;

            StateHasChanged();

            return Task.CompletedTask;
        }

        #region METHODS

        public Task AddProduct(int id)
        {
            return UserCartService.AddProductAsyc(id);
        }

        public void SelectPaymentMethod(int id)
        {
            StateHasChanged();
        }

        #endregion

    }
}