using Microsoft.AspNetCore.Components;

namespace AICalendar.WebApp.Components;

public partial class Promo : AiCalendarBaseComponent
{
	[Inject]
	public required NavigationManager NavigationManager { get; set; }

	private void SignIn()
	{
		NavigationManager.NavigateTo("authentication/login", true);
	}

}