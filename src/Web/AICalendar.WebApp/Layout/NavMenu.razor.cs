using System.Reflection;
using System.Runtime.Versioning;
using AICalendar.WebApp.Components;

namespace AICalendar.WebApp.Layout;

public partial class NavMenu : AiCalendarBaseComponent
{
	private string? frameworkName = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
	}
}