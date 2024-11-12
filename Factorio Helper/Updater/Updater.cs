using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Updater
{
    public partial class Updater : Form
    {
        string token;
        public string source;

        public Updater()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (Directory.GetFiles(@"./").Length <= 2)
            {
                string[] files;
                MessageBox.Show("Внимание!\nПри первом запуске приложения производиться автоматическая распаковка файлов программы и их обновление");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\files.list", Properties.Resources.FHfiles);
                files = File.ReadAllLines("files.list");
                File.Delete("files.list");
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        File.WriteAllText(file, "");
                    }
                }
            }
            MessageBox.Show("Не используйте папку с FH для хранения каких либо файлов!\nВсе посторонние файлы будут удалены!\nЖми ОК, если готов...");
            Refresh();
            update(null);
            System.Diagnostics.Process.Start("Factorio Helper.exe", "-0523719");
            Application.ExitThread();
        }

        void update(string v)
        {
            source = "Updater.exe";
            GETmd5FromServer();
            if (token == "error")
            {
                MessageBox.Show("Updater устарел и будет обновлен автоматически");
            FHCheck:;
                source = "Factorio Helper.exe";
                GETmd5FromServer();
                if (token == "true")
                {
                    System.Diagnostics.Process.Start("Factorio Helper.exe", "-update");
                    Application.ExitThread();
                }
                else
                {
                    File.Delete(source);
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri("http://londev.ru/factorio-helper/files/" + source), source);
                    goto FHCheck;
                }
            }

            string[] filesname = Directory.GetFiles(@"./");
            int count = 0;
            progressBar1.Maximum = (filesname.Length * 10000) / 100;
            foreach (var st in filesname)
            {
                source = st.Split('/')[1].ToString();
                GETmd5FromServer();
                if (token == "error" && source != "Updater.exe")
                {
                    File.Delete(source);
                    WebClient client = new WebClient();
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri("http://londev.ru/factorio-helper/files/" + source), source);
                }
                else if (token == "NE" && source != "Updater.exe" && source != "uap.dll" && source != "dir.config")
                {
                    File.Delete(source);
                    label1.Text = "File " + source + " removed.";
                }
                else if (token == "true")
                {
                    label1.Text = source + " OK";
                }

                count++;
                progressBar1.Value = (count * 10000) / 100;
            }
            Application.ExitThread();
        }

        void debug(string v)
        {
            if (source == "Factorio Helper.exe" && v == "get test")
            {
                Console.WriteLine(source + " " + token);
            }
            else if (v == "get test")
            {
                Console.WriteLine(source);
            }
        }

        void GETmd5FromServer()
        {
            MD5 md5Hash = MD5.Create();
            string md5 = GetMd5Hash(md5Hash, source);
            string uri = "http://londev.ru/factorio-helper/files/";
            string data = "fileName=" + source + "&md5=" + md5;
            token = GET(uri, data);
            //debug("get test");
        }

        private static string GET(string Url, string Data)
        {
            WebRequest req = WebRequest.Create(Url + "?" + Data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        
        string GetMd5Hash(MD5 md5Hash, string input)
        {
            FileStream file = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #region Downloading module
        #region Downloading completed
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label1.Text = source + " updated";
        }
        #endregion
        #region Download progress changed
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar2.Value = (int)e.BytesReceived / 100;
        }
        #endregion
        #endregion
    }
}
