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

        private void ParameterChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => {
                comboBox1.SelectedIndex = comm.camParam.image_selector;
                comboBox2.SelectedIndex = comm.camParam.trigger_source;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
