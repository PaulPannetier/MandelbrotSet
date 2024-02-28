using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SME.Audio;

namespace SME
{
    abstract public class Scene
    {
        public List<Sprite> lstSpriteCollider;
        public List<Sprite> lstSpriteTrigger;
        public List<Sprite> lstSprite;//tout les sprite, qu'il soit collider ou non
        private AudioManager audioManager = new AudioManager();

        public virtual void Load()
        {
            lstSpriteCollider = new List<Sprite>();
            lstSpriteTrigger = new List<Sprite>();
            lstSprite = new List<Sprite>();
            for (int i = 5; i <= 40; i++)
            {
                AssetsManager.LoadContent<SpriteFont>("Arial" + i, "Fonts/Arial/Arial" + i);
            }
        }

        public virtual void UnLoad()
        {
            lstSpriteCollider.Clear();
            lstSpriteTrigger.Clear();
            lstSprite.Clear();
            AssetsManager.UnloadContent();
        }

        public virtual void Update()
        {
            Input.Update();
            foreach(Sprite s in lstSprite)
            {
                s.Update();
            }
            lstSprite.RemoveAll(s => s.ToRemove);
            lstSpriteCollider.RemoveAll(s => s == null || s.ToRemove);
            lstSpriteTrigger.RemoveAll(s => s == null || s.ToRemove);
            if (Camera.mainCamera != null)
            {
                Camera.mainCamera.Update();
            }
            audioManager.Update();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(Camera.mainCamera != null)
            {
                foreach (Sprite s in lstSprite)
                {
                    s.Draw(spriteBatch);
                }
            }
            else
            {
                spriteBatch.DrawString(Screen.screenBound, AssetsManager.fonts["Arial20"], "Error, no camera rendering", Color.Black,Geometrie.Layout.Center, 0.5f);
            }
        }
    }
}
