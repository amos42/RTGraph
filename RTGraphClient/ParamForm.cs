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
            this.comm.camParam.PropertyChanged += new PropertyChangedEventHandler(ParameterChanged);
            ParamToUI(this.comm.camParam);
        }

        private void ParamForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.comm.camParam.PropertyChanged -= new PropertyChangedEventHandler(ParameterChanged);
        }

        private void ParamToUI(RTGraphParameter camParam)
        {
            try
            {
                comboBox1.SelectedIndex = (int)camParam.ImageSelector;
                comboBox2.SelectedIndex = (int)camParam.TriggerSource;
                numExposureTime.Value = camParam.ExposureTime;
                numLineRate.Value = camParam.LineRate;
                numGain.Value = camParam.Gain;
                numRch.Value = camParam.RCH;
                numTre1.Value = camParam.TRE1;
                numTre2.Value = camParam.TRE2;
                numTsl.Value = camParam.TSL;
                numTde.Value = camParam.TDE;
                numTwd.Value = camParam.TWD;
                numStart.Value = camParam.Start;
                numSize.Value = camParam.Size;
                numLevel.Value = camParam.Level;
                numMinSize.Value = camParam.MinSize;
            } 
            catch(Exception ex)
            {

            }
        }

        private void UIToParam(RTGraphParameter camParam)
        {
            camParam.ImageSelector = (RTGraphParameterImageSelector)comboBox1.SelectedIndex;
            camParam.TriggerSource = (RTGraphParameterTriggerSource)comboBox2.SelectedIndex;
            camParam.ExposureTime = (byte)numExposureTime.Value;
            camParam.LineRate = (short)numLineRate.Value;
            camParam.Gain = (short)numGain.Value;
            camParam.RCH = (short)numRch.Value;
            camParam.TRE1 = (short)numTre1.Value;
            camParam.TRE2 = (short)numTre2.Value;
            camParam.TSL = (short)numTsl.Value;
            camParam.TDE = (short)numTde.Value;
            camParam.TWD = (short)numTwd.Value;
            camParam.Start = (short)numStart.Value;
            camParam.Size = (short)numSize.Value;
            camParam.Level = (short)numLevel.Value;
            camParam.MinSize = (short)numMinSize.Value;
        }

        private void ParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Invoke(new Action(() => {
                ParamToUI(comm.camParam);
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
            numStart.Value = trackBar1.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            numSize.Value = trackBar2.Value;
        }
    }
}
