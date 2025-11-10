using Microsoft.Extensions.Logging;
using DotNetEnv;

namespace Player;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

		var envPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
		Env.Load(envPath);

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
