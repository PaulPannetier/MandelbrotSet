using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SME.JSON;

namespace SME
{
    public static class Input
    {
        /*
        Pour les manettes :
        GamePadState newGamePadState = GamePad.GetState(PlayerIndex.One);//récupère letat de la manette numéro 1
        newGamePadState.ThumbSticks.Right;//renvoie un vector2 décrivant la positionx et y du joyristick droit
        newGamePadState.ThumbSticks.Left; //renvoie un vector2 décrivant la positionx et y du joyristick gauche
        newGamePadState.IsConnected;//un booléen pour savoir si la première manette est connecté
        GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);//on fait vibré la manette de 50%  dintensité
        newGamePadState.IsButtonDown(Buttons.A)//le boutton A est appuyer ou pas
        */

        private static KeyboardState newKBState;
        private static KeyboardState oldKBState;
        private static MouseState newMouseState;
        private static MouseState oldMouseState;
        private static GamePadState newGamePadStateOne;
        private static GamePadState oldGamePadStateOne;
        private static GamePadState newGamePadStateTwo;
        private static GamePadState oldGamePadStateTwo;
        private static GamePadState newGamePadStateThree;
        private static GamePadState oldGamePadStateThree;
        private static GamePadState newGamePadStateFour;
        private static GamePadState oldGamePadStateFour;

        private static Dictionary<string, int> defaultkbController = new Dictionary<string, int>();
        private static Dictionary<string, int> defaultGB1Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> defaultGB2Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> defaultGB3Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> defaultGB4Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> kbController = new Dictionary<string, int>();
        private static Dictionary<string, int> gp1Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> gp2Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> gp3Controller = new Dictionary<string, int>();
        private static Dictionary<string, int> gp4Controller = new Dictionary<string, int>();

        private static string[] letters = new string[36] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public enum ControllerType
        {
            Keyboard,
            Gamepad,
            KeyboardAndGamepad
        }
        public enum MouseButtons
        {
            left = -1,
            right = -2,
            thumb1 = -3,
            thumb2 = -4,
            wheel = -5
        }
        public enum GamepadStick
        {
            right,
            left
        }
        public enum GamepadIndex
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            All = 5
        }
        public enum MouseWheelDirection
        {
            up,
            down,
            none
        }

        #region SetVibration

        public static void SetVivration(in float intensity, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (gamepadIndex)
            {
                case Input.GamepadIndex.One:
                    GamePad.SetVibration(PlayerIndex.One, intensity, intensity);
                    break;
                case Input.GamepadIndex.Two:
                    GamePad.SetVibration(PlayerIndex.Two, intensity, intensity);
                    break;
                case Input.GamepadIndex.Three:
                    GamePad.SetVibration(PlayerIndex.Three, intensity, intensity);
                    break;
                case Input.GamepadIndex.Four:
                    GamePad.SetVibration(PlayerIndex.Four, intensity, intensity);
                    break;
                case Input.GamepadIndex.All:
                    GamePad.SetVibration(PlayerIndex.One, intensity, intensity);
                    GamePad.SetVibration(PlayerIndex.Two, intensity, intensity);
                    GamePad.SetVibration(PlayerIndex.Three, intensity, intensity);
                    GamePad.SetVibration(PlayerIndex.Four, intensity, intensity);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region GamePadIsConnected

        /// <returns>true if the gamepad define by the gamepadIndex is pluged at the curretn frame, false otherwise </returns>
        public static bool GetGamepadPlugged(out GamepadIndex gamepadIndex)
        {
            if (newGamePadStateOne.IsConnected && !oldGamePadStateOne.IsConnected)
            {
                gamepadIndex = GamepadIndex.One;
                return true;
            }
            if (newGamePadStateTwo.IsConnected && !oldGamePadStateTwo.IsConnected)
            {
                gamepadIndex = GamepadIndex.Two;
                return true;
            }
            if (newGamePadStateThree.IsConnected && !oldGamePadStateThree.IsConnected)
            {
                gamepadIndex = GamepadIndex.Three;
                return true;
            }
            if (newGamePadStateFour.IsConnected && !oldGamePadStateFour.IsConnected)
            {
                gamepadIndex = GamepadIndex.Four;
                return true;
            }
            gamepadIndex = GamepadIndex.One;
            return false;
        }
        /// <returns>true if the gamepad define by the gamepadIndex is connected, false otherwise </returns>
        public static bool GamePadIsConnected(GamepadIndex gamepadIndex)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    return newGamePadStateOne.IsConnected;
                case GamepadIndex.Two:
                    return newGamePadStateTwo.IsConnected;
                case GamepadIndex.Three:
                    return newGamePadStateThree.IsConnected;
                case GamepadIndex.Four:
                    return newGamePadStateFour.IsConnected;
                case GamepadIndex.All:
                    return newGamePadStateOne.IsConnected && newGamePadStateTwo.IsConnected && newGamePadStateThree.IsConnected && newGamePadStateFour.IsConnected;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Renvoie true lorsqu'une manette viens de ce déconnecter
        /// </summary>
        public static bool GetGamepadUnPlugged(out GamepadIndex gamepadIndex)
        {
            if (!newGamePadStateOne.IsConnected && oldGamePadStateOne.IsConnected)
            {
                gamepadIndex = GamepadIndex.One;
                return true;
            }
            if (!newGamePadStateTwo.IsConnected && oldGamePadStateTwo.IsConnected)
            {
                gamepadIndex = GamepadIndex.Two;
                return true;
            }
            if (!newGamePadStateThree.IsConnected && oldGamePadStateThree.IsConnected)
            {
                gamepadIndex = GamepadIndex.Three;
                return true;
            }
            if (!newGamePadStateFour.IsConnected && oldGamePadStateFour.IsConnected)
            {
                gamepadIndex = GamepadIndex.Four;
                return true;
            }
            gamepadIndex = GamepadIndex.One;
            return false;
        }

        #endregion

        #region GetKeyDown / GetKey / GetKeyUp

        /// <returns> true during the frame when the key assigned with the action is pressed</returns>
        public static bool GetKeyDown(string action, in ControllerType controlerType = ControllerType.Keyboard)
        {
            if(controlerType == ControllerType.Keyboard)
            {
                return GetKeyDown(action, controlerType, GamepadIndex.One);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                return GetKeyDown(action, controlerType, GamepadIndex.One);
            }
            if(controlerType == ControllerType.KeyboardAndGamepad)
            {
                return GetKeyDown(action, ControllerType.Keyboard, GamepadIndex.One) || GetKeyDown(action, ControllerType.Gamepad, GamepadIndex.One);
            }
            return false;
        }

        /// <returns> true during the frame when the key assigned with the action is unpressed</returns>
        public static bool GetKeyUp(string action, in ControllerType controlerType = ControllerType.Keyboard)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                return GetKeyUp(action, controlerType, GamepadIndex.One);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                return GetKeyUp(action, controlerType, GamepadIndex.One);
            }
            if (controlerType == ControllerType.KeyboardAndGamepad)
            {
                return GetKeyUp(action, ControllerType.Keyboard, GamepadIndex.One) || GetKeyUp(action, ControllerType.Gamepad, GamepadIndex.One);
            }
            return false;
        }

