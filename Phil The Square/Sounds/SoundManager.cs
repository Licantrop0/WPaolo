using System;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Windows;
using FillTheSquare.Localization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using FillTheSquare.ViewModel;

namespace FillTheSquare.Sounds
{
    public static class SoundManager
    {
        private static SoundEffect _moveSound;
        public static void PlayMove()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_moveSound == null)
                _moveSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\Move.wav", UriKind.Relative)).Stream);

            _moveSound.Play();
        }

        private static SoundEffect _errorSound;
        public static void PlayError()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_errorSound == null)
                _errorSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\Error.wav", UriKind.Relative)).Stream);

            _errorSound.Play();
        }

        private static SoundEffect _undoSound;
        public static void PlayUndo()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_undoSound == null)
                _undoSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\Undo.wav", UriKind.Relative)).Stream);

            _undoSound.Play();
        }

        private static SoundEffect _victorySound;
        public static void PlayVictory()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_victorySound == null)
                _victorySound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\Victory.wav", UriKind.Relative)).Stream);

            _victorySound.Play();
        }

        private static SoundEffect _resetSound;
        public static void PlayReset()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_resetSound == null)
                _resetSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\Reset.wav", UriKind.Relative)).Stream);
            _resetSound.Play();
        }

        private static SoundEffect _startSound;
        public static void PlayStart()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_startSound == null)
                _startSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\start.wav", UriKind.Relative)).Stream);

            _startSound.Play();
        }

        private static SoundEffect _ohNoSound;
        public static void PlayOhNo()
        {
            if (!SettingsViewModel.Instance.SoundEffectsEnabled)
                return;

            if (_ohNoSound == null)
                _ohNoSound = SoundEffect.FromStream(App.GetResourceStream(
                    new Uri("Sounds\\OhNo.wav", UriKind.Relative)).Stream);

            _ohNoSound.Play();
        }
    }
}