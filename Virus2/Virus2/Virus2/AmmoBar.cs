using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public class AmmoBar
    {
        Texture2D _texture;
        Color[] _ammobarLookUp = new Color[100];
        Color _ammoBarColor;
        float _scaleX = 1;

        public AmmoBar(Texture2D ammoBarTexture)
        {
            _texture = ammoBarTexture;

            Initialize();
        }

        private void Initialize()
        {
            // initialize color bar
            _ammobarLookUp[0] = new Color(255, 0, 0);     // red
            _ammobarLookUp[32] = new Color(255, 127, 0);  // orange
            _ammobarLookUp[65] = new Color(255, 255, 0);  // yellow
            _ammobarLookUp[99] = new Color(0, 255, 0);    // green

            int i;
            Vector3 m, rgb;
            m = (new Vector3(_ammobarLookUp[32].R, _ammobarLookUp[32].G, _ammobarLookUp[32].B) - new Vector3(_ammobarLookUp[0].R, _ammobarLookUp[0].G, _ammobarLookUp[0].B)) / 32;

            // interpolating from red to orange
            m = (new Vector3(_ammobarLookUp[32].R, _ammobarLookUp[32].G, _ammobarLookUp[32].B) - new Vector3(_ammobarLookUp[0].R, _ammobarLookUp[0].G, _ammobarLookUp[0].B)) / 32;
            for (i = 1; i < 32; i++)
            {
                rgb = new Vector3(_ammobarLookUp[0].R, _ammobarLookUp[0].G, _ammobarLookUp[0].B) + m * (i - 0);
                _ammobarLookUp[i].R = (byte)rgb.X;
                _ammobarLookUp[i].G = (byte)rgb.Y;
                _ammobarLookUp[i].B = (byte)rgb.Z;
            }

            // interpolating from orange to yellow
            m = (new Vector3(_ammobarLookUp[65].R, _ammobarLookUp[65].G, _ammobarLookUp[65].B) - new Vector3(_ammobarLookUp[32].R, _ammobarLookUp[32].G, _ammobarLookUp[32].B)) / 32;
            for (i = 33; i < 65; i++)
            {
                rgb = new Vector3(_ammobarLookUp[32].R, _ammobarLookUp[32].G, _ammobarLookUp[32].B) + m * (i - 32);
                _ammobarLookUp[i].R = (byte)rgb.X;
                _ammobarLookUp[i].G = (byte)rgb.Y;
                _ammobarLookUp[i].B = (byte)rgb.Z;
            }

            // interpolating from yellow to green
            m = (new Vector3(_ammobarLookUp[99].R, _ammobarLookUp[99].G, _ammobarLookUp[99].B) - new Vector3(_ammobarLookUp[65].R, _ammobarLookUp[65].G, _ammobarLookUp[65].B)) / 33;
            for (i = 66; i < 99; i++)
            {
                rgb = new Vector3(_ammobarLookUp[65].R, _ammobarLookUp[65].G, _ammobarLookUp[65].B) + m * (i - 33);
                _ammobarLookUp[i].R = (byte)rgb.X;
                _ammobarLookUp[i].G = (byte)rgb.Y;
                _ammobarLookUp[i].B = (byte)rgb.Z;
            }
        }

        public void Update(int virusAmmo)
        {
            //int lookUpIndex = MathHelper.Clamp(virusAmmo, 0, 99);
            int lookUpIndex = virusAmmo < 100 ? virusAmmo : 99;
            lookUpIndex = lookUpIndex > 0 ? lookUpIndex : 0;

            _ammoBarColor = _ammobarLookUp[lookUpIndex];
            _scaleX = (float)virusAmmo / 100f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                             new Vector2(0, 790),
                             null,
                             new Color(_ammoBarColor.R, _ammoBarColor.G, _ammoBarColor.B),
                             0,
                             Vector2.Zero,
                             new Vector2(_scaleX, 1),
                             SpriteEffects.None, 0);
        }
    }
}
