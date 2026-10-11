namespace backendKiosk;

using backendKiosk.Project.Views;
using backendKiosk.Project.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;

public partial class AppShell : Shell
{
    public AppShell()
    {
        try
        {
            Debug.WriteLine("AppShell constructor starting");
            InitializeComponent();
            RegisterRoutes();
            
            // Set up navigation failed handler
            Navigating += Shell_Navigating;
            Debug.WriteLine("AppShell constructor completed successfully");
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Error in AppShell constructor: {ex.Message}");
        }
    }

    private void Shell_Navigating(object? sender, ShellNavigatingEventArgs e)
    {
        Debug.WriteLine($"Shell navigating to: {e.Target.Location.OriginalString}");
    }

    private void RegisterRoutes()
    {
        try
        {
            Debug.WriteLine("Registering routes...");
            
            // Register routes for parameter passing
            Routing.RegisterRoute("itemAdd", typeof(ItemAddPage));
            
            // Register explicit routes for all pages
            Debug.WriteLine("Routes registered successfully");
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Error registering routes: {ex.Message}");
        }
    }
    
    // Helper methods to simplify navigation
    public async Task NavigateToCombo()
    {
        try
        {
            Debug.WriteLine("Beginning navigation to combo page");
            
            // Create and display the ComboBuilderPage directly
            var comboPage = new ComboBuilderPage();
            
            // Get the current navigation page
            var currentPage = Current.CurrentPage;
            Debug.WriteLine($"Current page: {currentPage?.GetType().Name}");
            
            // Push the new page onto the navigation stack using the Navigation property
            await Current.Navigation.PushModalAsync(comboPage);
            
            Debug.WriteLine("Navigation to combo page completed");
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"NavigateToCombo error: {ex}");
        }
    }
    
    public async Task NavigateToItemAdd(MenuItemType itemType)
    {
        try
        {
            Debug.WriteLine($"Beginning navigation to ItemAddPage with type: {itemType}");
            
            // Create the ItemAddPage directly
            var itemAddPage = new ItemAddPage();
            itemAddPage.ItemType = itemType.ToString();
            
            // Push the new page onto the navigation stack as a modal
            await Current.Navigation.PushModalAsync(itemAddPage);
            
            Debug.WriteLine("Navigation to ItemAddPage completed");
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"NavigateToItemAdd error: {ex}");
        }
    }
    
    public async Task NavigateToCheckout()
    {
        try
        {
            Debug.WriteLine("Beginning navigation to checkout page");
            
            // Create and display the CheckoutView directly
            var checkoutPage = new CheckoutView();
            
            // Push the new page onto the navigation stack
            await Current.Navigation.PushModalAsync(checkoutPage);
            
            Debug.WriteLine("Navigation to checkout page completed");
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"NavigateToCheckout error: {ex}");
        }
    }
    
    public async Task NavigateToDashboard()
    {
        try
        {
            Debug.WriteLine("Beginning navigation to dashboard page");
            
            // Create and display the MenuDashboardPage directly
            var dashboardPage = new MenuDashboardPage();
            
            // Push the new page onto the navigation stack
            await Current.Navigation.PushModalAsync(dashboardPage);
            
            Debug.WriteLine("Navigation to dashboard page completed");
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"NavigateToDashboard error: {ex}");
        }
    }
}
