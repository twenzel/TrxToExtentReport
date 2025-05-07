using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record Results
{
	[XmlElement("UnitTestResult")]
	public List<UnitTestResult> UnitTestResults { get; set; }
}

public record UnitTestResult
{
	[XmlAttribute("executionId")]
	public string Id { get; init; } = string.Empty;

	[XmlAttribute("parentExecutionId")]
	public string ParentId { get; init; } = string.Empty;

	[XmlAttribute("testId")]
	public string TestId { get; init; } = string.Empty;

	[XmlAttribute("testListId")]
	public string TestListId { get; init; } = string.Empty;

	[XmlAttribute("testName")]
	public string TestName { get; init; } = string.Empty;

	[XmlAttribute("computerName")]
	public string ComputerName { get; init; } = string.Empty;

	[XmlAttribute("duration")]
	public string Duration { get; init; } = string.Empty;

	[XmlAttribute("startTime")]
	public DateTimeOffset StartTime { get; set; }

	[XmlAttribute("endTime")]
	public DateTimeOffset EndTime { get; set; }

	[XmlAttribute("testType")]
	public string TestTypeId { get; init; } = string.Empty;

	[XmlAttribute("relativeResultsDirectory")]
	public string RelativeResultsDirectoryId { get; init; } = string.Empty;

	[XmlAttribute("outcome")]
	public string Outcome { get; init; } = string.Empty;

	[XmlAttribute("resultType")]
	public string ResultType { get; init; } = string.Empty;

	[XmlAttribute("dataRowInfo")]
	public string DataRowInfo { get; init; } = string.Empty;

	[XmlElement("Output")]
	public Output? Output { get; set; }

	[XmlElement("InnerResults")]
	public Results? InnerResults { get; set; }
}