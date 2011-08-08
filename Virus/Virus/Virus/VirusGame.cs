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
    public class VirusGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //ContentManager[] levelsContentManagers;

        // writings
        SpriteFont _bombString;
        SpriteFont _delayString;
        SpriteFont _timeString;

        // monstres and bonuses factory
        MonsterFactory _monsterFactory;
        BonusFactory _bonusFactory;

        // virus
        Virus _virus;

        // ammo bar
        AmmoBar _ammoBar;

        // levels
        List<Level> _levels = new List<Level>(3);

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
            //_enemiesKilledByAmmoTriggerNumber = 50;

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

            // fa il load content del primo livello
            _levels.Add(new Level(this, graphics, spriteBatch, 1, _virus, _ammoBar));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            // fa l'update del livello corrente
            _levels[0].Update(gameTime);
          
            base.Update(gameTime);
        }
               
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // disegna il livello corrente
            _levels[0].Draw(gameTime);

            spriteBatch.Begin();

            // draw lifes and ammobar
            if (_virus != null)
            {
                DrawLifes(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Texture2D _virusLifeTexture { get; set; }
    }
}
