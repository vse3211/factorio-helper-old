using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Factorio_Helper
{
    /// <summary>
    /// Mods
    /// </summary>
    public class ModsInfoList
    {
        public List<ModInfo> mods { get; set; }
    }

    public class ModInfo
    {
        public string name { get; set; }
        public string version { get; set; }
        public string title { get; set; }
        public string factorio_version { get; set; }
        public string[] dependencies { get; set; }
    }
    /// <summary>
    /// END Mods
    /// </summary>

    public class Links
    {
        public object first { get; set; }
        public string next { get; set; }
        public object prev { get; set; }
        public string last { get; set; }
    }

    public class Pagination
    {
        public int count { get; set; }
        public int page { get; set; }
        public int page_count { get; set; }
        public int page_size { get; set; }
        public Links links { get; set; }
    }

    public class InfoJson
    {
        public string factorio_version { get; set; }
    }

    public class LatestRelease
    {
        public string download_url { get; set; }
        public string file_name { get; set; }
        public InfoJson info_json { get; set; }
        public DateTime released_at { get; set; }
        public string version { get; set; }
        public string sha1 { get; set; }
    }

    public class Result
    {
        public string name { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public string summary { get; set; }
        public int downloads_count { get; set; }
        public string category { get; set; }
        public double score { get; set; }
        public LatestRelease latest_release { get; set; }
    }

    public class LegalMods
    {
        public Pagination pagination { get; set; }
        public IList<Result> results { get; set; }
    }



    /// <summary>
    /// Game
    /// </summary>
    //Game st ex
    public class MyArray
    {
        public List<Versions> stable { get; set; }
        public List<Versions> experimental { get; set; }
    }

    //Game versions
    public class Versions
    {
        public string version { get; set; }
        public string system { get; set; }
    }

    //Game version data class
    class gameVersionInfo
    {
        public string version { get; set; }
    }
    /// <summary>
    /// END Game
    /// </summary>

    /// <summary>
    /// Search universval
    /// </summary>
    partial class Form1
    {
        void searchMessage()
        {
            richTextBox1.Text = richTextBox1.Text + "\n\nВведите частичное или полное название для поиска и нажмите кнопку \"Найти\"";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
    }
    /// <summary>
    /// END Search universal
    /// </summary>

    partial class Form1
    {
        public string downloader(string downloadGroup, string fileToDownload)
        {
            WebRequest request = WebRequest.Create("http://londev.ru/downloads/factorio/" + downloadGroup + "/" + fileToDownload);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }

        public string simpleDownloader(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
    }

    partial class UpdatedMods
    {
        public string downloader(string sc)
        {
            WebRequest request = WebRequest.Create("https://mods.factorio.com/api/mods" + sc);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
    }
}
