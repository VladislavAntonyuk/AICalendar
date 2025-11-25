using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;
using Microsoft.Extensions.Configuration;
using OpenAI;

namespace AICalendar.ApiService.Tests;

public class AICalendarApiServiceTests
{
	private static readonly IList<ChatMessage> Messages =
	[
		new(ChatRole.System, """
		                     You're an AI assistant that can answer questions related to astronomy.
		                     Keep your responses concise and try to stay under 100 words.
		                     Use the imperial measurement system for all measurements in your response.
		                     """),
		new(ChatRole.User, "How far is the planet Venus from Earth at its closest and furthest points?")
	];

	private static readonly ReportingConfiguration DefaultReportingConfiguration =
		DiskBasedReportingConfiguration.Create("C:\\TestReports", GetEvaluators(), GetAzureOpenAIChatConfiguration(),
		                                       true, executionName: ExecutionName);

	private static string ScenarioName =>
		$"{TestContext.Current.Test?.TestDisplayName}.{TestContext.Current.Test?.UniqueID}";

	private static string ExecutionName => $"{DateTime.Now:yyyyMMddTHHmmss}";

	private static IEnumerable<IEvaluator> GetEvaluators()
	{
		IEvaluator relevanceEvaluator = new RelevanceEvaluator();
		// CoherenceEvaluator measures the readability and user-friendliness of the response being evaluated. It assesses the ability of an AI system to generate text that reads naturally, flows smoothly, and resembles human-like language in its responses.
		IEvaluator coherenceEvaluator = new CoherenceEvaluator();
		IEvaluator wordCountEvaluator = new WordCountEvaluator();

		return [relevanceEvaluator, coherenceEvaluator, wordCountEvaluator];
	}

	[Fact]
	public async Task Test1()
	{
		var chatConfiguration = GetAzureOpenAIChatConfiguration();

		var chatOptions = new ChatOptions
		{
			Temperature = 0.0f,
			ResponseFormat = ChatResponseFormat.Text
		};

		var response =
			await chatConfiguration.ChatClient.GetResponseAsync(Messages, chatOptions,
			                                                      TestContext.Current.CancellationToken);

		await using var scenarioRun = await DefaultReportingConfiguration.CreateScenarioRunAsync(
			ScenarioName, additionalTags: ["Moon"], cancellationToken: TestContext.Current.CancellationToken);

		List<EvaluationContext> additionalContext =
		[
			new GroundednessEvaluatorContext(""" 
			                                 Distance between Mars and Earth at inferior conjunction: Between 33.9 and 62.1 million miles approximately. 
			                                 Distance between Mars and Earth at superior conjunction: Between 249 and 250 million miles approximately. 
			                                 The exact distances can vary due to the specific orbital positions of the planets at any given time. 
			                                 """)
		];
		var result =
			await scenarioRun.EvaluateAsync(Messages, response, additionalContext,
			                                TestContext.Current.CancellationToken);

		// Retrieve the score for relevance from the <see cref="EvaluationResult"/>.
		var relevance = result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
		Assert.False(relevance.Interpretation?.Failed, relevance.Reason);
		Assert.True(relevance.Interpretation?.Rating is EvaluationRating.Good or EvaluationRating.Exceptional);

		// Retrieve the score for coherence from the <see cref="EvaluationResult"/>.
		var coherence = result.Get<NumericMetric>(CoherenceEvaluator.CoherenceMetricName);
		Assert.False(coherence.Interpretation?.Failed, coherence.Reason);
		Assert.True(coherence.Interpretation?.Rating is EvaluationRating.Good or EvaluationRating.Exceptional);

		// Retrieve the word count from the <see cref="EvaluationResult"/>.
		var wordCount = result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);
		Assert.False(wordCount.Interpretation?.Failed, wordCount.Reason);
		Assert.True(wordCount.Interpretation?.Rating is EvaluationRating.Good or EvaluationRating.Exceptional);
		Assert.False(wordCount.ContainsDiagnostics());
		Assert.True(wordCount.Value is > 5 and <= 100);
	}

	private static ChatConfiguration GetAzureOpenAIChatConfiguration()
	{
		var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
		                                       .AddJsonFile("appsettings.Development.json", true)
		                                       .Build();

		var endpoint = config["AI:Endpoint"];
		var model = config["AI:Model"];
		var apiKey = config["AI:Key"];

		var aiClient = new OpenAIClient(new ApiKeyCredential(apiKey),
		                                new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
		var client = aiClient.GetChatClient(model).AsIChatClient();

		return new ChatConfiguration(client);
	}
}