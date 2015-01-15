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
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/nologo_1.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/cocacola_1.jpeg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/fedex_1.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/fedex_2.jpg",
                "C:/Users/Onit/Documents/GitHub/logo-detector/test images/db/fedex_3.jpg"
            };
        public static List<String> labels =
            new List<String> {
                "starbucks",
                "starbucks",
                "starbucks",
                "starbucks",
                "starbucks",
                "tidak ditemukan logo",
                "cocacola",
                "fedex",
                "fedex",
                "fedex"
            };
        public static List<DateTime> dates =
            new List<DateTime>
            {
                new DateTime(2014,11,25),
                new DateTime(2014,11,25),
                new DateTime(2014,11,24),
                new DateTime(2014,11,24),
                new DateTime(2014,11,23),
                new DateTime(2014,11,22),
                new DateTime(2014,11,29),
                new DateTime(2014,11,29),
                new DateTime(2014,11,28),
                new DateTime(2014,11,28)
            };
        public static List<String> location =
            new List<String> 
            { 
                "31",
                "31",
                "31",
                "32",
                "32",
                "91",
                "11",
                "62",
                "74",
                "16"
            };
        public static List<String> username =
            new List<String> 
            { 
                "ponta0225",
                "thenewboston",
                "alexanderwangny",
                "beachb0y",
                "aditpk",
                "muhammadiqbal2777",
                "cobolover221",
                "taylorswiftfan123",
                "maskuman28",
                "josgojos44"
            };
        public static List<int> followers =
            new List<int> 
            { 
                551,
                105,
                664,
                14,
                60,
                420,
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
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,29), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,28), 2)
            };
        public static List<KeyValuePair<DateTime, int>> dummyChartStats =
            new List<KeyValuePair<DateTime, int>>
            {
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,25), 72),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,24), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,23), 31),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,22), 19),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,21), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,20), 52),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,19), 81),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,18), 95),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,17), 42),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,16), 32),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,15), 21),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,14), 12),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,13), 23),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,12), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,11), 8),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,10), 5),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,9), 30),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,8), 51),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,7), 33),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,6), 66),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,5), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,4), 10),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,3), 9),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,2), 17),
                new KeyValuePair<DateTime,int>(new DateTime(2014,12,1), 12),
                
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,25), 72),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,24), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,23), 31),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,22), 19),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,21), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,20), 52),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,19), 81),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,18), 95),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,17), 42),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,16), 32),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,15), 21),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,14), 12),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,13), 23),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,12), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,11), 8),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,10), 5),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,9), 30),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,8), 51),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,7), 33),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,6), 66),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,5), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,4), 10),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,3), 9),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,2), 17),
                new KeyValuePair<DateTime,int>(new DateTime(2014,11,1), 12),

                new KeyValuePair<DateTime,int>(new DateTime(2014,10,25), 72),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,24), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,23), 31),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,22), 19),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,21), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,20), 52),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,19), 81),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,18), 95),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,17), 42),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,16), 32),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,15), 21),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,14), 12),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,13), 23),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,12), 22),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,11), 8),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,10), 5),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,9), 30),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,8), 51),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,7), 33),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,6), 66),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,5), 2),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,4), 10),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,3), 9),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,2), 17),
                new KeyValuePair<DateTime,int>(new DateTime(2014,10,1), 12)
            };
    }
}
