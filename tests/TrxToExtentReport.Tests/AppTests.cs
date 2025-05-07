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
			Verbose = true
		};

		_app = new App(_options, new Logger<App>(new LoggerFactory()));
	}

	[Test]
	public async Task CreateReport()
	{
		await Shouldly.Should.NotThrowAsync(async () => await _app.Run(CancellationToken.None));

		File.Exists(_options.TrxFilePath).ShouldBeTrue();
		File.Delete(_options.TrxFilePath);
	}
}
