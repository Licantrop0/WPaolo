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
using System.Diagnostics;

namespace Virus
{
    public enum LevelState
    {
        none,
        loading,
        running,
        lost,
        lostAndStopped,
        finished
    }

    public class VirusLevel
    {
        // graphics manager and sprite batch (references to game graphics)
       // GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // content manager
        ContentManager contentManager;

        // used for debug purpouses
        Texture2D _debugSquare;
        Vector2 _debugPos1;
        Vector2 _debugPos2;
        bool _drawDebug;
        int _debugCounter = 0;

        // condition of level finished
        //public bool Finished { get; set; }
        public LevelState State { get; set; }
        float _levelTimer = 0;
        float _stopTimer = 0;
        
        MonsterGenerator _monsterGenerator;
        BonusGenerator _bonusGenerator;

        // event scheduler
        GameEventsManager _eventsManager = new GameEventsManager();

        // enemies
        List<Enemy> _enemies = new List<Enemy>();

        // bonuses
        List<GoToVirusBonus> _bonuses = new List<GoToVirusBonus>();

        // boss
        List<Boss> _bossContainer = new List<Boss>();

        // virus
        Virus _virus;
        Texture2D _virusLifeTexture;

        // writings and indicators
        SpriteFont _infoString;
        AmmoBar _ammoBar;
      
        // background
        MovingBackground _mainbackground;
        MovingBackground _firstPlanBackground;

        // difficulty parameters
        LevelDifficultyPack[] _levelDifficultyPack;

        // virus ammo management
        //int _enemiesKilledByAmmoCounter;
        int _enemiesKilledByAmmoTriggerNumber;
        //int _ammoQuantityPerBonus;

        #region constructor

        public VirusLevel(int level, SpriteBatch spriteBatch, ContentManager contentManager)
        {
            this.spriteBatch = spriteBatch;
            this.contentManager = contentManager;

            State = LevelState.loading;

            LoadLevel(level);
            InitializeLevel(level, false);
        }

        #endregion

        #region LoadContent

        private void LoadLevel(int level)
        {
            CreateVirus();
            CreateBackground(level);
            CreateMonsterGenerator(level);
            CreateBonusGenerator(level);
            LoadSounds(level);
            CreateDifficulty(level);

            // debug
            _debugSquare = contentManager.Load<Texture2D>("debugSpot");
        }

        private void CreateVirus()
        {
            Texture2D virusTexture = contentManager.Load<Texture2D>("Sprites/Virus/virusMedium");

            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new SpriteSheetAnimation(7, true, virusTexture));

            _virus = new Virus(
                new MassDoubleIntegratorDynamicSystem(),
                new Sprite(virusAnimations),
                new CircularShape(40, 40))
            {
                Ammo = 70,
                Bombs = 5,
                Lifes = GameGlobalState.VirusLives
            };

            // create life virus
            _virusLifeTexture = contentManager.Load<Texture2D>("virusLifeLittle");

            // create virus info (ammobar, lives and bombs indicator)
            _infoString = contentManager.Load<SpriteFont>("Segoe20");
            _ammoBar = new AmmoBar(contentManager.Load<Texture2D>("ammobar"));

        }

