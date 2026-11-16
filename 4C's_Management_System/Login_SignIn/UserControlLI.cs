
using System;
using System.Data;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControlLI : UserControl
    {
        public UserControlLI()
        {
            InitializeComponent();
        }

        public string branchid = string.Empty;

        // =========================================
        // SIGN UP BUTTON
        // =========================================

        private void LOGIN_signupbtn_Click(object sender, EventArgs e)
        {
            UserControlSU su = new UserControlSU();
            MainControlClass.ShowControl(su, this.Parent);
        }

        // =========================================
        // LOGIN BUTTON
        // =========================================

        private void LOGIN_loginBtn_Click(object sender, EventArgs e)
        {
            string username = LOGIN_usernameTxt.Text.Trim();
            string password = LOGIN_passwordtxt.Text;

            // =====================================
            // VALIDATE INPUT
            // =====================================

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // =====================================
            // SYSTEM ADMIN LOGIN
            // =====================================

            if (username == "System Admin" && password == "sysad123")
            {
                try
                {
                    Form? loginForm = FindForm();

                    Admin_Navigation admin = new Admin_Navigation();

                    if (loginForm != null)
                    {
                        admin.FormClosed += (s, args) =>
                        {
                            loginForm.Close();
                        };
                    }

                    admin.Show();

                    loginForm?.Hide();

                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Unable to open System Admin dashboard: " + ex.Message,
                        "Login Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
            }

            // =====================================
            // BRANCH ADMIN / USER LOGIN
            // =====================================

            try
            {
                const string loginQuery = @"
                    SELECT
                        Status,
                        BranchId,
                        StaffId
                    FROM UserTbl
                    WHERE UserName = @Username
                    AND Password = @Password";

                var parameters = new
                {
                    Username = username,
                    Password = password
                };

                var result = AppDB.Instance.TableData(
                    loginQuery,
                    parameters
                );

                // =================================
                // CHECK USER ACCOUNT
                // =================================

                if (result.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Invalid username or password.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // =================================
                // CHECK ACCOUNT STATUS
                // =================================

                string status = Convert.ToString(
                    result.Rows[0]["Status"]
                ) ?? "";

                if (status.Equals(
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Your account is inactive. Please contact the administrator.",
                        "Account Inactive",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // =================================
                // GET BRANCH ID
                // =================================

                string branchId = Convert.ToString(
                    result.Rows[0]["BranchId"]
                ) ?? "";

                string staffid = Convert.ToString(
                    result.Rows[0]["StaffId"]
                ) ?? "";

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    MessageBox.Show(
                        "No branch is assigned to this account.",
                        "Login Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                branchid = branchId;

                AccessBranch_ByForm.SetBranch(branchId);

                // =================================
                // OPEN USER NAVIGATION
                // =================================

                Form? loginForm = FindForm();

                UserNavigation dashboard = new UserNavigation();

                if (loginForm != null)
                {
                    dashboard.FormClosed += (s, args) =>
                    {
                        loginForm.Close();
                    };
                }

                dashboard.Show();

                loginForm?.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Login Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // SHOW PASSWORD
        // =========================================

        private void LOGIN_openeyeBtn_Click_1(object sender, EventArgs e)
        {
            LOGIN_passwordtxt.PasswordChar = '\0';
            LOGIN_closedeyeBtn.BringToFront();
        }

        // =========================================
        // HIDE PASSWORD
        // =========================================

        private void LOGIN_closedeyeBtn_Click(object sender, EventArgs e)
        {
            LOGIN_passwordtxt.PasswordChar = '*';
            LOGIN_openeyeBtn.BringToFront();
        }

        // =========================================
        // USERCONTROL LOAD
        // =========================================

        private void UserControlLI_Load(object sender, EventArgs e)
        {
            LOGIN_openeyeBtn.BringToFront();
        }

    }
}
