using WindowsInput;

namespace KinecTh.common
{
    class GameAction
    {
        #region キー入力ラッパー
        public static void PressKey(VirtualKeyCode keyCode)
        {
            if (Status.isKeyEnabled)
            {
                InputSimulator.SimulateKeyPress(keyCode);
            }
        }

        public static void DownKey(VirtualKeyCode keyCode)
        {
            if (Status.isKeyEnabled && !InputSimulator.IsKeyDown(keyCode))
            {
                InputSimulator.SimulateKeyDown(keyCode);
            }
        }

        public static void UpKey(VirtualKeyCode keyCode)
        {
            if (Status.isKeyEnabled && InputSimulator.IsKeyDown(keyCode))
            {
                InputSimulator.SimulateKeyUp(keyCode);
            }
        }
        #endregion

        #region 十字キー操作
        public static void MoveRight()
        {
            UpKey(VirtualKeyCode.LEFT);
            DownKey(VirtualKeyCode.RIGHT);
        }

        public static void MoveLeft()
        {
            UpKey(VirtualKeyCode.RIGHT);
            DownKey(VirtualKeyCode.LEFT);
        }

        public static void MoveUp()
        {
            UpKey(VirtualKeyCode.DOWN);
            DownKey(VirtualKeyCode.UP);
        }

        public static void MoveDown()
        {
            UpKey(VirtualKeyCode.UP);
            DownKey(VirtualKeyCode.DOWN);
        }
        #endregion

        #region
        public static void AutoShot()
        {
            DownKey(VirtualKeyCode.VK_Z);
        }

        public static void Fast()
        {
            UpKey(VirtualKeyCode.SHIFT);
        }

        public static void Slow()
        {
            DownKey(VirtualKeyCode.SHIFT);
        }

        public static void Stay()
        {
            UpKey(VirtualKeyCode.UP);
            UpKey(VirtualKeyCode.DOWN);
            UpKey(VirtualKeyCode.RIGHT);
            UpKey(VirtualKeyCode.LEFT);
        }

        public static void Bomb()
        {
            PressKey(VirtualKeyCode.VK_X);
        }

        public static void ReleaseAll()
        {
            Stay();
            UpKey(VirtualKeyCode.SHIFT);
            UpKey(VirtualKeyCode.VK_X);
            UpKey(VirtualKeyCode.VK_Z);

        }
        #endregion
    }
}
