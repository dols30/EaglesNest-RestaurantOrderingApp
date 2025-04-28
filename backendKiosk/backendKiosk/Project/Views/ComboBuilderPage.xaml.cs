using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Linq;

namespace backendKiosk.Project.Views;
using Models;

public partial class ComboBuilderPage : ContentPage
{
	MenuItem? selectedEntree = null;
	MenuItem? selectedSide = null;
	MenuItem? selectedDrink = null;
	Models.MenuSizeType MealSize = Models.MenuSizeType.SMALL;
    
    private ObservableCollection<ComboItemView> _entreeViews = new();
    private ObservableCollection<ComboItemView> _sideViews = new();
    private ObservableCollection<ComboItemView> _drinkViews = new();
    private ObservableCollection<SizeTypeView> _sizeOptions = new()
    {
        new SizeTypeView {Text = "Small" },
        new SizeTypeView {Text = "Medium", Rate=$"+{MenuItem.MEDIUM_RATE:F2}%" },
        new SizeTypeView {Text = "Large", Rate=$"+{MenuItem.LARGE_RATE:F2}%" }
    };
    private bool _isLoading = false;

	public ComboBuilderPage()
	{
		try 
		{
			InitializeComponent();
			
            collEntreeSelection.ItemsSource = _entreeViews;
            collSideSelection.ItemsSource = _sideViews;
            collDrinkSelection.ItemsSource = _drinkViews;
            collSizeSelection.ItemsSource = _sizeOptions;
		}
		catch (Exception ex)
		{
			MainThread.BeginInvokeOnMainThread(async () => {
				await DisplayAlert("Error", $"Failed to initialize combo builder: {ex.Message}", "OK");
				System.Diagnostics.Debug.WriteLine($"ComboBuilderPage initialization error: {ex.Message}");
			});
		}
	}
	
