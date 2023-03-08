using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class CalibForm : Form
    {
        private RTGraphComm comm;

        public CalibForm(RTGraphComm comm)
        {
            this.comm = comm;
            InitializeComponent();
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
                rtGraphChartControl1.AddValueLine(-1, comm.calData, 0, 1024);
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
