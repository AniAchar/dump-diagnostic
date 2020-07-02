using AnalysisEngine.Engines;
using ReportEngine;
using System;
using System.IO;
using AnalysisEngine;
using Microsoft.Diagnostics.Runtime;
using System.CommandLine.Invocation;
using System.CommandLine;

namespace DumpDiagnostic
{
    class Program
    {
        static void Main(InvocationContext invocationContext, FileInfo dumpPath, DiagnosisType diagnose, ReportTypes reportType, bool verbose)
        {
            bool initFailed = false;

            IReportEngine reportEngine;
            IAnalysisEngine analysisEngine;

            switch (reportType)
            {
                case ReportTypes.console:
                    {
                        reportEngine = new ConsoleEngine(diagnose.ToString("G"), invocationContext.Console);
                        break;
                    }
                default:
                    reportEngine = null;
                    break;

            }

            switch (diagnose)
            {
                case DiagnosisType.memory:
                    {
                        try
                        {
                            analysisEngine = new MemoryAnalysis(dumpPath.FullName, verbose);
                            
                            RunAnalysis(analysisEngine);
                            GenerateReport(analysisEngine);
                        }
                        catch(Exception e)when(e is FileNotFoundException || e is ArchitectureNotMatchException)
                        {
                            initFailed = true;
                            Console.WriteLine(e.Message);
                        }
                        catch(ClrDiagnosticsException e)
                        {
                            initFailed = true;
                            Console.WriteLine($"Failed to create the runtime. Use the --verbose switch to get more information\n {e.Message}");
                        }
                        break;
                    }
                case DiagnosisType.crash:
                    {
                        try
                        {
                            analysisEngine = new CrashAnalysis(dumpPath.FullName, verbose);

                            RunAnalysis(analysisEngine);
                            GenerateReport(analysisEngine);
                        }
                        catch (Exception e) when (e is FileNotFoundException || e is ArchitectureNotMatchException)
                        {
                            initFailed = true;
                            Console.WriteLine(e.Message);
                        }
                        catch (ClrDiagnosticsException e)
                        {
                            initFailed = true;
                            Console.WriteLine($"Failed to create the runtime. Use the --verbose switch to get more information\n {e.Message}");
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            void RunAnalysis(IAnalysisEngine analysisEngine)
            {
                if(!initFailed)
                {
                    analysisEngine.RunAnalysis();
                }
                else
                {
                    Console.WriteLine("Initialization failed. Cannot run analysis");
                }
            }
            void GenerateReport(IAnalysisEngine analysisEngine)
            {
                if (!initFailed)
                {
                    analysisEngine.GenerateReport(reportEngine);
                }
                else
                {
                    Console.WriteLine("Initialization failed. Cannot run analysis");
                }
            }
            if (!Console.IsOutputRedirected)
            {
                Console.ReadKey();
            }

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

        private static Command CrashDiagCommand() => new Command("crash", description: "Analyze dotnet crash of the dump");

        private static Option VerboseOption => new Option(

            aliases: new[] { "-v", "--verbose" },
            description: "Set to get the verbose logs")
        {
            Argument = new Argument<bool>(name:"verbose")
        };

        private static Option DumpPathOption => new Option(
            aliases: new[] { "-d", "--dump-path" },
            description: "Absolute path to the dump file")
        {
            Argument = new Argument<FileInfo>(name: "dump-path")
        };

        private static Option ReportTypeOption => new Option(
            aliases: new[] { "--report-type" },
            description: "Absolute path to the dump file")
        {
            Argument = new Argument<ReportTypes>(name: "report-type")
        };


    }
}
