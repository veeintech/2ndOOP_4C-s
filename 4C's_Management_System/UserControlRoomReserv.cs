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
            guna2ComboBox1.Items.Clear();

            guna2ComboBox1.Items.Add("All Status");
            guna2ComboBox1.Items.Add("Checked In");
            guna2ComboBox1.Items.Add("Confirmed");
            guna2ComboBox1.Items.Add("Pending");
            guna2ComboBox1.Items.Add("Cancelled");

            guna2ComboBox1.SelectedIndex = 0;
        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
