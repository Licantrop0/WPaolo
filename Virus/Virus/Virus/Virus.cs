using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Virus
{
    class Virus : Sprite
    {
        public Virus(Dictionary<string, Animation> animations)
            :base(animations)
        {
            _physicalPoint = new PhysicalPoint();
            _currentAnimation = "main";
            _animations["main"].FramePerSecond = 4f;
            Position = new Vector2(240, 400);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _animations["main"].Animate(_elapsedTime);
        }
    }
}
