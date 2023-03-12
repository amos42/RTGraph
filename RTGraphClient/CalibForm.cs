using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
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

        public CalibForm(RTGraphComm comm)
        {
            this.comm = comm;
            InitializeComponent();

            chart1.ValuesCount = 2;
            //chart1.Values[1].GraphColor = Color.
        }

        private void CalibForm_Load(object sender, EventArgs e)
        {
            comm.CalibrationChanged += new EventHandler(CalChanged);
        }

        private void CalibForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.CalibrationChanged -= new EventHandler(CalChanged);
        }

        private void CalChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => {
                chart1.AddValueLine(-1, comm.CalibrationData, 0, 1024);

                var curr = comm.CalibrationData.Clone() as byte[];
                for (int i = 0; i < 1024; i++)
                {
                    sums[i] += curr[i];
                }

                queue.Enqueue(curr);
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

                for (int i = 0; i < 1024; i++)
                {
                    chart1.Values[1].Items[i] = (byte)(sums[i] / cnt);
                }

                chart1.Refresh();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // comm.RequesCalibration(-1); // default
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comm.RequesCalibration();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comm.RequesCalibration(1); // load
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comm.ApplyCalibration(true, checkBox1.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comm.ApplyCalibration(false, checkBox1.Checked);
        }

    }
}
