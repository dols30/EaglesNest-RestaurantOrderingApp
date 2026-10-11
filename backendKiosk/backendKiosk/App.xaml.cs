using backendKiosk.Project.Controllers;
using backendKiosk.Project.Data;

namespace backendKiosk;

public partial class App : Application
{
    public static MenuController Menu { get; } = new(new MenuDatabase());
    public static CartController Cart { get; } = new();

    public App()
    {
        InitializeComponent();
        // The kiosk has a deliberate palette that stays readable under either OS theme.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell()) { Title = "Eagles Nest Restaurant" };
        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst || DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            window.Width = 1200;
            window.Height = 800;
            window.MinimumWidth = 390;
            window.MinimumHeight = 650;
#if MACCATALYST
            var initialWidth = 1200d;
            var initialHeight = 840d;
            bool lockForQa = false;
#if DEBUG
            // A reproducible native viewport for visual QA, with an isolated DB.
            var requested = Environment.GetEnvironmentVariable("EAGLESNEST_QA_WINDOW")?.Split('x');
            if (requested?.Length == 2 && double.TryParse(requested[0], out var qaWidth)
                && double.TryParse(requested[1], out var qaHeight) && qaWidth >= 390 && qaHeight >= 650)
            {
                initialWidth = qaWidth;
                initialHeight = qaHeight;
                lockForQa = true;
            }
#endif
            window.Created += (_, _) =>
            {
                // Mac Catalyst ignores Width/Height. Updating scene size bounds
                // after creation establishes a useful initial desktop window.
                window.MinimumWidth = window.MaximumWidth = initialWidth;
                window.MinimumHeight = window.MaximumHeight = initialHeight;
                if (!lockForQa) window.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(150), () =>
                {
                    window.MinimumWidth = 390;
                    window.MinimumHeight = 650;
                    window.MaximumWidth = window.MaximumHeight = double.PositiveInfinity;
                });
            };
#endif
        }
        return window;
    }
}
