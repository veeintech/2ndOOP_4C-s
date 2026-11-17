
using System;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Staff : UserControl
    {
        // ==========================================
        // VARIABLES
        // ==========================================

        private string selectedStaffID = "";

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public UserControl_Admin_Staff()
        {
            InitializeComponent();

            // FORM LOAD
            this.Load -= UserControl_Admin_Staff_Load;
            this.Load += UserControl_Admin_Staff_Load;

            // BUTTON EVENTS
            btnAddStaff.Click -= btnAddStaff_Click;
            btnAddStaff.Click += btnAddStaff_Click;

            btnUpdate.Click -= btnUpdate_Click_1;
            btnUpdate.Click += btnUpdate_Click_1;

            btnDelete.Click -= btnDelete_Click_1;
            btnDelete.Click += btnDelete_Click_1;

            btnclear.Click -= btnclear_Click_1;
            btnclear.Click += btnclear_Click_1;

            btnApprove.Click -= btnApprove_Click;
            btnApprove.Click += btnApprove_Click;

            btnReject.Click -= btnReject_Click_1;
            btnReject.Click += btnReject_Click_1;

            // DATAGRID EVENTS
            Request_Dgv.CellClick -= registrationreq_cellContentclick;
            Request_Dgv.CellClick += registrationreq_cellContentclick;

            Staff_Directory_Dgv.CellClick -= Staff_Directory_Dgv_CellClick;
            Staff_Directory_Dgv.CellClick += Staff_Directory_Dgv_CellClick;

            dgv_attendance.CellClick -= attendancedgv_cellClick;
            dgv_attendance.CellClick += attendancedgv_cellClick;

            // SEARCH EVENTS
            txtSearchStaff.TextChanged -= txtSearchStaff_TextChanged;
            txtSearchStaff.TextChanged += txtSearchStaff_TextChanged;

            txtSearchAttendance.TextChanged -= txtSearchAttendance_TextChanged;
            txtSearchAttendance.TextChanged += txtSearchAttendance_TextChanged;

            // GENDER EVENTS
            rdoMale.CheckedChanged -= Gender_CheckedChanged;
            rdoMale.CheckedChanged += Gender_CheckedChanged;

            rdoFemale.CheckedChanged -= Gender_CheckedChanged;
            rdoFemale.CheckedChanged += Gender_CheckedChanged;
        }

        // ==========================================
        // FORM LOAD
        // ==========================================

        private void UserControl_Admin_Staff_Load(object sender, EventArgs e)
        {
            try
            {
                cbEmploymentStatus.Items.Clear();

                cbEmploymentStatus.Items.Add("Active");
                cbEmploymentStatus.Items.Add("Inactive");
                cbEmploymentStatus.Items.Add("Pending");

                cbEmploymentStatus.SelectedItem = "Active";

                txtStaffID.ReadOnly = true;

                btnApprove.Visible = false;
                btnReject.Visible = false;

                LoadBranches();
                LoadStaffData();
                LoadAttendance();

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error initializing staff management: " + ex.Message,
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // LOAD BRANCHES
        // ==========================================

        private void LoadBranches()
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(
                    @"SELECT BranchId, BranchName
                      FROM BranchTbl
                      ORDER BY BranchName");

                cbBranch.DataSource = null;

                cbBranch.DisplayMember = "BranchName";
                cbBranch.ValueMember = "BranchId";
                cbBranch.DataSource = dt;

                cbBranch.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading branches: " + ex.Message);
            }
        }

        // ==========================================
        // STAFF QUERY
        // ==========================================

        private string GetStaffQuery()
        {
            return @"
                SELECT
                    s.StaffID,
                    s.FullName,
                    b.BranchName,
                    s.Gender,
                    s.Adress,
                    s.JobPosition,
                    s.Status,
                    s.BranchID,
                    s.ContactNumber,
                    s.Email,
                    s.Datehired,
                    s.SalaryRate
                FROM StaffTbl s
                LEFT JOIN BranchTbl b
                    ON s.BranchID = b.BranchId
                WHERE
                    s.StaffID LIKE @Search
                    OR s.FullName LIKE @Search
                    OR s.Email LIKE @Search
                    OR b.BranchName LIKE @Search
                ORDER BY s.FullName";
        }

        // ==========================================
        // LOAD STAFF DATA
        // ==========================================

        private void LoadStaffData()
        {
            LoadStaffData("");
        }

        private void LoadStaffData(string search)
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(
                    GetStaffQuery(),
                    new
                    {
                        Search = "%" + search.Trim() + "%"
                    });

                DataTable pending = dt.Clone();
                DataTable directory = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    string status =
                        Convert.ToString(row["Status"]) ?? "";

                    if (status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        pending.ImportRow(row);
                    }
                    else if (
                        status.Equals(
                            "Active",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        status.Equals(
                            "Inactive",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        directory.ImportRow(row);
                    }
                }

                Request_Dgv.DataSource = pending;
                Staff_Directory_Dgv.DataSource = directory;

                FormatStaffGrid(Request_Dgv);
                FormatStaffGrid(Staff_Directory_Dgv);

                UpdateStaffSummary(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading staff: " + ex.Message);
            }
        }

        // ==========================================
        // FORMAT STAFF DATAGRIDVIEW
        // ==========================================

        private void FormatStaffGrid(DataGridView dgv)
        {
            string[] visibleColumns =
            {
                "StaffID",
                "FullName",
                "BranchName",
                "Gender",
                "Adress",
                "JobPosition",
                "Status"
            };

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.Visible =
                    visibleColumns.Contains(column.Name);
            }

            if (dgv.Columns.Contains("StaffID"))
                dgv.Columns["StaffID"].HeaderText = "Staff ID";

            if (dgv.Columns.Contains("FullName"))
                dgv.Columns["FullName"].HeaderText = "Full Name";

            if (dgv.Columns.Contains("BranchName"))
                dgv.Columns["BranchName"].HeaderText = "Branch";

            if (dgv.Columns.Contains("Gender"))
                dgv.Columns["Gender"].HeaderText = "Gender";

            if (dgv.Columns.Contains("Adress"))
                dgv.Columns["Adress"].HeaderText = "Address";

            if (dgv.Columns.Contains("JobPosition"))
                dgv.Columns["JobPosition"].HeaderText = "Position";

            if (dgv.Columns.Contains("Status"))
                dgv.Columns["Status"].HeaderText = "Status";

            dgv.ClearSelection();
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
                string status =
                    Convert.ToString(row["Status"]) ?? "";

                if (status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                {
                    active++;
                    total++;
                }
                else if (status.Equals(
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
                {
                    inactive++;
                    total++;
                }
                else if (status.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
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
        // GET CELL VALUE SAFELY
        // ==========================================

        private string GetCellValue(
            DataGridViewRow row,
            string columnName)
        {
            if (!row.DataGridView.Columns.Contains(columnName))
                return "";

            object value = row.Cells[columnName].Value;

            if (value == null || value == DBNull.Value)
                return "";

            return Convert.ToString(value) ?? "";
        }

        // ==========================================
        // SELECT PENDING REQUEST
        // ==========================================

        private void registrationreq_cellContentclick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row =
                    Request_Dgv.Rows[e.RowIndex];

                DisplaySelectedStaff(row);

                btnApprove.Visible = true;
                btnReject.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error selecting staff request: " + ex.Message);
            }
        }

        // ==========================================
        // SELECT ATTENDANCE RECORD
        // ==========================================

        private void attendancedgv_cellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row =
                    dgv_attendance.Rows[e.RowIndex];

                string staffID =
                    GetCellValue(row, "StaffID");

                if (string.IsNullOrWhiteSpace(staffID))
                    return;

                // Retrieve the complete staff information.
                DataTable dt = AppDB.Instance.TableData(
                    GetStaffQuery().Replace(
                        @"WHERE
                    s.StaffID LIKE @Search
                    OR s.FullName LIKE @Search
                    OR s.Email LIKE @Search
                    OR b.BranchName LIKE @Search",
                        "WHERE s.StaffID = @Search"),
                    new { Search = staffID });

                if (dt.Rows.Count == 0)
                    return;

                DataGridViewRow staffRow =
                    CreateStaffRow(dt.Rows[0]);

                DisplaySelectedStaff(staffRow);

                btnApprove.Visible = false;
                btnReject.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error selecting attendance: " + ex.Message);
            }
        }

        // ==========================================
        // CREATE STAFF GRID ROW
        // ==========================================

        private DataGridViewRow CreateStaffRow(DataRow data)
        {
            DataGridView tempGrid = new DataGridView();

            tempGrid.Columns.Add("StaffID", "StaffID");
            tempGrid.Columns.Add("FullName", "FullName");
            tempGrid.Columns.Add("BranchName", "BranchName");
            tempGrid.Columns.Add("Gender", "Gender");
            tempGrid.Columns.Add("Adress", "Adress");
            tempGrid.Columns.Add("JobPosition", "JobPosition");
            tempGrid.Columns.Add("Status", "Status");
            tempGrid.Columns.Add("BranchID", "BranchID");
            tempGrid.Columns.Add("ContactNumber", "ContactNumber");
            tempGrid.Columns.Add("Email", "Email");
            tempGrid.Columns.Add("Datehired", "Datehired");
            tempGrid.Columns.Add("SalaryRate", "SalaryRate");

            int index = tempGrid.Rows.Add();

            DataGridViewRow row = tempGrid.Rows[index];

            foreach (DataColumn column in data.Table.Columns)
            {
                if (tempGrid.Columns.Contains(column.ColumnName))
                {
                    row.Cells[column.ColumnName].Value =
                        data[column.ColumnName];
                }
            }

            return row;
        }

        // ==========================================
        // SELECT STAFF DIRECTORY
        // ==========================================

        private void Staff_Directory_Dgv_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row =
                    Staff_Directory_Dgv.Rows[e.RowIndex];

                DisplaySelectedStaff(row);

                btnApprove.Visible = false;
                btnReject.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error selecting staff: " + ex.Message);
            }
        }

        // ==========================================
        // DISPLAY SELECTED STAFF
        // ==========================================

        private void DisplaySelectedStaff(DataGridViewRow row)
        {
            selectedStaffID = GetCellValue(row, "StaffID");

            if (string.IsNullOrWhiteSpace(selectedStaffID))
                return;

            string fullName = GetCellValue(row, "FullName");
            string branchID = GetCellValue(row, "BranchID");
            string branchName = GetCellValue(row, "BranchName");
            string contact = GetCellValue(row, "ContactNumber");
            string email = GetCellValue(row, "Email");
            string address = GetCellValue(row, "Adress");
            string gender = GetCellValue(row, "Gender");
            string position = GetCellValue(row, "JobPosition");
            string status = GetCellValue(row, "Status");

            // ======================================
            // RIGHT PANEL
            // ======================================

            lblInfoFullName.Text = fullName;
            lblInfoName.Text = fullName;
            lblInfoStaffID.Text = selectedStaffID;
            lblEmpStaffID.Text = selectedStaffID;
            lblEmpBranch.Text = branchName;

            lblInfoContact.Text = contact;
            lblInfoEmail.Text = email;
            lblInfoAddress.Text = address;

            lblInfoGender.Text =
                gender == "M" ? "Male" :
                gender == "F" ? "Female" : "----";

            lblInfoStatus.Text = status;
            lblEmpStatus.Text = status;

            // ======================================
            // LOWER EDIT FORM
            // ======================================

            txtStaffID.Text = selectedStaffID;
            txtStaffID.ReadOnly = true;

            txtFullName.Text = fullName;
            txtContact.Text = contact;
            txtEmail.Text = email;
            txtAddress.Text = address;

            // BRANCH
            if (cbBranch.DataSource != null &&
                !string.IsNullOrWhiteSpace(branchID))
            {
                cbBranch.SelectedValue = branchID;
            }
            else
            {
                cbBranch.SelectedIndex = -1;
            }

            // JOB POSITION
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

            // EMPLOYMENT STATUS
            if (cbEmploymentStatus.Items.Contains(status))
                cbEmploymentStatus.SelectedItem = status;
            else
                cbEmploymentStatus.SelectedIndex = -1;

            // GENDER
            rdoMale.Checked = gender == "M";
            rdoFemale.Checked = gender == "F";

            // DATE HIRED
            object dateValue = row.Cells["Datehired"].Value;

            if (dateValue != null &&
                dateValue != DBNull.Value)
            {
                DateTime dateHired =
                    Convert.ToDateTime(dateValue);

                dtpDateHired.Value =
                    dateHired < dtpDateHired.MinDate
                    ? dtpDateHired.MinDate
                    : dateHired > dtpDateHired.MaxDate
                    ? dtpDateHired.MaxDate
                    : dateHired;

                lblEmpDateHired.Text =
                    dateHired.ToString("MMMM dd, yyyy");
            }
            else
            {
                dtpDateHired.Value = DateTime.Today;
                lblEmpDateHired.Text = "----";
            }

            // SALARY RATE
            object salaryValue = row.Cells["SalaryRate"].Value;

            if (salaryValue != null &&
                salaryValue != DBNull.Value)
            {
                decimal salary = Convert.ToDecimal(salaryValue);

                txtSalaryRate.Text = salary.ToString(
                    CultureInfo.CurrentCulture);

                lblEmpSalaryRate.Text = salary.ToString("N2");
            }
            else
            {
                txtSalaryRate.Clear();
                lblEmpSalaryRate.Text = "----";
            }
        }

        // ==========================================
        // GENDER DISPLAY
        // ==========================================

        private void Gender_CheckedChanged(
            object sender,
            EventArgs e)
        {
            if (rdoMale.Checked)
                lblInfoGender.Text = "Male";
            else if (rdoFemale.Checked)
                lblInfoGender.Text = "Female";
        }

        // ==========================================
        // GET GENDER
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
        // GET SALARY
        // ==========================================

        private decimal? GetSalary()
        {
            if (string.IsNullOrWhiteSpace(txtSalaryRate.Text))
                return null;

            if (decimal.TryParse(
                txtSalaryRate.Text.Trim(),
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal salary))
            {
                return salary;
            }

            return null;
        }

        // ==========================================
        // VALIDATION
        // ==========================================

        private bool ValidateStaffForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show(
                    "Please complete all required staff information.");

                return false;
            }

            if (cbBranch.SelectedIndex < 0 ||
                cbBranch.SelectedValue == null)
            {
                MessageBox.Show("Please select a branch.");
                return false;
            }

            if (GetGender() == "")
            {
                MessageBox.Show("Please select a gender.");
                return false;
            }

            if (cbEmploymentStatus.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Please select an employment status.");

                return false;
            }

            if (txtContact.Text.Trim().Length > 12 ||
                txtEmail.Text.Trim().Length > 40 ||
                txtAddress.Text.Trim().Length > 100 ||
                txtFullName.Text.Trim().Length > 100)
            {
                MessageBox.Show(
                    "Some information exceeds the allowed database length.");

                return false;
            }

            if (txtContact.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter a contact number.");
                return false;
            }

            if (txtEmail.Text.Trim().Length > 0 &&
                !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtSalaryRate.Text))
            {
                if (!decimal.TryParse(
                    txtSalaryRate.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal salary))
                {
                    MessageBox.Show("Please enter a valid salary rate.");
                    return false;
                }

                if (salary < 0)
                {
                    MessageBox.Show(
                        "Salary rate cannot be negative.");

                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(
                Convert.ToString(cbJobPosition.SelectedItem)) &&
                Convert.ToString(cbJobPosition.SelectedItem).Length > 30)
            {
                MessageBox.Show(
                    "Job position cannot exceed 30 characters.");

                return false;
            }

            return true;
        }

        // ==========================================
        // ADD STAFF
        // ==========================================

        private void btnAddStaff_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffForm())
                return;

            try
            {
                string status =
                    Convert.ToString(cbEmploymentStatus.SelectedItem)
                    ?? "Active";

                if (status.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Admin-added staff must be Active or Inactive.");

                    return;
                }

                string branchID =
                    Convert.ToString(cbBranch.SelectedValue) ?? "";

                if (string.IsNullOrWhiteSpace(branchID))
                {
                    MessageBox.Show("Please select a valid branch.");
                    return;
                }

                // AUTOMATIC STAFF ID
                string newStaffID =
                    IDGenerator.GenerateNextID("STF", branchID);

                if (string.IsNullOrWhiteSpace(newStaffID))
                {
                    MessageBox.Show(
                        "Unable to generate Staff ID.");

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

                int affected = AppDB.Instance.CUD(query, new
                {
                    StaffID = newStaffID,
                    BranchID = branchID,
                    FullName = txtFullName.Text.Trim(),
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Adress = txtAddress.Text.Trim(),
                    Gender = GetGender(),
                    JobPosition = cbJobPosition.SelectedItem == null
                        ? null
                        : Convert.ToString(cbJobPosition.SelectedItem),
                    Datehired = dtpDateHired.Value,
                    SalaryRate = GetSalary(),
                    Status = status
                });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff successfully added.\nStaff ID: " + newStaffID,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadStaffData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Unable to add staff.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding staff: " + ex.Message);
            }
        }

        // ==========================================
        // UPDATE STAFF
        // ==========================================

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff record first.");

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

                int affected = AppDB.Instance.CUD(query, new
                {
                    StaffID = selectedStaffID,
                    BranchID = cbBranch.SelectedValue,
                    FullName = txtFullName.Text.Trim(),
                    ContactNumber = txtContact.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Adress = txtAddress.Text.Trim(),
                    Gender = GetGender(),
                    JobPosition = cbJobPosition.SelectedItem == null
                        ? null
                        : Convert.ToString(cbJobPosition.SelectedItem),
                    Datehired = dtpDateHired.Value,
                    SalaryRate = GetSalary(),
                    Status = Convert.ToString(
                        cbEmploymentStatus.SelectedItem)
                });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff information successfully updated.");

                    LoadStaffData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "No staff record was updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating staff: " + ex.Message);
            }
        }

        // ==========================================
        // DELETE STAFF
        // ==========================================

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff record first.");

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
                int affected = AppDB.Instance.CUD(
                    @"DELETE FROM StaffTbl
                      WHERE StaffID = @StaffID",
                    new { StaffID = selectedStaffID });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff successfully deleted.");
                }
                else
                {
                    MessageBox.Show(
                        "No staff record was deleted.");
                }

                LoadStaffData();
                LoadAttendance();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error deleting staff: " + ex.Message +
                    "\n\nThis staff member may have related records " +
                    "in other tables.");
            }
        }

        // ==========================================
        // APPROVE REQUEST
        // ==========================================

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a pending request first.");

                return;
            }

            try
            {
                int affected = AppDB.Instance.CUD(
                    @"UPDATE StaffTbl
                      SET Status = @Status
                      WHERE StaffID = @StaffID
                      AND Status = 'Pending'",
                    new
                    {
                        Status = "Active",
                        StaffID = selectedStaffID
                    });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff request successfully approved.");

                    LoadStaffData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "This request is no longer pending.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error approving request: " + ex.Message);
            }
        }

        // ==========================================
        // REJECT REQUEST
        // ==========================================

        private void btnReject_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a pending request first.");

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
                int affected = AppDB.Instance.CUD(
                    @"UPDATE StaffTbl
                      SET Status = @Status
                      WHERE StaffID = @StaffID
                      AND Status = 'Pending'",
                    new
                    {
                        Status = "Rejected",
                        StaffID = selectedStaffID
                    });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff request successfully rejected.");

                    LoadStaffData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        "This request is no longer pending.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error rejecting request: " + ex.Message);
            }
        }

        // ==========================================
        // CLEAR BUTTON
        // ==========================================

        private void btnclear_Click_1(object sender, EventArgs e)
        {
            ClearForm();
        }

        // ==========================================
        // CLEAR FORM
        // ==========================================

        private void ClearForm()
        {
            selectedStaffID = "";

            txtStaffID.Clear();
            txtStaffID.ReadOnly = true;

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

            lblEmpDateHired.Text = "----";
            lblEmpSalaryRate.Text = "----";

            btnApprove.Visible = false;
            btnReject.Visible = false;

            Request_Dgv.ClearSelection();
            Staff_Directory_Dgv.ClearSelection();
            dgv_attendance.ClearSelection();
        }

        // ==========================================
        // SEARCH STAFF
        // ==========================================

        private void txtSearchStaff_TextChanged(
            object sender,
            EventArgs e)
        {
            LoadStaffData(txtSearchStaff.Text);
        }

        // ==========================================
        // LOAD ATTENDANCE
        // ==========================================

        private string GetAttendanceQuery()
        {
            return @"
                SELECT
                    a.AttendanceID,
                    a.StaffID,
                    s.FullName,
                    a.[Date],
                    a.Time_IN,
                    a.Time_OUT,

                    s.BranchID,
                    b.BranchName,
                    s.ContactNumber,
                    s.Email,
                    s.Adress,
                    s.Gender,
                    s.JobPosition,
                    s.Status,
                    s.Datehired,
                    s.SalaryRate

                FROM AttendanceTbl a

                INNER JOIN StaffTbl s
                    ON a.StaffID = s.StaffID

                LEFT JOIN BranchTbl b
                    ON s.BranchID = b.BranchId

                WHERE
                    a.StaffID LIKE @Search
                    OR s.FullName LIKE @Search
                    OR CONVERT(VARCHAR(10), a.[Date], 101) LIKE @Search

                ORDER BY a.[Date] DESC";
        }

        private void LoadAttendance()
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(
                    GetAttendanceQuery(),
                    new { Search = "%%" });

                dgv_attendance.DataSource = dt;

                FormatAttendanceGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading attendance: " + ex.Message);
            }
        }

        // ==========================================
        // FORMAT ATTENDANCE GRID
        // ==========================================

        private void FormatAttendanceGrid()
        {
            string[] visibleColumns =
            {
                "AttendanceID",
                "StaffID",
                "FullName",
                "Date",
                "Time_IN",
                "Time_OUT"
            };

            foreach (DataGridViewColumn column in dgv_attendance.Columns)
            {
                column.Visible =
                    visibleColumns.Contains(column.Name);
            }

            if (dgv_attendance.Columns.Contains("AttendanceID"))
                dgv_attendance.Columns["AttendanceID"].HeaderText =
                    "Attendance ID";

            if (dgv_attendance.Columns.Contains("StaffID"))
                dgv_attendance.Columns["StaffID"].HeaderText =
                    "Staff ID";

            if (dgv_attendance.Columns.Contains("FullName"))
                dgv_attendance.Columns["FullName"].HeaderText =
                    "Full Name";

            if (dgv_attendance.Columns.Contains("Date"))
                dgv_attendance.Columns["Date"].HeaderText =
                    "Date";

            if (dgv_attendance.Columns.Contains("Time_IN"))
                dgv_attendance.Columns["Time_IN"].HeaderText =
                    "Time In";

            if (dgv_attendance.Columns.Contains("Time_OUT"))
                dgv_attendance.Columns["Time_OUT"].HeaderText =
                    "Time Out";

            dgv_attendance.ClearSelection();
        }

        // ==========================================
        // SEARCH ATTENDANCE
        // ==========================================

        private void txtSearchAttendance_TextChanged(
            object sender,
            EventArgs e)
        {
            try
            {
                string search =
                    txtSearchAttendance.Text.Trim();

                DataTable dt = AppDB.Instance.TableData(
                    GetAttendanceQuery(),
                    new { Search = "%" + search + "%" });

                dgv_attendance.DataSource = dt;

                FormatAttendanceGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error searching attendance: " + ex.Message);
            }
        }

        // ==========================================
        // DISPLAY EMPLOYEE INFORMATION
        // ==========================================

        public void displaynon()
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
                return;

            try
            {
                const string query = @"
                    SELECT Datehired, SalaryRate
                    FROM StaffTbl
                    WHERE StaffID = @StaffID";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new { StaffID = selectedStaffID });

                if (dt.Rows.Count > 0)
                {
                    object dateValue = dt.Rows[0]["Datehired"];
                    object salaryValue = dt.Rows[0]["SalaryRate"];

                    lblEmpDateHired.Text =
                        dateValue == DBNull.Value
                        ? "----"
                        : Convert.ToDateTime(dateValue)
                            .ToString("MMMM dd, yyyy");

                    lblEmpSalaryRate.Text =
                        salaryValue == DBNull.Value
                        ? "----"
                        : Convert.ToDecimal(salaryValue)
                            .ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying employee information: " +
                    ex.Message);
            }
        }
    }
}
