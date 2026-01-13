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
    public partial class IconButton : ButtonBase
    {
        #region FIELDS

        private bool _canExecute = true;

        private ICommand _previousCommand;

        #endregion

        #region PROPERTIES

        [Parameter]
        public ButtonVariants Variant { get; set; } = ButtonVariants.Fill;

        /// <summary>
        /// Inline label of Button.
        /// </summary>
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Icons? SVGIcon { get; set; }

        [Parameter]
        public string IconColor { get; set; }

        [Parameter]
        public string IconBackgroundColor { get; set; }

        [Parameter]
        public int? ZIndex { get; set; }

        [Parameter]
        public string Tooltip { get; set; }

        [Parameter]
        public TooltipOpenDirections TooltipOpenDirection { get; set; } = TooltipOpenDirections.Top;

        [Parameter]
        public Visibilities Visibility { get; set; } = Visibilities.Default;

        [Parameter]
        public bool StopPropagation { get; set; }

        #endregion

        #region EVENTS

        protected Task OnClickButtonHandler(MouseEventArgs args)
        {
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
            }
            catch (Exception) { }

            base.Dispose();
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-icon-button")
                 .Add($"giz-icon-button--{Size.ToDescriptionString()}")
                 .Add($"{Color.ToDescriptionString()}")
                 .If("giz-icon-button--fill", () => Variant == ButtonVariants.Fill)
                 .If("giz-icon-button--outline", () => Variant == ButtonVariants.Outline)
                 .If("giz-icon-button--text", () => Variant == ButtonVariants.Text)
                 .If("giz-icon-button--shadow", () => HasShadow)
                 .If("disabled", () => IsDisabled || !_canExecute)
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"z-index: {ZIndex}", () => ZIndex.HasValue)
                 .If($"visibility: {Visibility.ToDescriptionString()}", () => Visibility != Visibilities.Default)
                 .AsString();

        protected string ButtonIcon => new ClassMapper()
                 .AsString();

        protected string TooltipClassName => new ClassMapper()
                 .Add("giz-tooltip")
                 .Add($"giz-tooltip--{TooltipOpenDirection.ToDescriptionString()}")
                 .AsString();

        #endregion

    }
}
