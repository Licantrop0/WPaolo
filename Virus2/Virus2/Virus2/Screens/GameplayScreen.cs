#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Virus
{
    public enum ProtectionBarrierState
    {
        waitForTouch,
        trackTouching,
        released
    }

    public class ProtectionBarrierGesture
    {
        public bool IsAccomplished;
        public ProtectionBarrierState State;
        public int TouchId;
        public Vector2 StartPosition;
        public Vector2 EndPosition;

        public ProtectionBarrierGesture()
        {
            IsAccomplished = false;
            State = ProtectionBarrierState.waitForTouch;
            TouchId = -1;
        }

        public void Reset()
        {
            IsAccomplished = false;
            State = ProtectionBarrierState.waitForTouch;
            TouchId = -1;
        }

        public void Update(TouchCollection touchState)
        {
            switch (State)
            {
                case ProtectionBarrierState.waitForTouch:

                    // look for single user press
                    if (touchState.Count == 1 && touchState[0].State == TouchLocationState.Pressed)
                    {
                        TouchId = touchState[0].Id;
                        StartPosition = touchState[0].Position;
                        State = ProtectionBarrierState.trackTouching;
                    }

                    break;

                case ProtectionBarrierState.trackTouching:

                    // keep track of touch state id
                    if (touchState.Count == 1 && touchState[0].Id == TouchId) 
                    {
                        // touch state can be moved or release, if it is moved we keep waiting,
                        // if it is release we check actual position, if it is far enough from
                        // start position we accomplished the gesture
                        if (touchState[0].State == TouchLocationState.Released)
                        {
                            Vector2 releasePosition = touchState[0].Position;
                            if (Vector2.Distance(StartPosition, releasePosition) > 75)
                            {
                                IsAccomplished = true;
                                EndPosition = releasePosition;
                                State = ProtectionBarrierState.released;
                            }
                        }
                    }
                    else
                    {
                        Reset();
                    }

                    break;

                case ProtectionBarrierState.released:

                    Reset();

                    break;
            }
        }

        public bool Triggered (ref Vector2 startPosition, ref Vector2 endPosition)
        {
            if (IsAccomplished)
            {
                startPosition = StartPosition;
                endPosition = EndPosition;
            }

            return IsAccomplished;
        }
    }

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;

        float pauseAlpha;

        InputAction pauseAction;

        VirusLevel level;

        // input logic (it is passed to level logic)
        // tap
        bool tapped;          
        Vector2 tapPosition;

        // protection barrier
        bool isProtectionBarrierTriggered;
        Vector2 protectionBarrierStartPosition;
        Vector2 protectionBarrierEndPosition;

        // protection barrier detection
        ProtectionBarrierGesture protectionBarrier = new ProtectionBarrierGesture();

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            EnabledGestures = GestureType.Tap;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");  // VirusContent ?

                level = new VirusLevel(GameGlobalState.ActualLevel, ScreenManager.SpriteBatch, content);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();      // PS che succede in caso di resume per il monster e bonus generator ?
            }

            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                /*playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];*/

                // PS TODO: qua devo mettere il salvataggio di tutto lo stato (posizione nel livello, stato eventi, stato e posizione
                // mostri, dovrò implementare nella classe body il metodo Resume() che faccia riprendere tutti dallo stato in cui
                // erano
            }
        }


        public override void Deactivate()
        {
           /* Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;*/

            // PS TODO salvare lo stato del livello
            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                level.Update(gameTime, tapped, tapPosition, isProtectionBarrierTriggered, protectionBarrierStartPosition, protectionBarrierEndPosition);

                if (level.State == LevelState.lostAndStopped)
                {
                    ScreenManager.AddScreen(new PhoneRetryScreen(level), ControllingPlayer);
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
            }
            else
            {
                // PS handle tap gesture
                bool auxTouch = false;
                foreach (GestureSample gesture in input.Gestures)
                {
                    // If we have a tap
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        tapped = true;
                        auxTouch = true;
                        tapPosition = gesture.Position;
                        break;
                    }
                }
                if (auxTouch == false)
                {
                    tapped = false;
                }

                // PS hanlde protection barrier gesture 
                protectionBarrier.Update(input.TouchState);
                isProtectionBarrierTriggered = protectionBarrier.Triggered(ref protectionBarrierStartPosition, ref protectionBarrierEndPosition);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            level.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
