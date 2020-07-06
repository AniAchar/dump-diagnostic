using ReportEngine.ReportRenderer;
using ReportEngine.ReportTemplates.ConsoleTemplates;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine
{
    public class ConsoleEngine : IReportEngine
    {
        private ConsoleRenderer _consoleRenderer;
        private IConsole _invocationContext;
        private IReportRender _reportRender;
        public ConsoleEngine(IReportRender reportRender)
        {
            _invocationContext = new SystemConsole();
            _consoleRenderer = new ConsoleRenderer(_invocationContext, resetAfterRender: true);
            _reportRender = reportRender;
            _reportRender.Init(_invocationContext, _consoleRenderer);
        }
        public void GenerateReport(BaseReportView viewModel, string templatePath)
        {
            _reportRender.RenderReport(viewModel);

        }

        private void RenderMemoryReport(MemoryReportView viewModel)
        {

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
            console.Append(new ContentView("\n\n"));
            StackLayoutView stackLayoutView2 = new StackLayoutView
            {
                new SplitterView(),
                new ContentView($"Total GC Heap Size: {viewModel.TotalGCMemory}"),
                new SplitterView(),
                new ContentView("\n\n"),
                new TemplateStackView("GC split per logical heap", new HeapBalanceView(viewModel)),
                new TemplateStackView("Memory stats per GC type", new GCHeapBreakupView(viewModel)),
                new TemplateStackView("LOH stats", new LOHView(viewModel)),
                new TemplateStackView("Finalizer Object Stats", new FinalizerView(viewModel)),
                new TemplateStackView("Top 50 Types consumig memory", new HeapStatsView(viewModel)),
                new SplitterView()
            };
            var screen = new ScreenView(_consoleRenderer, _invocationContext) { Child = stackLayoutView2 };
            screen.Render(region);
        }

        private void RenderCrashReport(CrashReportView viewModel)
        {
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
