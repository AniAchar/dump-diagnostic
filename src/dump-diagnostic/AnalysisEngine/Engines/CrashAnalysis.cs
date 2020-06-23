using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnalysisEngine.Engines
{
    public sealed class CrashAnalysis : BaseAnalysisEngine, IAnalysisEngine
    {
        private string dumpPath { get; set; }
        private IReportEngine reportEngine { get; set; }

        public CrashAnalysis(string dumpPath, IReportEngine reportEngine, bool verbose = false) :
            base(verbose)
        {
            this.dumpPath = dumpPath;
            this.reportEngine = reportEngine;
            try
            {
                CreateDataTarget(dumpPath);
                MatchArchitecture();
            }
            catch (Exception e) when (e is ArchitectureNotMatchException || e is FileNotFoundException)
            {
                throw e;
            }

            try
            {
                CreateRuntime();
            }
            catch (ClrDiagnosticsException e)
            {
                throw e;
            }

        }
        public void GenerateReport(IReportEngine reportEngine)
        {
            throw new NotImplementedException();
        }

        public void RunAnalysis()
        {
            throw new NotImplementedException();
        }

        
    }
}
