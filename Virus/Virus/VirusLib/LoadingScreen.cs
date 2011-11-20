using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VirusLib
{
    class LoadingScreen
    {
        SpritePrototypeContainer _loader;
        ContentManager _content;
        SpriteBatch _sb;
        Texture2D barTex;
        Texture2D backroundTex;

        public LoadingScreen(ContentManager content, SpriteBatch sb)
        {
            _content = content;

            _loader = new SpritePrototypeContainer(content, "AnimationConfig\\Enemies.xml");
            _sb = sb;

            // This texture will be drawn behind the loading bar, centred on screen.
            // The rest of the screen will be filled with the top left pixel color.
            backroundTex = content.Load<Texture2D>("Loading");

            // 1-pixel white texture for solid bars
            barTex = content.Load<Texture2D>("pixel");
        }

        public void Load()
        {
            for (int i = 0; i < _loader.TotalItems; i++)
            {
                _loader.LoadCurrentElement(_content);
                Draw(i / _loader.TotalItems);
            }
        }

        //public Loader Update()
        //{
        //    // enumerator.MoveNext() will load one item and return false when all done.
        //    bool incomplete = enumerator.MoveNext();
        //    return incomplete ? null : loader;
        //}

        private void Draw(float pct)
        {
            Rectangle screenRect = new Rectangle(0, 0, _sb.GraphicsDevice.Viewport.Width, _sb.GraphicsDevice.Viewport.Height);
            Vector2 backroundTexPos = new Vector2(screenRect.Center.X - backroundTex.Width / 2, screenRect.Center.Y - backroundTex.Height / 2);

            // Size of of the loading bar and position relative to top left of bitmap.
            Rectangle loadingBarPos = new Rectangle();
            loadingBarPos.Width = 400; // pixels
            loadingBarPos.Height = 20;
            loadingBarPos.X = backroundTex.Width / 2 - loadingBarPos.Width / 2; // centered horizontally
            loadingBarPos.Y = 440;
            loadingBarPos.Offset((int)backroundTexPos.X, (int)backroundTexPos.Y);

            // Loading screen colors.
            Color screenBackgroundColor = topLeftPixelColor(backroundTex);
            Color barColor = Color.Lime;
            Color barBackgroundColor = Color.Blue;
            int barBackgroundExpand = 2; // Width of loading bar border in pixels

            _sb.GraphicsDevice.Clear(screenBackgroundColor);
            _sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            _sb.Draw(backroundTex, backroundTexPos, Color.White);

            Rectangle barBackground = loadingBarPos;
            barBackground.Inflate(barBackgroundExpand, barBackgroundExpand);
            _sb.Draw(barTex, barBackground, barBackgroundColor);

            Rectangle bar = loadingBarPos;
            bar.Width = (int)(loadingBarPos.Width * pct);

            _sb.Draw(barTex, bar, barColor);
            _sb.End();
        }

        Color topLeftPixelColor(Texture2D tex)
        {
            Color[] colors = { Color.Magenta };
            tex.GetData<Color>(0, new Rectangle(0, 0, 1, 1), colors, 0, 1);
            return colors[0];
        }
    }
}
