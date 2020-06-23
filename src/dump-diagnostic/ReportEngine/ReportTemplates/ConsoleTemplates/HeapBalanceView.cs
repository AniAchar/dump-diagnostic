using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    public class HeapBalanceView:TableView<(int heapId, long heapSize)>
    {
        public HeapBalanceView(MemoryReportView reportView)
        {
            Items = reportView.HeapBalance.ToList();
            AddColumn(h => h.heapId, "Heap ID");
            AddColumn(h => h.heapSize, "Heap Size");
        }
    }
}
