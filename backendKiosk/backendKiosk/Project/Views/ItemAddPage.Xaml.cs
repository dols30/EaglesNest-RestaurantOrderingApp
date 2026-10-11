using backendKiosk.Project.Models;
using System.Collections.ObjectModel;
using MenuItem = backendKiosk.Project.Models.MenuItem;

namespace backendKiosk.Project.Views;

[QueryProperty(nameof(ItemType), "ItemType")]
public partial class ItemAddPage : ContentPage
{
    private MenuItemType _type = MenuItemType.ENTREE;
    private readonly ObservableCollection<ItemSelectView> _items = new();
    private readonly ObservableCollection<SizeTypeView> _sizes = new()
    {
        new() { Text = "Small", Rate = "Base price" },
        new() { Text = "Medium", Rate = $"+{MenuItem.MEDIUM_RATE * 100:F0}%" },
        new() { Text = "Large", Rate = $"+{MenuItem.LARGE_RATE * 100:F0}%" }
    };
    private MenuItem? _selected;
    private bool _loading;
    private bool _adding;

    public string ItemType
    {
        get => _type.ToString();
        set
        {
            if (Enum.TryParse<MenuItemType>(value, out var type)) _type = type;
            var article = _type is MenuItemType.ENTREE or MenuItemType.APPETIZER ? "an" : "a";
            Title = $"Choose {article} {_type.ToString().ToLowerInvariant()}";
            CategoryHeading.Text = Title;
        }
    }

    public ItemAddPage()
    {
        InitializeComponent();
        collItemSelection.ItemsSource = _items;
        collSizeSelection.ItemsSource = _sizes;
        SizeChanged += (_, _) => UpdateAddButton();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_loading) return;
        _loading = true;
        LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = true;
        try
        {
            var items = await App.Menu.GetItemsByTypeAsync(_type);
            _items.Clear();
            foreach (var item in items) _items.Add(new ItemSelectView { Item = item.DeepCopy() });
            _selected = null;
            collItemSelection.SelectedItem = null;
            layoutSizeSelectView.IsVisible = false;
            AddButton.IsEnabled = false;
        }
        catch (Exception ex) { await DisplayAlert("Unable to load menu", ex.Message, "OK"); }
        finally { _loading = false; LoadingIndicator.IsVisible = LoadingIndicator.IsRunning = false; }
    }

    private void SelectItem(object sender, SelectionChangedEventArgs e)
    {
        _selected = (e.CurrentSelection.FirstOrDefault() as ItemSelectView)?.Item;
        layoutSizeSelectView.IsVisible = _selected?.HasSize == true;
        if (_selected is not null) _selected.Size = MenuSizeType.SMALL;
        collSizeSelection.SelectedItem = _selected?.HasSize == true ? _sizes[0] : null;
        UpdateAddButton();
    }

    private void SelectSize(object sender, SelectionChangedEventArgs e)
    {
        if (_selected is null) return;
        var size = (e.CurrentSelection.FirstOrDefault() as SizeTypeView)?.Text;
        _selected.Size = size == "Large" ? MenuSizeType.LARGE : size == "Medium" ? MenuSizeType.MEDIUM : MenuSizeType.SMALL;
        UpdateAddButton();
    }

    private void UpdateAddButton()
    {
        AddButton.IsEnabled = _selected is not null;
        AddButton.Text = _selected is null ? "Add to Order" : Width < 500 ? $"Add • ${_selected.GetCost():F2}" : $"Add to Order • ${_selected.GetCost():F2}";
    }

    private async void AddItemClicked(object sender, EventArgs e)
    {
        if (_selected is null || _adding) return;
        _adding = true;
        AddButton.IsEnabled = false;
        App.Cart.AddItem(_selected.DeepCopy());
        await Navigation.PopAsync();
    }

    private async void CancelClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}
