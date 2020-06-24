using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportEngine.Helpers
{
    public class AnalysisThread
    {
        public ulong Address { get; set; }
        public uint OSThreadId { get; set; }
        public int ManagedThreadId { get; set; }
        public ulong StackBase { get; set; }
        public ulong StackLimit { get; set; }
        public uint LockCount { get; set; }
        public string CurrentException { get; private set; }
        public string Apartment { get; private set; }
        public string Finalizer { get; private set; }
        public string GCMode { get; private set; }

        private readonly ClrThread _thread;

        public AnalysisThread(ClrThread t)
        {
            _thread = t;
        }

        public void SetApartment()
        {
            if (_thread.IsSTA)
            {
                Apartment = "STA";
            }
            else if (_thread.IsMTA)
            {
                Apartment = "MTA";
            }
            else
            {
                Apartment = "UNK";
            }
        }

        public void SetException()
        {
            ClrException currentEx = _thread.CurrentException;
            if (currentEx is ClrException ex)
                CurrentException = $"Exception: {ex.Type.Name}, HResult= {ex.HResult}";
        }

        public void SetFinalizer()
        {
            if (_thread.IsFinalizer)
                Finalizer = "Finalizer thread";
        }

        public void SetGCMode()
        {
            GCMode = _thread.GCMode.ToString("G");
        }

    }
}
