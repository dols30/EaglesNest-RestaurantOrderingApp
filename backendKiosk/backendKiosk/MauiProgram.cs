using Microsoft.Extensions.Logging;
using backendKiosk.Project.Controllers;
using backendKiosk.Project.Data;
using backendKiosk.Project.Views;
using SQLitePCL;

namespace backendKiosk
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Initialize SQLite
            Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register Services
            builder.Services.AddSingleton<MenuDatabase>();
            builder.Services.AddSingleton<MenuController>();
            builder.Services.AddSingleton<CartController>();

            // Register Pages
            builder.Services.AddSingleton<KioskStartPage>();
            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<ComboBuilderPage>();
            builder.Services.AddTransient<CheckoutView>();
            builder.Services.AddTransient<ItemAddPage>();
            builder.Services.AddTransient<MenuDashboardPage>();
            builder.Services.AddTransient<MenuItemEditPage>();


            return builder.Build();
        }
    }
}