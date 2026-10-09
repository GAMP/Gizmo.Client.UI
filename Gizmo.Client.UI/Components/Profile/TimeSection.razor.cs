using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Components
{
    public partial class TimeSection : ShellComponentBase
    {
        private static readonly TimeSpan SHORTEST = TimeSpan.FromHours(4);
        private static readonly TimeSpan LONGEST = TimeSpan.FromHours(48);
        private const double LABEL_WIDTH = 14;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject] ILocalizationService LocalizationService { get; set; }
        [Inject] NavigationService NavigationService { get; set; }
        [Inject] TimeProductsViewState ViewState { get; set; }
        [Inject] TimeProductsViewService TimeProductsService { get; set; }
        [Inject] ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }
        [Inject] UserBalanceViewState Balance { get; set; }
        [Inject] UserProductViewStateLookupService ProductLookup { get; set; }
        [Inject] ILogger<TimeSection> Logger { get; set; }

        private readonly Dictionary<int, UserProductViewState> _details = new();
        private readonly HashSet<int> _detailsLoading = new();
        private TimePlan _plan;

        private bool ProductDetailsNavigationEnabled => ProductDetailsPageViewState.ProductDetailsNavigationEnabled;

        private IReadOnlyList<TimeProductViewState> OrderedTimeProducts => ViewState.IsInitialized == true
            ? ViewState.TimeProducts
                .OrderBy(a => a.ActivationOrder.HasValue ? 0 : 1)
                .ThenBy(a => a.ActivationOrder)
                .ToList()
            : Array.Empty<TimeProductViewState>();

        private string HoursMinutesFormat =>
            LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_PRODUCT_HOURS_MINUTES), "{0}", "{1}");

        private string Headline => Balance.Time.HasValue
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TIME_PLAY_UNTIL, _plan?.EndText ?? Clock(DateTime.Now, DateTime.Now + TimeLeft.NotNegative(Balance.Time)))
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TIME_UNLIMITED);

        private sealed record Tick(string At, string Label);

        private sealed record Segment(string Kind, string From, string Width, bool ShowLabel, string Name, string Left);

        private sealed record TimePlan(IReadOnlyList<Tick> Ticks, IReadOnlyList<Segment> Segments, string Now, string End, string EndText, bool HasRate, bool EndFirst);

        private sealed record Span(string Kind, DateTime From, DateTime To, string Name, string Left);

        private TimePlan BuildPlan()
        {
            if (ViewState.IsInitialized != true || !Balance.Time.HasValue)
                return null;

            var now = DateTime.Now;
            var spans = Spans(now);

            if (spans is null)
                return null;

            var until = now + TimeLeft.NotNegative(Balance.Time);
            var cursor = spans.Count > 0 ? spans[^1].To : now;

            if (until > cursor)
                spans.Add(new Span("rate", cursor, until, null, null));

            var last = until > cursor ? until : cursor;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Kind);
            var end = new DateTime(last.Year, last.Month, last.Day, last.Hour, 0, 0, last.Kind);

            if (end < last)
                end = end.AddHours(1);

            if (end - start < SHORTEST)
                end = start + SHORTEST;

            if (end - start > LONGEST)
                end = start + LONGEST;

            var total = (end - start).TotalMinutes;
            double At(DateTime t) => Math.Clamp((t - start).TotalMinutes / total * 100, 0, 100);
            static string Pct(double value) => CssValue.Percent(value);

            var hours = (int)Math.Round((end - start).TotalHours);
            var step = hours <= 14 ? 1 : hours <= 28 ? 2 : 4;
            var ticks = new List<Tick>();

            for (var h = 0; h <= hours; h += step)
            {
                var t = start.AddHours(h);
                ticks.Add(new Tick(Pct(At(t)), t.ToString("HH", CultureInfo.CurrentCulture)));
            }

            var segments = spans
                .Where(a => a.To > a.From)
                .Select(a =>
                {
                    var from = At(a.From);
                    var width = At(a.To) - from;

                    return new Segment(a.Kind, Pct(from), Pct(width), a.Name is not null && width >= LABEL_WIDTH, a.Name, a.Left);
                })
                .Where(a => a.Width != "0%")
                .ToList();

            return new TimePlan(ticks, segments, Pct(At(now)), Pct(At(last)), Clock(now, last), spans.Any(a => a.Kind == "rate"), At(last) < 30);
        }

        private List<Span> Spans(DateTime now)
        {
            var spans = new List<Span>();
            var format = HoursMinutesFormat;
            var cursor = now;

            foreach (var product in TimeQueue.Queue(ViewState.TimeProducts))
            {
                var usable = TimeQueue.UsableText(product);

                if (TimeQueue.Minutes(usable, format) is not int minutes)
                    return null;

                if (minutes <= 0)
                    continue;

                var from = cursor;
                var to = from.AddMinutes(minutes);

                if (product.ProductId is int id && _details.GetValueOrDefault(id) is { } details &&
                    TimeQueue.Window(details.TimeProduct?.UsageAvailability, cursor) is { } window)
                {
                    var open = cursor.Date + window.Start;
                    var close = cursor.Date + window.End;

                    if (close <= open)
                        close = close.AddDays(1);

                    if (cursor >= close)
                    {
                        open = open.AddDays(1);
                        close = close.AddDays(1);
                    }

                    if (cursor < open)
                    {
                        spans.Add(new Span("rate", cursor, open, null, null));
                        from = open;
                    }

                    to = from.AddMinutes(minutes) < close ? from.AddMinutes(minutes) : close;
                }

                var kind = spans.Any(a => a.Kind != "rate") || from > now ? "next" : "now";

                spans.Add(new Span(kind, from, to, TimeProductText.Name(product, GrafitLocalization), usable));
                cursor = to;
            }

            return spans;
        }

        private void Rebuild()
        {
            RequestDetails();
            _plan = BuildPlan();
        }

        private void RequestDetails()
        {
            if (ViewState.IsInitialized != true)
                return;

            foreach (var product in TimeQueue.Queue(ViewState.TimeProducts))
            {
                if (product.ProductId is not int productId || _details.ContainsKey(productId) || !_detailsLoading.Add(productId))
                    continue;

                DispatchWorkflow(async () =>
                {
                    try
                    {
                        _details[productId] = await ProductLookup.GetStateAsync(productId);
                    }
                    catch (Exception exception)
                    {
                        Logger.LogWarning(exception, "Could not load the time product {ProductId} for the time axis.", productId);
                        _details[productId] = null;
                    }

                    _plan = BuildPlan();
                    StateHasChanged();
                });
            }
        }

        private void OnTimeChanged(object sender, EventArgs e) => DispatchWorkflow(() =>
        {
            Rebuild();
            StateHasChanged();
            return Task.CompletedTask;
        });

        private static string Clock(DateTime now, DateTime time) => time - now < TimeSpan.FromHours(20)
            ? time.ToString("t", CultureInfo.CurrentCulture)
            : $"{time.ToString("d MMM", CultureInfo.CurrentCulture)}, {time.ToString("t", CultureInfo.CurrentCulture)}";

        private void OpenDetails(int productId)
        {
            if (!ProductDetailsNavigationEnabled)
                return;

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={productId}");
        }

        protected override void OnInitialized()
        {
            ViewState.OnChange += OnTimeChanged;
            Balance.OnChange += OnTimeChanged;
            this.SubscribeChange(ProductDetailsPageViewState);
            Rebuild();

            DispatchWorkflow(() => TimeProductsService.LoadAsync());

            base.OnInitialized();
        }

        public override void Dispose()
        {
            ViewState.OnChange -= OnTimeChanged;
            Balance.OnChange -= OnTimeChanged;
            this.UnsubscribeChange(ProductDetailsPageViewState);

            base.Dispose();
        }
    }
}
