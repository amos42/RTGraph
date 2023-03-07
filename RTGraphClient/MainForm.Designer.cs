
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
            this.chart1 = new RTGraph.RTGraphChartControl();
            this.button3 = new System.Windows.Forms.Button();
            this.SocketOpenBtn = new System.Windows.Forms.Button();
            this.CaptureBtn = new System.Windows.Forms.Button();
            this.ConnectBtn = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.Color.Black;
            this.chart1.BufferCount = 100;
            this.chart1.GraphMargin = new System.Windows.Forms.Padding(10, 200, 10, 10);
            this.chart1.Location = new System.Drawing.Point(14, 49);
            this.chart1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1013, 538);
            this.chart1.StartPos = 0;
            this.chart1.TabIndex = 5;
            this.chart1.TriggerValue = 0;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(68, 8);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(34, 34);
            this.button3.TabIndex = 11;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SocketOpenBtn
            // 
            this.SocketOpenBtn.Location = new System.Drawing.Point(14, 9);
            this.SocketOpenBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SocketOpenBtn.Name = "SocketOpenBtn";
            this.SocketOpenBtn.Size = new System.Drawing.Size(57, 33);
            this.SocketOpenBtn.TabIndex = 4;
            this.SocketOpenBtn.Text = "Open";
            this.SocketOpenBtn.UseVisualStyleBackColor = true;
            this.SocketOpenBtn.Click += new System.EventHandler(this.SocketOpenBtn_Click);
            // 
            // CaptureBtn
            // 
            this.CaptureBtn.Location = new System.Drawing.Point(3, 3);
            this.CaptureBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CaptureBtn.Name = "CaptureBtn";
            this.CaptureBtn.Size = new System.Drawing.Size(115, 31);
            this.CaptureBtn.TabIndex = 1;
            this.CaptureBtn.Text = "Start Capture";
            this.CaptureBtn.UseVisualStyleBackColor = true;
            this.CaptureBtn.Click += new System.EventHandler(this.CaptureBtn_Click);
            // 
            // ConnectBtn
            // 
            this.ConnectBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ConnectBtn.Location = new System.Drawing.Point(121, 9);
            this.ConnectBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ConnectBtn.Name = "ConnectBtn";
            this.ConnectBtn.Size = new System.Drawing.Size(126, 34);
            this.ConnectBtn.TabIndex = 9;
            this.ConnectBtn.Text = "Connect";
            this.ConnectBtn.UseVisualStyleBackColor = true;
            this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(327, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 30);
            this.button2.TabIndex = 12;
            this.button2.Text = "Calibration";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(222, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 30);
            this.button1.TabIndex = 11;
            this.button1.Text = "Parameter";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.CaptureBtn);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(603, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(425, 37);
            this.panel1.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 600);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ConnectBtn);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.SocketOpenBtn);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "RealTime Graph";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private RTGraphChartControl chart1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button SocketOpenBtn;
        private System.Windows.Forms.Button CaptureBtn;
        private System.Windows.Forms.Button ConnectBtn;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
    }
}

