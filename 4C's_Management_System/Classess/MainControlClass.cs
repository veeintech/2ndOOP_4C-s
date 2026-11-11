using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    internal class MainControlClass
    {
        public static void ShowControl(Control control, Control Content)
        {
            Content.Controls.Clear();

            control.Dock = DockStyle.Fill;
            control.BringToFront();
            control.Focus();

            Content.Controls.Add(control);
        }
    }
}
