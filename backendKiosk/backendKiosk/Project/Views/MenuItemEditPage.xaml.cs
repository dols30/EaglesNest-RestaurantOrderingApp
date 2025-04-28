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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace backendKiosk.Project.Views;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class MenuItemEditPage : ContentPage, INotifyPropertyChanged
{
    private readonly MenuController _menuController;
    // Initialize fields to address CS8618 warnings
    private backendKiosk.Project.Models.MenuItem _currentMenuItem = new backendKiosk.Project.Models.MenuItem();
    private string _pageTitle = "Add Item";
    private bool _isEditMode = false;
    private string _itemId = string.Empty;
    private string _selectedMenuItemType = string.Empty;

    // Add parameterless constructor that uses the App.Menu static instance
    public MenuItemEditPage()
    {
        InitializeComponent();
        _menuController = App.Menu; // Use the static Menu instance
        _selectedMenuItemType = MenuItemTypes.FirstOrDefault() ?? string.Empty;
        BindingContext = this;
    }

     public string ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            LoadMenuItem(int.TryParse(value, out int id) ? id : 0);
        }
    }


    // Use fully qualified name for MenuItem
    public backendKiosk.Project.Models.MenuItem CurrentMenuItem
    {
        get => _currentMenuItem;
        set => SetProperty(ref _currentMenuItem, value);
    }

    public string PageTitle
    {
        get => _pageTitle;
        set => SetProperty(ref _pageTitle, value);
    }

     public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ObservableCollection<string> MenuItemTypes { get; } = new ObservableCollection<string>(Enum.GetNames(typeof(MenuItemType)));

    public string SelectedMenuItemType
    {
        get => _selectedMenuItemType;
        set
        {
            if (SetProperty(ref _selectedMenuItemType, value) && CurrentMenuItem != null)
            {
                 if (Enum.TryParse<MenuItemType>(value, true, out var type))
                 {
                     CurrentMenuItem.Type = type;
                 }
            }
        }
    }


    public MenuItemEditPage(MenuController menuController)
    {
        InitializeComponent();
        _menuController = menuController;
        _selectedMenuItemType = MenuItemTypes.FirstOrDefault() ?? string.Empty;
        BindingContext = this;
    }

     private async void LoadMenuItem(int itemId)
    {
        if (itemId > 0)
        {
            CurrentMenuItem = await _menuController.GetItemByIdAsync(itemId);
            PageTitle = $"Edit {CurrentMenuItem?.ItemName ?? "Item"}";
            IsEditMode = true;
            SelectedMenuItemType = CurrentMenuItem?.Type.ToString();
        }
        else
        {
            // Initialize with empty strings for required properties
            CurrentMenuItem = new backendKiosk.Project.Models.MenuItem 
            { 
                ItemPrice = 0.0,
                ItemName = string.Empty,
                Icon = string.Empty
            };
            PageTitle = "Add New Menu Item";
            IsEditMode = false;
            SelectedMenuItemType = MenuItemTypes.FirstOrDefault();
        }
         OnPropertyChanged(nameof(CurrentMenuItem));
         OnPropertyChanged(nameof(SelectedMenuItemType));
    }


    private async void SaveButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate current menu item
            if (string.IsNullOrWhiteSpace(CurrentMenuItem.ItemName))
            {
                await DisplayAlert("Validation Error", "Item name is required", "OK");
                return;
            }

            if (CurrentMenuItem.ItemPrice <= 0)
            {
                await DisplayAlert("Validation Error", "Item price must be greater than zero", "OK");
                return;
            }

            if (!IsEditMode)
            {
                // Add new item
                await App.Menu.AddItemAsync(CurrentMenuItem);
                await DisplayAlert("Success", "Item added successfully!", "OK");
            }
            else
            {
                // Update existing item
                await App.Menu.UpdateItemAsync(CurrentMenuItem);
                await DisplayAlert("Success", "Item updated successfully!", "OK");
            }
            
            // Return to dashboard
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopModalAsync in SaveButtonClicked");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save item: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"SaveButtonClicked error: {ex}");
        }
    }

    private async void CancelButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Return to dashboard without saving
            await Navigation.PopModalAsync();
            System.Diagnostics.Debug.WriteLine("Completed PopModalAsync in CancelButtonClicked");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"CancelButtonClicked error: {ex}");
        }
    }
    
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        try
        {
            bool confirm = await DisplayAlert("Confirm Delete", 
                $"Are you sure you want to delete {CurrentMenuItem.ItemName}?", "Yes", "No");

            if (confirm)
            {
                await App.Menu.DeleteItemAsync(CurrentMenuItem);
                await DisplayAlert("Success", "Item deleted successfully", "OK");
                
                // Return to dashboard
                await Navigation.PopModalAsync();
                System.Diagnostics.Debug.WriteLine("Completed PopModalAsync after deletion");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete item: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"OnDeleteClicked error: {ex}");
        }
    }


    #region INotifyPropertyChanged Implementation
    // Make event nullable and use 'new' keyword
    public new event PropertyChangedEventHandler? PropertyChanged;

    // Use 'new' keyword and make parameter nullable
    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Make parameter nullable
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}