        /// <returns> true during the frame when a key assigned with one of the actions is pressed</returns>
        public static bool GetKeyDown(List<string> actions, in ControllerType controlerType = ControllerType.Keyboard)
        {
            foreach (string action in actions)
            {
                if(GetKeyDown(action, controlerType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <returns> true during the frame when a key assigned with one of the actions is unpressed</returns>
        public static bool GetKeyUp(List<string> actions, in ControllerType controlerType = ControllerType.Keyboard)
        {
            foreach (string action in actions)
            {
                if (GetKeyUp(action, controlerType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <returns> true during the frame when the key assigned with the action is pressed</returns>
        public static bool GetKeyDown(string action, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                if(!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                int key = kbController[action];
                if (key < 0)//la souris
                {
                    switch ((MouseButtons)key)
                    {
                        case MouseButtons.left:
                            return newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
                        case MouseButtons.right:
                            return newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released;
                        case MouseButtons.thumb1:
                            return newMouseState.XButton2 == ButtonState.Pressed && oldMouseState.XButton2 == ButtonState.Released;
                        case MouseButtons.thumb2:
                            return newMouseState.XButton1 == ButtonState.Pressed && oldMouseState.XButton1 == ButtonState.Released;
                        case MouseButtons.wheel:
                            return newMouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released;
                        default:
                            return false;
                    }
                }
                return newKBState.IsKeyDown((Keys)key) && oldKBState.IsKeyUp((Keys)key);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonDown(key) && oldGamePadStateOne.IsButtonUp(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonDown(key) && oldGamePadStateTwo.IsButtonUp(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonDown(key) && oldGamePadStateThree.IsButtonUp(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonDown(key) && oldGamePadStateFour.IsButtonUp(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return (newGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action]) && oldGamePadStateOne.IsButtonUp((Buttons)gp1Controller[action])) ||
                               (newGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action]) && oldGamePadStateTwo.IsButtonUp((Buttons)gp2Controller[action])) ||
                               (newGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action]) && oldGamePadStateThree.IsButtonUp((Buttons)gp3Controller[action])) ||
                               (newGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]) && oldGamePadStateFour.IsButtonUp((Buttons)gp4Controller[action]));
                    default:
                        return false;
                }
            }
            if (controlerType == ControllerType.KeyboardAndGamepad)
            {
                bool result = false;
                if (!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                int keys = kbController[action];
                if (keys < 0)//la souris
                {
                    switch ((MouseButtons)keys)
                    {
                        case MouseButtons.left:
                            result = newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
                            break;
                        case MouseButtons.right:
                            result = newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released;
                            break;
                        case MouseButtons.thumb1:
                            result = newMouseState.XButton2 == ButtonState.Pressed && oldMouseState.XButton2 == ButtonState.Released;
                            break;
                        case MouseButtons.thumb2:
                            result = newMouseState.XButton1 == ButtonState.Pressed && oldMouseState.XButton1 == ButtonState.Released;
                            break;
                        case MouseButtons.wheel:
                            result = newMouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    result = newKBState.IsKeyDown((Keys)keys) && oldKBState.IsKeyUp((Keys)keys);
                }
                if (result)
                    return true;
                //GP
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonDown(key) && oldGamePadStateOne.IsButtonUp(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonDown(key) && oldGamePadStateTwo.IsButtonUp(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonDown(key) && oldGamePadStateThree.IsButtonUp(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonDown(key) && oldGamePadStateFour.IsButtonUp(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return (newGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action]) && oldGamePadStateOne.IsButtonUp((Buttons)gp1Controller[action])) ||
                               (newGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action]) && oldGamePadStateTwo.IsButtonUp((Buttons)gp2Controller[action])) ||
                               (newGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action]) && oldGamePadStateThree.IsButtonUp((Buttons)gp3Controller[action])) ||
                               (newGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]) && oldGamePadStateFour.IsButtonUp((Buttons)gp4Controller[action]));
                    default:
                        return false;
                }
            }
            return false;
        }

