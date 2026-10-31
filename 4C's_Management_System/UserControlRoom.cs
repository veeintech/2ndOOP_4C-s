using Guna.UI2.WinForms;
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
    public partial class UserControlRoom : UserControl
    {
        public UserControlRoom()
        {
            InitializeComponent();
        }

        private void guna2CustomGradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void UserControlRoom_Load(object sender, EventArgs e)
        {
            guna2ComboBox1.Items.Clear();

            guna2ComboBox1.Items.Add("All Status");
            guna2ComboBox1.Items.Add("Available");
            guna2ComboBox1.Items.Add("Occupied");
            guna2ComboBox1.Items.Add("Maintenance");
            guna2ComboBox1.Items.Add("Reserved");

            guna2ComboBox1.SelectedIndex = 0;

        }
    }
}
