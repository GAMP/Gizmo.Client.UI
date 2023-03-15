using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using Gizmo.Client.Interfaces;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class CultureService : ICultureService
    {
        /// <summary>
        /// Sets current UI culture.
        /// </summary>
        /// <param name="culture">Culture.</param>
        public async Task SetCurrentUICultureAsync(CultureInfo culture)
        {
            await Application.Current?.Dispatcher.InvokeAsync(new Action(() => { CultureInfo.CurrentUICulture = culture; }));
        }
    }
}
