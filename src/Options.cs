using CommandLine;

namespace TrxToExtentReport;
public class Options
{

	[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
	public bool Verbose { get; set; }

	[Option('o', "output", Required = true, HelpText = "Output file for the report.")]
	public string OutputFile { get; set; }

	[Option('t', "trx", Required = false, HelpText = "Path to the TRX file.")]
	public string TrxFilePath { get; set; }

	[Option('d', "directory", Required = false, HelpText = "Path to a directory containing multiple TRX files.")]
	public string TrxDirectory { get; set; }

	[Option('e', "environment", Required = false, HelpText = "Environment name.")]
	public string? Environment { get; set; }
}
