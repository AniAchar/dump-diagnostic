using Microsoft.Diagnostics.Runtime;
using ReportEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportEngine.Helpers;
using System.Runtime.CompilerServices;
using System.Resources;

namespace AnalysisEngine.Engines
{
    public class SQLClientAnalysis : BaseAnalysisEngine, IAnalysisEngine
    {
        /// <summary>
        /// This is the structure used by the performance counter. We need to access the Value field here.
        /// </summary>
        private struct CounterEntry
        {
            public int SpinLock;
            public int CounterNameHashCode;
            public int CounterNameOffset;
            public int LifetimeOffset;
            public long Value;
            public int NextCounterOffset;
            public int padding2;
        }

        public SQLClientAnalysis(string dumpPath,
            bool verbose = false) : base(dumpPath, verbose)
        {

        }

        public void GenerateReport(IReportEngine reportEngine)
        {
            throw new NotImplementedException();
        }

        public void RunAnalysis()
        {
            foreach (SQLConnectionInfo connectionDetail in GetConnectionDetails())
            {
                //TODO: Map to reportview as needed.
            }
            foreach (var perfCounter in GetSQLPerformanceCounters())
            {
                //TODO: Map to reportview as needed.
            }

            foreach (var command in GetSqlCommandInfo())
            {
                //TODO: Map to reportview as needed.
            }
        }

        private IEnumerable<SQLConnectionInfo> GetConnectionDetails()
        {
            List<ClrObject> connectionPools = new List<ClrObject>();
            connectionPools.AddRange(FilterObjectByType("System.Data.ProviderBase.DbConnectionPool"));
            connectionPools.AddRange(FilterObjectByType("Microsoft.Data.ProviderBase.DbConnectionPool"));
            if (connectionPools.Count == 0)
            {
                yield break;
            }
            foreach (var conPool in connectionPools)
            {
                yield return GetConnectionDetails(conPool);
            }

        }

        private SQLConnectionInfo GetConnectionDetails(ClrObject connectionPool)
        {
            int maxPoolSize, minPoolSize;
            string connectionString;
            (connectionString, maxPoolSize, minPoolSize) = GetConnectionStringDetails(connectionPool);
            return new SQLConnectionInfo
            {
                OpenConnectionCount = GetActiveConnectionCount(connectionPool),
                ConnectionString = connectionString,
                CurrentConnectionCount = connectionPool.ReadField<int>("_totalObjects"),
                MaxPoolSize = maxPoolSize,
                MinPoolSize = minPoolSize
            };
        }

        private int GetActiveConnectionCount(ClrObject connectionPool)
        {
            var connections = connectionPool.ReadObjectField("_objectList");
            ClrArray dbConnectionInternals = connections.ReadObjectField("_items").AsArray();
            int openConnections = 0;
            for (int i = 0; i < dbConnectionInternals.Length; i++)
            {
                var SqlInternalConnectionTds = dbConnectionInternals.GetObjectValue(i);
                bool isConnectioOpen = false;
                try
                {
                    isConnectioOpen = SqlInternalConnectionTds.ReadField<bool>("_fConnectionOpen");
                }
                catch (Exception e)
                {
                    PrintDebugMessage($"Exception while processing active connections {e.Message}");
                    //the array can have null objects. We will be catching them here.
                }
                if (isConnectioOpen)
                    openConnections++;
            }
            return openConnections;
        }

        private (string connectionString, int maxPoolSize, int minPoolSize) GetConnectionStringDetails(ClrObject connectionPool)
        {
            string connectionString;

            ClrObject poolGroup = connectionPool.ReadObjectField("_connectionPoolGroup");
            ClrObject poolOptions = poolGroup.ReadObjectField("_connectionOptions");
            connectionString = poolOptions.ReadStringField("_usersConnectionString");

            int maxPoolSize = poolOptions.ReadField<int>("_maxPoolSize");

            int minPoolSize = poolOptions.ReadField<int>("_minPoolSize");

            return (connectionString, maxPoolSize, minPoolSize);
        }

