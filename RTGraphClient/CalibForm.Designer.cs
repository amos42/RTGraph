﻿
namespace RTGraph
{
    partial class CalibForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chart1 = new RTGraph.RTGraphChartControl();
            this.SuspendLayout();
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button5.Location = new System.Drawing.Point(749, 279);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(103, 34);
            this.button5.TabIndex = 15;
            this.button5.Text = "Close";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(645, 279);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(103, 34);
            this.button4.TabIndex = 14;
            this.button4.Text = "Apply";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Location = new System.Drawing.Point(541, 279);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(103, 34);
            this.button3.TabIndex = 13;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(437, 279);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 34);
            this.button2.TabIndex = 12;
            this.button2.Text = "Load";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Location = new System.Drawing.Point(307, 279);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(129, 34);
            this.button6.TabIndex = 16;
            this.button6.Text = "Calibration Start";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(15, 291);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(146, 19);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Calibration Enable";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.Color.Black;
            this.chart1.BorderLineWidth = 1;
            this.chart1.BufferCount = 0;
            this.chart1.GraphAreaMinHeight = 100;
            this.chart1.GraphMargin = new System.Windows.Forms.Padding(10);
            this.chart1.Location = new System.Drawing.Point(1, 2);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(866, 271);
            this.chart1.TabIndex = 0;
            this.chart1.TriggerValue = 0;
            this.chart1.ValueCount = 1024;
            this.chart1.ValuesCount = 1;
            // 
            // CalibForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 325);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.chart1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CalibForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calibration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CalibForm_FormClosed);
            this.Load += new System.EventHandler(this.CalibForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RTGraphChartControl chart1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}