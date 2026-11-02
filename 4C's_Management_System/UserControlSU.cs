using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlSU : UserControl
    {
        public UserControlSU()
        {
            InitializeComponent();
        }

        private void LOGIN_lblsignupclk(object sender, EventArgs e)
        {
            UserControlLI li = new UserControlLI();
            MainControlClass.ShowControl(li, this.Parent);
        }
    }
}
