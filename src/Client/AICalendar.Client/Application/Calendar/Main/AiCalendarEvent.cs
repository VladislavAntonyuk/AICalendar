using Syncfusion.Maui.Scheduler;

namespace AICalendar.Client.Application.Calendar.Main;

public class AiCalendarEvent : SchedulerAppointment
{
	public Guid OrganizerId { get; set; }
}