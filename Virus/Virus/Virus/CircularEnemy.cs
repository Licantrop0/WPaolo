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
        rotatingScaling,    // temp debug!
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
            _touchable = true;

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
            return (Touchable() && Vector2.Distance(_physicalPoint.Position, fingerPosition) < _touchRadius );
        }

        public bool Touchable()
        {
            return _touchable;
        }

        public override void Update(GameTime gameTime)
        {
            // it sets _elapsedTime and _actSpriteEvent
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
                        if ((int)Position.X % 2 == 0)   // random, just to try...
                        {
                            _state = CircularEnemyState.fading;
                            _touchable = false;
                            Speed = Vector2.Zero;
                            _animations["main"].FadeSpeed = 1.0f;
                            _utilityTimer = 0;
                        }
                        else
                        {
                            _state = CircularEnemyState.rotatingScaling;
                            _touchable = false;
                            Speed = Vector2.Zero;
                            _animations["main"].ScalingSpeed = -1f;
                            _animations["main"].RotationSpeed = 4 * (float)Math.PI;
                            _utilityTimer = 0;
                        }
                    }
                    
                    break;

                case CircularEnemyState.fading:

                    _animations["main"].Fade(_elapsedTime);
                    _utilityTimer += _elapsedTime;
                    if (_utilityTimer > 1)
                    {
                        _state = CircularEnemyState.died;
                    }

                    break;

                case CircularEnemyState.rotatingScaling:

                    _animations["main"].ChangeDimension(_elapsedTime);
                    _animations["main"].Rotate(_elapsedTime);
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
