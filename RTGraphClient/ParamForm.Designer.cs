
namespace RTGraph
{
    partial class ParamForm
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboExposureLevel = new System.Windows.Forms.ComboBox();
            this.lblLineScanRate = new System.Windows.Forms.Label();
            this.trkLineScanRate = new System.Windows.Forms.TrackBar();
            this.cboGainLevel = new System.Windows.Forms.ComboBox();
            this.Gain = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboTriggerSource = new System.Windows.Forms.ComboBox();
            this.cboImageSelect = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numTpw = new System.Windows.Forms.NumericUpDown();
            this.numTsl = new System.Windows.Forms.NumericUpDown();
            this.numTre2 = new System.Windows.Forms.NumericUpDown();
            this.numTre1 = new System.Windows.Forms.NumericUpDown();
            this.numTch = new System.Windows.Forms.NumericUpDown();
            this.numTde = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numThresholdWidth = new System.Windows.Forms.NumericUpDown();
            this.numThresholdLevel = new System.Windows.Forms.NumericUpDown();
            this.numRoiEnd = new System.Windows.Forms.NumericUpDown();
            this.numRoiStart = new System.Windows.Forms.NumericUpDown();
            this.trkRoiEnd = new System.Windows.Forms.TrackBar();
            this.trkRoiStart = new System.Windows.Forms.TrackBar();
            this.defaultButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkLineScanRate)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTpw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTsl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTre2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTre1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTde)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRoiEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRoiStart)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboExposureLevel);
            this.groupBox1.Controls.Add(this.lblLineScanRate);
            this.groupBox1.Controls.Add(this.trkLineScanRate);
            this.groupBox1.Controls.Add(this.cboGainLevel);
            this.groupBox1.Controls.Add(this.Gain);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboTriggerSource);
            this.groupBox1.Controls.Add(this.cboImageSelect);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 206);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Setting";
            // 
            // cboExposureLevel
            // 
            this.cboExposureLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExposureLevel.FormattingEnabled = true;
            this.cboExposureLevel.Items.AddRange(new object[] {
            "0%",
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%",
            "100%"});
            this.cboExposureLevel.Location = new System.Drawing.Point(140, 115);
            this.cboExposureLevel.Name = "cboExposureLevel";
            this.cboExposureLevel.Size = new System.Drawing.Size(163, 23);
            this.cboExposureLevel.TabIndex = 16;
            this.cboExposureLevel.SelectedIndexChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // lblLineScanRate
            // 
            this.lblLineScanRate.Location = new System.Drawing.Point(252, 84);
            this.lblLineScanRate.Name = "lblLineScanRate";
            this.lblLineScanRate.Size = new System.Drawing.Size(52, 23);
            this.lblLineScanRate.TabIndex = 15;
            this.lblLineScanRate.Text = "30000";
            this.lblLineScanRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trkLineScanRate
            // 
            this.trkLineScanRate.AutoSize = false;
            this.trkLineScanRate.LargeChange = 10;
            this.trkLineScanRate.Location = new System.Drawing.Point(140, 86);
            this.trkLineScanRate.Maximum = 300;
            this.trkLineScanRate.Minimum = 50;
            this.trkLineScanRate.Name = "trkLineScanRate";
            this.trkLineScanRate.Size = new System.Drawing.Size(115, 23);
            this.trkLineScanRate.TabIndex = 14;
            this.trkLineScanRate.Value = 300;
            this.trkLineScanRate.ValueChanged += new System.EventHandler(this.trkLineScanRate_ValueChanged);
            // 
            // cboGainLevel
            // 
            this.cboGainLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGainLevel.FormattingEnabled = true;
            this.cboGainLevel.Items.AddRange(new object[] {
            "Level 0",
            "Level 1",
            "Level 2",
            "Level 3",
            "Level 4",
            "Level 5",
            "Level 6",
            "Level 7"});
            this.cboGainLevel.Location = new System.Drawing.Point(140, 145);
            this.cboGainLevel.Name = "cboGainLevel";
            this.cboGainLevel.Size = new System.Drawing.Size(164, 23);
            this.cboGainLevel.TabIndex = 13;
            this.cboGainLevel.SelectedIndexChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // Gain
            // 
            this.Gain.Location = new System.Drawing.Point(13, 148);
            this.Gain.Name = "Gain";
            this.Gain.Size = new System.Drawing.Size(117, 23);
            this.Gain.TabIndex = 11;
            this.Gain.Text = "Gain";
            this.Gain.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(13, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 23);
            this.label8.TabIndex = 10;
            this.label8.Text = "Line Scan Rate";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(13, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 23);
            this.label7.TabIndex = 9;
            this.label7.Text = "Exposure Level";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(13, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 23);
            this.label6.TabIndex = 8;
            this.label6.Text = "Trigger Source";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 23);
            this.label5.TabIndex = 7;
            this.label5.Text = "Image Select";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboTriggerSource
            // 
            this.cboTriggerSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTriggerSource.Enabled = false;
            this.cboTriggerSource.FormattingEnabled = true;
            this.cboTriggerSource.Items.AddRange(new object[] {
            "Image Trigger",
            "External Trigger"});
            this.cboTriggerSource.Location = new System.Drawing.Point(140, 53);
            this.cboTriggerSource.Name = "cboTriggerSource";
            this.cboTriggerSource.Size = new System.Drawing.Size(164, 23);
            this.cboTriggerSource.TabIndex = 3;
            this.cboTriggerSource.SelectedIndexChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // cboImageSelect
            // 
            this.cboImageSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboImageSelect.FormattingEnabled = true;
            this.cboImageSelect.Items.AddRange(new object[] {
            "Sensor",
            "Pattern 1",
            "Pattern 2",
            "Stop"});
            this.cboImageSelect.Location = new System.Drawing.Point(140, 24);
            this.cboImageSelect.Name = "cboImageSelect";
            this.cboImageSelect.Size = new System.Drawing.Size(164, 23);
            this.cboImageSelect.TabIndex = 2;
            this.cboImageSelect.SelectedIndexChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.numTpw);
            this.groupBox2.Controls.Add(this.numTsl);
            this.groupBox2.Controls.Add(this.numTre2);
            this.groupBox2.Controls.Add(this.numTre1);
            this.groupBox2.Controls.Add(this.numTch);
            this.groupBox2.Controls.Add(this.numTde);
            this.groupBox2.Location = new System.Drawing.Point(339, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(201, 206);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trigger Pulse Timing";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(158, 169);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 25);
            this.label16.TabIndex = 23;
            this.label16.Text = "us";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(158, 140);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(30, 25);
            this.label17.TabIndex = 22;
            this.label17.Text = "ms";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(158, 111);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(30, 25);
            this.label18.TabIndex = 21;
            this.label18.Text = "ms";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(158, 82);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 25);
            this.label19.TabIndex = 20;
            this.label19.Text = "ms";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(158, 53);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(30, 25);
            this.label20.TabIndex = 19;
            this.label20.Text = "ms";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(158, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(30, 25);
            this.label21.TabIndex = 18;
            this.label21.Text = "ms";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(8, 169);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 25);
            this.label15.TabIndex = 17;
            this.label15.Text = "Tpw";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(8, 140);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 25);
            this.label10.TabIndex = 16;
            this.label10.Text = "Tsl";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(8, 111);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 25);
            this.label11.TabIndex = 15;
            this.label11.Text = "Tre2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 82);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 25);
            this.label12.TabIndex = 14;
            this.label12.Text = "Tre1";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 53);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 25);
            this.label13.TabIndex = 13;
            this.label13.Text = "Tch";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(8, 24);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 25);
            this.label14.TabIndex = 12;
            this.label14.Text = "Tde";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numTpw
            // 
            this.numTpw.Location = new System.Drawing.Point(66, 169);
            this.numTpw.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTpw.Name = "numTpw";
            this.numTpw.Size = new System.Drawing.Size(86, 25);
            this.numTpw.TabIndex = 10;
            this.numTpw.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTpw.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numTsl
            // 
            this.numTsl.Location = new System.Drawing.Point(66, 140);
            this.numTsl.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTsl.Name = "numTsl";
            this.numTsl.Size = new System.Drawing.Size(86, 25);
            this.numTsl.TabIndex = 9;
            this.numTsl.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTsl.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numTre2
            // 
            this.numTre2.Location = new System.Drawing.Point(66, 111);
            this.numTre2.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTre2.Name = "numTre2";
            this.numTre2.Size = new System.Drawing.Size(86, 25);
            this.numTre2.TabIndex = 8;
            this.numTre2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTre2.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numTre1
            // 
            this.numTre1.Location = new System.Drawing.Point(66, 82);
            this.numTre1.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTre1.Name = "numTre1";
            this.numTre1.Size = new System.Drawing.Size(86, 25);
            this.numTre1.TabIndex = 7;
            this.numTre1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTre1.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numTch
            // 
            this.numTch.Location = new System.Drawing.Point(66, 53);
            this.numTch.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTch.Name = "numTch";
            this.numTch.Size = new System.Drawing.Size(86, 25);
            this.numTch.TabIndex = 6;
            this.numTch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTch.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numTde
            // 
            this.numTde.Location = new System.Drawing.Point(66, 24);
            this.numTde.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.numTde.Name = "numTde";
            this.numTde.Size = new System.Drawing.Size(86, 25);
            this.numTde.TabIndex = 5;
            this.numTde.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numTde.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.numThresholdWidth);
            this.groupBox3.Controls.Add(this.numThresholdLevel);
            this.groupBox3.Controls.Add(this.numRoiEnd);
            this.groupBox3.Controls.Add(this.numRoiStart);
            this.groupBox3.Controls.Add(this.trkRoiEnd);
            this.groupBox3.Controls.Add(this.trkRoiStart);
            this.groupBox3.Location = new System.Drawing.Point(12, 224);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(528, 142);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Detect Condition";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(13, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 25);
            this.label9.TabIndex = 10;
            this.label9.Text = "Threshold";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(310, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "Width";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(119, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "Level";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "ROI End";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "ROI Start";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numThresholdWidth
            // 
            this.numThresholdWidth.Location = new System.Drawing.Point(359, 103);
            this.numThresholdWidth.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numThresholdWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThresholdWidth.Name = "numThresholdWidth";
            this.numThresholdWidth.Size = new System.Drawing.Size(74, 25);
            this.numThresholdWidth.TabIndex = 5;
            this.numThresholdWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numThresholdWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThresholdWidth.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numThresholdLevel
            // 
            this.numThresholdLevel.Location = new System.Drawing.Point(167, 103);
            this.numThresholdLevel.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numThresholdLevel.Name = "numThresholdLevel";
            this.numThresholdLevel.Size = new System.Drawing.Size(79, 25);
            this.numThresholdLevel.TabIndex = 4;
            this.numThresholdLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numThresholdLevel.ValueChanged += new System.EventHandler(this.Item_ValueChanged);
            // 
            // numRoiEnd
            // 
            this.numRoiEnd.Location = new System.Drawing.Point(435, 65);
            this.numRoiEnd.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.numRoiEnd.Name = "numRoiEnd";
            this.numRoiEnd.Size = new System.Drawing.Size(70, 25);
            this.numRoiEnd.TabIndex = 3;
            this.numRoiEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRoiEnd.Value = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.numRoiEnd.ValueChanged += new System.EventHandler(this.numRoiEnd_ValueChanged);
            // 
            // numRoiStart
            // 
            this.numRoiStart.Location = new System.Drawing.Point(435, 33);
            this.numRoiStart.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.numRoiStart.Name = "numRoiStart";
            this.numRoiStart.Size = new System.Drawing.Size(72, 25);
            this.numRoiStart.TabIndex = 2;
            this.numRoiStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numRoiStart.ValueChanged += new System.EventHandler(this.numRoiStart_ValueChanged);
            // 
            // trkRoiEnd
            // 
            this.trkRoiEnd.AutoSize = false;
            this.trkRoiEnd.Location = new System.Drawing.Point(91, 64);
            this.trkRoiEnd.Maximum = 1023;
            this.trkRoiEnd.Name = "trkRoiEnd";
            this.trkRoiEnd.Size = new System.Drawing.Size(338, 26);
            this.trkRoiEnd.TabIndex = 1;
            this.trkRoiEnd.TickFrequency = 10;
            this.trkRoiEnd.Value = 1023;
            this.trkRoiEnd.ValueChanged += new System.EventHandler(this.trkRoiEnd_ValueChanged);
            // 
            // trkRoiStart
            // 
            this.trkRoiStart.AutoSize = false;
            this.trkRoiStart.Location = new System.Drawing.Point(91, 33);
            this.trkRoiStart.Maximum = 1023;
            this.trkRoiStart.Name = "trkRoiStart";
            this.trkRoiStart.Size = new System.Drawing.Size(338, 25);
            this.trkRoiStart.TabIndex = 0;
            this.trkRoiStart.TickFrequency = 10;
            this.trkRoiStart.ValueChanged += new System.EventHandler(this.trkRoiStart_ValueChanged);
            // 
            // defaultButton
            // 
            this.defaultButton.Location = new System.Drawing.Point(204, 372);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(75, 30);
            this.defaultButton.TabIndex = 5;
            this.defaultButton.Text = "Default";
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Enabled = false;
            this.loadButton.Location = new System.Drawing.Point(291, 372);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 30);
            this.loadButton.TabIndex = 6;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(378, 372);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 30);
            this.saveButton.TabIndex = 7;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(465, 372);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 30);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ParamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 412);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ParamForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parameter Setting";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ParamForm_FormClosed);
            this.Load += new System.EventHandler(this.ParamForm_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trkLineScanRate)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numTpw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTsl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTre2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTre1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTde)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThresholdLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRoiStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRoiEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkRoiStart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboTriggerSource;
        private System.Windows.Forms.ComboBox cboImageSelect;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numTsl;
        private System.Windows.Forms.NumericUpDown numTre2;
        private System.Windows.Forms.NumericUpDown numTre1;
        private System.Windows.Forms.NumericUpDown numTch;
        private System.Windows.Forms.NumericUpDown numTde;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numThresholdWidth;
        private System.Windows.Forms.NumericUpDown numThresholdLevel;
        private System.Windows.Forms.NumericUpDown numRoiEnd;
        private System.Windows.Forms.NumericUpDown numRoiStart;
        private System.Windows.Forms.TrackBar trkRoiEnd;
        private System.Windows.Forms.TrackBar trkRoiStart;
        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numTpw;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Gain;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cboGainLevel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar trkLineScanRate;
        private System.Windows.Forms.Label lblLineScanRate;
        private System.Windows.Forms.ComboBox cboExposureLevel;
    }
}