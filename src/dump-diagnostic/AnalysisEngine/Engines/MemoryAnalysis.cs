using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using ReportEngine.ReportView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisEngine.Engines
{
    public sealed class MemoryAnalysis : BaseAnalysisEngine, IAnalysisEngine
    {
        private string DumpPath { get; set; }

        public MemoryReportView viewModel;

        /// <summary>
        /// Constructs a usable object that can be used to analyze and render the report.
        /// </summary>
        /// <param name="dumpPath">Ful path to the dump file</param>
        /// <param name="reportEngine">The rendering engine that we will be using to create the reports</param>
        /// <param name="verbose">This is for debugging the app.</param>
        public MemoryAnalysis(string dumpPath, bool verbose = false):
            base(verbose)
        {
            this.DumpPath = dumpPath;
            try
            {
                CreateDataTarget(dumpPath);
                MatchArchitecture();
            }
            catch(Exception e) when(e is ArchitectureNotMatchException || e is FileNotFoundException)
            {
                throw e;
            }
            
            try
            {
                CreateRuntime();
            }
            catch(ClrDiagnosticsException e)
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
            viewModel = new MemoryReportView {
                GCMode = GetGCMode(),
                ClrVersion = GetClrVersion(),
                DotnetFlavor = GetDotnetFlavor(),
                FinilazerStats = GetFinalizableObjectsStats(),
                HeapBalance = GetHeapBalance(),
                HeapStats = GetHeapStats(),
                GCHeapBrekup = GetGenerationMemoryBreakUp(),
                LOH = GetLOHStats(),
                MainModuleName = GetMainModule(),
                TopGCRoots = GetTopGCRoot(),
                TotalGCMemory = GetGCHeapSize()
            };
        }

        /// <summary>
        /// Gets the GC size on each of the logical heaps that are present.
        /// </summary>
        /// <returns>List of tuples with the first value of the tuple being heap id and the second being the heap size.</returns>
        private IEnumerable<(int heapId, long heapSize)> GetHeapBalance()
        {
            foreach (var item in from seg in Runtime.Heap.Segments
                                 group seg by seg.LogicalHeap into g
                                 orderby g.Key
                                 select new
                                 {
                                     Heap = g.Key,
                                     Size = g.Sum(p => (uint)p.Length)
                                 })
            {
                yield return (item.Heap, item.Size);
            }
        }

        /// <summary>
        /// Gets the memory usage of each of the generations.
        /// </summary>
        /// <returns>List of tuple with GCType, Committed Memory and Reserved memory</returns>
        private IEnumerable<(string GCtype, MemoryRange commitedMemory, MemoryRange reservedMemory)> GetGenerationMemoryBreakUp()
        {
            List<Tuple<String, MemoryRange, MemoryRange>> breakup = new List<Tuple<String, MemoryRange, MemoryRange>>();

            foreach (var segment in Runtime.Heap.Segments)
            {
                string GCType;
                if (segment.IsEphemeralSegment)
                    GCType = "Ephemeral";
                else if (segment.IsLargeObjectSegment)
                    GCType = "Large";
                else
                    GCType = "Gen2";

                yield return (GCType, segment.CommittedMemory, segment.ReservedMemory);
            }
        }

        /// <summary>
        /// Gets the LOH size and the object summary in the LOH
        /// </summary>
        /// <param name="minTotalSize">The min size that we will be filtering on.</param>
        /// <returns>List with ClrType's Name, Count of Object of ClrType and total size of that type</returns>
        private IEnumerable<(string type, int count, long size)> GetLOHStats()
        {
            ClrSegment lohSegment = Runtime.Heap.Segments.Where(s => s.IsLargeObjectSegment).First();
            IEnumerable<ClrObject> objects = lohSegment.EnumerateObjects();
            var stats = GetStats(objects);
            return stats.Select(s => (type: s.type.Name, count: s.objects.Count(), s.size));
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<(string type, int count, long size)> GetFinalizableObjectsStats()
        {
            var stats = GetStats(Runtime.Heap.EnumerateFinalizableObjects());
            return stats.Select(s => (type: s.type.Name, count: s.objects.Count(), s.size));
        }


        /// <summary>
        /// 
        /// </summary>

        private IEnumerable<(string type, int count, long size)> GetHeapStats()
        {
            var stats = GetStats(Runtime.Heap.EnumerateObjects());
            return stats.Select(s => (type: s.type.Name, count: s.objects.Count(), s.size));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topn"></param>
        /// <returns></returns>
        private IEnumerable<(string typeName, string moduleName, IEnumerable<(string objectAddress, string GCRoot)> gcRoots)> GetTopGCRoot(int topn = 3)
        {
            var stats = GetStats(Runtime.Heap.EnumerateObjects());
            var topnTypes = stats.Where(s => s.type.Name.ToLower() != "free").Take(topn);
            foreach (var (type, objects, _) in topnTypes)
            {
                yield return (type.Name, type.Module.Name, GetGCRoot(objects));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="topn"></param>
        /// <returns></returns>
        private IEnumerable<(string objectAddress, string GCRoot)> GetGCRoot
            ( IEnumerable<ClrObject> objects, int topn = 3)
        {
            GCRoot gCRoot = new GCRoot(Runtime.Heap);
            var topObjs = objects.Take(topn);
            return topObjs.Select(o => (objectAddress: o.Address.ToString(), 
                                        GCRoot: gCRoot.EnumerateGCRoots(o, false, Environment.ProcessorCount).First().ToString()));
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private IEnumerable<(ClrType type, IEnumerable<ClrObject> objects, long size)> GetStats( IEnumerable<ClrObject> obj)
        {
            return obj
                .GroupBy(o => o.Type, o => o)
                .Select(o => (type: o.Key, objects: (IEnumerable<ClrObject>)o, totalSize: o.Sum(s => (long)s.Size)))
                .OrderByDescending(t => t.totalSize);
        }

        private long GetGCHeapSize()
        {
            return Runtime.Heap.Segments.Sum(s => (long)s.Length);
        }
    }
}
