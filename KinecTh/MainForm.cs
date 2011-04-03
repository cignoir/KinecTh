using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using KinecTh.common;
using WindowsInput;
using xn;

namespace KinecTh
{
    public partial class MainForm : Form
    {
        #region ホットキー関連
        HotKey hotKey_K; // キー入力の受付ON/OFF
        HotKey hotKey_Z; // Zキー入力
        HotKey hotKey_A; // AUTO SHOT ON/OFF
        HotKey hotKey_I; // 初期化
        HotKey hotKey_D; // ログ消去
        HotKey[] hotKey_User = new HotKey[Settings.MAX_USER_COUNT];
        #endregion

        Thread openNiThread;
        Context context;
        DepthGenerator depthGenerator;
        UserGenerator userGenerator;
        ImageGenerator imageGenerator;
        PoseDetectionCapability poseDetectionCapability;
        SkeletonCapability skeletonCapability;

        Color[] colors = { Color.Red, Color.Blue, Color.ForestGreen, Color.Yellow, Color.Orange, Color.Purple, Color.White };
        Color[] anticolors = { Color.Green, Color.Orange, Color.Red, Color.Purple, Color.Blue, Color.Yellow, Color.Black };
        int ncolors = 6;

        bool shouldDrawPixels = true;
        bool shouldDrawBackground = true;
        bool shouldRun;

        MapOutputMode mapMode;
        int[] histogram;

        Bitmap bitmap;
        Bitmap bg;
        Bitmap loading;
        MQOViewer mqo;

        public MainForm()
        {
            InitializeComponent();

            // 背景画像読み込み
            this.bg = new Bitmap(System.IO.Directory.GetCurrentDirectory() + Settings.IMGPATH_BG);
            
            // 少女認識中画像
            //this.loading = new Bitmap(System.IO.Directory.GetCurrentDirectory() + Settings.IMGPATH_LOADING);

            // MQOビューア
            //SetMQO(new MQOViewer());

            // ウィンドウ調整
            InitBounds();

            // HotKey登録
            RegistHotKey();

            // OpenNI thread起動
            StartOpenNI();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.shouldRun = false;
            this.openNiThread.Join();
            UnRegistHotKey();
            base.OnClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            lock (this)
            {
                e.Graphics.DrawImage(this.bitmap,
                    this.panelView.Location.X,
                    this.panelView.Location.Y,
                    this.panelView.Size.Width,
                    this.panelView.Size.Height);
            }
        }

        /// <summary>
        /// OpenNIスレッドの起動
        /// 
        /// </summary>
        private void StartOpenNI()
        {
            try
            {
                context = new Context(Application.StartupPath + Settings.OPENNI_CONFIG);
            } catch(GeneralException ge){
                Console.WriteLine(ge.Message);
                this.Close();
                return;
            }
            depthGenerator = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.depthGenerator == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }

            userGenerator = new UserGenerator(context);
            userGenerator.NewUser += new UserGenerator.NewUserHandler(userGenerator_NewUser);
            userGenerator.LostUser += new UserGenerator.LostUserHandler(userGenerator_LostUser);
            userGenerator.StartGenerating();

            imageGenerator = context.FindExistingNode(NodeType.Image) as ImageGenerator;
            imageGenerator.StartGenerating();

            poseDetectionCapability = new PoseDetectionCapability(userGenerator);
            poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(poseDetectionCapability_PoseDetected);

            skeletonCapability = new SkeletonCapability(userGenerator);
            skeletonCapability.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(skeletonCapability_CalibrationEnd);
            skeletonCapability.SetSkeletonProfile(SkeletonProfile.All);

            depthGenerator.GetAlternativeViewPointCap().SetViewPoint(imageGenerator);

            histogram = new int[depthGenerator.GetDeviceMaxDepth()];

            // 出力モード
            //this.mapMode = depthGenerator.GetMapOutputMode();
            this.mapMode = imageGenerator.GetMapOutputMode();

            bitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes/*, System.Drawing.Imaging.PixelFormat.Format24bppRgb*/);
            shouldRun = true;
            openNiThread = new Thread(ReaderThread);
            openNiThread.Start();
        }
        
        /// <summary>
        /// コンポーネントの位置・大きさ調整
        /// 
        /// </summary>
        private void InitBounds()
        {
            var hWnd = Win32.FindWindow(Settings.TARGET_PROCESS);
            if (hWnd != IntPtr.Zero)
            {
                Win32.RECT rect;
                Win32.GetWindowRect(hWnd, out rect);

                Rectangle r = Screen.PrimaryScreen.Bounds;
                var width = r.Width - rect.right;
                var height = rect.bottom - rect.top;
                if(width == 0 && height == 0){
                    return;
                }
                this.SetBounds(rect.right, rect.top, width, height);

                var panelHeight = height / 2;
                var panelWidth = panelHeight * 4 / 3;
                if(true || panelWidth > width){
                    panelWidth = width;
                }

                var pad = 10;
                panelWidth -= pad * 2;
                panelHeight -= pad * 2;
                this.panelView.SetBounds(pad, pad, panelWidth, panelHeight);

                if (this.mqoView.Visible)
                {
                    this.mqoView.SetBounds(this.panelView.Location.X, this.panelView.Location.Y + this.panelView.Height, width / 2, height / 2 - pad * 3);
                    this.consoleTextBox.SetBounds(this.mqoView.Location.X + this.mqoView.Width, this.mqoView.Location.Y, this.mqoView.Width, this.mqoView.Height);
                }
                else
                {
                    this.consoleTextBox.SetBounds(this.panelView.Location.X, this.panelView.Location.Y + this.panelView.Height, panelWidth, height / 2 - pad * 3);
                }
            }
        }

        /// <summary>
        /// MQO Viewer の設定
        /// </summary>
        /// <param name="mqo">MQOモデルのファイルパス</param>
        private void SetMQO(MQOViewer mqo){
            this.mqoView.RenderingScene = mqo;
            this.mqoView.Refresh();
            this.mqoView.Visible = true;
        }

        /// <summary>
        /// OpenNI 描画スレッド
        /// </summary>
        private unsafe void ReaderThread()
        {
            if (this.shouldRun == false) return;

            DepthMetaData depthMD = new DepthMetaData();

            // 背景画像のリサイズ
            this.bg = KinecThUtils.ResizeBmp(this.bg, this.bitmap.Width, this.bitmap.Height);
            //this.loading = KthUtils.ResizeBmp(this.loading, (int)(this.bitmap.Width * 0.4), (int)(this.bitmap.Height * 0.4));

            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.depthGenerator);
                } catch(Exception e){
                }

                // 深度マップ描画
                //this.depthGenerator.GetMetaData(depthMD);
                //CalcHist(depthMD);
                //DrawHist(depthMD);

                // カメラ画像描画
                var imageMD = this.imageGenerator.GetMetaData();
                if (imageGenerator.IsDataNew())
                {
                    DrawImage(imageMD);
                }

                // 検出済みユーザの取得
                var users = userGenerator.GetUsers();
                if (users.Length != 0)
                {
                    uint user = KinecThUtils.GetActiveUser(users);

                    if (skeletonCapability.IsTracking(user))
                    {
                        // ユーザが切り替わっていたらイメージ変更
                        //if (IsUserSwitched(user, users)) {;};

                        // オートショット
                        if (Status.isKeyEnabled && Status.isAutoShot) GameAction.AutoShot();

                        // 骨格情報取得
                        var skeleton = KinecThUtils.GetSkeleton(skeletonCapability, depthGenerator, user);
                        // 骨格情報の信頼度チェック
                        if (KinecThUtils.IsConfident(skeleton)) return;
                        // ポーズ判定＋入力
                        new Pose(skeleton).JudgeAndAction(skeleton);
                    }
                }

            }
            
        }

        /// <summary>
        /// 深度マップの算出
        /// </summary>
        /// <param name="depthMD">DepthMetaData</param>
        private unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        this.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }

        /// <summary>
        /// 深度マップの描画
        /// </summary>
        /// <param name="depthMD">DepthMetaData</param>
        private unsafe void DrawHist(DepthMetaData depthMD)
        {
            lock (this)
            {
                Rectangle rect = new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height);
                BitmapData data = this.bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                if (this.shouldDrawPixels)
                {
                    ushort* pDepth = (ushort*)this.depthGenerator.GetDepthMapPtr().ToPointer();
                    ushort* pLabels = (ushort*)this.userGenerator.GetUserPixels(0).SceneMapPtr.ToPointer();

                    // set pixels
                    for (int y = 0; y < depthMD.YRes; ++y)
                    {
                        byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                        for (int x = 0; x < depthMD.XRes; ++x, ++pDepth, ++pLabels, pDest += 3)
                        {
                            pDest[0] = pDest[1] = pDest[2] = 0;

                            ushort label = *pLabels;
                            if (this.shouldDrawBackground || *pLabels != 0)
                            {
                                Color labelColor = Color.White;
                                if (label != 0)
                                {
                                    labelColor = colors[label % ncolors];
                                }

                                byte pixel = (byte)this.histogram[*pDepth];
                                pDest[0] = (byte)(pixel * (labelColor.B / 256.0));
                                pDest[1] = (byte)(pixel * (labelColor.G / 256.0));
                                pDest[2] = (byte)(pixel * (labelColor.R / 256.0));
                            }
                        }
                    }
                }
                this.bitmap.UnlockBits(data);
                this.Invalidate();
            }
        }

        /// <summary>
        /// カメラ画像の描画
        /// </summary>
        /// <param name="imageMD">ImageMetaData</param>
        private unsafe void DrawImage(ImageMetaData imageMD)
        {
            lock (this)
            {
                Rectangle rect = new Rectangle(0, 0, this.bitmap.Width, this.bitmap.Height);
                BitmapData data = this.bitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Rectangle bgRect = new Rectangle(0, 0, this.bg.Width, this.bg.Height);
                BitmapData bgData = this.bg.LockBits(bgRect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                byte* pSrc = (byte*)imageGenerator.GetImageMapPtr().ToPointer();

                ushort* pLabels = (ushort*)this.userGenerator.GetUserPixels(0).SceneMapPtr.ToPointer();
                int userCount = userGenerator.GetUsers().Length;

                for (int y = 0; y < imageMD.YRes; y++)
                {
                    byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                    byte* pBg = (byte*)bgData.Scan0.ToPointer() + y * bgData.Stride;

                    for (int x = 0; x < imageMD.XRes; x++, pSrc += 3, pDest += 3, pBg += 3, pLabels++)
                    {
                        if (*pLabels != 0 || userCount == 0 || skeletonCapability.IsTracking((uint)Status.activeUser) == false)
                        {
                            pDest[0] = pSrc[2];
                            pDest[1] = pSrc[1];
                            pDest[2] = pSrc[0];
                        }
                        else
                        {
                            // トラッキング中は背景画像を合成
                            pDest[0] = *pBg;
                            pDest[1] = *(pBg + 1);
                            pDest[2] = *(pBg + 2);
                        }
                    }
                }
                this.bitmap.UnlockBits(data);
                this.bg.UnlockBits(bgData);
            }
            this.Invalidate();
        }

        #region イベントハンドラ
        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            //新しいユーザを検出した時
            Log(String.Format("New User {0}", id));
            //新しいユーザのポーズ検出を開始します
            poseDetectionCapability.StartPoseDetection(skeletonCapability.GetCalibrationPose(), id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            //ユーザをロストした時
            Log(String.Format("Lost User {0}", id));
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            //Ψを検出した時
            Log(String.Format("PoseDetected {1} {0}", id, pose));
            //新しいユーザのポーズ検出を終了し
            poseDetectionCapability.StopPoseDetection(id);
            //キャリブレーションを開始します
            skeletonCapability.RequestCalibration(id, true);
        }

        void skeletonCapability_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            //キャリブレーション完了した時
            Log(String.Format("CalibrationEnd {1} {0}", id, success));
            if (success)
            {
                //成功したなら、トラッキング開始
                Status.isUserTracking = true;
                skeletonCapability.StartTracking(id);
                Log("Calibration Success" + id);
            }
            else
            {
                //失敗したなら、再度検出開始
                Status.isUserTracking = false;
                poseDetectionCapability.StartPoseDetection(skeletonCapability.GetCalibrationPose(), id);
            }
        }
        #endregion

        #region ホットキー関連
        void hotKey_HotKeyPush(object sender, EventArgs e)
        {
            if (sender == hotKey_K)
            {
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
                    Log("[Key Input] - ON");
                    GameAction.AutoShot();
                }
                else
                {
                    Log("[Key Input] - OFF");
                }
            }
            else if (sender == hotKey_Z)
            {
                InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_Z);
                Status.isAutoShot = !Status.isAutoShot;
                GameAction.AutoShot();
                Log("Press Z key");
            }
            else if (sender == hotKey_A)
            {
                InputSimulator.SimulateKeyUp(VirtualKeyCode.MENU);
                InputSimulator.SimulateKeyUp(VirtualKeyCode.CONTROL);
                Status.isAutoShot = !Status.isAutoShot;
                GameAction.AutoShot();
                if (Status.isAutoShot)
                {
                    Log("[Auto Shot] - ON");
                }
                else
                {
                    Log("[Auto Shot] - OFF");
                }

            }
            else if (sender == hotKey_I)
            {
                this.consoleTextBox.Text = "Initialized";
                this.consoleTextBox.ScrollToCaret();
            }
            else if (sender == hotKey_D)
            {
                this.consoleTextBox.Text = "";
                this.consoleTextBox.ScrollToCaret();
            }
            else
            {
            }
        }

        /// <summary>
        /// ホットキー登録
        /// </summary>
        public unsafe void RegistHotKey()
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
            for (int i = 0; i < Settings.MAX_USER_COUNT; i++)
            {
                hotKey_User[i].HotKeyPush += new EventHandler(hotKey_HotKeyPush);
            }
        }

        /// <summary>
        /// ホットキー解除
        /// </summary>
        public void UnRegistHotKey()
        {
            hotKey_K.Dispose();
            hotKey_Z.Dispose();
            hotKey_A.Dispose();
            hotKey_I.Dispose();
            hotKey_D.Dispose();
            for (int i = 0; i < Settings.MAX_USER_COUNT; i++)
            {
                hotKey_User[i].Dispose();
            }
        }
        #endregion

        /// <summary>
        /// textbox表示用ロガー
        /// </summary>
        /// <param name="msg">ログメッセージ</param>
        public void Log(string msg)
        {
            if (this.consoleTextBox != null)
            {
                try
                {
                    Regex r = new Regex(".*" + msg + @"[.]*");
                    if (r.Match(this.consoleTextBox.Text).Success)
                    {
                        this.consoleTextBox.AppendText(".");
                    }
                    else
                    {
                        this.consoleTextBox.AppendText("\r\n" + msg);
                    }
                    this.consoleTextBox.ScrollToCaret();
                    Console.WriteLine(msg);
                } catch(InvalidOperationException e){
                    Console.WriteLine(e.StackTrace);
                }
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
