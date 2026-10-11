namespace backendKiosk.Project.Views;

public partial class KioskStartPage : ContentPage
{
    private bool _navigating;

    public KioskStartPage()
    {
        InitializeComponent();
        SizeChanged += (_, _) => UpdateLayout();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Application.Current is not null) Application.Current.UserAppTheme = AppTheme.Dark;
        CustomerNameEntry.Text = App.Cart.CustomerName;
        UpdateLayout();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (Application.Current is not null) Application.Current.UserAppTheme = AppTheme.Light;
    }

    private void UpdateLayout()
    {
        if (Width <= 0) return;
        bool narrow = Width < 780;
        HeroGrid.ColumnSpacing = narrow ? 0 : 36;
        StepsGrid.ColumnSpacing = narrow ? 0 : 28;
        WelcomeLayout.Padding = new Thickness(narrow ? 24 : 48, 28);
        HeroGrid.ColumnDefinitions[0].Width = narrow ? GridLength.Star : new GridLength(0.95, GridUnitType.Star);
        HeroGrid.ColumnDefinitions[1].Width = narrow ? new GridLength(0) : new GridLength(1.05, GridUnitType.Star);
        Grid.SetColumn(HeroPhoto, narrow ? 0 : 1);
        Grid.SetRow(HeroPhoto, narrow ? 1 : 0);
        HeroPhoto.HeightRequest = narrow ? 250 : 440;
        OrderForm.WidthRequest = Math.Min(390, Math.Max(200, narrow ? Width - 48 : (Math.Min(Width, 1440) - 132) * 0.475));
        var contentWidth = Math.Min(Width, 1440);
        HeroHeading.FontSize = Math.Round(narrow
            ? Math.Clamp((contentWidth - 48) * 0.13, 40, 52)
            : Math.Clamp((contentWidth - 96) * 0.06, 42, 68));
        FooterGrid.ColumnDefinitions[1].Width = narrow ? new GridLength(0) : GridLength.Auto;
        Grid.SetColumn(FooterNote, narrow ? 0 : 1);
        Grid.SetRow(FooterNote, narrow ? 1 : 0);
        Grid.SetColumn(StepTwo, narrow ? 0 : 1);
        Grid.SetColumn(StepThree, narrow ? 0 : 2);
        Grid.SetRow(StepTwo, narrow ? 1 : 0);
        Grid.SetRow(StepThree, narrow ? 2 : 0);
        StepsGrid.ColumnDefinitions[1].Width = narrow ? new GridLength(0) : GridLength.Star;
        StepsGrid.ColumnDefinitions[2].Width = narrow ? new GridLength(0) : GridLength.Star;
    }

    private async void StartOrderButton(object sender, EventArgs e)
    {
        if (_navigating) return;
        _navigating = true;
        StartButton.IsEnabled = false;
        try
        {
            App.Cart.CustomerName = CustomerNameEntry.Text?.Trim() ?? "";
            await Shell.Current.GoToAsync("///menu");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Unable to open menu", ex.Message, "OK");
        }
        finally
        {
            _navigating = false;
            StartButton.IsEnabled = true;
        }
    }

    private async void AdminButton(object sender, EventArgs e)
    {
        try { await Shell.Current.GoToAsync("///dashboard"); }
        catch (Exception ex) { await DisplayAlert("Unable to open dashboard", ex.Message, "OK"); }
    }
}
