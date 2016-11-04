using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.IO;
using About;
using System.Threading;

namespace sys_passport_configurations
{
    public partial class f_sys_passport_configs : Form
    {
        RToolTip rt = new RToolTip();

        public f_sys_passport_configs()
        {
            InitializeComponent();
        }
        private static string _DbName = string.Empty;
        public static bool ok = true;

        #region events
        private void smth_chandged(object sender, EventArgs e)
        {
            show_msgbox = true;
        }

        private void c_indexvz_CheckedChanged(object sender, EventArgs e)
        {
            t_indexvz2.Enabled = c_indexvz.Checked;
            if (c_indexvz.Checked) rt.ShowToolTip(t_indexvz2, "Введіть індекс !");
            else
            {
                rt.HideTollTip();
                t_indexvz2.Text = string.Empty;
            }
            show_msgbox = true;
        }

        private void c_add_new_org_SelectedIndexChanged(object sender, EventArgs e)
        {
            t_add_new_org.Visible = c_add_new_org.SelectedIndex == 1 ? true : false;
            l_nomenkl.Visible = c_add_new_org.SelectedIndex == 1 ? true : false;
            if (c_add_new_org.SelectedIndex == 1)
            {
                t_add_new_org.Visible = true;
                l_nomenkl.Visible = true;
                if (c_add_new_org.Focus())
                    rt.ShowToolTip(t_add_new_org, "Введіть номенклатуру !");
            }
            else
            {
                t_add_new_org.Visible = false;
                l_nomenkl.Visible = false;
                rt.HideTollTip();
            }
            show_msgbox = true;
        }

        private void date_events(ComboBox cb, MaskedTextBox mtb)
        {
            mtb.Visible = cb.SelectedIndex == 3 ? true : false;
            if (cb.SelectedIndex == 3 && cb.Focus())
                rt.ShowToolTip(mtb, "Введіть дату " + (cb.Name.IndexOf("start") != -1 ? "початку" : "кінця") + " періоду !");
            else rt.HideTollTip();
            show_msgbox = true;
        }

        private void c_start_open_SelectedIndexChanged(object sender, EventArgs e)
        {
            date_events(c_start_open, t_start_open);
        }

        private void c_end_open_SelectedIndexChanged(object sender, EventArgs e)
        {
            date_events(c_end_open, t_end_open);
        }

        private void c_start_close_SelectedIndexChanged(object sender, EventArgs e)
        {
            date_events(c_start_close, t_start_close);
        }

        private void c_end_close_SelectedIndexChanged(object sender, EventArgs e)
        {
            date_events(c_end_close, t_end_close);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)18 && ModifierKeys == Keys.Control)
                InitAllConfigs(true);
            if (e.KeyChar == (char)19 && ModifierKeys == Keys.Control)
            {
                SaveAllConfigs();
                show_msgbox = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }

