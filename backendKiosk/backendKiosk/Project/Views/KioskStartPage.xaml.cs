namespace backendKiosk.Project.Views;

public partial class KioskStartPage : ContentPage
{
	public KioskStartPage()
	{
		InitializeComponent();
	}

    private void StartOrderButton(object sender, EventArgs e)
    {
		if (Application.Current?.Windows.Count > 0)
		{
			Application.Current.Windows[0].Page = new MainMenuPage();
		}
		else
		{
			DisplayAlert("Error", "Navigation failed. Please try again.", "Ok");
		}
    }
}