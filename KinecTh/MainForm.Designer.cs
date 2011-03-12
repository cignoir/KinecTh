namespace KinecTh
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mqoView = new GLSharp.GLSControl();
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.histView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.histView)).BeginInit();
            this.SuspendLayout();
            // 
            // mqoView
            // 
            this.mqoView.Location = new System.Drawing.Point(12, 12);
            this.mqoView.Name = "mqoView";
            this.mqoView.PixelFormat.AccumBits = ((byte)(64));
            this.mqoView.PixelFormat.AlphaBits = ((byte)(8));
            this.mqoView.PixelFormat.AuxBuffers = ((byte)(4));
            this.mqoView.PixelFormat.ColorBits = ((byte)(32));
            this.mqoView.PixelFormat.DepthBits = ((byte)(24));
            this.mqoView.PixelFormat.DoubleBuffer = true;
            this.mqoView.PixelFormat.PixelType = GLSharp.PfdPixelType.RGBA;
            this.mqoView.PixelFormat.StencilBits = ((byte)(0));
            this.mqoView.PixelFormatAttribute.Acceleration = GLSharp.PFAAccelerationType.FullAcceleration;
            this.mqoView.PixelFormatAttribute.SampleBuffers = 0;
            this.mqoView.PixelFormatAttribute.Samples = 0;
            this.mqoView.Size = new System.Drawing.Size(370, 292);
            this.mqoView.TabIndex = 0;
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.BackColor = System.Drawing.Color.Black;
            this.consoleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.consoleTextBox.ForeColor = System.Drawing.Color.Fuchsia;
            this.consoleTextBox.Location = new System.Drawing.Point(12, 643);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Size = new System.Drawing.Size(370, 85);
            this.consoleTextBox.TabIndex = 5;
            this.consoleTextBox.Text = "Running...\r\n";
            // 
            // histView
            // 
            this.histView.InitialImage = null;
            this.histView.Location = new System.Drawing.Point(12, 310);
            this.histView.Name = "histView";
            this.histView.Size = new System.Drawing.Size(370, 327);
            this.histView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.histView.TabIndex = 6;
            this.histView.TabStop = false;
            this.histView.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(394, 740);
            this.Controls.Add(this.histView);
            this.Controls.Add(this.consoleTextBox);
            this.Controls.Add(this.mqoView);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(960, 0);
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "KinecTh";
            ((System.ComponentModel.ISupportInitialize)(this.histView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public GLSharp.GLSControl mqoView;
        public System.Windows.Forms.TextBox consoleTextBox;
        public System.Windows.Forms.PictureBox histView;
    }
}

