using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace SME.Audio
{
    public sealed class AudioManager
    {
        private class SoundEffectPlayer
        {
            public SoundEffectInstance soundEffect;
            public float timer = 0f;
            public Vector2 position;
            public bool toRemove = false;
            public Sprite sprite;
            public string name;
            private float duration, maxVolume, originalVolume;
            private static float Io = 0.000000000001f;
            private bool loop, useStereo;

            public SoundEffectPlayer(SoundEffectInstance soundEffect, in Vector2 position, Sprite sprite, in float duration, in float originalVolume, in bool loop, string name, in bool useStereo)
            {
                this.soundEffect = soundEffect;
                this.position = position;
                this.sprite = sprite;
                this.duration = duration;
                this.originalVolume = originalVolume;
                SetVolume();
                this.name = name;
                this.loop = loop;
                this.useStereo = useStereo;
            }

            public void SetVolume()
            {
                this.maxVolume = originalVolume * effectsVolume;
            }

            public void Update()
            {
                if(sprite != null)
                {
                    position = sprite.position;
                }
                if(soundEffect.State == SoundState.Playing)
                {
                    timer += Time.dt;
                    if (timer > duration)
                    {
                        toRemove = true;
                    }
                }
                float d = Vector2.Distance(position, mainLisener.position) / oneMeterEquivalent;//en mètre
                float I = 0.000000015f / (d * d);
                float L = 10f * MathF.Log(I / Io);

                soundEffect.Volume = Useful.Clamp(((L - 10f) / 90f), 0f, 1f) * maxVolume;
                //soundEffect.Volume = Useful.MarkOut(0f, 1f, ((L - 10f) / 90f) * maxVolume);
                //stereo
                if(useStereo)
                {
                    //cos(alpha) = 1 => pan = 1, cos(alpha) = -1 => pan = -1
                    float alpha = Useful.Angle(mainLisener.position, position);
                    soundEffect.Pan = MathF.Cos(alpha);
                }
            }
        }

        private static float _musicVolume = 1f;
        public static float musicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Useful.Clamp(value, 0f, 1f);
                if(currentMusic != null)
                    currentMusic.Volume = musicVolume;
            }
        }
        /// <summary>
        /// At a volume of 1, the sound is an equivalent to 100dB, at 0 it's a equivalent of 10 dB
        /// </summary>
        private static float _effectsVolume = 1f;
        public static float effectsVolume
        {
            get => _effectsVolume;
            set
            {
                _effectsVolume = Useful.Clamp(value, 0f, 1f);
                foreach (SoundEffectPlayer s in lstSoundEffects)
                {
                    s.SetVolume();
                }
            }
        }
        /// <summary>
        /// Describe the position and the rotation of lisener
        /// </summary>
        public static Sprite mainLisener { get; private set; }
        /// <summary>
        /// The number of pixel équivalent to one meter.
        /// </summary>
        public static float oneMeterEquivalent = 10f;
        public static bool useLisenerRotation = false;
        public static SoundEffectInstance currentMusic;
        private static List<SoundEffectPlayer> lstSoundEffects = new List<SoundEffectPlayer>();

        public static void PlayMusic(SoundEffect soundEffect, in float volume = 1f, in bool isLoop = true)
        {
            currentMusic = soundEffect.CreateInstance();
            currentMusic.Volume = musicVolume * volume;
            currentMusic.IsLooped = isLoop;
            currentMusic.Play();
        }

        public static void StopSoundEffects()
        {
            lstSoundEffects.Clear();
        }
        public static void PauseSoundEffects()
        {
            foreach (SoundEffectPlayer s in lstSoundEffects)
            {
                s.soundEffect.Pause();
            }
        }
        public static void PauseSoundEffects(SoundEffect soundEffect, in bool all = false)
        {
            foreach (SoundEffectPlayer s in lstSoundEffects)
            {
                if(soundEffect.Name == s.name)
                {
                    s.soundEffect.Pause();
                    if (!all)
                        break;
                }
            }
        }
        public static void ResumeSoundEffects()
        {
            foreach (SoundEffectPlayer s in lstSoundEffects)
            {
                s.soundEffect.Resume();
            }
        }
        public static void ResumeSoundEffects(SoundEffect soundEffect, in bool all = false)
        {
            foreach (SoundEffectPlayer s in lstSoundEffects)
            {
                if (soundEffect.Name == s.name)
                {
                    s.soundEffect.Resume();
                    if (!all)
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <param name="all"> If the function remove or not all the soundEffect define like soundEffect in param or only the first one</param>
        /// <returns> true if at least one of the soundEffect are found, false otherwise</returns>
        public static bool StopSoundEffect(SoundEffect soundEffect, in bool all = false)
        {
            bool result = false;
            foreach(SoundEffectPlayer s in lstSoundEffects)
            {
                if(soundEffect.Name == s.name)
                {
                    result = true;
                    s.toRemove = true;
                    s.soundEffect.Stop();
                    if(!all)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public static void PlaySoundEffectAt(SoundEffect soundEffect, in Vector2 position, in float volume = 1f, in bool loop = false, in bool useStereo = true)
        {
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.Play();
            lstSoundEffects.Add(new SoundEffectPlayer(soundEffectInstance, position, null, (float)soundEffect.Duration.TotalSeconds, volume, loop, soundEffect.Name, useStereo));
        }

        public static void PlaySoundEffectAt(SoundEffect soundEffect, Sprite sprite, in float volume = 1f, in bool loop = false, in bool useStereo = true)
        {
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.Play();
            lstSoundEffects.Add(new SoundEffectPlayer(soundEffectInstance, sprite.position, sprite, (float)soundEffect.Duration.TotalSeconds, volume, loop, soundEffect.Name, useStereo));
        }

        /// <summary>
        /// Attach the mainLisener to the sprite in argument
        /// </summary>
        public static void SetMainLisener(Sprite sprite)
        {
            mainLisener = sprite;
        }

        public void Update()
        {
            if(mainLisener != null)
            {
                foreach (SoundEffectPlayer s in lstSoundEffects)
                {
                    s.Update();
                }
                lstSoundEffects.RemoveAll(s => s.toRemove);
            }
        }
    }
}
