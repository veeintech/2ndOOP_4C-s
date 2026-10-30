
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

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void LogInForm_Load(object sender, EventArgs e)
        {

        }

        public void ShowLogin()
        {
            userControlli1.Show();
            userControlli1.BringToFront();
            userControlsu1.Hide();
        }

        public void ShowSignup()
        {
            userControlsu1.Show();
            userControlsu1.BringToFront();
            userControlli1.Hide();
        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void userControlsu1_Load(object sender, EventArgs e)
        {

        }

        private void LogInForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
