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
        static InstaSharp.InstagramConfig config = new InstaSharp.InstagramConfig("https://api.instagram.com/v1",
                                            "https://api.instagram.com/oauth", "10ddb3d97c6744b29e87fe0688067a95",
                                            "03391bad215a4f23b71cdf4e99e27b91", "http://localhost:8080");

        static InstaSharp.Endpoints.Locations.Unauthenticated IG = new InstaSharp.Endpoints.Locations.Unauthenticated(config);

        String result = IG.RecentJson("40174");

        //{1,2,3,4,5,6,"images":{"low_resolution":{"url":"http:\/\/scontent-b.cdninstagram.com\/hphotos-xaf1\/t51.2885-15\/10661047_567786680013411_1236800010_a.jpg","width":306,"height":306},"thumbnail":{"url":"http:\/\/scontent-b.cdninstagram.com\/hphotos-xaf1\/t51.2885-15\/10661047_567786680013411_1236800010_s.jpg","width":150,"height":150},"standard_resolution":{"url":"http:\/\/scontent-b.cdninstagram.com\/hphotos-xaf1\/t51.2885-15\/10661047_567786680013411_1236800010_n.jpg","width":640,"height":640}},"users_in_photo":[],"caption":{"created_time":"1409629703","text":"? ??? ?? ???? \u000a??, ??, ??, ????, ?? ??\u000a20? ??? ?? ????\u000a?? ? ????;;\u000a?? ???? ??? ?? ?? ??? ?? ? ????? ??? ?? ??? ????^^*\u000a#????#?????#????\u000a#??????#??????","from":{"username":"carrotjjung","profile_picture":"http:\/\/photos-h.ak.instagram.com\/hphotos-ak-xpf1\/10507859_338023903013191_1834813196_a.jpg","id":"575533907","full_name":"???, ???, ??? ??~"},"id":"800354416329560598"},"type":"image","id":"800354415817855219_575533907","user":{"username":"carrotjjung","website":"","profile_picture":"http:\/\/photos-h.ak.instagram.com\/hphotos-ak-xpf1\/10507859_338023903013191_1834813196_a.jpg","full_name":"???, ???, ??? ??~","bio":"","id":"575533907"}}


        JsonValue result2 = JsonValue.Parse(IG.RecentJson("40174"));

        public Form1()
        {
            InitializeComponent();
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        public Image<Bgr, Byte> FAST(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            long matchTime;
            Stopwatch watch;

            HomographyMatrix homography = null;

            FastDetector fastCPU = new FastDetector(10, true);
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            BriefDescriptorExtractor descriptor = new BriefDescriptorExtractor();

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            watch = Stopwatch.StartNew();

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

            watch.Stop();

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

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion

            matchTime = watch.ElapsedMilliseconds;
            richTextBox1.Clear();
            richTextBox1.AppendText("waktu pendeteksian FAST: " + matchTime + "ms\n");
            richTextBox1.AppendText("fitur model yang terdeteksi: " + modelKeyPoints.Size + "\n");
            richTextBox1.AppendText("match yang ditemukan: " + CvInvoke.cvCountNonZero(mask).ToString());

            return result;
        }

        public Image<Bgr, Byte> SURF(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            long matchTime;
            Stopwatch watch;
            HomographyMatrix homography = null;

            SURFDetector surfCPU = new SURFDetector(500, false);
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            watch = Stopwatch.StartNew();

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
                using (Stream stream = new Stream())
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

                    watch.Stop();
                }
            }

            ////extract features from the object image
            //modelKeyPoints = surfCPU.DetectKeyPointsRaw(modelImage, null);
            //Matrix<float> modelDescriptors = surfCPU.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);

            //// extract features from the observed image
            //observedKeyPoints = surfCPU.DetectKeyPointsRaw(observedImage, null);
            //Matrix<float> observedDescriptors = surfCPU.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);
            //BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            //matcher.Add(modelDescriptors);

            //indices = new Matrix<int>(observedDescriptors.Rows, k);
            //using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
            //{
            //    matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
            //    mask = new Matrix<byte>(dist.Rows, 1);
            //    mask.SetValue(255);
            //    Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
            //}

            //int nonZeroCount = CvInvoke.cvCountNonZero(mask);
            //if (nonZeroCount >= 4)
            //{
            //    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
            //    if (nonZeroCount >= 4)
            //        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
            //}

            //watch.Stop();

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

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion

            matchTime = watch.ElapsedMilliseconds;
            richTextBox1.Clear();
            richTextBox1.AppendText("waktu pendeteksian SURF: " + matchTime + "ms\n");
            richTextBox1.AppendText("fitur model yang terdeteksi: " + modelKeyPoints.Size + "\n");
            richTextBox1.AppendText("match yang ditemukan: " + CvInvoke.cvCountNonZero(mask).ToString());
            return result;
        }

        public Image<Bgr, Byte> SIFT(Image<Gray, Byte> modelImage, Image<Gray, byte> observedImage)
        {
            long matchTime;
            Stopwatch watch;
            HomographyMatrix homography = null;

            SIFTDetector siftCPU = new SIFTDetector();
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;

            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;

            watch = Stopwatch.StartNew();

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

            watch.Stop();

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

                result.DrawPolyline(Array.ConvertAll<PointF, Point>(pts, Point.Round), true, new Bgr(Color.LightGreen), 5);
            }
            #endregion

            matchTime = watch.ElapsedMilliseconds;
            richTextBox1.Clear();
            richTextBox1.AppendText("waktu pendeteksian SIFT: " + matchTime + "ms\n");
            richTextBox1.AppendText("fitur mentah pada model yang terdeteksi: " + modelKeyPoints.Size + "\n");
            richTextBox1.AppendText("match yang ditemukan: " + CvInvoke.cvCountNonZero(mask).ToString());
            return result;
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
                        richTextBox1.AppendText(radio.Text + "\n");
                        for (int i = 0; i < result2["data"].Count; i++) {
                            string output = (string)result2["data"][i]["images"]["standard_resolution"]["url"];
                            Console.WriteLine(output.Replace(@"\", String.Empty) + "\n");
                        }
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
                        richTextBox1.AppendText(radio.Text);
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
                        richTextBox1.AppendText(radio.Text);
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

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "pilih gambar target";

            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "jpg";

            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.ReadOnlyChecked = true;
            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
                observed = new Image<Gray, Byte>(openFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
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
                        imageBox1.Image = FAST(model, observed);
                        break;
                    case 2:
                        imageBox1.Image = SURF(model, observed);
                        break;
                    case 3:
                        imageBox1.Image = SIFT(model, observed);
                        break;
                }
            }
        }
    }
}
