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
        private bool userHandle = true;

        private RTGraphParameter latestCamParam;
        private byte latestCamParamMask;
        private DateTime latestCamParamSendTime;
        private int retryCount;

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

        private void kickTimer()
        {
            timer1.Stop();
            if (userHandle)
            {
                timer1.Start();
            }
        }

        private void ParamToUI(RTGraphParameter camParam)
        {
            try
            {
                userHandle = false;
                cboImageSelect.SelectedIndex = (int)camParam.ImageSelector;
                cboTriggerSource.SelectedIndex = (int)camParam.TriggerSource;
                trkLineScanRate.Value = camParam.LineScanRate / 100;
                cboExposureLevel.SelectedIndex = (int)camParam.ExposureLevel; // 현재 사용 안 함
                cboGainLevel.SelectedIndex = (int)camParam.GainLevel;
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
                userHandle = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UIToParam(RTGraphParameter camParam)
        {
            camParam.ImageSelector = (RTGraphParameter.ImageSelectorEnum)cboImageSelect.SelectedIndex;
            // camParam.TriggerSource = (RTGraphParameter.TriggerSourceEnum)cboTriggerSource.SelectedIndex; // 현재 세팅 불가
            camParam.LineScanRate = (UInt16)(trkLineScanRate.Value * 100);
            camParam.ExposureLevel = (RTGraphParameter.ExposureLevelEnum)cboExposureLevel.SelectedIndex; // 현재 사용 안 함
            camParam.GainLevel = (RTGraphParameter.GainLevelEnum)cboGainLevel.SelectedIndex;
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
            this.Invoke(new MethodInvoker(() => {
                timer2.Stop();
                ParamToUI(comm.DeviceParameter);
            }));
        }

        private void defaultButton_Click(object sender, EventArgs e)
        {
            comm.RequestParam(true);
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            comm.RequestParam(false);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var camParam = new RTGraphParameter();
            UIToParam(camParam);
            comm.ApplyParam(camParam, true);
        }

        private void trkLineScanRate_ValueChanged(object sender, EventArgs e)
        {
            lblLineScanRate.Text = $"{trkLineScanRate.Value * 100}";

            kickTimer();
        }

        private void Item_ValueChanged(object sender, EventArgs e)
        {
            kickTimer();
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

            kickTimer();
        }

        private void numRoiEnd_ValueChanged(object sender, EventArgs e)
        {
            var minValue = Math.Min(numRoiStart.Value + numThresholdWidth.Value, 1023);
            if (numRoiEnd.Value < minValue)
            {
                numRoiEnd.Value = minValue;
            }

            if (trkRoiEnd.Value != (int)numRoiEnd.Value)
            {
                trkRoiEnd.Value = (int)numRoiEnd.Value;
            }

            kickTimer();
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

            var camParam = new RTGraphParameter(comm.DeviceParameter);
            UIToParam(camParam);
            byte msk = 0;
            if (camParam.group_1_refCnt > 0)
                msk |= RTGraphParameter.MASK_GROUP_1;
            if (camParam.group_2_refCnt > 0)
                msk |= RTGraphParameter.MASK_GROUP_2;
            if (camParam.group_3_refCnt > 0)
                msk |= RTGraphParameter.MASK_GROUP_3;
            if (camParam.group_4_refCnt > 0)
                msk |= RTGraphParameter.MASK_GROUP_4;
            comm.ApplyParam(camParam, false, msk);

            latestCamParam = camParam;
            latestCamParamMask = msk;
            latestCamParamSendTime = DateTime.Now;
            retryCount = 0;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now - latestCamParamSendTime > TimeSpan.FromSeconds(3))
            {
                if (retryCount < 3)
                {
                    comm.ApplyParam(latestCamParam, false, latestCamParamMask);
                    latestCamParamSendTime = DateTime.Now;
                    retryCount++;
                }
                else
                {
                    MessageBox.Show("패킷 전송이 3회 실패했습니다.");
                    timer2.Stop();
                }
            }
        }
    }
}
