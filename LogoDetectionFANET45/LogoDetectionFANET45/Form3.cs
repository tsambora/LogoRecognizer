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
        public Form3()
        {
            InitializeComponent();

            SharpMap.Layers.VectorLayer vlay = new SharpMap.Layers.VectorLayer("Indonesia");
            vlay.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\Users\Onit\Desktop\map.shp", true);

            //Create the style for Land
            VectorStyle landStyle = new VectorStyle();
            landStyle.Fill = new SolidBrush(Color.FromArgb(232, 232, 232));

            //Create the style for Water
            VectorStyle waterStyle = new VectorStyle();
            waterStyle.Fill = new SolidBrush(Color.FromArgb(198, 198, 255));

            //Create the map
            Dictionary<string, SharpMap.Styles.IStyle> styles = new Dictionary<string, IStyle>();
            styles.Add("51", landStyle);
            styles.Add("36", waterStyle);

            //Assign the theme
            vlay.Theme = new SharpMap.Rendering.Thematics.UniqueValuesTheme<string>("kode", styles, landStyle);

            mapBox1.Map.Layers.Add(vlay);
            mapBox1.Map.ZoomToExtents();
            mapBox1.Refresh();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
          
        }

        private void mapBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
