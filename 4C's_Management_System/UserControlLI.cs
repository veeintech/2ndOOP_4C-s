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
            UserControlSU su = new UserControlSU();
            MainControlClass.ShowControl(su, this.Parent);
        }
        private void LOGIN_loginBtn_Click(object sender, EventArgs e)
        {
            string username = LOGIN_usernameTxt.Text.Trim();
            string password = LOGIN_passwordtxt.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query =
                    "SELECT 1 FROM UserTbl WHERE UserName = @Username AND Password = @Password";

                var result = AppDB.Instance.Scalar(query, new
                {
                    Username = username,
                    Password = password
                });

                if ((result is null || result is DBNull))
                {
                    MessageBox.Show(
                        "Invalid username or password.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                if (!LOGIN_ternconCB.Checked)
                {
                    MessageBox.Show(
                        "Please accept the terms and conditions to proceed.",
                        "Terms and Conditions",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                if(result is not null && result != DBNull.Value && LOGIN_ternconCB.Checked)
                {
                    Form? loginForm = FindForm();
                    var dashboard = new UserNavigation();

                    if (loginForm is not null)
                        dashboard.FormClosed += (_, _) => loginForm.Close();

                    dashboard.Show();
                    loginForm?.Hide();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LOGIN_openeyeBtn_Click_1(object sender, EventArgs e)
        {
            LOGIN_passwordtxt.PasswordChar = '\0';
            LOGIN_closedeyeBtn.BringToFront();
        }

        private void LOGIN_closedeyeBtn_Click(object sender, EventArgs e)
        {
            LOGIN_passwordtxt.PasswordChar = '*';
            LOGIN_openeyeBtn.BringToFront();
        }
        private void UserControlLI_Load(object sender, EventArgs e)
        {

        }
    }
}