        private IEnumerable<SQLPerformanceCounter> GetSQLPerformanceCounters()
        {
            List<ClrObject> perfCounterObj = new List<ClrObject>();
            perfCounterObj.AddRange(FilterObjectByType("System.Data.SqlClient.SqlPerformanceCounters"));
            perfCounterObj.AddRange(FilterObjectByType("Microsoft.Data.SqlClient.SqlPerformanceCounters"));

            foreach (var obj in perfCounterObj)
            {
                //TODO: Add verbose logging support.
                long HardConnectsPerSecond = 0;
                try
                {
                    HardConnectsPerSecond = GetPerfCounterValue(GetPerformanceObject(obj, "HardConnectsPerSecond"));
                }
                catch (ArgumentNullException)
                {
                }

                long NumberOfActiveConnectionPools = 0;
                try
                {
                    NumberOfActiveConnectionPools = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfActiveConnectionPools"));
                }
                catch (ArgumentNullException)
                {
                }

                long HardDisconnectsPerSecond = 0;
                try
                {
                    HardDisconnectsPerSecond = GetPerfCounterValue(GetPerformanceObject(obj, "HardDisconnectsPerSecond"));
                }
                catch (ArgumentNullException)
                {
                }
                long NumberOfActiveConnectionPoolGroups = 0;
                try
                {
                    NumberOfActiveConnectionPoolGroups = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfActiveConnectionPoolGroups"));
                }
                catch (ArgumentNullException)
                {
                }
                long NumberOfActiveConnections = 0;
                try
                {
                    NumberOfActiveConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfActiveConnections"));
                }
                catch (ArgumentNullException)
                {

                }
                long NumberOfFreeConnections = 0;
                try
                {
                    NumberOfFreeConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfFreeConnections"));
                }
                catch (ArgumentNullException)
                {
                    PrintDebugMessage("");
                }
                long NumberOfInactiveConnectionPoolGroups = 0;
                try
                {
                    NumberOfInactiveConnectionPoolGroups = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfInactiveConnectionPoolGroups"));
                }
                catch (ArgumentNullException)
                { }
                long NumberOfInactiveConnectionPools = 0;
                try
                {
                    NumberOfInactiveConnectionPools = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfInactiveConnectionPools"));
                }
                catch (ArgumentNullException) { }
                long NumberOfNonPooledConnections = 0;
                try
                {
                    NumberOfNonPooledConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfNonPooledConnections"));
                }
                catch (ArgumentNullException)
                { }
                long NumberOfPooledConnections = 0;
                try
                {
                    NumberOfPooledConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfPooledConnections"));
                }
                catch (ArgumentNullException) { }
                long NumberOfReclaimedConnections = 0;
                try
                {
                    NumberOfReclaimedConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfReclaimedConnections"));
                }
                catch (ArgumentNullException) { }
                long NumberOfStasisConnections = 0;
                try
                {
                    NumberOfStasisConnections = GetPerfCounterValue(GetPerformanceObject(obj, "NumberOfStasisConnections"));
                }
                catch (ArgumentNullException) { }
                long SoftConnectsPerSecond = 0;
                try
                {
                    SoftConnectsPerSecond = GetPerfCounterValue(GetPerformanceObject(obj, "SoftConnectsPerSecond"));
                }
                catch (ArgumentNullException) { }
                long SoftDisconnectsPerSecond = 0;
                try
                {
                    SoftDisconnectsPerSecond = GetPerfCounterValue(GetPerformanceObject(obj, "SoftDisconnectsPerSecond"));
                }
                catch (ArgumentNullException) { }

                yield return new SQLPerformanceCounter
                {
                    HardConnectsPerSecond = HardConnectsPerSecond,
                    HardDisconnectsPerSecond = HardDisconnectsPerSecond,
                    NumberOfActiveConnectionPoolGroups = NumberOfActiveConnectionPoolGroups,
                    NumberOfActiveConnectionPools = NumberOfActiveConnectionPools,
                    NumberOfActiveConnections = NumberOfActiveConnections,
                    NumberOfFreeConnections = NumberOfFreeConnections,
                    NumberOfInactiveConnectionPoolGroups = NumberOfInactiveConnectionPoolGroups,
                    NumberOfInactiveConnectionPools = NumberOfInactiveConnectionPools,
                    NumberOfNonPooledConnections = NumberOfNonPooledConnections,
                    NumberOfPooledConnections = NumberOfPooledConnections,
                    NumberOfReclaimedConnections = NumberOfReclaimedConnections,
                    NumberOfStasisConnections = NumberOfStasisConnections,
                    SoftConnectsPerSecond = SoftConnectsPerSecond,
                    SoftDisconnectsPerSecond = SoftDisconnectsPerSecond
                };
            }
        }

