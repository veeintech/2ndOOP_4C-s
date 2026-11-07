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
    public partial class UserControlRoomReserv : UserControl
    {
        public UserControlRoomReserv()
        {
            InitializeComponent();
        }
        private void UserControlRoomReserv_Load(object sender, EventArgs e)
        {
            RRes_AccomodationCB.Items.Clear();

            RRes_AccomodationCB.Items.Add("All Status");
            RRes_AccomodationCB.Items.Add("Checked In");
            RRes_AccomodationCB.Items.Add("Confirmed");
            RRes_AccomodationCB.Items.Add("Pending");
            RRes_AccomodationCB.Items.Add("Cancelled");

            RRes_AccomodationCB.SelectedIndex = 0;
        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
