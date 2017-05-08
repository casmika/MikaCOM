using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace MikaCOM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void getAvailablePorts()
        {
            try
            {
                toolStripComboBox1.Items.Clear();
                String[] ports = SerialPort.GetPortNames();
                toolStripComboBox1.Items.AddRange(ports);
            }
            catch (Exception)
            {
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            getAvailablePorts();
            toolStripComboBox2.SelectedIndex = 5;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            getAvailablePorts();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBox1.Text == "" || toolStripComboBox2.Text == "")
                {
                    MessageBox.Show("Please Select Port!");
                }
                else
                {
                    if (toolStripButton1.Text == "Open")
                    {
                        serialPort1.PortName = toolStripComboBox1.Text;
                        serialPort1.BaudRate = Convert.ToInt32(toolStripComboBox2.Text);
                        serialPort1.ReadTimeout = 1000;
                        serialPort1.WriteTimeout = 1000;
                        serialPort1.DataReceived += SerialPort1_DataReceived;
                        serialPort1.Open();
                        toolStripButton1.Text = "Close";
                        panel2.Enabled = true;
                    }
                    else
                    {
                        serialPort1.Close();
                        toolStripButton1.Text = "Open";
                        panel2.Enabled = false;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error Port Detile: Unauthorizzed Accessed!");
            }
        }
        SerialPort sp;
        string dataStrGrap;
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            sp = (SerialPort)sender;
            if (checkBox8.Checked)
            {
                this.Invoke(new EventHandler(displayGraph));
            }
            else
            {
                this.Invoke(new EventHandler(displayText));
            }
            serialPort1.DiscardInBuffer();
        }

        private void displayGraph(object sender, EventArgs e)
        {            
            try
            {
                dataStrGrap = sp.ReadLine();
                char[] spliter = { '#', ' ', ';', '\n' };
                string[] datas = dataStrGrap.Split(spliter);
                if (datas.Length == 2)
                {
                    DateTime localDate = DateTime.Now;
                    chart1.Series["Series1"].Points.AddXY(localDate, datas[0]);
                    chart1.Series["Series2"].Points.AddXY(localDate, datas[1]);
                    textBox3.Text += localDate.ToString() + "." + localDate.ToString("fff") + "\t" + datas[0] + "\t" + datas[1] + "\r\n";
                    textBox4.Text = datas[0];
                    textBox5.Text = datas[1];
                }
                else if (datas.Length == 1)
                {
                    DateTime localDate = DateTime.Now;
                    chart1.Series["Series1"].Points.AddXY(localDate, datas[0]);
                    textBox3.Text += localDate.ToString() + "." + localDate.ToString("fff") + "\t" + datas[0] + "\r\n";
                    textBox4.Text = datas[0];
                }
            }
            catch (Exception)
            {
                serialPort1.DiscardInBuffer();
            }
        }

        private void displayText(object sender, EventArgs e)
        {
            try
            {
                if (radioButton4.Checked)
                {
                    string serIn = sp.ReadExisting();
                    textBox3.Text += serIn;
                }
                else
                {
                    byte[] s = new byte[64];
                    int leng = sp.Read(s, 0, 64);
                    textBox3.Text += BytesToStrHex(s, leng);
                }
            }
            catch (Exception)
            {
            }
        }

        private string BytesToStrHex(byte[] B, int L)
        {
            string outStr = "";
            for (int i = 0; i < L; i++)
            {
                outStr += (String.Format("{0:x} ", B[i]));
            }
            return outStr;
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                chart1.Series["Series1"].Enabled = true;
                textBox4.Enabled = true;
            }
            else
            {
                chart1.Series["Series1"].Enabled = false;
                textBox4.Enabled = false;
            }
        }
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                chart1.Series["Series2"].Enabled = true;
                textBox5.Enabled = true;
            }
            else
            {
                chart1.Series["Series2"].Enabled = false;
                textBox5.Enabled = false;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            chart1.Series["Series1"].Points.Clear();
            chart1.Series["Series2"].Points.Clear();
            textBox4.Text = "";
            textBox5.Text = "";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox2.Checked)
                {
                    if (radioButton1.Checked)
                    {
                        sendDataHex(textBox1.Text);
                        if (checkBox3.Checked) serialPort1.Write("\r");
                        if (checkBox4.Checked) serialPort1.Write("\n");
                    }
                    else {
                        serialPort1.Write(textBox1.Text);
                        if (checkBox3.Checked) serialPort1.Write("\r");
                        if (checkBox4.Checked) serialPort1.Write("\n");
                    }
                }
                else
                {
                    if (listBox1.SelectedIndex >= 0)
                    { 
                        if (listPacketType[listBox1.SelectedIndex])
                        {
                            sendDataHex(listPacket[listBox1.SelectedIndex]);
                            if (checkBox10.Checked) serialPort1.Write("\r");
                            if (checkBox9.Checked) serialPort1.Write("\n");
                        }
                        else {
                            serialPort1.Write(listPacket[listBox1.SelectedIndex]);
                            if (checkBox10.Checked) serialPort1.Write("\r");
                            if (checkBox9.Checked) serialPort1.Write("\n");
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        private void sendDataHex(String TextIn)
        {
            try
            {
                char[] delimiterChars = { ' ', ',', '.', ':', ';', '#' };
                string[] splitWords = TextIn.Split(delimiterChars);
                byte[] sendData = new byte[splitWords.Length];
                for (int i = 0; i < splitWords.Length; i++)
                {
                    String tempSplit = splitWords[i];
                    for (int k = 0; k < tempSplit.Length; k++)
                    {
                        char pow = (char)Math.Pow(2, 4 * (tempSplit.Length - k - 1));
                        if (tempSplit[k] > 47 && tempSplit[k] < 58)
                        {
                            sendData[i] += Convert.ToByte((tempSplit[k] - 48) * pow);
                        }
                        if (tempSplit[k] > 64 && tempSplit[k] < 71)
                        {
                            sendData[i] += Convert.ToByte((tempSplit[k] - 55) * pow);
                        }
                        if (tempSplit[k] > 96 && tempSplit[k] < 103)
                        {
                            sendData[i] += Convert.ToByte((tempSplit[k] - 87) * pow);
                        }
                    }
                }
                serialPort1.Write(sendData, 0, sendData.Length);             
            }
            catch (Exception)
            {
            }
        }

        string[] listPacket = new string[10];
        string[] listPacketName = new string[10];
        bool[] listPacketType = new bool[10];

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                AddPacket ap = new AddPacket();
                ap.ShowDialog();
                if (ap.isOk)
                {
                    int numItem = listBox1.Items.Count;
                    if (numItem < 10)
                    {
                        listPacket[numItem] = ap.content;
                        listPacketName[numItem] = ap.pacName;
                        listPacketType[numItem] = ap.isHex;
                        listBox1.Items.Add(ap.pacName);
                    }
                    else
                    {
                        MessageBox.Show("List of Packet has been Reach Maximum Number!");
                        button2.Enabled = false;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                int i = listBox1.SelectedIndex;
                if (i >= 0)
                {                   
                    textBox1.Text = listPacketName[i];
                    for (int j = i; j < 9; j++)
                    {
                        listPacketName[j] = listPacketName[j + 1];
                        listPacket[j] = listPacket[j + 1];
                        listPacketType[j] = listPacketType[j + 1];
                    }
                    listPacketName[9] = "";
                    listPacket[9] = "";
                    listPacketType[9] = false;
                    button2.Enabled = true;
                    listBox1.Items.Remove(listBox1.SelectedItem);
                }
            }
            catch (Exception)
            {
            }
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox5.Checked)
            {
                timer1.Enabled = true;
                timer1.Interval = Convert.ToInt32(numericUpDown1.Value);
                timer1.Start();
            }
            else
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(panel2.Enabled)
            {
                button3.PerformClick();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text File|*.txt";
            sfd.Title = "Save Received Data As ...";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = sfd.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Text = "Saving ...";
            if(textBox2.Text == "")
            {
                button6.PerformClick();
            }
            if (textBox2.Text != "")
            {
                StreamWriter sw = new StreamWriter(textBox2.Text);
                sw.Write(textBox3.Text);
                sw.Close();
                button4.Text = "Saved";
            }
            else
            {
                button4.Text = "Save ?";
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            button4.Text = "Save";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Help h = new Help();
            h.ShowDialog();
        }  
    }
}
