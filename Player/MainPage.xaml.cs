namespace Player;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void Register(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new SecondPage("Регистрация"));
    }
}