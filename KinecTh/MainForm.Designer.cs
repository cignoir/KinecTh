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
            this.mqoViewer = new GLSharp.GLSControl();
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mqoViewer
            // 
            this.mqoViewer.Location = new System.Drawing.Point(12, 12);
            this.mqoViewer.Name = "mqoViewer";
            this.mqoViewer.PixelFormat.AccumBits = ((byte)(64));
            this.mqoViewer.PixelFormat.AlphaBits = ((byte)(8));
            this.mqoViewer.PixelFormat.AuxBuffers = ((byte)(4));
            this.mqoViewer.PixelFormat.ColorBits = ((byte)(32));
            this.mqoViewer.PixelFormat.DepthBits = ((byte)(24));
            this.mqoViewer.PixelFormat.DoubleBuffer = true;
            this.mqoViewer.PixelFormat.PixelType = GLSharp.PfdPixelType.RGBA;
            this.mqoViewer.PixelFormat.StencilBits = ((byte)(0));
            this.mqoViewer.PixelFormatAttribute.Acceleration = GLSharp.PFAAccelerationType.FullAcceleration;
            this.mqoViewer.PixelFormatAttribute.SampleBuffers = 0;
            this.mqoViewer.PixelFormatAttribute.Samples = 0;
            this.mqoViewer.Size = new System.Drawing.Size(370, 389);
            this.mqoViewer.TabIndex = 0;
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.BackColor = System.Drawing.Color.Black;
            this.consoleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.consoleTextBox.ForeColor = System.Drawing.Color.Fuchsia;
            this.consoleTextBox.Location = new System.Drawing.Point(12, 422);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Size = new System.Drawing.Size(370, 306);
            this.consoleTextBox.TabIndex = 5;
            this.consoleTextBox.Text = "Running...\r\n";
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox.InitialImage")));
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(370, 373);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 6;
            this.pictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(394, 740);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.consoleTextBox);
            this.Controls.Add(this.mqoViewer);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(960, 0);
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "KinecTh";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public GLSharp.GLSControl mqoViewer;
        public System.Windows.Forms.TextBox consoleTextBox;
        public System.Windows.Forms.PictureBox pictureBox;
    }
}

