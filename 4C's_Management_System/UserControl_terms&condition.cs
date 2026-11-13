
using System;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_terms_condition : UserControl
    {
        public event EventHandler Accepted;

        public UserControl_terms_condition()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void agreeBTN_Click(object sender, EventArgs e)
        {

        }

        private void UserControl_terms_condition_Load(object sender, EventArgs e)
        {

        }

        private void agreeBTN_Click_1(object sender, EventArgs e)
        {
            Accepted?.Invoke(this, EventArgs.Empty);
            this.Hide();
        }
    }
}
