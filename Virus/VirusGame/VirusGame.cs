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
using OpenXLive;
using OpenXLive.Forms;
using OpenXLive.Controls;

namespace Virus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VirusGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Open Xlive manager
        private string APISecretKey = "sgTU4GJkRcbKFM3XuaqjtN5V";
        XLiveFormManager manager;

        // writings
        SpriteFont _bombString;
        SpriteFont _delayString;
        SpriteFont _timeString;

        // virus
        Virus _virus;
        Texture2D _virusLifeTexture;

        // ammo bar
        AmmoBar _ammoBar;

        // levels
        Level _currentLevel;

        // prophiling
        int _delayCount = 0;

        public VirusGame()
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
            // Create XLive FormManager
            manager = new XLiveFormManager(this, APISecretKey);
            manager.OpenSession();

            // PS create custom form and asscociate it with NewGame event
            //manager.NewGameEvent += new EventHandler(manager_NewGameEvent);
            //manager.

            // Add XLive FormManager in Components
            Components.Add(manager);

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

            // Load OpenXLive stuff
            //Texture2D background = this.Content.Load<Texture2D>("OpenXLive");
            Texture2D background = this.Content.Load<Texture2D>("MenuBackground");
            XLiveStartupForm form = new XLiveStartupForm(this.manager);

            form.Background = background;
            form.AnimationIntervalSeconds = 0;
            form.AnimationSpeed = 5;
            form.BackgroundAnimation = true;
            form.Show();

            // create score and debug fonts and ammo bar
            _bombString = Content.Load<SpriteFont>("Segoe20");
            _delayString = Content.Load<SpriteFont>("Segoe20");
            _timeString = Content.Load<SpriteFont>("Segoe20");
            _ammoBar = new AmmoBar(Content.Load<Texture2D>("ammobar"));

            // create our friend virus
            Texture2D virusTexture = Content.Load<Texture2D>("Sprites/Virus/virusMedium");
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new SpriteSheetAnimation(7, true, virusTexture));
            _virus = new Virus(new MassDoubleIntegratorDynamicSystem(),
                               new Sprite(virusAnimations),
                               new CircularShape(40, 40));

            // create life virus
            _virusLifeTexture = Content.Load<Texture2D>("virusLifeLittle");

            // fa il load content del primo livello -> va fatto una volta scelto il livello...
            _currentLevel = new Level(this, graphics, spriteBatch, 1, _virus);
            _currentLevel.InitializeLevel(1);
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
            {
                if (manager.IsRunning)
                {
                    XLivePauseForm form = new XLivePauseForm(this.manager);
                    form.Show();
                }
            }

            if (manager.IsRunning)
            {
                // TODO: Add your update logic here
                //manager.
                if (_currentLevel.State == LevelState.running)
                {
                    // faccio l'update del livello corrente
                    _currentLevel.Update(gameTime);

                    // update ammobar
                    if (_virus != null)
                    {
                        _ammoBar.Update(_virus.Ammo);
                    }
                }
                else
                {
                    XLivePauseForm form = new XLivePauseForm(this.manager);
                    form.Show();
                }
            }
            else
            {
                manager.ActiveForm.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (manager.IsRunning)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                // disegna il livello corrente
                _currentLevel.Draw(gameTime);

                spriteBatch.Begin();

                // draw lifes and ammobar
                if (_virus != null)
                {
                    DrawLifes(spriteBatch);
                }

                // VEDO IL CONTORNO DOVE DISEGNARLO!!! SE NEL LIVELLO O FUORI!!!
                // write score
                if (_virus != null)
                {
                    spriteBatch.DrawString(_bombString, _virus.Bombs.ToString(), new Vector2(30, 4), Color.Yellow);
                    _ammoBar.Draw(spriteBatch);
                }

                if (gameTime.IsRunningSlowly)
                    _delayCount++;

                spriteBatch.DrawString(_delayString, _delayCount.ToString(), new Vector2(30, 768), Color.Yellow);
                spriteBatch.DrawString(_timeString, Math.Round(gameTime.TotalGameTime.TotalSeconds, 0).ToString(), new Vector2(410, 768), Color.Yellow);

                spriteBatch.End();
            }
            else
            {
                manager.ActiveForm.Draw(gameTime);
            }


            base.Draw(gameTime);
        }

        private void DrawLifes(SpriteBatch spriteBatch)
        {
            Vector2 position = new Vector2(450, 8);
            for (int i = 0; i < _virus.Lifes; i++)
            {
                spriteBatch.Draw(_virusLifeTexture, position, Color.White);
                position -= new Vector2(20, 0);
            }
        }

        void manager_NewGameEvent(object sender, EventArgs e)
        {
            //XLiveAboutForm form = new XLiveAboutForm(manager);
            //form.Show();

            XLiveLevelChoiceForm form = new XLiveLevelChoiceForm(manager);
            form.AddControl(new XLiveTextBox(form));
            form.Show();
        }  
    }
}
