using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using Hooks;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CsQuery.ExtensionMethods.Internal;

namespace Factorio_Helper
{
    public partial class Form1 : Form
    {
        class StackPanel : TabControl
        {
            protected override void WndProc(ref Message m)
            {
                // Hide tabs by trapping the TCM_ADJUSTRECT message
                if (m.Msg == 0x1328 && !DesignMode) m.Result = (IntPtr)1;
                else base.WndProc(ref m);
            }
        }

        public int debug = 1;


        #region Start & config
        #region Публичные переменные
        public int i;
        public int verID;
        public string downloadingType = null;
        public string[][] stable = new string[101][];
        public string[][] experimental = new string[101][];
        public string[][] stParsed;
        public string[][] expParsed;
        public string fileName;
        public bool overlay = false;
        public string theFileName = "";
        private Point mouseOffset;
        private bool isMouseDown = false;
        List<Keys> _pressedKeys;
        public ModsInfoList MLI;
        public string modFileName;
        public string modGameVersion;
        public string modName;
        public string modVersion;
        public string[] modDependencies;
        public string toUpdateMod;
        public string authStatus;
        #endregion
        #region Инициализация программы

        private static string GetID()
        {
            string str = "";
            ManagementObjectSearcher searcher =
                   new ManagementObjectSearcher("root\\CIMV2",
                   "SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                str = queryObj["ProcessorId"].ToString();
            }
            return str;
        }

        public Form1(SecurityProtocolType secConf = SecurityProtocolType.SystemDefault)
        {
            InitializeComponent();
            button5.Enabled = false;
            if (File.Exists("network.fha"))
            {
                switch (File.ReadAllText("network.fha"))
                {
                    case "Ssl3":
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                        break;
                    default:
                    case "SystemDefault":
                        ServicePointManager.SecurityProtocol = secConf;
                        break;
                    case "Tls":
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                        break;
                    case "Tls11":
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                        break;
                    case "Tls12":
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        break;
                }
            }
            else
            {
                ServicePointManager.SecurityProtocol = secConf;
            }
            #region TestID
            //MessageBox.Show(GetID());
            #endregion

            this.Size = new Size { Width = 623, Height = 489 };
            textBox5.Enabled = true;
            textBox5.Visible = true;
            textBox5.Visible = false;
            #region ToolTips
            toolTip1.SetToolTip(button9, "Страница управления модами");
            toolTip1.SetToolTip(button10, "Страница управления игрой");
            toolTip1.SetToolTip(textBox1, "Введите любой текст для поиска. Желательно первые буквы названия мода");
            toolTip1.SetToolTip(checkBox1, "Активирует и деактивирует кнопку перезапуска игры");
            toolTip1.SetToolTip(button8, "Перезапускает игру");
            toolTip1.SetToolTip(comboBox1, "Выберите нужный вам инструмент\nЕсли инструмент не открылся - повторите выбор");
            toolTip1.SetToolTip(button32, "ТОЛЬКО для опытных пользователей!");
            toolTip1.SetToolTip(this, "Перетащите в окно файл обновления, скачанный с https://londev.ru/factorio-helper");
            toolTip1.SetToolTip(pictureBox1, "Я помогу тебе переместить окно в другое место!\nПотяни за меня ;)");
            toolTip1.SetToolTip(richTextBox1, "Это консоль\nЗдесь ты увидишь все то, что программа делала, делает или будет делать");
            toolTip1.SetToolTip(label16, "Выбери элемент в списке требований и я расскажу, есть он у тебя или нет");
            toolTip1.SetToolTip(button12, "Настройки программы");
            toolTip1.SetToolTip(linkLabel1, "Mods path");
            toolTip1.SetToolTip(linkLabel2, "Game path");
            toolTip1.SetToolTip(linkLabel8, "Поверх всех окон\nПо умолчанию включена в режиме оверлея");
            toolTip1.SetToolTip(linkLabel14, "Переключает режим программы в OverlayMode\nПрограмма должна быть запущена в окне, чтобы это работало корректно!");
            toolTip1.SetToolTip(linkLabel9, "Откроет файл с путем к модам и игре в текстовом редакторе по умолчанию");
            toolTip1.SetToolTip(linkLabel12, "Просто чистит консоль");
            toolTip1.SetToolTip(button31, "Будет доступно в FHM после выхода GamesHelper");
            toolTip1.SetToolTip(button11, "Будет доступно в FHM после выхода GamesHelper");
            toolTip1.SetToolTip(checkBox4, "Включаете на свой страх и риск! Не рекомендуется на компьютерах без антивирусной защиты и системах старше Windows 10.");
            #endregion
            button30.Enabled = false;
            button31.Enabled = false;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            button8.Enabled = checkBox1.Checked;
            if (!File.Exists("days.checker"))
            {
                File.WriteAllText("days.checker", "0");
                var result = MessageBox.Show(
                    "Игра установлена на вашем ПК?",
                    "Настройка при первом запуске...",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.DefaultDesktopOnly);
                if (result == DialogResult.Yes)
                {
                    dirPathCheck();
                }
                else
                {
                    
                }
            }
            else
            {
                if (File.ReadAllText("days.checker") == "0")
                {
                    var result = MessageBox.Show(
                    "Игра установлена на вашем ПК?",
                    "Проверка наличия игры",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.DefaultDesktopOnly);
                    if (result == DialogResult.Yes)
                    {
                        dirPathCheck();
                    }
                    else
                    {
                        
                    }
                    File.WriteAllText("days.checker", Convert.ToString(Convert.ToInt32(File.ReadAllText("days.checker")) + 1));
                }
                else if (Convert.ToInt32(File.ReadAllText("days.checker")) >= 1)
                {
                    dirPathCheck();
                    File.WriteAllText("days.checker", Convert.ToString(Convert.ToInt32(File.ReadAllText("days.checker")) + 1));
                }
            }
            GetGameVersion();
            updateModsList();
            getAvalibleGameVersion();
            //button5.Visible = false;
            this.TopMost = false;
            linkLabel8.Text = "Не активно";
            linkLabel14.Text = "Overlay disabled";
            notifyIcon1.ShowBalloonTip(5);
            button1.Text = "Выбери мод";
            button1.Enabled = false;
            button2.Enabled = false;

            _pressedKeys = new List<Keys>();

            KBDHook.KeyDown += new KBDHook.HookKeyPress(KBDHook_KeyDown);
            KBDHook.KeyUp += new KBDHook.HookKeyPress(KBDHook_KeyUp);
            KBDHook.LocalHook = false;
            KBDHook.InstallHook();
            this.FormClosed += (s, e) => {
                KBDHook.UnInstallHook();
            };

            if (debug == 0)
            {
                comboBox1.Visible = true;
            }
            button28_Click(null, null);
        }

