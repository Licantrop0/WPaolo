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

        // score
        SpriteFont _scoreString;
        SpriteFont _bombString;
        SpriteFont _fpsString;

        // event scheduler
        GameEventsManager _eventsManager = new GameEventsManager();

        // white globulos factory
        MonsterBonusFactory _whiteGlobulosFactory;

        // white globulos
        List<WhiteGlobulo> _whiteGlobulos = new List<WhiteGlobulo>();
        List<GoToVirusBonus> _bonuses = new List<GoToVirusBonus>();     // TEMP, bisogna ritrutturare bene la gerarchia di classi mostri / bonus!!!

        // virus
        Virus _virus;

        // background
        MovingBackground _background;
        MovingBackground _firstPlanBackground;

        // easter egg
        Texture2D _blazeBaley;

        Vector2 _touchPoint;

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

            // create score fonts
            _scoreString = Content.Load<SpriteFont>("Segoe20");
            _bombString = Content.Load<SpriteFont>("Segoe20");
            _fpsString = Content.Load<SpriteFont>("Segoe20");

            // create easter egg
            _blazeBaley = Content.Load<Texture2D>("BB");

            // create our friend virus
            Texture2D virusTexture = Content.Load<Texture2D>("virus");
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new Animation(virusTexture, 7));
            _virus = new Virus(virusAnimations, 37, 37);
            
            // create white globulos factory
            //Texture2D whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulos");
            Texture2D whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulosBigger");
            Texture2D whiteGlobuloExTexture = Content.Load<Texture2D>("whiteGlobulosEx");
            Texture2D whiteGlobulosOrbTexture = Content.Load<Texture2D>("whiteGlobulosOrb");
            Texture2D bonusBombTexture = Content.Load<Texture2D>("bonusBomb");
            _whiteGlobulosFactory = new MonsterBonusFactory(_eventsManager, _whiteGlobulos, _bonuses,
                whiteGlobulosTexture, whiteGlobuloExTexture, whiteGlobulosOrbTexture, bonusBombTexture,
               TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(3000),     // time interval period for enemies creation schedule (min,max)
               TimeSpan.FromMilliseconds(100) , TimeSpan.FromMilliseconds(1000),     // time offset from schedule to creation (min,max) 
               60, 150,                                                              // enemies speed (min,max)
               2, 6);                                                                // number of enemies created per schedule

            // create background
            Texture2D backgroundTexture0 = Content.Load<Texture2D>("polmoni0");
            Texture2D backgroundTexture1 = Content.Load<Texture2D>("polmoni1");
            _background = new MovingBackground(new Texture2D[2] { backgroundTexture0, backgroundTexture1 });
            _background.Speed = 15f;    // [px/sec]

            // create first plan background
            Texture2D firstPlanBackground0 = Content.Load<Texture2D>("b0");
            Texture2D firstPlanBackground1 = Content.Load<Texture2D>("b1");
            Texture2D firstPlanBackground2 = Content.Load<Texture2D>("b2");
            Texture2D firstPlanBackground3 = Content.Load<Texture2D>("b3");
            _firstPlanBackground = new MovingBackground(new Texture2D[4] { firstPlanBackground0, firstPlanBackground1, firstPlanBackground2, firstPlanBackground3 });
            _firstPlanBackground.Speed = 30f;   // [px/sec]

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3),
               GameEventType.scheduleSimpleEnemyCreation,
               _whiteGlobulosFactory));

            // SOLO GLOBULI BIANCHI PER ADESSO!!!
            /*_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(5),
                GameEventType.scheduleAcceleratedEnemyCreation,
                _whiteGlobulosFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(20),
                GameEventType.scheduleOrbitalEnemyCreation,
                _whiteGlobulosFactory));*/

            // create bonusbomb every 30 seconds (OVVIAMENTE VA RIFATTO BENE!!!)
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(30),
                GameEventType.createBombBonus,
            _whiteGlobulosFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(60),
                GameEventType.createBombBonus,
            _whiteGlobulosFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(90),
                GameEventType.createBombBonus,
            _whiteGlobulosFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(120),
                GameEventType.createBombBonus,
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

     
        private void GetUserTouch()
        {
            // we use raw touch points for selection, since they are more appropriate
            // for that use than gestures. so we need to get that raw touch data
            TouchCollection touches = TouchPanel.GetState();

            // see if we have a new primary point down. when the first touch
            // goes down, we do hit detection to try and select one of our enemies
            if (_virus != null && _virus.Score > 0 && touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
            {
                // convert the touch position into a Point for hit testing
                _touchPoint = new Vector2(touches[0].Position.X, touches[0].Position.Y);
                _virus.Score -= 1;
            }
            else
            {
                _touchPoint = Vector2.Zero;
            }
        }

        private void DetectTouchCollisions()
        {
            // if virus has been touched, a bomb may explode...
            if (_virus != null)
            {
                if (_touchPoint != Vector2.Zero && _virus.Touched(_touchPoint) && _virus.Bombs > 0)
                {
                    _virus.Bombs--;
                    _whiteGlobulos.ForEach(wg => wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit)));
                }
            }

            // iterate our enemied sprites to find which sprite is being touched.
            foreach (CircularSprite wg in _whiteGlobulos)
            {
                if (_touchPoint != Vector2.Zero && wg.Touched(_touchPoint))
                {
                    wg.AddSpriteEvent(new SpriteEvent((int)SpriteEventCode.fingerHit));
                }
            }
        }

        private void DetectVirusGlobulosCollision()
        {
            if (_virus != null)
            {
                foreach (WhiteGlobulo wg in _whiteGlobulos)
                {
                    if ((wg.State == WhiteGlobuloState.moving) && Vector2.Distance(wg.Position, _virus.Position) < wg.Radius + _virus.Radius)
                    {
                        // collision occurred, calculate angle of impact
                        float collisionAngle = (float)Math.Atan2(wg.Position.Y - _virus.Position.Y, wg.Position.X - _virus.Position.X);

                        _virus.AddSpriteEvent(new SpriteEvent(SpriteEventCode.virusGlobuloCollision, new Object[] { collisionAngle }));
                        wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.virusGlobuloCollision));
                    }
                }

                foreach (GoToVirusBonus b in _bonuses)
                {
                    if ((b.State == BonusState.moving) && Vector2.Distance(b.Position, _virus.Position) < b.Radius + _virus.Radius)
                    {
                        _virus.AddSpriteEvent(new SpriteEvent(SpriteEventCode.virusBonusCollision));
                        b.AddSpriteEvent(new SpriteEvent(SpriteEventCode.virusBonusCollision));
                    }
                }
            }
        }

        private void ClearOutOfBoundOrDeadEnemies()
        {
            // destroy out of screens enemies or dead enemies
            int iterations = _whiteGlobulos.Count;
            for (int i = 0, j = 0; i < iterations; i++, j++)
            {
                if (_whiteGlobulos[j].State == WhiteGlobuloState.died ||
                    _whiteGlobulos[j].Position.X < -250 || _whiteGlobulos[j].Position.X > 730 ||
                    _whiteGlobulos[j].Position.Y < -250 || _whiteGlobulos[j].Position.Y > 1050)
                {
                    _whiteGlobulos.RemoveAt(j);
                    j--;
                }
            }
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

            // handle user input
            GetUserTouch();

            // detect touch collisions
            DetectTouchCollisions();

            // detect collisions between our friend virus and evil globulos
            DetectVirusGlobulosCollision();

            // scroll the background
            _background.Update(gameTime);
            _firstPlanBackground.Update(gameTime);

            // update the enemies
            _whiteGlobulos.ForEach(wg => wg.Update(gameTime));

            // update the bonuses
            _bonuses.ForEach(b => b.Update(gameTime));

            // animate our friend virus
            if(_virus != null)
                _virus.Update(gameTime);

            // clear enemies
            ClearOutOfBoundOrDeadEnemies();

            // clear virus  :-(
            if (_virus != null && _virus.State == ViruState.died)
                _virus = null;

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
            //GraphicsDevice.Clear(Color.CornflowerBlue); // poi ci sarà da disegnare lo sfondo...
            //GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // draw background
            _background.Draw(spriteBatch);
            _firstPlanBackground.Draw(spriteBatch);

            // draw bonuses
            _bonuses.ForEach(b => b.Draw(spriteBatch));

            // draw enemies
            _whiteGlobulos.ForEach(wg => wg.Draw(spriteBatch));

            // draw our friend virus
            if(_virus != null)
                _virus.Draw(spriteBatch);

            // write score
            if (_virus != null)
            {
                spriteBatch.DrawString(_scoreString, _virus.Score.ToString(), new Vector2(370, 12), Color.Yellow);
                spriteBatch.DrawString(_bombString, _virus.Bombs.ToString(), new Vector2(30, 12), Color.Yellow);
            }
            else
            {
                //spriteBatch.DrawString(_segoe20, "YOU\nSUCK!", new Vector2(370, 12), Color.White);
                // easter egg
                spriteBatch.DrawString(_scoreString, "YOU SUCK!", new Vector2(170, 300), Color.White);
                spriteBatch.Draw(_blazeBaley, new Vector2(240 - _blazeBaley.Width / 2, 400 - _blazeBaley.Height / 2), Color.White);
            }

            spriteBatch.DrawString(_fpsString, (Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2).ToString()), new Vector2(30, 768), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
