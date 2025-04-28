using backendKiosk.Project.Controllers;
using backendKiosk.Project.Data;
using System.Diagnostics;

namespace backendKiosk
{
    public partial class App : Application
    {
        public static MenuController Menu { get; private set; }
        public static CartController Cart { get; private set; }
        
        static App()
        {
            try
            {
                // Create the database first, then inject it into the MenuController
                MenuDatabase database = new MenuDatabase();
                Menu = new MenuController(database);
                Cart = new CartController();
                Debug.WriteLine("Static initialization complete: Menu and Cart controllers created successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in static constructor: {ex.Message}");
            }
        }
        
        public App()
        {
            try
            {
                InitializeComponent();
                MainPage = new AppShell();
                Debug.WriteLine("App initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in App constructor: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            try
            {
                Window window = base.CreateWindow(activationState);
                
                // Set window size for desktop platforms
                if (DeviceInfo.Platform == DevicePlatform.MacCatalyst || 
                    DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    window.Width = 1024;
                    window.Height = 768;
                }
                
                return window;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating window: {ex.Message}");
                return base.CreateWindow(activationState);
            }
        }
    }
}