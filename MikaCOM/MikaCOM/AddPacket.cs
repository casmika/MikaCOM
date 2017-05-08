using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MikaCOM
{
    public partial class AddPacket : Form
    {
        public string pacName = "";
        public bool isOk = false;
        public bool isHex = false;
        public string content = "";
        public AddPacket()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pacName = textBox1.Text;
            isHex = radioButton2.Checked;
            content = textBox2.Text;
            isOk = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddPacket_Load(object sender, EventArgs e)
        {

        }


    }
}
