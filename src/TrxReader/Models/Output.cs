using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record Output
{
	[XmlElement("StdOut")]
	public string? StdOut { get; set; }

	[XmlElement("StdErr")]
	public string? StdErr { get; set; }

	[XmlElement("ErrorInfo")]
	public ErrorInfo? ErrorInfo { get; set; }
}