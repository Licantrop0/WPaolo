using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Virus
{
    public class SoundEffectProxy
    {
        SoundEffect _soundEffect;
        List<SoundEffectInstance> _soundFXIstances = new List<SoundEffectInstance>();

        public SoundEffectProxy(SoundEffect soundEffect)
        {
            _soundEffect = soundEffect;
            _soundFXIstances.Add(_soundEffect.CreateInstance());
        }

        public void Play()
        {
            int i;
            for (i = 0; i < _soundFXIstances.Count; i++)
            {
                if (_soundFXIstances[i].State == SoundState.Stopped)
                {
                    _soundFXIstances[i].Play();
                    return;
                }
            }

            _soundFXIstances.Add(_soundEffect.CreateInstance());
            _soundFXIstances[i].Play();
        }

        public void Stop()
        {
            _soundFXIstances[0].Stop();
        }
    }

    public static class SoundManager
    {
        private static Dictionary<string, SoundEffectProxy> _soundFXs = new Dictionary<string, SoundEffectProxy>();

        public static void Load(string soundName, SoundEffect soundEffect)
        {
            _soundFXs.Add(soundName, new SoundEffectProxy(soundEffect));
        }

        public static void Play(string soundName)
        {
            _soundFXs[soundName].Play();
        }

        public static void Stop(string soundName)
        {
            _soundFXs[soundName].Stop();
        }
    }
}
