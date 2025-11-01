using System.Security.Claims;
using Microsoft.Identity.Web;

namespace AICalendar.ApiService.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
	extension(ClaimsPrincipal claims)
	{
		public Guid GetUserId()
		{
			Guid.TryParse(claims.GetObjectId(), out var currentUserId);
			return currentUserId;
		}
	}
}