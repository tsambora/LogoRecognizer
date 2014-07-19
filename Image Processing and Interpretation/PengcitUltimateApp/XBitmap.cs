using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PengcitUltimateApp
{
    class XBitmap
    {
        private int[,,] bitarr;
        public int Width;
        public int Height;

        public XBitmap(Bitmap _b) {
            bitarr = getImgArrFromBitmap(_b);
            Width = _b.Width;
            Height = _b.Height;
        }

        public Color getPixel(int x, int y) {
            Color temp = Color.FromArgb(bitarr[0,x,y],bitarr[1,x,y],bitarr[2,x,y]);
            return temp;
        }

        public void setPixel(int x, int y, Color _c) {
            bitarr[0, x, y] = _c.R;
            bitarr[1, x, y] = _c.G;
            bitarr[2, x, y] = _c.B;
        }

        public Bitmap getBitmap() {
            return getBitmapFromImgArr(bitarr);
        }
        
        public static int[,,] getImgArrFromBitmap(Bitmap _b) {
            int[,,] imgarr = new int[3,_b.Width,_b.Height];
            for (int i = 0; i < _b.Width; i++) {
                for (int j = 0; j < _b.Height; j++) {
                    Color ct = _b.GetPixel(i, j);
                    imgarr[0, i, j] = ct.R;
                    imgarr[1, i, j] = ct.G;
                    imgarr[2, i, j] = ct.B;
                }
            }
            return imgarr;
        }

        public static Bitmap getBitmapFromImgArr(int[,,] _imgarr) {
            Bitmap bmp = new Bitmap(_imgarr.GetLength(1), _imgarr.GetLength(2));
            for (int i = 0; i < bmp.Width; i++) {
                for (int j = 0; j < bmp.Height; j++) {
                    Color ct = Color.FromArgb(_imgarr[0,i,j], _imgarr[1,i,j], _imgarr[2,i,j]);
                    bmp.SetPixel(i,j,ct);
                }
            }
            return bmp;
        }
    }
}
