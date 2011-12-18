using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;

namespace Virus
{
    public enum LevelState
    {
        none,
        loading,
        running,
        lost,
        finished
    }

    public class Level : GameScreen
    {
        // graphics manager and sprite batch (references to game graphics)
       // GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // condition of level finished
        //public bool Finished { get; set; }
        public LevelState State { get; set; }
        

        bool _bossDead = false;
        //float  _bossDeath; 

        // content manager
        ContentManager contentManager;

        // monster and globulos factory, initialized from level number
        MonsterFactory _monsterFactory;
        BonusFactory _bonusFactory;

        // event scheduler
        GameEventsManager _eventsManager = new GameEventsManager();

        // enemies
        List<Enemy> _enemies = new List<Enemy>();

        // bonuses
        List<GoToVirusBonus> _bonuses = new List<GoToVirusBonus>();

        // boss
        List<Boss> _bossContainer = new List<Boss>();

        InputAction pauseAction;

        // virus (reference)
        Virus _virus;
      
        // background
        MovingBackground _mainbackground;
        MovingBackground _firstPlanBackground;

        // difficulty parameters
        LevelDifficultyPack[] _levelDifficultyPack;

        // touch point
        Vector2 _touchPoint;

        // virus ammo management
        int _enemiesKilledByAmmoCounter;
        int _enemiesKilledByAmmoTriggerNumber;
        int _ammoQuantityPerBonus;

        public Level(int level, Virus virus)
        {
            _virus = virus;

            State = LevelState.loading;

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

        }



        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (contentManager == null)
                    contentManager = new ContentManager(ScreenManager.Game.Services, "Content");

