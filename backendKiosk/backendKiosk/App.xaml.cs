using backendKiosk.Project.Controllers;

namespace backendKiosk
{
    public partial class App : Application
    {
        public static MenuController Menu;
        public static CartController Cart;
        
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new Project.Views.MainMenuPage());
        }

        static App()
        {
            Menu = new MenuController();
            Cart = new CartController();
        }
    }
}