/*
I acknowledge the following statements:
That the code I provide below is my own work and NOT copied from any outside resource, which includes, but not limited to, an artificial intelligence program unless given explicit permission by the instructor.
That the code I provide below is my own work and NOT the work of my peers, tutors, or any other individual unless given explicit permission by the instructor.
That if the code below is in violation of statements 1 and 2 above, I may be reported to the Academic Integrity office and subject to penalties as described in the Academic Integrity Policy.
Your Name: Dol Raj Bashyal
Your Student ID: w10169035
*/

using backendKiosk.Project.Models;
using System.Collections.ObjectModel;
using MenuItem = backendKiosk.Project.Models.MenuItem; 

namespace backendKiosk.Project.Views;

/*
    Hint: A class called ItemSelectView is defined that "wraps" around the data model and provides properties
    that are referenced in the collection view's data template.  The collection view should be an observable
    list of ItemSelectView objects that bind to the menu items being shown.
*/

public partial class ItemAddPage : ContentPage
{
    
    private readonly MenuItemType _itemTypeToShow; 
    private ObservableCollection<ItemSelectView> _itemsForView; 
    private ObservableCollection<SizeTypeView> _sizeOptions; 
    private MenuItem? _selectedMenuItem = null;
    private MenuSizeType _selectedSize = MenuSizeType.SMALL; 

    public ItemAddPage(MenuItemType type)
    {
        InitializeComponent();

        _itemTypeToShow = type;
        Title = $"Choose a {_itemTypeToShow}";  // Set the title of the page based on the type passed in

        
        // These collections are bound to the CollectionView in XAML
        _itemsForView = new ObservableCollection<ItemSelectView>();
        _sizeOptions = new ObservableCollection<SizeTypeView>
        {
            new SizeTypeView { Text = "Small" },
            new SizeTypeView { Text = "Medium", Rate = $"+{(MenuItem.MEDIUM_RATE * 100):F0}%" },
            new SizeTypeView { Text = "Large", Rate = $"+{(MenuItem.LARGE_RATE * 100):F0}%" }
        };

        // Load menu items into the observable collection
        LoadMenuItems();

        // Bind collections to the CollectionViews in XAML
        collItemSelection.ItemsSource = _itemsForView;
        collSizeSelection.ItemsSource = _sizeOptions;

        layoutSizeSelectView.IsVisible = false;
    }

    // This method loads the menu items of the specified type into the observable collection
    private void LoadMenuItems()
    {
        var menuItemsFromController = App.Menu.GetItems(_itemTypeToShow);

        foreach (var menuItem in menuItemsFromController)
        {
            // Create a deep copy of the menu item to avoid modifying the original
            var itemCopy = menuItem.DeepCopy();
            
            _itemsForView.Add(new ItemSelectView { Item = itemCopy });
        }
    }

    private void SelectItem(object sender, SelectionChangedEventArgs e)
    {
        // This method is called when an item is selected in the Collection View...
        if (e.CurrentSelection.FirstOrDefault() is ItemSelectView selectedViewItem)
        {
            _selectedMenuItem = selectedViewItem.Item; 

            // Check if the selected item has size options
            if (_selectedMenuItem.HasSize)
            {
                layoutSizeSelectView.IsVisible = true; 
                // Reset size selection and default internal size state when a new item is picked
                collSizeSelection.SelectedItem = null;
                _selectedSize = MenuSizeType.SMALL; // Default to small when newly shown
                                                
                collSizeSelection.SelectedItem = _sizeOptions.FirstOrDefault(s => s.Text == "Small");
            }
            else
            {
                layoutSizeSelectView.IsVisible = false; 
                _selectedMenuItem = selectedViewItem.Item; 
                _selectedSize = MenuSizeType.SMALL; // Default to small when no size options
            }
        }
        else
        {
            // Item was deselected
            _selectedMenuItem = null;
            layoutSizeSelectView.IsVisible = false;
            collSizeSelection.SelectedItem = null;
        }
    }

    private void SelectSize(object sender, SelectionChangedEventArgs e)
    {
        // This method is called when a size is selected in the Collection View...
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

            // Update the selected menu item with the new size
            if (_selectedMenuItem != null && _selectedMenuItem.HasSize)
            {
                _selectedMenuItem.Size = _selectedSize;
               
            }
        }
        else
        {
            // If no size is selected, reset the selected size to default
            _selectedSize = MenuSizeType.SMALL;
            if (_selectedMenuItem != null && _selectedMenuItem.HasSize)
            {
                _selectedMenuItem.Size = _selectedSize;
            }
        }
    }

    private async void AddItemClicked(object sender, EventArgs e)
    {
        // This method is called when the "Add Item" button is clicked.
        if (_selectedMenuItem == null)
        {
            await DisplayAlert("Selection Error", "Please select an item before adding to the order.", "OK");
            return;
        }

        // Check if a size is selected for items that require it
        if (_selectedMenuItem.HasSize && collSizeSelection.SelectedItem == null)
        {
            await DisplayAlert("Selection Error", "Please select a size for this item.", "OK");
            return;
        }

        
        App.Cart.AddItem(_selectedMenuItem);
        App.Current.Windows[0].Page = new MainMenuPage();
    }
}