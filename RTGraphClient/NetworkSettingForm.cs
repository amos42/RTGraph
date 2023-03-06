using RTGraphProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class NetworkSettingForm : Form
    {
        private RTGraphComm comm = null;

        public NetworkSettingForm(RTGraphComm comm)
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

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection cfgCollection = config.AppSettings.Settings;

            cfgCollection.Remove("HostIP");
            cfgCollection.Add("HostIP", comm.HostIP);
            cfgCollection.Remove("SendPort");
            cfgCollection.Add("SendPort", comm.SendPort.ToString());
            cfgCollection.Remove("RecvPort");
            cfgCollection.Add("RecvPort", comm.RecvPort.ToString());

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