             spriteBatch = ScreenManager.SpriteBatch;
               LoadLevel(1);


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();

                
            }

            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                //playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                //enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
        }


        public override void Deactivate()
        {
            //Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            //Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            contentManager.Unload();

            //Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            //Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
        }



        public void InitializeLevel(int level)
        {
            _eventsManager.ClearAllEvents();
            ScheduleEvents(level);
            
            _enemiesKilledByAmmoTriggerNumber = 50;
            State = LevelState.running;
        }

        private void LoadLevel(int level)
        {
            LoadContent(level);
            CreateDifficulty(level);

            EnabledGestures = GestureType.Tap;

        }


        private void CreateBackground(int level)
        {
            switch (level)
            {
                case 1:
                    // main background
                    Texture2D backgroundTexture0 = contentManager.Load<Texture2D>("NuoviSfondi\\SfondoCarne (bora)");
                    MovingBackgroundConfig[] mainBackGroundConfig =
                        new MovingBackgroundConfig[1] { new MovingBackgroundConfig(backgroundTexture0, 2) };

                    /*Texture2D backgroundTexture0 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni(boraprova)_0");
                    Texture2D backgroundTexture1 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni(boraprova)_1");
                    MovingBackgroundConfig[] mainBackGroundConfig =
                        new MovingBackgroundConfig[2] { new MovingBackgroundConfig(backgroundTexture0, 1), new MovingBackgroundConfig(backgroundTexture1, 1) };*/
                    _mainbackground = new MovingBackground(mainBackGroundConfig, 30, 30);
                    _mainbackground.Speed = 15f;    // [px/sec]

                    // create first plan background
                    Texture2D firstPlanBackground0 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni primo piano(bora)_0");
                    Texture2D firstPlanBackground1 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni primo piano(bora)_1");
                    Texture2D firstPlanBackground2 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni primo piano(bora)_2");
                    Texture2D firstPlanBackground3 = contentManager.Load<Texture2D>("NuoviSfondi\\polmoni primo piano(bora)_3");
                    MovingBackgroundConfig[] firstPlanBackgroundConfig =
                        new MovingBackgroundConfig[4] { new MovingBackgroundConfig(firstPlanBackground0, 1), new MovingBackgroundConfig(firstPlanBackground1, 1),
                                                        new MovingBackgroundConfig(firstPlanBackground2, 1), new MovingBackgroundConfig(firstPlanBackground3, 1) };
                    _firstPlanBackground = new MovingBackground(firstPlanBackgroundConfig, 60, 60);
                    _firstPlanBackground.Speed = 30f;   // [px/sec]

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void CreateBonusFactory(int level)
        {
            switch (level)
            {
                case 1:

                    // create Bonus Factory
                    _bonusFactory = new BonusFactory(_eventsManager, _bonuses, new SpritePrototypeContainer(contentManager, "AnimationConfig/Bonuses.xml"))
                    {
                        VirusPosition = new Vector2(240, 400),
                        BonusSpeed = 50
                    };
                    _bonusFactory.SetBombBonusTimePeriod(30, 50);

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void CreateMonsterFactory(int level)
        {
            switch (level)
            {
                case 1:

                    // create Monster Factory
                    _monsterFactory = new MonsterFactory(_eventsManager, _enemies, _bossContainer, new SpritePrototypeContainer(contentManager, "AnimationConfig/Enemies.xml"))
                    {
                        VirusPosition = new Vector2(240, 400),
                    };

                    MyLoadingScreen pippo = new MyLoadingScreen(contentManager, spriteBatch);
                    pippo.Load();
                    

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void CreateDifficulty(int level)
        {
            switch (level)
            {
                case 1:

                    _levelDifficultyPack = new Level1DifficultyPackEnemies[3];

                    _levelDifficultyPack[0] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(2500),
                                                                      TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000),
                                                                      2.4f, 3.2f,
                                                                      2, 3);
                    _levelDifficultyPack[1] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(1750), TimeSpan.FromMilliseconds(2500),
                                                                              TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000),
                                                                              2.1f, 3.2f,
                                                                              2, 5);
                    _levelDifficultyPack[2] = new Level1DifficultyPackEnemies(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(2500),
                                                                              TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000),
                                                                              1.9f, 3.2f,
                                                                              3, 4);

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void LoadContent(int level)
        {
            // 
            CreateBackground(level);
            CreateMonsterFactory(level);
            CreateBonusFactory(level);
            LoadSounds(level);
        }

        private void LoadSounds(int level)
        {
            switch (level)
            {
                case 1:
                    SoundManager.Load("barf", contentManager.Load<SoundEffect>("Sounds/barf"));
                    SoundManager.Load("hit", contentManager.Load<SoundEffect>("Sounds/hit"));
                    SoundManager.Load("miss", contentManager.Load<SoundEffect>("Sounds/miss"));
                    SoundManager.Load("powerup-hit", contentManager.Load<SoundEffect>("Sounds/powerup-hit"));
                    SoundManager.Load("powerup", contentManager.Load<SoundEffect>("Sounds/powerup"));
                    SoundManager.Load("small-mouth-death", contentManager.Load<SoundEffect>("Sounds/small-mouth-death"));
                    SoundManager.Load("small-mouth-opens", contentManager.Load<SoundEffect>("Sounds/small-mouth-opens"));
                    SoundManager.Load("virus-bomb", contentManager.Load<SoundEffect>("Sounds/virus-bomb"));
                    SoundManager.Load("virus-hit", contentManager.Load<SoundEffect>("Sounds/virus-hit"));

                    break;
            }
        }

        private void ScheduleEvents(int level)
        {
            //// set difficulty
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _levelDifficultyPack[0] }));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(30), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _levelDifficultyPack[1] }));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(100), GameEventType.ChangeLevel1Difficulty, _monsterFactory, new Object[] { _levelDifficultyPack[2] }));

            // schedule and go on scheduling white globulos creation
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(3), GameEventType.scheduleSimpleEnemyCreation, _monsterFactory));

            // create bonusbomb every 30/50 seconds (as set up before)
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(0), GameEventType.ScheduleBombBonusCreation, _bonusFactory));

            // create bombplus at 45 and 130 seconds
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(45), GameEventType.createBombPlusBonus, _bonusFactory));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(130), GameEventType.createBombPlusBonus, _bonusFactory));

            // create one up bonus at 135 seconds
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(135), GameEventType.createOneUpBonus, _bonusFactory));        

            // boss arrives
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(149), GameEventType.createOneUpBonus, _bonusFactory)); 
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(150), GameEventType.changeBonusSpeed, _bonusFactory, new object[] { 180 }));
            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(150), GameEventType.clearEvents, _monsterFactory, new object[] { new GameEventType[] { GameEventType.createSimpleEnemy, GameEventType.scheduleSimpleEnemyCreation }}));

            _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(153), GameEventType.createBossLung, _monsterFactory));
        }

        #region user input handle

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            //KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            //GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];


            // Read in our gestures
            foreach (GestureSample gesture in input.Gestures)
            {
                // If we have a tap
                if (gesture.GestureType == GestureType.Tap)
                {
                    _touchPoint = gesture.Position;
                }
                else
                {
                    _touchPoint = Vector2.Zero;
                }

            }


            //TouchCollection touches = TouchPanel.GetState();
            //if (_virus.Ammo > 0 && touches.Count > 0 && touches[0].State == TouchLocationState.Pressed)
            //{
            //    // convert the touch position into a Point for hit testing
            //    _touchPoint = new Vector2(touches[0].Position.X, touches[0].Position.Y);
            //    _virus.Ammo--;
            //}
            //else
            //{
            //    _touchPoint = Vector2.Zero;
            //}

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
            }
        }



        private void GetUserTouch()
        {
            // we use raw touch points for selection, since they are more appropriate
            // for that use than gestures. so we need to get that raw touch data

            // see if we have a new primary point down. when the first touch
            // goes down, we do hit detection to try and select one of our enemies
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

                    // send bomHit event to each enemy
                    _enemies.ForEach(wg => wg.AddBodyEvent(new BodyEvent(BodyEventCode.bombHit)));
                    SoundManager.Play("virus-bomb");

                    // send bombHit event to each bonus, 
                    foreach (GoToVirusBonus b in _bonuses)
                    {
                        b.AddBodyEvent(new BodyEvent(BodyEventCode.bombHit));
                        if (b.Type == BonusType.ammo && b.State == BonusState.moving)
                            recreateAmmoBonus = true;
                    }

                    // send bombHit event to the boss
                    if (_bossContainer.Count != 0)
                    {
                        _bossContainer[0].AddBodyEvent(new BodyEvent(BodyEventCode.bombHit));
                    }

                    // make the screen tremble
                    CustomTimeVariable trembleAmplitude = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 60), new Vector2(1, 60), new Vector2(1.25f, 0) });
                    CustomTimeVariable trembleAmplitude2 = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 30), new Vector2(1, 30), new Vector2(1.25f, 0) });
                    _mainbackground.Tremble(2, trembleAmplitude, 30, (float)Math.PI, false);
                    _firstPlanBackground.Tremble(2, trembleAmplitude2, 60, (float)Math.PI, false);
                }
            }

            // iterate our enemied sprites to find which sprite is being touched.
            foreach (Enemy e in _enemies)
            {
                if (e.Touched(_touchPoint))
                {
                    e.AddBodyEvent(new BodyEvent(BodyEventCode.fingerHit));
                    enemiesKilled++;
                }
            }

            // iterate boss hittable sprites to find which sprite is being touched
            if (_bossContainer.Count != 0)
            {
                _bossContainer[0].HandleUserTouch(_touchPoint, ref enemiesKilled);
            }

            if (enemiesKilled > 0)
            {
                SoundManager.Play("hit");
            }
            else
            {
                SoundManager.Play("miss");
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
                    b.AddBodyEvent(new BodyEvent((int)BodyEventCode.fingerHit));
                    SoundManager.Play("powerup-hit");

                    if (b.Type == BonusType.ammo)
                        recreateAmmoBonus = true;
                }
            }

            if (recreateAmmoBonus)
            {
                _eventsManager.ScheduleEvent(new GameEvent(TimeSpan.FromSeconds(1), GameEventType.createAmmoBonus, _bonusFactory));
            }

        }

        #endregion

        #region collisions detections

        private void DetectGlobulosBorderCollision()
        {
            foreach (Enemy e in _enemies)
            {
                if (CollisionDetector.CollisionWithBorder(e.Position, e.Angle, e.Speed, e.Shape, BorderType.rightPortrait))
                {
                     e.AddBodyEvent(new BodyEvent(BodyEventCode.borderCollision, new Object[] { new Vector2(-1, 0) }));
                     continue;
                }
                else if(CollisionDetector.CollisionWithBorder(e.Position, e.Angle, e.Speed, e.Shape, BorderType.left))
                {
                    e.AddBodyEvent(new BodyEvent(BodyEventCode.borderCollision, new Object[] { new Vector2(1, 0) }));
                        continue;
                }
                else if(CollisionDetector.CollisionWithBorder(e.Position, e.Angle, e.Speed, e.Shape, BorderType.bottomPortrait))
                {
                     e.AddBodyEvent(new BodyEvent(BodyEventCode.borderCollision, new Object[] { new Vector2(0, -1) }));
                        continue;
                }
                else if(CollisionDetector.CollisionWithBorder(e.Position, e.Angle, e.Speed, e.Shape, BorderType.top))
                {
                     e.AddBodyEvent(new BodyEvent(BodyEventCode.borderCollision, new Object[] { new Vector2(0, 1) }));
                     continue;
                }
            }
        }

        private void DetectVirusCollision()
        {
            if (_virus != null)
            {
                foreach (Enemy e in _enemies)
                {
                    if (e.Moving &&
                        CollisionDetector.CircularBodyGeneralBodyCollision(_virus.Position, _virus.Shape.GetShapeSize().X, e.Position, e.Angle, e.Shape))
                    {
                        // collision occurred, calculate angle of impact
                        float collisionAngle = (float)Math.Atan2(e.Position.Y - _virus.Position.Y, e.Position.X - _virus.Position.X);

                        _virus.AddBodyEvent(new BodyEvent(BodyEventCode.virusGlobuloCollision, new Object[] { collisionAngle }));
                        e.AddBodyEvent(new BodyEvent(BodyEventCode.virusGlobuloCollision));
                    }
                }

                foreach (GoToVirusBonus b in _bonuses)
                {
                    if ((b.State == BonusState.moving) &&
                        CollisionDetector.CirularBodyCollision(_virus.Position, _virus.Shape.GetShapeSize().X, b.Position, b.Shape.GetShapeSize().X))
                    {
                        BonusType param = b.Type;

                        _virus.AddBodyEvent(new BodyEvent(BodyEventCode.virusBonusCollision, new Object[] { param }));
                        b.AddBodyEvent(new BodyEvent(BodyEventCode.virusBonusCollision));
                    }
                }
            }
        }

        #endregion

        private void HandleBossSpecialMove()
        {
            // see if boss has delivered the special move
            if (_bossContainer[0].SpecialMoveHit)
            {
                if (_virus != null)
                {
                    _virus.AddBodyEvent(new BodyEvent(BodyEventCode.virusGlobuloCollision, new Object[] { (float)(Math.PI / 2) }));
                    _bossContainer[0].SpecialMoveHit = false;
                }
            }
        }

        private void ClearOutOfBoundOrDeadEnemies()
        {
            // destroy out of screens enemies or dead enemies
            int iterations = _enemies.Count;
            for (int i = 0, j = 0; i < iterations; i++, j++)
            {
                if (_enemies[j].Died ||
                    _enemies[j].Position.X < -250 || _enemies[j].Position.X > 730 ||
                    _enemies[j].Position.Y < -250 || _enemies[j].Position.Y > 1050)
                {
                    _enemies.RemoveAt(j);
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

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            // handle user input
            _touchPoint = Vector2.Zero;
            if (_virus != null)
                GetUserTouch();

            // detect touch collisions
            if (_touchPoint != Vector2.Zero)
                DetectTouchCollisions();

            // detect collisions between our friend virus and evil globulos, and bonuses
            DetectVirusCollision();

            if (_bossContainer.Count != 0)
                HandleBossSpecialMove();

            DetectGlobulosBorderCollision();

            // scroll the background
            _mainbackground.Update(gameTime.ElapsedGameTime);
            _firstPlanBackground.Update(gameTime.ElapsedGameTime);

            // update the enemies
            _enemies.ForEach(wg => wg.Update(gameTime.ElapsedGameTime));

            // update the bonuses
            _bonuses.ForEach(b => b.Update(gameTime.ElapsedGameTime));

            // udpate boss
            if (_bossContainer.Count != 0)
                _bossContainer[0].Update(gameTime.ElapsedGameTime);

            // animate our friend virus
            if (_virus != null)
                _virus.Update(gameTime.ElapsedGameTime);

            // clear enemies
            ClearOutOfBoundOrDeadEnemies();

            // clear bonuses
            ClearBonuses();

            // clear virus  :-( DEVO FINIRE IL LIVELLO ANCHE SE MUORE VIRUS, NEL QUAL CASO PERO' DEVO FARE IL ROLLBACK DI TUTTO (METEODO PER FARE IL ROLLBACK DEL LIVELLO!!!)
            if (_virus != null && _virus.State == ViruState.died)
                _virus = null;

            if (_bossContainer.Count != 0 && _bossContainer[0].Died)
            {
                _bossContainer.RemoveAt(0);
                _virus.AddBodyEvent(new BodyEvent(BodyEventCode.go));
                _bossDead = true;
                //_bossDeath = (float)gameTime.TotalSeconds;
            }

            //if (_bossDead && (gameTime.TotalSeconds - _bossDeath > 5))
            if (_virus != null && _virus.Position.Y < -50f)
            {
                State = LevelState.finished;
            }

            // manage current event (if any)
            _eventsManager.ManageCurrentEvent(gameTime.TotalGameTime);    // total time
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // draw background
            _mainbackground.Draw(spriteBatch);
            _firstPlanBackground.Draw(spriteBatch);

            // draw boss
            if (_bossContainer.Count != 0)
                _bossContainer[0].Draw(spriteBatch);

            // draw bonuses
            _bonuses.ForEach(b => b.Draw(spriteBatch));

            // draw enemies
            _enemies.ForEach(e => e.Draw(spriteBatch));

            // draw our friend virus
            if (_virus != null)
                _virus.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
