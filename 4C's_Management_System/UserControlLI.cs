using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;
using Microsoft.Data.SqlClient;

namespace _4C_s_Management_System
{
    public partial class UserControlLI : UserControl
    {
        
        public UserControlLI()
        {
            InitializeComponent();
        }

        public string branchid = string.Empty;

        private void LOGIN_signupbtn_Click(object sender, EventArgs e)
        {
            UserControlSU su = new UserControlSU();
            MainControlClass.ShowControl(su, this.Parent);
        }
        private void LOGIN_loginBtn_Click(object sender, EventArgs e)
        {
            string username = LOGIN_usernameTxt.Text.Trim();
            string password = LOGIN_passwordtxt.Text;

            if(username == "System Admin" && password == "sysad123")
            {
                this.Hide();
                Admin_Navigation ad = new Admin_Navigation();
                ad.Show();
            }

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
                const string statusCheckQuery =
                    "SELECT Status FROM UserTbl WHERE UserName = @Username AND Password = @Password";
                const string loginQuery =
                    "SELECT 1 FROM UserTbl WHERE UserName = @Username AND Password = @Password";
                const string branchQuery =
                    "SELECT BranchId FROM UserTbl WHERE UserName = @Username AND Password = @Password";

                var parameters = new
                {
                    Username = username,
                    Password = password
                };

                var result = AppDB.Instance.Scalar(loginQuery, parameters);

                if (result is null || result == DBNull.Value)
                {
                    MessageBox.Show(
                        "Invalid username or password.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                var statusResult = AppDB.Instance.Scalar(statusCheckQuery, parameters);

                if (statusResult is not null &&
                    statusResult != DBNull.Value &&
                    statusResult.ToString() == "Inactive")
                {
                    MessageBox.Show(
                        "Your account is inactive. Please contact the administrator.",
                        "Account Inactive",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var branchResult = AppDB.Instance.Scalar(branchQuery, parameters);
                string branchId = branchResult?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    MessageBox.Show(
                        "No branch is assigned to this account.",
                        "Login Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                branchid = branchId;
                AccessBranch_ByForm.SetBranch(branchId);

                Form? loginForm = FindForm();
                var dashboard = new UserNavigation();

                if (loginForm is not null)
                {
                    dashboard.FormClosed += (_, _) => loginForm.Close();
                }

                dashboard.Show();
                loginForm?.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
