/*
I acknowledge the following statements:
That the code I provide below is my own work and NOT copied from any outside resource, which includes, but not limited to, an artificial intelligence program unless given explicit permission by the instructor.
That the code I provide below is my own work and NOT the work of my peers, tutors, or any other individual unless given explicit permission by the instructor.
That if the code below is in violation of statements 1 and 2 above, I may be reported to the Academic Integrity office and subject to penalties as described in the Academic Integrity Policy.
Your Name: Dol Raj Bashyal
Your Student ID: w10169035
*/
using backendKiosk.Project.Controllers;
using backendKiosk.Project.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace backendKiosk.Project.Views;

public partial class MenuDashboardPage : ContentPage
{
    private readonly MenuController _menuController;
    private ObservableCollection<backendKiosk.Project.Models.MenuItem> _allItems;
    
    public ObservableCollection<backendKiosk.Project.Models.MenuItem> MenuItems { get; private set; }

    // Add a parameterless constructor that uses App.Menu
    public MenuDashboardPage()
    {
        _menuController = App.Menu; // Use the static Menu instance
        _allItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
        MenuItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
        
        InitializeComponent();
        BindingContext = this;
    }
    
    public MenuDashboardPage(MenuController menuController)
    {
        _menuController = menuController;
        _allItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
        MenuItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
        
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMenuItems();
    }

    private async Task LoadMenuItems()
    {
        try
        {
            await _menuController.LoadMenuItemsAsync();
            var items = await _menuController.GetMenuItemsAsync();
            
            _allItems.Clear();
            foreach (var item in items)
            {
                _allItems.Add(item);
            }
            
            ApplyFilters();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load menu items: {ex.Message}", "OK");
        }
    }

    private void ApplyFilters()
    {
        try
        {
            // Safety check - if UI controls aren't initialized yet, don't proceed
            if (SearchBarName == null || PickerType == null || CheckBoxHasSize == null || MenuItemsCollectionView == null)
            {
                System.Diagnostics.Debug.WriteLine("Filter controls not initialized yet, skipping filter application");
                return;
            }

            // Safety check for _allItems
            if (_allItems == null)
            {
                _allItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
                System.Diagnostics.Debug.WriteLine("_allItems was null, created new empty collection");
            }

            // Apply filters
            var filteredItems = _allItems.AsEnumerable();

            string nameFilter = SearchBarName.Text?.ToLowerInvariant() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                filteredItems = filteredItems.Where(item => 
                    item.ItemName != null && 
                    item.ItemName.ToLowerInvariant().Contains(nameFilter));
            }

            if (PickerType.SelectedIndex > 0 && PickerType.SelectedItem is string selectedTypeString)
            {
                if (!string.IsNullOrEmpty(selectedTypeString) && 
                    Enum.TryParse<MenuItemType>(selectedTypeString, true, out var selectedType))
                {
                    filteredItems = filteredItems.Where(item => item.Type == selectedType);
                }
            }

            if (CheckBoxHasSize.IsChecked == true)
            {
                filteredItems = filteredItems.Where(item => item.HasSize);
            }

            // Initialize MenuItems if null
            if (MenuItems == null)
            {
                MenuItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();
                System.Diagnostics.Debug.WriteLine("MenuItems was null, created new collection");
            }

            MenuItems.Clear();
            foreach (var item in filteredItems)
            {
                MenuItems.Add(item);
            }
            
            // Update the ItemsSource
            MenuItemsCollectionView.ItemsSource = MenuItems;
            
            System.Diagnostics.Debug.WriteLine($"Applied filters - showing {MenuItems.Count} items");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex}");
            MainThread.BeginInvokeOnMainThread(async () => {
                await DisplayAlert("Filter Error", $"Error applying filters: {ex.Message}", "OK");
            });
        }
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        if (SearchBarName != null) SearchBarName.Text = string.Empty;
        if (PickerType != null) PickerType.SelectedIndex = 0;
        if (CheckBoxHasSize != null) CheckBoxHasSize.IsChecked = false;
        ApplyFilters();
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate to edit page with simplified route
            await Shell.Current.GoToAsync("///editItem");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Navigation Error", $"Failed to navigate to edit page: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"OnAddItemClicked error: {ex}");
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.CommandParameter is backendKiosk.Project.Models.MenuItem itemToEdit)
            {
                // Use query parameters with the simplified route
                var parameters = new Dictionary<string, object>
                {
                    { "itemId", itemToEdit.DatabaseID }
                };
                await Shell.Current.GoToAsync("///editItem", parameters);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Navigation Error", $"Failed to navigate to edit page: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"OnEditClicked error: {ex}");
        }
    }

    private async void OnRemoveClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is backendKiosk.Project.Models.MenuItem itemToRemove)
        {
            bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to remove {itemToRemove.ItemName}?", "Yes", "No");
            if (confirm)
            {
                await _menuController.DeleteItemAsync(itemToRemove);
                await LoadMenuItems(); // Refresh list
            }
        }
    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        // Optional: Handle item selection if needed
        // if (e.CurrentSelection.FirstOrDefault() != null)
        // {
        //    (sender as CollectionView).SelectedItem = null; // Deselect
        // }
    }
}

