using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record ResultSummary
{
	[XmlAttribute("outcome")]
	public string? Outcome { get; set; }

	[XmlElement("Counters")]
	public Counters? Counters { get; set; }

	[XmlElement("RunInfos")]
	public RunInfos? RunInfos { get; set; }

	[XmlElement("Output")]
	public Output? Output { get; set; }
}