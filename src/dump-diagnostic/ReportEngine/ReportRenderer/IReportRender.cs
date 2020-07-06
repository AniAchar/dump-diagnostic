using ReportEngine.ReportView;
using System.CommandLine;
using System.CommandLine.Rendering;

namespace ReportEngine.ReportRenderer
{
    public interface IReportRender
    {
        void RenderReport(BaseReportView viewModel);
        void Init(IConsole invocationContext, ConsoleRenderer consoleRenderer);
    }
}
