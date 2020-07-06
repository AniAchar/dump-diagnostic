using ReportEngine.ReportTemplates.ConsoleTemplates;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Text;

namespace ReportEngine.ReportRenderer
{
    public class CrashReportRenderer : IReportRender
    {
        private ConsoleRenderer _consoleRenderer;
        private IConsole _invocationContext;

        public void Init(IConsole invocationContext, ConsoleRenderer consoleRenderer)
        {
            _invocationContext = invocationContext;
            _consoleRenderer = consoleRenderer;
        }

        public void RenderReport(BaseReportView baseViewModel)
        {
            CrashReportView viewModel = (CrashReportView)baseViewModel;
            var region = new Region(0,
                                    0,
                                    1080,
                                    10800,
                                    true);



            var console = _invocationContext;

            if (console is ITerminal terminal)
            {
                terminal.Clear();
            }

            console.Append(new ContentView("Dump details"));
            console.Append(new SplitterView());
            console.Append(new DumpDetailsView(viewModel));
            StackLayoutView stackLayoutView2 = new StackLayoutView
            {
                new TemplateStackView("Thread details", new ThreadsDetailView(viewModel)),
                new TemplateStackView("Threads with exceptions", new StackFramesStackView(viewModel)),
                new SplitterView()
            };
            var screen = new ScreenView(_consoleRenderer, _invocationContext) { Child = stackLayoutView2 };
            screen.Render(region);
        }
    }
}