        void authCheck(object sender)
        {
            switchMode(0, 1);
            return;
            if (authStatus == "true")
            {
                switchMode(0, 1);
            }
            else if (authStatus == "false")
            {
                MessageBox.Show("Не корректный логин или пароль\nЕсли проблема повторится - обратитесь в поддержку");
                if (File.Exists("uap.dll")) File.Delete("uap.dll");
                MessageBox.Show("В целях безопасности мы удалили сохраненные авторизационные данные с вашего ПК");
                switchMode(0, 0);
            }
            else if (authStatus == "errorDatabase")
            {
                MessageBox.Show("Ошибка базы данных!\nМы запустим программу один раз, но вы должны сообщить об ошибке в поддержку.\n" + textBox5.Text + "\n" + textBox4.Text);
                switchMode(0, 1);
                //Report add
            }
            else if (authStatus == "errorGroup")
            {
                MessageBox.Show("Вы не подписаны на группу!\nПодпишитесь и попробуйте снова...");
                Process.Start("https://vk.com/factoriosave");
                MessageBox.Show("Запускаем браузер...");
                if (File.Exists("uap.dll")) File.Delete("uap.dll");
                MessageBox.Show("В целях безопасности мы удалили сохраненные авторизационные данные с вашего ПК");
                switchMode(0, 0);
            }
            else if (authStatus == "errorAccount")
            {
                MessageBox.Show("Имя пользователя уже кем-то занято\nПопробуйте другое");
                if (File.Exists("uap.dll")) File.Delete("uap.dll");
                MessageBox.Show("В целях безопасности мы удалили сохраненные авторизационные данные с вашего ПК");
                switchMode(0, 0);
            }
            else if (authStatus == "errorID")
            {
                if (sender == null)
                {
                    string[] lines;
                    lines = File.ReadAllLines("uap.dll");
                    authStatus = regUser(lines[0], lines[1], lines[2]);
                    if (authStatus == "true")
                    {
                        switchMode(0, 1);
                        goto end;
                    }
                    else if (authStatus == "false")
                    {
                        MessageBox.Show("Не корректный логин или пароль\nЕсли проблема повторится - обратитесь в поддержку");
                        switchMode(0, 0);
                        goto end;
                    }
                }
                else
                {
                    authStatus = auth(textBox5.Text, textBox4.Text, maskedTextBox2.Text);
                    if (authStatus == "true")
                    {
                        switchMode(0, 1);
                        goto end;
                    }
                    else if (authStatus == "false")
                    {
                        MessageBox.Show("Не корректный логин или пароль\nЕсли проблема повторится - обратитесь в поддержку");
                        switchMode(0, 0);
                        goto end;
                    }
                }
                MessageBox.Show("Аккаунт ВКонтакте уже кем-то используется!\nЕсли это ваш аккаунт - обратитесь в поддержку!");
                switchMode(0, 0);
            end:;
            }
            else if (authStatus == "errorArray")
            {
                MessageBox.Show("Вы не заполнили форму!");
                if (File.Exists("uap.dll")) File.Delete("uap.dll");
                MessageBox.Show("В целях безопасности мы удалили сохраненные авторизационные данные с вашего ПК");
                switchMode(0, 0);
            }
            else if (authStatus == "")
            {
                MessageBox.Show("Ошибка сервера!\nМы запустим программу один раз, но вы должны сообщить об ошибке в поддержку.\n" + textBox5.Text + "\n" + textBox4.Text);
                switchMode(0, 1);
                //Report add
            }
            else if (authStatus == "wrongRequest")
            {
                MessageBox.Show("Не корректный запрос!\nМы запустим программу один раз, но вы должны сообщить об ошибке в поддержку.\n" + textBox5.Text + "\n" + textBox4.Text);
                switchMode(0, 1);
                //Report add
            }
            else if (authStatus == "errorIdValidation")
            {
                MessageBox.Show("Проверьте, корректно ли введен ID" + textBox5.Text + "\n" + textBox4.Text);
                switchMode(0, 0);
            }
            else
            {
                MessageBox.Show("Неизвестная ошибка:\n" + authStatus);
                Application.ExitThread();
            }
        }
        #endregion
        #endregion

