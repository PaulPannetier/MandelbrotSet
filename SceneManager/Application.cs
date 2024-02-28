using System.IO;

namespace SME
{
    public static class Application
    {
        /// <summary>
        /// Define the targeted Frame Rate for your game, it is 60 by default, use -1 to unlimited frame Rate.
        /// </summary>
        /// <param name="frameRate"></param>
        public static void SetTargetedFrameRate(in float targetFrameRate)
        {
            if(targetFrameRate <= 0f)
            {
                MainGame.mainGame.IsFixedTimeStep = false;
                MainGame.mainGame.graphics.SynchronizeWithVerticalRetrace = false;
                MainGame.mainGame.graphics.ApplyChanges();
            }
            else
            {
                MainGame.mainGame.IsFixedTimeStep = true;
                MainGame.mainGame.graphics.SynchronizeWithVerticalRetrace = false;
                MainGame.mainGame.TargetElapsedTime = new System.TimeSpan((long)((1000d / targetFrameRate) * 10000d));
                MainGame.mainGame.graphics.ApplyChanges();
            }
        }

        public static void EnableVerticalSynchronisation(bool enable)
        {
            Screen.verticalSynchronisation = enable;
        }

        public static void Close()
        {
            SceneManager.currentScene.UnLoad();
            MainGame.mainGame.Exit();
        }

        public static string MainDirectory() => Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
    }
}
