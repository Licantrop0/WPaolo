using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class SpriteSheetAnimation : Animation
    {
        public SpriteSheetAnimation(int frames, bool looping, Texture2D sourceTexture)
            : base(frames, looping)
        {
            _sourceTexture = sourceTexture;

            _frameWidth = _sourceTexture.Width / frames;
            _frameHeight = _sourceTexture.Height;

            _rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                _rectangles[i] = new Rectangle(i * _frameWidth, 0, _frameWidth, _frameHeight);
            }

            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
        }

        public override Animation Clone()
        {
            return new SpriteSheetAnimation(_frames, _looping, _sourceTexture);
        }
    }
}