        #region Panel switcher

        //0 == FH auth, 1 == FH main, 2 == FD auth, 3 == FD main
        void switchAuth(int v)
        {
            //FH auth
            if (v == 0)
            {
                fhContainer.Panel1Collapsed = true;
                MessageBox.Show("Спасибо за использование Factorio Helper!\nПройдите авторизацию:\n VK ID = Номер страницы ВКонтакте (только цифры)\n Login = Любой логин для входа в программу\n ***** = Любой пароль для входа в программу");
            }
            //FH main
            else if (v == 1)
            {
                fhContainer.Panel2Collapsed = true;
            }
            //FD auth
            else if (v == 2)
            {
                fdContainer.Panel1Collapsed = true;
            }
            //FD main
            else if (v == 3)
            {
                fdContainer.Panel2Collapsed = true;
            }
        }

        //0 == FH, 1 == FD
        //v == Mode, d == Auth
        void switchMode(int v, int d)
        {
            if (v == 0)
            {
                mainContainer.Panel2Collapsed = true;
                switchAuth(d);
            }
            else if (v == 1)
            {
                mainContainer.Panel1Collapsed = true;
                switchAuth(d);
            }
        }
        #endregion


        #region Check and set directory path
        void dirPathCheck()
        {
            if (File.Exists("dir.config"))
            {
                string[] cnfPath = File.ReadAllLines("dir.config");
                if (cnfPath.Length != 0)
                {
                    folderBrowserDialog1.SelectedPath = cnfPath[0];
                    folderBrowserDialog2.SelectedPath = cnfPath[1];
                }
                else
                {
                    reSETdir();
                }
            }
            else
            {
                reSETdir();
            }
            linkLabel1.Text = folderBrowserDialog1.SelectedPath;
            linkLabel2.Text = folderBrowserDialog2.SelectedPath;
            GetGameVersion();
        }

        void reSETdir()
        {
            folderBrowserDialog2.ShowDialog();
            folderBrowserDialog1.ShowDialog();
            string[] cnfPath = { folderBrowserDialog1.SelectedPath, folderBrowserDialog2.SelectedPath };
            File.WriteAllLines("dir.config", cnfPath);
        }

        void savePath()
        {
            if (File.Exists("dir.config"))
            {
                File.Delete("dir.config");
            }
            string[] cnfPath = { folderBrowserDialog1.SelectedPath, folderBrowserDialog2.SelectedPath };
            File.WriteAllLines("dir.config", cnfPath);
        }
        #endregion


        #region Работа с версиями игры

        #region Get and set game version
        void GetGameVersion()
        {
            if (File.Exists(folderBrowserDialog2.SelectedPath + @"\data\base\info.json"))
            {
                string infoJSON = File.ReadAllText(folderBrowserDialog2.SelectedPath + @"\data\base\info.json");
                var tmp = JsonConvert.DeserializeObject<gameVersionInfo>(infoJSON);
                label6.Text = tmp.version;
            }
            else
            {
                richTextBox1.Text = "ВНИМАНИЕ!\nПроизошла ошибка во время получения версии игры!\nПроверьте путь до папки с игрой!";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                label6.Text = "nan";
                button6.Enabled = false;
                button7.Enabled = false;
            }
            getAvalibleGameVersion();
        }
        #endregion

        #region Update avalable game version's
        string tempServerGame = "";
        void getAvalibleGameVersion()
        {
            WebRequest request = WebRequest.Create("https://fh.londev.ru/factorio/games.txt");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            try
            {
                tempServerGame = reader.ReadToEnd().RemoveWhitespace();
                button5.Enabled = true;
            }
            catch
            {

            }

            //MyArray temp = (MyArray)JsonConvert.DeserializeObject(downloader("Game", "available"), typeof(MyArray));
            /*
            if (temp != null && temp.stable != null)
            foreach (Versions item in temp.stable)
            {
                stable[i] = new string[] { null, item.version, item.system };
                i++;
            }
            i = 0;
            if (temp.experimental != null)
                foreach (Versions item in temp.experimental)
            {
                experimental[i] = new string[] { null, item.version, item.system };
                i++;
            }
            i = 0;*/
        }
        #endregion

        #region Выбор ветки игры и вывод информации
        //if stable
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            button5.Visible = false;
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            stParsed = null;
            stParsed = new string[101][];
            foreach (string[] gVer in stable)
            {
                if (gVer != null)
                {
                    string[] games = listBox3.Items.OfType<string>().ToArray();
                    bool has = games.Contains(gVer[1]);
                    if (has == false)
                    {
                        listBox3.Items.Add(gVer[1]);
                    }
                    else if (has == true)
                    {
                        stParsed[i] = new string[] {"true", gVer[1], gVer[2] };
                        i++;
                    }
                }
            }
            i = 0;
        }

