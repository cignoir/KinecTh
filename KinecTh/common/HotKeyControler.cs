using System;
using System.Windows.Forms;
using WindowsInput;

namespace KinecTh.common
{
    class HotKeyControler
    {
        HotKey hotKey_K; // キー入力の受付ON/OFF
        HotKey hotKey_Z; // Zキー入力
        HotKey hotKey_A; // AUTO SHOT ON/OFF
        HotKey hotKey_I; // 初期化
        HotKey hotKey_D; // ログ消去
        HotKey[] hotKey_User = new HotKey[Settings.MAX_USER_COUNT];
        
        private MainForm mainForm;

        public HotKeyControler(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        // イベントハンドラ
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            if(sender == hotKey_K){
                InputSimulator.SimulateKeyUp(VirtualKeyCode.UP);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.DOWN);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.LEFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.RIGHT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.SHIFT);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_X);
                Status.isKeyEnabled = !Status.isKeyEnabled;
                if (Status.isKeyEnabled)
                {
                    Logger.log(mainForm.consoleTextBox, "[Key Input] - ON");
                    AutoShot();
                }
                else
                {
                    Logger.log(mainForm.consoleTextBox, "[Key Input] - OFF");
                }
            } else if(sender == hotKey_Z){
                InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_Z);
                Status.isAutoShot = !Status.isAutoShot;
                AutoShot();
                Logger.log(mainForm.consoleTextBox, "Press Z key");
            } else if(sender == hotKey_A){
                InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                Status.isAutoShot = !Status.isAutoShot;
                AutoShot();
                if (Status.isAutoShot)
                {
                    Logger.log(mainForm.consoleTextBox, "[Auto Shot] - ON");
                }
                else
                {
                    Logger.log(mainForm.consoleTextBox, "[Auto Shot] - OFF");
                }

            }
            else if (sender == hotKey_I)
            {
                mainForm.consoleTextBox.Text = "Initialized";
                mainForm.consoleTextBox.ScrollToCaret();
                var openNi = mainForm.GetOpenNI();
                if(openNi != null){
                    openNi.Reset();
                }
            }
            else if (sender == hotKey_D)
            {
                mainForm.consoleTextBox.Text = "";
                mainForm.consoleTextBox.ScrollToCaret();
            }
            else if (IsUserChanged(sender))
            {
                Logger.log(mainForm.consoleTextBox, "[Active] - User" + Status.activeUser);
            }
            else
            {
            }
        }

        void AutoShot()
        {
            if (Status.isAutoShot && Status.isKeyEnabled && Status.isUserTracking)
            {
                InputSimulator.SimulateKeyDown(VirtualKeyCode.VK_Z);
            }
            else
            {
                InputSimulator.SimulateKeyUp(VirtualKeyCode.VK_Z);
            }
        }

        bool IsUserChanged(object sender)
        {
            for (int i = 0; i < Settings.MAX_USER_COUNT; i++ )
            {
                if(sender == hotKey_User[i]){
                    Status.activeUser = i;
                    return true;
                }
            }
            return false;
        }

        public void RegistHotKey()
        {
            // キーインプットのON/OFF
            hotKey_K = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.K);
            hotKey_K.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            // Z キー押すだけ
            hotKey_Z = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.Z);
            hotKey_Z.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            // オートショットON/OFF
            hotKey_A = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.A);
            hotKey_A.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            // 初期化
            hotKey_I = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.I);
            hotKey_I.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            // テキストボックスのログ消去
            hotKey_D = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D);
            hotKey_D.HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            // ユーザ切り替え用
            hotKey_User[0] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D0);
            hotKey_User[1] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D1);
            hotKey_User[2] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D2);
            hotKey_User[3] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D3);
            hotKey_User[4] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D4);
            hotKey_User[5] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D5);
            hotKey_User[6] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D6);
            hotKey_User[7] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D7);
            hotKey_User[8] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D8);
            hotKey_User[9] = new HotKey(MOD_KEY.CONTROL | MOD_KEY.ALT | MOD_KEY.NOREPEAT, Keys.D9);
            for (int i = 0; i < Settings.MAX_USER_COUNT; i++ )
            {
                hotKey_User[i].HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            }
        }

        public void UnRegistHotKey()
        {
            hotKey_K.Dispose();
            hotKey_Z.Dispose();
            hotKey_A.Dispose();
            hotKey_I.Dispose();
            hotKey_D.Dispose();
            for (int i = 0; i < Settings.MAX_USER_COUNT; i++ )
            {
                hotKey_User[i].Dispose();
            }            
        }
        
    }

    public enum MOD_KEY : uint
    {
        ALT = 0x0001,
        CONTROL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x8,
        NOREPEAT = 0x4000
    }
}
