using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    public class DumpDetailsView : GridView
    {
        public DumpDetailsView(BaseReportView reportView)
        {
            SetColumns(ColumnDefinition.Star(0.3), ColumnDefinition.Star(1));
            SetRows(RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1), RowDefinition.Star(1));
            SetChild(new ContentView("Main Module Name"), 0, 0);
            SetChild(new ContentView($"{reportView.MainModuleName}"), 1, 0);
            SetChild(new ContentView("Clr Version"), 0, 1);
            SetChild(new ContentView($"{reportView.ClrVersion}"), 1, 1);
            SetChild(new ContentView("Dotnet type"), 0, 2);
            SetChild(new ContentView($"{reportView.DotnetFlavor}"), 1, 2);
            SetChild(new ContentView("GC Mode"), 0, 3);
            SetChild(new ContentView($"{reportView.GCMode}"), 1, 3);
        }

    }
}