        //if experimental
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            button5.Visible = false;
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            expParsed = null;
            expParsed = new string[101][];
            foreach (string[] gVer in experimental)
            {
                if (gVer != null)
                {
                    string[] games = listBox3.Items.OfType<string>().ToArray();
                    bool has = games.Contains(gVer[1]);
                    if (has == false)
                    {
                        listBox3.Items.Add(gVer[1]);
                    }
                    else if (has == true)
                    {
                        expParsed[i] = new string[] { "true", gVer[1], gVer[2] };
                        i++;
                    }
                }
            }
            i = 0;
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
            {
                button5.Visible = false;
                listBox4.Items.Clear();
                if (radioButton1.Checked == true)
                {
                    bool has = stParsed.Contains(listBox3.SelectedItem);
                    if (has == true)
                    {
                        listBox4.Items.Add("x86");
                        listBox4.Items.Add("x64");
                    }
                    else
                    {
                        foreach (string[] gVer in stable)
                        {
                            if (gVer != null && gVer[1] == Convert.ToString(listBox3.SelectedItem))
                            {
                                listBox4.Items.Add(gVer[2]);
                            }
                        }
                    }
                }
                else if (radioButton2.Checked == true)
                {
                    bool has = expParsed.Contains(listBox3.SelectedItem);
                    if (has == true)
                    {
                        listBox4.Items.Add("x86");
                        listBox4.Items.Add("x64");
                    }
                    else
                    {
                        foreach (string[] gVer in experimental)
                        {
                            if (gVer != null && gVer[1] == Convert.ToString(listBox3.SelectedItem))
                            {
                                listBox4.Items.Add(gVer[2]);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Download game button
        private void button5_Click(object sender, EventArgs e)
        {
            string isStExp;
            button5.Visible = false;
            if (radioButton1.Checked == true) isStExp = "stable"; else isStExp = "experimental";
            fileName = tempServerGame;
            downloadingType = "game";
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri("https://fh.londev.ru/factorio/" + fileName), fileName);
        }
        #endregion

        #region Update game version's button
        private void button4_Click(object sender, EventArgs e)
        {
            GetGameVersion();
            getAvalibleGameVersion();
        }
        #endregion

        #region Launch game
        private void button6_Click(object sender, EventArgs e)
        {
            gameLauncher();
        }
        void gameLauncher()
        {
            if (label6.Text != null && label6.Text != "" && label6.Text != "label6")
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    string sysType = "x64";
                    Process.Start(folderBrowserDialog2.SelectedPath + @"\bin\" + sysType + @"\factorio.exe");
                }
                else
                {
                    string sysType = "x86";
                    Process.Start(folderBrowserDialog2.SelectedPath + @"\bin\" + sysType + @"\factorio.exe");
                }
            }
            else
            {
                richTextBox1.Text = richTextBox1.Text + "\n======================================================" +
                    "\nЗапуск игры не возможен!" +
                    "\nУказан не корректный путь до папки с игрой.";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }
        #endregion

        #region Удаление игры
        private void button7_Click(object sender, EventArgs e)
        {
            if (label6.Text != null && label6.Text != "" && label6.Text != "label6")
            {
                Process.Start(folderBrowserDialog2.SelectedPath + @"\unins000.exe");
            }
        }
        #endregion

        #region Активация кнопки загрузки
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            button5.Visible = true;
        }
        #endregion

        #endregion
        

        #region Выбор каталогов
        #region Choise Mods Folder
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            linkLabel1.Text = folderBrowserDialog1.SelectedPath;
            savePath();
        }
        #endregion
        #region Choise Game Folder
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            linkLabel2.Text = folderBrowserDialog2.SelectedPath;
            savePath();
        }
        #endregion
        #endregion


        #region Mods module

        #region GET version list
        void wrtmodver()
        {
            MLI = (ModsInfoList)JsonConvert.DeserializeObject(simpleDownloader("https://mods.factorio.com/api/mods?page_size=max"), typeof(ModsInfoList));
        }
        #endregion

        #region Printing mods list
        void updateModsList()
        {
            #region Outside mods info parcer
            //wrtmodver();
            if (MLI is null || (MLI != null && MLI.mods is null)) return;
            richTextBox1.Text = richTextBox1.Text + "\nОбновлние списка модов...";
            listBox2.Items.Clear();
            listBox5.Items.Clear();
            listBox6.Items.Clear();
            label16.Text = "Welcome back!";
            label16.ForeColor = Color.OliveDrab;
            button1.Text = "Выбери мод";
            button1.Enabled = false;
            button2.Enabled = false;
            #region to history
            //WebRequest request = WebRequest.Create("https://londev.ru/downloads/factorio/Mods/");
            //WebResponse response = request.GetResponse();
            //Stream dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream);
            /*reader.ReadToEnd();*/
            #endregion
            #region only for test
            //var afterWebRequests = File.ReadAllText("my.json");
            //MLI = (ModsInfoList)JsonConvert.DeserializeObject(afterWebRequests, typeof(ModsInfoList));
            #endregion
            foreach (var item in MLI.mods)
            {
                if (item != null)
                {
                    bool has = listBox1.Items.Contains(item.title);
                    if (has == false)
                    {
                        listBox1.Items.Add(item.title);
                    }
                }
            }
            #endregion
            richTextBox1.Text = richTextBox1.Text + "\nГотово.";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        #endregion

        #region Download mod BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            getModInfo();
            string fileName = modFileName;

            if (File.Exists(folderBrowserDialog1.SelectedPath + "\\" + toUpdateMod)) File.Delete(folderBrowserDialog1.SelectedPath + "\\" + toUpdateMod);
            downloadingType = "mods";
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri("https://londev.ru/downloads/factorio/Mods/" + fileName), folderBrowserDialog1.SelectedPath + "\\" + fileName);
        }
        #endregion

        #region Delete mod BUTTON
        private void button2_Click(object sender, EventArgs e)
        {
            string fname = listBox2.SelectedItem.ToString();
            if (File.Exists(fname))
            {
                File.Delete(fname);
                listBox2.Items.Clear();
                string[] pathUserMods = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);
                string fnum;
                foreach (var fumPath in pathUserMods)
                {
                    fnum = Path.GetFileName(fumPath);
                    if (fnum != "mod-list.json")
                    {
                        listBox2.Items.Add(fnum);
                    }
                }
            }
            else
            {
                listBox2.Items.Clear();
                string[] pathUserMods = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);
                string fnum;
                foreach (var fumPath in pathUserMods)
                {
                    fnum = Path.GetFileName(fumPath);
                    if (fnum != "mod-list.json")
                    {
                        listBox2.Items.Add(fnum);
                    }
                }
            }
        }
        #endregion

        #region Update mods list BUTTON
        private void button3_Click(object sender, EventArgs e)
        {
            dirPathCheck();
            updateModsList();
        }
        #endregion

        #region selector
        //MODS Title selected
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                label16.Text = null;
                var selectedItem = listBox1.SelectedItem.ToString();
                //List<ModInfo> ModsInfo = Mods.FindAll(s => s.title == selectedItem); //it's work
                listBox2.Items.Clear();
                listBox5.Items.Clear();
                listBox6.Items.Clear();
                button1.Text = "Выбери версии";
                button1.Enabled = false;
                button2.Enabled = false;
                foreach (var item in MLI.mods)
                {
                    if (item != null)
                    {
                        if (item.factorio_version != null)
                        {
                            bool has = listBox2.Items.Contains(item.factorio_version);
                            if (item.title == selectedItem && has == false)
                            {
                                listBox2.Items.Add(item.factorio_version);
                            }
                        }
                        else
                        {
                            bool has = listBox2.Items.Contains("0.0.unknown");
                            if (item.title == selectedItem && has == false)
                            {
                                listBox2.Items.Add("0.0.unknown");
                            }
                        }
                    }
                }
            }
        }

        //MODS Game version selected
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                var selectedTitle = listBox1.SelectedItem.ToString();
                var selectedFactorioVersion = listBox2.SelectedItem.ToString();
                listBox5.Items.Clear();
                listBox6.Items.Clear();
                string selectedModGameVersion = listBox2.SelectedItem.ToString();
                string[] currentGameVersion = label6.Text.Split('.');
                string[] selectedModGVersion = selectedModGameVersion.Split('.');
                string vers1 = "0.0.0";
                try
                {
                    vers1 = currentGameVersion[1];
                }
                catch
                {
                    richTextBox1.Text = richTextBox1.Text + "\n\nВНИМАНИЕ! Вы не указали папку с игрой!";
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
                string vers2 = selectedModGVersion[1];
                if (vers1 != vers2)
                {
                    richTextBox1.Text = richTextBox1.Text + "\n\nВНИМАНИЕ! Поддерживаемая модом версия игры " + selectedModGameVersion + " не соответствует установленной версии игры " + label6.Text;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
                foreach (var item in MLI.mods)
                {
                    if (item != null)
                    {
                        if (item.factorio_version != null)
                        {
                            bool has = listBox5.Items.Contains(item.version);
                            if (item.title == selectedTitle && item.factorio_version == selectedFactorioVersion && has == false)
                            {
                                listBox5.Items.Add(item.version);
                            }
                        }
                        else
                        {
                            bool has = listBox5.Items.Contains(item.version);
                            if (item.title == selectedTitle && "0.0.unknown" == selectedFactorioVersion && has == false)
                            {
                                listBox5.Items.Add(item.version);
                            }
                        }

                    }
                }
            }
        }

        //MODS version selected
        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            getModInfo();
            listBox6.Items.Clear();
            if (modDependencies != null)
            {
                foreach (var i2 in modDependencies)
                {
                    listBox6.Items.Add(i2);
                }
            }
            string[] pathUserMods;
            try
            {
                pathUserMods = null;
                pathUserMods = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);
            }
            catch
            {
                pathUserMods = new string[] { "C:\\" };
                richTextBox1.Text = richTextBox1.Text + "\n\nВНИМАНИЕ! Вы указали не корректный путь до папки с модами!";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
            string fnum;
            bool result = false;
            foreach (var fumPath in pathUserMods)
            {
                fnum = Path.GetFileName(fumPath);
                foreach (var versionsMod in listBox5.Items)
                {
                    string checkFNUM = modName + "_" + versionsMod + ".zip";
                    if (fnum == checkFNUM)
                    {
                        button1.Text = "Обновить";
                        button1.Enabled = true;
                        button2.Enabled = true;
                        result = true;
                        toUpdateMod = checkFNUM.ToString();
                    }
                    else if (result)
                    {
                        button1.Text = "Обновить";
                        button2.Enabled = true;
                        button1.Enabled = true;
                    }
                    else if (result == false)
                    {
                        button1.Text = "Скачать";
                        button1.Enabled = true;
                        button2.Enabled = false;
                        toUpdateMod = null;
                    }
                }
            }
        }

        //Search & get mod info
        void getModInfo()
        {
            if (listBox5.SelectedItem != null)
            {
                var selectedTitle = listBox1.SelectedItem.ToString();
                var selectedFactorioVersion = listBox2.SelectedItem.ToString();
                var selectedModVersion = listBox5.SelectedItem.ToString();
                foreach (var item in MLI.mods)
                {
                    if (item != null)
                    {
                        if (item.title == selectedTitle && item.factorio_version == selectedFactorioVersion && item.version == selectedModVersion)
                        {
                            theFileName = selectedTitle;
                            modFileName = item.name + "_" + item.version + ".zip";
                            modGameVersion = item.factorio_version;
                            modName = item.name;
                            modVersion = item.version;
                            modDependencies = item.dependencies;
                        }
                    }
                }
            }
        }

        //MODS dependencies selected
        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] pathUserMods = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*", SearchOption.AllDirectories);
                string fnum;
                bool result = false;
                bool needed = false;
                bool conflict = false;
                string p = listBox6.SelectedItem.ToString();
                richTextBox1.Text += $"\n\n{listBox6.SelectedItem.ToString()}";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                string p1 = p;
                p1 = p1.Trim(new char[] { '?', '>', ' ', '\n', '!', '(', ')' });
                string[] p2 = p1.Split('=', '>', '\n', ' ');
                string versionsMod = listBox5.SelectedItem.ToString();
                foreach (var fumPath in pathUserMods)
                {
                    fnum = Path.GetFileName(fumPath);
                    if (fnum.Contains($"{p2[0]}_{p2[p2.Length - 1]}") && !p.Contains('!'))
                    {
                        result = true;
                        needed = true;
                    }
                    else if (fnum.Contains(p2[0]) && !p.Contains('!'))
                    {
                        result = true;
                    }
                    else if (fnum.Contains(p2[0]) && p.Contains('!'))
                    {
                        result = true;
                        conflict = true;
                    }
                    else if (!fnum.Contains(p2[0]) && p.Contains('!'))
                    {
                        conflict = true;
                    }
                    else if (p2[0] == "base" && label6.Text.Contains(p2[p2.Length - 1]))
                    {
                        result = true;
                        needed = true;
                    }
                    else if (p2[0] == "base" && !label6.Text.Contains(p2[p2.Length - 1]))
                    {
                        result = true;
                    }

                    if (result && !needed && !conflict)
                    {
                        if (p2[0] != "base")
                        {
                            string st = fnum.Substring(0, fnum.Length - 4);
                            st = st.Split('_')[st.Split('_').Length - 1];
                            label16.Text = $"*Имеется {st}";
                            label16.Visible = true;
                            label16.ForeColor = Color.Goldenrod;
                        }
                        else
                        {
                            label16.Text = $"*Имеется {label6.Text}";
                            label16.Visible = true;
                            label16.ForeColor = Color.Goldenrod;
                        }
                    }
                    else if (result && needed && !conflict)
                    {
                        if (p2[0] != "base")
                        {
                            string st = fnum.Substring(0, fnum.Length - 4);
                            st = st.Split('_')[st.Split('_').Length - 1];
                            label16.Text = $"Имеется {st}";
                            label16.Visible = true;
                            label16.ForeColor = Color.OliveDrab;
                        }
                        else
                        {
                            label16.Text = $"Имеется {label6.Text}";
                            label16.Visible = true;
                            label16.ForeColor = Color.OliveDrab;
                        }
                    }
                    else if (conflict && result)
                    {
                        label16.Text = "Конфликтует!";
                        label16.Visible = true;
                        label16.ForeColor = Color.OrangeRed;
                    }
                    else if (conflict && !result)
                    {
                        label16.Text = "Отсутствует";
                        label16.Visible = true;
                        label16.ForeColor = Color.OliveDrab;
                    }
                    else if (!result && !conflict)
                    {
                        label16.Text = "Отсутствует";
                        label16.Visible = true;
                        label16.ForeColor = Color.OrangeRed;
                    }
                }
            }
            catch (Exception ex)
            {
                label16.Text = null;
            }
        }
        #endregion

        #endregion


        #region Downloading module
        #region Downloading completed
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            button2.Enabled = true;
            button5.Enabled = true;
            progressBar1.Value = 0;
            if (downloadingType == "mods")
            {
                updateModsList();
                notifyIcon1.BalloonTipTitle = null;
                notifyIcon1.BalloonTipText = "Загрузка мода " + theFileName + " завершена!";
                notifyIcon1.ShowBalloonTip(5);
            }
            else if (downloadingType == "game")
            {
                Process.Start(fileName);
                downloadingType = null;
                button6.Enabled = true;
                button7.Enabled = true;
                notifyIcon1.BalloonTipTitle = null;
                notifyIcon1.BalloonTipText = "Игра загружена, запускаю установку...";
                notifyIcon1.ShowBalloonTip(5);
            }
        }
        #endregion
        #region Download progress changed
        int prevProgress = 0;
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            try
            {
                progressBar1.Maximum = 10000;
                progressBar1.Value = Convert.ToInt32((double)e.BytesReceived / (double)e.TotalBytesToReceive * 10000);
            }
            catch
            {
                //throw;
            }
            if (prevProgress < progressBar1.Value)
            {
                richTextBox1.Clear();
                richTextBox1.Text = richTextBox1.Text + "\nDownloading " + downloadingType + $" {progressBar1.Value/100}%:" + e.BytesReceived + "/" + e.TotalBytesToReceive + " bytes...";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                prevProgress = progressBar1.Value;
            }
        }
        #endregion
        #endregion


        #region Other link's
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/factoriosave");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://factorio.com");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://store.steampowered.com/app/427520/Factorio/");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.gog.com/game/factorio");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://factorio.com/buy");
        }

        private void linkLabel13_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.sensou.me");
        }
        #region Clean console LINK
        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox1.Clear();
        }
        #endregion
        #endregion


        #region On TOP check/change_button
        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.TopMost == true)
            {
                this.TopMost = false;
                linkLabel8.Text = "Не активно";
            }
            else if (this.TopMost == false)
            {
                this.TopMost = true;
                linkLabel8.Text = "Активно";
            }
        }
        #endregion


        #region Open dir.config button
        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("notepad", "dir.config"));
            richTextBox1.Text = richTextBox1.Text + "\n======================================================" +
                "\nВНИМАНИЕ! Необходимо обновить список модов после редактирования!";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        #endregion
        

        #region Overlay MODE
        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*--- Overlay MODE button ---*/
            if (overlay == true)
            {
                overlay = false;
                pictureBox1.Visible = false;
                linkLabel8.Enabled = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                progressBar1.Location = new Point(progressBar1.Location.X - 32, progressBar1.Location.Y);
                this.progressBar1.Size = new Size(progressBar1.Size.Width + 32, progressBar1.Size.Height);
                this.Size = new Size(623, 489);
                this.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                this.TransparencyKey = Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(1)))), ((int)(((byte)(1)))));
                linkLabel14.Text = "Overlay disabled";
                notifyIcon1.BalloonTipTitle = null;
                notifyIcon1.BalloonTipText = "Overlay disabled";
                notifyIcon1.ShowBalloonTip(5);
            }
            else if (overlay == false)
            {
                overlay = true;
                pictureBox1.Visible = true;
                linkLabel8.Enabled = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                progressBar1.Location = new Point(progressBar1.Location.X + 32, progressBar1.Location.Y);
                this.progressBar1.Size = new Size(progressBar1.Size.Width - 32, progressBar1.Size.Height);
                this.Size = new Size(250, 450);
                this.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                this.TransparencyKey = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                linkLabel14.Text = "Overlay enabled";
                notifyIcon1.BalloonTipTitle = null;
                notifyIcon1.BalloonTipText = "Overlay mode enabled\nДля скрытия и отображения программы используйте Shift + F3";
                notifyIcon1.ShowBalloonTip(5);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
                this.pictureBox1.Image = global::Factorio_Helper.Properties.Resources.construction_robot;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
                this.pictureBox1.Image = global::Factorio_Helper.Properties.Resources.personal_roboport_equipment;
            }
        }


        #endregion


        #region Folder path => console
        private void linkLabel1_MouseHover(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text + "\n" + "Текущий путь до папки с модами:" + "\n" + linkLabel1.Text;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void linkLabel2_MouseHover(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text + "\n" + "Текущий путь до папки с игрой:" + "\n" + linkLabel2.Text;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }
        #endregion
        

        #region Key Hook
        void KBDHook_KeyUp(Hooks.LLKHEventArgs e)
        {
            _pressedKeys.Remove(e.Keys);
        }

        void KBDHook_KeyDown(Hooks.LLKHEventArgs e)
        {
            if (!_pressedKeys.Contains(e.Keys))
                _pressedKeys.Add(e.Keys);
            string pressed = string.Join<Keys>(" + ", _pressedKeys);

            if (pressed == "F3 + LShiftKey" || pressed == "LShiftKey + F3" || pressed == "F3 + RShiftKey" || pressed == "RShiftKey + F3")
            {
                if (this.Visible == true) this.Visible = false;
                else this.Visible = true;
            }
        }
        #endregion


        #region Context menu click
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion


        #region Debug code
        //Debug code
        private void label8_Click(object sender, EventArgs e)
        {
            listView1.Clear();
        }
        #endregion

        

        //Предохранитель
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                richTextBox1.Text = richTextBox1.Text + "\n\n!!!ВНИМАНИЕ!!! Данная кнопка принудительно перезапустит игру!";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
            button8.Enabled = checkBox1.Checked;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Process.GetProcessesByName("factorio")[0].Kill();
            gameLauncher();
        }

        

        //Search button
        private void button13_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox5.Items.Clear();
            listBox6.Items.Clear();
            List<string> results = new List<string>();
            List<string> notresults = new List<string>();
            foreach (var item in MLI.mods)
            {
                try
                {
                    if (item != null)
                    {
                        foreach (var st in textBox1.Text.ToLower())
                        {
                            if (item.title.ToLower().Contains(st) && item.title.ToLower().Contains(textBox1.Text.ToLower()))
                            {
                                if (!results.Contains(item.title) && !notresults.Contains(item.title))
                                {
                                    results.Add(item.title);
                                }
                            }
                            else
                            {
                                if (results.Contains(item.title))
                                {
                                    results.Remove(item.title);
                                }
                                if (!notresults.Contains(item.title))
                                {
                                    notresults.Add(item.title);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = $"{richTextBox1.Text}\n\nSearch ERROR:\n{ex.Message}";
                }
            }
            foreach (var st in results)
            {
                listBox1.Items.Add(st);
            }
        }

        private void textBox3_MouseDown(object sender, MouseEventArgs e)
        {
            textBox3.Text = "";
            searchMessage();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
            searchMessage();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://vk.com/factoriosave");
        }

        //Help Button in FH auth
        private void button29_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed)
            {
                splitContainer1.Panel1Collapsed = true;
                button29.Text = "?";
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
                button29.Text = "✔";
            }
        }

        //License mode button
        private void button30_Click(object sender, EventArgs e)
        {
            switchMode(1, 2);
        }
        private void button31_Click(object sender, EventArgs e)
        {
            switchMode(1, 2);
        }

        private void startTool(object sender, EventArgs e)
        {
            Refresh();
            if (comboBox1.SelectedText == "UpdatedMods")
            {
                Form UpdatedMods = new UpdatedMods();
                UpdatedMods.Show();
            }
            else if (comboBox1.SelectedText == "XML")
            {
                Form XMLparse = new XMLparse();
                XMLparse.Show();
            }
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/settings");
        }

        private void button28_Click(object sender, EventArgs e)
        {
            if (File.Exists("uap.dll"))
            {
                string[] lines;
                lines = File.ReadAllLines("uap.dll");
                if (lines.Length <= 1)
                {
                    File.Delete("uap.dll");
                    MessageBox.Show("Внимание!\nНеобходимо повторить действие с введенными в поля данными!");
                    switchMode(0, 0);
                }
                else if (lines.Length >= 3)
                {
                    //authStatus = regUser(lines[0], lines[1], lines[2]);
                    authCheck(sender);
                }
            }
            else if (sender == null)
            {
                switchMode(0, 0);
            }
            else if (sender != null && textBox5.Text == "VK ID" && radioButton9.Checked == true)
            {
                MessageBox.Show("Не все поля заполнены");
                File.Delete("uap.dll");
                switchMode(0, 0);
            }
            else
            {
                if (radioButton8.Checked)
                {
                    //authStatus = regUser (textBox5.Text, textBox4.Text, maskedTextBox2.Text);
                }
                else
                {
                    //authStatus = auth (textBox5.Text, textBox4.Text, maskedTextBox2.Text);
                }
                
                authCheck(sender);
                if (checkBox5.Checked)
                {
                    if (File.Exists("uap.dll")) File.Delete("uap.dll");
                    File.WriteAllLines("uap.dll", new string[] { textBox5.Text, textBox4.Text, maskedTextBox2.Text });
                }
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            button32.Visible = false;
            comboBox1.Visible = true;
        }

        private void linkLabel11_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel15.Visible = true;
            linkLabel16.Visible = true;
        }

        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            File.Delete("uap.dll");
            if (textBox5.Text != "VK ID" && textBox5.Text != "")
            {
                foreach (char c in textBox5.Text)
                    if (!Char.IsNumber(c))
                    {
                        MessageBox.Show("ID должен содержать только цифры!");
                        goto breakIDCheck;
                    }
                File.WriteAllLines("uap.dll", new string[] { textBox5.Text, textBox4.Text, maskedTextBox2.Text });
                MessageBox.Show("Программа настроена и будет перезапущена...\nЕсли после перезапуска у вас запросит авторизацию - обратитесь в поддержку");
                Application.Restart();
            }
            else
            {
                MessageBox.Show("Введите авторизационные данные и нажмите эту кнопку снова (НЕ нажимая \"Вход\"\nVK ID может содержать только цифры!\n Номер страницы = ID");
            }
        breakIDCheck:;
        }

        private void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/factoriosave");
        }

        //Drag&Drop
        private void fhContainer_Panel1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && e.Effect == DragDropEffects.Move)
            {
                string[] objects = (string[])e.Data.GetData(DataFormats.FileDrop);
                // В objects хранятся пути к папкам и файлам
                textBox1.Text = null;
                for (int i = 0; i < objects.Length; i++)
                {
                    if (string.Equals(Path.GetExtension(objects[i]), ".exe", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // EXE
                        if (File.Exists("FH.new.exe")) File.Delete("FH.new.exe");
                        File.Move(objects[i], "FH.new.exe");
                        DialogResult msgResult = MessageBox.Show("Это действительно файл программы Factorio Helper Aplication?\n" + objects[i], "Update request", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3, MessageBoxOptions.DefaultDesktopOnly);
                        if (msgResult == DialogResult.Yes)
                        {
                            Process.Start("FH.new.exe", "-startupdate");
                            Application.ExitThread();
                        }
                        else if (msgResult == DialogResult.Cancel)
                        {
                            richTextBox1.Text = richTextBox1.Text + "Обновление отменено\n";
                            goto exitVoid;
                        }
                        else if (msgResult == DialogResult.No)
                        {

                        }
                    }
                    // ZIP?
                }
            }
        exitVoid:;
        }

        private void fhContainer_Panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))

                e.Effect = DragDropEffects.Move;
        }

        //Auth checked changed
        private void radioButton8_CheckedChanged(dynamic sender, EventArgs e)
        {
            if (sender.Checked)
            {
                button28.Text = "Вход";
                label31.Visible = false;
                textBox5.Visible = false;
            }
            else
            {
                button28.Text = "Регистрация";
                label31.Visible = true;
                textBox5.Visible = true;
            }
            button28.Enabled = true;
        }

        //Register checked changed
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {

        }

        //Pirate mod button in license auth page
        private void Button33_Click(object sender, EventArgs e)
        {
            switchMode(0, 0);
        }

        private void Label34_Click(object sender, EventArgs e)
        {

        }
    }
}
