using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Domain.Settings;
using Sidekick.Domain.Views;
using Sidekick.Presentation.Wpf.About;
using Sidekick.Presentation.Wpf.Cheatsheets;
using Sidekick.Presentation.Wpf.Initialization;
using Sidekick.Presentation.Wpf.Settings;
using Sidekick.Presentation.Wpf.Setup;
using Sidekick.Presentation.Wpf.Views.ApplicationLogs;
using Sidekick.Presentation.Wpf.Views.MapInfo;
using Sidekick.Presentation.Wpf.Views.Prices;

namespace Sidekick.Presentation.Wpf.Views
{
    public class ViewInstance : IDisposable
    {
        private static readonly Dictionary<View, Type> ViewTypes = new Dictionary<View, Type>() {
            { View.About, typeof(AboutView) },
            { View.Initialization, typeof(InitializationView) },
            { View.Map, typeof(MapInfoView) },
            { View.League, typeof(LeagueView) },
            { View.Logs, typeof(ApplicationLogsView) },
            { View.ParserError, typeof(Errors.ParserError) },
            { View.Price, typeof(PriceView) },
            { View.Settings, typeof(SettingsView) },
            { View.Setup, typeof(SetupView) },
        };

        private readonly ViewLocator viewLocator;

        public ViewInstance(ViewLocator viewLocator, IServiceProvider serviceProvider, View view, params object[] args)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ViewInstance>>();
            var dispatcher = serviceProvider.GetRequiredService<Dispatcher>();

            if (!ViewTypes.ContainsKey(view))
            {
                logger.LogError($"The view {view} could not be opened.");
                return;
            }

            this.viewLocator = viewLocator;
            View = view;
            Scope = serviceProvider.CreateScope();

            // Still needed for localization of league overlay models
            var settings = Scope.ServiceProvider.GetRequiredService<ISidekickSettings>();
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(settings.Language_UI);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(settings.Language_UI);

            // View initialization and show
            WpfView = (ISidekickView)Scope.ServiceProvider.GetRequiredService(ViewTypes[view]);
            WpfView.Closed += View_Closed;

            dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    await WpfView.Open(args);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"The view {view} could not be opened. {e.Message}");
                    Dispose();
                }
            });
        }

        private IServiceScope Scope { get; set; }

        public ISidekickView WpfView { get; set; }

        public View View { get; }

        private void View_Closed(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (WpfView != null)
            {
                WpfView.Closed -= View_Closed;
                WpfView.Close();
            }
            viewLocator.Views.Remove(this);
            Scope?.Dispose();
        }
    }
}