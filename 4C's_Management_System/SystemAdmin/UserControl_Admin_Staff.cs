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
    public partial class UserControl_Admin_Staff : UserControl
    {
        private string selectedStaffID = "";

        public UserControl_Admin_Staff()
        {
            InitializeComponent();

            this.Load -= UserControl_Admin_Staff_Load;
            this.Load += UserControl_Admin_Staff_Load;

            btnAddStaff.Click -= btnAddStaff_Click;
            btnAddStaff.Click += btnAddStaff_Click;

            btnUpdate.Click -= btnUpdate_Click;
            btnUpdate.Click += btnUpdate_Click;

            btnDelete.Click -= btnDelete_Click;
            btnDelete.Click += btnDelete_Click;

            btnclear.Click -= btnclear_Click;
            btnclear.Click += btnclear_Click;

            btnReject.Click -= btnReject_Click;
            btnReject.Click += btnReject_Click;

            txtSearchStaff.TextChanged -= txtSearchStaff_TextChanged;
            txtSearchStaff.TextChanged += txtSearchStaff_TextChanged;

            txtSearchAttendance.TextChanged -= txtSearchAttendance_TextChanged;
            txtSearchAttendance.TextChanged += txtSearchAttendance_TextChanged;

            rdoMale.CheckedChanged += Gender_CheckedChanged;
            rdoFemale.CheckedChanged += Gender_CheckedChanged;
        }

        // ==========================================
        // FORM LOAD
        // ==========================================

        private void UserControl_Admin_Staff_Load(object sender, EventArgs e)
        {
            LoadBranches();
            LoadStaffData();
            LoadAttendance();

            cbEmploymentStatus.Items.Clear();
            cbEmploymentStatus.Items.Add("Pending");
            cbEmploymentStatus.Items.Add("Active");
            cbEmploymentStatus.Items.Add("Inactive");

            cbEmploymentStatus.SelectedItem = "Active";

            btnApprove.Visible = false;
            btnReject.Visible = false;
        }

        // ==========================================
        // LOAD BRANCHES
        // ==========================================

        private void LoadBranches()
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(
                    "SELECT BranchId, BranchName FROM BranchTbl ORDER BY BranchName");

                cbBranch.DisplayMember = "BranchName";
                cbBranch.ValueMember = "BranchId";
                cbBranch.DataSource = dt;
                cbBranch.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading branches: " + ex.Message);
            }
        }

        // ==========================================
        // LOAD STAFF DATA
        // ==========================================

        private void LoadStaffData()
        {
            try
            {
                string query = @"
                    SELECT
                        s.StaffID,
                        s.BranchID,
                        b.BranchName,
                        s.FullName,
                        s.ContactNumber,
                        s.Email,
                        s.Adress,
                        s.Gender,
                        s.JobPosition,
                        s.Datehired,
                        s.SalaryRate,
                        s.Status
                    FROM StaffTbl s
                    LEFT JOIN BranchTbl b
                        ON s.BranchID = b.BranchId
                    ORDER BY s.FullName";

                DataTable dt = AppDB.Instance.TableData(query);

                DataTable pending = dt.Clone();
                DataTable directory = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    string status = Convert.ToString(row["Status"]) ?? "";

                    if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        pending.ImportRow(row);
                    }
                    else if (
                        status.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
                        status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        directory.ImportRow(row);
                    }
                }

                Request_Dgv.DataSource = pending;
                Staff_Directory_Dgv.DataSource = directory;

                UpdateStaffSummary(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading staff: " + ex.Message);
            }
        }

        // ==========================================
        // STAFF SUMMARY
        // ==========================================

        private void UpdateStaffSummary(DataTable dt)
        {
            int total = 0;
            int active = 0;
            int pending = 0;
            int inactive = 0;

            foreach (DataRow row in dt.Rows)
            {
                string status = Convert.ToString(row["Status"]) ?? "";

                if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    active++;
                    total++;
                }
                else if (status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    inactive++;
                    total++;
                }
                else if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    pending++;
                    total++;
                }
            }

            lblTotalStaff.Text = total.ToString();
            lblActiveStaff.Text = active.ToString();
            lblPendingStaff.Text = pending.ToString();
            lblInactiveStaff.Text = inactive.ToString();
        }

        // ==========================================
        // SELECT PENDING REQUEST
        // ==========================================

        private void Request_Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Request_Dgv.Rows[e.RowIndex];

            DisplaySelectedStaff(row);

            btnApprove.Visible = true;
            btnReject.Visible = true;
        }

        // ==========================================
        // SELECT STAFF DIRECTORY
        // ==========================================

        private void Staff_Directory_Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = Staff_Directory_Dgv.Rows[e.RowIndex];

            DisplaySelectedStaff(row);

            btnApprove.Visible = false;
            btnReject.Visible = false;
        }

        // ==========================================
        // DISPLAY SELECTED STAFF
        // RIGHT PANEL + LOWER EDIT FORM
        // ==========================================

        private void DisplaySelectedStaff(DataGridViewRow row)
        {
            selectedStaffID = Convert.ToString(row.Cells["StaffID"].Value) ?? "";

            string staffID = selectedStaffID;
            string fullName = Convert.ToString(row.Cells["FullName"].Value) ?? "";
            string branchID = Convert.ToString(row.Cells["BranchID"].Value) ?? "";
            string branchName = Convert.ToString(row.Cells["BranchName"].Value) ?? "";
            string contact = Convert.ToString(row.Cells["ContactNumber"].Value) ?? "";
            string email = Convert.ToString(row.Cells["Email"].Value) ?? "";
            string address = Convert.ToString(row.Cells["Adress"].Value) ?? "";
            string gender = Convert.ToString(row.Cells["Gender"].Value) ?? "";
            string position = Convert.ToString(row.Cells["JobPosition"].Value) ?? "";
            string status = Convert.ToString(row.Cells["Status"].Value) ?? "";

            // RIGHT PANEL - DISPLAY ONLY

            lblInfoFullName.Text = fullName;
            lblInfoName.Text = fullName;
            lblInfoStaffID.Text = staffID;
            lblEmpStaffID.Text = staffID;
            lblEmpBranch.Text = branchName;
            lblInfoContact.Text = contact;
            lblInfoEmail.Text = email;
            lblInfoAddress.Text = address;
            lblInfoGender.Text = gender == "M" ? "Male" :
                                 gender == "F" ? "Female" : "";

            lblInfoStatus.Text = status;
            lblEmpStatus.Text = status;

            // LOWER PANEL - EDITABLE FORM

            txtStaffID.Text = staffID;
            txtFullName.Text = fullName;
            txtContact.Text = contact;
            txtEmail.Text = email;
            txtAddress.Text = address;

            if (cbBranch.DataSource != null)
                cbBranch.SelectedValue = branchID;

            if (!string.IsNullOrWhiteSpace(position))
            {
                if (!cbJobPosition.Items.Contains(position))
                    cbJobPosition.Items.Add(position);

                cbJobPosition.SelectedItem = position;
            }
            else
            {
                cbJobPosition.SelectedIndex = -1;
            }

            cbEmploymentStatus.SelectedItem = status;

            rdoMale.Checked = gender == "M";
            rdoFemale.Checked = gender == "F";

            if (row.Cells["Datehired"].Value != DBNull.Value &&
                row.Cells["Datehired"].Value != null)
            {
                dtpDateHired.Value = Convert.ToDateTime(row.Cells["Datehired"].Value);
            }

            txtSalaryRate.Text =
                row.Cells["SalaryRate"].Value == DBNull.Value
                ? ""
                : Convert.ToString(row.Cells["SalaryRate"].Value) ?? "";
        }

        // ==========================================
        // GENDER DISPLAY
        // ==========================================

        private void Gender_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoMale.Checked)
                lblInfoGender.Text = "Male";
            else if (rdoFemale.Checked)
                lblInfoGender.Text = "Female";
        }

        // ==========================================
        // GET SELECTED GENDER
        // ==========================================

        private string GetGender()
        {
            if (rdoMale.Checked)
                return "M";

            if (rdoFemale.Checked)
                return "F";

            return "";
        }

        // ==========================================
        // VALIDATION
        // ==========================================

        private bool ValidateStaffForm()
        {
            if (string.IsNullOrWhiteSpace(txtStaffID.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please complete all required staff information.");
                return false;
            }

            if (cbBranch.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a branch.");
                return false;
            }

            if (GetGender() == "")
            {
                MessageBox.Show("Please select a gender.");
                return false;
            }

            if (!decimal.TryParse(txtSalaryRate.Text, out decimal salary))
            {
                if (!string.IsNullOrWhiteSpace(txtSalaryRate.Text))
                {
                    MessageBox.Show("Please enter a valid salary rate.");
                    return false;
                }
            }

            return true;
        }

        private decimal? GetSalary()
        {
            if (string.IsNullOrWhiteSpace(txtSalaryRate.Text))
                return null;

            return decimal.Parse(txtSalaryRate.Text);
        }

        // ==========================================
        // ADD STAFF - SYSTEM ADMIN
        // ==========================================

        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffForm())
                return;

            try
            {
                string status = Convert.ToString(cbEmploymentStatus.SelectedItem) ?? "Active";

                if (status == "Pending")
                {
                    MessageBox.Show("Admin-added staff must be Active or Inactive.");
                    return;
                }

                string query = @"
                    INSERT INTO StaffTbl
                    (
                        StaffID,
                        BranchID,
                        FullName,
                        ContactNumber,
                        Email,
                        Adress,
                        Gender,
                        JobPosition,
                        Datehired,
                        SalaryRate,
                        Status
                    )
                    VALUES
                    (
                        @StaffID,
                        @BranchID,
                        @FullName,
                        @ContactNumber,
                        @Email,
                        @Adress,
                        @Gender,
                        @JobPosition,
                        @Datehired,
                        @SalaryRate,
                        @Status
                    )";

                AppDB.Instance.CUD(query, new
                {
                    StaffID = txtStaffID.Text.Trim(),
                    BranchID = cbBranch.SelectedValue,
                    FullName = txtFullName.Text.Trim(),
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Adress = txtAddress.Text.Trim(),
                    Gender = GetGender(),
                    JobPosition = Convert.ToString(cbJobPosition.SelectedItem),
                    Datehired = dtpDateHired.Value,
                    SalaryRate = GetSalary(),
                    Status = status
                });

                MessageBox.Show("Staff successfully added.");

                LoadStaffData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding staff: " + ex.Message);
            }
        }

        // ==========================================
        // UPDATE STAFF - LOWER FORM
        // ==========================================

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a staff record first.");
                return;
            }

            if (!ValidateStaffForm())
                return;

            try
            {
                string query = @"
                    UPDATE StaffTbl
                    SET
                        BranchID = @BranchID,
                        FullName = @FullName,
                        ContactNumber = @ContactNumber,
                        Email = @Email,
                        Adress = @Adress,
                        Gender = @Gender,
                        JobPosition = @JobPosition,
                        Datehired = @Datehired,
                        SalaryRate = @SalaryRate,
                        Status = @Status
                    WHERE StaffID = @StaffID";

                AppDB.Instance.CUD(query, new
                {
                    StaffID = selectedStaffID,
                    BranchID = cbBranch.SelectedValue,
                    FullName = txtFullName.Text.Trim(),
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Adress = txtAddress.Text.Trim(),
                    Gender = GetGender(),
                    JobPosition = Convert.ToString(cbJobPosition.SelectedItem),
                    Datehired = dtpDateHired.Value,
                    SalaryRate = GetSalary(),
                    Status = Convert.ToString(cbEmploymentStatus.SelectedItem)
                });

                MessageBox.Show("Staff information successfully updated.");

                LoadStaffData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating staff: " + ex.Message);
            }
        }

        // ==========================================
        // DELETE STAFF
        // ==========================================

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a staff record first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this staff record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                AppDB.Instance.CUD(
                    "DELETE FROM StaffTbl WHERE StaffID = @StaffID",
                    new { StaffID = selectedStaffID });

                MessageBox.Show("Staff successfully deleted.");

                LoadStaffData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting staff: " + ex.Message);
            }
        }

        // ==========================================
        // APPROVE REQUEST
        // ==========================================

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a pending request first.");
                return;
            }

            try
            {
                AppDB.Instance.CUD(
                    @"UPDATE StaffTbl
                      SET Status = @Status
                      WHERE StaffID = @StaffID AND Status = 'Pending'",
                    new
                    {
                        Status = "Active",
                        StaffID = selectedStaffID
                    });

                MessageBox.Show("Staff request approved.");

                LoadStaffData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving request: " + ex.Message);
            }
        }

        // ==========================================
        // REJECT REQUEST
        // ==========================================

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a pending request first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reject this staff request?",
                "Confirm Rejection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                AppDB.Instance.CUD(
                    @"UPDATE StaffTbl
                      SET Status = @Status
                      WHERE StaffID = @StaffID AND Status = 'Pending'",
                    new
                    {
                        Status = "Rejected",
                        StaffID = selectedStaffID
                    });

                MessageBox.Show("Staff request rejected.");

                LoadStaffData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error rejecting request: " + ex.Message);
            }
        }

        // ==========================================
        // CLEAR FORM
        // ==========================================

        private void btnclear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedStaffID = "";

            txtStaffID.Clear();
            txtFullName.Clear();
            txtContact.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtSalaryRate.Clear();

            cbBranch.SelectedIndex = -1;
            cbJobPosition.SelectedIndex = -1;
            cbEmploymentStatus.SelectedItem = "Active";

            rdoMale.Checked = false;
            rdoFemale.Checked = false;

            dtpDateHired.Value = DateTime.Today;

            lblInfoFullName.Text = "----";
            lblInfoName.Text = "----";
            lblInfoStaffID.Text = "----";
            lblEmpStaffID.Text = "----";
            lblEmpBranch.Text = "----";
            lblInfoContact.Text = "----";
            lblInfoEmail.Text = "----";
            lblInfoAddress.Text = "----";
            lblInfoGender.Text = "----";
            lblInfoStatus.Text = "----";
            lblEmpStatus.Text = "----";

            btnApprove.Visible = false;
            btnReject.Visible = false;
        }

        // ==========================================
        // SEARCH STAFF
        // ==========================================

        private void txtSearchStaff_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string search = txtSearchStaff.Text.Trim();

                string query = @"
                    SELECT
                        s.StaffID,
                        s.BranchID,
                        b.BranchName,
                        s.FullName,
                        s.ContactNumber,
                        s.Email,
                        s.Adress,
                        s.Gender,
                        s.JobPosition,
                        s.Datehired,
                        s.SalaryRate,
                        s.Status
                    FROM StaffTbl s
                    LEFT JOIN BranchTbl b
                        ON s.BranchID = b.BranchId
                    WHERE
                        s.StaffID LIKE @Search
                        OR s.FullName LIKE @Search
                        OR s.Email LIKE @Search
                    ORDER BY s.FullName";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new { Search = "%" + search + "%" });

                DataTable pending = dt.Clone();
                DataTable directory = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    string status = Convert.ToString(row["Status"]) ?? "";

                    if (status == "Pending")
                        pending.ImportRow(row);
                    else if (status == "Active" || status == "Inactive")
                        directory.ImportRow(row);
                }

                Request_Dgv.DataSource = pending;
                Staff_Directory_Dgv.DataSource = directory;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching staff: " + ex.Message);
            }
        }

        // ==========================================
        // LOAD ATTENDANCE
        // ==========================================

        private void LoadAttendance()
        {
            try
            {
                string query = @"
                    SELECT
                        a.AttendanceID,
                        a.StaffID,
                        s.FullName,
                        a.[Date],
                        a.Time_IN,
                        a.Time_OUT
                    FROM AttendanceTbl a
                    INNER JOIN StaffTbl s
                        ON a.StaffID = s.StaffID
                    ORDER BY a.[Date] DESC";

                dgv_attendance.DataSource = AppDB.Instance.TableData(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance: " + ex.Message);
            }
        }

        // ==========================================
        // SEARCH ATTENDANCE
        // ==========================================

        private void txtSearchAttendance_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string search = txtSearchAttendance.Text.Trim();

                string query = @"
                    SELECT
                        a.AttendanceID,
                        a.StaffID,
                        s.FullName,
                        a.[Date],
                        a.Time_IN,
                        a.Time_OUT
                    FROM AttendanceTbl a
                    INNER JOIN StaffTbl s
                        ON a.StaffID = s.StaffID
                    WHERE
                        a.StaffID LIKE @Search
                        OR s.FullName LIKE @Search
                        OR CONVERT(VARCHAR(10), a.[Date], 101) LIKE @Search
                    ORDER BY a.[Date] DESC";

                dgv_attendance.DataSource = AppDB.Instance.TableData(
                    query,
                    new { Search = "%" + search + "%" });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching attendance: " + ex.Message);
            }
        }
    }
}