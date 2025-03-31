using CommunityToolkit.Maui;
using GamesCatalog.ViewModels;
using GamesCatalog.ViewModels.IGDBSearch;
using GamesCatalog.Views;
using GamesCatalog.Views.IGDBSearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repo;
using Services;

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
                    fonts.AddFont("Free-Solid-900.otf", "FontAwesomeIcons");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            if (!System.IO.Directory.Exists(GameService.ImagesPath))
                System.IO.Directory.CreateDirectory(GameService.ImagesPath);

            builder.Services.AddTransientWithShellRoute<IGDBResults, IGDBResultsVM>(nameof(IGDBResults));
            builder.Services.AddTransientWithShellRoute<AddGame, AddGameVM>(nameof(AddGame));
            builder.Services.AddTransientWithShellRoute<Main, MainVM>(nameof(Main));

            builder.Services.AddDbContextFactory<DbCtx>(options =>
            options.UseSqlite($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GamesCatalog.db")}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            builder.Services.Services();
            builder.Services.Repositories();

            return builder.Build();
        }

        public static IServiceCollection Repositories(this IServiceCollection services)
        {
            services.AddScoped<IGameRepo, GameRepo>();
            return services;
        }

        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<IBuildDbService, BuildDbService>();
            services.AddScoped<IGameService, GameService>();
            return services;
        }
    }
}
