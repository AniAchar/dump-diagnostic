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
            CheckArguments(dumpPath, diagnose, reportType);
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
            if(dumpPath== null || diagnose == null || reportType == null)
            {
                Console.WriteLine("Missing argument(s). Please use dump-diagnostic -h to look at all the required arguments");
                Environment.Exit(-1);
            }
        }


    }
}
