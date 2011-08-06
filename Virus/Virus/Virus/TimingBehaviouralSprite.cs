using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class TimingBehaviouralSprite : Sprite
    {
        // main timer
        float _timer = 0;
        bool _timerRunning = false;
        float _endingTime;

        // behaviours
        List<Behaviour> _behaviours = new List<Behaviour>();

        // specific behaviours
        public bool Freezed { get; set; }
        public bool Blinking { get; set; }

        public TimingBehaviouralSprite(Dictionary<string, Animation> animations)
            :base(animations)
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

        protected bool Exceeded()
        {
            return _timer >= _endingTime;
        }

        public override void Update(GameTime gameTime)
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

        private void StartBehaviour(Behaviour behaviour)
        {
            _behaviours.Remove(_behaviours.Where(b => b.GetCode() == behaviour.GetCode()).First());

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
        private void BehaviourBlinkingInitialize(BlinkingBehaviour b)
        {
            Blinking = true;
            b.BlinkingTimer = 0;
        }

        private void BehaviourBlinkingRun(BlinkingBehaviour b)
        {
            b.BlinkingTimer += _elapsedTime;

            if (b.BlinkingTimer > b.BlinkingPeriod)
            {
                b.BlinkingTimer -= b.BlinkingPeriod;
                Tint = Tint == b.RollbackTint ? b.BlinkingTint : b.RollbackTint;
            }
        }

        private void BehaviourBlinkingExpire(BlinkingBehaviour b)
        {
            Blinking = false;
            Tint = b.RollbackTint;
        }

        protected void StartBlinking(float timeToExpire, float blinkingFrequency, Color blinkingTint)
        {
            BlinkingBehaviour blink =
                new BlinkingBehaviour(timeToExpire, BehaviourBlinkingInitialize, BehaviourBlinkingRun, BehaviourBlinkingExpire, blinkingFrequency, blinkingTint, Tint);
        }

        // freezing
        /*protected void BehaviourBlinkingInitialize(BlinkingBehaviour b)
        {
            Blinking = true;
            b.BlinkingTimer = 0;
        }

        protected void BehaviourBlinkingRun(BlinkingBehaviour b)
        {
            b.BlinkingTimer += _elapsedTime;

            if (b.BlinkingTimer > b.BlinkingPeriod)
            {
                b.BlinkingTimer -= b.BlinkingPeriod;
                Tint = Tint == b.RollbackTint ? b.BlinkingTint : b.RollbackTint;
            }
        }

        protected void BehaviourBLinkingExpire(BlinkingBehaviour b)
        {
            Blinking = false;
            Tint = b.RollbackTint;
        }*/
    }
 
    public delegate void ContainerBehaviourDelegate(Behaviour behaviour);

    public enum BehaviourCode
    {
        blinking,
        freezed
    }

    public abstract class Behaviour
    {
        public abstract BehaviourCode GetCode();

        float _timer = 0;
        float _timeToExpire;
        ContainerBehaviourDelegate _containerDelegateInitialize;
        ContainerBehaviourDelegate _containerDelegateRun;
        ContainerBehaviourDelegate _containerDelegateExpire;

        public Behaviour(float timeToExpire, ContainerBehaviourDelegate cbi, ContainerBehaviourDelegate cbr, ContainerBehaviourDelegate cbe)
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
    }

    public class BlinkingBehaviour : Behaviour
    {
        float _blinkingPeriod;
        Color _blinkingTint;
        Color _rollbackTint;

        public float BlinkingPeriod { get { return _blinkingPeriod; } }
        public Color BlinkingTint { get { return _blinkingTint; } }
        public Color RollbackTint { get { return _rollbackTint; } }
        public float BlinkingTimer { get; set; }

        public BlinkingBehaviour(float timeToExpire, ContainerBehaviourDelegate cbi, ContainerBehaviourDelegate cbr, ContainerBehaviourDelegate cbe,
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
}
