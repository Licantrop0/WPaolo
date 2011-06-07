using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Virus
{
    public enum BackgroundEffects
    {
        none,
        trembling
    }

    public class MovingBackground
    {
        Texture2D[] _backgroundTextureArray;

        // general state data
        float _cursor;       
        float _speed;
        BackgroundEffects _effects;
        int _frameHeight;

        // trembling data
        float _trembleCursor1;
        float _trembleCursor2;
        float _tremblePhase;
        float _trembleFrequency;
        float _trembleAmplitude;
        float _trembleTime;
        bool _separationDistorion;

        CustomTimeVariable _trembleAmplitudeCurve;

        // utility timer
        float _utilityTimer;

        public float Speed { get {return _speed;} set {_speed = value;} }

        public MovingBackground(Texture2D[] textureArray, float offset)
        {
            _backgroundTextureArray = textureArray;;
            _speed = 0;
            _effects = BackgroundEffects.none;

            _frameHeight = _backgroundTextureArray[0].Height;
            _cursor = _frameHeight * _backgroundTextureArray.Length - 800 - offset;     // offset is an offset used to have some "room" for effect like trembling and such...
        }

        public void Tremble(float duration, CustomTimeVariable amplitude, float frequency, float phase, bool separationDistortion)
        {
            _trembleTime = duration;
            _trembleAmplitudeCurve = amplitude;
            _trembleAmplitude = _trembleAmplitudeCurve.GetCurrentValue();
            _trembleFrequency = frequency;
            _tremblePhase = phase;
            _separationDistorion = separationDistortion;
            _utilityTimer = 0;
            _effects = BackgroundEffects.trembling;
        }

        private void DrawWindow(SpriteBatch spriteBatch, float position, float alphaBlending)
        {
            if (position < 0)
                position = 0;

            int cursor = (int)Math.Round(position);

            // determino la texture a cui appartiene il cursore
            int frameIndexTop = (int)(position / _frameHeight);

            // determino la texture a cui appartiene il punto Bottom della finestra
            int frameIndexBot = (int)((position + 800) / _frameHeight);

            int top = cursor % _frameHeight;

            if (top == 0)   // flickering avoid!
            {
                frameIndexTop = frameIndexBot;
            }

            // se i due punti appartengono allo stesso frame faccio un unico disegno
            if (frameIndexTop == frameIndexBot)
            {
                Rectangle window = new Rectangle(1, top, 480, 800);
                spriteBatch.Draw(_backgroundTextureArray[frameIndexTop], Vector2.Zero, window, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }
            else
            // altrimenti devo spezzare in due disegni
            {
                Rectangle topWindow = new Rectangle(1, top, 480, _frameHeight - top);
                spriteBatch.Draw(_backgroundTextureArray[frameIndexTop], Vector2.Zero, topWindow, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                Rectangle botWindow = new Rectangle(1, 1, 480, 800 - (_frameHeight - top));
                spriteBatch.Draw(_backgroundTextureArray[frameIndexBot], new Vector2(0, _frameHeight - top), botWindow, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (_effects)
            {
                case BackgroundEffects.none:
                    DrawWindow(spriteBatch, _cursor, 1f);
                    break;

                case BackgroundEffects.trembling:
                    if (!_separationDistorion)
                    {
                        DrawWindow(spriteBatch, _trembleCursor1, 1f);
                    }
                    else
                    {
                        DrawWindow(spriteBatch, _trembleCursor1, 0.5f);
                        DrawWindow(spriteBatch, _trembleCursor2, 0.5f);
                    }
                    break;

                default:
                    break;
            }  
        }

        public void Update(GameTime gameTime)
        {
            // normal scrolling
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _cursor = _cursor - _speed * dt;

            // handle trembling if active
            if (_effects == BackgroundEffects.trembling)
            {
                _utilityTimer += dt;
                if (_utilityTimer > _trembleTime)
                {
                    _effects = BackgroundEffects.none;
                }
                else
                {
                    _trembleAmplitude = _trembleAmplitudeCurve.NextVariableValue(dt);
                    _trembleCursor1 = _cursor - (float)(_trembleAmplitude * Math.Sin(_trembleFrequency * _utilityTimer));
                    _trembleCursor2 = _cursor - (float)(_trembleAmplitude * Math.Sin(_trembleFrequency * _utilityTimer + _tremblePhase));
                } 
            }
        }
    }
}
