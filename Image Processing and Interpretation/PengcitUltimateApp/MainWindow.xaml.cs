using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Drawing;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows;
using System.Drawing.Imaging;

namespace PengcitUltimateApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap original = null;
        private Bitmap processed = null;

        private List<List<int>> chaincodes = new List<List<int>>();

        List<String> stringCodes = new List<String>();

        //variabel knowledge base
        List<String> codeBase = new List<String>();
        List<String> labels = new List<String>();

        bool ispreprocessed = false;
        Color preprocessed = Color.FromArgb(255,255,255);
        Color unpreprocessed = Color.FromArgb(0, 0, 0);

        public MainWindow()
        {
            InitializeComponent();
            //inisialisasi knowledge base
            codeBase.Add("222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222222224444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666660000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            codeBase.Add("22222222222322223223223233232333233333433343343434344344434444344444444444454444544454454545454555545555556556556565665665666656666666666676666766766767767677767777707770707707070070007000070000000000001000010001001011010101111011121111212112122122122221");
            codeBase.Add("223343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343343445666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666667001101101101101101101101101101101101101101101110110110110110110110110110110110110110110110110111011011011011011011011011011011011011011011011101");
            codeBase.Add("244444444444444444444444444444444444444444444444444444444444444444444444444343323222224666666666666666666666666666602222212111000000000000000000000000000000000000000000000000000000000000007077766665650121212121212122121");
            codeBase.Add("22222444444444444444444444444444444444444444444444444444322222222244444446666666665444444444444444444446666666600000000000000000000766666666666666666666666666666666600000001110110110111011011011101101101101110110110111011011");
            codeBase.Add("22222232232332333343343343434434443444434444444444444444444445444454454454454554554555655656566566666676676767777707707070707007007000070000000000000000000010000100010010101010111111112121221");
            codeBase.Add("22222223223232333433443444444444544545454554555455555554555555532222222222222222222121444444444444466666666666666666666666666666666666601101111101111011101110110101010100010000000007077777676666665665555546001010101011112121221");
            codeBase.Add("222222222222222222222222222245445454454666666666666666666666666654454532222232222322232322332333333434343444444444445445454555555656566566656666666666766676777000010212223232332333232322222121111100000000070707777767767667667666766666666660010010100100100100100101001001");
            codeBase.Add("22222222222222222222222222222222224444444454454454454454454454454454454454544544544544544544544546666661001001001001001001001001001001001001001001001001007666666666666666666656665655545000100010001000010");
            codeBase.Add("22222222223222323233343434443544445455555656533333333343434444344544445454555565566566566666666666766767677770707000000000101011112112210776777777070700000000001011012111221221");

            labels.Add("rectangle");
            labels.Add("circle");
            labels.Add("triangle");
            labels.Add("angka satu");
            labels.Add("angka empat");
            labels.Add("angka nol");
            labels.Add("angka dua");
            labels.Add("angka lima");
            labels.Add("angka tujuh");
            labels.Add("angka delapan");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Memuat Gambar";
            op.Filter = "Semua file grafik|*.jpg;*.jpeg;*.png|" +
                "JPEG (jpg, jpeg)|*.jpg;*jpeg|" +
                "Portable Network Graphic|*.png";
            if (op.ShowDialog() == true)
            {
                image1.Source = new BitmapImage(new Uri(op.FileName));
                original = new Bitmap(op.FileName);

                image2.Source = new BitmapImage(new Uri(op.FileName));
                processed = new Bitmap(op.FileName);

                chaincodes.Clear();
                stringCodes.Clear();
                ispreprocessed = false;
            }
        }

        private void image1_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox2 != null)
            {
                if (comboBox1.SelectedValue.ToString().Equals("Matrix 1 Degree"))
                {
                    comboBox2.Visibility = Visibility.Visible;
                    comboBox3.Visibility = Visibility.Hidden;
                }
                else if (comboBox1.SelectedValue.ToString().Equals("Matrix 2 Degree"))
                {
                    comboBox2.Visibility = Visibility.Hidden;
                    comboBox3.Visibility = Visibility.Visible;
                }
                else
                {
                    comboBox2.Visibility = Visibility.Hidden;
                    comboBox3.Visibility = Visibility.Hidden;
                }
            }
        }

        private Bitmap Grayscale(Bitmap image)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    int value = (int)(result.GetPixel(i, j).R * 0.299 + result.GetPixel(i, j).G * 0.587 + result.GetPixel(i, j).B * 0.114);
                    Color color = Color.FromArgb(value, value, value);
                    result.SetPixel(i, j, color);
                }
            }
            return result;
        }

        private Bitmap Sharpen(Bitmap image)
        {
            int range = 3;

            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    int rmax = 0;
                    int gmax = 0;
                    int bmax = 0;

                    for (int ni = i - range / 2; ni < (i + range / 2); ni++)
                    { 
                        for (int nj = j - range / 2; nj < (j + range / 2); nj++)
                        {
                            ni = (ni % image.Width < 0) ? image.Width + ni : ni % image.Width;
                            nj = (nj % image.Height < 0) ? image.Height + nj : nj % image.Height;
                            
                            Color color = image.GetPixel(ni, nj);
                            rmax = (rmax < color.R) ? color.R : rmax;
                            gmax = (gmax < color.G) ? color.G : gmax;
                            bmax = (bmax < color.B) ? color.B : bmax;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(rmax, gmax, bmax));
                }
            }
            return result;
        }

        private Bitmap Contrast(Bitmap image)
        {
            int rmin = 0;
            int rmax = 255;
            int gmin = 0;
            int gmax = 255;
            int bmin = 0;
            int bmax = 255;

            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = result.GetPixel(i, j);
                    rmax = (color.R <= rmax) ? color.R : rmax;
                    rmin = (color.R >= rmin) ? color.R : rmin;
                    gmax = (color.G <= gmax) ? color.G : gmax;
                    gmin = (color.G >= gmin) ? color.G : gmin;
                    bmax = (color.B <= bmax) ? color.B : bmax;
                    bmin = (color.B >= bmin) ? color.B : bmin;
                }
            }
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = result.GetPixel(i, j);
                    int red = 255 * (Math.Abs(color.R - rmax)) / (Math.Abs(rmin - rmax));
                    int green = 255 * (Math.Abs(color.G - gmax)) / (Math.Abs(gmin - gmax));
                    int blue = 255 * (Math.Abs(color.B - bmax)) / (Math.Abs(bmin - bmax));
                    result.SetPixel(i, j, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }

        private Bitmap Normalisasi(Bitmap image)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = result.GetPixel(j, i);

                    if (color.R < 255 && color.R > 128 || color.G < 255 && color.G > 128 || color.B < 255 && color.B > 128)
                    {
                        result.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                    }
                    else if (color.R > 0 && color.R <= 128 || color.G > 0 && color.G <= 128 || color.B > 0 && color.B <= 128)
                        result.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                }
            }
            return result;
        }

        private Bitmap AddFrame(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width + 2, image.Height + 2);

            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    Color color = Color.FromArgb(255, 255, 255);
                    result.SetPixel(i, j, color);
                }
            }

            // isi pojok
            result.SetPixel(0, 0, image.GetPixel(0, 0));
            result.SetPixel(result.Width - 1, 0, image.GetPixel(image.Width - 1, 0));
            result.SetPixel(0, result.Height - 1, image.GetPixel(0, image.Height - 1));
            result.SetPixel(result.Width - 1, result.Height - 1, image.GetPixel(image.Width - 1, image.Height - 1));

            // isi pinggiran

            // isi tengah
            for (int i = 1; i < result.Width - 1; i++)
            {
                for (int j = 1; j < result.Height - 1; j++)
                {
                    Color color = image.GetPixel(i - 1, j - 1);
                    result.SetPixel(i, j, color);
                }
            }

            return result;
        }

        private Bitmap EdgeDiff4(Bitmap image)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            Bitmap framed = AddFrame(result);

            for (int i = 1; i < framed.Width - 1; i++)
            {
                for (int j = 1; j < framed.Height - 1; j++)
                {
                    Color color1;
                    Color color2;
                    Color color3;
                    Color color4;

                    color1 = Color.FromArgb(Math.Abs(framed.GetPixel(i, j - 1).R - framed.GetPixel(i, j + 1).R),
                        Math.Abs(framed.GetPixel(i, j - 1).G - framed.GetPixel(i, j + 1).G),
                        Math.Abs(framed.GetPixel(i, j - 1).B - framed.GetPixel(i, j + 1).B)
                        );

                    color2 = Color.FromArgb(Math.Abs(framed.GetPixel(i + 1, j).R - framed.GetPixel(i - 1, j).R),
                            Math.Abs(framed.GetPixel(i + 1, j).G - framed.GetPixel(i - 1, j).G),
                            Math.Abs(framed.GetPixel(i + 1, j).B - framed.GetPixel(i - 1, j).B)
                            );

                    color3 = Color.FromArgb(Math.Abs(framed.GetPixel(i + 1, j + 1).R - framed.GetPixel(i - 1, j - 1).R),
                        Math.Abs(framed.GetPixel(i + 1, j + 1).G - framed.GetPixel(i - 1, j - 1).G),
                        Math.Abs(framed.GetPixel(i + 1, j + 1).B - framed.GetPixel(i - 1, j - 1).B)
                        );

                    color4 = Color.FromArgb(Math.Abs(framed.GetPixel(i + 1, j - 1).R - framed.GetPixel(i - 1, j + 1).R),
                            Math.Abs(framed.GetPixel(i + 1, j - 1).G - framed.GetPixel(i - 1, j + 1).G),
                            Math.Abs(framed.GetPixel(i + 1, j - 1).B - framed.GetPixel(i - 1, j + 1).B)
                            );

                    int rmax = Math.Max(color1.R, Math.Max(color2.R, Math.Max(color3.R, color4.R)));
                    int gmax = Math.Max(color1.G, Math.Max(color2.G, Math.Max(color3.G, color4.G)));
                    int bmax = Math.Max(color1.B, Math.Max(color2.B, Math.Max(color3.B, color4.B)));

                    result.SetPixel(i - 1, j - 1, Color.FromArgb(rmax, gmax, bmax));
                }
            }
            return result;
        }

        private Bitmap EdgeDiff2(Bitmap image)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            Bitmap framed = AddFrame(result);

            for (int i = 1; i < framed.Width - 1; i++)
            {
                for (int j = 1; j < framed.Height - 1; j++)
                {
                    Color color1;
                    Color color2;

                    color1 = Color.FromArgb(Math.Abs(framed.GetPixel(i, j - 1).R - framed.GetPixel(i, j + 1).R),
                        Math.Abs(framed.GetPixel(i, j - 1).G - framed.GetPixel(i, j + 1).G),
                        Math.Abs(framed.GetPixel(i, j - 1).B - framed.GetPixel(i, j + 1).B)
                        );

                    color2 = Color.FromArgb(Math.Abs(framed.GetPixel(i + 1, j).R - framed.GetPixel(i - 1, j).R),
                            Math.Abs(framed.GetPixel(i + 1, j).G - framed.GetPixel(i - 1, j).G),
                            Math.Abs(framed.GetPixel(i + 1, j).B - framed.GetPixel(i - 1, j).B)
                            );

                    int rmax = Math.Max(color1.R, color2.R);
                    int gmax = Math.Max(color1.G, color2.G);
                    int bmax = Math.Max(color1.B, color2.B);

                    result.SetPixel(i - 1, j - 1, Color.FromArgb(rmax, gmax, bmax));
                }
            }
            return result;
        }

        private Bitmap Thinning(Bitmap image) {

            Console.WriteLine("masuk thinning");
            int a, b;
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            image = Normalisasi(image);
            result = Normalisasi(result);

            List<System.Drawing.Point> pointsToChange = new List<System.Drawing.Point>();
            bool hasChange;

            do{
                hasChange = false;

                for (int j = 1; j + 1 < image.Height; j++){
                    for(int i = 1; i + 1 < image.Width; i++){
                        a = getA(image, j, i);
                        b = getB(image, j, i);
                        Console.WriteLine("getA = " + a + " getB = " + b);

                        if ( image.GetPixel(j,i).R == 0 && 2 <= b && b <= 6 && a == 1
                            && (image.GetPixel(j-1,i).R * image.GetPixel(j,i+1).R * image.GetPixel(j + 1,i).R == 0)
                            && (image.GetPixel(j,i+1).R * image.GetPixel(j+1,i).R * image.GetPixel(j,i-1).R == 0)) {
                                pointsToChange.Add(new System.Drawing.Point(i, j));
 
                                hasChange = true;
                        }
                    }
                }
                foreach (System.Drawing.Point point in pointsToChange) {
                    result.SetPixel(point.X, point.Y, Color.FromArgb(255,255,255));
                }
 
                pointsToChange.Clear();

                for (int j = 1; j + 1 < image.Height; j++)
                {
                    for (int i = 1; i + 1 < image.Width; i++)
                    {
                        a = getA(image, j, i);
                        b = getB(image, j, i);
                        Console.WriteLine("getA = " + a + " getB = " + b);

                        if (image.GetPixel(j, i).R == 0 && 2 <= b && b <= 6 && a == 1
                            && (image.GetPixel(j - 1, i).R * image.GetPixel(j, i + 1).R * image.GetPixel(j, i - 1).R == 0)
                            && (image.GetPixel(j - 1, i).R * image.GetPixel(j + 1, i).R * image.GetPixel(j, i - 1).R == 0))
                        {
                            pointsToChange.Add(new System.Drawing.Point(i, j));

                            hasChange = true;
                        }
                    }
                }
                foreach (System.Drawing.Point point in pointsToChange)
                {
                    result.SetPixel(point.X, point.Y, Color.FromArgb(255, 255, 255));
                }

                pointsToChange.Clear();

            } while (hasChange);
            return result;
        }

        private int getA(Bitmap image, int y, int x) {
            int count = 0;

            //p2 p3
            if (image.GetPixel(x, y - 1).R == 255 && image.GetPixel(x + 1, y - 1).R == 0)
            {
                count++;
            }
            //p3 p4
            if (image.GetPixel(x + 1, y - 1).R == 255 && image.GetPixel(x + 1, y).R == 0)
            {
                count++;
            }
            //p4 p5
            if (image.GetPixel(x + 1, y).R == 255 && image.GetPixel(x + 1, y + 1).R == 0)
            {
                count++;
            }
            //p5 p6
            if (image.GetPixel(x + 1, y + 1).R == 255 && image.GetPixel(x, y + 1).R == 0)
            {
                count++;
            }
            //p6 p7
            if (image.GetPixel(x, y + 1).R == 255 && image.GetPixel(x - 1, y + 1).R == 0)
            {
                count++;
            }
            //p7 p8
            if (image.GetPixel(x - 1, y + 1).R == 255 && image.GetPixel(x - 1, y).R == 0)
            {
                count++;
            }
            //p8 p9
            if (image.GetPixel(x - 1, y).R == 255 && image.GetPixel(x - 1, y - 1).R == 0)
            {
                count++;
            }
            //p9 p2
            if (image.GetPixel(x - 1, y - 1).R == 255 && image.GetPixel(x, y - 1).R == 0)
            {
                count++;
            }

            return count;
        }

        private int getB(Bitmap image, int y, int x) {

            return image.GetPixel(x, y - 1).R == 0 ? image.GetPixel(x, y - 1).R : 1 +
                   image.GetPixel(x + 1, y - 1).R == 0 ? image.GetPixel(x + 1, y - 1).R : 1 +
                   image.GetPixel(x + 1, y).R == 0 ? image.GetPixel(x + 1, y).R : 1 +
                   image.GetPixel(x + 1, y + 1).R == 0 ? image.GetPixel(x + 1, y + 1).R : 1 +
                   image.GetPixel(x, y + 1).R == 0 ? image.GetPixel(x, y + 1).R : 1 +
                   image.GetPixel(x - 1, y + 1).R == 0 ? image.GetPixel(x - 1, y + 1).R : 1 +
                   image.GetPixel(x - 1, y).R == 0 ? image.GetPixel(x - 1, y).R : 1 +
                   image.GetPixel(x - 1, y - 1).R == 0 ? image.GetPixel(x - 1, y - 1).R : 1;
        }

        private Bitmap ObjectDetection(Bitmap image, Color edge)
        {
            // 7 0 1
            // 6 - 2
            // 5 4 3
            Console.WriteLine("masuk deteksi");
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            result = Normalisasi(result);
            Color[] adjacentColor = new Color[8];
            Console.WriteLine("masuk deteksi 2");
            System.Drawing.Point startingPoint = new System.Drawing.Point();
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color color = result.GetPixel(j, i);
                    if (color.R == edge.R && color.G == edge.G && color.B == edge.B)
                    {
                        startingPoint.X = j;
                        startingPoint.Y = i;
                        goto sini;
                    }
                }
            }

            return result;

            sini:

            List<System.Drawing.Point> koords = new List<System.Drawing.Point>();
            List<int> chainCode = new List<int>();
            int arah = 0;

            adjacentColor[0] = result.GetPixel(startingPoint.X, startingPoint.Y - 1);
            adjacentColor[1] = result.GetPixel(startingPoint.X + 1, startingPoint.Y - 1);
            adjacentColor[2] = result.GetPixel(startingPoint.X + 1, startingPoint.Y);
            adjacentColor[3] = result.GetPixel(startingPoint.X + 1, startingPoint.Y + 1);
            adjacentColor[4] = result.GetPixel(startingPoint.X, startingPoint.Y + 1);
            adjacentColor[5] = result.GetPixel(startingPoint.X - 1, startingPoint.Y + 1);
            adjacentColor[6] = result.GetPixel(startingPoint.X - 1, startingPoint.Y);
            adjacentColor[7] = result.GetPixel(startingPoint.X - 1, startingPoint.Y - 1);

            System.Drawing.Point tujuan = startingPoint;

            if(ispreprocessed)
                SearchAdjacent(result, ref adjacentColor, ref koords, ref tujuan, ref chainCode, ref arah, Color.FromArgb(255,255,255), Color.FromArgb(0,0,0));
            else
                SearchAdjacent(result, ref adjacentColor, ref koords, ref tujuan, ref chainCode, ref arah, Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));

            do
            {
                if (ispreprocessed)
                    SearchAdjacent(result, ref adjacentColor, ref koords, ref tujuan, ref chainCode, ref arah, Color.FromArgb(255, 255, 255), Color.FromArgb(0, 0, 0));
                else
                    SearchAdjacent(result, ref adjacentColor, ref koords, ref tujuan, ref chainCode, ref arah, Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255));

            } while (tujuan.X != startingPoint.X || tujuan.Y != startingPoint.Y);

            chaincodes.Add(chainCode);
            stringCodes.Add(ListIntConv(chainCode));
            if (ispreprocessed)
                result = Bombing(result, koords, Color.FromArgb(255,255,255), Color.FromArgb(0,0,0));
            else
                result = Bombing(result, koords, Color.FromArgb(0,0,0), Color.FromArgb(255,255,255));

            return result;
        }

        private Bitmap BombingOLD(Bitmap image, List<System.Drawing.Point> koords, Color edge, Color bg)
        {
            Console.WriteLine("masuk bombing");
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);

            bool found = false;
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    foreach (System.Drawing.Point point in koords)
                    {
                        // detek edge
                        if ((j == point.X && i == point.Y))
                        {
                            found = true;
                            result.SetPixel(j, i, Color.FromArgb(bg.R, bg.G, bg.B));
                        }
                    }

                    if (!found && (result.GetPixel(j, i).R == edge.R && result.GetPixel(j, i).G == edge.G && result.GetPixel(j, i).B == edge.B))
                    {
                        // cek Y
                        int x1 = -1;
                        int x2 = -1;
                        foreach (System.Drawing.Point point in koords)
                        {
                            if (i == point.Y)
                            {
                                if (x1 == -1)
                                    x1 = point.X;
                                else
                                    x2 = point.X;
                            }
                        }

                        int xmax = Math.Max(x1, x2);
                        int xmin = Math.Min(x1, x2);

                        // cek X
                        int y1 = -1;
                        int y2 = -1;
                        foreach (System.Drawing.Point point in koords)
                        {
                            if (j == point.X)
                            {
                                if (y1 == -1)
                                    y1 = point.Y;
                                else
                                    y2 = point.Y;
                            }
                        }

                        int ymax = Math.Max(y1, y2);
                        int ymin = Math.Min(y1, y2);

                        // cek diantara
                        if (i < ymax && i > ymin && j < xmax && j > xmin)
                            result.SetPixel(j, i, Color.FromArgb(bg.R, bg.G, bg.B));
                    }
                    
                    found = false;
                }
            }

            return result;
        }

        private Bitmap Bombing(Bitmap image, List<System.Drawing.Point> koords, Color edge, Color bg)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);

            bool found = false; for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    foreach (System.Drawing.Point point in koords)
                    { // detek edge 
                        if ((j == point.X && i == point.Y)) { found = true; result.SetPixel(j, i, Color.FromArgb(bg.R, bg.G, bg.B)); }
                    }

                    if (!found && (result.GetPixel(j, i).R == edge.R && result.GetPixel(j, i).G == edge.G && result.GetPixel(j, i).B == edge.B))
                    { // cek Y 
                        int xmin = image.Width; int xmax = -1; foreach (System.Drawing.Point point in koords) { if (i == point.Y) { xmin = Math.Min(xmin, point.X); xmax = Math.Max(xmax, point.X); } }

                        // cek X 
                        int ymin = image.Height; int ymax = -1; foreach (System.Drawing.Point point in koords) { if (j == point.X) { ymin = Math.Min(ymin, point.Y); ymax = Math.Max(ymax, point.Y); } }

                        // cek diantara 
                        if (xmax != image.Width && xmin != -1 && ymax != image.Height && ymin != -1) if (i < ymax && i > ymin && j < xmax && j > xmin) result.SetPixel(j, i, Color.FromArgb(bg.R, bg.G, bg.B));
                    }

                    found = false;
                }
            }

            return result;
        }

        private void SearchAdjacent(Bitmap result, ref Color[] adjacentColor, ref List<System.Drawing.Point> koords, ref System.Drawing.Point tujuan, ref List<int> chainCode, ref int arah, Color edge, Color bg)
        {
            while (adjacentColor[arah].R == bg.R && adjacentColor[arah].G == bg.G && adjacentColor[arah].B == bg.B)
            {
                if (arah == 7)
                    arah = 0;
                else
                    arah++;
            }

            while (adjacentColor[arah].R == edge.R && adjacentColor[arah].G == edge.G && adjacentColor[arah].B == edge.B)
            {
                if (arah == 0)
                    arah = 7;
                else
                    arah--;
            }

            if (arah == 7)
                arah = 0;
            else
                arah++;

            if (arah == 0)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X, tujuan.Y - 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(0);
                }
            }
            else if (arah == 1)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X + 1, tujuan.Y - 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(1);
                }
            }
            else if (arah == 2)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X + 1, tujuan.Y);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(2);
                }
            }
            else if (arah == 3)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X + 1, tujuan.Y + 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(3);
                }
            }
            else if (arah == 4)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X, tujuan.Y + 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(4);
                }
            }
            else if (arah == 5)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X - 1, tujuan.Y + 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(5);
                }
            }
            else if (arah == 6)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X - 1, tujuan.Y);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(6);
                }
            }
            else if (arah == 7)
            {
                System.Drawing.Point next = new System.Drawing.Point(tujuan.X - 1, tujuan.Y - 1);
                if (!koords.Contains(next))
                {
                    koords.Add(next);
                    tujuan.X = next.X;
                    tujuan.Y = next.Y;
                    chainCode.Add(7);
                }
            }

            adjacentColor[0] = result.GetPixel(tujuan.X, tujuan.Y - 1);
            adjacentColor[1] = result.GetPixel(tujuan.X + 1, tujuan.Y - 1);
            adjacentColor[2] = result.GetPixel(tujuan.X + 1, tujuan.Y);
            adjacentColor[3] = result.GetPixel(tujuan.X + 1, tujuan.Y + 1);
            adjacentColor[4] = result.GetPixel(tujuan.X, tujuan.Y + 1);
            adjacentColor[5] = result.GetPixel(tujuan.X - 1, tujuan.Y + 1);
            adjacentColor[6] = result.GetPixel(tujuan.X - 1, tujuan.Y);
            adjacentColor[7] = result.GetPixel(tujuan.X - 1, tujuan.Y - 1);
        }

        private BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private double DotProduct(Bitmap image, double[,] matrix, int i, int j)
        {
            double result = (double)image.GetPixel(i - 1, j - 1).R * matrix[0, 0] +
                (double)image.GetPixel(i - 1, j).R * matrix[0, 1] +
                (double)image.GetPixel(i - 1, j + 1).R * matrix[0, 2] +
                (double)image.GetPixel(i, j - 1).R * matrix[1, 0] +
                (double)image.GetPixel(i, j).R * matrix[1, 1] +
                (double)image.GetPixel(i, j + 1).R * matrix[1, 2] +
                (double)image.GetPixel(i + 1, j - 1).R * matrix[2, 0] +
                (double)image.GetPixel(i + 1, j).R * matrix[2, 1] +
                (double)image.GetPixel(i + 1, j + 1).R * matrix[2, 2];

            return result;
        }

        private double[,] Rotate(double[,] matrix)
        {
            double[,] result = (double[,])matrix.Clone();
            
            double temp = result[0, 0];
            result[0, 0] = result[0, 1];
            result[0, 1] = result[0, 2];
            result[0, 2] = result[1, 2];
            result[1, 2] = result[2, 2];
            result[2, 2] = result[2, 1];
            result[2, 1] = result[2, 0];
            result[2, 0] = result[1, 0];
            result[1, 0] = temp;

            return result;
        }

        private Bitmap OneDegree(Bitmap image, int mode)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            Bitmap framed = AddFrame(result);

            double[,] matrix;
            switch (mode)
            {
                case 0:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                    break;
                case 1:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                    break;
                case 2:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -1.414, 0, 1.414 }, { -1, 0, 1 } };
                    break;
                case 3:
                    matrix = new double[3, 3] { { 0, 0, -1 }, { 0, 1, 0 }, { 0, 0, 0 } };
                    break;
                case 4:
                    matrix = new double[3, 3] { { 6, 0, -6 }, { 0, 0, 0 }, { -6, 0, 6 } };
                    break;
                default:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                    break;
            }

            for (int i = 1; i < framed.Width - 1; i++)
            {
                for (int j = 1; j < framed.Height - 1; j++)
                {
                    double north = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(Rotate(matrix));
                    double east = DotProduct(framed, matrix, i, j);

                    int distance = (int)Distance(north, east);
                    distance = distance > 255 ? 255 : distance;

                    result.SetPixel(i - 1, j - 1, Color.FromArgb(distance, distance, distance));
                }
            }

            return result;
        }

        private double Distance(double x, double y)
        {
            return Math.Pow(Math.Pow(x, 2) + Math.Pow(y, 2), 0.5);
        }

        private Bitmap TwoDegree(Bitmap image, int mode)
        {
            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            Bitmap framed = AddFrame(result);

            double[,] matrix;
            switch (mode)
            {
                case 0:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                    break;
                case 1:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                    break;
                case 2:
                    matrix = new double[3, 3] { { -3, -3, 5 }, { -3, 0, 5 }, { -3, -3, 5 } };
                    break;
                default:
                    matrix = new double[3, 3] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } };
                    break;
            }

            for (int i = 1; i < framed.Width - 1; i++)
            {
                for (int j = 1; j < framed.Height - 1; j++)
                {
                    double n = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double ne = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double e = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double se = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double s = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double sw = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double w = DotProduct(framed, matrix, i, j);
                    matrix = Rotate(matrix);
                    double nw = DotProduct(framed, matrix, i, j);

                    int distance = (int)Distance(n, Distance(ne, Distance(e, Distance(se, Distance(s, Distance(sw, Distance(w, nw)))))));
                    distance = distance > 255 ? 255 : distance;

                    result.SetPixel(i - 1, j - 1, Color.FromArgb(distance, distance, distance));
                }
            }

            return result;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (original != null)
            {
                if (comboBox1.Text.Equals("Contrast"))
                {
                    processed = Contrast(processed);
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Grayscale"))
                {
                    processed = Grayscale(processed);
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Sharpen"))
                {
                    processed = Sharpen(processed);
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Binary Image"))
                {
                    processed = Normalisasi(processed);
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("2 Differences"))
                {
                    processed = EdgeDiff2(processed);
                    ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("4 Differences"))
                {
                    processed = EdgeDiff4(processed);
                    ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Matrix 1 Degree"))
                {
                    processed = Grayscale(processed);

                    if (comboBox2.Text.Equals("Prewitt"))
                    {
                        processed = OneDegree(processed, 0);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox2.Text.Equals("Sobel"))
                    {
                        processed = OneDegree(processed, 1);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox2.Text.Equals("Frei Chen"))
                    {
                        processed = OneDegree(processed, 2);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox2.Text.Equals("Robert"))
                    {
                        processed = OneDegree(processed, 3);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox2.Text.Equals("Kayalli"))
                    {
                        processed = OneDegree(processed, 4);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                }
                else if (comboBox1.Text.Equals("Matrix 2 Degree"))
                {
                    processed = Grayscale(processed);

                    if (comboBox3.Text.Equals("Prewitt"))
                    {
                        processed = TwoDegree(processed, 0);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox3.Text.Equals("Robinson"))
                    {
                        processed = TwoDegree(processed, 1);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                    else if (comboBox3.Text.Equals("Kirsch"))
                    {
                        processed = TwoDegree(processed, 2);
                        ispreprocessed = true;
                        image2.Source = CreateBitmapSourceFromBitmap(processed);
                    }
                }
                else if (comboBox1.Text.Equals("Object Detection"))
                {
                    if (ispreprocessed)
                        processed = ObjectDetection(processed, preprocessed);
                    else
                        processed = ObjectDetection(processed, unpreprocessed);

                    PrintListCodes();
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Zhang-Suen Thinning"))
                {
                    processed = zhangsuen(processed);
                    //ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Face Count"))
                {
                    processed = ScanSkintone(processed);
                    processed = ScanFace(processed);
                    //ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Morph"))
                {
                    System.Drawing.Point A = new System.Drawing.Point(0, 0);
                    System.Drawing.Point B = new System.Drawing.Point(1, 0);
                    System.Drawing.Point C = new System.Drawing.Point(1, 2);
                    System.Drawing.Point D = new System.Drawing.Point(0, 1);
                    processed = RectangleMorph(processed, A, B, C, D);
                    //ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
                else if (comboBox1.Text.Equals("Compress"))
                {
                    processed = compress10(processed);
                    //ispreprocessed = true;
                    image2.Source = CreateBitmapSourceFromBitmap(processed);
                }
            }
        }

        private void PrintListCodes()
        {
            textBox1.Clear();
            textBox1.AppendText("================= \n");
            textBox1.AppendText("Jumlah objek terdeteksi = " + stringCodes.Count + "\n");
            textBox1.AppendText("Daftar Chain Code \n");
            int n = 1;
            foreach (List<int> objek in chaincodes)
            {
                textBox1.AppendText("Objek #" + n + ": \n");
                
                textBox1.AppendText(stringCodes[n-1] + " \n");

                textBox1.AppendText("Hasil interpretasi = " + interpret(stringCodes[n - 1]) + " \n");

                textBox1.AppendText("\n");
                n++;
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            image2.Source = image1.Source;
            processed = original;
            ispreprocessed = false;
        }

        private int LevenshteinDistance(string s, string t) {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n+1, m+1];

            //step 1
            if (n == 0)
                return m;

            if (m == 0)
                return n;

            //step 2
            for (int i = 0; i < n; d[i, 0] = i++) { 
            }

            for (int j = 0; j < m; d[0, m] = j++) { 
            }

            //step 3
            for (int i = 1; i <= n; i++) { 
                //step 4
                for (int j = 1; j <= m; j++) { 
                    //step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    //step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
        //step 7
        return d[n, m];
        }

        private String ListIntConv(List<int> a) {
            String res = "";

            foreach (int i in a) {
                res += i.ToString();
            }

            return res;
        }

        private String interpret(String code) {
            List<int> distances = new List<int>();
            

            foreach (String s in codeBase) {
                distances.Add(LevenshteinDistance(s, code));
                Console.WriteLine("levenshtein = " + LevenshteinDistance(s, code));
            }

            int minima = distances[0];
            int mindex = 0;

            for (int i = 0; i < distances.Count; i++)
            {
                if (distances[i] < minima)
                { minima = distances[i]; mindex = i; }
            }

            Console.WriteLine("interpret index = " + mindex);
            return labels[mindex];
        }

        private Bitmap ScanSkintone(Bitmap image) {
            Console.WriteLine("Scanning Skintone");

            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    //cari warna kulit
                    if (image.GetPixel(i, j).R >= 82 && image.GetPixel(i, j).R <= 152 &&
                        image.GetPixel(i, j).G >= 87 && image.GetPixel(i, j).G <= 157 &&
                        image.GetPixel(i, j).B >= 75 && image.GetPixel(i, j).B <= 145)
                    {
                        result.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    else
                        result.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                }
            }

            return result;
        }

        private Bitmap ScanFace(Bitmap image) {
            Console.WriteLine("Scanning Face");

            int c = 0;
            int windowWidth = 50;
            int windowHeight = 90;
            int tempWindowW = windowWidth;
            int tempWindowH = windowHeight;

            Bitmap result = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
            
            int i = 0;
            int j = 0;
            do
            {
                int whiteCount = 0;
                List<int> FacePointX = new List<int>();
                List<int> FacePointY = new List<int>();
                List<int> NoneFaceX = new List<int>();
                List<int> NoneFaceY = new List<int>();

                //pendekin lebar window kalo melebihi ukuran gambar
                if (i + tempWindowW > image.Width)
                {
                    tempWindowW = tempWindowW - (tempWindowW + i - image.Width);
                    Console.WriteLine("window size changed: tempWindow(W) = " + tempWindowW);
                }
                //pendekin tinggi window kalo melebihi ukuran gambar
                if (j + tempWindowH > image.Height)
                {
                    tempWindowH = tempWindowH - (tempWindowH + j - image.Height);
                    Console.WriteLine("window size changed: tempWindow(H) = " + tempWindowH);
                }

                //hitung pixel putih dalam window
               for (int a = i; a < i + tempWindowW; a++)
               {
                   for (int b = j; b < j + tempWindowH; b++)
                   {
                       if (image.GetPixel(a, b).R == 255)
                       {
                           whiteCount++;

                           FacePointX.Add(a);
                           FacePointY.Add(b);
                       }
                       else {
                           NoneFaceX.Add(a);
                           NoneFaceY.Add(b);
                       }
                   }
               }

                Console.WriteLine("White count = " + whiteCount);
                if (whiteCount > 1000)
                {
                    c++;
                    Random rnd = new Random();
                    int colorR = rnd.Next(0, 255);
                    int colorG = rnd.Next(0, 255);
                    int colorB = rnd.Next(0, 255);

                    //warnain yang punya pixel kulit > treshold
                    for (int a = 0; a < FacePointX.Count; a++)
                    {
                        result.SetPixel(FacePointX[a], FacePointY[a], Color.FromArgb(colorR, colorG, colorB));
                    }
                    for (int a = 0; a < NoneFaceX.Count; a++)
                    {
                        result.SetPixel(NoneFaceX[a], NoneFaceY[a], Color.White);
                    }
                }
                else {
                    for (int a = 0; a < FacePointX.Count; a++)
                    {
                        result.SetPixel(FacePointX[a], FacePointY[a], Color.Black);
                    }
                }

                tempWindowW = windowWidth;
                tempWindowH = windowHeight;

                Console.WriteLine("Current location: (" + i + "," + j + ")");
                if (i + windowWidth >= image.Width)
                {
                    i = 0;
                    j = j + windowHeight;

                    Console.WriteLine("WIDTH EXCEEDED");
                    Console.WriteLine("Change location to: (" + i + "," + j + ")");
                    Console.WriteLine("");
                    Console.WriteLine("Windows Size = " + tempWindowW + " x " + tempWindowH);
                    Console.WriteLine("==============");

                }
                else
                { 
                    i = i + windowWidth;

                    Console.WriteLine("Change location to: (" + i + "," + j + ")");
                    Console.WriteLine("");
                    Console.WriteLine("Windows Size = " + tempWindowW + " x " + tempWindowH);
                    Console.WriteLine("xxxxxxxxxxxxxxx");
                }
            } while (i + windowWidth <= image.Width || j + windowHeight <= image.Height);
         
            Console.WriteLine("Face Found:" + c);
            return result;
        }

        private Bitmap Morph(Bitmap image, System.Drawing.Point tl, System.Drawing.Point bl, System.Drawing.Point tr, System.Drawing.Point br)
        {
            int topX = Math.Max(Math.Max(Math.Max(tl.X, bl.X), tr.X), br.X);
            int topY = Math.Max(Math.Max(Math.Max(tl.Y, bl.Y), tr.Y), br.Y);

            Bitmap res = new Bitmap(topX, topY);

            Color[][] pixels = new Color[image.Width][];

            //buat array yang diisi semua warna pada gambar
            for (int i = 0; i < image.Width; i++) {
                for (int j = 0; j < image.Height; j++) {
                    pixels[i][j] = image.GetPixel(i, j);
                }
            }

            //cari skala tiap pixel di gambar awal
            List<float> scaleIX = new List<float>(); //tiap pixel x di gambar awal
            List<float> scaleIY = new List<float>(); //tiap pixel y di gambar awal

            
            //isi scaleIX dan scale IY pake skala
            for (int i = 0; i < image.Width; i++) {
                scaleIX.Add(i / image.Width);
            }
            for (int j = 0; j < image.Height; j++) {
                scaleIY.Add(j / image.Height);
            }

            //cari koordinat sisi-sisi trapesium via bresenham
            List<System.Drawing.Point> TLBL = IEnumToList(GetPointsOnLine(tl.X, tl.Y, bl.X, bl.Y));
            List<System.Drawing.Point> TLTR = IEnumToList(GetPointsOnLine(tl.X, tl.Y, tr.X, tr.Y));
            List<System.Drawing.Point> BLBR = IEnumToList(GetPointsOnLine(bl.X, bl.Y, br.X, br.Y));
            List<System.Drawing.Point> TRBR = IEnumToList(GetPointsOnLine(tr.X, tr.Y, br.X, br.Y));

            //gabungin koordinat 4 sisi diatas kedalam satu list (biar gampang ntar)
            List<System.Drawing.Point> koords = new List<System.Drawing.Point>();
            foreach (System.Drawing.Point a in TLBL) {
                koords.Add(a);
            }
            foreach (System.Drawing.Point a in TLTR)
            {
                koords.Add(a);
            }
            foreach (System.Drawing.Point a in BLBR)
            {
                koords.Add(a);
            }
            foreach (System.Drawing.Point a in TRBR)
            {
                koords.Add(a);
            }


            return res;
        }

        public static IEnumerable<System.Drawing.Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new System.Drawing.Point((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }

        public static List<System.Drawing.Point> IEnumToList(IEnumerable<System.Drawing.Point> I){
            List<System.Drawing.Point> res = I.ToList();
            return res;
        }

        public static Bitmap Compress(Bitmap image) {
            Bitmap res = image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);

            return res;
        }

        public static Bitmap addFrame(Bitmap _b, Color _f)
        {
            Bitmap framedImage = new Bitmap(_b.Width + 2, _b.Height + 2);
            for (int i = 0; i < framedImage.Width; i++)
            {
                for (int j = 0; j < framedImage.Height; j++)
                {
                    framedImage.SetPixel(i, j, _f);
                }
            }
            for (int i = 0; i < _b.Width; i++)
            {
                for (int j = 0; j < _b.Height; j++)
                {
                    Color ct = _b.GetPixel(i, j);
                    framedImage.SetPixel(i + 1, j + 1, ct);
                }
            }
            return framedImage;
        }

        private static Color latar = Color.FromArgb(0, 0, 0);

        private static bool isLatar(XBitmap _b, int i, int j)
        {
            Color temp = _b.getPixel(i, j);
            return XColor.isEqual(temp, latar);
        }

        private static bool isNotLatar(XBitmap _b, int i, int j)
        {
            return !isLatar(_b, i, j);
        }

        private static int getA(XBitmap image, int x, int y)
        {
            int count = 0;
            //p2 p3 
            if (isLatar(image, x, y - 1) && isNotLatar(image, x + 1, y - 1)) count++;
            //p3 p4 
            if (isLatar(image, x + 1, y - 1) && isNotLatar(image, x + 1, y)) count++;
            //p4 p5 
            if (isLatar(image, x + 1, y) && isNotLatar(image, x + 1, y + 1)) count++;
            //p5 p6 
            if (isLatar(image, x + 1, y + 1) && isNotLatar(image, x, y + 1)) count++;
            //p6 p7 
            if (isLatar(image, x, y + 1) && isNotLatar(image, x - 1, y + 1)) count++;
            //p7 p8 
            if (isLatar(image, x - 1, y + 1) && isNotLatar(image, x - 1, y)) count++;
            //p8 p9 
            if (isLatar(image, x - 1, y) && isNotLatar(image, x - 1, y - 1)) count++;
            //p9 p2 
            if (isLatar(image, x - 1, y - 1) && isNotLatar(image, x, y - 1)) count++;
            return count;
        }

        private static int getB(XBitmap image, int x, int y, ref int p2, ref int p4, ref int p6, ref int p8)
        {
            int count = 0;
            //p2 
            if (isNotLatar(image, x, y - 1)) { p2 = 1; count++; } else p2 = 0;
            //p3 
            if (isNotLatar(image, x + 1, y - 1)) count++;
            //p4 
            if (isNotLatar(image, x + 1, y)) { p4 = 1; count++; } else p4 = 0;
            //p5 
            if (isNotLatar(image, x + 1, y + 1)) count++;
            //p6 
            if (isNotLatar(image, x, y + 1)) { p6 = 1; count++; } else p6 = 0;
            //p7 
            if (isNotLatar(image, x - 1, y + 1)) count++;
            //p8 
            if (isNotLatar(image, x - 1, y)) { p8 = 1; count++; } else p8 = 0;
            //p9 
            if (isNotLatar(image, x - 1, y - 1)) count++;
            return count;
        }

        public static Bitmap zhangsuen(Bitmap _b)
        {
            Bitmap framedImage = addFrame(_b, latar);
            XBitmap framedImage_xbmp = new XBitmap(framedImage);
            List<System.Drawing.Point> pointsToChange = new List<System.Drawing.Point>();
            int a, b; int p2 = 0; int p4 = 0; int p6 = 0; int p8 = 0;
            bool notSkeleton = true;
            while (notSkeleton)
            {
                notSkeleton = false;
                for (int i = 1; i < _b.Height + 1; i++)
                {
                    for (int j = 1; j < _b.Width + 1; j++)
                    {
                        if (isNotLatar(framedImage_xbmp, j, i))
                        {
                            a = getA(framedImage_xbmp, j, i);
                            b = getB(framedImage_xbmp, j, i, ref p2, ref p4, ref p6, ref p8);
                            if (2 <= b && b <= 6 && a == 1 && (p2 * p4 * p6 == 0) && (p4 * p6 * p8 == 0))
                            {
                                pointsToChange.Add(new System.Drawing.Point(j, i));
                                notSkeleton = true;
                            }
                        }
                    }
                }
                foreach (System.Drawing.Point point in pointsToChange)
                {
                    framedImage_xbmp.setPixel(point.X, point.Y, latar);
                }
                pointsToChange.Clear();
                for (int i = 1; i < _b.Height + 1; i++)
                {
                    for (int j = 1; j < _b.Width + 1; j++)
                    {
                        if (isNotLatar(framedImage_xbmp, j, i))
                        {
                            a = getA(framedImage_xbmp, j, i);
                            b = getB(framedImage_xbmp, j, i, ref p2, ref p4, ref p6, ref p8);
                            if (2 <= b && b <= 6 && a == 1 && (p2 * p4 * p8 == 0) && (p2 * p6 * p8 == 0))
                            {
                                pointsToChange.Add(new System.Drawing.Point(j, i));
                                notSkeleton = true;
                            }
                        }
                    }
                }
                foreach (System.Drawing.Point point in pointsToChange)
                {
                    framedImage_xbmp.setPixel(point.X, point.Y, latar);
                }
                pointsToChange.Clear();
            }
            return XImage.removeFrame(framedImage_xbmp.getBitmap());
        }

        public static Bitmap RectangleMorph(Bitmap _b, System.Drawing.Point _E, System.Drawing.Point _F, System.Drawing.Point _G, System.Drawing.Point _H)
        { // gradation diff
            System.Drawing.Point E = new System.Drawing.Point(_b.Width * _E.X, _b.Height * _E.Y);
            System.Drawing.Point F = new System.Drawing.Point(_b.Width * _F.X, _b.Height * _F.Y);
            System.Drawing.Point G = new System.Drawing.Point(_b.Width * _G.X, _b.Height * _G.Y);
            System.Drawing.Point H = new System.Drawing.Point(_b.Width * _H.X, _b.Height * _H.Y);
            int res_w = Math.Max(F.X - E.X, G.X - H.X);
            int res_h = Math.Max(H.Y - E.Y, G.Y - F.Y);
            Bitmap result = new Bitmap(res_w, res_h);
            XBitmap result_xbmp = new XBitmap(result);
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    int new_width = j * ((G.X - H.X) - (F.X - E.X)) / H.Y - E.Y + F.X - E.X;
                    int new_height = i * ((G.Y - F.Y) - (H.Y - E.Y)) / F.X - E.X + H.Y - E.Y;
                    int x1 = i * _b.Width / new_width;
                    int y1 = j * _b.Height / new_height;
                    Color px = (y1 >= _b.Height || x1 >= _b.Width || y1 < 0 || x1 < 0) ? Color.FromArgb(0, 0, 0) : _b.GetPixel(x1, y1);
                    result_xbmp.setPixel(i, j, px);
                }
            }
            return (result_xbmp.getBitmap());
        }

        public static Bitmap compress50(Bitmap _b)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID 
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            try
            {
                using (Bitmap tempImage = new Bitmap(_b))
                {
                    tempImage.Save(@"testing_images\tempFifty.jpg", jgpEncoder, myEncoderParameters);
                }
            }
            catch (Exception e)
            {

            }

            return _b;
        }

        public static Bitmap compress10(Bitmap _b)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID 
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 10L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            try
            {
                using (Bitmap tempImage = new Bitmap(_b))
                {
                    tempImage.Save(@"testing_images\tempTen.jpg", jgpEncoder, myEncoderParameters);
                }
            }
            catch (Exception e)
            {

            }

            return new Bitmap(@"testing_images\tempTen.jpg");
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
