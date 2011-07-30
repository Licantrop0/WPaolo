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

        // writings
        SpriteFont _ammoString;
        SpriteFont _bombString;
        SpriteFont _delayString;
        SpriteFont _timeString;

        // event scheduler
        GameEventsManager _eventsManager = new GameEventsManager();

        // white globulos and bonuses factory
        MonsterFactory _monsterFactory;
        BonusFactory _bonusFactory;

        // white globulos
        List<WhiteGlobulo> _whiteGlobulos = new List<WhiteGlobulo>();

        // bonuses
        List<GoToVirusBonus> _bonuses = new List<GoToVirusBonus>();

        // boss
        List<BossLung> _bossContainer = new List<BossLung>();

        // virus
        Virus _virus;

        // virus lifes
        Texture2D _virusLife;

        // ammo bar
        Texture2D _ammobar;

        // virus ammo management
        int _enemiesKilledByAmmoCounter;
        int _enemiesKilledByAmmoTriggerNumber;
        int _ammoQuantityPerBonus;  

        // difficulty parameters
        Level1DifficultyPackEnemies[] _level1DifficultyPack = new Level1DifficultyPackEnemies[3];

        // background
        MovingBackground _background;
        MovingBackground _firstPlanBackground;

        // touch point
        Vector2 _touchPoint;

        // profiling
        int _delayCount = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set screen features
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.IsFullScreen = true;

            //IsFixedTimeStep = false;

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
            _enemiesKilledByAmmoTriggerNumber = 50;

            base.Initialize();

            GC.Collect();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create score and debug fonts and ammo bar
            _ammoString = Content.Load<SpriteFont>("Segoe20");
            _bombString = Content.Load<SpriteFont>("Segoe20");
            _delayString = Content.Load<SpriteFont>("Segoe20");
            _timeString = Content.Load<SpriteFont>("Segoe20");
            _ammobar = Content.Load<Texture2D>("ammobar");

            // create our friend virus
            Texture2D virusTexture = Content.Load<Texture2D>("Sprites/Virus/virusMedium");
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new SpriteSheetAnimation(7, true, virusTexture));
            _virus = new Virus(virusAnimations, 40, 40);
            
            // set ascending climax difficulty pack!

            _level1DifficultyPack[0] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(2500),
                                                                      TimeSpan.FromMilliseconds(100)  , TimeSpan.FromMilliseconds(1000),
                                                                      2.4f, 3.2f,
                                                                      2, 3);
            _level1DifficultyPack[1] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(1750), TimeSpan.FromMilliseconds(2500),
                                                                      TimeSpan.FromMilliseconds(100)  , TimeSpan.FromMilliseconds(1000),
                                                                      2.1f, 3.2f,
                                                                      2, 5);
            _level1DifficultyPack[2] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(2500),
                                                                      TimeSpan.FromMilliseconds(100)  , TimeSpan.FromMilliseconds(1000),
                                                                      1.9f, 3.2f,
                                                                      3, 5);

            // create Monster Factory
            _monsterFactory = new MonsterFactory(_eventsManager, _whiteGlobulos,  _bossContainer, new AnimationFactory( Content, "Animations/Enemies.xml"))
            {
                VirusPosition = new Vector2(240, 400),
            };

            // create Bonus Factory
            _bonusFactory = new BonusFactory(_eventsManager, _bonuses, new AnimationFactory( Content, "Animations/Bonuses.xml"))
            {
                VirusPosition = new Vector2(240, 400),
            };
            _bonusFactory.SetBombBonusTimePeriod(30, 50);
            
            // create life virus
            _virusLife = Content.Load<Texture2D>("virusLifeLittle");

            // create background
            Texture2D backgroundTexture0 = Content.Load<Texture2D>("polmoni0");
            Texture2D backgroundTexture1 = Content.Load<Texture2D>("polmoni1");
            _background = new MovingBackground(new Texture2D[2] { backgroundTexture0, backgroundTexture1 }, 30, 30);
            _background.Speed = 15f;    // [px/sec]

            // create first plan background
            Texture2D firstPlanBackground0 = Content.Load<Texture2D>("b0");
            Texture2D firstPlanBackground1 = Content.Load<Texture2D>("b1");
            Texture2D firstPlanBackground2 = Content.Load<Texture2D>("b2");
            Texture2D firstPlanBackground3 = Content.Load<Texture2D>("b3");
            _firstPlanBackground = new MovingBackground(new Texture2D[4] { firstPlanBackground0, firstPlanBackground1, firstPlanBackground2, firstPlanBackground3 }, 60, 60);
            _firstPlanBackground.Speed = 30f;   // [px/sec]

            ScheduleEvents();
        }

        private void ScheduleEvents()
        {
            //// set difficulty
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _level1DifficultyPack[0] }));
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(30), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _level1DifficultyPack[1] }));
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(100), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _level1DifficultyPack[2] }));

            //// schedule and go on scheduling white globulos creation
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3), GameEventType.scheduleSimpleEnemyCreation, _monsterFactory));

            //// create bonusbomb every 30/50 seconds (as set up before)
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.ScheduleBombBonusCreation, _bonusFactory));

            //// create bombplus at 45 and 130 seconds
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(45), GameEventType.createBombPlusBonus, _bonusFactory));
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(130), GameEventType.createBombPlusBonus, _bonusFactory));

            //// create one up bonus at 135 seconds
            //_eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(135), GameEventType.createOneUpBonus, _bonusFactory));        

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.createBombPlusBonus, _bonusFactory));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.createBombPlusBonus, _bonusFactory));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.createBombPlusBonus, _bonusFactory));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3), GameEventType.createBossLung, _monsterFactory));
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
                _virus.Ammo--;
            }
            else
            {
                _touchPoint = Vector2.Zero;
            }
        }

        private void DetectTouchCollisions()
        {
            bool recreateAmmoBonus = false;
            int enemiesKilled = 0;

            // if virus has been touched, a bomb explodes!
            if (_virus != null)
            {
                if (_virus.Touched(_touchPoint) && _virus.Bombs > 0)
                {
                    // bomb explosion handling!
                    _virus.Bombs--;
                    enemiesKilled++;
                    _whiteGlobulos.ForEach(wg => wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit)));

                    foreach(GoToVirusBonus b in _bonuses)
                    {
                        b.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
                        if (b.Type == BonusType.ammo && b.State == BonusState.moving)
                            recreateAmmoBonus = true;
                    }

                    // tremble
                    CustomTimeVariable trembleAmplitude = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 60), new Vector2(1, 60), new Vector2(1.25f, 0) });
                    CustomTimeVariable trembleAmplitude2 = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 30), new Vector2(1, 30), new Vector2(1.25f, 0) });
                    _background.Tremble(2, trembleAmplitude, 30, (float)Math.PI, false);
                    _firstPlanBackground.Tremble(2, trembleAmplitude2, 60, (float)Math.PI, false);
                }
            }

            // iterate our enemied sprites to find which sprite is being touched.
            foreach (WhiteGlobulo wg in _whiteGlobulos)
            {
                if ( wg.Touched(_touchPoint))
                {
                    wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.fingerHit));
                    enemiesKilled++;
                }
            }

            // iterate boss hittable sprites to find which sprite is being touched
            if (_bossContainer.Count != 0)
            {
                _bossContainer[0].HandleUserTouch(_touchPoint, ref enemiesKilled);
            }

            // ammo bonus handling
            _enemiesKilledByAmmoCounter += enemiesKilled;
            if (_enemiesKilledByAmmoCounter >= _enemiesKilledByAmmoTriggerNumber)
            {
                _enemiesKilledByAmmoCounter = 0;
                _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(1), GameEventType.createAmmoBonus, _bonusFactory));
            }


            // iterate our bonus sprites to find which sprite is being touched
            foreach (GoToVirusBonus b in _bonuses)
            {
                if (b.Touched(_touchPoint))
                {
                    b.AddSpriteEvent(new SpriteEvent((int)SpriteEventCode.fingerHit));
                    if (b.Type == BonusType.ammo)
                        recreateAmmoBonus = true;
                }
            }

            if (recreateAmmoBonus)
            {
                _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(1), GameEventType.createAmmoBonus, _bonusFactory));
            }
            
        }

        private void DetectGlobulosBorderCollision()
        {
            foreach (WhiteGlobulo wg in _whiteGlobulos)
            {

                if (wg.Speed.X > 0)
                {
                    // collision with right border
                    if (wg.Position.X + wg.Radius >= 480)
                    {
                        wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.borderCollision, new Object[] { new Vector2(-1, 0) }));
                        continue;
                    } 
                }
                else
                {
                    // collision with left border
                    if (wg.Position.X - wg.Radius <= 0)
                    {
                        wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.borderCollision, new Object[] { new Vector2(1, 0) }));
                        continue;
                    }       
                }

                if (wg.Speed.Y > 0)
                {
                    // collision with bottom border
                    if (wg.Position.Y + wg.Radius >= 800)
                    {
                        wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.borderCollision, new Object[] { new Vector2(0, -1) }));
                        continue;
                    }      
                }
                else
                {
                    // collision with top border
                    if (wg.Position.Y - wg.Radius <= 0)
                    {
                        wg.AddSpriteEvent(new SpriteEvent(SpriteEventCode.borderCollision, new Object[] { new Vector2(0, 1) }));
                        continue;
                    } 
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
                        BonusType param = b.Type;

                        _virus.AddSpriteEvent(new SpriteEvent(SpriteEventCode.virusBonusCollision, new Object[] { param } ));
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
            if (_touchPoint != Vector2.Zero)
                DetectTouchCollisions();

            // detect collisions between our friend virus and evil globulos
            DetectVirusGlobulosCollision();

            DetectGlobulosBorderCollision();

            // scroll the background
            _background.Update(gameTime);
            _firstPlanBackground.Update(gameTime);
            
            // update the enemies
            _whiteGlobulos.ForEach(wg => wg.Update(gameTime));

            // update the bonuses
            _bonuses.ForEach(b => b.Update(gameTime));

            // udpate boss
            if (_bossContainer.Count != 0)
                _bossContainer[0].Update(gameTime);

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

            // draw boss
            if (_bossContainer.Count != 0)
                _bossContainer[0].Draw(spriteBatch);

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
                spriteBatch.DrawString(_ammoString, _virus.Ammo.ToString(), new Vector2(80, 4), Color.Yellow);
                spriteBatch.DrawString(_bombString, _virus.Bombs.ToString(), new Vector2(30, 4), Color.Yellow);

                // draw ammobar
                //spriteBatch.Draw(_ammobar, new Vector2(0, 790), null, Color.Green, Vector2.One, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(_ammobar, new Vector2(0, 790), null, Color.Green, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            }

            if ( gameTime.IsRunningSlowly )
                _delayCount++;

            spriteBatch.DrawString(_delayString, _delayCount.ToString(), new Vector2(30, 768), Color.Yellow);
            spriteBatch.DrawString(_timeString, Math.Round(gameTime.TotalGameTime.TotalSeconds, 0).ToString(), new Vector2(410, 768), Color.Yellow);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
