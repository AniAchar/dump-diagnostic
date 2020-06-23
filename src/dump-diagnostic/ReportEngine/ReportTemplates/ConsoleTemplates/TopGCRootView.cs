using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class TopGCRootView:GridView
    {
        public TopGCRootView(MemoryReportView reportView)
        {
            SetColumns(ColumnDefinition.Star(1), ColumnDefinition.Star(1), ColumnDefinition.Star(1));
            SetRows(RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1));
            SetChild(new ContentView("CLR Type"), 0, 0);
            SetChild(new ContentView("CLR Type's Module"), 1, 0);
            SetChild(new ContentView("GC Roots of 3 objects"), 2, 0);
            int i = 1;
            foreach(var (typeName, moduleName, gcRoots) in reportView.TopGCRoots)
            {
                SetChild(new ContentView(typeName), 0, i);
                SetChild(new ContentView(moduleName), 1, i);
                foreach(var r in gcRoots)
                {
                    SetChild(new ContentView(r.GCRoot), 2, i);
                }
            }
        }
    }
}
