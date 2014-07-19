using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PengcitUltimateApp
{
    class XColor {

        public static Color toGrayscale(Color _c){
            int itung = (int)(_c.R * 0.299 +_c.G * 0.587 + _c.B * 0.114);
            Color temp = Color.FromArgb(itung,itung,itung);
            return temp;
        }
        
        public static Color toInverted(Color _c) {
            Color temp = Color.FromArgb(255 - _c.R, 255 - _c.G, 255 - _c.B);
            return temp;
        }
        
        public static Color toBinary(Color _c, int _thres) {
            Color temp = Color.FromArgb((_c.R<_thres)?0:255, (_c.G<_thres)?0:255, (_c.B<_thres)?0:255);
            return temp;
        }

        public static Color differrence(Color _c, Color _c2) {
            Color temp = Color.FromArgb(Math.Abs(_c.R-_c2.R), Math.Abs(_c.G-_c2.G), Math.Abs(_c.B-_c2.B));
            return temp;
        }

        public static bool isEqual(Color _c, Color _c2) {
            bool temp = (_c.R==_c2.R)&&(_c.G==_c2.G)&&(_c.B==_c2.B);
            return temp;
        }
    }
}
