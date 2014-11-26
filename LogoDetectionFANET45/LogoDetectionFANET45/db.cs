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
        public static List<DateTime> dates =
            new List<DateTime>
            {
                new DateTime(2014,11,25),
                new DateTime(2014,11,25),
                new DateTime(2014,11,24),
                new DateTime(2014,11,24),
                new DateTime(2014,11,23),
                new DateTime(2014,11,22)
            };
        public static List<String> location =
            new List<String> 
            { 
                "31",
                "31",
                "31",
                "32",
                "32",
                "51"
            };
        public static List<String> username =
            new List<String> 
            { 
                "ponta0225",
                "thenewboston",
                "alexanderwangny",
                "beachb0y",
                "aditpk",
                "muhammadiqbal2777"
            };
        public static List<int> followers =
            new List<int> 
            { 
                551,
                105,
                664,
                14,
                60,
                420
            };
        public static List<KeyValuePair<DateTime, int>> chartStats =
            new List<KeyValuePair<DateTime, int>>
            {
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,25), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,24), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,23), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,22), 1),
            };

        public static List<KeyValuePair<DateTime, int>> dummyChartStats =
            new List<KeyValuePair<DateTime, int>>
            {
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,25), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,24), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,23), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,22), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,21), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,20), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,19), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,18), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,17), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,16), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,15), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,14), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,13), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,12), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,11), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,10), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,9), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,8), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,7), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,6), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,5), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,4), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,3), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,2), 1),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,1), 1)
            };
    }
}
