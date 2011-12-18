using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Virus
{
    public class TimingBehaviouralBody : Body
    {
        // main timer
        float _timer = 0;
        bool _timerRunning = false;
        float _endingTime;

        // behaviours
        List<BehaviourExecutor> _behaviours = new List<BehaviourExecutor>();

        // specific behaviours
        public bool Freezed { get; set; }
        public bool Blinking { get; set; }

        public TimingBehaviouralBody(DynamicSystem dynamicSystem, Sprite sprite, Shape shape)
            :base(dynamicSystem, sprite, shape)
        {

        }

        public void ResetTimer()
        {
            _timer = 0;
            _timerRunning = false;
        }

        protected void StartTimer(float endingTime)
        {
            _timerRunning = true;
            _endingTime = endingTime;
        }

        protected void StopTimer()
        {
            _timerRunning = false;
        }

        protected void ResetAndStartTimer(float endingTime)
        {
            _timer = 0;
            _timerRunning = true;
            _endingTime = endingTime;
        }

        protected bool Exceeded()
        {
            return _timer >= _endingTime;
        }

        protected void Recycle()
        {
            _timer -= _endingTime;
        }

        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            // handle main timer
            if (_timerRunning)
            {
                _timer += _elapsedTime;
            }

            // handle behaviours
            HandleBehaviours(_elapsedTime);
        }

        private void FinalizeBehaviour(BehaviourCode code)
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (_behaviours[i].GetCode() == code)
                {
                    _behaviours[i].End();
                    _behaviours.RemoveAt(i);
                    i--;
                }
            }
        }

        private void StartBehaviour(BehaviourExecutor behaviour)
        {
            behaviour.Initialize();
            _behaviours.Add(behaviour);
        }

        private void HandleBehaviours(float elapsedTime)
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (!_behaviours[i].Run(elapsedTime))
                {
                    _behaviours.RemoveAt(i);
                    i--;
                }
            }
        }

        // specific behaviours
        // blinking
        private void BehaviourBlinkingInitialize(BehaviourExecutor b)
        {
            BlinikingBehaviourExecutor blink = (BlinikingBehaviourExecutor)b;

            Blinking = true;
            blink.BlinkingTimer = 0;
        }

        private void BehaviourBlinkingRun(BehaviourExecutor b)
        {
            BlinikingBehaviourExecutor blink = (BlinikingBehaviourExecutor)b;

            blink.BlinkingTimer += _elapsedTime;

            if (blink.BlinkingTimer > blink.BlinkingPeriod)
            {
                blink.BlinkingTimer -= blink.BlinkingPeriod;
                Sprite.Tint = Sprite.Tint == blink.RollbackTint ? blink.BlinkingTint : blink.RollbackTint;
            }
        }

        private void BehaviourBlinkingExpire(BehaviourExecutor b)
        {
            BlinikingBehaviourExecutor blink = (BlinikingBehaviourExecutor)b;

            Blinking = false;
            Sprite.Tint = blink.RollbackTint;
        }

        protected void StartBlinking(float timeToExpire, float blinkingFrequency, Color blinkingTint)
        {
            FinalizeBehaviour(BehaviourCode.blinking);

            BehaviourExecutor blink =
                new BlinikingBehaviourExecutor(timeToExpire, BehaviourBlinkingInitialize, BehaviourBlinkingRun, BehaviourBlinkingExpire, blinkingFrequency, blinkingTint, Sprite.Tint);

            StartBehaviour(blink);
        }

        protected void EndBlinking()
        {
            FinalizeBehaviour(BehaviourCode.blinking);
        }

        // freezing
        protected void BehaviourFreezedInitialize(BehaviourExecutor b)
        {
            FreezedBehaviourExecutor freeze = (FreezedBehaviourExecutor) b;

            Freezed = true;
            Speed = Vector2.Zero;
            AngularSpeed = 0;
            Sprite.FramePerSecond = 0;
            _timerRunning = false;
        }

        protected void BehaviourFreezedRun(BehaviourExecutor b)
        {
        }

        protected void BehaviourFreezedExpire(BehaviourExecutor b)
        {
            FreezedBehaviourExecutor freeze = (FreezedBehaviourExecutor) b;

            Freezed = false;
            Speed = freeze.RollbackSpeed;
            AngularSpeed = freeze.RollbackRotationSpeed;
            Sprite.FramePerSecond = freeze.RollbackFPS;
            _timerRunning = true;
        }

        protected void StartFreeze(float timeToExpire)
        {
            FinalizeBehaviour(BehaviourCode.freezed);

            BehaviourExecutor freeze =
                new FreezedBehaviourExecutor(timeToExpire, BehaviourFreezedInitialize, BehaviourFreezedRun, BehaviourFreezedExpire, Speed, Sprite.FramePerSecond, AngularSpeed);

            StartBehaviour(freeze);
        }

        protected void EndFreeze()
        {
            FinalizeBehaviour(BehaviourCode.freezed);
        }
    }
 
    public delegate void ContainerBehaviourDelegate(BehaviourExecutor behaviour);
    //public Action<BehaviourExecutor>;

    public enum BehaviourCode
    {
        blinking,
        freezed
    }


    public abstract class BehaviourExecutor
    {
        public abstract BehaviourCode GetCode();

        float _timer = 0;
        float _timeToExpire;
        ContainerBehaviourDelegate _containerDelegateInitialize;
        ContainerBehaviourDelegate _containerDelegateRun;
        ContainerBehaviourDelegate _containerDelegateExpire;

        public BehaviourExecutor(float timeToExpire, ContainerBehaviourDelegate cbi, ContainerBehaviourDelegate cbr, ContainerBehaviourDelegate cbe)
        {
            _timeToExpire = timeToExpire;
            _containerDelegateInitialize = cbi;
            _containerDelegateRun = cbr;
            _containerDelegateExpire = cbe;
        }

        public void Initialize()
        {
            _containerDelegateInitialize(this);
        }

        public bool Run(float elapsedTime)
        {
            _timer += elapsedTime;

            _containerDelegateRun(this);

            if (_timer >= _timeToExpire)
            {
                _containerDelegateExpire(this);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void End()
        {
            _containerDelegateExpire(this);
        }
    }

    public class BlinikingBehaviourExecutor : BehaviourExecutor
    {
        float _blinkingPeriod;
        Color _blinkingTint;
        Color _rollbackTint;

        public float BlinkingPeriod { get { return _blinkingPeriod; } }
        public Color BlinkingTint { get { return _blinkingTint; } }
        public Color RollbackTint { get { return _rollbackTint; } }
        public float BlinkingTimer { get; set; }

        public BlinikingBehaviourExecutor(float timeToExpire, ContainerBehaviourDelegate cbi, ContainerBehaviourDelegate cbr, ContainerBehaviourDelegate cbe,
            float blinkingFrequency, Color blinkingTint, Color rollbackTint)
            :base(timeToExpire, cbi, cbr, cbe)
        {
            _blinkingPeriod = 1 / blinkingFrequency;
            _blinkingTint = blinkingTint;
            _rollbackTint = rollbackTint;
        }

        public override BehaviourCode GetCode()
        {
            return BehaviourCode.blinking;
        }
    }

    public class FreezedBehaviourExecutor : BehaviourExecutor
    {
        Vector2 _rollbackSpeed;
        float _rollbackFPS;
        float _rollbackRoationSpeed;

        public Vector2 RollbackSpeed { get { return _rollbackSpeed; } }
        public float RollbackFPS { get { return _rollbackFPS; } }
        public float RollbackRotationSpeed { get { return _rollbackRoationSpeed;} }

        public FreezedBehaviourExecutor(float timeToExpire, ContainerBehaviourDelegate cbi, ContainerBehaviourDelegate cbr, ContainerBehaviourDelegate cbe,
            Vector2 rollbackSpeed, float rollbackFPS, float rollbackRotationSpeed)
            : base(timeToExpire, cbi, cbr, cbe)
        {
            _rollbackSpeed = rollbackSpeed;
            _rollbackFPS = rollbackFPS;
            _rollbackRoationSpeed = rollbackRotationSpeed;
        }

        public override BehaviourCode GetCode()
        {
            return BehaviourCode.freezed;
        }
    }
}
