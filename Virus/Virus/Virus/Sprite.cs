using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    public class SpriteEvent
    {
        public SpriteEventCode Code { get; set;}
        public Object[] Params { get; set; }

        public SpriteEvent(SpriteEventCode code)
        {
            Code = code;
        }

        public SpriteEvent(SpriteEventCode code, Object[] p)
        {
            Code = code;
            Params = p;
        }
    }

    public abstract class Sprite
    {
        // graphics
        protected Dictionary<string, Animation> _animations;
        protected string _currentAnimation;
        protected Queue<SpriteEvent> _spriteEventQueque = new Queue<SpriteEvent>();
        protected float _elapsedTime;
        protected SpriteEvent _actSpriteEvent;

        protected PhysicalKinematicPoint _physicalPoint; 

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _currentAnimation = _animations.Keys.First();
        }

        public Vector2 Position
        { get { return _physicalPoint.Position; } set { _physicalPoint.Position = value; _animations[_currentAnimation].Position = value; } }

        public Vector2 Speed
        { get { return _physicalPoint.Speed; } set { _physicalPoint.Speed = value; } }

        public void Move(float dt)
        {
            _physicalPoint.Move(dt);
            _animations[_currentAnimation].Position = _physicalPoint.Position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animations[_currentAnimation].Draw(spriteBatch);
        }

        public void AddSpriteEvent(SpriteEvent evnt)
        {
            _spriteEventQueque.Enqueue(evnt);
        }

        public virtual void Update(GameTime gameTime)
        {
            _elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _actSpriteEvent = _spriteEventQueque.Count > 0 ? _spriteEventQueque.Dequeue() : null;
        }
    }
}