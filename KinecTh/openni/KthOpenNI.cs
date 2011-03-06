using System;
using System.Collections;
using System.Threading;
using xn;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using WindowsInput;
using KinecTh.openni;
using System.Drawing;
using System.Drawing.Imaging;

namespace KinecTh
{
    public class KthOpenNI
    {
        MainForm mainForm;

        Context context;
        DepthGenerator depthGenerator;
        UserGenerator userGenerator;
        PoseDetectionCapability poseDetectionCapability;
        SkeletonCapability skeletonCapability;

        int[] histogram;
        Bitmap bitmap;
        bool shouldRun;


        Thread openNiThread;

        public KthOpenNI(MainForm mainForm)
        {
            this.mainForm = mainForm;
            context = new Context(Application.StartupPath + @"\data\config.xml");
            depthGenerator = context.FindExistingNode(NodeType.Depth) as DepthGenerator;

            userGenerator = new UserGenerator(context);
            userGenerator.NewUser += new UserGenerator.NewUserHandler(userGenerator_NewUser);
            userGenerator.LostUser += new UserGenerator.LostUserHandler(userGenerator_LostUser);

            poseDetectionCapability = new PoseDetectionCapability(userGenerator);
            poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(poseDetectionCapability_PoseDetected);

            skeletonCapability = new SkeletonCapability(userGenerator);
            skeletonCapability.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(skeletonCapability_CalibrationEnd);
            skeletonCapability.SetSkeletonProfile(SkeletonProfile.All);
        }

        public void Reset()
        {
            Thread.CurrentThread.Abort();
            openNiThread = new Thread(new ThreadStart(ReaderThread));
            openNiThread.Start();
        }

        public void Start()
        {
           userGenerator.StartGenerating();
           histogram = new int[depthGenerator.GetDeviceMaxDepth()];
           MapOutputMode mapMode = depthGenerator.GetMapOutputMode();
           bitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes/*, System.Drawing.Imaging.PixelFormat.Format24bppRgb*/);
           shouldRun = true;

           openNiThread = new Thread(new ThreadStart(ReaderThread));
           openNiThread.Start();
        }

        public void Exit()
        {
            if(openNiThread != null && openNiThread.IsAlive){
                shouldRun = false;
                openNiThread.Abort();
                context.StopGeneratingAll();
                context.Shutdown();
            }
        }

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

        private Color[] colors = { Color.Red, Color.Blue, Color.ForestGreen, Color.Yellow, Color.Orange, Color.Purple, Color.White };
        private Color[] anticolors = { Color.Green, Color.Orange, Color.Red, Color.Purple, Color.Blue, Color.Yellow, Color.Black };
        private int ncolors = 6;

        private bool shouldDrawPixels = true;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldPrintState = true;
        private bool shouldDrawSkeleton = true;

