using System.Collections.Generic;
using xn;
using System;
using KinecTh.common;

namespace KinecTh.openni
{
    class Pose
    {
        Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton;
        public Fulcrum fulcrum;
        public Point3D head;
        public Point3D neck;
        public Point3D torso;
        public Point3D rightHand;
        public Point3D rightElbow;
        public Point3D rightShoulder;
        public Point3D leftHand;
        public Point3D leftElbow;
        public Point3D leftShoulder;

        public Pose(Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            this.skeleton = skeleton;
            this.head = this.skeleton[SkeletonJoint.Head].position;
            this.neck = this.skeleton[SkeletonJoint.Neck].position;
            this.rightHand = this.skeleton[SkeletonJoint.RightHand].position;
            this.rightElbow = this.skeleton[SkeletonJoint.RightElbow].position;
            this.rightShoulder = this.skeleton[SkeletonJoint.RightShoulder].position;
            this.leftHand = this.skeleton[SkeletonJoint.LeftHand].position;
            this.leftElbow = this.skeleton[SkeletonJoint.LeftElbow].position;
            this.leftShoulder = this.skeleton[SkeletonJoint.LeftShoulder].position;
            this.torso = this.skeleton[SkeletonJoint.Torso].position;
            this.fulcrum = new Fulcrum(this.rightShoulder.X, this.rightShoulder.Y, this.rightShoulder.Z);
        }

        public void SetSkeleton(Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            this.skeleton = skeleton;
            
        }

        public Dictionary<SkeletonJoint, SkeletonJointPosition> GetSkeleton()
        {
            return skeleton;
        }
        
        #region ポーズ判定用
        private bool IsNeutral()
        {
            return !IsUp() && !IsDown() && !IsRight() && !IsLeft();
        }

        private bool IsSlow()
        {
            return 
                leftHand.X < leftElbow.X
                && leftElbow.X < torso.X
                && leftHand.Y < torso.Y
                && leftHand.Y > head.Y;
        }

        private bool IsBomb()
        {
            return rightHand.X > rightElbow.X && leftHand.X < leftElbow.X
                && rightHand.Y < head.Y && leftHand.Y < head.Y;
        }

        #region 上下左右判定
        private bool IsUp()
        {
            return rightHand.Y < fulcrum.Y - Settings.NEUTRAL_MARGIN;
        }

        private bool IsDown()
        {
            return rightHand.Y > fulcrum.Y + Settings.NEUTRAL_MARGIN;
        }

        private bool IsLeft()
        {
            return rightHand.X < fulcrum.X - Settings.NEUTRAL_MARGIN;
        }

        private bool IsRight()
        {
            return rightHand.X > fulcrum.X + Settings.NEUTRAL_MARGIN;
        }
        #endregion

        #endregion

        // ポーズ判定＋入力
        public void JudgeAndAction(Dictionary<SkeletonJoint, SkeletonJointPosition> skeleton)
        {
            SetSkeleton(skeleton);

            // ポーズ判定
            // 低速判定
            if (IsSlow())
            {
                KthAction.Slow();
            }
            else
            {
                KthAction.Fast();
            }

            if (IsNeutral())
            {
                // ニュートラル
                KthAction.Stay();
            }
            else
            {
                // 横軸
                if (IsRight())
                {
                    KthAction.MoveRight();
                }
                else if (IsLeft())
                {
                    KthAction.MoveLeft();
                }
                else
                {
                    // do nothing
                }

                // 縦軸
                if (IsUp())
                {
                    KthAction.MoveUp();
                }
                else if (IsDown())
                {
                    KthAction.MoveDown();
                }
                else
                {
                    // do nothing
                }
            }
            // ボム
            if (IsBomb())
            {
                KthAction.Bomb();
            }
        }

    }

    #region 支点クラス
    class Fulcrum
    {
        public float X;
        public float Y;
        public float Z;

        public Fulcrum(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        //public Fulcrum GetFulcrum(){
        //    float diff = Math.Abs(pose.head.Y - pose.neck.Y);
        //    //Fulcrum f = new Fulcrum(this.head.X + diff, this.leftShoulder.Y, this.leftShoulder.Z);
        //    Fulcrum f 
        //    return f;
        //}
    }
    #endregion
}
