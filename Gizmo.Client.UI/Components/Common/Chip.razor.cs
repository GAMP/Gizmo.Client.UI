using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Components
{
    public partial class Chip : CustomDOMComponentBase
    {
        #region FIELDS

        private bool _selected;
        private bool _isSelected;
        private bool _canExecute = true;

        private ICommand _previousCommand;

        #endregion

        #region PROPERTIES

        //[CascadingParameter]
        //protected ChipGroup ChipGroup { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool CanClose { get; set; }

        /// <summary>
        /// Gets or sets if element is disabled.
        /// </summary>
        [Parameter]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        [Parameter]
        public string Value { get; set; }

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

                //if (ChipGroup != null)
                //{
                //    ChipGroup.SelectItem(this, _isSelected);
                //}
            }
        }

        [Parameter]
        public ICommand Command { get; set; }

        [Parameter]
        public object CommandParameter { get; set; }

        [Parameter]
        public EventCallback<string> OnClose { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickChipHandler(MouseEventArgs args)
        {
            if (!IsDisabled)
            {
                //if (ChipGroup != null && !ChipGroup.IsDisabled)
                //{
                //    ChipGroup.SelectItem(this, !_selected);
                //}

                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }

            return Task.CompletedTask;
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _canExecute = Command.CanExecute(CommandParameter);
        }
        protected Task OnClickCloseButtonHandler(MouseEventArgs args)
        {
            return OnClose.InvokeAsync(Value);
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _selected = IsSelected;

            //if (ChipGroup != null)
            //{
            //    ChipGroup.Register(this);

            //    if (_selected)
            //    {
            //        ChipGroup.SelectItem(this, true);
            //    }
            //}
        }

        protected override async Task OnParametersSetAsync()
        {
            bool newCommand = !EqualityComparer<ICommand>.Default.Equals(_previousCommand, Command);

            if (newCommand)
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
            }

            _previousCommand = Command;

            await base.OnParametersSetAsync();
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
                //if (ChipGroup != null)
                //{
                //    ChipGroup.Unregister(this);
                //}
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region METHODS

        internal void SetSelected(bool selected)
        {
            if (IsDisabled || !_canExecute)
                return;

            if (_selected == selected)
                return;

            _selected = selected;
            IsSelected = _selected;

            StateHasChanged();
        }

        internal bool GetSelected()
        {
            return _selected;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-chip")
                 //.If("giz-chip--group-chip", () => ChipGroup != null)
                 .If("disabled", () => IsDisabled || !_canExecute)
                 .If("selected", () => _selected)
                 .AsString();

        #endregion

    }
}
