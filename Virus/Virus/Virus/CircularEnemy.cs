using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum CircularEnemyState
	{
	    moving,
        fading,
        stopped,
        died
	}

    public enum CircularEnemySpriteEvent
    {
        fingerHit,
        virusHit
    }

    public class CircularEnemy : Sprite, ITouchable
    {
        protected bool _touchable;
        protected float _radius;        // va incapsulato in una classe per gestire le collisioni
        protected float _touchRadius;
        protected CircularEnemyState _state;
        private float _utilityTimer;

        public CircularEnemy(Dictionary<string, Animation> animations, float radius, float touchRadius)
            :base(animations)
        {
            _radius = radius;
            _touchRadius = touchRadius;

            _physicalPoint = new PhysicalPoint();
            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 6;
            _state = CircularEnemyState.moving;
        }

        public CircularEnemyState State
        { get { return _state; } }

        public float Radius
        { get { return _radius; } set { _radius = value; } }

        // ITouchable implementation
        public bool Touched(Vector2 fingerPosition)
        {
            return (Vector2.Distance(_physicalPoint.Position, fingerPosition) < _touchRadius );
        }

        public bool Touchable()
        {
            return _touchable;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_state)
            {
                case CircularEnemyState.moving:

                    if (_actSpriteEvent == -1)
                    {
                        Move(_elapsedTime);
                        _animations["main"].Animate(_elapsedTime);
                    }
                    else if (_actSpriteEvent == (int)CircularEnemySpriteEvent.fingerHit)
                    {
                        _state = CircularEnemyState.fading;
                        Speed = Vector2.Zero;
                        _animations["main"].FadeSpeed = 1f;
                        _utilityTimer = 0;
                    }
                    
                    break;

                case CircularEnemyState.fading:

                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = CircularEnemyState.died;
                    }

                    break;
                case CircularEnemyState.stopped:
                    break;
                case CircularEnemyState.died:
                    break;
                default:
                    break;
            }


        }
    }
}
