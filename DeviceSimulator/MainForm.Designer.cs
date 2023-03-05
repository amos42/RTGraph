
namespace DeviceSimulator
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SocketOpenBtn = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.logControl1 = new DeviceSimulator.LogControl();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SocketOpenBtn
            // 
            this.SocketOpenBtn.Location = new System.Drawing.Point(12, 12);
            this.SocketOpenBtn.Name = "SocketOpenBtn";
            this.SocketOpenBtn.Size = new System.Drawing.Size(205, 69);
            this.SocketOpenBtn.TabIndex = 1;
            this.SocketOpenBtn.Text = "Open";
            this.SocketOpenBtn.UseVisualStyleBackColor = true;
            this.SocketOpenBtn.Click += new System.EventHandler(this.SocketOpenBtn_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.LargeChange = 10;
            this.trackBar1.Location = new System.Drawing.Point(351, 12);
            this.trackBar1.Maximum = 1023;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(731, 56);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Value = 100;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(147, 141);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(70, 25);
            this.textBox3.TabIndex = 11;
            this.textBox3.Text = "12000";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(147, 110);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(70, 25);
            this.textBox2.TabIndex = 10;
            this.textBox2.Text = "11000";
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(234, 12);
            this.trackBar2.Maximum = 100;
            this.trackBar2.Minimum = 10;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(111, 56);
            this.trackBar2.TabIndex = 12;
            this.trackBar2.Value = 40;
            // 
            // logControl1
            // 
            this.logControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logControl1.AutoScroll = true;
            this.logControl1.ListViewAutoScroll = true;
            this.logControl1.ListViewCapa = 500;
            this.logControl1.Location = new System.Drawing.Point(233, 74);
            this.logControl1.Name = "logControl1";
            this.logControl1.Size = new System.Drawing.Size(848, 583);
            this.logControl1.TabIndex = 13;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 671);
            this.Controls.Add(this.logControl1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.SocketOpenBtn);
            this.Name = "MainForm";
            this.Text = "Device Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button SocketOpenBtn;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TrackBar trackBar2;
        private LogControl logControl1;
    }
}

