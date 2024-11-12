using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Factorio_Helper
{
    public partial class XMLparse : Form
    {
        public XMLparse()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            XElement data = XElement.Parse(richTextBox1.Text);
            var el = data.Elements();
            foreach (dynamic element in el)
            {
                richTextBox2.Text = richTextBox2.Text + "\n" + element.FirstNode.Value;
            }
        }
    }
}
