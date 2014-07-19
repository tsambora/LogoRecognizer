using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PengcitUltimateApp
{
    class XHistogram
    {
        public int[][] data;  // histogram biasa
        public int[][] data2; // histogram incremental
        public XHistogram (Bitmap _b) {
            data = new int[3][];
            data2 = new int[3][];
            byte[][] r = new byte[_b.Width][];
            byte[][] g = new byte[_b.Width][];
            byte[][] b = new byte[_b.Width][];
            for (int i = 0; i < r.Length; i++) {
                r[i] = new byte[_b.Height];
                g[i] = new byte[_b.Height];
                b[i] = new byte[_b.Height]; 
            }
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = _b.GetPixel(i, j);
                    r[i][j] = ct.R;
                    g[i][j] = ct.G;
                    b[i][j] = ct.B;
                }
            }
            for (int i = 0; i < data.Length; i++) { // kosongin dulu
                data[i] = new int[256];
                data2[i] = new int[256];
                for (int j = 0; j < data[i].Length; j++) {
                    data[i][j] = 0;
                    data2[i][j] = 0;
                }
            }
            for (int i = 0; i < _b.Width; i++) { // isi histogram biasa
                for (int j = 0; j < _b.Height; j++) {
                    data[0][r[i][j]] += 1; // Red
                    data[1][g[i][j]] += 1; // Green
                    data[2][b[i][j]] += 1; // Blue
                }
            }
            for (int i = 0; i < data2.Length; i++) { // isi histogram incremental
                for (int j = 0; j < data2[i].Length; j++) {
                    for (int k = 0; k < j; k++) {
                        data2[i][j] += data[i][k];
                    }
                }
            }
        }

        public void scaleHistogram(float _scale) { // rescale histogram (buat ditampilkan)
            float rmax = data[0].Max();
            float gmax = data[1].Max();
            float bmax = data[2].Max();
            float r2max = data2[0].Max();
            float g2max = data2[1].Max();
            float b2max = data2[2].Max();
            for (int j = 0; j < data[0].Length; j++) {
                data[0][j] = (int)((float)data[0][j] * _scale / rmax);
                data[1][j] = (int)((float)data[1][j] * _scale / gmax);
                data[2][j] = (int)((float)data[2][j] * _scale / bmax);
                data2[0][j] = (int)((float)data2[0][j] * _scale / r2max);
                data2[1][j] = (int)((float)data2[1][j] * _scale / g2max);
                data2[2][j] = (int)((float)data2[2][j] * _scale / b2max);
            }
        }
    }
}
