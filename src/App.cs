using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using Microsoft.Extensions.Logging;
using TrxToExtentReport.TrxReader.Models;

namespace TrxToExtentReport;

internal class App
{
	private readonly Options _options;
	private readonly ILogger<App> _logger;

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

		// group the tests by their unique/full qualified method name
		// because same tests with different parameters should be grouped 
		foreach (var testGroup in testRun.Results.UnitTestResults.GroupBy(t => GetUniqueMethodName(t, testRun)))
		{
			if (testGroup.Key == null)
				continue;

			var testName = testGroup.Key.TestName;
			var description = GetTestDescription(testName);
			var extentTest = extent.CreateTest(testName, description);

			if (!string.IsNullOrEmpty(testGroup.Key.FullqualifiedMethodName))
				extentTest.Info($"Method name: {testGroup.Key.FullqualifiedMethodName}");

			if (testGroup.Count() == 1)
				AddSingleTestResult(extentTest, testGroup.First());
			else
				AddMultipleTestResults(extentTest, testGroup);

			if (!string.IsNullOrEmpty(testGroup.Key.Namespace))
				extentTest.AssignCategory(testGroup.Key.Namespace);
		}
	}

	private static void AddMultipleTestResults(ExtentTest extentTest, IGrouping<TestInfo?, UnitTestResult> testGroup)
	{
		foreach (var test in testGroup)
		{
			var testParameter = ExtractTestParameter(test.TestName);
			var node = extentTest.CreateNode(testParameter);

			AddSingleTestResult(node, test);
		}
	}

	private static void AddSingleTestResult(ExtentTest extentTest, UnitTestResult test)
	{
		var testParameter = ExtractTestParameter(test.TestName);
		if (!string.IsNullOrEmpty(testParameter))
			extentTest.Info($"Parameter: {testParameter}");

		var testStatus = test.Outcome;
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

		extentTest.Model.StartTime = test.StartTime.LocalDateTime;
		extentTest.Model.EndTime = test.EndTime.LocalDateTime;

		if (test.Output != null)
		{
			if (!string.IsNullOrEmpty(test.Output.StdOut))
				extentTest.Info($"StdOut: {test.Output.StdOut.ReplaceLineEndings("<br/>")}");

			if (!string.IsNullOrEmpty(test.Output.StdErr))
				extentTest.Info($"StdErr: {test.Output.StdErr}");

			if (test.Output.ErrorInfo != null)
			{
				if (!string.IsNullOrEmpty(test.Output.ErrorInfo.Message))
					extentTest.Fail(test.Output.ErrorInfo.Message);

				if (!string.IsNullOrEmpty(test.Output.ErrorInfo.StackTrace))
					extentTest.Fail(MarkupHelper.CreateCodeBlock(test.Output.ErrorInfo.StackTrace));
			}
		}
	}

	private static TestInfo? GetUniqueMethodName(UnitTestResult test, TestRun testRun)
	{
		var unitTest = testRun.TestDefinitions?.UnitTests.FirstOrDefault(x => x.Id == test.TestId);

		if (unitTest != null)
		{
			var className = unitTest.TestMethod.ClassName;
			var methodName = RemoveParameter(unitTest.TestMethod.Name);
			var testNamespace = string.Empty;

			var assemblyName = Path.GetFileNameWithoutExtension(unitTest.TestMethod.CodeBase);

			if (className.StartsWith(assemblyName))
			{
				testNamespace = className.Substring(assemblyName.Length + 1);
				var index = testNamespace.IndexOf('.');
				if (index > 0)
					testNamespace = testNamespace.Substring(0, index);
			}

			return new TestInfo { FullqualifiedMethodName = $"{className}.{methodName}", TestName = RemoveParameter(test.TestName), Namespace = testNamespace };
		}

		return null;
	}

	private static string RemoveParameter(string name)
	{
		var index = name.IndexOf('(');
		if (index > 0)
			name = name.Substring(0, index);

		return name;
	}

	private static string? GetTestDescription(string testName)
	{
		if (testName.Contains('_'))
			return testName.Replace('_', ' ');

		return null;
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

	private record TestInfo
	{
		public string TestName { get; set; }
		public string FullqualifiedMethodName { get; set; }

		public string? Namespace { get; set; }
	}
}
