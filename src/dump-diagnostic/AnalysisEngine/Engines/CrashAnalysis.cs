using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using ReportEngine.Helpers;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

namespace AnalysisEngine.Engines
{
    public sealed class CrashAnalysis : BaseAnalysisEngine, IAnalysisEngine
    {
        private string DumpPath { get; set; }

        CrashReportView viewModel;

        public CrashAnalysis(string dumpPath, bool verbose = false) :
            base(verbose)
        {
            this.DumpPath = dumpPath;
            try
            {
                CreateDataTarget(dumpPath);
                MatchArchitecture();
            }
            catch (Exception e) when (e is ArchitectureNotMatchException || e is FileNotFoundException)
            {
                throw e;
            }

            try
            {
                CreateRuntime();
            }
            catch (ClrDiagnosticsException e)
            {
                throw e;
            }

        }
        public void GenerateReport(IReportEngine reportEngine)
        {
            reportEngine.GenerateReport(viewModel, null);
        }

        public void RunAnalysis()
        {
            viewModel = new CrashReportView
            {
                ThreadDetails = GetThreadDetails(),

            };
        }

        private IEnumerable<AnalysisThread> GetThreadDetails()
        {
            foreach (var thread in Runtime.Threads)
            {
                var at = new AnalysisThread(thread)
                {
                    Address = thread.Address,
                    LockCount = thread.LockCount,
                    ManagedThreadId = thread.ManagedThreadId,
                    OSThreadId = thread.OSThreadId,
                    StackBase = thread.StackBase,
                    StackLimit = thread.StackBase
                };
                at.SetApartment();
                at.SetException();
                at.SetFinalizer();
                at.SetGCMode();
                yield return at;
            }
        }

        private string GetExceptionCallStack(ClrException exception, bool innerException = false)
        {
            StringBuilder @string = new StringBuilder();
            string innerEx = "";
            if(innerException)
            {
                @string.Append("Inner exception:\n");
            }
            @string.Append("Exception details:\n");
            @string.Append($"{exception.Type.Name}\n");
            @string.Append("Exception Message:\n");
            @string.Append($"{exception.Message}\n\n");
            @string.Append("Exception stack");
            @string.Append(GetExceptionStackFrames(exception.StackTrace));
            if (exception.Inner != null)
            {
                innerEx = GetExceptionCallStack(exception.Inner, true);
            }
            @string.Append(innerEx);
            return @string.ToString();
        }

        private IEnumerable<(int ManagedThreadId, uint OSThreadId, string stackFrames, string exceptionDetails)> GetThreadsWithException()
        {
            var threadsWithException = Runtime.Threads.Where(t => t.CurrentException != null);

            foreach(var t in threadsWithException)
            {
                yield return (t.ManagedThreadId, t.OSThreadId, GetStackFrames(t.EnumerateStackTrace()), GetExceptionCallStack(t.CurrentException));
            }
        }

        private string GetExceptionStackFrames(ImmutableArray<ClrStackFrame>  stackTrace)
        {
            StringBuilder @string = new StringBuilder();
            foreach (var frame in stackTrace)
            {
                if (frame.Kind.ToString("G") == "Internal")
                {
                    @string.Append("----Internal---\n");
                }
                else
                {
                    @string.Append($"{frame.Method}\n");
                }
            }

            return @string.ToString();
        }
        private string GetStackFrames(IEnumerable<ClrStackFrame> stackTrace)
        {
            StringBuilder @string = new StringBuilder();
            foreach (var frame in stackTrace)
            {
                if (frame.Kind.ToString("G") == "Internal")
                {
                    @string.Append("----Internal---\n");
                }
                else
                {
                    @string.Append($"{frame.Method}\n");
                }
            }

            return @string.ToString();
        }

    }
}
