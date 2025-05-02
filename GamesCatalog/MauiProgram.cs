using ApiRepo;
using CommunityToolkit.Maui;
using GamesCatalog.ViewModels;
using GamesCatalog.ViewModels.Game;
using GamesCatalog.ViewModels.IGDBSearch;
using GamesCatalog.Views;
using GamesCatalog.Views.Game;
using GamesCatalog.Views.Game.IGDBSearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Handlers;
using Repo;
using Services;
using Services.Interfaces;

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

            builder.Services.ShellRoutes();

            builder.Services.AddDbContextFactory<DbCtx>(options =>
            options.UseSqlite($"Filename={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GamesCatalog.db")}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


            //FOR LOCAL TESTS
            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
            {
                DeviceHandler.CurrentDevice = DeviceHandler.LocalTestDevice.Emulator;
                DeviceHandler.Url = "http://10.0.2.2:5048";
            }
            else
            {
                DeviceHandler.CurrentDevice = DeviceHandler.LocalTestDevice.Windows;
                DeviceHandler.Url = "http://localhost:5048";
            }

            builder.Services.AddTransient<AppShell, AppShellVM>();
            builder.Services.AddSingleton<UserStateVM>();

            builder.Services.Services();
            builder.Services.Repositories();
            builder.Services.ApiRepositories();

            return builder.Build();
        }

        public static IServiceCollection ShellRoutes(this IServiceCollection services)
        {
            services.AddTransientWithShellRoute<IGDBResults, IGDBResultsVM>(nameof(IGDBResults));
            services.AddTransientWithShellRoute<AddGame, AddGameVM>(nameof(AddGame));
            services.AddTransientWithShellRoute<Main, MainVM>(nameof(Main));
            services.AddTransientWithShellRoute<GameList, GameListVM>(nameof(GameList));
            services.AddTransientWithShellRoute<SignIn, SignInVM>(nameof(SignIn));
            services.AddTransientWithShellRoute<SignUp, SignUpVM>(nameof(SignUp));
            services.AddTransientWithShellRoute<UpdatePassword, UpdatePasswordVM>(nameof(UpdatePassword));
            return services;
        }

        public static IServiceCollection ApiRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserApiRepo, UserApiRepo>();
            services.AddScoped<IIGDBGamesAPIRepo, IGDBGamesAPIRepo>();
            services.AddScoped<IGameApiRepo, GameApiRepo>();

            return services;
        }

        public static IServiceCollection Repositories(this IServiceCollection services)
        {
            services.AddScoped<IGameRepo, GameRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IApiOperationRepo, ApiOperationRepo>();

            return services;
        }

        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<IBuildDbService, BuildDbService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIGDBGamesApiService, IGDBGamesApiService>();
            services.AddScoped<IGameApiService, GameApiService>();
            services.AddScoped<IApiOperationService, ApiOperationService>();

            return services;
        }
    }
}
