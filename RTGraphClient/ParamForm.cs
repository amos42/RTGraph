using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class ParamForm : Form
    {
        private RTGraphComm comm;

        public ParamForm(RTGraphComm comm)
        {
            this.comm = comm;
            InitializeComponent();
        }

        public void SetComm(RTGraphComm comm)
        {
            this.comm = comm;
        }

        private void ParamForm_Load(object sender, EventArgs e)
        {
            this.comm.DeviceParameter.PropertyChanged += new PropertyChangedEventHandler(ParameterChanged);
            ParamToUI(this.comm.DeviceParameter);
        }

        private void ParamForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.comm.DeviceParameter.PropertyChanged -= ParameterChanged;
        }

        private void ParamToUI(RTGraphParameter camParam)
        {
            try
            {
                comboBox1.SelectedIndex = (int)camParam.ImageSelector;
                comboBox2.SelectedIndex = (int)camParam.TriggerSource;
                // numExposureTime.Value = (int)camParam.ExposureLevel; // 현재 사용 안 함
                comboBox3.SelectedIndex = (int)camParam.LineScanRate;
                comboBox4.SelectedIndex = (int)camParam.GainLevel;
                numTde.Value = camParam.TCH;
                numTch.Value = camParam.TRE1;
                numTre1.Value = camParam.TRE2;
                numTre2.Value = camParam.TSL;
                numTsl.Value = camParam.TDE;
                numTpw.Value = camParam.TPW;
                numStart.Value = camParam.ROIStart;
                numSize.Value = camParam.ROIEnd;
                numLevel.Value = camParam.ThresholdLevel;
                numMinSize.Value = camParam.ThresholdWidth;
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UIToParam(RTGraphParameter camParam)
        {
            camParam.ImageSelector = (RTGraphParameter.ImageSelectorEnum)comboBox1.SelectedIndex;
            // camParam.TriggerSource = (RTGraphParameter.TriggerSourceEnum)comboBox2.SelectedIndex; // 현재 세팅 불가
            // camParam.ExposureLevel = (RTGraphParameter.ExposureLevelEnum)numExposureTime.Value; // 현재 사용 안 함
            camParam.LineScanRate = (RTGraphParameter.LineScanRateEnum)comboBox3.SelectedIndex;
            camParam.GainLevel = (RTGraphParameter.GainLevelEnum)comboBox4.SelectedIndex;
            camParam.TDE = (UInt16)numTsl.Value;
            camParam.TCH = (UInt16)numTde.Value;
            camParam.TRE1 = (UInt16)numTch.Value;
            camParam.TRE2 = (UInt16)numTre1.Value;
            camParam.TSL = (UInt16)numTre2.Value;
            camParam.TPW = (UInt16)numTpw.Value;
            camParam.ROIStart = (UInt16)numStart.Value;
            camParam.ROIEnd = (UInt16)numSize.Value;
            camParam.ThresholdLevel = (byte)numLevel.Value;
            camParam.ThresholdWidth = (UInt16)numMinSize.Value;
        }

        private void ParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Invoke(new Action(() => {
                ParamToUI(comm.DeviceParameter);
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comm.RequestParam(true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comm.RequestParam(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var camParam = new RTGraphParameter();
            UIToParam(camParam);
            comm.ApplyParam(camParam, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var camParam = new RTGraphParameter();
            UIToParam(camParam);
            comm.ApplyParam(camParam, false);
        }

        private void numStart_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numStart.Value;
        }

        private void numSize_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numSize.Value;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar1.Value > trackBar2.Value)
            {
                trackBar1.Value = trackBar2.Value;
            }

            numStart.Value = trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar1.Value > trackBar2.Value)
            {
                trackBar2.Value = trackBar1.Value;
            }

            numSize.Value = trackBar2.Value;
        }
    }
}
