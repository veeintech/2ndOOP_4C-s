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
    public partial class UserControlLI : UserControl
    {
        public UserControlLI()
        {
            InitializeComponent();
        }

        private void LOGIN_signupbtn_Click(object sender, EventArgs e)
        {
            ((LogInForm)this.FindForm()).ShowSignup();
        }
    }


}
