using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace SME
{
    static class AssetsManager
    {
        private static Dictionary<string, Texture2D> preloadTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SoundEffect> preloadSounds = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, SpriteFont> preloadFonts = new Dictionary<string, SpriteFont>();
        public static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
        private static ContentManager content;
        private static bool _preloadContent = false;
        public static bool preloadContent
        {
            get => _preloadContent;
            set
            {
                if(preloadContent && !value)
                {
                    textures = new Dictionary<string, Texture2D>(preloadTextures);
                    sounds = new Dictionary<string, SoundEffect>(preloadSounds);
                    fonts = new Dictionary<string, SpriteFont>(preloadFonts);
                    preloadTextures.Clear();
                    preloadSounds.Clear();
                    preloadFonts.Clear();
                }
                preloadContent = value;
            }
        }

        public static Texture2D CreateTexture2D(in int width, in int height) => new Texture2D(MainGame.mainGame.graphics.GraphicsDevice, width, height);

        public static void Initialize(MainGame mainGame)
        {
            content = MainGame.mainGame.Content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of content to load, can only load Texture2D, SoundEffect and SpriteFont</typeparam>
        /// <param name="key"></param>
        /// <param name="fileName"></param>
        public static void LoadContent<T>(string key, string fileName)
        {
            if(preloadContent)
            {
                if (typeof(T) == typeof(Texture2D))
                {
                    preloadTextures.Add(key, content.Load<Texture2D>(fileName));
                }
                else if (typeof(T) == typeof(SoundEffect))
                {
                    preloadSounds.Add(key, content.Load<SoundEffect>(fileName));
                }
                else if (typeof(T) == typeof(SpriteFont))
                {
                    preloadFonts.Add(key, content.Load<SpriteFont>(fileName));
                }
            }
            else
            {
                if (typeof(T) == typeof(Texture2D))
                {
                    textures.Add(key, content.Load<Texture2D>(fileName));
                }
                else if (typeof(T) == typeof(SoundEffect))
                {
                    sounds.Add(key, content.Load<SoundEffect>(fileName));
                }
                else if (typeof(T) == typeof(SpriteFont))
                {
                    fonts.Add(key, content.Load<SpriteFont>(fileName));
                }
            }
        }

        public static void UnloadContent()
        {
            preloadTextures.Clear();
            textures.Clear();
            preloadSounds.Clear();
            sounds.Clear();
            preloadFonts.Clear();
            fonts.Clear();
        }
    }
}
