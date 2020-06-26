using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AnalysisEngine
{
    public class BaseAnalysisEngine : IDisposable
    {
        private bool disposedValue;
        private readonly bool verbose;
        protected DataTarget Target { get; private set; }

        protected ClrRuntime Runtime { get; private set; }
        protected ClrInfo Version { get; private set; }

        protected bool CheckDumpx64()
        {
            bool isx64 = Target.DataReader.PointerSize == 8;
            string arch = isx64 ? "x64" : "x86";
            PrintDebugMessage($"The dump is a {arch} dump");
            return isx64;
        }

        protected void CreateDataTarget(string dumpPath)
        {
            PrintDebugMessage($"Creating target for {dumpPath}");
            if (!CheckDumpExists(dumpPath))
            {
                PrintDebugMessage("The dump couldn't be found or the process/user doesn't have access to the location");
                throw new FileNotFoundException($"The dump file {dumpPath} couldn't be found or the user doesn't have access to the file");
            }
            Target = DataTarget.LoadDump(dumpPath);
        }
        public BaseAnalysisEngine(bool verbose)
        {
            this.verbose = verbose;
        }
        protected void CreateRuntime()
        {
            //We will only be using the first CLRVerion we find. SxS is not something that we will be supporting right now.
            Version = Target.ClrVersions[0];
            DacInfo dac = Version.DacInfo;
            if (dac == null)
            {
                PrintDebugMessage("Cannot obtain the dac from the dump. Consider getting the DAC from the machine where the dumps were collected.");
                throw new ClrDiagnosticsException("Failed to get DAC.");
            }

            Runtime = Version.CreateRuntime();
            if (Runtime == null)
            {
                PrintDebugMessage("Creation of the runtime failed.");
                //TODO: Add a method to try an get the DAC from the user. This will need a new entry point. Likely override the constructor.
                throw new ClrDiagnosticsException("Failed to create CLR runtime.");
            }
        }

        protected bool MatchArchitecture()
        {
            if (Environment.Is64BitProcess != CheckDumpx64())
            {
                PrintDebugMessage(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", CheckDumpx64() ? "64 bit" : "32 bit"));
                throw new ArchitectureNotMatchException(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", CheckDumpx64() ? "64 bit" : "32 bit"));
            }
            return true;
        }

        protected bool CheckDumpExists(string dumpPath)
        {
            PrintDebugMessage("The dump couldn't be found or the process/user doesn't have access to the location");
            return File.Exists(dumpPath);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Target.Dispose();
                    Runtime.Dispose();
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseAnalysisEngine()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This will print additional messages when verbose is turned on.
        /// </summary>
        /// <param name="message">The message to be printed.</param>
        protected void PrintDebugMessage(string message)
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        protected string GetMainModule()
        {
            //var mainModule = Runtime.AppDomains.SelectMany(ad => ad.Modules).Single(m => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            //   ? m.Name.EndsWith(".exe") : File.Exists(Path.ChangeExtension(m.Name, null)));
            ClrModule mainModule = null;


            if (Runtime.ClrInfo.Flavor.ToString("G").ToLower() == "desktop")
            {
                foreach (var module in Runtime.AppDomains.SelectMany(ad => ad.Modules))
                {
                    if (module.Name != null)
                    {
                        if (module.Name.EndsWith(".exe"))
                        {
                            mainModule = module;
                        }
                    }
                }
            }
            else
            {
                var t = GetMainThread();
                var mainFunction = t.EnumerateStackTrace().Where(f => f.Method?.Name == "Main").FirstOrDefault();
                mainModule = mainFunction.Method.Type.Module;
            }


                
            return mainModule.Name;
            }

            protected ClrThread GetMainThread()
            {
                return Runtime.Threads.Single(t => !t.IsBackground);
            }

            /// <summary>
            /// Get the if the dump is using server GC or Workstation GC
            /// </summary>
            /// <returns></returns>
            protected string GetGCMode()
            {
                return Runtime.Heap.IsServer ? "Server" : "Workstation";
            }

            /// <summary>
            /// Gets if the dump is dotnet core of dotnet framework
            /// </summary>
            /// <returns>string either "Desktop" for framework or "Core" for dotnet core</returns>
            protected string GetDotnetFlavor()
            {
                if (Runtime.ClrInfo.Flavor.ToString("G") == "Desktop")
                {
                    return "Framework";
                }
                return Runtime.ClrInfo.Flavor.ToString("G");
            }

            protected string GetClrVersion()
            {
                return Runtime.ClrInfo.Version.ToString();
            }
        }

    }
