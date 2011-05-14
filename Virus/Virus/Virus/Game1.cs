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

namespace Virus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // event scheduler
        GameEventsManager _eventsManager = new GameEventsManager();

        // white globulos factory
        MonsterFactory _whiteGlobulosFactory;

        // white globulos
        List<SimpleEnemy> _whiteGlobulos = new List<SimpleEnemy>();

        // content
        Texture2D _whiteGlobulosTexture;

        // game timer
        //float _gameTimer = 0;

        // time sampling
        float dt;      // [sec]

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

            // set sampling time accordingly
            dt = 0.0333333f;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // enable the gestures we care about. you must set EnabledGestures before
            // you can use any of the other gesture APIs.
            // we use both Tap and DoubleTap to workaround a bug in the XNA GS 4.0 Beta
            // where some Taps are missed if only Tap is specified.
            TouchPanel.EnabledGestures =
                GestureType.Tap |
                GestureType.DoubleTap;

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
            _whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulos");
            // create white globulos factory
            _whiteGlobulosFactory = new MonsterFactory(_eventsManager, _whiteGlobulos, _whiteGlobulosTexture,
               TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(3000),     // time interval period for enemies creation schedule (min,max)
               TimeSpan.FromMilliseconds(100) , TimeSpan.FromMilliseconds(1000),     // time offset from schedule to creation (min,max) 
               60, 150,                                                              // enemies speed (min,max)
               1, 5);                                                                // number of enemies created per schedule

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3),
               GameEventType.scheduleSimpleEnemyCreation,
               _whiteGlobulosFactory));
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

            // move the enemies
            _whiteGlobulos.ForEach(wg => wg.Move(dt));

            // destroy out of screens enemies
            int iterations = _whiteGlobulos.Count;
            for (int i = 0, j = 0; i < iterations; i++, j++)
            {
                if (_whiteGlobulos[j].Position.X < -50 || _whiteGlobulos[j].Position.X > 530 ||
                    _whiteGlobulos[j].Position.Y < -50 || _whiteGlobulos[j].Position.Y > 850)
                {
                    _whiteGlobulos.RemoveAt(j);
                    j--;
                }
            }

            // detect collision and update ammo, life...


            // manage current event (if any)
            _eventsManager.ManageCurrentEvent(gameTime.TotalGameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); // poi ci sarà da disegnare lo sfondo...

            spriteBatch.Begin();
            _whiteGlobulos.ForEach(wg => spriteBatch.Draw(wg.Texture, wg.TexturePosition, Color.White));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
