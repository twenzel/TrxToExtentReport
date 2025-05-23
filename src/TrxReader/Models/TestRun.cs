using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

[XmlRoot(ElementName = "TestRun", Namespace = "")]
public record TestRun
{
	[XmlAttribute("id")]
	public string? Id { get; set; }

	[XmlAttribute("runUser")]
	public string? User { get; set; }

	[XmlAttribute("name")]
	public string? Name { get; set; }

	[XmlElement("Times")]
	public Times? Times { get; set; }

	[XmlElement("Results")]
	public Results? Results { get; set; }

	[XmlElement("ResultSummary")]
	public ResultSummary? ResultSummary { get; set; }

	[XmlElement("TestDefinitions")]
	public TestDefinitions? TestDefinitions { get; set; }
}
