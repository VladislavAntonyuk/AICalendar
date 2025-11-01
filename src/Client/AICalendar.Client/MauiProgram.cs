using AICalendar.Client.Application.Auth;
using AICalendar.Client.Application.Calendar.EventDetails;
using AICalendar.Client.Application.Calendar.Main;
using AICalendar.Client.ServiceDefaults;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;

namespace AICalendar.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9cdnRcRWZfVUNzXENWYEg=");

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionCore();

		builder.Configuration.AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), "appsettings.json", optional: false, false);

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<AuthPageViewModel>();
		builder.Services.Configure<AzureAdConfiguration>(builder.Configuration.GetSection(AzureAdConfiguration.SectionName));

		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddTransient<AuthHeaderHandler>();
		builder.Services.AddHttpClient("AuthClient", client =>
		{
			client.BaseAddress = new Uri("https://apiservice/api/v1/");
		}).AddHttpMessageHandler<AuthHeaderHandler>();

		builder.Services.AddTransientPopup<CalendarEventPopup, CalendarEventPopupViewModel>();
#if DEBUG
		builder.AddServiceDefaults();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