        private unsafe void ReaderThread()
        {
            DepthMetaData depthMD = new DepthMetaData();

            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.depthGenerator);
                }
                catch (Exception)
                {
                }

                this.depthGenerator.GetMetaData(depthMD);

                CalcHist(depthMD);

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

                    //Graphics g = Graphics.FromImage(this.bitmap);
                    mainForm.pictureBox.Image = this.bitmap;
                    // ゲームI/F
                    Play();
                    //g.Dispose();
                }

                mainForm.Invalidate();
            }
        }

        //void Loop()
        //{
        //    while (shouldRun)
        //    {
        //        if (mainForm.IsDisposed)
        //        {
        //            break;
        //        }
                
        //        try
        //        {   
        //            // 深度センサー用ジェネレータの更新待ち
        //            this.context.WaitOneUpdateAll(depthGenerator);
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        // 検出済みユーザの取得
        //        var users = userGenerator.GetUsers();
        //        if(users.Length != 0){
        //            uint user = GetActiveUser(users);
                
        //            if (skeletonCapability.IsTracking(user))
        //            {
        //                // ユーザが切り替わっていたらイメージ変更
        //                //if (IsUserSwitched(user, users)) {;};
                        
        //                // オートショット
        //                if(Status.isKeyEnabled && Status.isAutoShot) KthAction.AutoShot();

        //                // 骨格情報取得
        //                var skeleton = GetSkeleton(user);
        //                // 骨格情報の信頼度チェック
        //                if (IsConfident(skeleton)) continue;
        //                // ポーズ判定＋入力
        //                JudgeAndAction(skeleton);
        //            }
        //        }
        //    }
        //}

        void Play()
        {
            // 検出済みユーザの取得
            var users = userGenerator.GetUsers();
            if (users.Length != 0)
            {
                uint user = GetActiveUser(users);

                if (skeletonCapability.IsTracking(user))
                {
                    // ユーザが切り替わっていたらイメージ変更
                    //if (IsUserSwitched(user, users)) {;};

                    // オートショット
                    if (Status.isKeyEnabled && Status.isAutoShot) KthAction.AutoShot();

                    // 骨格情報取得
                    var skeleton = GetSkeleton(user);
                    // 骨格情報の信頼度チェック
                    if (IsConfident(skeleton)) return;
                    // ポーズ判定＋入力
                    new Pose(skeleton).JudgeAndAction(skeleton);
                }
            }
        }

        uint GetActiveUser(uint[] users)
        {
            Status.activeUser = Status.activeUser > users.Length - 1 ? users.Length - 1 : Status.activeUser;
            return users[Status.activeUser];

        }

        bool IsUserSwitched(uint user, uint[] users)
        {
            bool isUserSwitched = user != Status.activeUser && Status.activeUser < users.Length;
            if (isUserSwitched)
            {
                //SwitchPictureBoxView();
            }
            return isUserSwitched;
        }


        void SwitchPictureBoxView()
        {
            if (mainForm.pictureBox.Visible)
            {
                mainForm.HidePic(mainForm.pictureBox);
            }
            else
            {
                mainForm.ShowPic(mainForm.pictureBox);
            }
        }

        Dictionary<SkeletonJoint, SkeletonJointPosition> GetSkeleton(uint user)
        {
            // 骨格情報取得
            var dic = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
            dic.Add(SkeletonJoint.Head, Joint(user, SkeletonJoint.Head));
            dic.Add(SkeletonJoint.Neck, Joint(user, SkeletonJoint.Neck));
            dic.Add(SkeletonJoint.RightShoulder, Joint(user, SkeletonJoint.RightShoulder));
            dic.Add(SkeletonJoint.RightElbow, Joint(user, SkeletonJoint.RightElbow));
            dic.Add(SkeletonJoint.RightHand, Joint(user, SkeletonJoint.RightHand));
            dic.Add(SkeletonJoint.LeftShoulder, Joint(user, SkeletonJoint.LeftShoulder));
            dic.Add(SkeletonJoint.LeftElbow, Joint(user, SkeletonJoint.LeftElbow));
            dic.Add(SkeletonJoint.LeftHand, Joint(user, SkeletonJoint.LeftHand));
            dic.Add(SkeletonJoint.Torso, Joint(user, SkeletonJoint.Torso));
            dic.Add(SkeletonJoint.RightHip, Joint(user, SkeletonJoint.RightHip));
            dic.Add(SkeletonJoint.RightKnee, Joint(user, SkeletonJoint.RightKnee));
            dic.Add(SkeletonJoint.RightFoot, Joint(user, SkeletonJoint.RightFoot));
            dic.Add(SkeletonJoint.LeftHip, Joint(user, SkeletonJoint.LeftHip));
            dic.Add(SkeletonJoint.LeftKnee, Joint(user, SkeletonJoint.LeftKnee));
            dic.Add(SkeletonJoint.LeftFoot, Joint(user, SkeletonJoint.LeftFoot));

            return dic;
        }

        private SkeletonJointPosition Joint(uint user, SkeletonJoint joint)
        {
            var pos = new SkeletonJointPosition();
            skeletonCapability.GetSkeletonJointPosition(user, joint, ref pos);
            if (pos.position.Z == 0)
            {
                pos.fConfidence = 0;
            }
            else
            {
                pos.position = depthGenerator.ConvertRealWorldToProjective(pos.position);
            }
            return pos;
        }

        private bool IsConfident(Dictionary<SkeletonJoint, SkeletonJointPosition> dic)
        {
            SkeletonJointPosition[] posArray = { dic[SkeletonJoint.Head], dic[SkeletonJoint.RightShoulder], dic[SkeletonJoint.RightHand], dic[SkeletonJoint.LeftHand] };
            foreach(SkeletonJointPosition p in posArray){
                if(p.fConfidence < 0.5){
                    return false;
                }
            }
            return true;
        }

        #region イベントハンドラ
        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            //新しいユーザを検出した時
            Console.WriteLine("New User {0}", id);
            mainForm.Log(mainForm.consoleTextBox, "New User " + id);
            //新しいユーザのポーズ検出を開始します
            poseDetectionCapability.StartPoseDetection(skeletonCapability.GetCalibrationPose(), id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            //ユーザをロストした時
            Console.WriteLine(String.Format("Lost User {0}", id));
            mainForm.Log(mainForm.consoleTextBox, "Lost User " + id);
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            //Ψを検出した時
            Console.WriteLine(String.Format("PoseDetected {1} {0}", id, pose));
            mainForm.Log(mainForm.consoleTextBox, "PoseDetected " + pose + " " + id);
            //新しいユーザのポーズ検出を終了し
            poseDetectionCapability.StopPoseDetection(id);
            //キャリブレーションを開始します
            skeletonCapability.RequestCalibration(id, true);
        }

        void skeletonCapability_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            //キャリブレーション完了した時
            Console.WriteLine(String.Format("CalibrationEnd {1} {0}", id, success));
            mainForm.Log(mainForm.consoleTextBox, "CalibrationEnd " + success + " " + id);
            if (success)
            {
                //成功したなら、トラッキング開始
                mainForm.HidePic(mainForm.pictureBox);
                Status.isUserTracking = true;
                skeletonCapability.StartTracking(id);
                mainForm.Log(mainForm.consoleTextBox, "Calibration Success" + id);
            }
            else
            {
                //失敗したなら、再度検出開始
                mainForm.ShowPic(mainForm.pictureBox);
                Status.isUserTracking = false;
                poseDetectionCapability.StartPoseDetection(skeletonCapability.GetCalibrationPose(), id);
            }
        }
        #endregion

       
    }
}
