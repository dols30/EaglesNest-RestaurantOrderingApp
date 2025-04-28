using System.Collections.ObjectModel;

namespace backendKiosk.Project.Views;
using Models;

public partial class ComboBuilderPage : ContentPage
{
	MenuItem? selectedEntree = null;
	MenuItem? selectedSide = null;
	MenuItem? selectedDrink = null;
	Models.MenuSizeType MealSize = Models.MenuSizeType.SMALL;

	public ComboBuilderPage()
	{
		InitializeComponent();

		var entrees = App.Menu.GetItems(MenuItemType.ENTREE);
		var sides = App.Menu.GetItems(MenuItemType.SIDE);
		var drinks = App.Menu.GetItems(MenuItemType.DRINK);

		ObservableCollection<ComboItemView> entreeViews = new();
		foreach (var entree in entrees)
		{
			MenuItem item = entree.DeepCopy();
			entreeViews.Add(new ComboItemView { Item = item });
		}
		ObservableCollection<ComboItemView> sideViews = new();
		foreach (var side in sides)
		{
			MenuItem item = side.DeepCopy();
			sideViews.Add(new ComboItemView { Item = item });
		}
		ObservableCollection<ComboItemView> drinkViews = new();
		foreach (var drink in drinks)
		{
			MenuItem item = drink.DeepCopy();
			drinkViews.Add(new ComboItemView { Item = item });
		}
		collEntreeSelection.ItemsSource = entreeViews;
		collSideSelection.ItemsSource = sideViews;
		collDrinkSelection.ItemsSource = drinkViews;

		collSizeSelection.ItemsSource = new ObservableCollection<SizeTypeView>
		{
			new SizeTypeView {Text = "Small" },
			new SizeTypeView {Text = "Medium", Rate=$"+{MenuItem.MEDIUM_RATE:F2}%" },
			new SizeTypeView {Text = "Large", Rate=$"+{MenuItem.LARGE_RATE:F2}%" }
		};
	}

	private void SelectEntree(object sender, SelectionChangedEventArgs e)
	{
		if (collEntreeSelection.SelectedItem is ComboItemView comboItem)
		{
			selectedEntree = comboItem.Item;
			RefreshCost();
		}
	}

	private void SelectSide(object sender, SelectionChangedEventArgs e)
	{
		if (collSideSelection.SelectedItem is ComboItemView comboItem)
		{
			selectedSide = comboItem.Item;
			RefreshCost();
		}
	}

	private void SelectDrink(object sender, SelectionChangedEventArgs e)
	{
		if (collDrinkSelection.SelectedItem is ComboItemView comboItem)
		{
			selectedDrink = comboItem.Item;
			RefreshCost();
		}
	}
	private void SelectSize(object sender, SelectionChangedEventArgs e)
	{
		if (collSizeSelection.SelectedItem == null)
			return;
		else
		{
			SizeTypeView s = (SizeTypeView)collSizeSelection.SelectedItem;

			if (s != null && s.Text == "Small")
				MealSize = MenuSizeType.SMALL;
			else if (s != null && s.Text == "Medium")
				MealSize = MenuSizeType.MEDIUM;
			else if (s != null && s.Text == "Large")
				MealSize = MenuSizeType.LARGE;
		}

		RefreshCost();
	}

	void RefreshCost()
	{
		if (selectedEntree == null || selectedSide == null || selectedDrink == null)
		{
			lblCost.Text = "";
			return;
		}

		var combo = new Project.Models.ComboItem
		{
			Entree = selectedEntree,
			Side = selectedSide,
			Drink = selectedDrink
		};

		if (combo.Entree.HasSize)
			combo.Entree.Size = MealSize;
		if (combo.Side.HasSize)
			combo.Side.Size = MealSize;
		if (combo.Drink.HasSize)
			combo.Drink.Size = MealSize;

		lblCost.Text = $"${combo.GetCost():F2}";
	}

	private async void AddComboClicked(object sender, EventArgs e)
	{
		if (selectedEntree == null || selectedSide == null || selectedDrink == null)
		{
			await DisplayAlert("Error", "Error: You must select a combo, side, and drink.", "Ok");
			return;
		}

		var comboItem = new Project.Models.ComboItem
		{
			Entree = selectedEntree,
			Side = selectedSide,
			Drink = selectedDrink
		};

		// Set sizes
		if (comboItem.Entree.HasSize) comboItem.Entree.Size = MealSize;
		if (comboItem.Side.HasSize) comboItem.Side.Size = MealSize;
		if (comboItem.Drink.HasSize) comboItem.Drink.Size = MealSize;

		App.Cart.AddItem(comboItem);

		// Safely navigate back to main menu
		Application.Current.Windows[0].Page = new MainMenuPage();
	}

		// If you have CancelComboClicked:
	private void CancelComboClicked(object sender, EventArgs e)
	{
		
		App.Current.Windows[0].Page= new MainMenuPage();
	}


       
}