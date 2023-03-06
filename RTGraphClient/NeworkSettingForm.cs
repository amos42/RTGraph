﻿using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RTGraph
{
    public partial class NeworkSettingForm : Form
    {
        private RTGraphComm comm = null;

        public NeworkSettingForm(RTGraphComm comm)
        {
            this.comm = comm;
            InitializeComponent();
        }

        private void NeworkSettingForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = comm.HostIP;
            textBox2.Text = comm.SendPort.ToString();
            textBox3.Text = comm.RecvPort.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comm.HostIP = textBox1.Text;
            comm.SendPort = Int32.Parse(textBox2.Text);
            comm.RecvPort = Int32.Parse(textBox3.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
