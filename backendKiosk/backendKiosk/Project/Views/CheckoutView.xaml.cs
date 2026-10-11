using backendKiosk.Project.Models;

namespace backendKiosk.Project.Views;

public partial class CheckoutView : ContentPage
{
    private readonly decimal _subtotal;
    private bool _submitting;

    public CheckoutView()
    {
        InitializeComponent();
        var items = App.Cart.GetCartItems();
        collCheckoutCart.ItemsSource = items.Select(item => new CartPopupItemView { Item = item }).ToList();
        _subtotal = items.Sum(item => item.GetCost());
        UpdateCost();
    }

    private void UpdateMealCost(object sender, TextChangedEventArgs e) => UpdateCost();

    private void UpdateCost()
    {
        // TextChanged can fire during InitializeComponent, before all names exist.
        if (SubmitButton is null || TipError is null) return;
        var valid = OrderTotals.TryParseTip(txtTip.Text, out var tip);
        OrderTotals totals;
        try { totals = OrderTotals.Calculate(_subtotal, valid ? tip : 0); }
        catch (OverflowException) { valid = false; totals = OrderTotals.Calculate(_subtotal, 0); }
        lblSubTotal.Text = $"${totals.Subtotal:F2}";
        lblTax.Text = $"${totals.Tax:F2}";
        lblMealCost.Text = $"${totals.Total:F2}";
        TipError.IsVisible = !valid;
        SubmitButton.IsEnabled = valid && _subtotal > 0 && !_submitting;
    }

    private async void CompletePurchaseClicked(object sender, EventArgs e)
    {
        if (_submitting || !SubmitButton.IsEnabled) return;
        _submitting = true;
        SubmitButton.IsEnabled = false;
        try
        {
            await DisplayAlert("Thank you!", "Your demo order is complete. No payment has been collected.", "Done");
            App.Cart.ClearCart();
            await Navigation.PopModalAsync();
            await Shell.Current.GoToAsync("///welcome");
        }
        catch (Exception ex) { await DisplayAlert("Unable to complete order", ex.Message, "OK"); }
        finally { _submitting = false; UpdateCost(); }
    }

    private async void CancelClicked(object sender, EventArgs e) => await Navigation.PopModalAsync();
}
