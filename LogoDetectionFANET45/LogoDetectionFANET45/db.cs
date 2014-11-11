using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoDetectionFANET45
{
    public static class db
    {
        public static List<String> images =
            new List<String> {
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/starbucks_1.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/starbucks_2.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/starbucks_3.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/starbucks_4.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/starbucks_5.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/nologo_1.jpg"
            };
        public static List<String> labels =
            new List<String> {
                "starbucks",
                "starbucks",
                "starbucks",
                "starbucks",
                "starbucks",
                "mcdonalds"
            };
    }
}
