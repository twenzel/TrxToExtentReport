using System.Globalization;
using System.Reflection;
using Shouldly;
using TrxToExtentReport.TrxReader;

namespace TrxToExtentReport.Tests.TrxReader;

public class TrxDeserializerTests
{
	[Test]
	public void Can_Parse_Trx_File()
	{
		var fileContent = GetTrxContent("simple.trx");

		var testRun = TrxDeserializer.DeserializeContent(fileContent);

		testRun.ShouldNotBeNull();
		testRun.Results.ShouldNotBeNull();
		testRun.Results.UnitTestResults.ShouldNotBeNull();
		testRun.Results.UnitTestResults.ShouldNotBeEmpty();
	}

	[Test]
	public void Can_Parse_Trx_File_With_All_Details()
	{
		var fileContent = GetTrxContent("simple.trx");

		var testRun = TrxDeserializer.DeserializeContent(fileContent);

		testRun.ShouldNotBeNull();
		testRun.Id.ShouldBe("79a9d519-77bf-4efb-982e-2cb8d5750c67");
		testRun.User.ShouldBe("testuser");
		testRun.Name.ShouldBe("@33c8d6e82111 2025-04-30 07:17:21");

		testRun.Times.ShouldNotBeNull();
		testRun.Times.Creation.ShouldBe(DateTimeOffset.Parse("2025-04-30T07:17:21.5877997+00:00", CultureInfo.InvariantCulture));

		testRun.Results.ShouldNotBeNull();
		testRun.Results.UnitTestResults.ShouldNotBeNull();
		testRun.Results.UnitTestResults.ShouldNotBeEmpty();

		var testResult = testRun.Results.UnitTestResults[0];
		testResult.Id.ShouldBe("07259b0a-5a07-42a5-ae76-5afcc3e564f9");
		testResult.TestId.ShouldBe("b8b8e7ad-fb95-4b9f-4252-789b4908fa3b");
		testResult.TestName.ShouldBe("Return_Groups_And_Attributes");
		testResult.ComputerName.ShouldBe("33c8d6e82111");
		testResult.Duration.ShouldBe("00:00:00.3022210");
		var duration = TimeOnly.Parse(testResult.Duration, CultureInfo.InvariantCulture);
		testResult.StartTime.ShouldBe(DateTimeOffset.Parse("2025-04-30T07:17:28.6655516+00:00", CultureInfo.InvariantCulture));
		testResult.EndTime.ShouldBe(DateTimeOffset.Parse("2025-04-30T07:17:28.9677725+00:00", CultureInfo.InvariantCulture));
		testResult.TestTypeId.ShouldBe("13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b");
		testResult.Outcome.ShouldBe("Passed");
		testResult.TestListId.ShouldBe("8c84fa94-04c1-424b-9868-57a2d4851a1d");
	}

	private static string GetTrxContent(string fileName)
	{
		var assembly = Assembly.GetExecutingAssembly();
		var resourceName = $"TrxToExtentReport.Tests.TrxReader.TestData.{fileName}";

		using var stream = assembly.GetManifestResourceStream(resourceName)
			?? throw new InvalidOperationException("No test data file found!");

		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}
