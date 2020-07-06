using ReportEngine.ReportTemplates.ConsoleTemplates;
using ReportEngine.ReportView;
using System.CommandLine;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

namespace ReportEngine.ReportRenderer
{
    public class MemoryReportRenderer : IReportRender
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
            MemoryReportView viewModel = (MemoryReportView)baseViewModel;
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
    }
}
