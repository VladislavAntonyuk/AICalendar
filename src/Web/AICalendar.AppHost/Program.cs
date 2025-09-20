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

var sqlServer = builder.AddSqlServer("sqlserver")
                       .WithLifetime(ContainerLifetime.Persistent)
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

if (!builder.Environment.IsDevelopment())
{
	var serviceBus = builder.AddRabbitMQ("servicebus")
							.WithManagementPlugin()
							.WithLifetime(ContainerLifetime.Persistent)
							.WithDataVolume("AICalendar-servicebus");

	apiService.WithReference(serviceBus);
}

await builder.Build().RunAsync();