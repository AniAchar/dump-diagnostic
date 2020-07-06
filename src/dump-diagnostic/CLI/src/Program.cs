using AnalysisEngine.Engines;
using ReportEngine;
using System;
using System.IO;
using AnalysisEngine;
using Microsoft.Diagnostics.Runtime;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.IO;
using System.Threading.Tasks;
using ReportEngine.ReportRenderer;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace DumpDiagnostic
{
    class Program
    {


        public static Task<int> Main(string[] args)
        {
            var parser = new CommandLineBuilder()
                .AddCommand(MemoryDiagCommand())
                .AddCommand(CrashDiagCommand())
                .UseAnsiTerminalWhenAvailable()
                .UseDefaults()
                .Build();
            return parser.InvokeAsync(args);

        }


        public enum ReportTypes
        {
            //TODO: Implement the HTML rendering options.
            console,
        }
        public enum DiagnosisType
        {
            memory,
            crash
        }

        private static void CheckArguments(FileInfo dumpPath, DiagnosisType diagnose, ReportTypes reportType)
        {
            if (dumpPath == null)
            {
                Console.WriteLine("Missing argument(s). Please use dump-diagnostic -h to look at all the required arguments");
                Environment.Exit(-1);
            }
        }

        private static Option DumpPathOption() =>
            new Option(
                aliases: new[] { "-dp", "--dump-path" },
                description: "The absolute file path to the dump file that needs to be diagnosed.")
            {
                Argument = new Argument<FileInfo>(name: "dump path")
            };

        private static Option VerboseOptions() =>
            new Option(
                aliases: new[] { "-v", "--verbose" },
                description: "Get verbose logs to help debug the app.")
            {
                Argument = new Argument<bool>(name: "verbose")
            };

        private static Option ReportOptions() =>
            new Option(
                aliases: new[] { "-r", "--report" },
                description: "Generates a report of a specific format.")
            {
                Argument = new Argument<ReportTypes>(name: "report")
            };

        private static Command MemoryDiagCommand()
        {
            var cmd = new Command(
                    name: "memory",
                    description: "Runs a memory diagnosis on the dumps and shows dotnet memory profile of the dump.")
            {
                // Arguments and Options
                DumpPathOption(),
                VerboseOptions(),
                ReportOptions()
            };
            cmd.Handler = CommandHandler.Create((FileInfo dumpPath, ReportTypes reportTypes, bool verbose) =>
            {
                try
                {
                    IAnalysisEngine analysisEngine;
                    analysisEngine = new MemoryAnalysis(dumpPath.FullName, verbose);

                    analysisEngine.RunAnalysis();
                    var memoryReportRenderer = new MemoryReportRenderer();
                    analysisEngine.GenerateReport(new ConsoleEngine(memoryReportRenderer));
                }
                catch (Exception e) when (e is FileNotFoundException || e is ArchitectureNotMatchException)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ClrDiagnosticsException e)
                {
                    Console.WriteLine($"Failed to create the runtime. Use the --verbose switch to get more information\n {e.Message}");
                }
            }
                    );
            return cmd;
        }

        private static Command CrashDiagCommand()
        {
            var cmd = new Command(
                   name: "crash",
                   description: "Runs a crash analysis on the dumps and shows dotnet exception callstack.")
            {
                // Arguments and Options
                DumpPathOption(),
                VerboseOptions(),
                ReportOptions()
            };
            cmd.Handler = CommandHandler.Create((FileInfo dumpPath, ReportTypes reportTypes, bool verbose) =>
            {
                try
                {
                    IAnalysisEngine analysisEngine;
                    analysisEngine = new CrashAnalysis(dumpPath.FullName, verbose);

                    analysisEngine.RunAnalysis();
                    var reportRenderer = new CrashReportRenderer();
                    analysisEngine.GenerateReport(new ConsoleEngine(reportRenderer));
                }
                catch (Exception e) when (e is FileNotFoundException || e is ArchitectureNotMatchException)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ClrDiagnosticsException e)
                {
                    Console.WriteLine($"Failed to create the runtime. Use the --verbose switch to get more information\n {e.Message}");
                }
            }
                    );
            return cmd;
        }
    }
}
