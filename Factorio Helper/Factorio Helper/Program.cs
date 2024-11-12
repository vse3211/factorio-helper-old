using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using DirectorySearch;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;

namespace Factorio_Helper
{
    //class MDCheck
    //{
    //    string token;

    //    public void GETmd5FromServer(string source)
    //    {
    //        MD5 md5Hash = MD5.Create();
    //        string md5 = GetMd5Hash(md5Hash, source);
    //        string uri = "http://londev.ru/factorio-helper/files/";
    //        string data = "fileName=" + source + "&md5=" + md5;
    //        token = GET(uri, data);
    //        //debug("get test");
    //    }

    //    public string GetMd5Hash(MD5 md5Hash, string input)
    //    {
    //        FileStream file = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    //        MD5 md5 = new MD5CryptoServiceProvider();
    //        byte[] retVal = md5.ComputeHash(file);
    //        file.Close();

    //        StringBuilder sb = new StringBuilder();
    //        for (int i = 0; i < retVal.Length; i++)
    //        {
    //            sb.Append(retVal[i].ToString("x2"));
    //        }
    //        return sb.ToString();
    //    }

    //    private string GET(string Url, string Data)
    //    {
    //        WebRequest req = WebRequest.Create(Url + "?" + Data);
    //        WebResponse resp = req.GetResponse();
    //        Stream stream = resp.GetResponseStream();
    //        StreamReader sr = new StreamReader(stream);
    //        string Out = sr.ReadToEnd();
    //        sr.Close();
    //        return Out;
    //    }
    //}

