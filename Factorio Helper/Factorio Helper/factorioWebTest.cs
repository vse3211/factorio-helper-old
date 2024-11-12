using CsQuery;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;

namespace Factorio_Helper
{
    public partial class factorioWebTest : Form
    {
        public factorioWebTest()
        {
            InitializeComponent();
        }

        private void getVersionList()
        {
            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create("https://factorio.com/login");
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            #region Get CSRF-Token
            File.WriteAllText("site.html", responseFromServer);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load("site.html");

            File.Delete("site.html");

            dynamic node = doc.DocumentNode.SelectNodes("//input[@name=\"csrf_token\"]/@value");
            dynamic csrf = node[0].Attributes[3];
            dynamic csrfToken = csrf.Value;
            #endregion

            string data = $"csrf_token={csrfToken}&username_or_email={login.Text}&password={pass.Text}";
            request = WebRequest.Create("https://factorio.com/login");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] sentData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = sentData.Length;
            request.Credentials = CredentialCache.DefaultCredentials;
            Stream sendStream = request.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusDescription);
            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();


            // Display the content.
            richTextBox1.Text = responseFromServer;
            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            getVersionList();
        }
    }
}
