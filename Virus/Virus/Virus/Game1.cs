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

        // white globulos and bonuses factory
        SpriteFactory _spriteFactory;

        // white globulos
        List<WhiteGlobulo> _whiteGlobulos = new List<WhiteGlobulo>();
        List<GoToVirusBonus> _bonuses = new List<GoToVirusBonus>();     // TEMP, bisogna ritrutturare bene la gerarchia di classi mostri / bonus!!!

        // virus
        Virus _virus;

        // virus lifes
        Texture2D _virusLife;

        // virus ammo management
        bool _enemyKilledByAmmo;
        int _enemiesKilledByAmmoCounter;
        int _enemiesKilledByAmmoTriggerNumber;
        int _ammoQuantityPerBonus;

        // background
        MovingBackground _background;
        MovingBackground _firstPlanBackground;

        // touch point
        Vector2 _touchPoint;

        // profiling
        float _fpsSum = 0;
        int _fpsCount = 1;

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

            // create our friend virus
            //Texture2D virusTexture = Content.Load<Texture2D>("virus");
            //Texture2D virusTexture = Content.Load<Texture2D>("virusBigger");
            Texture2D virusTexture = Content.Load<Texture2D>("virusMedium");
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new Animation(virusTexture, 7));
            _virus = new Virus(virusAnimations, 40, 40);
            
            // create white globulos factory
            //Texture2D whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulos");
            //Texture2D whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulosBigger");
            Texture2D whiteGlobulosTexture = Content.Load<Texture2D>("whiteGlobulosBiggest");
            Texture2D whiteGlobuloExTexture = Content.Load<Texture2D>("whiteGlobulosEx");
            Texture2D whiteGlobulosOrbTexture = Content.Load<Texture2D>("whiteGlobulosOrb");
            Texture2D bonusBombTexture = Content.Load<Texture2D>("bonusBomb");
            _spriteFactory = new SpriteFactory(_eventsManager, _whiteGlobulos, _bonuses,
                whiteGlobulosTexture, whiteGlobuloExTexture, whiteGlobulosOrbTexture, bonusBombTexture,
               TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(2500),     // time interval period for enemies creation schedule (min,max)
               TimeSpan.FromMilliseconds(100) , TimeSpan.FromMilliseconds(1000),     // time offset from schedule to creation (min,max) 
               1.9f, 3.2f,                                                           // enemies time to reach virus (min,max)
               3, 5);                                                                // number of enemies created per schedule

            // create life virus
            _virusLife = Content.Load<Texture2D>("virusLifeLittle");

            // create background
            Texture2D backgroundTexture0 = Content.Load<Texture2D>("polmoni0");
            Texture2D backgroundTexture1 = Content.Load<Texture2D>("polmoni1");
            _background = new MovingBackground(new Texture2D[2] { backgroundTexture0, backgroundTexture1 }, 30);
            _background.Speed = 15f;    // [px/sec]

            // create first plan background
            Texture2D firstPlanBackground0 = Content.Load<Texture2D>("b0");
            Texture2D firstPlanBackground1 = Content.Load<Texture2D>("b1");
            Texture2D firstPlanBackground2 = Content.Load<Texture2D>("b2");
            Texture2D firstPlanBackground3 = Content.Load<Texture2D>("b3");
            _firstPlanBackground = new MovingBackground(new Texture2D[4] { firstPlanBackground0, firstPlanBackground1, firstPlanBackground2, firstPlanBackground3 }, 60);
            _firstPlanBackground.Speed = 30f;   // [px/sec]

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3),
               GameEventType.scheduleSimpleEnemyCreation,
               _spriteFactory));

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
            _spriteFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(60),
                GameEventType.createBombBonus,
            _spriteFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(90),
                GameEventType.createBombBonus,
            _spriteFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(120),
                GameEventType.createBombBonus,
            _spriteFactory));
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
            if (_virus.Ammo > 0 && touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
            {
                // convert the touch position into a Point for hit testing
                _touchPoint = new Vector2(touches[0].Position.X, touches[0].Position.Y);
                _virus.Ammo -= 1;
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

                    // tremble
                    CustomTimeVariable trembleAmplitude = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 30), new Vector2(1, 30), new Vector2(1.25f, 0) });
                    CustomTimeVariable trembleAmplitude2 = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 15), new Vector2(1, 15), new Vector2(1.25f, 0) });
                    _background.Tremble(2, trembleAmplitude, 40, (float)Math.PI, true);
                    _firstPlanBackground.Tremble(2, trembleAmplitude2, 80, (float)Math.PI, true);

                    foreach(GoToVirusBonus b in _bonuses)
                    {
                        b.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
                        if (b.GetBonusType == "bomb")
                        {
                            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(1), GameEventType.createAmmoBonus, _spriteFactory));
                        }
                    }
                }
            }

            // iterate our enemied sprites to find which sprite is being touched.
            foreach (WhiteGlobulo wg in _whiteGlobulos)
            {
                if (_touchPoint != Vector2.Zero && wg.Touched(_touchPoint))
                {
                    wg.AddSpriteEvent(new SpriteEvent((int)SpriteEventCode.fingerHit));
                    _enemyKilledByAmmo = true;
                }
            }

            if (_enemyKilledByAmmo)
            {
                _enemiesKilledByAmmoCounter++;

                if (_enemiesKilledByAmmoCounter == _enemiesKilledByAmmoTriggerNumber)
                {
                    _enemiesKilledByAmmoCounter = 0;
                    _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(1), GameEventType.createAmmoBonus, _spriteFactory));
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

        private void ClearBonuses()
        {
            int iterations = _bonuses.Count;
            for (int i = 0, j = 0; i < iterations; i++, j++)
            {
                if (_bonuses[j].State == BonusState.toBeCleared)
                {
                    _bonuses.RemoveAt(j);
                    j--;
                }
            }
        }

        private void DrawLifes(SpriteBatch spriteBatch)
        {
            Vector2 position = new Vector2(450, 8);
            for (int i = 0; i < _virus.Lifes; i++)
            {
                spriteBatch.Draw(_virusLife, position, Color.White);
                position -= new Vector2(20, 0);
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
            if (_virus != null)
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

            // clear bonuses
            ClearBonuses();

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

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

            // draw lifes
            if (_virus != null)
                DrawLifes(spriteBatch);

            // write score
            if (_virus != null)
            {
                //priteBatch.DrawString(_scoreString, _virus.Score.ToString(), new Vector2(370, 12), Color.Yellow);
                spriteBatch.DrawString(_bombString, _virus.Bombs.ToString(), new Vector2(30, 4), Color.Yellow);
            }
            else
            {
                spriteBatch.DrawString(_scoreString, "YOU\nSUCK!", new Vector2(370, 4), Color.White);
            }

            _fpsSum += (float)(1 / gameTime.ElapsedGameTime.TotalSeconds); 
            spriteBatch.DrawString(_fpsString, (_fpsSum / (float)_fpsCount).ToString(), new Vector2(30, 768), Color.Yellow);
            _fpsCount++;

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
