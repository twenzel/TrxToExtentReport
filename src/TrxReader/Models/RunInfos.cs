using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record RunInfos
{
	[XmlElement("RunInfo")]
	public List<RunInfo> Infos { get; set; }
}

public record RunInfo
{
	[XmlElement("Text")]
	public string? Text { get; set; }

	[XmlElement("Exception")]
	public string? Exception { get; set; }

	[XmlAttribute("computerName")]
	public string? ComputerName { get; set; }

	[XmlAttribute("outcome")]
	public string? Outcome { get; set; }

	[XmlAttribute("timestamp")]
	public string? Timestamp { get; set; }
}