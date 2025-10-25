using Microsoft.Extensions.Hosting;
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

//builder.AddProject<Projects.AICalendar_Client>("aicalendar-client")
//       .WithReference(apiService)
//       .WaitFor(apiService);

await builder.Build().RunAsync();