    class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0)
                {
                    if (args[0] == "-startupdate")
                    {
                        //Thread.Sleep(500);
                        //File.Move("Factorio Helper.exe", "FH.old.exe");
                        //Process.Start("FH.old.exe", "-update");
                        //Application.ExitThread();
                        Thread.Sleep(1500);
                        File.Delete("Factorio Helper.exe");
                        File.Copy("FH.new.exe", "Factorio Helper.exe");
                        Process.Start("Factorio Helper.exe", "-remove");
                        Application.Exit();
                        goto EndOfFile;
                    }
                    else if (args[0] == "-startupdate86")
                    {
                        Thread.Sleep(1500);
                        File.Delete("Factorio Helper_x86.exe");
                        File.Copy("FH_x86.new.exe", "Factorio Helper_x86.exe");
                        Process.Start("Factorio Helper_x86.exe", "-remove86");
                        Application.Exit();
                        goto EndOfFile;
                    }
                    else if (args[0] == "-update")
                    {
                        Thread.Sleep(1500);
                        File.Move("FH.new.exe", "Factorio Helper.exe");
                        Process.Start("Factorio Helper.exe", "-remove");
                        Application.Exit();
                        goto EndOfFile;
                    }
                    else if (args[0] == "-remove")
                    {
                        Thread.Sleep(1500);
                        /**/
                        File.Delete("FH.new.exe");
                    }
                    else if (args[0] == "-remove86")
                    {
                        Thread.Sleep(1500);
                        /**/
                        File.Delete("FH_x86.new.exe");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении FH.exe!\n\n" + "Error:\n" + ex.Message);
            }

            goto JustStart;
            #region old
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists("Factorio Helper.application"))
            {
                CopyFile(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Factorio Helper" + @"\Factorio Helper.exe", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Factorio Helper");
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Factorio Helper" + @"\Factorio Helper.exe", "-reinstall");
                goto EndOfFile;
            }
            
            try
            {
                if (args.Length != 0)
                {
                    if (args[0] == "-remove")
                    {
                        Thread.Sleep(500);
                        File.Delete(args[1]);
                        if (!File.Exists("Updater.exe")) File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Updater.exe", Properties.Resources.Updater);
                        Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"\Updater.exe");
                        goto EndOfFile;
                    }
                    else if (args[0] == "-reinstall")
                    {
                        Thread.Sleep(500);
                        try
                        {
                            Process.Start("rundll32", @"%SystemRoot%\system32\dfshim.dll CleanOnlineAppCache");
                            DS.RemoveSearch(DS.localAppData + @"\Apps\2.0", "fact");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            goto EndOfFile;
                        }
                        MessageBox.Show("!!!Внимание!!!\nБыла обнаружена старая система обновления и программа была переустановлена.\nПрограмма помещена на рабочий стол");
                        goto EndOfFile;
                    }
                    else if (args[0] == "-0523719")
                    {
                        goto JustStart;
                    }
                    else if (args[0] == "-update")
                    {
                        Thread.Sleep(500);
                        System.Net.WebClient client = new System.Net.WebClient();
                        client.DownloadFile(new Uri("http://londev.ru/factorio-helper/files/" + "Updater.exe"), "Updater.exe");
                        Process.Start("Updater.exe");
                        goto EndOfFile;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при установке!\nУдалите установочный файл из каталога загрузок самостоятельно.\n\n" + "Error:\n" + ex.Message);
                MessageBox.Show("Попытка запуска...");
            }
            if (File.Exists("uap.dll")) goto StartEXE;

            #region Installer (Отключен за ненадобностью)
            /*string currentPATH = Environment.CurrentDirectory;
            string currentFileName = Application.ExecutablePath;
            string pfPATH = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string targetPATH = pfPATH + @"\Factorio Helper";
            DialogResult installerDialogResult = DialogResult.None;

            

            if (Application.ExecutablePath != targetPATH + @"\Factorio Helper.exe")
            {
                installerDialogResult = MessageBox.Show(
                    "Для установки программы нажмите Да\n" +
                    "Для установки приложения в текущий каталог нажмите Нет",
                    "Установка Factorio Helper",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button3,
                    MessageBoxOptions.DefaultDesktopOnly);
            }

            if (installerDialogResult == DialogResult.Yes)
            {
                Directory.CreateDirectory(targetPATH);
                CopyFile(Application.ExecutablePath, targetPATH + @"\Factorio Helper.exe");
                ShortCut.Create(
                    targetPATH + @"\Factorio Helper.exe",
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Factorio Helper.lnk",
                    "",
                    "Factorio Helper by LonDev team");
                Process.Start(targetPATH + @"\Factorio Helper.exe", "-remove " + '"' + Application.ExecutablePath + '"');
                goto EndOfFile;
            }
            else if (installerDialogResult == DialogResult.No)
            {
                File.WriteAllText("uap.dll", "");
                goto StartEXE;
            }
            else if (installerDialogResult == DialogResult.Cancel) goto EndOfFile;

            
            */
            #endregion

            void CopyFile(string sourcefn, string destinfn, string dspath)
            {
                FileInfo fn = new FileInfo(sourcefn);
                if (!Directory.Exists(dspath)) Directory.CreateDirectory(dspath);
                fn.CopyTo(destinfn, true);
            }

        StartEXE:;

            Application.EnableVisualStyles();
            if (!File.Exists("Updater.exe")) File.WriteAllBytes("Updater.exe", Properties.Resources.Updater);
            Process.Start("Updater.exe");
            goto EndOfFile;
        #endregion

        //JustStart:;

            Updater:

            string updaterErrors = $"Updater v0.2 Errors log:\nDate: {new DateTime()}\nOS: {Environment.OSVersion.Version} {GetProductType()}\nErrors:\n";
            string updaterErrorsCheck = updaterErrors.ToString();
            string filesToUpdate = "";
            string filesToCheck = "";
            if (File.Exists("updaterErrorLog.txt")) File.Delete("updaterErrorLog.txt");

            var dir = new DirectoryInfo(".\\"); // папка с файлами 

            foreach (FileInfo file in dir.GetFiles())
            {
                dynamic filePath = file;
                if (filesToCheck != "")
                {
                    filesToCheck = filesToCheck + $",{filePath.Name}";
                    //filesToCheck = filesToCheck + $",{Path.GetFileNameWithoutExtension(file.FullName)}";
                }
                else if (filesToCheck == "")
                {
                    filesToCheck = filePath.Name;
                    //filesToCheck = Path.GetFileName(file.FullName);
                }
            }

            string[] filesListToCheck = filesToCheck.Split(',');
            foreach (string str in filesListToCheck)
            {
                if (str != "" && str != null)
                    GETmd5FromServer(str);


                if (token == "error" && str != "Factorio Helper.exe" && str != "Factorio Helper_x86.exe")
                {
                    //If update avalible
                    if (File.Exists(str)) File.Delete(str);
                    WebClient client = new WebClient();
                    client.DownloadFile(new Uri("http://londev.ru/factorio-helper/files/" + str), str);
                }
                else if (token == "NE2" && exFiles(str) == true)
                {
                    updaterErrors = updaterErrors + $"Update error in {str}\n";
                }
                else if (token == "true")
                {
                    Console.WriteLine(str + " OK");
                }
                else if (token == "error" && str == "Factorio Helper.exe")
                {
                    MessageBox.Show("Обнаружено обновление!\nПрограмма будет автоматически обновлена и запущена.");
                    WebClient client = new WebClient();
                    client.DownloadFile(new Uri("http://londev.ru/factorio-helper/files/" + str), "FH.new.exe");
                    Process.Start("FH.new.exe", "-startupdate");
                    goto EndOfFile;
                }
                else if (token == "error" && str == "Factorio Helper_x86.exe")
                {
                    MessageBox.Show("Обнаружено обновление!\nПрограмма будет автоматически обновлена и запущена.");
                    WebClient client = new WebClient();
                    client.DownloadFile(new Uri("http://londev.ru/factorio-helper/files/" + str), "FH_x86.new.exe");
                    Process.Start("FH_x86.new.exe", "-startupdate86");
                    goto EndOfFile;
                }
                else if (token == "NE" && exFiles(str) == true)
                {
                    Console.WriteLine(str + " SKIP");
                }
                else
                {
                    MessageBox.Show($"No internet connection!\nShutdown FHA...\nFor support:\n{token}");
                    goto EndOfFile;
                }
            }
            if (updaterErrors != updaterErrorsCheck)
            File.WriteAllText("updaterErrorLog.txt", updaterErrors);
            JustStart:;
            //Program:

            Application.EnableVisualStyles();
            try
            {
                //Application.Run(new CodeGen());
                //Application.Run(new UpdatedMods());
                Application.Run(new Form1());
                //Application.Run(new factorioWebTest());
            }
            catch (Exception ex)
            {
                DialogResult errorChoise = MessageBox.Show($"{ex.Message}\n{ex.StackTrace}\n{ex.Source}" + "\nПерезапустить программу с новыми настройками?", "Program error!", MessageBoxButtons.YesNo);
                if (File.Exists("network.fha"))
                {
                    switch (File.ReadAllText("network.fha"))
                    {
                        case "Ssl3":
                            File.WriteAllText("network.fha", "SystemDefault");
                            break;
                        default:
                        case "SystemDefault":
                            File.WriteAllText("network.fha", "Tls");
                            break;
                        case "Tls":
                            File.WriteAllText("network.fha", "Tls11");
                            break;
                        case "Tls11":
                            File.WriteAllText("network.fha", "Tls12");
                            break;
                        case "Tls12":
                            File.WriteAllText("network.fha", "Ssl3");
                            break;
                    }
                }
                if (errorChoise == DialogResult.Yes) Application.Run(new Form1());
                else MessageBox.Show("При следующем запуске будут использованы другие настройки!\nИзменить настройки вы можете в файле network.fha\n", "Crash handler info");
            }
        EndOfFile:;
        }
        static string token;

        static bool exFiles(string fname)
        {
            bool checkers = true;
            string[] filesList = new string[] { "CsQuery.xml"
                + "days.checker"
                + "dir.config"
                + "Factorio Helper.application"
                + "Factorio Helper.exe.config"
                + "Factorio Helper.exe.manifest"
                + "Factorio Helper.pdb"
                + "Flurl.Http.xml"
                + "Flurl.xml"
                + "HtmlAgilityPack.pdb"
                + "HtmlAgilityPack.xml"
                + "Microsoft.Extensions.DependencyInjection.Abstractions.xml"
                + "Microsoft.Extensions.DependencyInjection.xml"
                + "Microsoft.Extensions.Logging.Abstractions.xml"
                + "mods.json"
                + "Newtonsoft.Json.pdb"
                + "Newtonsoft.Json.Schema.xml"
                + "Newtonsoft.Json.xml" };
            foreach (string file in filesList)
            {
                if (fname == file) checkers = false;
            }
            return checkers;
        }

        static string GetProductType()
        {
            string value = String.Empty;
            string key = @"SYSTEM\CurrentControlSet\Control\ProductOptions";
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (regKey != null)
                {
                    try
                    {
                        switch (regKey.GetValue("ProductType").ToString())
                        {
                            case "WinNT": value = "work"; break;
                            case "LanmanNT": value = "domen"; break;
                            case "ServerNT": value = "server"; break;
                            default: value = "other"; break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                    return value;
                }
                else
                    return "не удалось найти указанный ключ";
            }
        }

        static public void GETmd5FromServer(string source)
        {
            MD5 md5Hash = MD5.Create();
            string md5 = GetMd5Hash(md5Hash, source);
            string uri = "http://londev.ru/factorio-helper/files/";
            string data = "fileName=" + source + "&md5=" + md5;
            token = GET(uri, data);
            //debug("get test");
        }

        static public string GetMd5Hash(MD5 md5Hash, string input)
        {
            FileStream file = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

        static private string GET(string Url, string Data)
        {
            WebRequest req = WebRequest.Create(Url + "?" + Data);
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }

    }
}