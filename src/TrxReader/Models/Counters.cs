using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record Counters
{
	[XmlAttribute("total")]
	public int Total { get; set; }

	[XmlAttribute("Executed")]
	public int Executed { get; set; }

	[XmlAttribute("passed")]
	public int Passed { get; set; }

	[XmlAttribute("Failed")]
	public int Failed { get; set; }

	[XmlAttribute("error")]
	public int Error { get; set; }

	[XmlAttribute("inconclusive")]
	public int Inconclusive { get; set; }
}