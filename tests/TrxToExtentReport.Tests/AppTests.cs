using Microsoft.Extensions.Logging;

namespace TrxToExtentReport.Tests;
internal class AppTests
{
	private readonly App _app;
	private readonly Options _options;

	public AppTests()
	{
		_options = new Options
		{
			TrxFilePath = "path/to/trx/file.trx",
			OutputFile = "path/to/output/report.html",
			Verbose = true
		};

		_app = new App(_options, new Logger<App>(new LoggerFactory()));
	}

	[Test]
	public async Task CreateReport()
	{
		await _app.Run(CancellationToken.None);
	}
}
