using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Factorio_Helper
{
    public partial class CodeGen : Form
    {
        public CodeGen()
        {
            InitializeComponent();
        }

        bool isList = false;

        private void selectChanged(dynamic sender, EventArgs e)
        {
            if (sender.Text == "int" || sender.Text == "string")
            {
                listFillZone(0);
            }
            else
            {
                listFillZone(1);
            }
        }

        void listFillZone(int v)
        {
            if (v == 0)
            {
                label2.Visible = false;
                textBox2.Visible = false;
                label3.Visible = false;
                isList = false;
            }
            else
            {
                label2.Visible = true;
                textBox2.Visible = true;
                label3.Visible = true;
                isList = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string type()
            {
                string v = null;
                if (radioButton1.Checked)
                {
                    v = "int";
                }
                else if (radioButton2.Checked)
                v = "string";
                return v;
            }
            if (!isList)
            {
                richTextBox1.Text = richTextBox1.Text + "\n" + "public " + type() + " " + textBox1.Text + @" { get; set; }";
            }
            else richTextBox1.Text = richTextBox1.Text + "\n" + "public " + @"List<" + textBox2.Text + "LegalMods> " + textBox1.Text + @" { get; set; }";
        }
    }
}
