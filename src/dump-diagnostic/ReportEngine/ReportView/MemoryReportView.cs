using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Diagnostics.Runtime;

namespace ReportEngine.ReportView
{
    public class MemoryReportView : BaseReportView
    {
        public long TotalGCMemory { get; set; }
        public IEnumerable<(int heapId, long heapSize)> HeapBalance { get; set; }
        public IEnumerable<(string GCtype, MemoryRange commitedMemory, MemoryRange reservedMemory)> GCHeapBrekup { get; set; }
        public IEnumerable<(string type, int count, long size)> LOH { get; set; }
        public IEnumerable<(string type, int count, long size)> FinilazerStats { get; set; }
        public IEnumerable<(string type, int count, long size)> HeapStats { get; set; }
        public IEnumerable<(string typeName, string moduleName, IEnumerable<(string objectAddress, string GCRoot)> gcRoots)> TopGCRoots 
                { get; set; }


    }
}
