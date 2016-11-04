using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace sys_passport_configurations
{
    public partial class SelectFile : Form
    {
        public SelectFile()
        {
            InitializeComponent();
        }

        private void InitCbData()
        {
            var dir = new DirectoryInfo(DirPath);
            List<string> files_full = new List<string>();
            foreach (FileInfo file in dir.GetFiles("REG*.GDB"))
                files_full.Add(file.FullName);

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2] { new DataColumn("full_name", typeof(string)),
                            new DataColumn("name",typeof(string)) });
            for (int i = 0; i < files_full.Count; i++)
            {
                string[] val = files_full[i].ToString().Split('\\');
                dt.Rows.Add(files_full[i].ToString(), val[val.Length - 1]);
            }

            cb_files.DataSource = dt;
            cb_files.ValueMember = "full_name";
            cb_files.DisplayMember = "name";
        }

        private string DirPath
        {
            get { return Configurator.GetCfg(Configurator.cfgType.BaseDir); }
        }

        private void SelectFile_Load(object sender, EventArgs e)
        {
            InitCbData();
        }

        private void b_Ok_Click(object sender, EventArgs e)
        {
            f_sys_passport_configs.DbName = cb_files.SelectedValue.ToString();
            this.Close();
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            f_sys_passport_configs.ok = false;
            this.Close();
        }
    }
}