	private async Task LoadItemsAsync()
	{
        if (_isLoading) return;
        _isLoading = true;

	    try
	    {
            await Task.Run(() =>
            {
                if (App.Menu == null)
                {
                    throw new InvalidOperationException("Menu controller is not initialized");
                }
                
                var entrees = App.Menu.GetItems(MenuItemType.ENTREE);
                var sides = App.Menu.GetItems(MenuItemType.SIDE);
                var drinks = App.Menu.GetItems(MenuItemType.DRINK);
        
                var entreeViewsToAdd = entrees.Select(entree => new ComboItemView { Item = entree.DeepCopy() }).ToList();
                var sideViewsToAdd = sides.Select(side => new ComboItemView { Item = side.DeepCopy() }).ToList();
                var drinkViewsToAdd = drinks.Select(drink => new ComboItemView { Item = drink.DeepCopy() }).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _entreeViews.Clear();
                    foreach (var view in entreeViewsToAdd) _entreeViews.Add(view);

                    _sideViews.Clear();
                    foreach (var view in sideViewsToAdd) _sideViews.Add(view);
                    
                    _drinkViews.Clear();
                    foreach (var view in drinkViewsToAdd) _drinkViews.Add(view);

                    if (collSizeSelection != null && collSizeSelection.SelectedItem == null && _sizeOptions.Any())
                    {
                        collSizeSelection.SelectedItem = _sizeOptions.FirstOrDefault(s => s.Text == "Small") ?? _sizeOptions.First();
                    }
                    RefreshCost();
                });
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadItemsAsync error: {ex.Message}");
            MainThread.BeginInvokeOnMainThread(async () => {
                await DisplayAlert("Error", $"Failed to load menu items: {ex.Message}", "OK");
            });
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _isLoading = false;
            });
        }
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		try
		{
            await LoadItemsAsync();
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"OnAppearing error: {ex.Message}");
		}
	}

	private void SelectEntree(object sender, SelectionChangedEventArgs e)
	{
		try
		{
		    if (e?.CurrentSelection == null)
                return;
                
			if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is ComboItemView comboItem && comboItem?.Item != null)
			{
				selectedEntree = comboItem.Item;
				RefreshCost();
			}
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"SelectEntree error: {ex.Message}");
		    selectedEntree = null;
		}
	}

	private void SelectSide(object sender, SelectionChangedEventArgs e)
	{
		try
		{
		    if (e?.CurrentSelection == null)
                return;
                
			if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is ComboItemView comboItem && comboItem?.Item != null)
			{
				selectedSide = comboItem.Item;
				RefreshCost();
			}
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"SelectSide error: {ex.Message}");
		    selectedSide = null;
		}
	}

	private void SelectDrink(object sender, SelectionChangedEventArgs e)
	{
		try
		{
		    if (e?.CurrentSelection == null)
                return;
                
			if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is ComboItemView comboItem && comboItem?.Item != null)
			{
				selectedDrink = comboItem.Item;
				RefreshCost();
			}
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"SelectDrink error: {ex.Message}");
		    selectedDrink = null;
		}
	}
	
	private void SelectSize(object sender, SelectionChangedEventArgs e)
	{
		try
		{
		    if (e?.CurrentSelection == null || e.CurrentSelection.Count == 0)
			    return;
			
			SizeTypeView? s = e.CurrentSelection[0] as SizeTypeView;
			if (s == null)
			    return;

			if (s.Text == "Small")
				MealSize = MenuSizeType.SMALL;
			else if (s.Text == "Medium")
				MealSize = MenuSizeType.MEDIUM;
			else if (s.Text == "Large")
				MealSize = MenuSizeType.LARGE;
		    else
		        MealSize = MenuSizeType.SMALL;

		    RefreshCost();
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"SelectSize error: {ex.Message}");
		    MealSize = MenuSizeType.SMALL;
		}
	}

	void RefreshCost()
	{
		try
		{
			bool itemsReady = selectedEntree != null && selectedSide != null && selectedDrink != null;

			if (!itemsReady) 
			{
				if (lblCost != null)
				    lblCost.Text = ""; 
				return;
			}

			var combo = new Project.Models.ComboItem
			{
				Entree = selectedEntree!, 
				Side = selectedSide!,
				Drink = selectedDrink!
			};

			if (combo.Entree?.HasSize == true)
				combo.Entree.Size = MealSize;
			if (combo.Side?.HasSize == true)
				combo.Side.Size = MealSize;
			if (combo.Drink?.HasSize == true)
				combo.Drink.Size = MealSize;

			if (lblCost != null)
			    lblCost.Text = $"{combo.GetCost():F2}";
		}
		catch (Exception ex)
		{
		    System.Diagnostics.Debug.WriteLine($"RefreshCost error: {ex.Message}");
		    if (lblCost != null)
		        lblCost.Text = "Error";
		}
	}

	private async void AddComboClicked(object sender, EventArgs e)
	{
		try
		{
			if (selectedEntree == null || selectedSide == null || selectedDrink == null)
			{
				await DisplayAlert("Error", "Error: You must select an entree, side, and drink.", "OK");
				return;
			}

			var comboItem = new Project.Models.ComboItem
			{
				Entree = selectedEntree!, 
				Side = selectedSide!,
				Drink = selectedDrink!
			};

			if (comboItem.Entree?.HasSize == true) 
			    comboItem.Entree.Size = MealSize;
			if (comboItem.Side?.HasSize == true) 
			    comboItem.Side.Size = MealSize;
			if (comboItem.Drink?.HasSize == true) 
			    comboItem.Drink.Size = MealSize;

			if (App.Cart != null)
			    App.Cart.AddItem(comboItem);
			else
			    throw new InvalidOperationException("Shopping cart is not initialized");

			await Navigation.PopAsync(); 
			System.Diagnostics.Debug.WriteLine("Completed PopAsync in AddComboClicked");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to add combo: {ex.Message}", "OK");
			System.Diagnostics.Debug.WriteLine($"AddComboClicked error: {ex}");
		}
	}

	private async void CancelComboClicked(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PopAsync(); 
			System.Diagnostics.Debug.WriteLine("Completed PopAsync in CancelComboClicked");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
			System.Diagnostics.Debug.WriteLine($"CancelComboClicked error: {ex}");
		}
	}
}