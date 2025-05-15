using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LocalAniHubFront.Helpers
{
    public class SmoothValueController
    {
        private double _current;
        private double _target;
        private readonly double _lerpFactor;
        private readonly DispatcherTimer _timer;
        private bool _isAtTarget = true;

        public event Action<double> ValueChanged;
        public event Action ReachedTarget;

        public SmoothValueController(double initialValue, double lerpFactor = 0.2, int fps = 60)
        {
            _current = _target = initialValue;
            _lerpFactor = lerpFactor;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000.0 / fps)
            };
            _timer.Tick += (s, e) => Update();
            _timer.Start();
        }

        public double Value => _current;
        public double Target => _target;

        public void SetTarget(double newTarget)
        {
            _target = newTarget;
        }

        private void Update()
        {
            if (Math.Abs(_target - _current) < 0.0001)
            {
                if (!_isAtTarget)
                {
                    _isAtTarget = true;
                    _current = _target;
                    ValueChanged?.Invoke(_current);
                    ReachedTarget?.Invoke();
                }
                return;
            }

            _isAtTarget = false;
            _current += (_target - _current) * _lerpFactor;
            ValueChanged?.Invoke(_current);
        }

        public void Stop() => _timer.Stop();
        public void Start() => _timer.Start();
    }

}
