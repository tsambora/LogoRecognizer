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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.GPU;
using Emgu.CV.UI;

using SharpMap.Base;
using SharpMap.Converters;
using SharpMap.Data;
using SharpMap.Forms;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Web;

namespace LogoDetectionFANET45
{
    public partial class Form3 : Form
    {
        SharpMap.Layers.VectorLayer vlay = new SharpMap.Layers.VectorLayer("Indonesia");

        public Form3()
        {
            InitializeComponent();

            fetch_GeoData(dateTimePicker1.Value);
            init_chart();
        }

        private void fetch_GeoData(DateTime date) {
            List<String> res = new List<String>();
            for (int i = 0; i < db.dates.Count; i++) {
                if (date.Year == db.dates[i].Year && date.Month == db.dates[i].Month && date.Day == db.dates[i].Day)
                {
                    res.Add(db.location[i]);
                }
            }

            int[] colorMap = new int[95];
            foreach (String s in res) {
                for (int i = 0; i < 95; i++) {
                    if (s == i.ToString()) {
                        colorMap[i]++;
                    }
                }
            }

            Dictionary<string, SharpMap.Styles.IStyle> styles = new Dictionary<string, IStyle>();
            VectorStyle detectedArea = new VectorStyle();
            VectorStyle def = new VectorStyle();
            def.Fill = new SolidBrush(Color.FromArgb(255,255,255));
            for(int i = 0; i < colorMap.Length; i++){
                detectedArea.Fill = new SolidBrush(Color.FromArgb(255, colorMap[i] * 30, colorMap[i] * 30));
                if (colorMap[i] > 0){
                    styles.Add(i.ToString(), detectedArea);
                }
                else 
                    styles.Add(i.ToString(), def);
            }

            vlay.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\Users\Onit\Desktop\map.shp", true);
            vlay.Theme = new SharpMap.Rendering.Thematics.UniqueValuesTheme<string>("kode", styles, def);
            mapBox1.Map.Layers.Add(vlay);
            mapBox1.Map.ZoomToExtents();
            mapBox1.Refresh();
        }

        private void fetch_ChartDate(string range) {
            DateTime[] dates;
            KeyValuePair<string, string> a = new KeyValuePair<string, string>();
        }

        private void init_chart() {
            // Data arrays.
            string[] seriesArray = { "Cats", "Dogs" };
            int[] pointsArray = { 1, 2 };

            this.chart1.Palette = ChartColorPalette.Fire;

            // Set title.
            this.chart1.Titles.Add("Pets");

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart1.Series.Add(seriesArray[i]);

                // Add point.
                series.Points.Add(pointsArray[i]);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            fetch_GeoData(dateTimePicker1.Value);
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void mapBox1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
