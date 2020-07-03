using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    public class SplitterView: ContentView
    {
        public SplitterView()
        {

            Span = new ContentSpan("\n\n==========================================\n\n");
        }
    }
}
