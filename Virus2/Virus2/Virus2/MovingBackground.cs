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

    public class MovingBackgroundConfig
    {
        public Texture2D Texture { get; set; }
        public int Repetitions { get; set; }

        public MovingBackgroundConfig(Texture2D texture, int rep)
        {
            Texture = texture;
            Repetitions = rep;
        }
    }

    public class MovingBackground
    {
        //Texture2D[] _backgroundTextureArray;
        MovingBackgroundConfig[] _backgroundTextures;
        int[] _cumulativeIndexLookUp;

        // general state data
        float _cursor;       
        float _speed;
        BackgroundEffects _effects;
        int _frameHeight;
        float _goalLine;

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

        public MovingBackground(MovingBackgroundConfig[] textureArray, float startOffset, float endOffset)
        {
            _backgroundTextures = textureArray;

            _cumulativeIndexLookUp = new int[_backgroundTextures.Sum(p => p.Repetitions)];
            int index = 0;
            for (int i = 0; i < _backgroundTextures.Length; i++)
            {
                for (int j = 0; j < _backgroundTextures[i].Repetitions; j++)
                {
                    _cumulativeIndexLookUp[index] = i;
                    index++;
                }
            }
            
            _speed = 0;
            _effects = BackgroundEffects.none;

            _frameHeight = _backgroundTextures[0].Texture.Height;
            // offset is an offset used to have some "room" for effect like trembling and such...
            _cursor = _frameHeight * _cumulativeIndexLookUp.Length - 800 - startOffset;     
            _goalLine = endOffset;
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

        /*private void DrawWindow(SpriteBatch spriteBatch, float position, float alphaBlending)
        {
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
        }*/

        private void DrawWindow(SpriteBatch spriteBatch, float position, float alphaBlending)
        {
            int cursor = (int)Math.Round(position);

            int absoluteFrameIndexTop, absoluteFrameIndexBot, frameIndexTop, frameIndexBot;

            absoluteFrameIndexTop = (int)(position / _frameHeight);
            frameIndexTop = _cumulativeIndexLookUp[absoluteFrameIndexTop];

            absoluteFrameIndexBot = (int)((position + 800) / _frameHeight);
            frameIndexBot = _cumulativeIndexLookUp[absoluteFrameIndexBot];

            int top = cursor % _frameHeight;

            if (top == 0)   // flickering avoid!
            {
                frameIndexTop = frameIndexBot;
            }

            // se i due punti appartengono allo stesso frame faccio un unico disegno
            if (absoluteFrameIndexTop == absoluteFrameIndexBot)
            {
                Rectangle window = new Rectangle(1, top, 480, 800);
                spriteBatch.Draw(_backgroundTextures[frameIndexTop].Texture, Vector2.Zero, window, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }
            else
            // altrimenti devo spezzare in due disegni
            {
                Rectangle topWindow = new Rectangle(1, top, 480, _frameHeight - top);
                spriteBatch.Draw(_backgroundTextures[frameIndexTop].Texture, Vector2.Zero, topWindow, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

                Rectangle botWindow = new Rectangle(1, 1, 480, 800 - (_frameHeight - top));
                spriteBatch.Draw(_backgroundTextures[frameIndexBot].Texture, new Vector2(0, _frameHeight - top), botWindow, new Color(1f, 1f, 1f, alphaBlending), 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
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
                        DrawWindow(spriteBatch, _trembleCursor1, 1f);
                        DrawWindow(spriteBatch, _trembleCursor2, 0.5f);
                    }
                    break;

                default:
                    break;
            }  
        }

        public void Update(TimeSpan gameTime)
        {
            // normal scrolling
            float dt = (float)gameTime.TotalSeconds;
            _cursor = _cursor - _speed * dt;

            // goal line range check!
            if (_cursor < _goalLine)
                _cursor = _goalLine;

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
