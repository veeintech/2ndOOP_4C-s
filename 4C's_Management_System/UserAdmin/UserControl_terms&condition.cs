
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

        private void agreeBTN_Click_1(object sender, EventArgs e)
        {
            Accepted?.Invoke(this, EventArgs.Empty);
            this.Hide();
        }
    }
}
