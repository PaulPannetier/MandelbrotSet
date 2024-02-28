using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SME;

namespace MyGame
{
    public class Scene2 : Scene
    {
        int n = 0;

        public override void Load()
        {
            base.Load();
            Camera.mainCamera = new Camera();
            Camera.mainCamera.backGroundColor = Color.Bisque;
        }

        public override void UnLoad()
        {
            base.UnLoad();

        }

        public override void Update()
        {
            base.Update();
            n++;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawCircle(Vector2.One * 450, 75, Color.BlueViolet);
            spriteBatch.DrawString(AssetsManager.fonts["Arial15"], n.ToString(), Vector2.One * 100, Color.Black);
        }
    }
}
