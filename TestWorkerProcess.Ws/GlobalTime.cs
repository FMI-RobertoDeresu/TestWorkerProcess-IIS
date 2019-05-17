using System.Diagnostics;

namespace TestWorkerProcess.Ws
{
    public class GlobalTime
    {
        public GlobalTime()
        {
            Sw = Stopwatch.StartNew();
        }

        public Stopwatch Sw { get; }
    }
}