using System.Collections.ObjectModel;

namespace backendKiosk.Project.Views;

public partial class CheckoutView : ContentPage
{
    ObservableCollection<CartPopupItemView> view_items;
    public const Decimal TAX_RATE = 0.07M;
    Decimal _tax = 0.0M;
    Decimal _subTotal = 0.0M;
    Decimal _mealCost = 0.0M;
    Decimal _tip_amount = 0.0M;

    public CheckoutView()
    {
        InitializeComponent();


        view_items = new();

        var controller_items = App.Cart.GetCartItems();

        foreach (var item in controller_items)
        {
            CartPopupItemView cartPopupItemView = new CartPopupItemView();
            cartPopupItemView.Item = item;
            view_items.Add(cartPopupItemView);
            _subTotal += item.GetCost();
        }

        collCheckoutCart.ItemsSource = view_items;

        UpdateCost();
    }

    private async void CompletePurchaseClicked(object sender, EventArgs e)
    {
        try
        {
            // Process the order...
            
            // Clear the cart
            App.Cart.ClearCart();
            
            await DisplayAlert("Success", "Your order has been placed successfully!", "OK");
            
            // Use PopModalAsync instead of Shell navigation to return to menu
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopModalAsync in CompletePurchaseClicked");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to complete purchase: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"CompletePurchaseClicked error: {ex}");
        }
    }
    
    private async void CancelClicked(object sender, EventArgs e)
    {
        try
        {
            // Use PopModalAsync instead of Shell navigation to return to menu
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopModalAsync in CancelClicked");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"CancelClicked error: {ex}");
        }
    }

    private void UpdateMealCost(object sender, TextChangedEventArgs e)
    {
        try
        {
           _tip_amount = Convert.ToDecimal(txtTip.Text);
            UpdateCost();
        }
        catch (Exception ex)
        {
            return;
        }
    }

    private void UpdateCost()
    {
        _tax = _subTotal * TAX_RATE;
        _mealCost = _subTotal + _tax + _tip_amount;

        lblSubTotal.Text = $"${_subTotal:F2}";
        lblTax.Text = $"${_tax:F2}";
        lblMealCost.Text = $"${_mealCost:F2}";
    }
}