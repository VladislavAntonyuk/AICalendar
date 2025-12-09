# AICalendar

AICalendar is a cross-platform AI-powered calendar application built with .NET MAUI and Blazor. It helps users efficiently manage their schedules, appointments, and events with intelligent features and a modern user experience.

## Features

- **AI Scheduling Assistant**: Automatically suggests optimal meeting times and event slots based on your preferences and availability.
- **Multi-Platform Support**: Runs on Android, iOS, Windows, and Mac Catalyst devices.
- **Blazor Web Interface**: Access your calendar from any browser with a rich, interactive UI.
- **Event Management**: Create, edit, and delete events with ease.
- **Reminders & Notifications**: Get timely reminders for upcoming events and tasks.
- **Theme Support**: Switch between light and dark modes for comfortable viewing.
- **Localization & RTL Support**: Right-to-left layout and multi-language support for global users.
- **Secure Authentication**: Login and manage your calendar securely.
- **Cloud Sync**: Synchronize your calendar data across devices.
- **Modern UI**: Built with MudBlazor for a clean and responsive design.

---

For setup instructions and development notes, see the documentation and project files.

```
netstat -aon | findstr :5083
taskkill /PID 17980 /F
```

```
dotnet tool update --global dotnet-ef
dotnet tool install --local Microsoft.Extensions.AI.Evaluation.Console
dotnet tool run aieval report --path C:/TestReports --output report.html --open
```