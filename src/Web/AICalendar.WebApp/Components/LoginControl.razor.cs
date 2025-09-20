using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace AICalendar.WebApp.Components;

public partial class LoginControl : AiCalendarBaseComponent
{
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	public void Logout()
	{
		NavigationManager.NavigateToLogout("authentication/logout");
	}
}