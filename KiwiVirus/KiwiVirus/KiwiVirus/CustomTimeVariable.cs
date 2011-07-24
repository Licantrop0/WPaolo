using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Kiwi
{
    public class CustomTimeVariable
    {
        Vector2[] _points;
        float _currentTime;
        float _currentDerivative;
        float _currentValue;
        int _currentInterval;
        int _totalPoints;

        public CustomTimeVariable(Vector2[] points)
        {
            _points = points;
            _currentTime = 0;
            _currentInterval = 1;
            _currentValue = _points[_currentInterval - 1].Y;
            _currentDerivative = (_points[_currentInterval].Y - _points[_currentInterval - 1].Y) / (_points[_currentInterval].X - _points[_currentInterval - 1].X);
            _totalPoints = points.Length;
        }

        public float NextVariableValue(float dt)
        {
            _currentTime += dt;

            if (_currentInterval >= _totalPoints)
                return 0;

            if (_currentTime < _points[_currentInterval].X)
            {
                _currentValue += _currentDerivative * dt;
            }
            else
            {
                _currentInterval++;
                if (_currentInterval < _totalPoints)
                {
                    _currentDerivative = (_points[_currentInterval].Y - _points[_currentInterval - 1].Y) / (_points[_currentInterval].X - _points[_currentInterval - 1].X);
                    _currentValue = _points[_currentInterval - 1].Y + _currentDerivative * (_currentTime - _points[_currentInterval - 1].X);
                }
                else
                {
                    _currentDerivative = _currentValue = 0;
                }
            }

            return _currentValue;
        }

        public float GetCurrentValue()
        {
            return _currentValue;
        }
    }
}
