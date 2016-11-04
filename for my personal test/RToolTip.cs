using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace sys_passport_configurations
{
    class RToolTip
    {
        public ToolTip tp;

        public void ShowToolTip(Control control, string text)
        {
            if (tp != null) tp.Dispose();
            tp = new ToolTip();
            tp.UseFading = true;
            tp.UseAnimation = true;
            tp.IsBalloon = true;
            tp.ShowAlways = true;
            if (string.IsNullOrEmpty(control.Text.Replace(".", "").Trim()))
                tp.Show(text, control, 0, -control.Height - 11, 5000);
        }

        public void ShowToolTip_ForAll(Control control, string text)
        {
            if (tp != null) tp.Dispose();
            tp = new ToolTip();
            tp.UseFading = true;
            tp.UseAnimation = true;
            tp.IsBalloon = true;
            tp.ShowAlways = true;
            tp.Show(text, control, 0, -control.Height - 11, 5000);
        }

        public void HideTollTip()
        {
            if (tp != null)
                tp.Dispose();
        }
    }
}
