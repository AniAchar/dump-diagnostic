using AnalysisEngine.Engines;
using ReportEngine;
using System;
using System.IO;
using AnalysisEngine;
using Microsoft.Diagnostics.Runtime;
using System.CommandLine.Invocation;

namespace DumpDiagnostic
{
    class Program
    {
        static void Main(InvocationContext invocationContext, FileInfo dumpPath, string diagnose, string reportType, bool verbose)
        {
            bool initFailed = false;

            IReportEngine reportEngine;
            IAnalysisEngine analysisEngine;

            switch (reportType.ToLower().Trim())
            {
                case "html":
                    {
                        reportEngine = new HtmlEngine();
                        break;
                    }
                case "console":
                    {
                        reportEngine = new ConsoleEngine(diagnose.ToLower().Trim(), invocationContext);
                        break;
                    }
                default:
                    reportEngine = null;
                    Usage();
                    break;

            }

            switch (diagnose.ToLower().Trim())
            {
                case "memory":
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
                case "crash":
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

        private static void Usage()
        {
            Console.WriteLine("To use the dump-diagnostic tool, you must pass the dump file path using the --dump-path switch and the dianostics that needs to be done using the --diagnose and the report type using the --report-type.\n --diagnose can be either `memory` or `crash`\n--report-type can be either `html` or `console`.");
        }
    }
}
