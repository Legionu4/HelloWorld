using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Xml;
using System.IO;

namespace sys_passport_configurations
{
    public partial class Configurations : Form
    {
        public Configurations()
        {
            InitializeComponent();
        }

        private void Configurations_Load(object sender, EventArgs e)
        {
            l_StartDir.Text = Configurator.GetCfg(Configurator.cfgType.BaseDir);
        }

        private void b_SelectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();

            if (!string.IsNullOrEmpty(fd.SelectedPath.ToString()))
            {
                var dir = new DirectoryInfo(fd.SelectedPath.ToString());
                List<string> files_full = new List<string>();
                foreach (FileInfo file in dir.GetFiles("REG*.GDB"))
                    files_full.Add(file.FullName);

                if (files_full.Count <= 0)
                    MessageBox.Show("В обраному каталозі відсутні файли бази даних !", "АС 'ОЛБП'", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    l_StartDir.Text = fd.SelectedPath.ToString() + "\\";
            }
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void b_save_Click(object sender, EventArgs e)
        {
            Configurator.updateCfg(Configurator.cfgType.BaseDir, l_StartDir.Text);
            f_sys_passport_configs.ok = false;
            Application.Restart();
            this.Close();
        }
    }
}
