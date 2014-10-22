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
using Emgu.CV.UI;

namespace LogoDetectionFANET45
{
    public partial class Form2 : Form
    {
        Point modelsPos = new Point(18, 75);
        Point observedPos;
        Image<Gray, Byte>[] testModels = new Image<Gray,Byte>[5];
        List<Image<Gray, Byte>> testObserved;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        void changeModelsPos(Point currentPos) {
            if (currentPos.Y >= 345)
            {
                currentPos.X = currentPos.X + 55;
                currentPos.Y = 75;
            }
            else
                currentPos.Y += 55;
        }

        private void imageBox1_Click(object sender, EventArgs e)
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
                testModels[0] = new Image<Gray, Byte>(openFileDialog1.FileName);
                imageBox1.Image = new Image<Gray, Byte>(openFileDialog1.FileName);
                imageBox1.Location = modelsPos;
                imageBox1.Size = new Size(50, 50);
                imageBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
