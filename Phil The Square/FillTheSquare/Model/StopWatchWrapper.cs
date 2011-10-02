using System;
using System.Diagnostics;

namespace FillTheSquare.Model
{
    class StopwatchWrapper
    {
        TimeSpan _offsetTimeSpan;
        Stopwatch _stopwatch;

        public StopwatchWrapper(TimeSpan offsetElapsedTimeSpan)
        {
            _offsetTimeSpan = offsetElapsedTimeSpan;
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _offsetTimeSpan = TimeSpan.Zero;
            _stopwatch.Reset();
        }

        public TimeSpan Elapsed
        {
            get
            {
                if (_offsetTimeSpan == null)
                    return _stopwatch.Elapsed;
                
                return _stopwatch.Elapsed + _offsetTimeSpan;
            }

            set { _offsetTimeSpan = value; }
        }

    }
}
