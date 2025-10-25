using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AICalendar.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton(s => new HttpClient()
		{
			BaseAddress = new Uri("https://localhost:7118/api/v1/")
		});

#if DEBUG
		builder.AddServiceDefaults();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