        /// <returns> true during the frame when the key assigned with the action is unpressed</returns>
        public static bool GetKeyUp(string action, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                if (!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                int key = kbController[action];
                if (key < 0)//la souris
                {
                    switch ((MouseButtons)key)
                    {
                        case MouseButtons.left:
                            return newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed;
                        case MouseButtons.right:
                            return newMouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed;
                        case MouseButtons.thumb1:
                            return newMouseState.XButton2 == ButtonState.Released && oldMouseState.XButton2 == ButtonState.Pressed;
                        case MouseButtons.thumb2:
                            return newMouseState.XButton1 == ButtonState.Released && oldMouseState.XButton1 == ButtonState.Pressed;
                        case MouseButtons.wheel:
                            return newMouseState.MiddleButton == ButtonState.Released && oldMouseState.MiddleButton == ButtonState.Pressed;
                        default:
                            return false;
                    }
                }
                return newKBState.IsKeyUp((Keys)key) && oldKBState.IsKeyDown((Keys)key);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonUp(key) && oldGamePadStateOne.IsButtonDown(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonUp(key) && oldGamePadStateTwo.IsButtonDown(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonUp(key) && oldGamePadStateThree.IsButtonDown(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonUp(key) && oldGamePadStateFour.IsButtonDown(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return (newGamePadStateOne.IsButtonUp((Buttons)gp1Controller[action]) && oldGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action])) ||
                               (newGamePadStateTwo.IsButtonUp((Buttons)gp2Controller[action]) && oldGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action])) ||
                               (newGamePadStateThree.IsButtonUp((Buttons)gp3Controller[action]) && oldGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action])) ||
                               (newGamePadStateFour.IsButtonUp((Buttons)gp4Controller[action]) && oldGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]));
                    default:
                        return false;
                }
            }
            if (controlerType == ControllerType.KeyboardAndGamepad)
            {
                bool result = false;
                if (!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                int keys = kbController[action];
                if (keys < 0)//la souris
                {
                    switch ((MouseButtons)keys)
                    {
                        case MouseButtons.left:
                            result = newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed;
                            break;
                        case MouseButtons.right:
                            result = newMouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed;
                            break;
                        case MouseButtons.thumb1:
                            result = newMouseState.XButton2 == ButtonState.Released && oldMouseState.XButton2 == ButtonState.Pressed;
                            break;
                        case MouseButtons.thumb2:
                            result = newMouseState.XButton1 == ButtonState.Released && oldMouseState.XButton1 == ButtonState.Pressed;
                            break;
                        case MouseButtons.wheel:
                            result = newMouseState.MiddleButton == ButtonState.Released && oldMouseState.MiddleButton == ButtonState.Pressed;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    result = newKBState.IsKeyUp((Keys)keys) && oldKBState.IsKeyDown((Keys)keys);
                }
                if (result)
                    return true;
                //GP
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonUp(key) && oldGamePadStateOne.IsButtonDown(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonUp(key) && oldGamePadStateTwo.IsButtonDown(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonUp(key) && oldGamePadStateThree.IsButtonDown(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonUp(key) && oldGamePadStateFour.IsButtonDown(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return (newGamePadStateOne.IsButtonUp((Buttons)gp1Controller[action]) && oldGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action])) ||
                               (newGamePadStateTwo.IsButtonUp((Buttons)gp2Controller[action]) && oldGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action])) ||
                               (newGamePadStateThree.IsButtonUp((Buttons)gp3Controller[action]) && oldGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action])) ||
                               (newGamePadStateFour.IsButtonUp((Buttons)gp4Controller[action]) && oldGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]));
                    default:
                        return false;
                }
            }
            return false;
        }

        /// <returns> true during the frame when a key assigned with one of the actions is pressed</returns>
        public static bool GetKeyDown(List<string> actions, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            foreach (string action in actions)
            {
                if (GetKeyDown(action, controlerType, gamepadIndex))
                {
                    return true;
                }
            }
            return false;
        }

        /// <returns> true during the frame when a key assigned with one of the actions is pressed</returns>
        public static bool GetKeyUp(List<string> actions, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            foreach (string action in actions)
            {
                if (GetKeyUp(action, controlerType, gamepadIndex))
                {
                    return true;
                }
            }
            return false;
        }

        /// <returns> true during the frame when the keyboard key is pressed</returns>
        public static bool GetKeyDown(Keys k) => newKBState.IsKeyDown(k) && oldKBState.IsKeyUp(k);
        public static bool GetKeyUp(Keys k) => newKBState.IsKeyUp(k) && oldKBState.IsKeyDown(k);
        /// <returns> true during the frame when the mouse button is pressed</returns>
        public static bool GetKeyDown(MouseButtons b)
        {
            switch (b)
            {
                case MouseButtons.left:
                    return newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
                case MouseButtons.right:
                    return newMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released;
                case MouseButtons.thumb1:
                    return newMouseState.XButton2 == ButtonState.Pressed && oldMouseState.XButton2 == ButtonState.Released;
                case MouseButtons.thumb2:
                    return newMouseState.XButton1 == ButtonState.Pressed && oldMouseState.XButton1 == ButtonState.Released;
                case MouseButtons.wheel:
                    return newMouseState.MiddleButton == ButtonState.Pressed && oldMouseState.MiddleButton == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <returns> true during the frame when the mouse button is unpressed</returns>
        public static bool GetKeyUp(MouseButtons b)
        {
            switch (b)
            {
                case MouseButtons.left:
                    return newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.right:
                    return newMouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.thumb1:
                    return newMouseState.XButton2 == ButtonState.Released && oldMouseState.XButton2 == ButtonState.Pressed;
                case MouseButtons.thumb2:
                    return newMouseState.XButton1 == ButtonState.Released && oldMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.wheel:
                    return newMouseState.MiddleButton == ButtonState.Released && oldMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <returns> true during the frame when the gamepad button is pressed</returns>
        public static bool GetKeyDown(Buttons gamepadButton, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    return newGamePadStateOne.IsButtonDown(gamepadButton) && oldGamePadStateOne.IsButtonUp(gamepadButton);
                case GamepadIndex.Two:
                    return newGamePadStateTwo.IsButtonDown(gamepadButton) && oldGamePadStateTwo.IsButtonUp(gamepadButton);
                case GamepadIndex.Three:
                    return newGamePadStateThree.IsButtonDown(gamepadButton) && oldGamePadStateThree.IsButtonUp(gamepadButton);
                case GamepadIndex.Four:
                    return newGamePadStateFour.IsButtonDown(gamepadButton) && oldGamePadStateFour.IsButtonUp(gamepadButton);
                case GamepadIndex.All:
                    return (newGamePadStateOne.IsButtonDown(gamepadButton) && oldGamePadStateOne.IsButtonUp(gamepadButton)) ||
                           (newGamePadStateTwo.IsButtonDown(gamepadButton) && oldGamePadStateTwo.IsButtonUp(gamepadButton)) ||
                           (newGamePadStateThree.IsButtonDown(gamepadButton) && oldGamePadStateThree.IsButtonUp(gamepadButton)) ||
                           (newGamePadStateFour.IsButtonDown(gamepadButton) && oldGamePadStateFour.IsButtonUp(gamepadButton));
                default:
                    return false;
            }
        }

        /// <returns> true during the frame when the gamepad button is unpressed</returns>
        public static bool GetKeyUp(Buttons gamepadButton, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    return newGamePadStateOne.IsButtonUp(gamepadButton) && oldGamePadStateOne.IsButtonDown(gamepadButton);
                case GamepadIndex.Two:
                    return newGamePadStateTwo.IsButtonUp(gamepadButton) && oldGamePadStateTwo.IsButtonDown(gamepadButton);
                case GamepadIndex.Three:
                    return newGamePadStateThree.IsButtonUp(gamepadButton) && oldGamePadStateThree.IsButtonDown(gamepadButton);
                case GamepadIndex.Four:
                    return newGamePadStateFour.IsButtonUp(gamepadButton) && oldGamePadStateFour.IsButtonDown(gamepadButton);
                case GamepadIndex.All:
                    return (newGamePadStateOne.IsButtonUp(gamepadButton) && oldGamePadStateOne.IsButtonDown(gamepadButton)) ||
                           (newGamePadStateTwo.IsButtonUp(gamepadButton) && oldGamePadStateTwo.IsButtonDown(gamepadButton)) ||
                           (newGamePadStateThree.IsButtonUp(gamepadButton) && oldGamePadStateThree.IsButtonDown(gamepadButton)) ||
                           (newGamePadStateFour.IsButtonUp(gamepadButton) && oldGamePadStateFour.IsButtonDown(gamepadButton));
                default:
                    return false;
            }
        }

        /// <returns> true while the key assigned to the action is pressed </returns>
        public static bool GetKey(string action, in ControllerType controlerType = ControllerType.Keyboard)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                return GetKey(action, controlerType, GamepadIndex.One);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                return GetKey(action, controlerType, GamepadIndex.One);
            }
            if (controlerType == ControllerType.KeyboardAndGamepad)
            {
                return GetKey(action, ControllerType.Keyboard, GamepadIndex.One) || GetKeyDown(action, ControllerType.Gamepad, GamepadIndex.One);
            }
            return false;
        }
        /// <returns> true while a key assigned to one of the actions is pressed </returns>
        public static bool GetKey(List<string> actions, in ControllerType controlerType = ControllerType.Keyboard)
        {
            foreach (string action in actions)
            {
                if (GetKey(action, controlerType))
                {
                    return true;
                }
            }
            return false;
        }
        /// <returns> true while the key assigned to the action is pressed </returns>
        public static bool GetKey(string action, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                if (!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                int key = kbController[action];
                if (key < 0)//la souris
                {
                    switch ((MouseButtons)key)
                    {
                        case MouseButtons.left:
                            return newMouseState.LeftButton == ButtonState.Pressed;
                        case MouseButtons.right:
                            return newMouseState.RightButton == ButtonState.Pressed;
                        case MouseButtons.thumb1:
                            return newMouseState.XButton2 == ButtonState.Pressed;
                        case MouseButtons.thumb2:
                            return newMouseState.XButton1 == ButtonState.Pressed;
                        case MouseButtons.wheel:
                            return newMouseState.MiddleButton == ButtonState.Pressed;
                        default:
                            return false;
                    }
                }
                return newKBState.IsKeyDown((Keys)key);
            }
            if (controlerType == ControllerType.Gamepad)
            {
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonDown(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonDown(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonDown(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonDown(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return newGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action]) || newGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action]) ||
                               newGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action]) || newGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]);
                    default:
                        return false;
                }
            }
            if (controlerType == ControllerType.KeyboardAndGamepad)
            {
                bool result = false;
                int keys = 10;//TODO : récup la clef dans le dico
                if (keys < 0)//la souris
                {
                    switch ((MouseButtons)keys)
                    {
                        case MouseButtons.left:
                            result = newMouseState.LeftButton == ButtonState.Pressed;
                            break;
                        case MouseButtons.right:
                            result = newMouseState.RightButton == ButtonState.Pressed;
                            break;
                        case MouseButtons.thumb1:
                            result = newMouseState.XButton2 == ButtonState.Pressed;
                            break;
                        case MouseButtons.thumb2:
                            result = newMouseState.XButton1 == ButtonState.Pressed;
                            break;
                        case MouseButtons.wheel:
                            result = newMouseState.MiddleButton == ButtonState.Pressed;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    result = newKBState.IsKeyDown((Keys)keys);
                }
                if (result)
                    return true;
                //GP
                Buttons key;
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        key = (Buttons)gp1Controller[action];
                        return newGamePadStateOne.IsButtonDown(key);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        key = (Buttons)gp2Controller[action];
                        return newGamePadStateTwo.IsButtonDown(key);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        key = (Buttons)gp3Controller[action];
                        return newGamePadStateThree.IsButtonDown(key);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        key = (Buttons)gp4Controller[action];
                        return newGamePadStateFour.IsButtonDown(key);
                    case GamepadIndex.All:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return newGamePadStateOne.IsButtonDown((Buttons)gp1Controller[action]) || newGamePadStateTwo.IsButtonDown((Buttons)gp2Controller[action]) ||
                               newGamePadStateThree.IsButtonDown((Buttons)gp3Controller[action]) || newGamePadStateFour.IsButtonDown((Buttons)gp4Controller[action]);
                    default:
                        return false;
                }
            }
            return false;
        }
        /// <returns> true while a key assigned to one of the actions is pressed </returns>
        public static bool GetKey(List<string> actions, in ControllerType controlerType, in GamepadIndex gamepadIndex)
        {
            foreach (string action in actions)
            {
                if (GetKey(action, controlerType, gamepadIndex))
                {
                    return true;
                }
            }
            return false;
        }
        /// <returns> true while the keyboard key is pressed </returns>
        public static bool GetKey(Keys k) => newKBState.IsKeyDown(k);
        /// <returns> true while the mouse button is pressed </returns>
        public static bool GetKey(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.left:
                    return newMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.right:
                    return newMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.thumb1:
                    return newMouseState.XButton2 == ButtonState.Pressed;
                case MouseButtons.thumb2:
                    return newMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.wheel:
                    return newMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        /// <returns> true while the gamepad button is pressed </returns>
        public static bool GetKey(Buttons gamepadButton, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    return newGamePadStateOne.IsButtonDown(gamepadButton);
                case GamepadIndex.Two:
                    return newGamePadStateTwo.IsButtonDown(gamepadButton);
                case GamepadIndex.Three:
                    return newGamePadStateThree.IsButtonDown(gamepadButton);
                case GamepadIndex.Four:
                    return newGamePadStateFour.IsButtonDown(gamepadButton);
                case GamepadIndex.All:
                    return newGamePadStateOne.IsButtonDown(gamepadButton) || newGamePadStateTwo.IsButtonDown(gamepadButton) || newGamePadStateThree.IsButtonDown(gamepadButton) || newGamePadStateFour.IsButtonDown(gamepadButton);
                default:
                    return false;
            }
        }

        #endregion

        #region Management controller

        /// <summary>
        /// Add an action to the input system. Multiply action can have the same key.
        /// </summary>
        /// <param name="action"> The action</param>
        /// <param name="keyboardKey"> The keyboard key link with the action</param>
        public static void AddInputAction(string action, in Keys keyboardKey)
        {
            if(kbController.ContainsKey(action))
            {
                ReplaceAction(action, keyboardKey);
            }
            else
            {
                kbController.Add(action, (int)keyboardKey);
            }
        }
        /// <summary>
        /// Change the keyboard key assigned to the action in param
        /// </summary>
        public static void ReplaceAction(string action, in Keys newKeyboardKey)
        {
            if(kbController.ContainsKey(action))
            {
                kbController[action] = (int)newKeyboardKey;
            }
        }
        /// <summary>
        /// Add an action to the input system. Multiply action can have the same key.
        /// </summary>
        /// <param name="action"> The action</param>
        /// <param name="keyboardKey"> The Mouse button link with the action</param>
        public static void AddInputAction(string action, in MouseButtons mouseButton)
        {
            if (kbController.ContainsKey(action))
            {
                ReplaceAction(action, mouseButton);
            }
            else
            {
                kbController.Add(action, (int)mouseButton);
            }
        }
        /// <summary>
        /// Change the mouse button assigned to the action in param
        /// </summary>
        public static void ReplaceAction(string action, in MouseButtons newMouseButton)
        {
            if (kbController.ContainsKey(action))
            {
                kbController[action] = (int)newMouseButton;
            }
        }
        /// <summary>
        /// Add an action to the input system. Multiply action can have the same key.
        /// </summary>
        /// <param name="action"> The action</param>
        /// <param name="keyboardKey"> The gamepad button link with the action</param>
        public static void AddInputAction(string action, in Buttons gamepadButton, in GamepadIndex gamepadIndex)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    if(gp1Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.One);
                    }
                    else
                    {
                        gp1Controller.Add(action, (int)gamepadButton);
                    }
                    break;
                case GamepadIndex.Two:
                    if (gp2Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Two);
                    }
                    else
                    {
                        gp2Controller.Add(action, (int)gamepadButton);
                    }
                    break;
                case GamepadIndex.Three:
                    if (gp3Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Three);
                    }
                    else
                    {
                        gp3Controller.Add(action, (int)gamepadButton);
                    }
                    break;
                case GamepadIndex.Four:
                    if (gp4Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Four);
                    }
                    else
                    {
                        gp4Controller.Add(action, (int)gamepadButton);
                    }
                    break;
                case GamepadIndex.All:
                    if (gp1Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.One);
                    }
                    else
                    {
                        gp1Controller.Add(action, (int)gamepadButton);
                    }
                    if (gp2Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Two);
                    }
                    else
                    { 
                        gp2Controller.Add(action, (int)gamepadButton);
                    }
                    if (gp3Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Three);
                    }
                    else
                    {
                        gp3Controller.Add(action, (int)gamepadButton);
                    }
                    if (gp4Controller.ContainsKey(action))
                    {
                        ReplaceAction(action, gamepadButton, GamepadIndex.Four);
                    }
                    else
                    {
                        gp4Controller.Add(action, (int)gamepadButton);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Change the gamepad button assigned to the action in param
        /// </summary>
        public static void ReplaceAction(string action, in Buttons newGamepadButton, in GamepadIndex gamepadIndex)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    if (gp1Controller.ContainsKey(action))
                    {
                        gp1Controller[action] = (int)newGamepadButton;
                    }
                    break;
                case GamepadIndex.Two:
                    if (gp2Controller.ContainsKey(action))
                    {
                        gp2Controller[action] = (int)newGamepadButton;
                    }
                    break;
                case GamepadIndex.Three:
                    if (gp3Controller.ContainsKey(action))
                    {
                        gp3Controller[action] = (int)newGamepadButton;
                    }
                    break;
                case GamepadIndex.Four:
                    if (gp4Controller.ContainsKey(action))
                    {
                        gp4Controller[action] = (int)newGamepadButton;
                    }
                    break;
                case GamepadIndex.All:
                    if (gp1Controller.ContainsKey(action))
                    {
                        gp1Controller[action] = (int)newGamepadButton;
                    }
                    if (gp2Controller.ContainsKey(action))
                    {
                        gp2Controller[action] = (int)newGamepadButton;
                    }
                    if (gp3Controller.ContainsKey(action))
                    {
                        gp3Controller[action] = (int)newGamepadButton;
                    }
                    if (gp4Controller.ContainsKey(action))
                    {
                        gp4Controller[action] = (int)newGamepadButton;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Remove the action from the input system
        /// </summary>
        /// <param name="action"> The action to remove.</param>
        /// <param name="controllerType">The controller where the action will be removed.</param>
        public static void RemoveAction(string action, ControllerType controllerType, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (controllerType)
            {
                case ControllerType.Keyboard:
                    kbController.Remove(action);
                    break;
                case ControllerType.Gamepad:
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            gp1Controller.Remove(action);
                            break;
                        case GamepadIndex.Two:
                            gp2Controller.Remove(action);
                            break;
                        case GamepadIndex.Three:
                            gp3Controller.Remove(action);
                            break;
                        case GamepadIndex.Four:
                            gp4Controller.Remove(action);
                            break;
                        case GamepadIndex.All:
                            gp1Controller.Remove(action);
                            gp2Controller.Remove(action);
                            gp3Controller.Remove(action);
                            gp4Controller.Remove(action);
                            break;
                        default:
                            break;
                    }
                    break;
                case ControllerType.KeyboardAndGamepad:
                    kbController.Remove(action);
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            gp1Controller.Remove(action);
                            break;
                        case GamepadIndex.Two:
                            gp2Controller.Remove(action);
                            break;
                        case GamepadIndex.Three:
                            gp3Controller.Remove(action);
                            break;
                        case GamepadIndex.Four:
                            gp4Controller.Remove(action);
                            break;
                        case GamepadIndex.All:
                            gp1Controller.Remove(action);
                            gp2Controller.Remove(action);
                            gp3Controller.Remove(action);
                            gp4Controller.Remove(action);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Define the default Controller at the current configuration
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="gamepadIndex"></param>
        public static void SetDefaultControler(ControllerType controllerType, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (controllerType)
            {
                case ControllerType.Keyboard:
                    defaultkbController = kbController;
                    break;
                case ControllerType.Gamepad:
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            defaultGB1Controller = gp1Controller;
                            break;
                        case GamepadIndex.Two:
                            defaultGB2Controller = gp2Controller;
                            break;
                        case GamepadIndex.Three:
                            defaultGB3Controller = gp3Controller;
                            break;
                        case GamepadIndex.Four:
                            defaultGB4Controller = gp4Controller;
                            break;
                        case GamepadIndex.All:
                            defaultGB1Controller = gp1Controller;
                            defaultGB2Controller = gp2Controller;
                            defaultGB3Controller = gp3Controller;
                            defaultGB4Controller = gp4Controller;
                            break;
                        default:
                            break;
                    }
                    break;
                case ControllerType.KeyboardAndGamepad:
                    defaultkbController = kbController;
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            defaultGB1Controller = gp1Controller;
                            break;
                        case GamepadIndex.Two:
                            defaultGB2Controller = gp2Controller;
                            break;
                        case GamepadIndex.Three:
                            defaultGB3Controller = gp3Controller;
                            break;
                        case GamepadIndex.Four:
                            defaultGB4Controller = gp4Controller;
                            break;
                        case GamepadIndex.All:
                            defaultGB1Controller = gp1Controller;
                            defaultGB2Controller = gp2Controller;
                            defaultGB3Controller = gp3Controller;
                            defaultGB4Controller = gp4Controller;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public static void LoadDefaultController(ControllerType controllerType, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            switch (controllerType)
            {
                case ControllerType.Keyboard:
                    kbController = defaultkbController;
                    break;
                case ControllerType.Gamepad:
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            gp1Controller = defaultGB1Controller;
                            break;
                        case GamepadIndex.Two:
                            gp2Controller = defaultGB2Controller;
                            break;
                        case GamepadIndex.Three:
                            gp3Controller = defaultGB3Controller;
                            break;
                        case GamepadIndex.Four:
                            gp4Controller = defaultGB4Controller;
                            break;
                        case GamepadIndex.All:
                            gp1Controller = defaultGB1Controller;
                            gp2Controller = defaultGB2Controller;
                            gp3Controller = defaultGB3Controller;
                            gp4Controller = defaultGB4Controller;
                            break;
                        default:
                            break;
                    }
                    break;
                case ControllerType.KeyboardAndGamepad:
                    kbController = defaultkbController;
                    switch (gamepadIndex)
                    {
                        case GamepadIndex.One:
                            gp1Controller = defaultGB1Controller;
                            break;
                        case GamepadIndex.Two:
                            gp2Controller = defaultGB2Controller;
                            break;
                        case GamepadIndex.Three:
                            gp3Controller = defaultGB3Controller;
                            break;
                        case GamepadIndex.Four:
                            gp4Controller = defaultGB4Controller;
                            break;
                        case GamepadIndex.All:
                            gp1Controller = defaultGB1Controller;
                            gp2Controller = defaultGB2Controller;
                            gp3Controller = defaultGB3Controller;
                            gp4Controller = defaultGB4Controller;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region SaveController

        private class InputConfig
        {
            public Dictionary<string, int> defaultkbController { get; set; }
            public Dictionary<string, int> defaultGB1Controller { get; set; }
            public Dictionary<string, int> defaultGB2Controller { get; set; }
            public Dictionary<string, int> defaultGB3Controller { get; set; }
            public Dictionary<string, int> defaultGB4Controller { get; set; }
            public Dictionary<string, int> kbController { get; set; }
            public Dictionary<string, int> gp1Controller { get; set; }
            public Dictionary<string, int> gp2Controller { get; set; }
            public Dictionary<string, int> gp3Controller { get; set; }
            public Dictionary<string, int> gp4Controller { get; set; }

            public InputConfig()
            {

            }

            public InputConfig(Dictionary<string, int> defaultkbController, Dictionary<string, int> defaultGB1Controller, Dictionary<string, int> defaultGB2Controller,
                Dictionary<string, int> defaultGB3Controller, Dictionary<string, int> defaultGB4Controller, Dictionary<string, int> kbController,
                Dictionary<string, int> gp1Controller, Dictionary<string, int> gp2Controller, Dictionary<string, int> gp3Controller,
                Dictionary<string, int> gp4Controller)
            {
                this.defaultkbController = defaultkbController; this.defaultGB1Controller = defaultGB1Controller;
                this.defaultGB2Controller = defaultGB2Controller; this.defaultGB3Controller = defaultGB3Controller;
                this.defaultGB4Controller = defaultGB4Controller; this.kbController = kbController;
                this.gp1Controller = gp1Controller; this.gp2Controller = gp2Controller;
                this.gp3Controller = gp3Controller; this.gp4Controller = gp4Controller;
            }
        }

        /// <summary>
        /// Save all the current input configuration (default and current actions and controllers keys link to the action) for the keyboard and the four gamepad in the file Save in the game repertory,
        /// can be load using the methode Input.LoadConfiguration().
        /// </summary>
        public static void SaveConfiguration()
        {
            InputConfig inputConfig = new InputConfig(defaultkbController, defaultGB1Controller, defaultGB2Controller, defaultGB3Controller, defaultGB4Controller,
                kbController, gp1Controller, gp2Controller, gp3Controller, gp4Controller);
            Save.WriteObject<InputConfig>(inputConfig, @"\Save\input.save");
        }

        /// <summary>
        /// Save all the default input configuration (default actions and controllers keys link to the action) for the keyboard and the four gamepad in the file Save in the game repertory,
        /// but don't touch at the current input configuration.
        /// Can be load using the methode Input.LoadDefaultConfiguration().
        /// </summary>
        public static void SaveDefaultConfiguration()
        {
            InputConfig i = Save.ReadObject<InputConfig>(@"\Save\input.save");
            InputConfig inputConfig = new InputConfig(defaultkbController, defaultGB1Controller, defaultGB2Controller, defaultGB3Controller, defaultGB4Controller,
                i.kbController, i.gp1Controller, i.gp2Controller, i.gp3Controller, i.gp4Controller);
            Save.WriteObject(inputConfig, @"\Save\input.save");
        }
        /// <summary>
        /// Save all the current input configuration (current actions and controllers keys link to the action) for the keyboard and the four gamepad in the file Save in the game repertory,
        /// but don't touch at the default input configuration.
        /// Can be load using the methode Input.LoadCurrentConfiguration().
        /// </summary>
        public static void SaveCurrentConfiguration()
        {
            InputConfig i = Save.ReadObject<InputConfig>(@"\Save\input.save");
            InputConfig inputConfig = new InputConfig(i.defaultkbController, i.defaultGB1Controller, i.defaultGB2Controller, i.defaultGB3Controller, i.defaultGB4Controller,
                kbController, gp1Controller, gp2Controller, gp3Controller, gp4Controller);
            Save.WriteObject(inputConfig, @"\Save\input.save");
        }

        /// <summary>
        /// Load from the file Save in the game repertory all the configuration of the Input system.
        /// </summary>
        public static void LoadConfiguration()
        {
            InputConfig i = Save.ReadObject<InputConfig>(@"\Save\input.save");
            defaultkbController = i.defaultkbController;
            defaultGB1Controller = i.defaultGB1Controller;
            defaultGB2Controller = i.defaultGB2Controller;
            defaultGB3Controller = i.defaultGB3Controller;
            defaultGB4Controller = i.defaultGB4Controller;
            kbController = i.kbController;
            gp1Controller = i.gp1Controller;
            gp2Controller = i.gp2Controller;
            gp3Controller = i.gp3Controller;
            gp4Controller = i.gp4Controller;
        }
        /// <summary>
        /// Load from the file Save in the game repertory the default configuration of the Input system.
        /// </summary>
        public static void LoadDefaultConfiguration()
        {
            InputConfig i = Save.ReadObject<InputConfig>(@"\Save\input.save");
            defaultkbController = i.defaultkbController;
            defaultGB1Controller = i.defaultGB1Controller;
            defaultGB2Controller = i.defaultGB2Controller;
            defaultGB3Controller = i.defaultGB3Controller;
            defaultGB4Controller = i.defaultGB4Controller;
        }
        /// <summary>
        /// Load from the file Save in the game repertory the current configuration of the Input system.
        /// </summary>
        public static void LoadCurrentConfiguration()
        {
            InputConfig i = Save.ReadObject<InputConfig>(@"\Save\input.save");
            kbController = i.kbController;
            gp1Controller = i.gp1Controller;
            gp2Controller = i.gp2Controller;
            gp3Controller = i.gp3Controller;
            gp4Controller = i.gp4Controller;
        }

        #endregion

        public static Vector2 GetMousePosition() => newMouseState.Position.ToVector2();

        public static Vector2 MouseVelocity() => (newMouseState.Position - oldMouseState.Position).ToVector2() / Time.dt;

        public static Vector2 GetGamepadStickPosition(in GamepadIndex gamepadIndex, in GamepadStick gamepadStick)
        {
            switch (gamepadIndex)
            {
                case GamepadIndex.One:
                    return gamepadStick == GamepadStick.right ? newGamePadStateOne.ThumbSticks.Right : newGamePadStateOne.ThumbSticks.Left;
                case GamepadIndex.Two:
                    return gamepadStick == GamepadStick.right ? newGamePadStateTwo.ThumbSticks.Right : newGamePadStateTwo.ThumbSticks.Left;
                case GamepadIndex.Three:
                    return gamepadStick == GamepadStick.right ? newGamePadStateThree.ThumbSticks.Right : newGamePadStateThree.ThumbSticks.Left;
                case GamepadIndex.Four:
                    return gamepadStick == GamepadStick.right ? newGamePadStateFour.ThumbSticks.Right : newGamePadStateFour.ThumbSticks.Left;
                default:
                    return Vector2.Zero;
            }
        }

        #region Useful region

        /// <param name="direction"> the direction of the mousewheel return by the function </param>
        /// <returns> true during the frame where the mouse wheel is moved.</returns>
        public static bool MouseWheel(out MouseWheelDirection direction)
        {
            if (newMouseState.ScrollWheelValue != oldMouseState.ScrollWheelValue)
            {
                direction = newMouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue ? MouseWheelDirection.up : MouseWheelDirection.down;
                return true;
            }
            direction = MouseWheelDirection.none;
            return false;
        }

        /// <summary>
        /// Convert a key into a string.
        /// </summary>
        /// <param name="key"> the key to convert to a string</param>
        public static string KeyToString(in int key, ControllerType controlerType)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                if (key < 0)
                {
                    MouseButtons button = (MouseButtons)(-key);
                    switch (button)
                    {
                        case MouseButtons.left:
                            return "lmb";
                        case MouseButtons.right:
                            return "rmb";
                        case MouseButtons.thumb1:
                            return "mb1";
                        case MouseButtons.thumb2:
                            return "mb2";
                        case MouseButtons.wheel:
                            return "mb3";
                        default:
                            return "";
                    }
                }
                else
                {
                    return ((Keys)key).ToString();
                }
            }
            else
            {
                return ((Buttons)key).ToString();
            }
        }

        public static int KeyToInt(in Keys key) => (int)key;
        public static int KeyToInt(in MouseButtons mouseButton) => (int)mouseButton;
        public static int KeyToInt(in Buttons gamepadButton) => (int)gamepadButton;

        /// <summary>
        /// Convert an action into the string who define the control of the action, according to the controller.
        /// </summary>
        public static string KeyToString(string action, ControllerType controlerType, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            if(controlerType == ControllerType.Keyboard)
            {
                if(!kbController.ContainsKey(action))
                {
                    throw new InvalideInputActionExeption("keyboard", action);
                }
                return KeyToString(kbController[action], ControllerType.Keyboard);
            }
            else
            {
                switch (gamepadIndex)
                {
                    case GamepadIndex.One:
                        if (!gp1Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad one", action);
                        return KeyToString(gp1Controller[action], ControllerType.Gamepad);
                    case GamepadIndex.Two:
                        if (!gp2Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad two", action);
                        return KeyToString(gp2Controller[action], ControllerType.Gamepad);
                    case GamepadIndex.Three:
                        if (!gp3Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad three", action);
                        return KeyToString(gp3Controller[action], ControllerType.Gamepad);
                    case GamepadIndex.Four:
                        if (!gp4Controller.ContainsKey(action))
                            throw new InvalideInputActionExeption("gamepad four", action);
                        return KeyToString(gp3Controller[action], ControllerType.Gamepad);
                    case GamepadIndex.All:
                        if (gp1Controller.ContainsKey(action))
                            return KeyToString(gp1Controller[action], ControllerType.Gamepad);
                        if (gp2Controller.ContainsKey(action))
                            return KeyToString(gp2Controller[action], ControllerType.Gamepad);
                        if (!gp3Controller.ContainsKey(action))
                            return KeyToString(gp3Controller[action], ControllerType.Gamepad);
                        if (!gp4Controller.ContainsKey(action))
                            return KeyToString(gp4Controller[action], ControllerType.Gamepad);
                        throw new InvalideInputActionExeption("gamepad one to four", action);
                    default:
                        return "";
                }
            }
        }

        /// <param name="key"> the key pressed, castable to an Keys, MouseButton or Buttons according to the controler type</param>
        /// <param name="gamepadIndex"></param>
        /// <returns> true if a key of the controler is pressed this frame, false otherwise </returns>
        public static bool KeyPressed(ControllerType controlerType, out int key, GamepadIndex gamepadIndex = GamepadIndex.One)
        {
            if (controlerType == ControllerType.Keyboard)
            {
                for (int i = 0; i < 255; i++)
                {
                    if (GetKeyDown((Keys)i))
                    {
                        key = i;
                        return true;
                    }
                }
                for (int i = -1; i <= -5; i--)
                {
                    if (GetKeyDown((MouseButtons)i))
                    {
                        key = i;
                        return true;
                    }
                }
            }
            else
            {
                int index = 1;
                for (int i = 1; i <= 30; i++)
                {
                    if (GetKey((Buttons)i, gamepadIndex))
                    {
                        key = index;
                        return true;
                    }
                    index *= 2;
                }
            }

            key = 0;
            return false;
        }

        /// <param name="letter"> the letter pressed this frame</param>
        /// <returns>true if a key of the letter of the keyboard controller is pressed this frame, false otherwise</returns>
        public static bool LetterPressed(out string letter)
        {
            for (int i = 65; i <= 90; i++)
            {
                if (GetKeyDown((Keys)i))
                {
                    letter = letters[i - 65];
                    return true;
                }
            }
            for (int i = 96; i <= 105; i++)
            {
                if (GetKeyDown((Keys)i))
                {
                    letter = letters[i - 96 + 26];
                    return true;
                }
            }
            if (GetKeyDown(Keys.Space))
                letter = " ";
            letter = "";
            return false;
        }
        /// <param name="number"> the number pressed this frame</param>
        /// <returns>true if a key of the number of the keyboard controller is pressed this frame, false otherwise</returns>
        public static bool NumberPressed(out string number)
        {
            for (int i = 96; i <= 105; i++)
            {
                if (GetKeyDown((Keys)i))
                {
                    number = letters[i - 96 + 26];
                    return true;
                }
            }
            number = "";
            return false;
        }

        #endregion

        public static void Update()
        {
            oldKBState = newKBState;
            oldMouseState = newMouseState;
            oldGamePadStateOne = newGamePadStateOne;
            oldGamePadStateTwo = newGamePadStateTwo;
            oldGamePadStateThree = newGamePadStateThree;
            oldGamePadStateFour = newGamePadStateFour;

            newKBState = Keyboard.GetState();
            newMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            newGamePadStateOne = GamePad.GetState(PlayerIndex.One);
            newGamePadStateTwo = GamePad.GetState(PlayerIndex.Two);
            newGamePadStateThree = GamePad.GetState(PlayerIndex.Three);
            newGamePadStateFour = GamePad.GetState(PlayerIndex.Four);
        }

        [Serializable]
        private class InvalideInputActionExeption : Exception
        {
            public InvalideInputActionExeption(string dico, string action) : base("The action : '" + action + "' is not added to the " + dico +  " controler.")
            {

            }
        }
    }
}
