
namespace SME
{
    public static class SceneManager
    {
        public static Scene currentScene;

        public static void LoadScene(Scene newScene, bool callLoadAndUnload = true)
        {
            if(currentScene != null && callLoadAndUnload)
            {
                currentScene.UnLoad();
            }
            currentScene = newScene;
            if(callLoadAndUnload)
                newScene.Load();
        }

        public static void LoadScene(Transition transition)
        {
            transition.Start();
            currentScene = transition;
        }
    }
}
