using DotNetEnv;
namespace Player;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	//Загрузка .env
    private async Task LoadEnvAsync()
    {
        try
        {
            //Место нахождения .env
            string tempPath = Path.Combine(FileSystem.CacheDirectory, ".env");
            //Отправка в Raw
            using var stream = await FileSystem.OpenAppPackageFileAsync(".env");
            using var output = File.Create(tempPath);
            await stream.CopyToAsync(output);

            //Загрузка
            Env.Load(tempPath);
            Console.WriteLine(".env загружен из: " + tempPath);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
    }
}
