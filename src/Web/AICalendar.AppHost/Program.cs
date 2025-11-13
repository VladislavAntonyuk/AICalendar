using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//var ollama = builder.AddOllama("ai")
//					.WithImage("ollama/ollama", "latest")
//					.WithLifetime(ContainerLifetime.Persistent)
//					.WithDataVolume("ollama")
//					.WithOpenWebUI(s => s.WithImage("ghcr.io/open-webui/open-webui", "0.5.20"))
//					.AddModel("llama3.2:latest"); // phi3.5:latest // llama3.2:latest

var cache = builder.AddRedis("cache")
                   .WithRedisInsight(s => s.WithLifetime(ContainerLifetime.Persistent))
				   .WithLifetime(ContainerLifetime.Persistent)
				   .WithDataVolume("AICalendar-cache");

var dbPassword = builder.AddParameter("dbPassword", () => "P@ssword123!", secret: true);
var sqlServer = builder
                .AddSqlServer("sqlserver", dbPassword, 1433)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithImageRegistry("mcr.microsoft.com")
                .WithImage("azure-sql-edge")
                .WithDataVolume("AICalendar-database");

var database = sqlServer.AddDatabase("database", "AICalendar");

var apiService = builder.AddProject<AICalendar_ApiService>("apiservice")
						.WithReference(database)
						.WaitFor(database)
						.WithReference(cache)
						.WaitFor(cache);
                        // .WithReference(ollama)
                        // .WaitFor(ollama);

builder.AddProject<AICalendar_WebApp>("webfrontend")
	   .WithExternalHttpEndpoints()
	   .WithReference(apiService)
	   .WaitFor(apiService)
	   .WithReference(cache)
	   .WaitFor(cache);
/*
// To easily reach your local API project from the
// emulator/Simulator/physical device, you can use the Dev Tunnels integration
var publicDevTunnel = builder.AddDevTunnel("devtunnel-public")
                             .WithAnonymousAccess()
                             .WithReference(apiService.GetEndpoint("https"));

// Add the .NET MAUI project resource
var mauiapp = builder.AddMauiProject("aicalendar-client", @"..\..\Client\AICalendar.Client\AICalendar.Client.csproj");

// Add MAUI app for Windows
mauiapp.AddWindowsDevice()
       .WithReference(apiService)
       .WaitFor(apiService);

// Add MAUI app for Mac Catalyst
mauiapp.AddMacCatalystDevice()
       .WithReference(apiService)
       .WaitFor(apiService);

// Add MAUI app for iOS running on the iOS Simulator (starts
// a random one, or uses the currently started one)
mauiapp.AddiOSSimulator()
       .WithOtlpDevTunnel() // Needed to get the OpenTelemetry data to "localhost"
       .WithReference(apiService, publicDevTunnel)// Needs a dev tunnel to reach "localhost"
       .WaitFor(apiService)
       .WaitFor(publicDevTunnel); 

// Add MAUI app for Android running on the emulator with
// default emulator (uses running or default emulator, needs to be started)
mauiapp.AddAndroidEmulator()
       .WithOtlpDevTunnel() // Needed to get the OpenTelemetry data to "localhost"
       .WithReference(apiService, publicDevTunnel)// Needs a dev tunnel to reach "localhost"
       .WaitFor(apiService)
       .WaitFor(publicDevTunnel);
*/
builder.AddProject<AICalendar_Client>("aicalendar-client")
	   .WithReference(apiService)
	   .WaitFor(apiService);

await builder.Build().RunAsync();