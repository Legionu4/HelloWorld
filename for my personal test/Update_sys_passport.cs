using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;

namespace sys_passport_configurations
{
    public static class Update_sys_passport
    {
        public enum sys_passport
        {
            sys57 = 57,
            sys59 = 59,
            sys60 = 60,
            sys61 = 61,
            sys62 = 62,
            sys65 = 65,
            sys67 = 67,
            sys95 = 95
        }
        public static void Check_sys_passport(string path_to_reg)
        {
            #region Dictionaries
            Dictionary<sys_passport, string> sys_passport_values = new Dictionary<sys_passport, string>();
            sys_passport_values.Add(sys_passport.sys57, "0;0;0;0");
            sys_passport_values.Add(sys_passport.sys59, "0");
            sys_passport_values.Add(sys_passport.sys60, "0");
            sys_passport_values.Add(sys_passport.sys61, "20");
            sys_passport_values.Add(sys_passport.sys62, "0");
            sys_passport_values.Add(sys_passport.sys65, "0");
            sys_passport_values.Add(sys_passport.sys67, "0");
            sys_passport_values.Add(sys_passport.sys95, "00");

            Dictionary<sys_passport, string> sys_passport_names = new Dictionary<sys_passport, string>();
            sys_passport_names.Add(sys_passport.sys57, "ОЛБП  Значення по замовчуванні для періоду");
            sys_passport_names.Add(sys_passport.sys59, "ОЛБП  Налаштування друку чеку");
            sys_passport_names.Add(sys_passport.sys60, "ОЛБП  Режим роботи");
            sys_passport_names.Add(sys_passport.sys61, "ОЛБП  Налаштування зведення");
            sys_passport_names.Add(sys_passport.sys62, "ОЛБП  Додатковий індекс");
            sys_passport_names.Add(sys_passport.sys65, "ОЛБП  Додавання нових організацій");
            sys_passport_names.Add(sys_passport.sys67, "Код ЦПЗ (Код ПФМ)");
            sys_passport_names.Add(sys_passport.sys95, "ОЛБП  Швидкий пошук/Пошук по телефону");
            #endregion

            using (OdbcConnection conn = new OdbcConnection("Driver=Firebird/InterBase(r) driver; Uid=SYSDBA; Pwd=masterkey; DbName=" + path_to_reg))
            {
                conn.Open();
                OdbcCommand sql = new OdbcCommand("", conn);

                for (sys_passport current = sys_passport.sys57; current <= sys_passport.sys95; current++)
                    if (sys_passport_names.ContainsKey(current))
                    {
                        TryToExecuteSql(sql, "insert into sys_passport (code, name, value_param) values (" + (int)current + ", '"
                            + sys_passport_names[current] + "', '" + sys_passport_values[current] + "')");

                        TryToExecuteSql(sql, "update sys_passport set name = '" + sys_passport_names[current] + "' where code = " + (int)current);
                    }
            }
        }

        private static void TryToExecuteSql(OdbcCommand command, string sql)
        {
            command.CommandText = sql;
            try { command.ExecuteNonQuery(); }
            catch { }
        }
    }
}
