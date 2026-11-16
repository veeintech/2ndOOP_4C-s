
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
    public partial class LogInForm : Form
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        public void logIn()
        {
            MainControlClass.ShowControl(new UserControlLI(), LogInPanel_shift);
        }
        public void SignUp()
        {
            MainControlClass.ShowControl(new UserControlSU(), LogInPanel_shift);
        }
    }
}
