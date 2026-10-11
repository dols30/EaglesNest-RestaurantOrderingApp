using backendKiosk.Project.Models;
using System.Collections.ObjectModel;

namespace backendKiosk.Project.Views;

public partial class MainMenuPage : ContentPage
{
    private readonly ObservableCollection<CartPopupItemView> _items = new();
    private bool _navigating;

    public MainMenuPage()
    {
        InitializeComponent();
        collCart.ItemsSource = _items;
        SizeChanged += (_, _) => UpdateLayout();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WelcomeLabel.Text = string.IsNullOrWhiteSpace(App.Cart.CustomerName)
            ? "Welcome! What are you craving?"
            : $"Welcome, {App.Cart.CustomerName}! What are you craving?";
        RefreshCart();
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (Width <= 0) return;
        bool narrow = Width < 780;
        MenuHeading.FontSize = Width < 500 ? 24 : 32;
        ManageButton.FontSize = Width < 500 ? 13 : 15;
        MenuLayout.ColumnSpacing = narrow ? 0 : 24;
        PageScroll.Orientation = narrow ? ScrollOrientation.Vertical : ScrollOrientation.Neither;
        MenuScroll.Orientation = narrow ? ScrollOrientation.Neither : ScrollOrientation.Vertical;
        PageGrid.HeightRequest = narrow ? -1 : Height;
        PageGrid.RowDefinitions[1].Height = narrow ? GridLength.Auto : GridLength.Star;
        MenuLayout.ColumnDefinitions[1].Width = narrow ? new GridLength(0) : new GridLength(300);
        Grid.SetColumn(CartPanel, narrow ? 0 : 1);
        Grid.SetRow(CartPanel, narrow ? 1 : 0);
        CartPanel.HeightRequest = narrow ? 360 : -1;
        ComboPhoto.IsVisible = Width >= 580;
        ComboBanner.ColumnDefinitions[1].Width = Width < 580 ? new GridLength(0) : new GridLength(0.8, GridUnitType.Star);
        bool phone = Width < 500;
        CategoriesGrid.ColumnSpacing = phone ? 0 : 12;
        CategoriesGrid.ColumnDefinitions[1].Width = phone ? new GridLength(0) : GridLength.Star;
        Grid.SetColumn(SideCategory, phone ? 0 : 1);
        Grid.SetRow(SideCategory, phone ? 1 : 0);
        Grid.SetRow(DrinkCategory, phone ? 2 : 1);
        Grid.SetColumn(DessertCategory, phone ? 0 : 1);
        Grid.SetRow(DessertCategory, phone ? 3 : 1);
        Grid.SetRow(AppetizerCategory, phone ? 4 : 2);
        Grid.SetColumnSpan(AppetizerCategory, phone ? 1 : 2);
        // Keep the menu and order independently scrollable on a small kiosk.
        MenuLayout.RowDefinitions[0].Height = narrow ? GridLength.Auto : GridLength.Star;
    }

    private void RefreshCart()
    {
        collCart.SelectedItem = null;
        _items.Clear();
        foreach (var item in App.Cart.GetCartItems()) _items.Add(new CartPopupItemView { Item = item });
        CartCountLabel.Text = $"{_items.Count} {(_items.Count == 1 ? "item" : "items")}";
        CartSubtotalLabel.Text = $"${App.Cart.GetCartItems().Sum(i => i.GetCost()):F2}";
        CheckoutButton.IsEnabled = _items.Count > 0;
        DeleteButton.IsEnabled = false;
    }

    private void CartSelectionChanged(object sender, SelectionChangedEventArgs e) => DeleteButton.IsEnabled = e.CurrentSelection.Count > 0;

    private void ButtonDeleteCartItems(object sender, EventArgs e)
    {
        if (collCart.SelectedItem is CartPopupItemView { Item: not null } selected)
            App.Cart.RemoveItem(selected.Item.ID);
        RefreshCart();
    }

    private async Task OpenPage(Page page, bool modal = false)
    {
        if (_navigating) return;
        _navigating = true;
        try
        {
            if (modal) await Navigation.PushModalAsync(page);
            else await Navigation.PushAsync(page);
        }
        catch (Exception ex) { await DisplayAlert("Unable to open page", ex.Message, "OK"); }
        finally { _navigating = false; }
    }

    private Task OpenCategory(MenuItemType type) => OpenPage(new ItemAddPage { ItemType = type.ToString() });
    private async void ButtonBuildCombo(object sender, EventArgs e) => await OpenPage(new ComboBuilderPage());
    private async void ButtonAddEntree(object sender, EventArgs e) => await OpenCategory(MenuItemType.ENTREE);
    private async void ButtonAddSide(object sender, EventArgs e) => await OpenCategory(MenuItemType.SIDE);
    private async void ButtonAddDrink(object sender, EventArgs e) => await OpenCategory(MenuItemType.DRINK);
    private async void ButtonAddDessert(object sender, EventArgs e) => await OpenCategory(MenuItemType.DESSERT);
    private async void ButtonAddAppetizer(object sender, EventArgs e) => await OpenCategory(MenuItemType.APPETIZER);
    private async void ManageMenu_Clicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("///dashboard");
    private async void BackToWelcome(object sender, EventArgs e) => await Shell.Current.GoToAsync("///welcome");
    private async void ButtonCheckoutCart(object sender, EventArgs e)
    {
        if (_items.Count > 0) await OpenPage(new CheckoutView(), modal: true);
    }
}