        private void CreateBackground(int level)
        {
            switch (level)
            {
                case 1:
                    // main background
                    //Texture2D backgroundTexture0 = contentManager.Load<Texture2D>("NuoviSfondi\\SfondoCarne (bora)");
                    Texture2D backgroundTexture0 = contentManager.Load<Texture2D>("SfondoCarne");
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

        private void CreateBonusGenerator(int level)
        {
            switch (level)
            {
                case 1:

                    // create Bonus Factory
                    _bonusGenerator = new BonusGenerator(_eventsManager, _virus,
                                                        _bonuses, new SpritePrototypeContainer(contentManager, "AnimationConfig/Bonuses.xml"));

                    _bonusGenerator.SetBombBonusTimePeriod(30, 50);

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void CreateMonsterGenerator(int level)
        {
            switch (level)
            {
                case 1:

                    // create Monster Factory
                    _monsterGenerator = new MonsterGenerator(_eventsManager, _virus,
                                                            _enemies, _bossContainer, new SpritePrototypeContainer(contentManager, "AnimationConfig/Enemies.xml"));

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        private void LoadSounds(int level)
        {
            SoundManager.Reset();

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

        private void CreateDifficulty(int level)
        {
            switch (level)
            {
                case 1:

                    _levelDifficultyPack = new Level1DifficultyPackEnemies[3];

                    _levelDifficultyPack[0] = new Level1DifficultyPackEnemies(2, 2.5f, 0.1f, 1, 2.4f, 3.2f, 2, 3);

                    _levelDifficultyPack[1] = new Level1DifficultyPackEnemies(1.75f, 2.5f, 0.1f, 1, 2.1f, 3.2f, 2, 5);

                    _levelDifficultyPack[2] = new Level1DifficultyPackEnemies(2, 2.5f, 0.1f, 1, 1.9f, 3.2f, 3, 4);

                    break;

                default:
                    throw new ArgumentException("Level not valid", "level");
            }
        }

        #endregion

        #region Initialize

        public void InitializeLevel(int level, bool keepScoring)
        {
            _eventsManager.ClearAllEvents();
            _mainbackground.Reset(30);
            _firstPlanBackground.Reset(60);
            _virus.Touchable = true;
            _virus.Position = new Vector2(240, 400);
            _virus.Lifes = 5;
            _virus.Ammo = 70;
            _virus.Bombs = 5;
            Statistics.Reset();
            if (!keepScoring)
                _virus.Score = 0;
            _virus.State = ViruState.tranquil;
            _bonuses.Clear();
            _enemies.Clear();
            _bossContainer.Clear();

            ScheduleFixedEvents(level);

            _enemiesKilledByAmmoTriggerNumber = 50;
            State = LevelState.running;
            _levelTimer = 0;

            GC.Collect();
        }

        private void ScheduleFixedEvents(int level)
        {
            //_eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleMassEnemyCycleCreation, _monsterGenerator), 5);
            //_eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleMassEnemyAllTogetherCreation, _monsterGenerator), 15);
            //_eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleMassEnemyAllTogetherCreation, _monsterGenerator), 16.0f);

            // set difficulty
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.changeLevel1Difficulty, _monsterGenerator, new Object[] { _levelDifficultyPack[0] }), 0);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.changeLevel1Difficulty, _monsterGenerator, new Object[] { _levelDifficultyPack[1] }), 30);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.changeLevel1Difficulty, _monsterGenerator, new Object[] { _levelDifficultyPack[2] }), 100);

            // initialize bonus speed
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.changeBonusSpeed, _bonusGenerator, new object[] { 50 }), 0);

