using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record ErrorInfo
{
	[XmlElement("Message")]
	public string Message { get; init; } = string.Empty;

	[XmlElement("StackTrace")]
	public string? StackTrace { get; set; }
}