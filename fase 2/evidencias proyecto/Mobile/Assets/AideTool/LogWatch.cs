using System;
using System.Diagnostics;

namespace AideTool
{
    internal sealed class LogWatch
    {
        private Stopwatch m_watch;
        internal string WatchId { get; private set; }
        internal string WatchOperationName { get; private set; }

        internal LogWatch(string operationName)
        {
            WatchId = Guid.NewGuid().ToString();
            m_watch = new Stopwatch();
            WatchOperationName = operationName;

        }

        internal string Start(bool log = true)
        {
            m_watch.Start();
            if(log)
                Aide.Log($"Stopwatch started for operation {WatchOperationName}");
            return WatchId;
        }

        internal void Stop(float div = 1f)
        {
            if(!m_watch.IsRunning)
            {
                Aide.Log($"Stopwatch {WatchId} is not running");
                return;
            }

            m_watch.Stop();
            float elapsedTime = (float)m_watch.ElapsedMilliseconds / div;
            string units = div switch
            {
                1f => "miliseconds",
                1000f => "seconds",
                _ => "time units"
            };
            Aide.Log($"Operation {WatchOperationName} took {elapsedTime} {units} to finish");

            Dispose();
        }

        internal void Dispose()
        {
            WatchId = null;
            m_watch = null;
            WatchOperationName = null;
        }
    }
}
