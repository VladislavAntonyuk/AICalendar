using AICalendar.Client.Auth;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace AICalendar.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9cdnRcRWZfVUNzXENWYEg=");
		builder.ConfigureSyncfusionCore();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Configuration.AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), "appsettings.json", optional: false, false);

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<AuthPageViewModel>();
		builder.Services.AddOptions<AzureAdConfiguration>()
		        .Configure<IConfiguration>((options, configuration) =>
			                                   configuration.Bind(AzureAdConfiguration.SectionName, options));

		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddTransient<AuthHeaderHandler>();
		builder.Services.AddHttpClient("AuthClient", client =>
		{
			client.BaseAddress = new Uri("https://localhost:7118/api/v1/");
		}).AddHttpMessageHandler<AuthHeaderHandler>();

		builder.Services.AddTransientPopup<CalendarEventPopup, CalendarEventPopupViewModel>();
#if DEBUG
		builder.AddServiceDefaults();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
