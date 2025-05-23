# TrxToExtentReport

[![NuGet](https://img.shields.io/nuget/v/TrxToExtentReport.svg)](https://nuget.org/packages/TrxToExtentReport/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![CI](https://github.com/twenzel/TrxToExtentReport/actions/workflows/build.yml/badge.svg)](https://github.com/twenzel/TrxToExtentReport/actions/workflows/build.yml)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=security_rating)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=bugs)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=twenzel_TrxToExtentReport&metric=coverage)](https://sonarcloud.io/dashboard?id=twenzel_TrxToExtentReport)


A dotnet tool that converts .trx files (Microsoft Test Results) into a beautiful readable Html file (ExtentReport)

![msedge_LIVm2Zptr1](https://github.com/user-attachments/assets/801f67ec-6c55-4ba3-be7d-9b47c8698e08)

## Usage

Execute the tool with the following command:

```bash
dotnet TrxToExtentReport [options]
```

## Options

| Option | Description |
|--------|-------------|
| `-v, --verbose` | Set output to verbose messages. |
| `-o, --output` | The output file path for the generated report. |
| `-t, --trx` | Path to the TRX file. |
| `-d, --directory` | Path to a directory containing multiple TRX files. |
| `-e, --environment` | Environment name printed in the report. |

## Icons
<a href="https://www.flaticon.com/free-icons/report" title="report icons">Report icons created by Pixel perfect - Flaticon</a>
