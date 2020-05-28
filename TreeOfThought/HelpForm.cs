using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree_Of_Thought
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.ReadOnly = false;
                string currentDir = Environment.CurrentDirectory;
                DirectoryInfo directory = new DirectoryInfo(currentDir);
                string helpFile = directory.FullName+  "\\Help_file.rtf";
                richTextBox1.LoadFile(helpFile, RichTextBoxStreamType.RichText);
                richTextBox1.ReadOnly = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
