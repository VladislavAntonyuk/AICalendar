using AICalendar.Client.Application.Auth;
using AICalendar.Client.Application.Calendar.EventDetails;
using AICalendar.Client.Application.Calendar.Main;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;

namespace AICalendar.Client;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF5cX2NCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH1cdXVUR2VZVkNyWkdWYEs=");

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionCore();

		builder.Configuration.AddJsonFile(new EmbeddedFileProvider(typeof(App).Assembly, typeof(App).Namespace), "appsettings.json", optional: false, false);

		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<AuthPageViewModel>();
		builder.Services.Configure<AzureAdConfiguration>(builder.Configuration.GetSection(AzureAdConfiguration.SectionName));
		var baseUrl = builder.Configuration.GetValue<Uri>("ApiUrl");

		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddTransient<AuthHeaderHandler>();
		builder.Services.AddHttpClient("AuthClient", client =>
		{
			client.BaseAddress = baseUrl;
		}).AddHttpMessageHandler<AuthHeaderHandler>();

		builder.Services.AddTransientPopup<CalendarEventPopup, CalendarEventPopupViewModel>();
#if DEBUG
		builder.AddServiceDefaults();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
