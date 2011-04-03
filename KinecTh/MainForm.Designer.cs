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
            this.consoleTextBox = new System.Windows.Forms.TextBox();
            this.panelView = new System.Windows.Forms.Panel();
            this.mqoView = new GLSharp.GLSControl();
            this.SuspendLayout();
            // 
            // consoleTextBox
            // 
            this.consoleTextBox.BackColor = System.Drawing.Color.Black;
            this.consoleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.consoleTextBox.ForeColor = System.Drawing.Color.Fuchsia;
            this.consoleTextBox.Location = new System.Drawing.Point(194, 372);
            this.consoleTextBox.Multiline = true;
            this.consoleTextBox.Name = "consoleTextBox";
            this.consoleTextBox.Size = new System.Drawing.Size(188, 356);
            this.consoleTextBox.TabIndex = 5;
            this.consoleTextBox.Text = "Running...\r\n";
            // 
            // panelView
            // 
            this.panelView.Location = new System.Drawing.Point(6, 3);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(385, 363);
            this.panelView.TabIndex = 6;
            this.panelView.Visible = false;
            // 
            // mqoView
            // 
            this.mqoView.Location = new System.Drawing.Point(12, 372);
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
            this.mqoView.Size = new System.Drawing.Size(176, 356);
            this.mqoView.TabIndex = 8;
            this.mqoView.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(394, 740);
            this.Controls.Add(this.mqoView);
            this.Controls.Add(this.panelView);
            this.Controls.Add(this.consoleTextBox);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(960, 0);
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "KinecTh";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox consoleTextBox;
        private System.Windows.Forms.Panel panelView;
        private GLSharp.GLSControl mqoView;
    }
}