        private unsafe long GetPerfCounterValue(ClrObject perfCounter)
        {
            if (perfCounter == null)
            {
                return 0;
            }
            ulong entryPoint = perfCounter.ReadField<ulong>("counterEntryPointer");
            int v = sizeof(CounterEntry);
            Span<byte> buffer = new byte[v];
            int read = Target.DataReader.Read(entryPoint, buffer);
            fixed (byte* ptr = buffer)
            {
                CounterEntry counterEntry = *(CounterEntry*)ptr;
                return counterEntry.Value;
            }
        }

        private ClrObject GetPerformanceObject(ClrObject obj, string counterName)
        {
            return obj.ReadObjectField(counterName).Address == 0 || obj.ReadObjectField(counterName).ReadObjectField("_instance").Address == 0
                ? throw new ArgumentNullException()
                : obj.ReadObjectField(counterName).ReadObjectField("_instance").ReadObjectField("sharedCounter");
        }

        private IEnumerable<SqlCommandInfo> GetSqlCommandInfo()
        {
            List<ClrObject> commandObjects = new List<ClrObject>();
            commandObjects.AddRange(FilterObjectByType("System.Data.SqlClient.SqlCommand"));
            commandObjects.AddRange(FilterObjectByType("Microsoft.Data.SqlClient.SqlCommand"));

            if (commandObjects.Count != 0)
                foreach (ClrObject commandObject in commandObjects)
                {
                    string commandString = commandObject.ReadStringField("_commandText");
                    string connectionString = commandObject.ReadObjectField("_activeConnection").ReadStringField("_connectionString");
                    ClrObject paramsObject = commandObject.ReadObjectField("_parameters");
                    int connState = commandObject.ReadObjectField("_activeConnection").ReadObjectField("_innerConnection").ReadField<int>("_state");
                    string connectionState = "Open";
                    if (connState == 0)
                    {
                        connectionState = "Closed";
                    }

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    if (paramsObject.Address != 0)
                    {
                        ClrArray paramsArray = paramsObject.ReadObjectField("_items").ReadObjectField("_items").AsArray();
                        parameters = GetParameters(paramsArray);
                    }

                    yield return parameters.Count == 0
                        ? new SqlCommandInfo { CommandString = commandString, ConnectionString = connectionString, Parameters = null, ConnectionState = connectionState }
                        : new SqlCommandInfo { CommandString = commandString, ConnectionString = commandString, Parameters = parameters, ConnectionState = connectionState };
                }
            else
                yield break;
        }

        private Dictionary<string, string> GetParameters(ClrArray parameters)
        {
            Dictionary<string, string> parameterValues = new Dictionary<string, string>();
            for (int i = 0; i < parameters.Length; i++)
            {
                ClrObject parameter = parameters.GetObjectValue(i);
                if (parameter.Address != 0)
                {
                    string parameterName = parameter.ReadStringField("_parameterName");
                    string parameterValue = "";
                    if (parameter.ReadObjectField("_value").Address != 0)
                    {
                        switch (parameter.ReadObjectField("_value").Type.Name)
                        {
                            case "System.Int64":
                                parameterValue = parameter.ReadField<Int64>("_value").ToString();
                                break;
                            case "System.String":
                                parameterValue = parameter.ReadObjectField("_value").AsString();
                                break;
                            case "System.Int32":
                                parameterValue = parameter.ReadField<Int32>("_value").ToString();
                                break;
                            case "System.DateTime":
                                parameterValue = parameter.ReadField<DateTime>("_value").ToString();
                                break;
                            case "System.Double":
                                parameterValue = parameter.ReadField<Double>("_value").ToString();
                                break;
                            case "System.Boolean":
                                parameterValue = parameter.ReadField<Boolean>("_value").ToString();
                                break;
                            default:
                                parameterValue = "cannot be read";
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(parameterValue) && !string.IsNullOrEmpty(parameterName))
                    {
                        parameterValues.Add(parameterName, parameterValue);
                    }
                }
            }
            return parameterValues;
        }
    }
}
