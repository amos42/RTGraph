
namespace RTGraph
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
            this.CaptureBtn = new System.Windows.Forms.Button();
            this.SocketOpenBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chart1 = new RTGraph.RTGraphChartControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CaptureBtn
            // 
            this.CaptureBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CaptureBtn.Location = new System.Drawing.Point(161, 5);
            this.CaptureBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CaptureBtn.Name = "CaptureBtn";
            this.CaptureBtn.Size = new System.Drawing.Size(151, 47);
            this.CaptureBtn.TabIndex = 1;
            this.CaptureBtn.Text = "Start Capture";
            this.CaptureBtn.UseVisualStyleBackColor = true;
            this.CaptureBtn.Click += new System.EventHandler(this.CaptureBtn_Click);
            // 
            // SocketOpenBtn
            // 
            this.SocketOpenBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SocketOpenBtn.Location = new System.Drawing.Point(28, 596);
            this.SocketOpenBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SocketOpenBtn.Name = "SocketOpenBtn";
            this.SocketOpenBtn.Size = new System.Drawing.Size(281, 40);
            this.SocketOpenBtn.TabIndex = 4;
            this.SocketOpenBtn.Text = "Socket Open";
            this.SocketOpenBtn.UseVisualStyleBackColor = true;
            this.SocketOpenBtn.Click += new System.EventHandler(this.SocketOpenBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(28, 566);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(124, 25);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "127.0.0.1";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.Location = new System.Drawing.Point(158, 566);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(70, 25);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "11000";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox3.Location = new System.Drawing.Point(234, 566);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(70, 25);
            this.textBox3.TabIndex = 8;
            this.textBox3.Text = "12000";
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConnectBtn.Location = new System.Drawing.Point(3, 3);
            this.ConnectBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(152, 49);
            this.ConnectBtn.TabIndex = 9;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.ConnectBtn);
            this.panel1.Controls.Add(this.CaptureBtn);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(710, 579);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(317, 56);
            this.panel1.TabIndex = 10;
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.Color.Black;
            this.chart1.BufferCount = 100;
            this.chart1.GraphMargin = new System.Windows.Forms.Padding(10, 200, 10, 10);
            this.chart1.Location = new System.Drawing.Point(14, 15);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1013, 544);
            this.chart1.StartPos = 0;
            this.chart1.TabIndex = 5;
            this.chart1.TriggerValue = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 647);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.SocketOpenBtn);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "RealTime Graph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CaptureBtn;
        private System.Windows.Forms.Button SocketOpenBtn;
        private RTGraphChartControl chart1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Panel panel1;
    }
}

