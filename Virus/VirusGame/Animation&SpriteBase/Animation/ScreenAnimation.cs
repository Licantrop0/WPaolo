using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class ScreenAnimation : Animation
    {
        #region private members

        // texture arrays from which to chose the source texture
        private Texture2D[] _textureArray;

        // parameter
        private bool _isPortrait;

        #endregion

        #region constructors

        public ScreenAnimation(int frames, bool looping, bool isPortrait, Texture2D[] textureArray)
            : base(frames, looping)
        {
            Initialize(frames, isPortrait, textureArray);

            _origin = new Vector2(_frameWidth / 2, _frameHeight / 2);
        }

        public ScreenAnimation(int frames, bool looping, bool isPortrait, Texture2D[] textureArray, Vector2 origin)
            : base(frames, looping)
        {
            Initialize(frames, isPortrait, textureArray);

            _origin = origin;
        }

        private void Initialize(int frames, bool isPortrait, Texture2D[] textureArray)
        {
            _textureArray = textureArray;
            _sourceTexture = _textureArray[0];

            _isPortrait = isPortrait;

            if (_isPortrait)
            {
                _frameWidth = 480;
                _frameHeight = 800;
            }
            else
            {
                _frameWidth = 800;
                _frameHeight = 480;
            }

            int horizontalPeriod = _isPortrait ? 4 : 2;
            int verticalParts = _isPortrait ? 4 : 4;

            _rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
            {
                int j = i % 8;

                _rectangles[i] = new Rectangle(
                    (j % horizontalPeriod) * _frameWidth,
                    (j / verticalParts) * _frameHeight,
                    _frameWidth, _frameHeight);
            }
        }

        #endregion

        #region ovverriden methods

        protected override void SetSourceTexture()
        {
            _sourceTexture = _textureArray[_frameIndex / 8];
        }

        public override void Reset()
        {
            base.Reset();

            _sourceTexture = _textureArray[0];
        }

        #endregion 
    }
}
