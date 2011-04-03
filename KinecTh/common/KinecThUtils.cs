using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using xn;

namespace KinecTh.common
{
    class KinecThUtils
    {
        // 画像合成
        public static Bitmap MixImages(Bitmap original, Bitmap mix, int x, int y, int width, int height)
        {
            Bitmap mixed = new Bitmap(original);
            Graphics g = Graphics.FromImage(mixed);
            g.DrawImage(mix, x, y, width, height);
            g.Dispose();
            return mixed;
        }

        public static uint GetActiveUser(uint[] users)
        {
            Status.activeUser = Status.activeUser > users.Length - 1 ? users.Length - 1 : Status.activeUser;
            return users[Status.activeUser];
        }

        public static bool IsUserSwitched(uint user, uint[] users)
        {
            bool isUserSwitched = user != Status.activeUser && Status.activeUser < users.Length;
            if (isUserSwitched)
            {
                //SwitchPictureBoxView();
            }
            return isUserSwitched;
        }

        private static SkeletonJointPosition Joint(SkeletonCapability skeletonCapability, DepthGenerator depthGenerator, uint user, SkeletonJoint joint)
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

        public static Dictionary<SkeletonJoint, SkeletonJointPosition> GetSkeleton(SkeletonCapability skeletonCapability, DepthGenerator depthGenerator, uint user)
        {
            // 骨格情報取得
            var dic = new Dictionary<SkeletonJoint, SkeletonJointPosition>();
            dic.Add(SkeletonJoint.Head, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.Head));
            dic.Add(SkeletonJoint.Neck, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.Neck));
            dic.Add(SkeletonJoint.RightShoulder, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightShoulder));
            dic.Add(SkeletonJoint.RightElbow, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightElbow));
            dic.Add(SkeletonJoint.RightHand, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightHand));
            dic.Add(SkeletonJoint.LeftShoulder, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftShoulder));
            dic.Add(SkeletonJoint.LeftElbow, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftElbow));
            dic.Add(SkeletonJoint.LeftHand, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftHand));
            dic.Add(SkeletonJoint.Torso, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.Torso));
            dic.Add(SkeletonJoint.RightHip, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightHip));
            dic.Add(SkeletonJoint.RightKnee, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightKnee));
            dic.Add(SkeletonJoint.RightFoot, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.RightFoot));
            dic.Add(SkeletonJoint.LeftHip, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftHip));
            dic.Add(SkeletonJoint.LeftKnee, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftKnee));
            dic.Add(SkeletonJoint.LeftFoot, Joint(skeletonCapability, depthGenerator, user, SkeletonJoint.LeftFoot));

            return dic;
        }

        

        public static bool IsConfident(Dictionary<SkeletonJoint, SkeletonJointPosition> dic)
        {
            SkeletonJointPosition[] posArray = { dic[SkeletonJoint.Head], dic[SkeletonJoint.RightShoulder], dic[SkeletonJoint.RightHand], dic[SkeletonJoint.LeftHand] };
            foreach (SkeletonJointPosition p in posArray)
            {
                if (p.fConfidence < 0.5)
                {
                    return false;
                }
            }
            return true;
        }

        public static byte[] GetPixels(Bitmap bitmap)
        {
            // BitmapDataの参照を取得
            BitmapData bitmapData =
                    bitmap.LockBits(
                        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadWrite,
                        bitmap.PixelFormat);
            // bitmapDataポインタ取得
            IntPtr bitmapPtr = bitmapData.Scan0;

            byte[] data = new byte[bitmap.Width * bitmap.Height * 3];

            // MarshalクラスのCopyメソッドで配列にコピー
            Marshal.Copy(bitmapPtr, data, 0, bitmap.Width * bitmap.Height * 3);

            bitmap.UnlockBits(bitmapData);
            return data;
        }

        public static Bitmap ResizeBmp(Bitmap bitmap, int width, int height)
        {
            var resized = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(resized);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap, 0, 0, width, height);
            g.Dispose();
            return resized;
        }
    }
}
