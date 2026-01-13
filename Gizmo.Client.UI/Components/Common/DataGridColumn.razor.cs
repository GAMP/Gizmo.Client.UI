using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// DataGrid column.
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    public partial class DataGridColumn<TItemType> : CustomDOMComponentBase
    {
        #region CONSTRUCTOR

        #region PUBLIC

        public DataGridColumn() : base()
        {
        }

        #endregion

        #endregion

        #region FIELDS

        private bool _isVisible  = true;

        #endregion

        #region PROPERTIES

        #region PUBLIC

        /// <summary>
        /// Gets or sets field name of the data object.
        /// </summary>
        [Parameter]
        public string Field
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets parent grid.
        /// </summary>
        [CascadingParameter(Name = "Parent")]
        public DataGrid<TItemType> Parent
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets header template.
        /// </summary>
        [Parameter]
        public RenderFragment HeaderTemplate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets template header.
        /// </summary>
        [Parameter]
        public RenderFragment<TItemType> CellTemplate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets template header.
        /// </summary>
        [Parameter]
        public RenderFragment<TItemType> CellEditTemplate
        {
            get; set;
        }

        [Parameter]
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible == value)
                    return;

                _isVisible = value;

                Parent?.Refresh();
            }
        }

        [Parameter]
        public bool CanChangeVisibility { get; set; } = true;

        [Parameter]
        public bool CanSort { get; set; }

        [Parameter]
        public Func<SortDirections, IEnumerable<TItemType>> SortFunction { get; set; }

        [Parameter]
        public TextAlignments TextAlignment { get; set; }

        public bool IsSorted { get; set; }

        public SortDirections SortDirection { get; set; }

        #endregion

        #endregion

        #region OVERRIDES

        #region PROTECTED

        protected override void OnInitialized()
        {
            Parent?.AddColumn(this);
        }

        #endregion

        #region PUBLIC

        public override void Dispose()
        {
            base.Dispose();

            Parent?.RemoveColumn(this);
        }

        #endregion

        #endregion

        #region EVENT HANDLERS

        #region PRIVATE

        protected void OnHeaderMouseEvent(MouseEventArgs args)
        {
            if (Parent != null)
                Parent.OnHeaderRowMouseEvent(args, this);
        }

        protected async Task ContextMenuHandler(MouseEventArgs args)
        {
            if (Parent != null)
            {
                if (args.Button == 2)
                {
                    await Parent.RightClickItem(default(TItemType));

                    //if (Parent.ContextMenu != null)
                    //{
                    //    await Parent.OpenContextMenu(args.ClientX, args.ClientY);
                    //}
                }
            }
        }

        #endregion

        #endregion

        protected string ClassName => new ClassMapper()
                 .Add("giz-data-grid-column")
                 .AsString();

        protected string StyleValue => new StyleMapper()
                 .If($"cursor: pointer;", () => CanSort)
                 .AsString();

        protected string HeaderStyleValue => new StyleMapper()
                 .If($"justify-content: {TextAlignment.ToDescriptionString()};", () => TextAlignment != TextAlignments.Left)
                 .AsString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
