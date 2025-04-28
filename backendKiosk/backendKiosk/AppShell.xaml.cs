using backendKiosk.Project.Views;

namespace backendKiosk;

public partial class AppShell : Shell
{
	public AppShell()
	{
        

        Routing.RegisterRoute(nameof(MainMenuPage), typeof(MainMenuPage)); // Good practice to register main page too
        Routing.RegisterRoute(nameof(ComboBuilderPage), typeof(ComboBuilderPage));
        Routing.RegisterRoute(nameof(ItemAddPage), typeof(ItemAddPage));
        Routing.RegisterRoute(nameof(CheckoutView), typeof(CheckoutView));

        // If you had other pages like an Order Confirmation page, register them here too:
        // Routing.RegisterRoute(nameof(OrderConfirmationPage), typeof(OrderConfirmationPage));
    }
}
