using Microsoft.Extensions.Logging;
using backendKiosk.Project.Controllers;
using backendKiosk.Project.Data;
using backendKiosk.Project.Views;
using SQLitePCL;
#if MACCATALYST
using Microsoft.Maui.Platform;
#endif

namespace backendKiosk
{
    public static class MauiProgram
    {
#if MACCATALYST
        private static void StyleKioskButton(Microsoft.Maui.Handlers.IButtonHandler handler, IButton button)
        {
            handler.PlatformView.PreferredBehavioralStyle = UIKit.UIBehavioralStyle.Pad;
            handler.PlatformView.Configuration = null;
            handler.PlatformView.UpdateBackground(button.Background);
            if (button is ITextStyle textStyle)
            {
                handler.PlatformView.UpdateTextColor(textStyle);
                Microsoft.Maui.Handlers.ButtonHandler.MapFont(handler, textStyle);
            }
        }
#endif

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

#if IOS || MACCATALYST
            // Remove UIKit's white inset border so the XAML field surface is respected.
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("KioskEntry", (handler, view) =>
            {
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            });
#endif

#if MACCATALYST
            // Keep touch-sized kiosk buttons while the window and text use the
            // unscaled Mac idiom. macOS's compact bezel would otherwise clip them.
            Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("KioskButton", StyleKioskButton);
            Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping(nameof(IView.Background), StyleKioskButton);
            Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping(nameof(ITextStyle.TextColor), StyleKioskButton);
#endif

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