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

namespace LogoDetectionFANET45
{
    class Instagram
    {
        static RichTextBox _richTextBox2 = (RichTextBox)Application.OpenForms["Form1"].Controls.Find("richTextBox2", false).FirstOrDefault();

        public static void FetchByUser(string userid)
        {
            _richTextBox2.Clear();
            _richTextBox2.AppendText("Fetching Instagram API data...");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://api.instagram.com/v1/users/" + userid + "/media/recent/?access_token=" + Credentials.access_token);
            HttpWebResponse httpWebReponse = (HttpWebResponse)request.GetResponse();
            System.IO.Stream dataStream = httpWebReponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string UserJson = reader.ReadToEnd();
            dataStream.Close();
            reader.Close();

            JsonValue JsonUser = JsonObject.Parse(UserJson);
            String[] ImageURLUser = new String[JsonUser["data"].Count];

            for (int i = 0; i < JsonUser["data"].Count; i++)
            {
                ImageURLUser[i] = (string)JsonUser["data"][i]["images"]["standard_resolution"]["url"];
            }
            _richTextBox2.Clear();
            _richTextBox2.AppendText("Request client: https://api.instagram.com/v1/users/" + userid + "/media/recent/?access_token=**** \n");
            _richTextBox2.AppendText("Respon Instagram API: \n");
            for (int i = 0; i < ImageURLUser.Length; i++)
            {
                string output = (string)ImageURLUser[i];
                _richTextBox2.AppendText(output.Replace(@"\", String.Empty) + "\n");
            }
        }

        public static void FetchByTag(string tag)
        {
            InstaSharp.Endpoints.Tags.Unauthenticated IGTags = new InstaSharp.Endpoints.Tags.Unauthenticated(Credentials.config);
            InstaSharp.Model.Responses.MediasResponse responTags = IGTags.Recent(tag, "0", "1");
            String resultTags = responTags.Json;
            JsonValue JsonTags = JsonValue.Parse(resultTags);
            String[] ImageURLTags = new String[JsonTags["data"].Count];

            for (int i = 0; i < JsonTags["data"].Count; i++)
            {
                ImageURLTags[i] = (string)JsonTags["data"][i]["images"]["standard_resolution"]["url"];
            }
            _richTextBox2.Clear();
            _richTextBox2.AppendText("Request client: https://api.instagram.com/v1/" + tag + "/snow/media/recent \n");
            _richTextBox2.AppendText("Respon Instagram API: \n");
            for (int i = 0; i < ImageURLTags.Length; i++)
            {
                string output = (string)ImageURLTags[i];
                _richTextBox2.AppendText(output.Replace(@"\", String.Empty) + "\n");
            }
        }

        public static void FetchByLocation(string locationid)
        {
            InstaSharp.Endpoints.Locations.Unauthenticated IGLocation = new InstaSharp.Endpoints.Locations.Unauthenticated(Credentials.config);
            String resultLocation = IGLocation.RecentJson(locationid);
            JsonValue JsonLocation = JsonValue.Parse(resultLocation);
            String[] ImageURLLocation = new String[JsonLocation["data"].Count];

            for (int i = 0; i < JsonLocation["data"].Count; i++)
            {
                ImageURLLocation[i] = (string)JsonLocation["data"][i]["images"]["standard_resolution"]["url"];
            }
            _richTextBox2.Clear();
            _richTextBox2.AppendText("Request client: https://api.instagram.com/v1/locations/" + locationid + " \n");
            _richTextBox2.AppendText("Respon Instagram API: \n");
            for (int i = 0; i < ImageURLLocation.Length; i++)
            {
                string output = (string)ImageURLLocation[i];
                _richTextBox2.AppendText(output.Replace(@"\", String.Empty) + "\n");
            }
        }
    }
}
