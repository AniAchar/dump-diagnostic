using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine
{
    public interface IReportEngine
    {
        void GenerateReport(BaseReportView viewModel, string templatePath);
    }
}
