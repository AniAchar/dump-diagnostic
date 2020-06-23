using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using System;

namespace AnalysisEngine
{
    public interface IAnalysisEngine: IDisposable
    {
        void RunAnalysis();

        void GenerateReport(IReportEngine reportEngine);

    }
}
