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
    public partial class UserControlDB : UserControl
    {
        public UserControlDB()
        {
            InitializeComponent();
        }

        private void UCDash_NewReservationBtn_Click(object sender, EventArgs e)
        {
            UserNavigation? nav = FindForm() as UserNavigation;
            Control? parent = Parent;

            if (parent == null)
            {
                return;
            }

            UserControlRoomReserv rs = new UserControlRoomReserv();
            MainControlClass.ShowControl(rs, parent);

            nav?.reservationBTN();
        }

        private void UserControlDB_Load(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel26_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gunaChart1_Load(object sender, EventArgs e)
        {

        }
    }
}
