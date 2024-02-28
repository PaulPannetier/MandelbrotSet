
namespace SME
{
    public class Time
    {
        public static float timeScale;
        public static float dt { get; private set; }
        /// <summary>
        /// dt without the timeScale
        /// </summary>
        public static float fixedDt { get; private set; }

        public void Update(in float dt)
        {
            Time.dt = dt * timeScale;
            fixedDt = dt;
        }
    }
}
