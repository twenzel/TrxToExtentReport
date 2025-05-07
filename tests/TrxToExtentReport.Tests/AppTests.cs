using Microsoft.Extensions.Logging;
using Shouldly;

namespace TrxToExtentReport.Tests;
internal class AppTests
{
	private readonly App _app;
	private readonly Options _options;

	public AppTests()
	{
		_options = new Options
		{
			TrxFilePath = "../../../TrxReader/TestData/simple.trx",
			OutputFile = "path/to/output/report.html",
			Verbose = true,
			Environment = "QA"
		};

		_app = new App(_options, new Logger<App>(new LoggerFactory()));
	}

	[Test]
	public async Task CreateReport()
	{
		await Shouldly.Should.NotThrowAsync(async () => await _app.Run(CancellationToken.None));

		File.Exists(_options.OutputFile).ShouldBeTrue();
		File.Delete(_options.OutputFile);
	}

	[Test]
	public async Task CreateReport_From_Multiple_Files()
	{
		var options = new Options
		{
			TrxDirectory = "../../../TrxReader/TestData",
			OutputFile = "path/to/output/report2.html",
			Verbose = true,
			Environment = "QA"
		};

		var app = new App(options, new Logger<App>(new LoggerFactory()));

		await Shouldly.Should.NotThrowAsync(async () => await app.Run(CancellationToken.None));

		File.Exists(options.OutputFile).ShouldBeTrue();
		File.Delete(options.OutputFile);
	}
}
