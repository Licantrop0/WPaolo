using System;
using Microsoft.Devices.Sensors;
using System.Threading;

namespace WPCommon
{
    public class ShakeDetector
    {
        private const double MinMagnitude = 1.2;
        private const double MinMagnitudeSquared = MinMagnitude * MinMagnitude;

        private Accelerometer _accelerometer;
        private object SyncRoot = new object();
        private int _minimumShakes;

        private ShakeRecord[] _shakeRecordList;
        private int _shakeRecordIndex;

        private TimeSpan _minimumShakeTime = TimeSpan.FromMilliseconds(500);
        public TimeSpan MinimumShakeTime
        {
            get { return _minimumShakeTime; }
            set { _minimumShakeTime = value; }
        }

        public event EventHandler<EventArgs> ShakeDetected;
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

        private struct ShakeRecord
        {
            public Direction ShakeDirection;
            public DateTimeOffset EventTime;
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
                    _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_CurrentValueChanged);
                    //.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(_accelerometer_ReadingChanged);
                    _accelerometer.Start();
                }
            }
        }

        void _accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            //Does the currenet acceleration vector meet the minimum magnitude that we care about?
            var acc = e.SensorReading.Acceleration;
            if ((acc.X * acc.X + acc.Y * acc.Y) > MinMagnitudeSquared)
            {
                //In the following direction will contain the direction
                //in which the device was accelerating in degrees. 
                double degrees = 180.0 * Math.Atan2(acc.Y, acc.X) / Math.PI;
                Direction direction = DegreesToDirection(degrees);

                //If the shake detected is in the same direction as the last one then ignore it
                if ((direction & _shakeRecordList[_shakeRecordIndex].ShakeDirection) != Direction.None)
                    return;

                _shakeRecordIndex = (_shakeRecordIndex + 1) % _minimumShakes;
                _shakeRecordList[_shakeRecordIndex] = new ShakeRecord()
                {
                    EventTime = e.SensorReading.Timestamp,
                    ShakeDirection = direction
                };

                CheckForShakes();
            }

        }

        public void Stop()
        {
            lock (SyncRoot)
            {
                if (_accelerometer != null)
                {
                    _accelerometer.Stop();
                    _accelerometer.CurrentValueChanged -= _accelerometer_CurrentValueChanged;
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

        void CheckForShakes()
        {
            int startIndex = _shakeRecordIndex - 1;
            if (startIndex < 0) startIndex = _minimumShakes - 1;

            if (_shakeRecordList[_shakeRecordIndex].EventTime -
                _shakeRecordList[startIndex].EventTime <= MinimumShakeTime)
            {
                OnShakeDetected();
            }
        }

    }

    //public class ShakeDetector2 : IDisposable
    //{
    //    private const double ShakeThreshold = 0.7;
    //    private readonly Accelerometer _sensor = new Accelerometer();
    //    private AccelerometerReadingEventArgs _lastReading;
    //    private int _shakeCount;
    //    private bool _shaking;

    //    public SensorState State { get { return _sensor.State; } }

    //    public event EventHandler<EventArgs> ShakeDetected = null;
    //    protected void OnShakeDetected()
    //    {
    //        if (ShakeDetected != null)
    //        {
    //            ShakeDetected(this, new EventArgs());
    //        }
    //    }

    //    public ShakeDetector2()
    //    {
    //        var sensor = new Accelerometer();
    //        if (sensor.State == SensorState.NotSupported)
    //            throw new NotSupportedException("Accelerometer not supported on this device");
    //        _sensor = sensor;
    //    }


    //    public void Dispose()
    //    {
    //        if (_sensor != null)
    //            _sensor.Dispose();
    //    }


    //    public void Start()
    //    {
    //        if (_sensor != null)
    //            _sensor.Start();
    //    }

    //    public void Stop()
    //    {
    //        if (_sensor != null)
    //            _sensor.Stop();
    //    }

    //    private void ReadingChanged(object sender, AccelerometerReadingEventArgs e)
    //    {
    //        if (_sensor.State == SensorState.Ready)
    //        {
    //            try
    //            {
    //                if (_lastReading != null)
    //                {
    //                    if (!_shaking && CheckForShake(_lastReading, e, ShakeThreshold) && _shakeCount >= 1)
    //                    {
    //                        //We are shaking
    //                        _shaking = true;
    //                        _shakeCount = 0;
    //                        OnShakeDetected();
    //                    }
    //                    else if (CheckForShake(_lastReading, e, ShakeThreshold))
    //                    {
    //                        _shakeCount++;
    //                    }
    //                    else if (!CheckForShake(_lastReading, e, 0.2))
    //                    {
    //                        _shakeCount = 0;
    //                        _shaking = false;
    //                    }
    //                }
    //                _lastReading = e;
    //            }
    //            catch { /* ignore errors */ }
    //        }
    //    }


    //    private static bool CheckForShake(AccelerometerReadingEventArgs last, AccelerometerReadingEventArgs current,
    //                                        double threshold)
    //    {
    //        double deltaX = Math.Abs((last.X - current.X));
    //        double deltaY = Math.Abs((last.Y - current.Y));
    //        double deltaZ = Math.Abs((last.Z - current.Z));

    //        return (deltaX > threshold && deltaY > threshold) ||
    //                (deltaX > threshold && deltaZ > threshold) ||
    //                (deltaY > threshold && deltaZ > threshold);
    //    }
    //}

    //public class MagnitudeDetector
    //{
    //    private Accelerometer _accelerometer;
    //    public event EventHandler<EventArgs> ShakeDetected = null;
    //    protected void OnShakeDetected()
    //    {
    //        if (ShakeDetected != null)
    //        {
    //            ShakeDetected(this, EventArgs.Empty);
    //        }
    //    }

    //    public MagnitudeDetector()
    //    {
    //        if (!Accelerometer.IsSupported)
    //            return;

    //        _accelerometer = new Accelerometer();
    //        //_accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);

    //        Start();
    //    }

    //    void _accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
    //    {
    //        var acc = e.SensorReading.Acceleration;

    //        double f = Math.Sqrt(acc.X * acc.X + acc.Y * acc.Y + acc.Z * acc.Z);
    //        if (f > 1.5)
    //        {
    //            OnShakeDetected();
    //            _accelerometer.Stop();
    //        }
    //    }

    //    public void Start()
    //    {
    //        _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_CurrentValueChanged);
    //        _accelerometer.Start();
    //    }

    //}
}
