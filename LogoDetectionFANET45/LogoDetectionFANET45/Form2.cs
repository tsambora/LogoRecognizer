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
        List<Image<Gray, Byte>> ModelList = new List<Image<Gray, Byte>>();
        List<String> testLabel = new List<String>();
        List<Image<Gray, byte>> ObservedList = new List<Image<Gray, byte>>();

        int algorithm;

        public Form2()
        {
            InitializeComponent();
        }

        public bool testSIFT(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            bool isFound = false;
            HomographyMatrix homography = null;

            SIFTDetector siftCPU = new SIFTDetector();
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            //extract features from the object image
            modelKeyPoints = siftCPU.DetectKeyPointsRaw(modelImage, null);
            Matrix<float> modelDescriptors = siftCPU.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

            // extract features from the observed image
            observedKeyPoints = siftCPU.DetectKeyPointsRaw(observedImage, null);
            Matrix<float> observedDescriptors = siftCPU.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            matcher.Add(modelDescriptors);

            indices = new Matrix<int>(observedDescriptors.Rows, k);
            using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
            {
                matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                mask = new Matrix<byte>(dist.Rows, 1);
                mask.SetValue(255);
                Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
            }

            int nonZeroCount = CvInvoke.cvCountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
            }

            //Draw the matched keypoints
            Image<Bgr, Byte> result = Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
               indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            #region draw the projected region on the image
            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = new PointF[] { 
                   new PointF(rect.Left, rect.Bottom),
                   new PointF(rect.Right, rect.Bottom),
                   new PointF(rect.Right, rect.Top),
                   new PointF(rect.Left, rect.Top)};
                homography.ProjectPoints(pts);

                if (CvInvoke.cvCountNonZero(mask) >= 10)
                    isFound = true;

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion
            return isFound;
        }

        public String runSIFTTest(List<Image<Gray, Byte>> models, Image<Gray, byte> observed, bool log) {
            RichTextBox _richTextBox1 = (RichTextBox)Application.OpenForms["Form2"].Controls.Find("richTextBox1", false).FirstOrDefault();
            bool[] res = new bool[models.Count];

            int i = 0;
            foreach (Image<Gray, Byte> model in models) {
                res[i] = testSIFT(models[i], observed);
                i++;
            }

            String label = "";
            List<TextBox> boxes =
                new List<TextBox> { 
                    textBox1,
                    textBox2,
                    textBox3,
                    textBox4,
                    textBox5
                };

            if (log == true)
                _richTextBox1.AppendText(" Hasil deteksi: ");

            for (int j = 0; j < res.Length; j++)
            {
                if (res[j] == true)
                {
                    label = boxes[j].Text;
                }
            }
            if (label == "")
            {
                label = "tidak ditemukan logo";
            }

            if (log == true)
                _richTextBox1.AppendText("'" + label.ToUpper() + "'");

            return label;
        }

        public bool testSURF(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            bool isFound = false;

            HomographyMatrix homography = null;

            SURFDetector surfCPU = new SURFDetector(500, false);
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            GpuSURFDetector surfGPU = new GpuSURFDetector(surfCPU.SURFParams, 0.01f);
            using (GpuImage<Gray, Byte> gpuModelImage = new GpuImage<Gray, byte>(modelImage))
            //extract features from the object image
            using (GpuMat<float> gpuModelKeyPoints = surfGPU.DetectKeyPointsRaw(gpuModelImage, null))
            using (GpuMat<float> gpuModelDescriptors = surfGPU.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
            using (GpuBruteForceMatcher<float> matcher = new GpuBruteForceMatcher<float>(DistanceType.L2))
            {
                modelKeyPoints = new VectorOfKeyPoint();
                surfGPU.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                //watch = Stopwatch.StartNew();

                // extract features from the observed image
                using (GpuImage<Gray, Byte> gpuObservedImage = new GpuImage<Gray, byte>(observedImage))
                using (GpuMat<float> gpuObservedKeyPoints = surfGPU.DetectKeyPointsRaw(gpuObservedImage, null))
                using (GpuMat<float> gpuObservedDescriptors = surfGPU.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                using (GpuMat<int> gpuMatchIndices = new GpuMat<int>(gpuObservedDescriptors.Size.Height, k, 1, true))
                using (GpuMat<float> gpuMatchDist = new GpuMat<float>(gpuObservedDescriptors.Size.Height, k, 1, true))
                using (GpuMat<Byte> gpuMask = new GpuMat<byte>(gpuMatchIndices.Size.Height, 1, 1))
                using (Emgu.CV.GPU.Stream stream = new Emgu.CV.GPU.Stream())
                {
                    matcher.KnnMatchSingle(gpuObservedDescriptors, gpuModelDescriptors, gpuMatchIndices, gpuMatchDist, k, null, stream);
                    indices = new Matrix<int>(gpuMatchIndices.Size);
                    mask = new Matrix<byte>(gpuMask.Size);

                    //gpu implementation of voteForUniquess
                    using (GpuMat<float> col0 = gpuMatchDist.Col(0))
                    using (GpuMat<float> col1 = gpuMatchDist.Col(1))
                    {
                        GpuInvoke.Multiply(col1, new MCvScalar(uniquenessThreshold), col1, stream);
                        GpuInvoke.Compare(col0, col1, gpuMask, CMP_TYPE.CV_CMP_LE, stream);
                    }

                    observedKeyPoints = new VectorOfKeyPoint();
                    surfGPU.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                    //wait for the stream to complete its tasks
                    //We can perform some other CPU intesive stuffs here while we are waiting for the stream to complete.
                    stream.WaitForCompletion();

                    gpuMask.Download(mask);
                    gpuMatchIndices.Download(indices);

                    if (GpuInvoke.CountNonZero(gpuMask) >= 4)
                    {
                        int nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
                    }
                }
            }

            //Draw the matched keypoints
            Image<Bgr, Byte> result = Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
               indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            #region draw the projected region on the image
            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                homography.ProjectPoints(pts);

                if (CvInvoke.cvCountNonZero(mask) >= 10)
                    isFound = true;

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion
            return isFound;
        }

        public String runSURFTest(List<Image<Gray, Byte>> models, Image<Gray, byte> observed, bool log)
        {
            RichTextBox _richTextBox1 = (RichTextBox)Application.OpenForms["Form2"].Controls.Find("richTextBox1", false).FirstOrDefault();
            bool[] res = new bool[models.Count];

            int i = 0;
            foreach (Image<Gray, Byte> model in models)
            {
                res[i] = testSURF(models[i], observed);
                i++;
            }

            String label = "";
            List<TextBox> boxes =
                new List<TextBox> { 
                    textBox1,
                    textBox2,
                    textBox3,
                    textBox4,
                    textBox5
                };
            if (log == true)
                _richTextBox1.AppendText(" Hasil deteksi: "); 

            for (int j = 0; j < res.Length; j++)
            {
                if (res[j] == true)
                {
                    label = boxes[j].Text;
                }
            }
            if (label == "")
            {
                label = "tidak ditemukan logo";
            }

            if(log == true)
                _richTextBox1.AppendText("'" + label.ToUpper() + "'");
            
            return label;
        }

        public bool testFAST(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            bool isFound = false;

            HomographyMatrix homography = null;

            FastDetector fastCPU = new FastDetector(10, true);
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            BriefDescriptorExtractor descriptor = new BriefDescriptorExtractor();

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            //extract features from the object image
            modelKeyPoints = fastCPU.DetectKeyPointsRaw(modelImage, null);
            Matrix<Byte> modelDescriptors = descriptor.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

            // extract features from the observed image
            observedKeyPoints = fastCPU.DetectKeyPointsRaw(observedImage, null);
            Matrix<Byte> observedDescriptors = descriptor.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
            BruteForceMatcher<Byte> matcher = new BruteForceMatcher<Byte>(DistanceType.L2);
            matcher.Add(modelDescriptors);

            indices = new Matrix<int>(observedDescriptors.Rows, k);
            using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
            {
                matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
                mask = new Matrix<byte>(dist.Rows, 1);
                mask.SetValue(255);
                Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
            }

            int nonZeroCount = CvInvoke.cvCountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(
                    modelKeyPoints, observedKeyPoints, indices, mask, 2);
            }

            //Draw the matched keypoints
            Image<Bgr, Byte> result = Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
               indices, new Bgr(255, 255, 255), new Bgr(255, 255, 255), mask, Features2DToolbox.KeypointDrawType.DEFAULT);

            #region draw the projected region on the image
            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = new PointF[] { 
                 new PointF(rect.Left, rect.Bottom),
                 new PointF(rect.Right, rect.Bottom),
                 new PointF(rect.Right, rect.Top),
                 new PointF(rect.Left, rect.Top)};
                homography.ProjectPoints(pts);

                if (CvInvoke.cvCountNonZero(mask) >= 10)
                    isFound = true;

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion
            return isFound;
        }

        public String runFASTTest(List<Image<Gray, Byte>> models, Image<Gray, byte> observed, bool log)
        {
            RichTextBox _richTextBox1 = (RichTextBox)Application.OpenForms["Form2"].Controls.Find("richTextBox1", false).FirstOrDefault();
            bool[] res = new bool[models.Count];

            int i = 0;
            foreach (Image<Gray, Byte> model in models)
            {
                res[i] = testFAST(models[i], observed);
                i++;
            }

            String label = "";
            List<TextBox> boxes =
                new List<TextBox> { 
                    textBox1,
                    textBox2,
                    textBox3,
                    textBox4,
                    textBox5
                };

            if (log == true)
                _richTextBox1.AppendText(" Hasil deteksi: ");

            for (int j = 0; j < res.Length; j++)
            {
                if (res[j] == true)
                {
                    label = boxes[j].Text;
                }
            }
            if (label == "")
            {
                label = "tidak ditemukan logo";
            }

            if (log == true)
                _richTextBox1.AppendText("'" + label.ToUpper() + "'");

            return label;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                ModelList.Add(new Image<Gray, Byte>(openFileDialog1.FileName));
                imageBox1.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox1.Size = new Size(50, 50);
                imageBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox2_Click(object sender, EventArgs e)
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
                ModelList.Add(new Image<Gray, Byte>(openFileDialog1.FileName));
                imageBox2.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox2.Size = new Size(50, 50);
                imageBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox3_Click(object sender, EventArgs e)
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
                ModelList.Add(new Image<Gray, Byte>(openFileDialog1.FileName));
                imageBox3.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox3.Size = new Size(50, 50);
                imageBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox4_Click(object sender, EventArgs e)
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
                ModelList.Add(new Image<Gray, Byte>(openFileDialog1.FileName));
                imageBox4.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox4.Size = new Size(50, 50);
                imageBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox5_Click(object sender, EventArgs e)
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
                ModelList.Add(new Image<Gray, Byte>(openFileDialog1.FileName));
                imageBox5.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox5.Size = new Size(50, 50);
                imageBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox10_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox10.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox10.Size = new Size(50, 50);
                imageBox10.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox9_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox9.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox9.Size = new Size(50, 50);
                imageBox9.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox8_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox8.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox8.Size = new Size(50, 50);
                imageBox8.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox7_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox7.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox7.Size = new Size(50, 50);
                imageBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox6_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox6.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox6.Size = new Size(50, 50);
                imageBox6.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox15_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox15.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox15.Size = new Size(50, 50);
                imageBox15.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox14_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox14.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox14.Size = new Size(50, 50);
                imageBox14.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox13_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox13.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox13.Size = new Size(50, 50);
                imageBox13.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox12_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox12.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox12.Size = new Size(50, 50);
                imageBox12.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox11_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox11.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox11.Size = new Size(50, 50);
                imageBox11.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox20_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox20.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox20.Size = new Size(50, 50);
                imageBox20.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox19_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox19.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox19.Size = new Size(50, 50);
                imageBox19.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox18_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox18.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox18.Size = new Size(50, 50);
                imageBox18.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox17_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox17.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox17.Size = new Size(50, 50);
                imageBox17.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox16_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox16.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox16.Size = new Size(50, 50);
                imageBox16.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox25_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox25.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox25.Size = new Size(50, 50);
                imageBox25.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox24_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox24.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox24.Size = new Size(50, 50);
                imageBox24.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox23_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox23.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox23.Size = new Size(50, 50);
                imageBox23.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox22_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox22.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox22.Size = new Size(50, 50);
                imageBox22.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox21_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox21.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox21.Size = new Size(50, 50);
                imageBox21.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox30_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox30.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox30.Size = new Size(50, 50);
                imageBox30.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox29_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox29.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox29.Size = new Size(50, 50);
                imageBox29.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox28_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox28.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox28.Size = new Size(50, 50);
                imageBox28.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox27_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox27.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox27.Size = new Size(50, 50);
                imageBox27.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox26_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox26.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox26.Size = new Size(50, 50);
                imageBox26.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox35_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox35.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox35.Size = new Size(50, 50);
                imageBox35.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox34_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox34.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox34.Size = new Size(50, 50);
                imageBox34.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox33_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox33.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox33.Size = new Size(50, 50);
                imageBox33.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox32_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox32.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox32.Size = new Size(50, 50);
                imageBox32.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox31_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox31.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox31.Size = new Size(50, 50);
                imageBox31.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox40_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox40.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox40.Size = new Size(50, 50);
                imageBox40.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox39_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox39.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox39.Size = new Size(50, 50);
                imageBox39.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox38_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox38.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox38.Size = new Size(50, 50);
                imageBox38.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox37_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox37.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox37.Size = new Size(50, 50);
                imageBox37.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox36_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox36.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox36.Size = new Size(50, 50);
                imageBox36.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox45_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox45.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox45.Size = new Size(50, 50);
                imageBox45.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox44_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox44.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox44.Size = new Size(50, 50);
                imageBox44.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox43_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox43.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox43.Size = new Size(50, 50);
                imageBox43.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox42_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox42.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox42.Size = new Size(50, 50);
                imageBox42.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox41_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox41.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox41.Size = new Size(50, 50);
                imageBox41.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox50_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox50.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox50.Size = new Size(50, 50);
                imageBox50.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox49_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox49.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox49.Size = new Size(50, 50);
                imageBox49.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox48_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox48.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox48.Size = new Size(50, 50);
                imageBox48.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox47_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox47.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox47.Size = new Size(50, 50);
                imageBox47.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void imageBox46_Click(object sender, EventArgs e)
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
                ObservedList.Add(new Image<Gray, byte>(openFileDialog1.FileName));
                imageBox46.Image = new Image<Bgr, Byte>(openFileDialog1.FileName);
                imageBox46.Size = new Size(50, 50);
                imageBox46.SizeMode = PictureBoxSizeMode.StretchImage;
            }
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
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text + "\n");
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
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text + "\n");
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
                        richTextBox1.AppendText("algoritma terpilih: " + radio.Text + "\n");
                        algorithm = 3;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            float n, acc, perc;
            if (ModelList.Count == 0 || ObservedList.Count == 0)
            {
                richTextBox1.Clear();
                richTextBox1.AppendText("pilih gambar terlebih dahulu");
            }
            else
            {
                switch (algorithm)
                {
                    case 1:
                        n = 1;
                        acc = 0;
                        testLabel.Clear();
                        for (int i = 0; i < ObservedList.Count; i++) {
                            richTextBox1.AppendText(n + ". ");
                            testLabel.Add(runFASTTest(ModelList, ObservedList[i], true));
                            richTextBox1.AppendText(", jawaban yang benar: " + "'" + db.labels[i].ToUpper() + "'");
                            if (runFASTTest(ModelList, ObservedList[i], false) == db.labels[i])
                            {
                                acc++;
                                richTextBox1.AppendText("=> BETUL \n");
                            }
                            else
                            {
                                richTextBox1.AppendText("=> SALAH \n");
                            }
                            n++;
                        }
                        perc = (float)(acc / testLabel.Count) * 100;
                        richTextBox1.AppendText("Akurasi tes FAST:" + perc + "%\n");
                        break;
                    case 2:
                        n = 1;
                        acc = 0;
                        testLabel.Clear();
                        for (int i = 0; i < ObservedList.Count; i++) {
                            richTextBox1.AppendText(n + ". ");
                            testLabel.Add(runSURFTest(ModelList, ObservedList[i], true));
                            richTextBox1.AppendText(", jawaban yang benar: " + "'" + db.labels[i].ToUpper() + "'");
                            if (runSURFTest(ModelList, ObservedList[i], false) == db.labels[i])
                            {
                                acc++;
                                richTextBox1.AppendText("=> BETUL \n");
                            }
                            else
                            {
                                richTextBox1.AppendText("=> SALAH \n");
                            }
                            n++;
                        }
                        perc = acc / testLabel.Count * 100;
                        richTextBox1.AppendText("Akurasi tes SURF:" + perc + "%\n");
                        break;
                    case 3:
                        n = 1;
                        acc = 0;
                        testLabel.Clear();
                        for (int i = 0; i < ObservedList.Count; i++) {
                            richTextBox1.AppendText(n + ". ");
                            testLabel.Add(runSIFTTest(ModelList, ObservedList[i], true));
                            richTextBox1.AppendText(", jawaban yang benar: " + "'" + db.labels[i].ToUpper() + "'");
                            if (runSIFTTest(ModelList, ObservedList[i], false) == db.labels[i])
                            {
                                acc++;
                                richTextBox1.AppendText("=> BETUL \n");
                            }
                            else
                            {
                                richTextBox1.AppendText("=> SALAH \n");
                            }
                            n++;
                        }
                        perc = acc / testLabel.Count * 100;
                        richTextBox1.AppendText("Akurasi tes SIFT:" + perc + "%\n");
                        break;
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ObservedList.Clear();
            foreach (string dir in db.images) {

                Bitmap bmpImage = new Bitmap(Image.FromStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(dir)));
                Emgu.CV.Image<Gray, byte> imageOut = new Emgu.CV.Image<Gray, byte>(bmpImage);
                ObservedList.Add(imageOut);
            }
            List<ImageBox> boxes =
                new List<ImageBox> { 
                    imageBox10,
                    imageBox9,
                    imageBox8,
                    imageBox7,
                    imageBox6,
                    imageBox15,
                    imageBox14,
                    imageBox13,
                    imageBox12,
                    imageBox11,
                    imageBox20,
                    imageBox19,
                    imageBox18,
                    imageBox17,
                    imageBox16,
                    imageBox25,
                    imageBox24,
                    imageBox23,
                    imageBox22,
                    imageBox21,
                    imageBox30,
                    imageBox29,
                    imageBox28,
                    imageBox27,
                    imageBox26,
                    imageBox35,
                    imageBox34,
                    imageBox33,
                    imageBox32,
                    imageBox31,
                    imageBox40,
                    imageBox39,
                    imageBox38,
                    imageBox37,
                    imageBox36,
                    imageBox45,
                    imageBox44,
                    imageBox43,
                    imageBox42,
                    imageBox41,
                    imageBox50,
                    imageBox49,
                    imageBox48,
                    imageBox47,
                    imageBox46
                };
            for (int i = 0; i < db.images.Count; i++) {
                Bitmap bmpImage = new Bitmap(Image.FromStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(db.images[i])));
                boxes[i].Image = new Emgu.CV.Image<Rgba, Byte>(bmpImage);
                boxes[i].SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}
