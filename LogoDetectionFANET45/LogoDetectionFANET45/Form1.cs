using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Json;
using System.Net;
using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;

namespace LogoDetectionFANET45
{
    public partial class Form1 : Form
    {
        Image<Gray, Byte> model;
        Image<Gray, Byte> observed;
        int algorithm;

        public Form1()
        {
            InitializeComponent();

            //populate combo box and set it's default value
            comboBox1.Items.Add("search by user"); comboBox1.Items.Add("search by tag"); comboBox1.Items.Add("search by location");
            comboBox1.SelectedItem = "search by user";
            label4.Text = "user ID";
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in this.groupBox1.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        richTextBox1.Clear();
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text);
                        algorithm = 1;
                    }
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in this.groupBox1.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        richTextBox1.Clear();
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text);
                        algorithm = 2;
                    }
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control control in this.groupBox1.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        richTextBox1.Clear();
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text);
                        algorithm = 3;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "pilih gambar model";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "jpg";

            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                model = new Image<Gray, Byte>(openFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(textBox2.Text);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            System.IO.Stream stream = httpWebReponse.GetResponseStream();
            Bitmap testing = (Bitmap)Image.FromStream(stream);
            observed = new Image<Gray, Byte>(testing);

            if (model == null || observed == null)
            {
                richTextBox1.Clear();
                richTextBox1.AppendText("pilih gambar terlebih dahulu");
            }
            else
            {
                switch (algorithm)
                {
                    case 1:
                        imageBox1.Image = Detector.FAST(model, observed);
                        break;
                    case 2:
                        imageBox1.Image = Detector.SURF(model, observed);
                        break;
                    case 3:
                        imageBox1.Image = Detector.SIFT(model, observed);
                        break;
                }
            }
        }
        
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "search by user") {
                label4.Text = "user ID";
            }
            else if (comboBox1.SelectedItem == "search by tag") {
                label4.Text = "tag";
            }
            else if (comboBox1.SelectedItem == "search by location")
            {
                label4.Text = "location ID";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == null)
            {
                richTextBox2.Clear();
                richTextBox2.AppendText("harap isi user ID/tag/location ID terlebih dahulu \n");
            }
            else {
                if (comboBox1.SelectedItem == "search by user")
                {
                    Instagram.FetchByUser(textBox3.Text);
                }
                else if (comboBox1.SelectedItem == "search by tag")
                {
                    Instagram.FetchByTag(textBox3.Text);
                }
                else if (comboBox1.SelectedItem == "search by location")
                {
                    Instagram.FetchByLocation(textBox3.Text);
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