        private void s_Click(object sender, EventArgs e)
        {
            show_msgbox = false;
            try
            {
                SaveAllConfigs();
                InitAllConfigs(false);
                MessageBox.Show("Зміни успішно збережено", "АС 'ОЛБП'", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { MessageBox.Show("Під час збереження відбулась непередбачувана помилка !", "АС 'ОЛБП'", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void b_Ok_Click(object sender, EventArgs e)
        {
            show_msgbox = false;
            SaveAllConfigs();
            this.Close();
        }

        private void t_add_new_org_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar != 44) && (e.KeyChar != 46) && !(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
            if (e.KeyChar == 44)
                e.KeyChar = (char)46;
        }

        private bool show_msgbox;
        private void f_sys_passport_configs_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ok)
            {
                show_msgbox = !btn_cancel.Enabled ? true : show_msgbox;

                if (show_msgbox)
                    if (MessageBox.Show("Зберегти налаштування ?", "АС 'ОЛБП'", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        System.Windows.Forms.DialogResult.Yes) SaveAllConfigs();
            }
        }

        private void t_add_new_org_Enter(object sender, EventArgs e)
        {
            t_add_new_org.SelectionStart = 0;
            t_add_new_org.SelectionLength = t_add_new_org.Text.Length;
            rt.HideTollTip();
        }

        private void Tp_Dispose(object sender, EventArgs e)
        {
            rt.HideTollTip();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Update_sys_passport.Check_sys_passport(DbName);
                MessageBox.Show("Готово !!!", "АС 'ОЛБП'", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { MessageBox.Show("Помилка", "АС 'ОЛБП'"); }
        }

        private void t_cpz_code_MouseHover(object sender, EventArgs e)
        {
            rt.ShowToolTip_ForAll(t_cpz_code, @"Код ЦПЗ для роботи на пряму з постачальниками послуг.");
        }

        private void t_cpz_code_MouseLeave(object sender, EventArgs e)
        {
            rt.HideTollTip();
        }
        #endregion

        #region initialisations
        public void InitAllConfigs(bool ShowError)
        {
            try
            {
                IfFieldsExist();
                t_indexvz.Text = GetSingleConfig((int)sys_passport.name_vz);
                t_main_index.Text = GetSingleConfig((int)sys_passport.index_vz);
                InitIndex2();
                c_on_off.SelectedIndex = GetSingleConfig((int)sys_passport.on_off) == "0" ? 0 : 1;
                c_bill.SelectedIndex = Convert.ToInt32(GetSingleConfig((int)sys_passport.bill));
                InitFasAndPhoneSearch();
                InitAddNeOrg(GetSingleConfig((int)sys_passport.add_new_org));
                InitRedusing(GetSingleConfig((int)sys_passport.reducing));
                InitDate(GetSingleConfig((int)sys_passport.period));
                InitCpzCode();
                btn_cancel.Enabled = true;
            }
            catch
            {
                if (ok)
                {
                    MessageBox.Show("У вас не коректно налаштовано паспорт системи\n" +
                        "Для продовження роботи налаштуйте усі пункти та натисніть кнопку 'ОК' або 'Застосувати'",
                        "АС 'ОЛБП'", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btn_cancel.Enabled = false;
                    Update_sys_passport.Check_sys_passport(DbName);
                }
                else this.Close();
            }
        }

        private void IfFieldsExist()
        {
            string[] numbers = { "62", "59", "61", "60", "57", "65", "67", "95" };
            OdbcConnection conn = new OdbcConnection("Driver=Firebird/InterBase(r) driver; Uid=SYSDBA; Pwd=masterkey; DbName=" + DbName);
            using (conn)
            {
                conn.Open();
                foreach (string number in numbers)
                {
                    DataTable dt = new DataTable();
                    string sql = "select * from sys_passport sp where sp.code = " + number;
                    OdbcDataAdapter DA = new OdbcDataAdapter(sql, conn);
                    DA.Fill(dt);
                    if (dt == null || dt.Rows.Count <= 0) throw new Exception();
                }
            }
        }

        private void InitIndex2()
        {
            string vz2 = GetSingleConfig((int)sys_passport.index_vz_2);

            if (vz2 == "0")
                t_indexvz2.Enabled = false;
            else
            {
                t_indexvz2.Text = vz2;
                t_indexvz2.Enabled = true;
                c_indexvz.Checked = true;
            }
        }

        private void InitFasAndPhoneSearch()
        {
            string sVal = GetSingleConfig((int)sys_passport.fas_and_phone_search);
            char[] cVal = sVal.ToCharArray();
            c_fast_search.SelectedIndex = Convert.ToInt16(cVal[0].ToString());
            cb_phone_search.SelectedIndex = cVal.Length > 1 ? Convert.ToInt16(cVal[1].ToString()) : 0;
            cb_phone_count.SelectedIndex = cVal.Length > 2 ? Convert.ToInt16(cVal[2].ToString()) : 0;
        }

        private void InitDate(string val)
        {
            string[] tmp_val = val.Split(';');
            StartDate(tmp_val[0], true);
            StartDate(tmp_val[2], false);
            EndDate(tmp_val[1], true);
            EndDate(tmp_val[3], false);
        }

        private void StartDate(string val, bool isOpen)
        {
            string res = string.Empty;
            DateTime dateTime = new DateTime();
            if (DateTime.TryParse(val, out dateTime))
                res = dateTime.ToString("dd.MM.yyyy");
            else
            {
                if (val == "-1") res = "0";
                if (val == "0") res = "1";
                if (val == "1") res = "2";
            }
            StartDates_initialisation(isOpen, res);
        }

        private void StartDates_initialisation(bool isOpen, string res)
        {
            if (isOpen)
            {
                if (res.Length > 2)
                {
                    c_start_open.SelectedIndex = 3;
                    t_start_open.Text = res;
                    t_start_open.Visible = true;
                }
                else
                {
                    t_start_open.Visible = false;
                    c_start_open.SelectedIndex = Convert.ToInt32(res);
                }
            }
            else
            {
                if (res.Length > 2)
                {
                    c_start_close.SelectedIndex = 3;
                    t_start_close.Text = res;
                    t_start_close.Visible = true;
                }
                else
                {
                    t_start_close.Visible = false;
                    c_start_close.SelectedIndex = Convert.ToInt32(res);
                }
            }
        }
        private void EndDates_initialisation(bool isOpen, string res)
        {
            if (isOpen)
            {
                if (res.Length > 2)
                {
                    c_end_open.SelectedIndex = 3;
                    t_end_open.Text = res;
                    t_end_open.Visible = true;
                }
                else
                {
                    t_end_open.Visible = false;
                    c_end_open.SelectedIndex = Convert.ToInt32(res);
                }
            }
            else
            {
                if (res.Length > 2)
                {
                    c_end_close.SelectedIndex = 3;
                    t_end_close.Text = res;
                    t_end_close.Visible = true;
                }
                else
                {
                    t_end_close.Visible = false;
                    c_end_close.SelectedIndex = Convert.ToInt32(res);
                }
            }
        }

        private void EndDate(string val, bool isOpen)
        {
            string res = string.Empty;
            DateTime dateTime = new DateTime();
            if (DateTime.TryParse(val, out dateTime))
                res = dateTime.ToString("dd.MM.yyyy");
            else
            {
                if (val == "-1") res = "0";
                if (val == "0") res = "1";
                if (val == "1") res = "2";
            }
            EndDates_initialisation(isOpen, res);
        }

        private void InitRedusing(string val)
        {
            c_redusing_open_fields.SelectedIndex = Convert.ToInt32(val.Substring(0, 1));
            c_redusing_type.SelectedIndex = Convert.ToInt32(val.Substring(1, 1));
        }

        private void InitAddNeOrg(string val)
        {
            if (val == "0")
            {
                c_add_new_org.SelectedIndex = 0;
                t_add_new_org.Visible = false;
                l_nomenkl.Visible = false;
            }
            else
            {
                c_add_new_org.SelectedIndex = 1;
                t_add_new_org.Visible = true;
                t_add_new_org.Text = val;
                l_nomenkl.Visible = true;
            }
        }

        private void InitCpzCode()
        {
            string val = GetSingleConfig((int)sys_passport.cpz_code);
            t_cpz_code.Text = string.IsNullOrEmpty(val) ? "" : val;
        }

        private string GetSingleConfig(int id)
        {
            OdbcConnection conn = new OdbcConnection("Driver=Firebird/InterBase(r) driver; Uid=SYSDBA; Pwd=masterkey; DbName=" + DbName);
            DataTable dt = new DataTable();
            using (conn)
            {
                conn.Open();
                string sql = "select * from sys_passport sp where sp.code = " + id;
                OdbcDataAdapter DA = new OdbcDataAdapter(sql, conn);
                DA.Fill(dt);
            }
            return dt.Rows[0]["value_param"].ToString();
        }
        #endregion

        #region save cfg
        public void SaveAllConfigs()
        {
            Save((int)sys_passport.index_vz, t_main_index.Text);
            Save((int)sys_passport.on_off, c_on_off.SelectedIndex);
            SaveAdditionalIndex();
            Save((int)sys_passport.bill, c_bill.SelectedIndex);
            Save((int)sys_passport.fas_and_phone_search, c_fast_search.SelectedIndex.ToString() + cb_phone_search.SelectedIndex + cb_phone_count.SelectedIndex);
            Save((int)sys_passport.add_new_org, c_add_new_org.SelectedIndex == 0 ? "0" :
                string.IsNullOrEmpty(t_add_new_org.Text.Replace(',', '.')) ? "0" : t_add_new_org.Text.Replace(',', '.'));
            Save((int)sys_passport.reducing, c_redusing_open_fields.SelectedIndex.ToString() + c_redusing_type.SelectedIndex);
            Save((int)sys_passport.period, DateConfig());
            Save((int)sys_passport.cpz_code, t_cpz_code.Text, true);
        }

        private void SaveAdditionalIndex()
        {
            if (!c_indexvz.Checked) Save((int)sys_passport.index_vz_2, 0);
            else Save((int)sys_passport.index_vz_2, t_indexvz2.Text);
        }

        private string DateConfig()
        {
            string res = string.Empty;
            res += c_start_open.SelectedIndex == 3 ? CheckDate(t_start_open.Text) + ";" : (c_start_open.SelectedIndex - 1).ToString() + ";";
            res += c_end_open.SelectedIndex == 3 ? CheckDate(t_end_open.Text) + ";" : (c_end_open.SelectedIndex - 1).ToString() + ";";
            res += c_start_close.SelectedIndex == 3 ? CheckDate(t_start_close.Text) + ";" : (c_start_close.SelectedIndex - 1).ToString() + ";";
            res += c_end_close.SelectedIndex == 3 ? CheckDate(t_end_close.Text) : (c_end_close.SelectedIndex - 1).ToString();
            return res;
        }

        private string CheckDate(string val)
        {
            try
            {
                return DateTime.Parse(val).ToString("dd.MM.yyyy");
            }
            catch { return "0"; }
        }

        private void Save(int id, object val, bool CanSaveEmpty = false)
        {
            if (val == null) return;
            if (string.IsNullOrEmpty(val.ToString()) && !CanSaveEmpty) return;
            OdbcConnection conn = new OdbcConnection("Driver=Firebird/InterBase(r) driver; Uid=SYSDBA; Pwd=masterkey; DbName=" + DbName);
            string save_sql = string.Format("update sys_passport sp set sp.value_param = '{0}' where sp.code = {1}", val, id);
            try
            {
                using (conn)
                {
                    conn.Open();
                    OdbcCommand cmd = new OdbcCommand(save_sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
        #endregion

        private enum sys_passport
        {
            index_vz = 1,
            name_vz = 2,
            index_vz_2 = 62,
            bill = 59,
            reducing = 61,
            on_off = 60,
            period = 57,
            add_new_org = 65,
            cpz_code = 67,
            fas_and_phone_search = 95
        }

        private void f_sys_passport_configs_Load(object sender, EventArgs e)
        {
            c_add_new_org.SelectedIndex = 0;
            c_bill.SelectedIndex = 0;
            c_end_close.SelectedIndex = 1;
            c_end_open.SelectedIndex = 1;
            c_fast_search.SelectedIndex = 0;
            c_on_off.SelectedIndex = 0;
            c_redusing_open_fields.SelectedIndex = 0;
            c_redusing_type.SelectedIndex = 0;
            c_start_close.SelectedIndex = 1;
            c_start_open.SelectedIndex = 1;

            if (!File.Exists(DbName.Substring(10)))
            {
                SelectFile sf = new SelectFile();
                sf.ShowDialog();
            }

            this.Text = this.Text + " (" + DbName.Substring(10) + ")";

            InitAllConfigs(true);
            show_msgbox = false;
        }

        public static string DbName
        {
            get
            {
                if (string.IsNullOrEmpty(_DbName))
                {
                    string m = DateTime.Now.Month.ToString();
                    string y = DateTime.Now.Year.ToString();

                    return "localhost:" + Configurator.GetCfg(Configurator.cfgType.BaseDir) + "REG" + y + (m.Length == 1 ? "0" + m : m) + ".GDB";
                }
                return "localhost:" + _DbName;
            }
            set { _DbName = value; }
        }

        private void налаштуванняToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configurations cfg = new Configurations();
            cfg.ShowDialog();
        }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
    }
}
