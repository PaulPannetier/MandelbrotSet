using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame;
using System.Diagnostics;

namespace SME
{
    public class MainGame : Game
    {
        public static MainGame mainGame;
        public GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Time time;

        public MainGame()
        {
            if(mainGame != null)
            {
                return;
            }
            mainGame = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            time = new Time();
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }

        private void Show(float[][] m)
        {
            for (int i = 0; i < m.Length; i++)
            {
                string text = "[";
                for (int j = 0; j < m[i].Length - 1; j++)
                {
                    text += m[i][j].ToString() + ", ";
                }
                text += m[i][m[i].Length - 1].ToString() + "]";
                Debug.WriteLine(text);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Geometrie.Initialize(GraphicsDevice);
            AssetsManager.Initialize(this);
            Time.timeScale = 1f;
            Random.SetRandomSeed(1);
            BeginScene beginScene = new BeginScene();
            SceneManager.LoadScene(beginScene);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            time.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            SceneManager.currentScene.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Vector3 matrixScale = new Vector3(Screen.scaleFactor.X, Screen.scaleFactor.Y, 1f);
            if (Camera.mainCamera != null)
            {
                GraphicsDevice.Clear(Camera.mainCamera.backGroundColor);
                matrixScale = new Vector3(matrixScale.X * Camera.mainCamera.scale.X, matrixScale.Y * Camera.mainCamera.scale.Y, matrixScale.Z);
            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(matrixScale));
            SceneManager.currentScene.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}