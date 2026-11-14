
using System;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControlSU : UserControl
    {
        public UserControlSU()
        {
            InitializeComponent();

            SU_trmconCB.Checked = false;
            userControl_terms_condition2.Visible = false;

            userControl_terms_condition2.Accepted +=
                userControl_terms_condition2_Accepted;
        }

        private void LOGIN_lblsignupclk(object sender, EventArgs e)
        {
            UserControlLI li = new UserControlLI();
            MainControlClass.ShowControl(li, this.Parent);
        }

        private void SU_closedeyeBtn_Click_1(object sender, EventArgs e)
        {
            SU_passwordtxt.PasswordChar = '*';
            SU_conpasstxt.PasswordChar = '*';
            SU_openedeyeBtn.BringToFront();
        }

        private void SU_openedeyeBtn_Click(object sender, EventArgs e)
        {
            SU_passwordtxt.PasswordChar = '\0';
            SU_conpasstxt.PasswordChar = '\0';
            SU_closedeyeBtn.BringToFront();
        }

        // CREATE ACCOUNT
        private void SU_createaccBtn_Click_1(object sender, EventArgs e)
        {
            string branchid = SU_branchidtxt.Text.Trim();
            string password = SU_passwordtxt.Text;
            string username = SU_usernametxt.Text.Trim();
            string confirmPassword = SU_conpasstxt.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(branchid))
            {
                MessageBox.Show("Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!SU_trmconCB.Checked)
            {
                MessageBox.Show("Please accept the terms and conditions to proceed.",
                    "Terms and Conditions",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("Password must consist of at least 8 characters!",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                bool brcIDExists = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(1) FROM BranchTbl WHERE BranchId = @BranchId",
                        new { BranchId = branchid }
                    )) > 0;

                if (!brcIDExists)
                {
                    MessageBox.Show("The provided Branch ID does not exist. Please check and try again.",
                        "Invalid Branch ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                const string query =
                    "INSERT INTO UserTbl (UserId, BranchId, UserName, Password, Role, Status) " +
                    "VALUES (@UserID, @BranchId, @Username, @Password, @Role, @Status)";

                var result = AppDB.Instance.CUD(query, new
                {
                    UserID = IDGenerator.GenerateNextID("USR", branchid),
                    BranchId = branchid,
                    UserName = username,
                    Password = password,
                    Role = "User",
                    Status = "Active"
                });

                if (result > 0)
                {
                    MessageBox.Show("Account created successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    MessageBox.Show("Please proceed to login.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    UserControlLI li = new UserControlLI();
                    MainControlClass.ShowControl(li, this.Parent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // OPEN TERMS AND CONDITIONS
        private void LBL_TAC_Click(object sender, EventArgs e)
        {
            userControl_terms_condition2.Visible = true;
            userControl_terms_condition2.BringToFront();
        }

        // ACCEPT TERMS AND CONDITIONS
        private void userControl_terms_condition2_Accepted(
            object sender, EventArgs e)
        {
            SU_trmconCB.Checked = true;
            userControl_terms_condition2.Visible = false;
        }

        // FORM LOAD
        private void UserControlSU_Load(object sender, EventArgs e)
        {
            userControl_terms_condition2.Visible = false;
            SU_trmconCB.Checked = false;
        }
    }
}
