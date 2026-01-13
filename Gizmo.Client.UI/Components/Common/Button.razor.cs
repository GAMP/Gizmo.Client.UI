using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Components
{
    public partial class Button : ButtonBase
    {
        #region CONSTRUCTOR
        public Button()
        {
        }
        #endregion

        #region FIELDS

        private bool _isSelected;

        private bool _canExecute = true;

        private ICommand _previousCommand;

        protected bool _shouldRender;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Get parent ButtonGroup if exists.
        /// </summary>
        [CascadingParameter]
        protected ButtonGroup ButtonGroup { get; set; }

        #region PUBLIC

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        [Parameter]
        public bool IsFullWidth { get; set; }

        /// <summary>
        /// Gets or sets html element value.
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the label of the button.
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the content of the button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Icons? LeftSVGIcon { get; set; }

        [Parameter]
        public Icons? RightSVGIcon { get; set; }

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

                if (ButtonGroup != null)
                {
                    ButtonGroup.SelectItem(this, _isSelected);
                }
            }
        }

        [Parameter]
        public bool StopPropagation { get; set; }

        [Parameter]
        public decimal Progress { get; set; }

        [Parameter]
        public bool IsProgressIndeterminate { get; set; }

        #endregion

        #endregion

        #region EVENTS

        protected Task OnClickButtonHandler(MouseEventArgs args)
        {
            if (ButtonGroup != null && !ButtonGroup.IsDisabled)
            {
                ButtonGroup.SelectItem(this, !_selected);
            }

            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }

            return Task.CompletedTask;
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            _canExecute = Command.CanExecute(CommandParameter);
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _selected = IsSelected;

            if (ButtonGroup != null)
            {
                ButtonGroup.Register(this);

                if (_selected)
                {
                    ButtonGroup.SelectItem(this, true);
                }
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<string>(nameof(Value), out var newValue))
            {
                var valueChanged = Value != newValue;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<RenderFragment>(nameof(ChildContent), out var newChildContent))
            {
                var valueChanged = !EqualityComparer<RenderFragment>.Default.Equals(ChildContent, newChildContent);
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<string>(nameof(Text), out var newText))
            {
                var valueChanged = Text != newText;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<decimal>(nameof(Progress), out var newProgress))
            {
                var valueChanged = Progress != newProgress;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<bool>(nameof(IsSelected), out var newIsSelected))
            {
                var valueChanged = IsSelected != newIsSelected;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<ButtonColors>(nameof(Color), out var newColor))
            {
                var valueChanged = Color != newColor;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<ButtonVariants>(nameof(Variant), out var newVariant))
            {
                var valueChanged = Variant != newVariant;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<bool>(nameof(IsDisabled), out var newIsDisabled))
            {
                var valueChanged = IsDisabled != newIsDisabled;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<bool>(nameof(IsFullWidth), out var newIsFullWidth))
            {
                var valueChanged = IsFullWidth != newIsFullWidth;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<bool>(nameof(StopPropagation), out var newStopPropagation))
            {
                var valueChanged = StopPropagation != newStopPropagation;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<string>(nameof(Label), out var newLabel))
            {
                var valueChanged = Label != newLabel;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<Icons?>(nameof(LeftSVGIcon), out var newLeftSVGIcon))
            {
                var valueChanged = LeftSVGIcon != newLeftSVGIcon;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            if (parameters.TryGetValue<Icons?>(nameof(RightSVGIcon), out var newRightSVGIcon))
            {
                var valueChanged = RightSVGIcon != newRightSVGIcon;
                if (valueChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
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

                if (ButtonGroup != null)
                {
                    ButtonGroup.Unregister(this);
                }
            }
            catch (Exception) { }

            base.Dispose();
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        /// <summary>
        /// Called by the parent ButtonGroup to set the button's selected state.
        /// </summary>
        /// <param name="selected">The selected state of the button.</param>
        public override void SetSelected(bool selected)
        {
            if (IsDisabled || !_canExecute)
                return;

            if (_selected == selected)
                return;

            _selected = selected;
            IsSelected = _selected;

            _shouldRender = true;

            StateHasChanged();
        }

        /// <summary>
        /// Called by the parent ButtonGroup to get the button' selected state.
        /// </summary>
        /// <returns></returns>
        public override bool GetSelected()
        {
            return _selected;
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-button")
                 .Add($"giz-button--{Size.ToDescriptionString()}")
                 .Add($"{Color.ToDescriptionString()}")
                 .If("giz-button--group-button", () => ButtonGroup != null)
                 .If("giz-button--fill", () => ButtonGroup == null && Variant == ButtonVariants.Fill)
                 .If("giz-button--outline", () => ButtonGroup == null && Variant == ButtonVariants.Outline)
                 .If("giz-button--text", () => ButtonGroup == null && Variant == ButtonVariants.Text)
                 .If("giz-button--progress", () => ButtonGroup == null && Variant == ButtonVariants.Progress)
                 .If("giz-button--progress-indeterminate", () => ButtonGroup == null && IsProgressIndeterminate)
                 .If("giz-button--full-width", () => IsFullWidth)
                 .If("giz-button--shadow", () => HasShadow)
                 .If("giz-button--icon", () => ChildContent == null && string.IsNullOrEmpty(Text) && LeftSVGIcon.HasValue)
                 .If("disabled", () => IsDisabled || !_canExecute)
                 .If("selected", () => _selected)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 //TODO: A .If($"padding: 0 0.6rem;", () => string.IsNullOrEmpty(RightIcon) && !RightSVGIcon.HasValue && ChildContent == null)
                 .AsString();

        protected string ButtonIconLeft => new ClassMapper()
                .If("giz-button__icon-left", () => ChildContent != null)
                .AsString();

        protected string ButtonIconRight => new ClassMapper()
                .Add("giz-button__icon-right")
                .AsString();

        #endregion

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (!firstRender)
        //    {
        //        _shouldRender = false;
        //        await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
        //    }

        //    await base.OnAfterRenderAsync(firstRender);
        //}
    }
}
