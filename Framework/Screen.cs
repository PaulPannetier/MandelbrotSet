using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SME
{
    public static class Screen
    {
        public static int width { get; private set; }
        public static int height { get; private set; }
        public static Vector2 devScreenSize;
        public static Vector2 scaleFactor;
        public static Rectangle screenBound => new Rectangle(0, 0, width, height);

        /// <param name="developpementScreenSize">Les dim de l'écran lors du dévelopement du jeux, comme ca en changeant taille d'ecran le jeu se redimensionnera</param>
        public static void SetDevelopementScreen(in Vector2 developpementScreenSize)
        {
            devScreenSize = developpementScreenSize;
            scaleFactor = new Vector2(width / devScreenSize.X, height / devScreenSize.Y);
        }

        public static void SetDevelopementScreen(in int width, in int height) => SetDevelopementScreen(new Vector2(width, height));

        public static void ChangeResolution(int width, int height, bool IsFullScreen = false)
        {
            MainGame.mainGame.graphics.PreferredBackBufferWidth = Screen.width = width;
            MainGame.mainGame.graphics.PreferredBackBufferHeight = Screen.height = height;
            MainGame.mainGame.graphics.IsFullScreen = IsFullScreen;
            scaleFactor = new Vector2(width / devScreenSize.X, height / devScreenSize.Y);
            MainGame.mainGame.graphics.ApplyChanges();
        }

        public static bool verticalSynchronisation
        {
            get => MainGame.mainGame.graphics.SynchronizeWithVerticalRetrace;
            set
            {
                if(value)
                {
                    MainGame.mainGame.graphics.SynchronizeWithVerticalRetrace = true;
                    MainGame.mainGame.IsFixedTimeStep = true;
                }
                else
                {
                    MainGame.mainGame.graphics.SynchronizeWithVerticalRetrace = false;
                }
            }
        }

        public static void SetWindowsAt(in Vector2 position) { MainGame.mainGame.Window.Position = position.ToPoint(); }

        public static Vector2 GetUserMaxResolution() => new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
    }
}
