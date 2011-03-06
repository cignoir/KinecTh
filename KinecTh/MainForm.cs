using System;
using System.Drawing;
using System.Windows.Forms;
using GLSharp;
using KinecTh.common;
using System.Threading;
using KinecTh.openni;

namespace KinecTh
{
    public partial class MainForm : Form
    {
        KthOpenNI openNi;
        HotKeyControler hkManager;

        public MainForm()
        {
            InitializeComponent();

            Logger logger = new Logger(this.consoleTextBox);

            hkManager = new HotKeyControler(this);
            hkManager.RegistHotKey();

            this.mqoViewer.RenderingScene = new MQOViewer();
            this.mqoViewer.Refresh();

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
            this.Invoke(showPicDlg, new object[] { pictureBox });
        }
        #endregion


        ~MainForm()
        {
            hkManager.UnRegistHotKey();
            if (openNi != null)
            {
                openNi.Exit();
            }
        }

    }
}
