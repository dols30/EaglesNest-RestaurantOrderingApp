using backendKiosk.Project.Controllers;
using backendKiosk.Project.Models;
using System.Collections.ObjectModel;

namespace backendKiosk.Project.Views;

public partial class MainMenuPage : ContentPage
{
	bool CartShown = true;
	ObservableCollection<CartPopupItemView> view_items;

	public MainMenuPage()
	{
		try
		{
			InitializeComponent();
			collCart.IsVisible = true;

			view_items = new();
			
			RefreshCart();
			
			System.Diagnostics.Debug.WriteLine("MainMenuPage initialized successfully");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error initializing MainMenuPage: {ex}");
		}
	}
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            RefreshCart();
            System.Diagnostics.Debug.WriteLine("MainMenuPage OnAppearing - RefreshCart called");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnAppearing: {ex}");
        }
    }
    
    private void RefreshCart()
    {
        try
        {
            if (view_items == null)
            {
                view_items = new ObservableCollection<CartPopupItemView>();
            }
            
            view_items.Clear();
            
            if (App.Cart == null)
            {
                System.Diagnostics.Debug.WriteLine("App.Cart is null in RefreshCart!");
                return;
            }
            
            var controller_items = App.Cart.GetCartItems();
            System.Diagnostics.Debug.WriteLine($"Cart items count: {controller_items?.Count ?? 0}");

            foreach (var item in controller_items)
            {
                CartPopupItemView cartPopupItemView = new CartPopupItemView();
                cartPopupItemView.Item = item;
                view_items.Add(cartPopupItemView);
            }

            if (collCart != null)
            {
                collCart.ItemsSource = view_items;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("collCart is null in RefreshCart!");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RefreshCart error: {ex}");
            MainThread.BeginInvokeOnMainThread(async () => {
                await DisplayAlert("Error", $"Failed to refresh cart: {ex.Message}", "OK");
            });
        }
    }

	private void ButtonToggleCart(object sender, EventArgs e)
	{
		try
		{
			if (CartShown)
			{
				CartShown = false;
				grdPageFrame.ColumnDefinitions[1].Width = 80;
				collCart.IsVisible = false;
				layoutCartControls.IsVisible = false;
			}
			else
			{
				CartShown = true;
				grdPageFrame.ColumnDefinitions[1].Width = 300;
				collCart.IsVisible = true;
				layoutCartControls.IsVisible = true;
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonToggleCart error: {ex}");
		}
	}

	private async void ButtonBuildCombo(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonBuildCombo clicked - creating new ComboBuilderPage");
			// Create and display ComboBuilderPage directly
			var comboPage = new ComboBuilderPage();
			// Try non-modal navigation
			await Navigation.PushAsync(comboPage); 
			System.Diagnostics.Debug.WriteLine("Navigation to ComboBuilderPage complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonBuildCombo error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private void ButtonDeleteCartItems(object sender, EventArgs e)
	{
		try
		{
			ObservableCollection<CartPopupItemView> selected = new();

			foreach (var selected_item in collCart.SelectedItems)
			{
				CartPopupItemView? item = (selected_item as CartPopupItemView);
				if (item != null && item.Item != null)
				{
					App.Cart.RemoveItem(item.Item.ID);
					selected.Add(item);
				}
			}

			foreach (var item in selected)
				view_items.Remove(item);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonDeleteCartItems error: {ex}");
		}
	}

	private async void ButtonCheckoutCart(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonCheckoutCart clicked - creating new CheckoutView");
			// Create and display CheckoutView directly
			var checkoutPage = new CheckoutView();
			await Navigation.PushModalAsync(checkoutPage);
			System.Diagnostics.Debug.WriteLine("Navigation to CheckoutView complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonCheckoutCart error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private async void ButtonAddEntree(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonAddEntree clicked - creating new ItemAddPage");
			// Create and set up ItemAddPage directly
			var itemAddPage = new ItemAddPage();
			itemAddPage.ItemType = MenuItemType.ENTREE.ToString();
			await Navigation.PushAsync(itemAddPage);
			System.Diagnostics.Debug.WriteLine("Navigation to ItemAddPage (Entree) complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonAddEntree error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private async void ButtonAddSide(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonAddSide clicked - creating new ItemAddPage");
			// Create and set up ItemAddPage directly
			var itemAddPage = new ItemAddPage();
			itemAddPage.ItemType = MenuItemType.SIDE.ToString();
			await Navigation.PushAsync(itemAddPage);
			System.Diagnostics.Debug.WriteLine("Navigation to ItemAddPage (Side) complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonAddSide error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private async void ButtonAddDrink(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonAddDrink clicked - creating new ItemAddPage");
			// Create and set up ItemAddPage directly
			var itemAddPage = new ItemAddPage();
			itemAddPage.ItemType = MenuItemType.DRINK.ToString();
			await Navigation.PushAsync(itemAddPage);
			System.Diagnostics.Debug.WriteLine("Navigation to ItemAddPage (Drink) complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonAddDrink error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private async void ButtonAddDessert(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ButtonAddDessert clicked - creating new ItemAddPage");
			// Create and set up ItemAddPage directly
			var itemAddPage = new ItemAddPage();
			itemAddPage.ItemType = MenuItemType.DESSERT.ToString();
			await Navigation.PushAsync(itemAddPage);
			System.Diagnostics.Debug.WriteLine("Navigation to ItemAddPage (Dessert) complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ButtonAddDessert error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}

	private async void ManageMenu_Clicked(object sender, EventArgs e)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("ManageMenu_Clicked - creating new MenuDashboardPage");
			// Create and display MenuDashboardPage directly
			var dashboardPage = new MenuDashboardPage();
			await Navigation.PushModalAsync(dashboardPage);
			System.Diagnostics.Debug.WriteLine("Navigation to MenuDashboardPage complete");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ManageMenu_Clicked error: {ex}");
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
		}
	}
}