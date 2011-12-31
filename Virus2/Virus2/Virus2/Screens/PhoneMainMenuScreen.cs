#region File Description
//-----------------------------------------------------------------------------
// PhoneMainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Virus
{
    class PhoneMainMenuScreen : PhoneMenuScreen
    {
        public PhoneMainMenuScreen()
            : base("Main Menu")
        {
            // Create a button to start the game
            Button playButton = new Button("Play");
            playButton.Tapped += playButton_Tapped;
            MenuButtons.Add(playButton);

            // Create two buttons to toggle sound effects and music. This sample just shows one way
            // of making and using these buttons; it doesn't actually have sound effects or music
            BooleanButton sfxButton = new BooleanButton("Sound Effects", true);
            sfxButton.Tapped += sfxButton_Tapped;
            MenuButtons.Add(sfxButton);

            BooleanButton musicButton = new BooleanButton("Music", true);
            musicButton.Tapped += musicButton_Tapped;
            MenuButtons.Add(musicButton);
        }

        void playButton_Tapped(object sender, EventArgs e)
        {
            // TODO: il livello dipenderà dal pulsante, per ora lo cablo a 1
            GameGlobalState.ActualLevel = 1;

            // PS TODO: spostare l'inizializzazione di Virus, andrebbe nella classe game penso, quella che viene passata allo
            // Screen Manager, oppure vediamo...
            Texture2D virusTexture = ScreenManager.Game.Content.Load<Texture2D>("Sprites/Virus/virusMedium");
            
            Dictionary<string, Animation> virusAnimations = new Dictionary<string, Animation>();
            virusAnimations.Add("main", new SpriteSheetAnimation(7, true, virusTexture));

            GameGlobalState.Virus = new Virus(
                new MassDoubleIntegratorDynamicSystem(),
                new Sprite(virusAnimations),
                new CircularShape(40, 40));

            // When the "Play" button is tapped, we load the GameplayScreen
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen());
        }

        void sfxButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off sounds here. :)
        }

        void musicButton_Tapped(object sender, EventArgs e)
        {
            BooleanButton button = sender as BooleanButton;

            // In a real game, you'd want to store away the value of 
            // the button to turn off music here. :)
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
            base.OnCancel();
        }
    }
}
