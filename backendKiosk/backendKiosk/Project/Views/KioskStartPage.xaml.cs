namespace backendKiosk.Project.Views;

public partial class KioskStartPage : ContentPage
{
	public KioskStartPage()
	{
		InitializeComponent();
	}

    private async void StartOrderButton(object sender, EventArgs e)
    {
		try
		{
			// Simple navigation to the menu page
			await Shell.Current.GoToAsync("///menu");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
		}
    }
	
	private async void AdminButton(object sender, EventArgs e)
    {
		try
		{
			// Simple navigation to the dashboard page
			await Shell.Current.GoToAsync("///dashboard");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Navigation failed: {ex.Message}", "OK");
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
		}
    }
}