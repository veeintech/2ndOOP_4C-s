
using _4C_s_Management_System.Classess;
using System;
using System.Windows.Forms;

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

        // BACK TO LOGIN
        private void LOGIN_lblsignupclk(object sender, EventArgs e)
        {
            UserControlLI li = new UserControlLI();
            MainControlClass.ShowControl(li, this.Parent);
        }

        // HIDE PASSWORD
        private void SU_closedeyeBtn_Click_1(object sender, EventArgs e)
        {
            SU_passwordtxt.PasswordChar = '*';
            SU_conpasstxt.PasswordChar = '*';

            SU_openedeyeBtn.BringToFront();
        }

        // SHOW PASSWORD
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
            string staffid = TXTStaffID.Text.Trim();
            string username = SU_usernametxt.Text.Trim();
            string password = SU_passwordtxt.Text;
            string confirmPassword = SU_conpasstxt.Text;

            // CHECK EMPTY FIELDS
            if (string.IsNullOrWhiteSpace(branchid) ||
                string.IsNullOrWhiteSpace(staffid) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // CHECK BRANCH
            bool branchExists;

            try
            {
                branchExists = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        @"SELECT COUNT(1)
                          FROM BranchTbl
                          WHERE BranchId = @BranchId",
                        new { BranchId = branchid }
                    )
                ) > 0;

                if (!branchExists)
                {
                    MessageBox.Show(
                        "The provided Branch ID does not exist. Please check and try again.",
                        "Invalid Branch ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // CHECK STAFF ID AND BRANCH
                if (checkstaff(staffid, branchid) < 1)
                {
                    MessageBox.Show(
                        "Staff ID does not exist, is not assigned to this branch, or is not Active.",
                        "Invalid Staff",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // CHECK IF STAFF ALREADY HAS AN ACCOUNT
                int existingAccount = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        @"SELECT COUNT(1)
                          FROM UserTbl
                          WHERE StaffId = @StaffID",
                        new { StaffID = staffid }
                    )
                );

                if (existingAccount > 0)
                {
                    MessageBox.Show(
                        "This staff already has an account.",
                        "Existing Account",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // CHECK PASSWORD MATCH
                if (password != confirmPassword)
                {
                    MessageBox.Show(
                        "Passwords do not match.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // CHECK TERMS AND CONDITIONS
                if (!SU_trmconCB.Checked)
                {
                    MessageBox.Show(
                        "Please accept the terms and conditions to proceed.",
                        "Terms and Conditions",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // CHECK PASSWORD LENGTH
                if (password.Length < 8)
                {
                    MessageBox.Show(
                        "Password must consist of at least 8 characters!",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                // CHECK USERNAME
                int existingUsername = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        @"SELECT COUNT(1)
                          FROM UserTbl
                          WHERE UserName = @Username",
                        new { Username = username }
                    )
                );

                if (existingUsername > 0)
                {
                    MessageBox.Show(
                        "Username already exists. Please choose another username.",
                        "Duplicate Username",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // INSERT ACCOUNT
                const string query = @"
                    INSERT INTO UserTbl
                    (
                        UserId,
                        StaffId,
                        BranchId,
                        UserName,
                        Password,
                        Role,
                        Status
                    )
                    VALUES
                    (
                        @UserID,
                        @StaffID,
                        @BranchId,
                        @Username,
                        @Password,
                        @Role,
                        @Status
                    )";

                string newUserID =
                    IDGenerator.GenerateNextID("USR", branchid);

                int result = AppDB.Instance.CUD(query, new
                {
                    UserID = newUserID,
                    StaffID = staffid,
                    BranchId = branchid,
                    Username = username,
                    Password = password,
                    Role = "User",
                    Status = "Active"
                });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Account created successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    MessageBox.Show(
                        "Please proceed to login.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    UserControlLI li = new UserControlLI();
                    MainControlClass.ShowControl(li, this.Parent);
                }
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

        // CHECK STAFF
        public int checkstaff(string staff, string branchid)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM StaffTbl
                WHERE StaffID = @StaffID
                  AND BranchID = @BranchID
                  AND Status = 'Active'";

            int result = Convert.ToInt32(
                AppDB.Instance.Scalar(query, new
                {
                    StaffID = staff,
                    BranchID = branchid
                })
            );

            return result > 0 ? 1 : 0;
        }
    }
}
