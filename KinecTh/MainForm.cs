using System;
using System.Drawing;
using System.Windows.Forms;
using GLSharp;
using KinecTh.common;
using System.Threading;
using KinecTh.openni;
using System.ComponentModel;

namespace KinecTh
{
    public partial class MainForm : Form
    {
        KthOpenNI openNi;
        HotKeyControler hkManager;
        static Bitmap bitmap;

        public MainForm()
        {
            InitializeComponent();

            Logger logger = new Logger(this.consoleTextBox);

            hkManager = new HotKeyControler(this);
            hkManager.RegistHotKey();

            //this.mqoView.RenderingScene = new MQOViewer();
            //this.mqoView.Refresh();

            openNi = new KthOpenNI(this);
            openNi.Start();
        }

        public KthOpenNI GetOpenNI()
        {
            return openNi;
        }

        #region
        delegate void WriteLogDelegate(TextBox textBox, string str);
        WriteLogDelegate logDlg = (TextBox textBox, string str) => { Logger.log(textBox, str); };
        public void Log(TextBox textBox, string str)
        {
            this.Invoke(logDlg, new object[]{textBox, str});
        }

        delegate void HidePictureDelegate(PictureBox pictureBox);
        HidePictureDelegate hidePicDlg = (PictureBox pictureBox) => { pictureBox.Visible = false; };
        public void HidePic(PictureBox pictureBox)
        {
            //this.Invoke(hidePicDlg, new  object[]{pictureBox});
        }

        delegate void ShowPictureDelegate(PictureBox pictureBox);
        ShowPictureDelegate showPicDlg = (PictureBox pictureBox) => { pictureBox.Visible =true; };
        public void ShowPic(PictureBox pictureBox)
        {
            //this.Invoke(showPicDlg, new object[] { pictureBox });
        }

        delegate void UpdateBitmapDelegate(Bitmap bitmap);
        UpdateBitmapDelegate updateBitmap = (Bitmap bitmap) => { MainForm.bitmap = bitmap; };
        public void UpdateBitmap(Bitmap bitmap)
        {
            try
            {
                this.Invoke(updateBitmap, new object[] { bitmap });
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //openNi.Exit();
            hkManager.UnRegistHotKey();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.DrawImage(MainForm.bitmap,
                    0,
                    0,
                    histView.Size.Width,
                    histView.Size.Height);
        }

    }
}
