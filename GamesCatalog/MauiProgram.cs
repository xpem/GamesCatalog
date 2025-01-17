using CommunityToolkit.Maui;
using GamesCatalog.ViewModels;
using GamesCatalog.Views;
using GamesCatalog.Views.IGDBSearch;
using Microsoft.Extensions.Logging;

namespace GamesCatalog
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMauiHandlers(handlers => { })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransientWithShellRoute<IGDBResults, IGDBResultsVM>(nameof(IGDBResults));

            return builder.Build();
        }
    }
}
