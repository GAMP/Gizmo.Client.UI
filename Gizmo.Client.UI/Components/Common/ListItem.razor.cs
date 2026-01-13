using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Components
{
    public partial class ListItem : CustomDOMComponentBase
    {
        #region CONSTRUCTOR
        public ListItem()
        {
        }
        #endregion

        #region FIELDS

        private bool _isExpanded;

        private bool _isSelected;
        private bool _isActive;
        private bool _canExecute = true;

        private ICommand _previousCommand;

        #endregion

        #region PROPERTIES

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        protected List Parent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public NavLinkMatch Match { get; set; } = NavLinkMatch.All;

        [Parameter]
        public RenderFragment NestedList { get; set; }

        [Parameter]
        public bool HasBorder { get; set; }

        [Parameter]
        public string BorderColor { get; set; } = "#edeef2";

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        [Parameter]
        public string BackgroundImage { get; set; }

        [Parameter]
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;

                if (Parent != null)
                    _ = Parent.SetSelectedItem(this);
            }
        }

        [Parameter]
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                _ = IsExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }

        #endregion

        #region EVENTS

        protected async Task OnClickHandler(MouseEventArgs args)
        {
            if (IsDisabled)
                return;

            if (NestedList != null)
            {
                IsExpanded = !IsExpanded;
            }
            else if (Href != null)
            {
                await Parent?.SetClickedItem(this);
                NavigationManager.NavigateTo(Href);
            }
            else
            {
                if (Parent != null)
                    await Parent.SetClickedItem(this);
            }

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            await OnClick.InvokeAsync(args);
        }

        private async void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (Href != null)
            {
                if (IsActiveLink())
                    await Parent.SetSelectedItem(this);
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _canExecute = Command.CanExecute(CommandParameter);
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (IsDisabled || !_canExecute)
                return;

            if (_isSelected == selected)
                return;

            _isSelected = selected;

            StateHasChanged();
        }

        internal void SetActive(bool value)
        {
            if (IsDisabled || !_canExecute)
                return;

            if (_isActive == value)
                return;

            _isActive = value;

            StateHasChanged();
        }

        private bool IsActiveLink()
        {
            var relativePath = NavigationManager.ToBaseRelativePath(NavigationManager.Uri).ToLower();

            if (Match == NavLinkMatch.All)
                return relativePath == Href.ToLower();
            else
                return relativePath.StartsWith(Href.ToLower());
        }

        #endregion

        #region OVERRIDES

        protected override void OnParametersSet()
        {
            bool commandChanged = !EqualityComparer<ICommand>.Default.Equals(_previousCommand, Command);

            if (commandChanged)
            {
                if (_previousCommand != null)
                {
                    //Remove handler
                    _previousCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                }
                if (Command != null)
                {
                    //Add handler
                    Command.CanExecuteChanged += Command_CanExecuteChanged;
                }

                _previousCommand = Command;
            }

            base.OnParametersSet();
        }

        protected override async Task OnInitializedAsync()
        {
            if (Parent != null)
            {
                Parent.Register(this);

                if (Href != null)
                {
                    NavigationManager.LocationChanged += NavigationManager_LocationChanged;

                    if (IsActiveLink())
                        await Parent.SetSelectedItem(this);
                }
                else
                {
                    if (_isSelected)
                    {
                        await Parent.SetSelectedItem(this);
                    }
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                if (_previousCommand != null)
                {
                    //Remove handler
                    _previousCommand.CanExecuteChanged -= Command_CanExecuteChanged;
                }
                if (Parent != null)
                {
                    Parent.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-list-item")
                 .If("disabled", () => IsDisabled || !_canExecute)
                 .If("selected", () => _isSelected)
                 .If("active", () => _isActive)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"border: 1px solid {BorderColor}; border-radius: 0.4rem", () => HasBorder)
                 .If($"background: url('{BackgroundImage}') no-repeat; background-size: cover;", () => !string.IsNullOrEmpty(BackgroundImage))
                 .AsString();

        #endregion
    }
}
