using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace PengcitUltimateApp
{
    class XImage {

        private static Dictionary<String, double[,]> mat1degree = new Dictionary<String, double[,]>()
        {
            { "prewitt", new double[3, 3] {{-1, 0, 1}, {-1, 0, 1}, {-1, 0, 1}}},
            { "sobel", new double[3, 3] {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}}},
            { "freichen", new double[3, 3] {{-1, 0, 1}, {-1.414, 0, 1.414}, {-1, 0, 1}}},
            { "robert", new double[3, 3] {{0, 0, -1}, {0, 1, 0}, {0, 0, 0}}},
            { "kayyali", new double[3, 3] {{6, 0, -6}, {0, 0, 0}, {-6, 0, 6}}}
        };

        private static Dictionary<String, double[,]> mat2degree = new Dictionary<String, double[,]>()
        {
            { "prewitt", new double[3, 3] {{-1, 0, 1}, {-1, 0, 1}, {-1, 0, 1}}},
            { "robinson", new double[3, 3] {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}}},
            { "kirsch", new double[3, 3] {{-3, -3, 5}, {-3, 0, 5}, {-3, -3, 5}}}
        };

        public static Bitmap copy(Bitmap _b) {
            Bitmap temp = new Bitmap(_b);
            return temp;
        }
        
        public static Bitmap getPlainImage(Bitmap _b, Color _c) {
            Bitmap plainImage = new Bitmap(_b.Width, _b.Height);
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    plainImage.SetPixel(i, j, Color.FromArgb(_c.R, _c.G, _c.B));
                }
            }
            return plainImage;
        }
        
        public static Bitmap getJoinedImage(Bitmap _b, Bitmap _b2) {
            Bitmap plainImage = XImage.copy(_b);
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    if (XColor.isEqual(plainImage.GetPixel(i,j), Color.FromArgb(0,0,0))) {
                        plainImage.SetPixel(i, j, _b2.GetPixel(i,j));
                    }
                }
            }
            return plainImage;
        }

        public static Bitmap addFrame(Bitmap _b, Color _f) {
            Bitmap framedImage = new Bitmap(_b.Width+2, _b.Height+2);
            for (int i = 0; i < framedImage.Width; i++) {
                for (int j = 0; j < framedImage.Height; j++) {
                    framedImage.SetPixel(i, j, _f);
                }
            }
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = _b.GetPixel(i,j);
                    framedImage.SetPixel(i+1, j+1, ct);
                }
            }
            return framedImage;
        }

        public static Bitmap addNFrame(Bitmap _b, Color _f, int _n) {
            Bitmap framedImage = XImage.copy(_b);
            for (int i = 0; i < _n; i++) {
                framedImage = XImage.addFrame(framedImage, _f);
            }
            return framedImage;
        }

        public static Bitmap removeFrame(Bitmap _b) {
            Bitmap framedRemovedImage = new Bitmap(_b.Width-2, _b.Height-2);
            for (int i = 0; i < framedRemovedImage.Width; i++) {
                for (int j = 0; j < framedRemovedImage.Height; j++) {
                    Color ct = _b.GetPixel(i+1,j+1);
                    framedRemovedImage.SetPixel(i, j, ct);
                }
            }
            return framedRemovedImage;
        }
        
        public static Bitmap removeNFrame(Bitmap _b, int _n) {
            Bitmap framedRemovedImage = XImage.copy(_b);
            for (int i = 0; i < _n; i++) {
                framedRemovedImage = XImage.removeFrame(framedRemovedImage);
            }
            return framedRemovedImage;
        }

        public static Bitmap addMirroredFrame(Bitmap _b) {
            Bitmap framedImage = new Bitmap(_b.Width+2, _b.Height+2);
            for (int i = 0; i < framedImage.Width; i++) {
                for (int j = 0; j < framedImage.Height; j++) {
                    Color ct = Color.FromArgb(255, 255, 255);
                    framedImage.SetPixel(i, j, ct);
                    int ti = i;
                    int tj = j; 
                    if ((i==0)||(j==0)||(i>=framedImage.Width-2)||(j>=framedImage.Height-2)) {
                        if (ti==0)
                            ti=1;
                        if (tj==0)
                            tj=1;
                        if (ti>=framedImage.Width-2)
                            ti=framedImage.Width-4;
                        if (tj>=framedImage.Height-2)
                            tj=framedImage.Height-4;
                        ct = _b.GetPixel(ti, tj);
                        framedImage.SetPixel(i, j, ct);
                    }
                }
            }
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = _b.GetPixel(i,j);
                    framedImage.SetPixel(i+1, j+1, ct);
                }
            }
            return framedImage;
        }
        
        public static Bitmap edgeDiff4(Bitmap _b) { // inversi warna
            Bitmap result = copy(_b);
            Bitmap framedImage = addMirroredFrame(_b);
            XBitmap framedImage_xbmp = new XBitmap(framedImage);
            for (int i = 1; i < framedImage.Width-1; i++) {
                for (int j = 1; j < framedImage.Height-1; j++) {
                    Color dc1 = XColor.differrence(framedImage_xbmp.getPixel(i, j - 1), framedImage_xbmp.getPixel(i, j + 1));
                    Color dc2 = XColor.differrence(framedImage_xbmp.getPixel(i + 1, j - 1), framedImage_xbmp.getPixel(i - 1, j + 1));
                    Color dc3 = XColor.differrence(framedImage_xbmp.getPixel(i + 1, j), framedImage_xbmp.getPixel(i - 1, j));
                    Color dc4 = XColor.differrence(framedImage_xbmp.getPixel(i + 1, j + 1), framedImage_xbmp.getPixel(i - 1, j - 1));
                    int maxdr = Math.Max(dc1.R, Math.Max(dc2.R, Math.Max(dc3.R, dc4.R)));
                    int maxdg = Math.Max(dc1.G, Math.Max(dc2.G, Math.Max(dc3.G, dc4.G)));
                    int maxdb = Math.Max(dc1.B, Math.Max(dc2.B, Math.Max(dc3.B, dc4.B)));
                    result.SetPixel(i-1, j-1, Color.FromArgb(maxdr, maxdr, maxdb));
                }
            }
            return result;
        }
        
        public static Bitmap edgeDiff2(Bitmap _b) { // inversi warna
            Bitmap result = copy(_b);
            Bitmap framedImage = addMirroredFrame(_b);
            XBitmap framedImage_xbmp = new XBitmap(framedImage);
            for (int i = 1; i < framedImage.Width-1; i++) {
                for (int j = 1; j < framedImage.Height-1; j++) {
                    Color dc1 = XColor.differrence(framedImage_xbmp.getPixel(i, j - 1), framedImage_xbmp.getPixel(i, j + 1));
                    Color dc2 = XColor.differrence(framedImage_xbmp.getPixel(i + 1, j), framedImage_xbmp.getPixel(i - 1, j));
                    int maxdr = Math.Max(dc1.R, dc2.R);
                    int maxdg = Math.Max(dc1.G, dc2.G);
                    int maxdb = Math.Max(dc1.B, dc2.B);
                    result.SetPixel(i-1, j-1, Color.FromArgb(maxdr, maxdr, maxdb));
                }
            }
            return result;
        }

        public static Bitmap toGrayscale(Bitmap _b) { // jadikan abu-abu
            Bitmap temp = copy(_b);
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = XColor.toGrayscale(temp.GetPixel(i, j));
                    temp.SetPixel(i, j, ct);
                }
            }
            return temp;
        }
        
        public static Bitmap gradDiff4(Bitmap _b) { // gradation diff
            Bitmap result = copy(_b);
            Bitmap framedImage = addMirroredFrame(_b);
            XBitmap framedImage_xbmp = new XBitmap(framedImage);
            for (int i = 1; i < framedImage.Width-1; i++) {
                for (int j = 1; j < framedImage.Height-1; j++) {
                    Color px = framedImage_xbmp.getPixel(i, j);
                    int thres = 21;
                    bool isWarnaKulit = (px.R>102-thres && px.R<102+thres) && (px.G>107-thres && px.G<107+thres) && (px.B>95-thres && px.B<95+thres);
                    int maxdr = isWarnaKulit ? 255 : 0;
                    int maxdg = isWarnaKulit ? 255 : 0;
                    int maxdb = isWarnaKulit ? 255 : 0;
                    result.SetPixel(i-1, j-1, Color.FromArgb(maxdr, maxdr, maxdb));
                }
            }
            return result;
        }
        
        public static Bitmap filterColor(Bitmap _b, Color _c, Color _t, Color _mark) { // gradation diff
            Bitmap result = copy(_b);
            XBitmap framedImage_xbmp = new XBitmap(result);
            for (int i = 0; i < result.Width; i++) {
                for (int j = 0; j < result.Height; j++) {
                    Color px = framedImage_xbmp.getPixel(i, j);
                    bool isWarnaKulit = (px.R>_c.R-_t.R && px.R<_c.R+_t.R) && (px.G>_c.G-_t.G && px.G<_c.G+_t.G) && (px.B>_c.B-_t.B && px.B<_c.B+_t.B);
                    int maxdr = isWarnaKulit ? _mark.R : 0;
                    int maxdg = isWarnaKulit ? _mark.G : 0;
                    int maxdb = isWarnaKulit ? _mark.B : 0;
                    if (isWarnaKulit)
                        framedImage_xbmp.setPixel(i, j, _mark);
                    else
                        framedImage_xbmp.setPixel(i, j, Color.FromArgb(0,0,0));
                }
            }
            return (framedImage_xbmp.getBitmap());
        }
        
        public static Bitmap grad2(Bitmap _b, int _radius, Color _mark, int _distance) { // gradation diff
            //Bitmap result = XImage.copy(_b);
            int r = _radius;
            int r2 = _distance/2+1;
            int limit = (int)(Math.Pow((2*r+1), 2)*(0.3));
            Bitmap framedImage = addNFrame(_b, Color.FromArgb(0, 0, 0), r);
            XBitmap xbmp_framedImage = new XBitmap(framedImage);
            Bitmap result = XImage.getPlainImage(_b, Color.Black);
            Bitmap framedResultImage = addNFrame(_b, Color.FromArgb(0, 0, 0), r2);
            for (int i = 0; i < result.Width; i+=_distance) {
                for (int j = 0; j < result.Height; j+=_distance) {
                    int x = i+r;
                    int y = j+r;
                    int count = 0;
                    for (int k = x-r; k < x+r; k++) {
                        for (int l = y-r; l < y+r; l++) {
                            Color px = xbmp_framedImage.getPixel(k, l);
                            count += (px.R!=0) ? 1 : 0;
                        }
                    }
                    if (count>=limit) {
                        int d = _distance/2;
                        for (int k = i-d; k <= i+d; k++) {
                            for (int l = j-d; l <= j+d; l++) {
                                framedResultImage.SetPixel(k + r2, l + r2, _mark);
                            }
                        }/**/
                        //framedResultImage.SetPixel(i + r2, j + r2, _mark);
                    }
                }
            }/**/
            return XImage.removeNFrame(framedResultImage, r2);
        }

        private static int floorTo(int _i, int _k) {
            int res = _i/_k;
            res *= _k;
            return res;
        }

        public static Bitmap toInverted(Bitmap _b) { // inversi warna
            Bitmap temp = copy(_b);
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = XColor.toInverted(temp.GetPixel(i, j));
                    temp.SetPixel(i, j, ct);
                }
            }
            return temp;
        }
        
        public static Bitmap toBinary(Bitmap _b, int _thres) { // jadikan abu-abu
            Bitmap temp = copy(_b);
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = XColor.toBinary(temp.GetPixel(i, j), _thres);
                    temp.SetPixel(i, j, ct);
                }
            }
            return temp;
        }

        public static Bitmap sharpen(Bitmap _b, int _range) { // sharpen warna 3x3 pixel
            Bitmap temp = copy(_b);
            int range = _range;
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    int maxR = 0;
                    int maxG = 0;
                    int maxB = 0;
                    for (int ti=i-range/2; ti<(i+range/2); ti++) { // mulai proses pemilihan nilai pixel tertinggi, range: (i-r/2..i+r/2)
                        for (int tj=j-range/2; tj<(j+range/2); tj++) {
                            int tti = (ti%_b.Width<0) ? _b.Width+ti : ti%_b.Width;      // agar tidak keluar batas
                            int ttj = (tj%_b.Height<0) ? _b.Height+tj : tj%_b.Height;
                            Color colortemp = _b.GetPixel(tti,ttj);
                            maxR = (maxR < colortemp.R) ? colortemp.R : maxR;
                            maxG = (maxG < colortemp.G) ? colortemp.G : maxG;
                            maxB = (maxB < colortemp.B) ? colortemp.B : maxB;
                        }
                    }
                    Color ct = Color.FromArgb(maxR, maxG, maxB);
                    temp.SetPixel(i, j, ct);
                }
            }
            return temp;
        }

        public static Bitmap toContrast1(Bitmap _b) {
            Bitmap temp = copy(_b);
            int rmin = 255;
            int rmax = 0;
            int gmin = 255;
            int gmax = 0;
            int bmin = 255;
            int bmax = 0;
            for (int i = 0; i < _b.Width; i++) { // menentukan r g b max & min
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = temp.GetPixel(i, j);
                    rmin = (ct.R <= rmin) ? ct.R : rmin;
                    rmax = (ct.R >= rmax) ? ct.R : rmax;
                    gmin = (ct.G <= gmin) ? ct.G : gmin;
                    gmax = (ct.G >= gmax) ? ct.G : gmax;
                    bmin = (ct.B <= bmin) ? ct.B : bmin;
                    bmax = (ct.B >= bmax) ? ct.B : bmax;
                }
            }
            for (int i = 0; i < _b.Width; i++) { // edit gambar dgn transformasi linear
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = temp.GetPixel(i, j);
                    int dr = rmax-rmin;
                    int dg = gmax-gmin;
                    int db = bmax-bmin;
                    int rt = 255*(ct.R-rmin)/((dr==0) ? 1 : dr);
                    int gt = 255*(ct.G-gmin)/((dg==0) ? 1 : dg);
                    int bt = 255*(ct.B-bmin)/((db==0) ? 1 : db);
                    temp.SetPixel(i, j, Color.FromArgb(rt, gt, bt));
                }
            }
            return temp;
        }
        
        public static Bitmap toContrast2(Bitmap _b) {
            Bitmap temp = copy(_b);
            XHistogram his = new XHistogram(_b);
            his.scaleHistogram(255);
            int[][] LUT;  // look up table
            LUT = new int[3][];
            for (int i = 0; i < LUT.Length; i++) { // isi dengan 0
                LUT[i] = new int[256];
                for (int j = 0; j < LUT[i].Length; j++) {
                    LUT[i][j] = 0;
                }
            }
            int rmin = 255;
            int rmax = 0;
            int gmin = 255;
            int gmax = 0;
            int bmin = 255;
            int bmax = 0;
            for (int i = 0; i < his.data2[0].Length; i++) { // menentukan r g b max & min dari histo incremental
                rmin = (his.data2[0][i] <= rmin) ? his.data2[0][i] : rmin;
                rmax = (his.data2[0][i] >= rmax) ? his.data2[0][i] : rmax;
                gmin = (his.data2[1][i] <= gmin) ? his.data2[1][i] : gmin;
                gmax = (his.data2[1][i] >= gmax) ? his.data2[1][i] : gmax;
                bmin = (his.data2[2][i] <= bmin) ? his.data2[2][i] : bmin;
                bmax = (his.data2[2][i] >= bmax) ? his.data2[2][i] : bmax;
            }
            for (int j = 0; j < LUT[0].Length; j++) { // bikin LUT
                LUT[0][j] = 255 * (his.data2[0][j] - rmin) / (rmax - rmin);
                LUT[1][j] = 255 * (his.data2[1][j] - gmin) / (gmax - gmin);
                LUT[2][j] = 255 * (his.data2[2][j] - bmin) / (bmax - bmin);
            }
            for (int i = 0; i < _b.Width; i++) { // Apply LUT ke gambar
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = temp.GetPixel(i, j);
                    int rt = LUT[0][ct.R];
                    int gt = LUT[1][ct.G];
                    int bt = LUT[2][ct.B];
                    temp.SetPixel(i, j, Color.FromArgb(rt, gt, bt));
                }
            }
            return temp;
        }

        public static Bitmap processDegreeOne(Bitmap _b, int _mode)
        {
            XBitmap temp = new XBitmap(addMirroredFrame(_b));
            Bitmap output = new Bitmap(_b.Width, _b.Height);

            double[,] convmatrix;
            switch (_mode)
            {
                case 0:
                    convmatrix = (double[,])mat1degree["prewitt"].Clone();
                    break;
                case 1:
                    convmatrix = (double[,])mat1degree["sobel"].Clone();
                    break;
                case 2:
                    convmatrix = (double[,])mat1degree["freichen"].Clone();
                    break;
                case 3:
                    convmatrix = (double[,])mat1degree["robert"].Clone();
                    break;
                case 4:
                    convmatrix = (double[,])mat1degree["kayyali"].Clone();
                    break;
                default:
                    convmatrix = (double[,])mat1degree["prewitt"].Clone();
                    break;
            }

            for (int i = 1; i < temp.Width - 1; i++)
            {              // iterasi
                for (int j = 1; j < temp.Height - 1; j++)
                {
                    double N = XImage.dotProduct(temp, convmatrix, i, j); // North
                    convmatrix = (double[,])rotateMatrix(rotateMatrix(convmatrix)).Clone();
                    double E = XImage.dotProduct(temp, convmatrix, i, j); // East, rotated twice (90 degree);

                    int dis = (int)distance(N, E);
                    int jarak = dis > 255 ? 255 : dis;

                    output.SetPixel(i - 1, j - 1, Color.FromArgb(jarak, jarak, jarak));
                }
            }
            return output;
        }

        public static Bitmap processDegreeTwo(Bitmap _b, int _mode)
        {
            XBitmap temp = new XBitmap(addMirroredFrame(_b));
            Bitmap output = new Bitmap(_b.Width, _b.Height);

            double[,] convmatrix;
            switch (_mode) {
                case 0:
                    convmatrix = (double[,])mat2degree["prewitt"].Clone();
                    break;
                case 1:
                    convmatrix = (double[,])mat2degree["robinson"].Clone();
                    break;
                case 2:
                    convmatrix = (double[,])mat2degree["kirsch"].Clone();
                    break;
                default:
                    convmatrix = (double[,])mat2degree["prewitt"].Clone();
                    break;
            }

            for (int i = 1; i < temp.Width - 1; i++)
            {              // iterasi
                for (int j = 1; j < temp.Height - 1; j++)
                {
                    double N = XImage.dotProduct(temp, convmatrix, i, j);       // North
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double NE = XImage.dotProduct(temp, convmatrix, i, j);      // North-East
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double E = XImage.dotProduct(temp, convmatrix, i, j);       // East
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double SE = XImage.dotProduct(temp, convmatrix, i, j);      // South-East
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double S = XImage.dotProduct(temp, convmatrix, i, j);       // South
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double SW = XImage.dotProduct(temp, convmatrix, i, j);      // South-West
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double W = XImage.dotProduct(temp, convmatrix, i, j);       // West
                    convmatrix = (double[,])rotateMatrix(convmatrix).Clone();
                    double NW = XImage.dotProduct(temp, convmatrix, i, j);      // North-West

                    int dis = (int)distance(N, distance(NE, distance(E, distance(SE, distance(S, distance(SW, distance(W, NW)))))));
                    int jarak = dis > 255 ? 255 : dis;

                    output.SetPixel(i - 1, j - 1, Color.FromArgb(jarak, jarak, jarak));
                }
            }
            return output;
        }

        private static double distance(double a, double b)
        {
            return Math.Pow(Math.Pow(a, 2) + Math.Pow(b, 2), 0.5);
        }

        private static double[,] rotateMatrix(double[,] _mat) // putar matrix 45 drj, 8x jadi kembali ke asal, untuk derajat 2
        {
            double[,] res = (double[,])_mat.Clone();
            double temp = res[0, 0];
            res[0, 0] = res[0, 1];
            res[0, 1] = res[0, 2];
            res[0, 2] = res[1, 2];
            res[1, 2] = res[2, 2];
            res[2, 2] = res[2, 1];
            res[2, 1] = res[2, 0];
            res[2, 0] = res[1, 0];
            res[1, 0] = temp;
            return res;
        }

        private static double dotProduct(XBitmap _bmp, double[,] _convmatrix, int i, int j) // putar matrix 45 drj, 8x jadi kembali ke asal, untuk derajat 2
        {
            double res = (double)_bmp.getPixel(i - 1, j - 1).R * _convmatrix[0, 0] +
                        (double)_bmp.getPixel(i - 1, j).R * _convmatrix[0, 1] +
                        (double)_bmp.getPixel(i - 1, j + 1).R * _convmatrix[0, 2] +
                        (double)_bmp.getPixel(i, j - 1).R * _convmatrix[1, 0] +
                        (double)_bmp.getPixel(i, j).R * _convmatrix[1, 1] +
                        (double)_bmp.getPixel(i, j + 1).R * _convmatrix[1, 2] +
                        (double)_bmp.getPixel(i + 1, j - 1).R * _convmatrix[2, 0] +
                        (double)_bmp.getPixel(i + 1, j).R * _convmatrix[2, 1] +
                        (double)_bmp.getPixel(i + 1, j + 1).R * _convmatrix[2, 2];
            return res;
        }

        public static Bitmap faceCount(Bitmap _b, Color _mark, ref int _count) {
            Bitmap framedImage = XImage.addNFrame(XImage.copy(_b), Color.FromArgb(0,0,0), 1);
            Bitmap result = XImage.copy(framedImage);
            int count = 0;
            for (int i = 1; i < _b.Width; i+=1) {
                for (int j = 1; j < _b.Height; j+=1) {
                    if (XColor.isEqual(framedImage.GetPixel(i,j),_mark)) {
                        Point[] P = XImage.floodDelete(i,j,ref framedImage,_mark);
                        float dx = P[1].X - P[0].X;
                        float dy = P[1].Y - P[0].Y;
                        float dydx = (dy+1)/(dx+1);
                        if (dydx<=1.5 && dydx>=0.7 && dy > 20) {
                            count++;
                            result = drawRectangle(result, P[0], P[1], Color.FromArgb(255, 0, 0));
                        }
                    }
                }
            }
            _count = count;
            return XImage.removeNFrame(result, 1);
        }

        public static Bitmap drawRectangle(Bitmap _b, Point _p, Point _q, Color _mark) {
            Bitmap result = XImage.copy(_b);
            for (int i=_p.X; i<_q.X; i++) {
                result.SetPixel(i, _p.Y, _mark);
                result.SetPixel(i, _q.Y, _mark);
            }
            for (int j=_p.Y; j<_q.Y; j++) {
                result.SetPixel(_p.X, j, _mark);
                result.SetPixel(_q.X, j, _mark);
            }
            return result;
        }

        private static Point[] floodDelete(int _x, int _y, ref Bitmap _b, Color _mark) { // delete pixel non-latar
            Stack<int> stackx = new Stack<int>();
            Stack<int> stacky = new Stack<int>();
            stackx.Push(_x);
            stacky.Push(_y);
            int minX = _x;
            int minY = _y;
            int maxX = _x;
            int maxY = _y;
            while ((stackx.Count!=0)&&(stacky.Count!=0)) {
                int tx = stackx.Pop();
                int ty = stacky.Pop();
                if (XColor.isEqual(_mark, _b.GetPixel(tx+1, ty))) {
                    stackx.Push(tx + 1);
                    stacky.Push(ty);
                    maxX = (tx+1>maxX) ? tx+1 : maxX;
                }
                if (XColor.isEqual(_mark, _b.GetPixel(tx-1, ty))) {
                    stackx.Push(tx - 1);
                    stacky.Push(ty);
                    minX = (tx-1<minX) ? tx-1 : minX;
                }
                if (XColor.isEqual(_mark, _b.GetPixel(tx, ty+1))) {
                    stackx.Push(tx);
                    stacky.Push(ty + 1);
                    maxY = (ty+1>maxY) ? ty+1 : maxY;
                }
                if (XColor.isEqual(_mark, _b.GetPixel(tx, ty-1))) {
                    stackx.Push(tx);
                    stacky.Push(ty - 1);
                    minY = (ty-1<minY) ? ty-1 : minY;
                }
                _b.SetPixel(tx, ty, Color.FromArgb(0,0,0));
            }
            Point[] P = new Point[2];
            P[0] = new Point(minX, minY);
            P[1] = new Point(maxX, maxY);
            return P;
        }
        
        public static Bitmap RectangleMorph(Bitmap _b, Point _E, Point _F, Point _G, Point _H) { // gradation diff
            Point E = new Point(_b.Width * _E.X, _b.Height * _E.Y);
            Point F = new Point(_b.Width * _F.X, _b.Height * _F.Y);
            Point G = new Point(_b.Width * _G.X, _b.Height * _G.Y);
            Point H = new Point(_b.Width * _H.X, _b.Height * _H.Y);
            int res_w = Math.Max(F.X-E.X, G.X-H.X);
            int res_h = Math.Max(H.Y-E.Y, G.Y-F.Y);
            Bitmap result = new Bitmap(res_w, res_h);
            XBitmap result_xbmp = new XBitmap(result);
            for (int i = 0; i < result.Width; i++) {
                for (int j = 0; j < result.Height; j++) {
                    int new_width = j * ((G.X-H.X)-(F.X-E.X))/H.Y-E.Y + F.X-E.X;
                    int new_height = i * ((G.Y-F.Y)-(H.Y-E.Y))/F.X-E.X + H.Y-E.Y;
                    int x1 = i*_b.Width/new_width;
                    int y1 = j*_b.Height/new_height;
                    Color px = (y1>=_b.Height||x1>=_b.Width||y1<0||x1<0) ? Color.FromArgb(0,0,0) : _b.GetPixel(x1, y1);
                    result_xbmp.setPixel(i, j, px);
                }
            }
            return (result_xbmp.getBitmap());
        }
    }
}
