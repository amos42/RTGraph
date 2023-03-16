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
                cboImageSelect.SelectedIndex = (int)camParam.ImageSelector;
                cboTriggerSource.SelectedIndex = (int)camParam.TriggerSource;
                // numExposureTime.Value = (int)camParam.ExposureLevel; // 현재 사용 안 함
                cboLineScanRate.SelectedIndex = (int)camParam.LineScanRate;
                comboBox4.SelectedIndex = (int)camParam.GainLevel;
                numTde.Value = camParam.TDE;
                numTch.Value = camParam.TCH;
                numTre1.Value = camParam.TRE1;
                numTre2.Value = camParam.TRE2;
                numTsl.Value = camParam.TSL;
                numTpw.Value = camParam.TPW;
                numRoiStart.Value = camParam.ROIStart;
                numRoiEnd.Value = camParam.ROIEnd;
                numThresholdLevel.Value = camParam.ThresholdLevel;
                numThresholdWidth.Value = camParam.ThresholdWidth;
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UIToParam(RTGraphParameter camParam)
        {
            camParam.ImageSelector = (RTGraphParameter.ImageSelectorEnum)cboImageSelect.SelectedIndex;
            // camParam.TriggerSource = (RTGraphParameter.TriggerSourceEnum)comboBox2.SelectedIndex; // 현재 세팅 불가
            // camParam.ExposureLevel = (RTGraphParameter.ExposureLevelEnum)numExposureTime.Value; // 현재 사용 안 함
            camParam.LineScanRate = (RTGraphParameter.LineScanRateEnum)cboLineScanRate.SelectedIndex;
            camParam.GainLevel = (RTGraphParameter.GainLevelEnum)comboBox4.SelectedIndex;
            camParam.TDE = (UInt16)numTde.Value;
            camParam.TCH = (UInt16)numTch.Value;
            camParam.TRE1 = (UInt16)numTre1.Value;
            camParam.TRE2 = (UInt16)numTre2.Value;
            camParam.TSL = (UInt16)numTsl.Value;
            camParam.TPW = (UInt16)numTpw.Value;
            camParam.ROIStart = (UInt16)numRoiStart.Value;
            camParam.ROIEnd = (UInt16)numRoiEnd.Value;
            camParam.ThresholdLevel = (byte)numThresholdLevel.Value;
            camParam.ThresholdWidth = (UInt16)numThresholdWidth.Value;
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

        private void numRoiStart_ValueChanged(object sender, EventArgs e)
        {
            var maxValue = Math.Max(numRoiEnd.Value - numThresholdWidth.Value, 0);
            if (numRoiStart.Value > maxValue)
            {
                numRoiStart.Value = maxValue;
            }

            if (trkRoiStart.Value != (int)numRoiStart.Value)
            {
                trkRoiStart.Value = (int)numRoiStart.Value;
            }

            timer1.Stop();
            timer1.Start();
        }

        private void numRoiEnd_ValueChanged(object sender, EventArgs e)
        {
            var minValue = Math.Min(numRoiStart.Value + numThresholdWidth.Value, 1023);
            if (numRoiEnd.Value < minValue)
            {
                numRoiEnd.Value = minValue;
            }

            if(trkRoiEnd.Value != (int)numRoiEnd.Value)
            {
                trkRoiEnd.Value = (int)numRoiEnd.Value;
            }

            timer1.Stop();
            timer1.Start();
        }

        private void trkRoiStart_ValueChanged(object sender, EventArgs e)
        {
            numRoiStart.Value = trkRoiStart.Value;
        }

        private void trkRoiEnd_ValueChanged(object sender, EventArgs e)
        {
            numRoiEnd.Value = trkRoiEnd.Value;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            var camParam = new RTGraphParameter();
            UIToParam(camParam);
            comm.ApplyParam(camParam);
        }
    }
}
