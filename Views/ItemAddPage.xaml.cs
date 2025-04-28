namespace RestaurantAppFullImp.Project.Views;
using Project.Models;
using System.Collections.ObjectModel;

/*
    Hint: A class called ItemSelectView is defined that "wraps" around the data model and provides properties 
    that are referenced in the collection view's data template.  The collection view should be an observable
    list of ItemSelectView objects that bind to the menu items being shown.
*/


public partial class ItemAddPage : ContentPage
{
	//Add any page attributes here...


    public ItemAddPage(MenuItemType type)
	{
		InitializeComponent();
		
        //add constructor logic here.
    }

    private void SelectItem(object sender, SelectionChangedEventArgs e)
    {
        //This method is called when an item is selected in the Collection View...
    }

    private void SelectSize(object sender, SelectionChangedEventArgs e)
    {
        //This method is called when a new size is selected...
 
    }

    private async void AddItemClicked(object sender, EventArgs e)
    {
        //This method is called when the Add Item button is clicked.
    }

}