            // create bonus points (time periods reflect difficulty changes)
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 5.0f, 15.0f }), 0);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 5.0f, 15.0f }), 15);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 4.0f, 35.0f }), 30);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 4.0f, 35.0f }), 65);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 3.0f, 20.0f }), 100);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBonusPointsDistribution, _bonusGenerator, new object[] { 3.0f, 20.0f }), 120);

            // schedule and go on scheduling white globulos creation
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleSimpleEnemyCreation, _monsterGenerator), 3);

            // create bonusbomb every 30/50 seconds (as set up before)
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.scheduleBombBonusCreation, _bonusGenerator), 1);

            // create bomb plus at 45 and 130 seconds
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.createBombPlusBonus, _bonusGenerator), 45);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.createBombPlusBonus, _bonusGenerator), 130);

            // create one up bonus at 135 seconds
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.createOneUpBonus, _bonusGenerator), 135);

            // boss arrives
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.createOneUpBonus, _bonusGenerator), 149);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.changeBonusSpeed, _bonusGenerator, new object[] { 180 }), 150);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.clearEvents, _monsterGenerator, new object[] { new GameEventType[] { GameEventType.createSimpleEnemy, GameEventType.scheduleSimpleEnemyCreation } }), 150);
            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.clearEvents, _bonusGenerator, new object[] { new GameEventType[] { GameEventType.scheduleBombBonusCreation } }), 150);

            _eventsManager.ScheduleEventAtTime(new GameEvent(GameEventType.createBossLung, _monsterGenerator), 153);
        }

        #endregion

        private void HandleBombExplosion()
        {
            // let the earth temble!
            SoundManager.Play("virus-bomb");
            //(TODO si fa un po' meglio on i behaviour anche per gli schermi)
            CustomTimeVariable trembleAmplitude = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 60), new Vector2(1, 60), new Vector2(1.25f, 0) });
            CustomTimeVariable trembleAmplitude2 = new CustomTimeVariable(new Vector2[4] { new Vector2(0, 0), new Vector2(0.25f, 30), new Vector2(1, 30), new Vector2(1.25f, 0) });
            _mainbackground.Tremble(2, trembleAmplitude, 30, (float)Math.PI, false);
            _firstPlanBackground.Tremble(2, trembleAmplitude2, 60, (float)Math.PI, false);

            // decrement virus bomb counter
            _virus.Bombs--;

            // send bomHit event to each enemy
            _enemies.ForEach(wg => wg.AddBodyEvent(new BodyEvent(BodyEventCode.bombHit)));

            // send bombHit event to each bonus, 
            foreach (GoToVirusBonus b in _bonuses)
            {
                b.AddBodyEvent(new BodyEvent(BodyEventCode.bombHit));
                if (b.Type == BonusType.ammo && b.State == BonusState.moving)
                {
                    _eventsManager.ScheduleEventNow(new GameEvent(GameEventType.createAmmoBonus, _bonusGenerator));
                }       
            }

            // send bombHit event to the boss
            if (_bossContainer.Count != 0)
            {
                _bossContainer[0].AddBodyEvent(new BodyEvent(BodyEventCode.bombHit));
            }
            
        }

        private void DetectTapCollisions(Vector2 tapPosition)
        {
            if (_virus.Ammo <= 0)
            {
                // TODO: play the sound out of ammo or display a warning, something...
                SoundManager.Play("miss");
                return;
            }

            bool hit = false;
            bool bonusHit = false;
            _virus.Ammo--;

            // iterate our enemied sprites to find which sprite is being touched
            foreach (Enemy e in _enemies)
            {
                if (e.Touched(tapPosition))
                {
                    e.AddBodyEvent(new BodyEvent(BodyEventCode.tap));
                    Statistics.Hit++;
                    hit = true;
                }
            }

            // iterate boss hittable sprites to find which sprite is being touched
            if (_bossContainer.Count != 0)
            {
                _bossContainer[0].HandleUserTap(tapPosition, ref hit);
            }

            // iterate our bonus sprites to find which sprite is being touched
            foreach (GoToVirusBonus b in _bonuses)
            {
                if (b.Touched(tapPosition))
                {
                    b.AddBodyEvent(new BodyEvent((int)BodyEventCode.tap));
                    hit = true;
                    SoundManager.Play("powerup-hit");
                    
                    if (b.Type == BonusType.ammo)
                    {
                        _eventsManager.ScheduleEventNow(new GameEvent(GameEventType.createAmmoBonus, _bonusGenerator));
                    }
                }
            }

            if (!hit && !bonusHit)
            {
                SoundManager.Play("miss");
            }
            else if (hit)
            {
                SoundManager.Play("hit");
            }
        }

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

        #endregion

        private void AmmoBonusHandling()
        {
            if (Statistics.Hit >= _enemiesKilledByAmmoTriggerNumber)
            {
                _enemiesKilledByAmmoTriggerNumber += 50;
                _eventsManager.ScheduleEventNow(new GameEvent(GameEventType.createAmmoBonus, _bonusGenerator));
            }
        }

        private void ClearOutOfBoundOrDeadEnemies()
        {
            // destroy out of screens enemies or dead enemies
            int iterations = _enemies.Count;
            for (int i = 0, j = 0; i < iterations; i++, j++)
            {
                if (_enemies[j].Died)
                {
                    _virus.Score += _enemies[j].WorthPoints;
                    _enemies.RemoveAt(j);
                    j--;
                }
                else if(_enemies[j].Position.X < -250 || _enemies[j].Position.X > 730 ||
                        _enemies[j].Position.Y < -250 || _enemies[j].Position.Y > 1050  ) 
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

        // PS per adesso taglio via altre gestures che si possono implementare in futuro, TODO definizione più generale
        public void Update(GameTime gameTime,
                           bool tapped, Vector2 tapPosition,
                           bool protectionGesture, Vector2 protectionStartPosition, Vector2 protectionEndPosition)
        {
            // update level timer and event manager clock
            _levelTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _eventsManager.Timer = _levelTimer;

            // temp, draw something in protection gesture position to debug the gesture!
            /*------------------------------------------------------------------------*/
            if (protectionGesture)
            {
                _debugPos1 = protectionStartPosition;
                _debugPos2 = protectionEndPosition;
                _drawDebug = true;
                _debugCounter++;
            }
            else
            {
                _drawDebug = false;
            }
            /*-------------------------------------------------------------------------*/

            AmmoBonusHandling();

            // manage current event (if any)
            _eventsManager.ManageCurrentEvent();

            // detect tap effects
            if (tapped)
            {
                // if virus has been tapped, a bomb explodes!
                if (_virus.Touched(tapPosition) && _virus.Bombs > 0)
                {
                    Statistics.BombsUsed++;
                    HandleBombExplosion();
                }
                else
                {
                    Statistics.Tap++;
                    DetectTapCollisions(tapPosition);
                }  
            }
                
            // detect collisions between our friend virus and evil globulos, and bonuses
            if (_virus.State != ViruState.dead)
                DetectVirusCollision();
            
            DetectGlobulosBorderCollision();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // scroll the background
            _mainbackground.Update(dt);
            _firstPlanBackground.Update(dt);

            // update the enemies
            _enemies.ForEach(wg => wg.Update(dt));

            // update the bonuses
            _bonuses.ForEach(b => b.Update(dt));

            // udpate boss
            if (_bossContainer.Count != 0)
                _bossContainer[0].Update(dt);

            // check boss death
            if (_bossContainer.Count != 0 && _bossContainer[0].Died)
            {
                _virus.Score += _bossContainer[0].WorthPoints;
                _bossContainer.RemoveAt(0);
                _virus.AddBodyEvent(new BodyEvent(BodyEventCode.go));
            }

            // update our friend virus
            _virus.Update(dt);

            // update writings and indicators
            if (_virus.State != ViruState.dead)
                _ammoBar.Update(_virus.Ammo);

            // clear enemies
            ClearOutOfBoundOrDeadEnemies();

            // clear bonuses
            ClearBonuses();

            // switch state conditions
            // virus id dead :-(
            if (State == LevelState.running && _virus.State == ViruState.dead)
            {
                State = LevelState.lost;
                _eventsManager.ClearAllEvents();
                _stopTimer = 0;
            }
            // finish level condition: virus is out of screen after epic movement
            else if (_virus.Position.Y < -50f)
            {
                State = LevelState.finished;
            }
            // handle transition between lost and stopped, when the timer expires
            // state is switched to lostAndStopped, GamePlayScreen will load the retry pop up
            else if (State == LevelState.lost)
            {
                _stopTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_stopTimer >= 2.5f)
                    State = LevelState.lostAndStopped;
            } 
        }

        public void Draw(GameTime gameTime)
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

            // draw our friend virus and its info
            if (_virus.State != ViruState.dead)
            {
                _virus.Draw(spriteBatch);
                spriteBatch.Draw(_virusLifeTexture, new Vector2(10, 15), Color.White);
                spriteBatch.DrawString(_infoString, "x" + _virus.Lifes.ToString(), new Vector2(35, 4), Color.White);
                spriteBatch.DrawString(_infoString, _virus.Bombs.ToString(), new Vector2(75, 4), Color.White);
                // in realtà nei giochi viene allineato a dx il punteggio, quindi bisognerà prevedere un qualche tipo di logica
                spriteBatch.DrawString(_infoString, _virus.Score.ToString(), new Vector2(380, 4), Color.White);
                spriteBatch.DrawString(_infoString, _debugCounter.ToString(), new Vector2(20, 700), Color.Yellow);
                spriteBatch.DrawString(_infoString, Statistics.Hit.ToString(), new Vector2(430, 680), Color.Yellow);
                spriteBatch.DrawString(_infoString, Statistics.Tap.ToString(), new Vector2(430, 700), Color.Yellow);

                _ammoBar.Draw(spriteBatch);  
            }

            if (_drawDebug)
            {
                spriteBatch.Draw(_debugSquare, _debugPos1, Color.White);
                spriteBatch.Draw(_debugSquare, _debugPos2, Color.White);
            }

            spriteBatch.End();
        }
    }
}
