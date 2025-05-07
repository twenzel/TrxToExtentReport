using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TrxToExtentReport.TrxReader.Models;

namespace TrxToExtentReport.TrxReader;

internal static partial class TrxDeserializer
{
	public static async Task<TestRun?> DeserializeFile(string filePath, CancellationToken cancellationToken)
	{
		var content = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
		return DeserializeContent(content);
	}

	public static TestRun? DeserializeContent(string fileContent)
	{
		var xmlNamespaceRegex = XmlNameSpaceFinder();
		var contentWithoutNamespace = xmlNamespaceRegex.Replace(fileContent, string.Empty);
		var xs = new XmlSerializer(typeof(TestRun));
		using var reader = new StringReader(contentWithoutNamespace);
		var testRun = (TestRun?)xs.Deserialize(reader);
		return testRun;
	}

	/// <summary>
	/// Deserializes test results from multiple TRX files and returns the combined unit test results. 
	/// </summary>
	/// <remarks>Metadata from the test runs is discarded, and only the test results are returned.</remarks>
	/// <param name="trxFileContents">The contents of one or more TRX files.</param>
	/// <returns>A merged list of UnitTestResult.</returns>
	public static IReadOnlyList<UnitTestResult> DeserializeTestResultsFromMultipleFiles(IEnumerable<string> trxFileContents) =>
		trxFileContents.Select(TrxDeserializer.DeserializeContent)
			.Where(t => t != null)
			.SelectMany(t => t!.Results!.UnitTestResults)
			.ToArray();

	/// <summary>
	/// Deserializes test results from multiple TRX files and returns the combined unit test results. 
	/// </summary>
	/// <remarks>Metadata from the test runs is discarded, and only the test results are returned.</remarks>
	/// <param name="trxFilePaths">The file paths to parse.</param>
	/// <returns>A merged list of UnitTestResult.</returns>
	public static IReadOnlyList<UnitTestResult> DeserializeTestResultsFromMultipleFilePaths(IEnumerable<string> trxFilePaths) =>
		DeserializeTestResultsFromMultipleFiles(trxFilePaths.Select(File.ReadAllText));

	[GeneratedRegex("xmlns=\".*?\" ?")]
	private static partial Regex XmlNameSpaceFinder();
}
