using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace BalanceBall
{
    public enum State
    {
        idle,
        ballSizing,
        ballFallingAndBouncing,
        weighting
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game state
        State _state = State.idle;

        // game input
        MouseState _mouseState;

        // textures
        Texture2D _redBall;
        Texture2D _blackPlatform;

        // left ball features
        float _radius = 0;                  // [px]
        float _scalingSpeed = 10;           // [px / sec]
        Vector2 _position;                  // [px | px]   
        Vector2 _speed;                     // [px/sec | px/sec]
        float _mass;                        // [Kg]
        float _damping = 0f;                // [Kg / sec]
        float _platformYposition = 400;     // [px]
        float _targetPlatformYPosition;     // [px]
        float _platformSpeed = 50;          // [px]

        float _rightMass = 0;             // [Kg]
        
        // common features
        float _specificWeight = 0.0001f;    // [Kg / px^3]
        float _gainPulley = 180;
        float _gravity =  200;              // [(Kg*px) / sec^2]
        float _kCollision = 0.80f;          // 0 - 1 it represents the fraction of energy absorved in collision

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set screen features
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.IsFullScreen = true;

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _redBall = Content.Load<Texture2D>("redBall");
            _blackPlatform = Content.Load<Texture2D>("BlackPlatform");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // get the touch input
            HandleTouchInput();

            switch (_state)
            {
                case State.idle:

                    if (_mouseState.LeftButton == ButtonState.Pressed)
                    {
                        _radius = _radius + dt * _scalingSpeed;
                        _position = new Vector2(_mouseState.X, _mouseState.Y);
                        _state = State.ballSizing;
                    }
                    // else remain idle...

                    break;

                case State.ballSizing:

                    if (_mouseState.LeftButton == ButtonState.Pressed)
                    {
                        _radius = _radius + dt * _scalingSpeed;
                        Debug.WriteLine(_radius);
                    }
                    else
                    {
                        _mass = (4 / 3) * (float)Math.PI * _radius * _radius * _radius * _specificWeight;
                        _speed = new Vector2(0, 0);
                        _state = State.ballFallingAndBouncing;
                    }

                    break;

                case State.ballFallingAndBouncing:

                    // detect collision
                    if ( _speed.Y > 0 &&  _position.Y + _radius >= _platformYposition)
                    {
                        // bounce!
                        _speed.Y = - (float)Math.Sqrt((1 - _kCollision)) * _speed.Y; 
                    }

                    // move
                    Vector2 forceNet = new Vector2(0, _mass * _gravity - _damping * _speed.Y);
                    Vector2 a = forceNet / _mass;      // [px / sec2]
                    _speed     = _speed    + a      * dt;
                    _position  = _position + _speed  * dt;

                    if ( (Math.Abs(_speed.Y) < 3) && (Math.Abs(_position.Y - (_platformYposition - _radius)) < 3 ))
                    {
                        _speed.Y = 0;
                        _position.Y = _platformYposition - _radius;
                        float deltaMass = _mass - _rightMass;
                        _targetPlatformYPosition = 400 + _gainPulley * deltaMass;
                        _state = State.weighting;
                    }

                    break;

                case State.weighting:

                    if (Math.Abs(_platformYposition - _targetPlatformYPosition) > 1.6)
                    {
                        _platformYposition = _platformYposition + dt * _platformSpeed;
                        _position.Y = _platformYposition - _radius;
                    }
                    else
                    {
                        _platformYposition = _targetPlatformYPosition;
                        _position.Y = _platformYposition - _radius;
                        _state = State.idle;
                    }
                        
                    break;

                default:
                    break;
            }

            

            base.Update(gameTime);
        }

        private void HandleTouchInput()
        {
            // get the "mouse" state
            _mouseState = Mouse.GetState();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            int height = _redBall.Height;
            int width = _redBall.Width;
            float scale = (1f / 14f) * _radius;

            spriteBatch.Begin();
            // draw platforms
            spriteBatch.Draw(_blackPlatform, new Vector2(50, _platformYposition), Color.White);
            spriteBatch.Draw(_blackPlatform, new Vector2(290, 800 - _platformYposition), Color.White);

            spriteBatch.Draw(_redBall, _position, new Rectangle(0, 0, width, height), Color.White, 0, new Vector2(width / 2, height / 2), new Vector2(scale), SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
