using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class CalibForm : Form
    {
        private RTGraphComm comm;
        private Queue<byte[]> queue = new Queue<byte[]>(5);
        private int[] sums = new int[1024];
        private bool calMode = false;

        public CalibForm(RTGraphComm comm)
        {
            this.comm = comm;
            InitializeComponent();

            //chart1.ValuesCount = 3;
            //chart1.Values[2].GraphColor = Color.AliceBlue;

            chart1.Values.Add(new GraphItem(chart1.ValueCount, Color.Cyan, Color.Lime));
            chart1.Values.Add(new GraphItem(chart1.ValueCount, Color.White, Color.Lime));
        }

        private void CalibForm_Load(object sender, EventArgs e)
        {
            comm.CalibrationChanged += new EventHandler(CalChanged);
            comm.PacketReceived += new PacketReceivedEventHandler(RecvChanged);

            chart1.SetValueLine(1, comm.CalibrationData, 0, 1024);

            comm.RequesCalibration(); // load
            checkBox1.Checked = comm.DeviceParameter.CalibrationEnable; 
        }

        private void CalibForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.PacketReceived -= RecvChanged;
            comm.CalibrationChanged -= CalChanged;
        }

        private void RecvChanged(object sender, PacketReceivedEventArgs e)
        {
            if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabDataReceivced)
            {
                var data = new byte[1024];
                chart1.SetValueLine(0, e.Packet.Data, 2, 1024);

                if (calMode)
                {
                    Array.Copy(e.Packet.Data, 2, data, 0, 1024);

                    for (int i = 0; i < 1024; i++)
                    {
                        sums[i] += data[i];
                    }

                    queue.Enqueue(data);
                    int cnt = queue.Count;

                    if (cnt > 5)
                    {
                        var last = queue.Dequeue();
                        for (int i = 0; i < 1024; i++)
                        {
                            sums[i] -= last[i];
                        }
                        cnt--;
                    }

                    var itms = chart1.Values[2].Items;

                    for (int i = 0; i < 1024; i++)
                    {
                        itms[i] = (byte)(sums[i] / cnt);
                    }
                }

                this.Invoke(new Action(() => {
                    chart1.Refresh();
                }));
            }
        }

        private void CalChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => {
                chart1.SetValueLine(1, comm.CalibrationData, 0, 1024);
                chart1.Refresh();
            }));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!calMode) 
            {
                queue.Clear(); ;
                Array.Clear(sums, 0, 1024);
                calMode = true;
                button6.Text = "Calibration Stop";
            } 
            else
            {
                calMode = false;
                button6.Text = "Calibration Start";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comm.RequesCalibration(); // load
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comm.ApplyCalibration(chart1.Values[2].Items, true, checkBox1.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comm.ApplyCalibration(chart1.Values[2].Items, false, checkBox1.Checked);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked != comm.DeviceParameter.CalibrationEnable)
            {
                var param = new RTGraphParameter();
                param.CalibrationEnable = checkBox1.Checked;
                comm.ApplyParam(param, false, RTGraphParameter.MASK_GROUP_4);
            }
        }
    }
}
