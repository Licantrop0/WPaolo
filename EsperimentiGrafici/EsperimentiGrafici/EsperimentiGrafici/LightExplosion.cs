using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EsperimentiGrafici
{
    public class LightExplosion
    {
        const int W = 480;
        const int H = 800;

        // setup data
        public float _innerWidth;
        public float _outerWidth;

        // derived data
        float _gradient;  // = 1.0 / ((_outerWidth - _innerWidth) / 2);

        // texture
        public Texture2D _whitePixel;
        Color[] _colorData = new Color[W * H];

        // state
        float _distance;

        public LightExplosion(Texture2D whitePixel, float outerWidth, float innerWidth)
        {
            _whitePixel = whitePixel;
            _outerWidth = outerWidth;
            _innerWidth = innerWidth;

            _gradient = 0.75f / ((_outerWidth - _innerWidth) / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float t1 = _distance;
            float t2 = t1 + ((_outerWidth - _innerWidth) / 2);
            float t3 = t2 + _innerWidth;
            float t4 = t3 + ((_outerWidth - _innerWidth) / 2);
            float luminance = 0;

            for (int y = 0; y < H / 2; y ++)
            {
                for (int x = 0; x < W / 2; x ++)
                {
                    float r = (float)Math.Sqrt(x * x + y * y);
                    luminance = 0;

                    if (r >= t1 && r < t2)
                        luminance = (r - t1) * _gradient;
                    else if (r >= t2 && r < t3)
                        luminance = 0.75f;
                    else if (r >= t3 && r < t4)
                        luminance = 1 - (r - t3) * _gradient;

                    if (luminance > 0)
                    {
                        spriteBatch.Draw(_whitePixel, new Vector2(x + W / 2, -y + H / 2), new Color(luminance, luminance, luminance, 0));
                        spriteBatch.Draw(_whitePixel, new Vector2(-x + W / 2, -y + H / 2), new Color(luminance, luminance, luminance, 0));
                        spriteBatch.Draw(_whitePixel, new Vector2(x + W / 2, y + H / 2), new Color(luminance, luminance, luminance, 0));
                        spriteBatch.Draw(_whitePixel, new Vector2(-x + W / 2, y + H / 2), new Color(luminance, luminance, luminance, 0));
                    }
                        
                }
            }
        }

        public void Update(float distance)
        {
            _distance = distance;
        }
    }
}
