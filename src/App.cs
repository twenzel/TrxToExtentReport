using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Extensions.Logging;
using TrxToExtentReport.TrxReader.Models;

namespace TrxToExtentReport;

internal class App
{
	private readonly Options _options;
	private readonly ILogger<App> _logger;

	private readonly static System.Reflection.FieldInfo s_stackTrace = typeof(Exception).GetPrivateField("_stackTraceString") ??
		throw new InvalidOperationException("Could not find the private field _stackTraceString in Exception class.");

	public App(Options options, ILogger<App> logger)
	{
		_options = options ?? throw new ArgumentNullException(nameof(options));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		// read the trx file
		_logger.LogInformation("Reading TRX file: {TrxFilePath}", _options.TrxFilePath);
		var trxFilePath = _options.TrxFilePath;

		if (!Path.IsPathRooted(trxFilePath))
			trxFilePath = Path.GetFullPath(trxFilePath);

		if (!File.Exists(trxFilePath))
		{
			_logger.LogError("TRX file not found: {TrxFilePath}", trxFilePath);
			return;
		}

		var testRun = await TrxReader.TrxDeserializer.DeserializeFile(trxFilePath, cancellationToken)
			?? throw new InvalidOperationException("Could not deserialize trx file.");

		var outputFile = _options.OutputFile;

		if (!Path.IsPathRooted(outputFile))
			outputFile = Path.GetFullPath(outputFile);

		var outputDirectory = Path.GetDirectoryName(outputFile);

		if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
			Directory.CreateDirectory(outputDirectory);

		var htmlReporter = new ExtentSparkReporter(outputFile);
		var extent = new ExtentReports();
		extent.AttachReporter(htmlReporter);

		if (!string.IsNullOrEmpty(_options.Environment))
			extent.AddSystemInfo("Environment", _options.Environment);

		if (!string.IsNullOrEmpty(testRun.User))
			extent.AddSystemInfo("UserName", testRun.User);

		AddTestResults(testRun, extent);

		if (testRun.Times != null)
		{
			extent.Report.StartTime = testRun.Times.Start.LocalDateTime;
			extent.Report.EndTime = testRun.Times.Finish.LocalDateTime;
		}

		if (testRun.ResultSummary != null)
		{
			if (testRun.ResultSummary.Output?.StdOut != null)
				extent.AddTestRunnerLogs(testRun.ResultSummary.Output.StdOut);

			if (testRun.ResultSummary.Output?.StdErr != null)
				extent.AddTestRunnerLogs(testRun.ResultSummary.Output.StdErr);

			if (testRun.ResultSummary.RunInfos != null)
			{
				foreach (var runInfo in testRun.ResultSummary.RunInfos.Infos)
				{
					if (!string.IsNullOrEmpty(runInfo.Text))
						extent.AddTestRunnerLogs(runInfo.Text);

					if (!string.IsNullOrEmpty(runInfo.Exception))
						extent.AddTestRunnerLogs(runInfo.Exception);
				}
			}
		}

		_logger.LogInformation("Writing file...");
		extent.Flush();

		_logger.LogInformation("Report generated: {OutputFile}", outputFile);
	}

	private static void AddTestResults(TestRun testRun, ExtentReports extent)
	{
		if (testRun.Results == null)
			return;

		var hostNames = testRun.Results.UnitTestResults.Select(x => x.ComputerName).Distinct().ToList();

		if (hostNames != null && hostNames.Count > 0)
			extent.AddSystemInfo("Host Name", string.Join(", ", hostNames));



		foreach (var test in testRun.Results.UnitTestResults)
		{
			var unitTest = testRun.TestDefinitions?.UnitTests.FirstOrDefault(x => x.Id == test.TestId);

			var testName = GetTestName(test.TestName);
			var description = GetTestDescription(testName);
			var testStatus = test.Outcome;
			var extentTest = extent.CreateTest(testName, description);

			var testParameter = ExtractTestParameter(test.TestName);
			if (!string.IsNullOrEmpty(unitTest?.TestMethod.ClassName))
				extentTest.Info($"Class name: {unitTest?.TestMethod.ClassName}");
			if (!string.IsNullOrEmpty(testParameter))
				extentTest.Info($"Parameter: {testParameter}");

			switch (testStatus)
			{
				case "Passed":
					extentTest.Pass("Test passed");
					break;
				case "Failed":
					extentTest.Fail("Test failed");
					break;
				default:
					extentTest.Warning($"Test status unknown ({testStatus})");
					break;
			}

			extentTest.Test.StartTime = test.StartTime.LocalDateTime;
			extentTest.Test.EndTime = test.EndTime.LocalDateTime;

			var category = GetTestNamespace(unitTest);

			if (category != null)
				extentTest.AssignCategory(category);

			if (test.Output != null)
			{
				if (!string.IsNullOrEmpty(test.Output.StdOut))
					extentTest.Info($"StdOut: {test.Output.StdOut.ReplaceLineEndings("<br/>")}");

				if (!string.IsNullOrEmpty(test.Output.StdErr))
					extentTest.Info($"StdErr: {test.Output.StdErr}");

				if (test.Output.ErrorInfo != null)
					extentTest.Fail(CreateException(test.Output.ErrorInfo));
			}
		}
	}

	private static Exception CreateException(ErrorInfo errorInfo)
	{
		var exception = new Exception(errorInfo.Message);

		if (errorInfo.StackTrace != null)
			s_stackTrace.SetValue(exception, errorInfo.StackTrace);

		return exception;
	}

	private static string? GetTestNamespace(UnitTest? unitTest)
	{
		if (unitTest != null)
		{
			var assemblyName = Path.GetFileNameWithoutExtension(unitTest.TestMethod.CodeBase);
			var className = unitTest.TestMethod.ClassName;

			if (className.StartsWith(assemblyName))
			{
				className = className.Substring(assemblyName.Length + 1);
				var index = className.IndexOf('.');
				if (index > 0)
					return className.Substring(0, index);
			}
		}

		return null;
	}

	private static string? GetTestDescription(string testName)
	{
		if (testName.Contains('_'))
			return testName.Replace('_', ' ');

		return null;
	}

	private static string GetTestName(string testName)
	{
		var index = testName.IndexOf('(');

		if (index > 0)
			return testName.Substring(0, index);

		return testName;
	}

	private static string? ExtractTestParameter(string testName)
	{
		var index = testName.IndexOf('"');

		if (index > 0)
		{
			var endIndex = testName.IndexOf('"', index + 1);

			if (endIndex > 0)
			{
				var parameter = testName.Substring(index + 1, endIndex - index - 1);
				return parameter;
			}
		}

		return null;
	}
}
