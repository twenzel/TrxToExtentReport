using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrxToExtentReport;

static class Program
{
	static async Task<int> Main(string[] args)
	{
		try
		{
			var result = await Parser.Default.ParseArguments<Options>(args)
				.WithParsedAsync(RunOptions);

			return result.Tag == ParserResultType.Parsed ? 0 : 1;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Tool terminated unexpectedly: {ex.Message}");
			return 1;
		}
	}

	static async Task RunOptions(Options opts)
	{
		var host = CreateHostBuilder(opts).Build();
		var app = host.Services.GetRequiredService<App>();
		await app.Run(CancellationToken.None);
	}

	public static IHostBuilder CreateHostBuilder(Options opts) =>
		Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				ConfigureServices(services, opts);
			})
		.ConfigureLogging(builder =>
		{
			builder.AddConsole();

			if (opts.Verbose)
				builder.SetMinimumLevel(LogLevel.Debug);
		});


	private static void ConfigureServices(IServiceCollection services, Options opts)
	{
		services.AddSingleton<App>();
		services.AddSingleton(opts);

	}
}