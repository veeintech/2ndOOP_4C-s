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
    public partial class UserNavigation : Form
    {
        public UserNavigation()
        {
            InitializeComponent();
        }

        private void UserNavigation_Load(object sender, EventArgs e)
        {
            UserControlDB DB = new UserControlDB();
            MainControlClass.ShowControl(DB, UserNav_Panel_shift);
            userNav_DashboardBtn.FillColor = Color.FromArgb(212, 167, 44);
        }

        private void userNav_DashboardBtn_Click(object sender, EventArgs e)
        {
            userNav_DashboardBtn.FillColor = Color.FromArgb(212, 167, 44);
            userNav_RoomsBtn.FillColor = Color.Transparent;
            UserControlDB db = new UserControlDB();
            MainControlClass.ShowControl(db, UserNav_Panel_shift);
        }

        private void userNav_RoomsBtn_Click(object sender, EventArgs e)
        {
            userNav_RoomsBtn.FillColor = Color.FromArgb(212, 167, 44);
            userNav_DashboardBtn.FillColor = Color.Transparent;
            UserControlRoom rm = new UserControlRoom();
            MainControlClass.ShowControl(rm, UserNav_Panel_shift);
        }

        private void dashboard_Panel_nav_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {

        }
    }
}
