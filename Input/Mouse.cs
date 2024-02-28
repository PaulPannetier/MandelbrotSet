using Microsoft.Xna.Framework;

namespace SME
{
    public static class Mouse
    {
        public static void SetVisible(bool visible)
        {
            MainGame.mainGame.IsMouseVisible = visible;
        }

        public static Vector2 Position() => Input.GetMousePosition();
        /// <param name="direction"> the direction of the mousewheel return by the function </param>
        /// <returns> true during the frame where the mouse wheel is moved.</returns>
        public static bool Wheel(out Input.MouseWheelDirection direction) => Input.MouseWheel(out direction);
        public static bool GetButtonDown(Input.MouseButtons button) => Input.GetKeyDown(button);
        public static bool GetButton(Input.MouseButtons button) => Input.GetKey(button);
        public static Vector2 Velocity
        {
            get => Input.MouseVelocity();
        }
    }
}
