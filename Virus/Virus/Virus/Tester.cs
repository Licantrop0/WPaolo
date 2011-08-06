using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Virus
{
    public class Tester : CircularSprite
    {
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.WriteLine(gameTime.TotalGameTime.TotalSeconds.ToString());
            Debug.WriteLine(Tint.ToString());

            Animate();

            if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.fingerHit)
            {
                Debug.WriteLine("Tint when hit: " + Tint.ToString());
                StartBlinking(10, 20, Color.Transparent);
            }

            if (_actSpriteEvent != null && _actSpriteEvent.Code == SpriteEventCode.bombHit)
            {
                Freeze(5);
            }
        }

        public Tester(Dictionary<string, Animation> animations, float r1, float r2)
            :base(animations, r1, r2)
        {
            FramePerSecond = 5;
        }

        protected override void InitializePhysics()
        {
            _physicalPoint = new PhysicalKinematicPoint();
        }
    }
}
