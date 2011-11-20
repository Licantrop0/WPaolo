using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VirusLib;


namespace Virus
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager { get { return  (Application.Current as App).Content;} }
        public AppServiceProvider AppServices { get { return (Application.Current as App).Services; } }

        GameTimer timer;
        SpriteBatch spriteBatch;

        // writings
        SpriteFont _bombString;
        SpriteFont _delayString;
        SpriteFont _timeString;

        // virus
        VirusLib.Virus _virus;
        Texture2D _virusLifeTexture;

        // ammo bar
        AmmoBar _ammoBar;

        // levels
        Level _currentLevel;


        public GamePage()
        {
            InitializeComponent();

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromSeconds(1 / 30);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // create score and debug fonts and ammo bar
            _bombString = contentManager.Load<SpriteFont>("Segoe20");
            _delayString = contentManager.Load<SpriteFont>("Segoe20");
            _timeString = contentManager.Load<SpriteFont>("Segoe20");
            _ammoBar = new AmmoBar(contentManager.Load<Texture2D>("ammobar"));

            // create our friend virus
            Texture2D virusTexture = contentManager.Load<Texture2D>("Sprites/Virus/virusMedium");
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new SpriteSheetAnimation(7, true, virusTexture));
            
            _virus = new VirusLib.Virus(
                new MassDoubleIntegratorDynamicSystem(),
                new Sprite(virusAnimations),
                new CircularShape(40, 40));

            // create life virus
            _virusLifeTexture = contentManager.Load<Texture2D>("virusLifeLittle");

            // fa il load content del primo livello -> va fatto una volta scelto il livello...
            _currentLevel = new Level(AppServices, spriteBatch, 1, _virus);
            _currentLevel.InitializeLevel(1);

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // faccio l'update del livello corrente
            _currentLevel.Update(e);

            // update ammobar
            if (_virus != null)
            {
                _ammoBar.Update(_virus.Ammo);
            }
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            // disegna il livello corrente
            _currentLevel.Draw(e.ElapsedTime);

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
            
            spriteBatch.DrawString(_timeString, Math.Round(e.TotalTime.TotalSeconds, 0).ToString(), new Vector2(410, 768), Color.Yellow);

            spriteBatch.End();
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
    }
}