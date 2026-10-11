using System.Collections.ObjectModel;
using backendKiosk.Project.Models;
using MenuItem = backendKiosk.Project.Models.MenuItem;

namespace backendKiosk.Project.Views;

public partial class ComboBuilderPage : ContentPage
{
    private MenuItem? _entree, _side, _drink;
    private MenuSizeType _size = MenuSizeType.SMALL;
    private readonly ObservableCollection<ComboItemView> _entrees = new(), _sides = new(), _drinks = new();
    private readonly ObservableCollection<SizeTypeView> _sizes = new()
    {
        new() { Text = "Small", Rate = "Base price" },
        new() { Text = "Medium", Rate = $"+{MenuItem.MEDIUM_RATE * 100:F0}%" },
        new() { Text = "Large", Rate = $"+{MenuItem.LARGE_RATE * 100:F0}%" }
    };
    private bool _loading, _adding;

    public ComboBuilderPage()
    {
        InitializeComponent();
        collEntreeSelection.ItemsSource = _entrees;
        collSideSelection.ItemsSource = _sides;
        collDrinkSelection.ItemsSource = _drinks;
        collSizeSelection.ItemsSource = _sizes;
        collSizeSelection.SelectedItem = _sizes[0];
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loading) return;
        _loading = true;
        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
        try
        {
            var items = await App.Menu.GetMenuItemsAsync();
            Fill(_entrees, items.Where(i => i.Type == MenuItemType.ENTREE));
            Fill(_sides, items.Where(i => i.Type == MenuItemType.SIDE));
            Fill(_drinks, items.Where(i => i.Type == MenuItemType.DRINK));
            _entree = _side = _drink = null;
            RefreshCost();
        }
        catch (Exception ex) { await DisplayAlert("Unable to load menu", ex.Message, "OK"); }
        finally { _loading = false; LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false; }
    }

    private static void Fill(ObservableCollection<ComboItemView> target, IEnumerable<MenuItem> items)
    {
        target.Clear();
        foreach (var item in items) target.Add(new ComboItemView { Item = item.DeepCopy() });
    }

    private void SelectEntree(object sender, SelectionChangedEventArgs e) { _entree = (e.CurrentSelection.FirstOrDefault() as ComboItemView)?.Item; RefreshCost(); }
    private void SelectSide(object sender, SelectionChangedEventArgs e) { _side = (e.CurrentSelection.FirstOrDefault() as ComboItemView)?.Item; RefreshCost(); }
    private void SelectDrink(object sender, SelectionChangedEventArgs e) { _drink = (e.CurrentSelection.FirstOrDefault() as ComboItemView)?.Item; RefreshCost(); }
    private void SelectSize(object sender, SelectionChangedEventArgs e)
    {
        var text = (e.CurrentSelection.FirstOrDefault() as SizeTypeView)?.Text;
        _size = text == "Large" ? MenuSizeType.LARGE : text == "Medium" ? MenuSizeType.MEDIUM : MenuSizeType.SMALL;
        RefreshCost();
    }

    private ComboItem? CreateCombo()
    {
        if (_entree is null || _side is null || _drink is null) return null;
        var combo = new ComboItem { Entree = _entree.DeepCopy(), Side = _side.DeepCopy(), Drink = _drink.DeepCopy() };
        foreach (var item in new[] { combo.Entree, combo.Side, combo.Drink }) if (item.HasSize) item.Size = _size;
        return combo;
    }

    private void RefreshCost()
    {
        var combo = CreateCombo();
        lblCost.Text = combo is null ? "Choose 3 items" : $"${combo.GetCost():F2}";
        AddButton.IsEnabled = combo is not null && !_adding;
    }

    private async void AddComboClicked(object sender, EventArgs e)
    {
        var combo = CreateCombo();
        if (combo is null || _adding) return;
        _adding = true;
        AddButton.IsEnabled = false;
        App.Cart.AddItem(combo);
        await Navigation.PopAsync();
    }

    private async void CancelComboClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}
