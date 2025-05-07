using System.Xml.Serialization;

namespace TrxToExtentReport.TrxReader.Models;

public record Times
{
	[XmlAttribute(AttributeName = "creation")]
	public DateTimeOffset Creation { get; init; }

	[XmlAttribute(AttributeName = "queuing")]
	public DateTimeOffset Queuing { get; init; }

	[XmlAttribute(AttributeName = "start")]
	public DateTimeOffset Start { get; init; }

	[XmlAttribute(AttributeName = "finish")]
	public DateTimeOffset Finish { get; init; }
}
