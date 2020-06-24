using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class LOHView:TableView<(string type, int count, long size)>
    {
        public LOHView(MemoryReportView reportView)
        {
            Items = reportView.LOH.Take(50).ToList();
            AddColumn(h => h.type, "Type");
            AddColumn(h => h.count, "Count");
            AddColumn(h => h.size, "Size");
        }
    }
}
