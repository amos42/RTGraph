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
            this.comm.ParameterChanged += new EventHandler(ParameterChanged);
        }

        private void ParamForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.comm.ParameterChanged -= new EventHandler(ParameterChanged);
        }

        private void ParamForm_Shown(object sender, EventArgs e)
        {
            comm.RequestParam();
        }

        private void ParamToUI()
        {
            try
            {
                comboBox1.SelectedIndex = (int)comm.camParam.ImageSelector;
                comboBox2.SelectedIndex = (int)comm.camParam.TriggerSource;
                numExposureTime.Value = comm.camParam.ExposureTime;
                numLineRate.Value = comm.camParam.LineRate;
                numGain.Value = comm.camParam.Gain;
                numRch.Value = comm.camParam.RCH;
                numTre1.Value = comm.camParam.TRE1;
                numTre2.Value = comm.camParam.TRE2;
                numTsl.Value = comm.camParam.TSL;
                numTde.Value = comm.camParam.TDE;
                numTwd.Value = comm.camParam.TWD;
                numStart.Value = comm.camParam.Start;
                numSize.Value = comm.camParam.Size;
                numLevel.Value = comm.camParam.Level;
                numMinSize.Value = comm.camParam.MinSize;
            } 
            catch(Exception ex)
            {

            }
        }

        private void UIToParam()
        {
            comm.camParam.ImageSelector = (RTGraphParameterImageSelector)comboBox1.SelectedIndex;
            comm.camParam.TriggerSource = (RTGraphParameterTriggerSource)comboBox2.SelectedIndex;
            comm.camParam.ExposureTime = (byte)numExposureTime.Value;
            comm.camParam.LineRate = (short)numLineRate.Value;
            comm.camParam.Gain = (short)numGain.Value;
            comm.camParam.RCH = (short)numRch.Value;
            comm.camParam.TRE1 = (short)numTre1.Value;
            comm.camParam.TRE2 = (short)numTre2.Value;
            comm.camParam.TSL = (short)numTsl.Value;
            comm.camParam.TDE = (short)numTde.Value;
            comm.camParam.TWD = (short)numTwd.Value;
            comm.camParam.Start = (short)numStart.Value;
            comm.camParam.Size = (short)numSize.Value;
            comm.camParam.Level = (short)numLevel.Value;
            comm.camParam.MinSize = (short)numMinSize.Value;
        }

        private void ParameterChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => {
                ParamToUI();
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
            UIToParam();
            comm.ApplyParam(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UIToParam();
            comm.ApplyParam(false);
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
            numStart.Value = trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            numSize.Value = trackBar2.Value;
        }
    }
}
