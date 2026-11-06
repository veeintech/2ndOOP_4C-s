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
            RRes_RoomTypeCB.Items.Clear();

            RRes_RoomTypeCB.Items.Add("All Status");
            RRes_RoomTypeCB.Items.Add("Checked In");
            RRes_RoomTypeCB.Items.Add("Confirmed");
            RRes_RoomTypeCB.Items.Add("Pending");
            RRes_RoomTypeCB.Items.Add("Cancelled");

            RRes_RoomTypeCB.SelectedIndex = 0;
        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
