using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    class TemplateStackView : StackLayoutView
    {
        public TemplateStackView(string viewTitle, View view)
        {
            Add(new SplitterView());
            Add(new ContentView(viewTitle));
            Add(new SplitterView());
            Add(view);
        }
    }
}
