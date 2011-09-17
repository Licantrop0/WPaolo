using System;
using System.Threading;
using Microsoft.Devices.Sensors;

namespace WPCommon
{
    public class ShakeDetector
    {
        private Accelerometer _accelerometer;
        object SyncRoot = new object();
        private int _minimumShakes;
        ShakeRecord[] _shakeRecordList;
        private int _shakeRecordIndex;
        private const double MinMagnitude = 1.2;
        private const double MinMagnitudeSquared = MinMagnitude * MinMagnitude;
        private readonly TimeSpan MinimumShakeTime = TimeSpan.FromMilliseconds(500);
        private TimeSpan Delay = TimeSpan.FromSeconds(1);

        public event EventHandler<EventArgs> ShakeDetected = null;
        protected void OnShakeDetected()
        {
            if (ShakeDetected != null)
            {
                ShakeDetected(this, EventArgs.Empty);
            }
        }

        [Flags]
        public enum Direction
        {
            None = 0,
            North = 1,
            South = 2,
            West = 4,
            East = 8,
            NorthWest = North | West,
            SouthWest = South | West,
            SouthEast = South | East,
            NorthEast = North | East
        }

        public struct ShakeRecord
        {
            public Direction ShakeDirection;
            public DateTime EventTime;
        }

        public ShakeDetector(int minShakes = 2)
        {
            _minimumShakes = minShakes;
            _shakeRecordList = new ShakeRecord[minShakes];
        }

        public void Start()
        {
            lock (SyncRoot)
            {
                if (_accelerometer == null)
                {
                    _accelerometer = new Accelerometer();
                    _accelerometer.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(_accelerometer_ReadingChanged);
                    _accelerometer.Start();
                }
            }
        }

        public void Stop()
        {
            lock (SyncRoot)
            {
                if (_accelerometer != null)
                {
                    _accelerometer.Stop();
                    _accelerometer.ReadingChanged -= _accelerometer_ReadingChanged;
                    _accelerometer = null;
                }
            }
        }

        Direction DegreesToDirection(double direction)
        {
            if ((direction >= 337.5) || (direction <= 22.5))
                return Direction.North;
            if ((direction <= 67.5))
                return Direction.NorthEast;
            if (direction <= 112.5)
                return Direction.East;
            if (direction <= 157.5)
                return Direction.SouthEast;
            if (direction <= 202.5)
                return Direction.South;
            if (direction <= 247.5)
                return Direction.SouthWest;
            if (direction <= 292.5)
                return Direction.West;
            return Direction.NorthWest;
        }

        void _accelerometer_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            //Does the currenet acceleration vector meet the minimum magnitude that we care about?
            if ((e.X * e.X + e.Y * e.Y) > MinMagnitudeSquared)
            {
                //In the following direction will contain the direction
                //in which the device was accelerating in degrees. 
                double degrees = 180.0 * Math.Atan2(e.Y, e.X) / Math.PI;
                Direction direction = DegreesToDirection(degrees);

                //If the shake detected is in the same direction as the last one then ignore it
                if ((direction & _shakeRecordList[_shakeRecordIndex].ShakeDirection) != Direction.None)
                    return;

                _shakeRecordIndex = (_shakeRecordIndex + 1) % _minimumShakes;
                _shakeRecordList[_shakeRecordIndex] = new ShakeRecord()
                {
                    EventTime = DateTime.Now,
                    ShakeDirection = direction
                };

                CheckForShakes();
            }
        }

        void CheckForShakes()
        {
            int startIndex = (_shakeRecordIndex - 1);
            if (startIndex < 0) startIndex = _minimumShakes - 1;
            int endIndex = _shakeRecordIndex;

            if ((_shakeRecordList[endIndex].EventTime.Subtract(_shakeRecordList[startIndex].EventTime)) <= MinimumShakeTime)
            {
                OnShakeDetected();
                Thread.Sleep(Delay);
            }
        }

    }

    public class ShakeDetector2 : IDisposable
    {
        private const double ShakeThreshold = 0.7;
        private readonly Accelerometer _sensor = new Accelerometer();
        private AccelerometerReadingEventArgs _lastReading;
        private int _shakeCount;
        private bool _shaking;

        public SensorState State { get { return _sensor.State; } }

        public event EventHandler<EventArgs> ShakeDetected = null;
        protected void OnShakeDetected()
        {
            if (ShakeDetected != null)
            {
                ShakeDetected(this, new EventArgs());
            }
        }

        public ShakeDetector2()
        {
            var sensor = new Accelerometer();
            if (sensor.State == SensorState.NotSupported)
                throw new NotSupportedException("Accelerometer not supported on this device");
            _sensor = sensor;
        }


        public void Dispose()
        {
            if (_sensor != null)
                _sensor.Dispose();
        }


        public void Start()
        {
            if (_sensor != null)
                _sensor.Start();
        }

        public void Stop()
        {
            if (_sensor != null)
                _sensor.Stop();
        }

        private void ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            if (_sensor.State == SensorState.Ready)
            {
                try
                {
                    if (_lastReading != null)
                    {
                        if (!_shaking && CheckForShake(_lastReading, e, ShakeThreshold) && _shakeCount >= 1)
                        {
                            //We are shaking
                            _shaking = true;
                            _shakeCount = 0;
                            OnShakeDetected();
                        }
                        else if (CheckForShake(_lastReading, e, ShakeThreshold))
                        {
                            _shakeCount++;
                        }
                        else if (!CheckForShake(_lastReading, e, 0.2))
                        {
                            _shakeCount = 0;
                            _shaking = false;
                        }
                    }
                    _lastReading = e;
                }
                catch { /* ignore errors */ }
            }
        }


        private static bool CheckForShake(AccelerometerReadingEventArgs last, AccelerometerReadingEventArgs current,
                                            double threshold)
        {
            double deltaX = Math.Abs((last.X - current.X));
            double deltaY = Math.Abs((last.Y - current.Y));
            double deltaZ = Math.Abs((last.Z - current.Z));

            return (deltaX > threshold && deltaY > threshold) ||
                    (deltaX > threshold && deltaZ > threshold) ||
                    (deltaY > threshold && deltaZ > threshold);
        }
    }
}
