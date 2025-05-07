using CommandLine;

namespace TrxToExtentReport;
public class Options
{

	[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
	public bool Verbose { get; set; }

	[Option('o', "output", Required = true, HelpText = "Output file for the report.")]
	public string OutputFile { get; internal set; }

	[Option('t', "trx", Required = true, HelpText = "Path to the TRX file.")]
	public string TrxFilePath { get; internal set; }

	[Option('e', "environment", Required = false, HelpText = "Environment name.")]
	public string? Environment { get; set; }
}
