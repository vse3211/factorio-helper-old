using System;
using System.Dynamic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace Factorio_Helper
{
    public partial class UpdatedMods : Form
    {
        WebClient client = new WebClient();
        string source;
        dynamic MLI;
        dynamic NML;
        string[] oldList;
        string[] newList;
        int oldN = 0;
        int newN = 0;
        bool downloadCheck = false;

        public UpdatedMods()
        {
            InitializeComponent();
            MLI = JsonConvert.DeserializeObject(downloader("?page_size=max"), typeof(LegalMods));
            NML = JsonConvert.DeserializeObject(File.ReadAllText("mods.json"), typeof(ModsInfoList));
            int MLIL = MLI.results.Count;
            int NMLL = NML.mods.Count;
            oldList = new string[MLIL];
            newList = new string[NMLL];
            getMod(null);
        }
        
        private string getMod(string DM)
        {
            string returned = null;
            if (DM == null)
            {
                var s1 = MLI.results;
                foreach (var v in s1)
                {
                    string name = v.name;
                    string version = v.latest_release.version;
                    foreach (var ss in NML.mods)
                    {
                        if (ss != null)
                        {
                            if (ss.name == name && !listBox2.Items.Contains(ss.name))
                            {
                                if (ss.version != version)
                                {
                                    listBox2.Items.Add(name);
                                    newList[newN] = name;
                                    newN++;
                                }
                                else
                                {
                                    if (newList.Contains(name))
                                    {
                                        for (int i = 0; i == newList.Length; i++)
                                        {
                                            if (newList[i] == name)
                                            {
                                                newList[i] = null;
                                            }
                                        }
                                    }
                                    listBox1.Items.Add(name);
                                    oldList[oldN] = name;
                                    oldN++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var s1 = MLI.results;
                foreach (var v in s1)
                {
                    string version = null;
                    string factorio_version = null;
                    string name = null;
                    try { version = v.latest_release.version; }
                    catch (Exception ex) { richTextBox1.Text = richTextBox1.Text + "Error: " + ex + "\n"; }
                    try { factorio_version = v.latest_release.info_json.factorio_version; }
                    catch (Exception ex) { richTextBox1.Text = richTextBox1.Text + "Error: " + ex + "\n"; }
                    try { name = v.name; }
                    catch (Exception ex) { richTextBox1.Text = richTextBox1.Text + "Error: " + ex + "\n"; }
                    if (DM == name)
                    {
                        downloadCheck = true;
                        source = v.latest_release.file_name;
                        returned = v.latest_release.download_url;
                    }
                }
            }
            return returned;
        }

        private void saveList(dynamic sender, EventArgs e)
        {
            if (sender.Text.Contains("старый"))
            {
                saveFileDialog1.Title = "Сохранить список старых модов...";
                saveFileDialog1.ShowDialog();
                File.WriteAllLines(saveFileDialog1.FileName, oldList);
            }
            else
            {
                saveFileDialog1.Title = "Сохранить список новых модов...";
                saveFileDialog1.ShowDialog();
                File.WriteAllLines(saveFileDialog1.FileName, newList);
            }
        }

        private void downloadMod(object sender, EventArgs e)
        {
            downloadCheck = false;
            string fileName = null;
            if (radioButton1.Checked && !checkBox1.Checked)
            {
                fileName = getMod(listBox1.SelectedItem.ToString());
            }
            else if (radioButton2.Checked && !checkBox1.Checked)
            {
                fileName = getMod(listBox2.SelectedItem.ToString());
            }
            else
            {
                if (!checkBox1.Checked)
                {
                    richTextBox1.Text = richTextBox1.Text + "List not selected";
                    goto endDownloading;
                }
            }

            if (downloadCheck && fileName != null && !checkBox1.Checked)
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("https://mods.factorio.com" + fileName + "?username=" + textBox1.Text + "&token=" + textBox2.Text), @".\Mods\" + source);
            }
            else if (checkBox1.Checked)
            {
                foreach (var ss in selectList())
                {
                    if (ss != null && ss != "")
                    {
                        fileName = getMod(ss);
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        client.DownloadFile(new Uri("https://mods.factorio.com" + fileName + "?username=" + textBox1.Text + "&token=" + textBox2.Text), @".\Mods\" + source);
                        richTextBox1.Text = richTextBox1.Text + ss + " downloaded.\n";
                    }
                }

                string[] selectList()
                {
                    string[] ss;
                    if (radioButton1.Checked) ss = oldList;
                    else ss = newList;
                    return ss;
                }
            }
            else richTextBox1.Text = richTextBox1.Text + "Mod not founded";
            endDownloading:;
        }

        private void cleanString(dynamic sender, EventArgs e)
        {
            sender.Text = "";
        }

        #region Downloading module
        #region Downloading completed
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text + "Downloading mod " + source + " completed";
        }
        #endregion
        #region Download progress changed
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar1.Value = (int)e.BytesReceived / 100;
        }
        #endregion
        #endregion

        private void radioButton1_CheckedChanged(dynamic sender, EventArgs e)
        {
            if (sender.Checked)
            {
                source = listBox1.Text;
            }
        }
        
        private void radioButton2_CheckedChanged(dynamic sender, EventArgs e)
        {
            if (sender.Checked)
            {
                source = getFileName(listBox2.Text);
                string getFileName(string st)
                {
                    

                    return st;
                }
            }
        }
    }


}
