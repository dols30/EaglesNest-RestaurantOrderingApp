using backendKiosk.Project.Models;
using System.Collections.ObjectModel;
using MenuItem = backendKiosk.Project.Models.MenuItem;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace backendKiosk.Project.Views;

[QueryProperty(nameof(ItemType), "ItemType")]
public partial class ItemAddPage : ContentPage
{
    
    private MenuItemType _itemTypeToShow = MenuItemType.ENTREE;
    private ObservableCollection<ItemSelectView> _itemsForView = new();
    private ObservableCollection<SizeTypeView> _sizeOptions = new()
    {
        new SizeTypeView { Text = "Small" },
        new SizeTypeView { Text = "Medium", Rate = $"+{(MenuItem.MEDIUM_RATE * 100):F0}%" },
        new SizeTypeView { Text = "Large", Rate = $"+{(MenuItem.LARGE_RATE * 100):F0}%" }
    };
    private MenuItem? _selectedMenuItem = null;
    private MenuSizeType _selectedSize = MenuSizeType.SMALL;
    private bool _isLoading = false;
    
    public string ItemType
    {
        get => _itemTypeToShow.ToString();
        set
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Received ItemType parameter: {value}");
                MenuItemType defaultType = MenuItemType.ENTREE;
                
                if (string.IsNullOrEmpty(value))
                {
                    System.Diagnostics.Debug.WriteLine($"ItemType parameter is empty, using default: {defaultType}");
                    _itemTypeToShow = defaultType;
                }
                else if (Enum.TryParse<MenuItemType>(value, out var itemType))
                {
                    _itemTypeToShow = itemType;
                    System.Diagnostics.Debug.WriteLine($"Successfully parsed ItemType: {_itemTypeToShow}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to parse ItemType: {value}, using default: {defaultType}");
                    _itemTypeToShow = defaultType;
                }
                
                Title = $"Choose a {_itemTypeToShow}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting ItemType: {ex.Message}");
                _itemTypeToShow = MenuItemType.ENTREE;
                
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    await DisplayAlert("Error", $"Failed to load item type: {ex.Message}", "OK");
                });
            }
        }
    }

    public ItemAddPage()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("ItemAddPage constructor called");

            InitializeComponent();

            collItemSelection.ItemsSource = _itemsForView;
            collSizeSelection.ItemsSource = _sizeOptions;

            layoutSizeSelectView.IsVisible = false;
            
            System.Diagnostics.Debug.WriteLine($"ItemAddPage initialized with ItemType: {_itemTypeToShow}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ItemAddPage initialization error: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                await DisplayAlert("Error", $"Failed to initialize page: {ex.Message}", "OK");
            });
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"ItemAddPage OnAppearing with ItemType: {_itemTypeToShow}");
            
            await LoadMenuItemsAsync();
            
            if (_selectedMenuItem?.HasSize == true)
            {
                layoutSizeSelectView.IsVisible = true;
                
                if (collSizeSelection?.SelectedItem == null)
                {
                    var smallOption = _sizeOptions.FirstOrDefault(s => s.Text == "Small");
                    if (smallOption != null)
                    {
                        collSizeSelection.SelectedItem = smallOption;
                    }
                }
            }
            else
            {
                layoutSizeSelectView.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OnAppearing error: {ex.Message}");
        }
    }

    private async Task LoadMenuItemsAsync()
    {
        if (_isLoading) return;
        _isLoading = true;
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"Loading menu items for type: {_itemTypeToShow}");
            
            await Task.Run(() =>
            {
                if (App.Menu == null)
                {
                    System.Diagnostics.Debug.WriteLine("App.Menu is null!");
                    throw new InvalidOperationException("Menu controller is not initialized");
                }
                
                var menuItemsFromController = App.Menu.GetItems(_itemTypeToShow);
                System.Diagnostics.Debug.WriteLine($"Found {menuItemsFromController?.Count ?? 0} items of type {_itemTypeToShow}");

                var itemsToAdd = new List<ItemSelectView>();
                if (menuItemsFromController != null)
                {
                    foreach (var menuItem in menuItemsFromController)
                    {
                        var itemCopy = menuItem.DeepCopy();
                        itemsToAdd.Add(new ItemSelectView { Item = itemCopy });
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _itemsForView.Clear();
                    foreach(var item in itemsToAdd)
                    {
                        _itemsForView.Add(item);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMenuItemsAsync error: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                await DisplayAlert("Error", $"Failed to load menu items: {ex.Message}", "OK");
            });
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void SelectItem(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e?.CurrentSelection == null)
            {
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is ItemSelectView selectedViewItem && selectedViewItem?.Item != null)
            {
                _selectedMenuItem = selectedViewItem.Item; 

                if (_selectedMenuItem.HasSize)
                {
                    layoutSizeSelectView.IsVisible = true; 
                    _selectedSize = MenuSizeType.SMALL;
                                                    
                    var smallOption = _sizeOptions?.FirstOrDefault(s => s.Text == "Small");
                    if (collSizeSelection != null && smallOption != null)
                    {
                        collSizeSelection.SelectedItem = smallOption;
                    }
                    else if (collSizeSelection != null)
                    {
                        collSizeSelection.SelectedItem = null;
                    }
                }
                else
                {
                    layoutSizeSelectView.IsVisible = false; 
                    _selectedSize = MenuSizeType.SMALL;
                }
            }
            else
            {
                _selectedMenuItem = null;
                layoutSizeSelectView.IsVisible = false;
                if (collSizeSelection != null)
                {
                    collSizeSelection.SelectedItem = null;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SelectItem error: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                await DisplayAlert("Error", $"Failed to select item: {ex.Message}", "OK");
            });
        }
    }

    private void SelectSize(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (e?.CurrentSelection == null)
            {
                return;
            }

            if (e.CurrentSelection.FirstOrDefault() is SizeTypeView selectedSizeView)
            {
                
                switch (selectedSizeView.Text)
                {
                    case "Small":
                        _selectedSize = MenuSizeType.SMALL;
                        break;
                    case "Medium":
                        _selectedSize = MenuSizeType.MEDIUM;
                        break;
                    case "Large":
                        _selectedSize = MenuSizeType.LARGE;
                        break;
                    default:
                        _selectedSize = MenuSizeType.SMALL; 
                        break;
                }

                if (_selectedMenuItem != null && _selectedMenuItem.HasSize)
                {
                    _selectedMenuItem.Size = _selectedSize;
                }
            }
            else
            {
                _selectedSize = MenuSizeType.SMALL;
                if (_selectedMenuItem != null && _selectedMenuItem.HasSize)
                {
                    _selectedMenuItem.Size = _selectedSize;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SelectSize error: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                await DisplayAlert("Error", $"Failed to select size: {ex.Message}", "OK");
            });
        }
    }

    private async void AddItemClicked(object sender, EventArgs e)
    {
        try
        {
            if (_selectedMenuItem == null)
            {
                await DisplayAlert("Selection Error", "Please select an item before adding to the order.", "OK");
                return;
            }

            if (_selectedMenuItem.HasSize && collSizeSelection?.SelectedItem == null)
            {
                await DisplayAlert("Selection Error", "Please select a size for this item.", "OK");
                return;
            }

            App.Cart?.AddItem(_selectedMenuItem);
            
            await Navigation.PopAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopAsync in AddItemClicked");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddItemClicked error: {ex.Message}");
            await DisplayAlert("Error", $"Failed to add item: {ex.Message}", "OK");
        }
    }

    private async void CancelClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopAsync in CancelClicked");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"CancelClicked error: {ex}");
        }
    }
}