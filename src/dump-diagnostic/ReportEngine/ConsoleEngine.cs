using ReportEngine.ReportTemplates.ConsoleTemplates;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Text;

namespace ReportEngine
{
    public class ConsoleEngine : IReportEngine
    {
        private readonly string _reportType;
        private ConsoleRenderer _consoleRenderer;
        private InvocationContext _invocationContext;
        public ConsoleEngine(string reportType, InvocationContext invocationContext)
        {
            _reportType = reportType;
            _invocationContext = invocationContext;
            _consoleRenderer = new ConsoleRenderer(invocationContext.Console, mode: invocationContext.BindingContext.OutputMode(),
        resetAfterRender: true);
        }
        public void GenerateReport(BaseReportView viewModel, string templatePath)
        {
            if(_reportType.ToLower() == "memory")
            {
                RenderMemoryReport((MemoryReportView)viewModel);
            }
            if(_reportType.ToLower() == "crash")
            {
                RenderCrashReport((CrashReportView)viewModel);
            }

        }

        private void RenderMemoryReport(MemoryReportView viewModel)
        {

            var region = new Region(0,
                                    0,
                                    1080,
                                    10800,
                                    true);

            

            var console = _invocationContext.Console;

            if (console is ITerminal terminal)
            {
                terminal.Clear();
            }

            console.Append(new ContentView("Dump details"));
            console.Append(new SplitterView());
            console.Append(new DumpDetailsView(viewModel));
            StackLayoutView stackLayoutView2 = new StackLayoutView
            {
                new SplitterView(),
                new ContentView($"Total GC Heap Size: {viewModel.TotalGCMemory}"),
                new SplitterView(),
                new TemplateStackView("GC split per logical heap", new HeapBalanceView(viewModel)),
                new TemplateStackView("Memory stats per GC type", new GCHeapBreakupView(viewModel)),
                new TemplateStackView("LOH stats", new LOHView(viewModel)),
                new TemplateStackView("Finalizer Object Stats", new FinalizerView(viewModel)),
                new TemplateStackView("Top 50 Types consumig memory", new HeapStatsView(viewModel)),
                new SplitterView()
            };
            var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = stackLayoutView2 };
            screen.Render(region);
        }

        private void RenderCrashReport(CrashReportView viewModel)
        {
            var region = new Region(0,
                                    0,
                                    1080,
                                    10800,
                                    true);



            var console = _invocationContext.Console;

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
            var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = stackLayoutView2 };
            screen.Render(region);
        }

    }
}
