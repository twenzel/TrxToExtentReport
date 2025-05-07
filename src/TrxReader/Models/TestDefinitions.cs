using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record TestDefinitions
{
	[XmlElement("UnitTest")]
	public List<UnitTest> UnitTests { get; set; }
}

public record UnitTest
{
	[XmlAttribute("id")]
	public string Id { get; set; }

	[XmlAttribute("name")]
	public string Name { get; set; }

	[XmlAttribute("storage")]
	public string Storage { get; set; }

	[XmlElement("Execution")]
	public Execution Execution { get; set; }

	[XmlElement("TestMethod")]
	public TestMethod TestMethod { get; set; }
}

public record TestMethod
{
	[XmlAttribute("adapterTypeName")]
	public string AdapterTypeName { get; set; }

	[XmlAttribute("className")]
	public string ClassName { get; set; }

	[XmlAttribute("name")]
	public string Name { get; set; }

	[XmlAttribute("codeBase")]
	public string CodeBase { get; set; }
}

public record Execution
{
	[XmlAttribute("id")]
	public string Id { get; set; }
}