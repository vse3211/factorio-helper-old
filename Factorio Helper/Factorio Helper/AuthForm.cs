using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using InputBox;

namespace Factorio_Helper
{
    public partial class AuthForm : Form
    {
        public bool authed = false;
        public long? uid = null;

        private Point mouseOffset;
        private bool isMouseDown = false;

        public AuthForm()
        {
            InitializeComponent();
            FileCheck();
        }

        void FileCheck()
        {
            if (File.Exists("uap.dll"))
            {
                //Read settings & check
                string securityFileCheck = File.ReadAllText("uap.dll");
                if (securityFileCheck == "l1[0xx]")
                {
                    this.Close();
                }
                else /*if (securityFileCheck == "l1[0x1]")*/
                {
                    label2.Text = "Необходима повторная авторизация т.к. в прошлый раз вы не состояли в нашей группе.";
                }
            }
            else
            {
                label2.Text = "Необходимо пройти авторизацию";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var api = new VkApi();
                if (authed == false)
                {
                    goto VKAuth;
                }
                else
                {
                    goto CheckGroup;
                }

            VKAuth:
                api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 6765724,
                    Login = textBox1.Text,
                    Password = maskedTextBox1.Text,
                    Settings = Settings.Groups,
                    TwoFactorAuthorization = () =>
                    {
                        string verCode = null;
                        if (api.Token == null)
                        {
                            InputBox.InputBox inputBox = new InputBox.InputBox();
                            verCode = inputBox.getString();
                        }
                        return verCode;
                    }
                });
                uid = api.UserId;
                /*if (File.Exists("uap.dll"))
                {
                    File.Delete("uap.dll");
                    File.WriteAllText("uap.dll", "l1[0x1]");
                }
                else
                {
                    File.WriteAllText("uap.dll", "l1[0x1]");
                }*/
                authed = true;
            
            
                CheckGroup:
                string groupId = "127017434";
                var accountInfo = api.Account.GetProfileInfo();
                Console.WriteLine(api.UserId);
                var inGroupCheck = api.Groups.IsMember(groupId, uid, null, null).Select(x => x.Member).FirstOrDefault();

                if (inGroupCheck == true)
                {
                    if (File.Exists("uap.dll"))
                    {
                        File.Delete("uap.dll");
                        File.WriteAllText("uap.dll", "l1[0xx]");
                    }
                    else
                    {
                        File.WriteAllText("uap.dll", "l1[0xx]");
                    }
                    this.Close();
                }
                else
                {
                    label2.Text = "Подпишитесь и авторизуйтесь повторно!";
                }
            }
            catch
            {
                if (authed != true)
                    label2.Text = "Не верный логин/пароль";
                else label2.Text = "Вы не подписаны на группу!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
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
            }
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }
    }
}
