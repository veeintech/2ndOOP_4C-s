
using System;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControlStaff : UserControl
    {
        // =========================
        // VARIABLES
        // =========================

        public string branchid = AccessBranch_ByForm.BranchId;

        public string attendanceid = "";
        public TimeOnly? timeIn = null;
        public TimeOnly? timeOut = null;

        private string selectedStaffID = "";
        private DateTime? selectedDateHired = null;
        private bool isEditing = false;

        public UserControlStaff()
        {
            InitializeComponent();

            DGV_Attendance.CellFormatting -= DGV_Attendance_CellFormatting;
            DGV_Attendance.CellFormatting += DGV_Attendance_CellFormatting;

            DGV_Attendance.DataError -= DGV_Attendance_DataError;
            DGV_Attendance.DataError += DGV_Attendance_DataError;
        }

        // =====================================================
        // CONVERT SQL TIME TO TIMEONLY
        // =====================================================

        private TimeOnly? ConvertToTimeOnly(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (value is TimeSpan timeSpan)
                return TimeOnly.FromTimeSpan(timeSpan);

            if (value is TimeOnly timeOnly)
                return timeOnly;

            if (value is DateTime dateTime)
                return TimeOnly.FromDateTime(dateTime);

            if (TimeSpan.TryParse(
                Convert.ToString(value),
                out TimeSpan parsedTime))
            {
                return TimeOnly.FromTimeSpan(parsedTime);
            }

            return null;
        }

        // =====================================================
        // AUTOMATIC DAILY ATTENDANCE GENERATION
        // =====================================================

        public void EnsureDailyAttendanceRecords()
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(branchid))
                return;

            try
            {
                const string query = @"
                    SELECT StaffID
                    FROM dbo.StaffTbl
                    WHERE BranchID = @BranchID
                    AND Status = 'Active';";

                DataTable staffData = AppDB.Instance.TableData(
                    query,
                    new { BranchID = branchid.Trim() }
                );

                foreach (DataRow row in staffData.Rows)
                {
                    string staffID =
                        Convert.ToString(row["StaffID"]) ?? "";

                    if (string.IsNullOrWhiteSpace(staffID))
                        continue;

                    const string checkQuery = @"
                        SELECT COUNT(*)
                        FROM dbo.AttendanceTbl
                        WHERE StaffID = @StaffID
                        AND [Date] = @Date;";

                    int exists = Convert.ToInt32(
                        AppDB.Instance.Scalar(
                            checkQuery,
                            new
                            {
                                StaffID = staffID,
                                Date = DateTime.Today
                            }
                        )
                    );

                    if (exists > 0)
                        continue;

                    string newAttendanceID =
                        IDGenerator.GenerateNextID("ATT", branchid);

                    const string insertQuery = @"
                        INSERT INTO dbo.AttendanceTbl
                        (
                            AttendanceID,
                            StaffID,
                            [Date],
                            Time_IN,
                            Time_OUT
                        )
                        SELECT
                            @AttendanceID,
                            S.StaffID,
                            @Date,
                            NULL,
                            NULL
                        FROM dbo.StaffTbl S
                        WHERE S.StaffID = @StaffID
                        AND S.BranchID = @BranchID
                        AND S.Status = 'Active'
                        AND NOT EXISTS
                        (
                            SELECT 1
                            FROM dbo.AttendanceTbl A
                            WHERE A.StaffID = S.StaffID
                            AND A.[Date] = @Date
                        );";

                    AppDB.Instance.CUD(
                        insertQuery,
                        new
                        {
                            AttendanceID = newAttendanceID,
                            StaffID = staffID,
                            BranchID = branchid.Trim(),
                            Date = DateTime.Today
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error generating daily attendance:\n" + ex.Message,
                    "Attendance Generation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // CREATE ATTENDANCE WHEN STAFF IS ACTIVATED
        // =====================================================

        public void CreateAttendanceForActivatedStaff(string staffID)
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(staffID) ||
                string.IsNullOrWhiteSpace(branchid))
                return;

            try
            {
                const string checkQuery = @"
                    SELECT COUNT(*)
                    FROM dbo.AttendanceTbl
                    WHERE StaffID = @StaffID
                    AND [Date] = @Date;";

                int exists = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        checkQuery,
                        new
                        {
                            StaffID = staffID.Trim(),
                            Date = DateTime.Today
                        }
                    )
                );

                if (exists > 0)
                    return;

                string newAttendanceID =
                    IDGenerator.GenerateNextID("ATT", branchid);

                const string query = @"
                    INSERT INTO dbo.AttendanceTbl
                    (
                        AttendanceID,
                        StaffID,
                        [Date],
                        Time_IN,
                        Time_OUT
                    )
                    SELECT
                        @AttendanceID,
                        S.StaffID,
                        @Date,
                        NULL,
                        NULL
                    FROM dbo.StaffTbl S
                    WHERE S.StaffID = @StaffID
                    AND S.BranchID = @BranchID
                    AND S.Status = 'Active'
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM dbo.AttendanceTbl A
                        WHERE A.StaffID = S.StaffID
                        AND A.[Date] = @Date
                    );";

                AppDB.Instance.CUD(
                    query,
                    new
                    {
                        AttendanceID = newAttendanceID,
                        StaffID = staffID.Trim(),
                        BranchID = branchid.Trim(),
                        Date = DateTime.Today
                    }
                );

                LoadStaff();
                LoadStaffCounts();
                loadattendance();

                if (selectedStaffID == staffID)
                {
                    LoadSelectedStaffAttendance();
                    UpdateAttendanceButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error creating attendance for activated staff:\n" +
                    ex.Message,
                    "Attendance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // LOAD ATTENDANCE GRID
        // =====================================================

        public void loadattendance()
        {
            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(branchid))
                {
                    DGV_Attendance.DataSource = null;
                    return;
                }

                const string attQ = @"
                    SELECT
                        A.AttendanceID,
                        A.StaffID,
                        S.FullName,
                        A.[Date],
                        A.Time_IN,
                        A.Time_OUT
                    FROM dbo.AttendanceTbl A
                    INNER JOIN dbo.StaffTbl S
                        ON A.StaffID = S.StaffID
                    WHERE S.BranchID = @BranchID
                    ORDER BY A.[Date] DESC, S.FullName ASC;";

                DataTable dt = AppDB.Instance.TableData(
                    attQ,
                    new { BranchID = branchid.Trim() }
                );

                DGV_Attendance.DataSource = null;
                DGV_Attendance.AutoGenerateColumns = true;
                DGV_Attendance.DataSource = dt;

                DGV_Attendance.ReadOnly = true;
                DGV_Attendance.AllowUserToAddRows = false;
                DGV_Attendance.AllowUserToDeleteRows = false;
                DGV_Attendance.MultiSelect = false;
                DGV_Attendance.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                if (DGV_Attendance.Columns.Count > 0)
                {
                    DGV_Attendance.Columns["AttendanceID"].Visible = false;

                    DGV_Attendance.Columns["StaffID"].HeaderText = "Staff ID";
                    DGV_Attendance.Columns["FullName"].HeaderText = "Staff Name";
                    DGV_Attendance.Columns["Date"].HeaderText = "Date";
                    DGV_Attendance.Columns["Time_IN"].HeaderText = "Time In";
                    DGV_Attendance.Columns["Time_OUT"].HeaderText = "Time Out";

                    DGV_Attendance.Columns["Date"]
                        .DefaultCellStyle.Format = "MMM dd, yyyy";

                    // Do not set "hh:mm tt" directly on SQL TIME columns.
                    // The CellFormatting event handles their display.

                    DGV_Attendance.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading attendance:\n" + ex.Message,
                    "Attendance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // ATTENDANCE GRID TIME FORMATTING
        // =====================================================

        private void DGV_Attendance_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName =
                DGV_Attendance.Columns[e.ColumnIndex].Name;

            if (columnName == "Time_IN" ||
                columnName == "Time_OUT")
            {
                if (e.Value == null || e.Value == DBNull.Value)
                {
                    e.Value = "Not recorded";
                    e.FormattingApplied = true;
                    return;
                }

                TimeOnly? time = ConvertToTimeOnly(e.Value);

                e.Value = time.HasValue
                    ? time.Value.ToString("hh:mm tt")
                    : "Not recorded";

                e.FormattingApplied = true;
            }
        }

        // =====================================================
        // ATTENDANCE GRID DATA ERROR
        // =====================================================

        private void DGV_Attendance_DataError(
            object sender,
            DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        // =====================================================
        // FORM LOAD
        // =====================================================

        private void UserControlStaff_Load(object sender, EventArgs e)
        {
            branchid = AccessBranch_ByForm.BranchId;

            cb_job.Items.Clear();

            cb_job.Items.AddRange(new object[]
            {
                "Manager",
                "Receptionist",
                "Housekeeping",
                "Maintenance",
                "Security",
                "Staff"
            });

            Male_RB.Checked = false;
            Female_RB.Checked = false;

            dgv_staff.ReadOnly = true;
            dgv_staff.AllowUserToAddRows = false;
            dgv_staff.AllowUserToDeleteRows = false;
            dgv_staff.MultiSelect = false;
            dgv_staff.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            DGV_Attendance.ReadOnly = true;
            DGV_Attendance.AllowUserToAddRows = false;
            DGV_Attendance.AllowUserToDeleteRows = false;
            DGV_Attendance.MultiSelect = false;
            DGV_Attendance.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            ClearFields();

            EnsureDailyAttendanceRecords();

            LoadStaff();
            LoadStaffCounts();
            loadattendance();

            UpdateAttendanceButtons();
        }

        // =====================================================
        // LOAD SELECTED STAFF ATTENDANCE FOR TODAY
        // =====================================================

        private void LoadSelectedStaffAttendance()
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
                return;

            try
            {
                const string query = @"
                    SELECT
                        AttendanceID,
                        Time_IN,
                        Time_OUT
                    FROM dbo.AttendanceTbl
                    WHERE StaffID = @StaffID
                    AND [Date] = @Date;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        StaffID = selectedStaffID,
                        Date = DateTime.Today
                    }
                );

                attendanceid = "";
                timeIn = null;
                timeOut = null;

                if (dt.Rows.Count == 0)
                {
                    lbl_timein.Text = "Not recorded";
                    lbl_timeout.Text = "Not recorded";
                    return;
                }

                DataRow row = dt.Rows[0];

                attendanceid =
                    Convert.ToString(row["AttendanceID"]) ?? "";

                timeIn = ConvertToTimeOnly(row["Time_IN"]);
                timeOut = ConvertToTimeOnly(row["Time_OUT"]);

                lbl_timein.Text =
                    timeIn?.ToString("hh:mm tt") ?? "Not recorded";

                lbl_timeout.Text =
                    timeOut?.ToString("hh:mm tt") ?? "Not recorded";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load staff attendance:\n" + ex.Message,
                    "Attendance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // UPDATE TIME IN / TIME OUT BUTTON STATES
        // =====================================================

        private void UpdateAttendanceButtons()
        {
            btn_timein.Enabled = false;
            btn_timeout.Enabled = false;

            if (string.IsNullOrWhiteSpace(selectedStaffID))
                return;

            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(branchid))
                return;

            try
            {
                const string query = @"
                    SELECT
                        S.Status,
                        A.Time_IN,
                        A.Time_OUT
                    FROM dbo.StaffTbl S
                    LEFT JOIN dbo.AttendanceTbl A
                        ON S.StaffID = A.StaffID
                        AND A.[Date] = @Date
                    WHERE S.StaffID = @StaffID
                    AND S.BranchID = @BranchID;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        StaffID = selectedStaffID,
                        BranchID = branchid.Trim(),
                        Date = DateTime.Today
                    }
                );

                if (dt.Rows.Count == 0)
                    return;

                DataRow row = dt.Rows[0];

                string status =
                    Convert.ToString(row["Status"]) ?? "";

                if (!status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
                    return;

                bool hasTimeIn = row["Time_IN"] != DBNull.Value;
                bool hasTimeOut = row["Time_OUT"] != DBNull.Value;

                if (!hasTimeIn)
                {
                    btn_timein.Enabled = true;
                    btn_timeout.Enabled = false;
                }
                else if (!hasTimeOut)
                {
                    btn_timein.Enabled = false;
                    btn_timeout.Enabled = true;
                }
                else
                {
                    btn_timein.Enabled = false;
                    btn_timeout.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update attendance buttons:\n" + ex.Message,
                    "Attendance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // LOAD STAFF
        // =====================================================

        private void LoadStaff()
        {
            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(branchid))
                {
                    dgv_staff.DataSource = null;
                    return;
                }

                const string query = @"
                    SELECT
                        StaffID,
                        FullName,
                        Gender,
                        ContactNumber,
                        Email,
                        Adress,
                        JobPosition,
                        Datehired,
                        SalaryRate,
                        Status
                    FROM dbo.StaffTbl
                    WHERE BranchID = @BranchID
                    ORDER BY FullName ASC;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new { BranchID = branchid.Trim() }
                );

                dgv_staff.DataSource = null;
                dgv_staff.AutoGenerateColumns = true;
                dgv_staff.DataSource = dt;

                if (dgv_staff.Columns.Count == 0)
                    return;

                dgv_staff.Columns["StaffID"].Visible = true;
                dgv_staff.Columns["FullName"].Visible = true;
                dgv_staff.Columns["Datehired"].Visible = true;
                dgv_staff.Columns["JobPosition"].Visible = true;
                dgv_staff.Columns["Status"].Visible = true;

                dgv_staff.Columns["Gender"].Visible = false;
                dgv_staff.Columns["ContactNumber"].Visible = false;
                dgv_staff.Columns["Email"].Visible = false;
                dgv_staff.Columns["Adress"].Visible = false;
                dgv_staff.Columns["SalaryRate"].Visible = false;

                dgv_staff.Columns["StaffID"].HeaderText = "Staff ID";
                dgv_staff.Columns["FullName"].HeaderText = "Name";
                dgv_staff.Columns["Datehired"].HeaderText = "Date Hired";
                dgv_staff.Columns["JobPosition"].HeaderText = "Position";
                dgv_staff.Columns["Status"].HeaderText = "Status";

                dgv_staff.Columns["StaffID"].DisplayIndex = 0;
                dgv_staff.Columns["FullName"].DisplayIndex = 1;
                dgv_staff.Columns["Datehired"].DisplayIndex = 2;
                dgv_staff.Columns["JobPosition"].DisplayIndex = 3;
                dgv_staff.Columns["Status"].DisplayIndex = 4;

                dgv_staff.Columns["Datehired"]
                    .DefaultCellStyle.Format = "MMM dd, yyyy";

                dgv_staff.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.None;

                dgv_staff.AllowUserToResizeColumns = false;
                dgv_staff.AllowUserToResizeRows = false;

                dgv_staff.EnableHeadersVisualStyles = false;
                dgv_staff.ColumnHeadersVisible = true;
                dgv_staff.ColumnHeadersHeight = 35;

                dgv_staff.ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                dgv_staff.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                dgv_staff.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load staff:\n" + ex.Message,
                    "Load Staff Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // DASHBOARD COUNTS
        // =====================================================

        private void LoadStaffCounts()
        {
            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(branchid))
                {
                    Display_TotaoRoom.Text = "0";
                    Display_Active.Text = "0";
                    Display_Inactive.Text = "0";
                    return;
                }

                const string totalQuery = @"
                    SELECT COUNT(*)
                    FROM dbo.StaffTbl
                    WHERE BranchID = @BranchID;";

                const string activeQuery = @"
                    SELECT COUNT(*)
                    FROM dbo.StaffTbl
                    WHERE BranchID = @BranchID
                    AND Status = 'Active';";

                const string inactiveQuery = @"
                    SELECT COUNT(*)
                    FROM dbo.StaffTbl
                    WHERE BranchID = @BranchID
                    AND Status = 'Inactive';";

                var parameters = new
                {
                    BranchID = branchid.Trim()
                };

                Display_TotaoRoom.Text = Convert.ToString(
                    AppDB.Instance.Scalar(totalQuery, parameters));

                Display_Active.Text = Convert.ToString(
                    AppDB.Instance.Scalar(activeQuery, parameters));

                Display_Inactive.Text = Convert.ToString(
                    AppDB.Instance.Scalar(inactiveQuery, parameters));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load staff counts:\n" + ex.Message,
                    "Count Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // GET GENDER VALUE
        // =====================================================

        private string GetGenderValue()
        {
            if (Male_RB.Checked)
                return "M";

            if (Female_RB.Checked)
                return "F";

            return "";
        }

        // =====================================================
        // DISPLAY GENDER VALUE
        // =====================================================

        private string GetGenderText(object value)
        {
            string gender = Convert.ToString(value) ?? "";

            if (gender.Trim().Equals(
                "M",
                StringComparison.OrdinalIgnoreCase))
                return "Male";

            if (gender.Trim().Equals(
                "F",
                StringComparison.OrdinalIgnoreCase))
                return "Female";

            return "";
        }

        // =====================================================
        // GET CONTACT NUMBER
        // =====================================================

        private string GetContactDigits()
        {
            string contact = txt_contact.Text.Trim();
            string digits = "";

            foreach (char c in contact)
            {
                if (char.IsDigit(c))
                    digits += c;
            }

            return digits;
        }

        // =====================================================
        // VALIDATE STAFF INPUT
        // =====================================================

        private bool ValidateStaffInput()
        {
            if (string.IsNullOrWhiteSpace(txt_fullname.Text))
            {
                MessageBox.Show("Please enter Full Name.");
                txt_fullname.Focus();
                return false;
            }

            if (txt_fullname.Text.Trim().Length > 100)
            {
                MessageBox.Show(
                    "Full Name cannot exceed 100 characters.");
                txt_fullname.Focus();
                return false;
            }

            if (!Male_RB.Checked && !Female_RB.Checked)
            {
                MessageBox.Show("Please select Gender.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_contact.Text))
            {
                MessageBox.Show("Please enter Contact Number.");
                txt_contact.Focus();
                return false;
            }

            string contactDigits = GetContactDigits();

            if (contactDigits.Length < 7 ||
                contactDigits.Length > 12)
            {
                MessageBox.Show(
                    "Contact Number must contain 7 to 12 digits.");
                txt_contact.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_email.Text))
            {
                MessageBox.Show("Please enter Email Address.");
                txt_email.Focus();
                return false;
            }

            string emailText = txt_email.Text.Trim();

            if (emailText.Length > 40)
            {
                MessageBox.Show(
                    "Email cannot exceed 40 characters.");
                txt_email.Focus();
                return false;
            }

            try
            {
                MailAddress email = new MailAddress(emailText);

                if (email.Address != emailText)
                {
                    MessageBox.Show(
                        "Please enter a valid Email Address.");
                    txt_email.Focus();
                    return false;
                }
            }
            catch
            {
                MessageBox.Show(
                    "Please enter a valid Email Address.");
                txt_email.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_address.Text))
            {
                MessageBox.Show("Please enter Address.");
                txt_address.Focus();
                return false;
            }

            if (txt_address.Text.Trim().Length > 100)
            {
                MessageBox.Show(
                    "Address cannot exceed 100 characters.");
                txt_address.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cb_job.Text))
            {
                MessageBox.Show("Please select Job Position.");
                cb_job.Focus();
                return false;
            }

            if (cb_job.Text.Trim().Length > 30)
            {
                MessageBox.Show(
                    "Job Position cannot exceed 30 characters.");
                cb_job.Focus();
                return false;
            }

            return true;
        }

        // =====================================================
        // CHECK DUPLICATE STAFF ID
        // =====================================================

        private bool StaffIDExists(string staffID)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM dbo.StaffTbl
                WHERE StaffID = @StaffID;";

            int count = Convert.ToInt32(
                AppDB.Instance.Scalar(
                    query,
                    new { StaffID = staffID }
                )
            );

            return count > 0;
        }

        // =====================================================
        // ADD STAFF
        // =====================================================

        private void btn_add_staff_Click(object sender, EventArgs e)
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(branchid))
            {
                MessageBox.Show("No Branch ID was found.");
                return;
            }

            if (!ValidateStaffInput())
                return;

            try
            {
                string staffID =
                    IDGenerator.GenerateNextID("STF", branchid);

                if (string.IsNullOrWhiteSpace(staffID) ||
                    staffID.Length > 20)
                {
                    MessageBox.Show(
                        "Unable to generate a valid Staff ID.",
                        "Staff Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                if (StaffIDExists(staffID))
                {
                    MessageBox.Show(
                        "Staff ID already exists. Please try again.",
                        "Duplicate Staff ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                const string query = @"
                    INSERT INTO dbo.StaffTbl
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
                        NULL,
                        NULL,
                        'Inactive'
                    );";

                var parameters = new
                {
                    StaffID = staffID,
                    BranchID = branchid.Trim(),
                    FullName = txt_fullname.Text.Trim(),
                    ContactNumber = GetContactDigits(),
                    Email = txt_email.Text.Trim(),
                    Adress = txt_address.Text.Trim(),
                    Gender = GetGenderValue(),
                    JobPosition = cb_job.Text.Trim()
                };

                int affected = AppDB.Instance.CUD(query, parameters);

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff added successfully!\nStaff ID: " + staffID +
                        "\n\nWaiting for System Admin activation.",
                        "Staff Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadStaff();
                    LoadStaffCounts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "Staff was not added. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to add staff:\n" + ex.Message,
                    "Add Staff Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // SELECT STAFF FROM STAFF GRID
        // =====================================================

        private void dgv_staff_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = dgv_staff.Rows[e.RowIndex];

                if (row.IsNewRow)
                    return;

                selectedStaffID =
                    Convert.ToString(row.Cells["StaffID"].Value) ?? "";

                if (string.IsNullOrWhiteSpace(selectedStaffID))
                    return;

                txt_staffID.Text = selectedStaffID;

                txt_fullname.Text =
                    Convert.ToString(row.Cells["FullName"].Value) ?? "";

                string gender = GetGenderText(
                    row.Cells["Gender"].Value);

                Male_RB.Checked = gender == "Male";
                Female_RB.Checked = gender == "Female";

                txt_contact.Text =
                    Convert.ToString(row.Cells["ContactNumber"].Value) ?? "";

                txt_email.Text =
                    Convert.ToString(row.Cells["Email"].Value) ?? "";

                txt_address.Text =
                    Convert.ToString(row.Cells["Adress"].Value) ?? "";

                cb_job.Text =
                    Convert.ToString(row.Cells["JobPosition"].Value) ?? "";

                object salaryValue = row.Cells["SalaryRate"].Value;

                txt_salary.Text =
                    salaryValue == null || salaryValue == DBNull.Value
                        ? ""
                        : Convert.ToString(salaryValue);

                object dateValue = row.Cells["Datehired"].Value;

                if (dateValue != null && dateValue != DBNull.Value)
                    selectedDateHired = Convert.ToDateTime(dateValue);
                else
                    selectedDateHired = null;

                isEditing = false;

                txt_staffID.Enabled = false;
                btn_add_staff.Enabled = false;
                btn_ud.Enabled = false;

                SetFieldsEnabled(false);

                DisplayRightPanel();

                LoadSelectedStaffAttendance();

                UpdateAttendanceButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to display staff information:\n" + ex.Message,
                    "Staff Selection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // RIGHT PANEL DETAILS
        // =====================================================

        private void DisplayRightPanel()
        {
            lbl_name.Text = txt_fullname.Text;
            lbl_jbp.Text = cb_job.Text;
            lbl__contact.Text = txt_contact.Text;
            lbl_email.Text = txt_email.Text;
            lbl_location.Text = txt_address.Text;

            lbl_gender.Text = GetGenderValue() == "M"
                ? "Male"
                : GetGenderValue() == "F"
                    ? "Female"
                    : "";

            lbl_status.Text =
                Convert.ToString(
                    dgv_staff.CurrentRow?.Cells["Status"].Value) ?? "";

            lbl_date_started.Text = selectedDateHired.HasValue
                ? selectedDateHired.Value.ToString("MMMM dd, yyyy")
                : "Not yet hired";

            lbl_salary.Text = txt_salary.Text;
        }

        // =====================================================
        // ENABLE / DISABLE FIELDS
        // =====================================================

        private void SetFieldsEnabled(bool enabled)
        {
            txt_fullname.Enabled = enabled;

            Male_RB.Enabled = enabled;
            Female_RB.Enabled = enabled;

            txt_contact.Enabled = enabled;
            txt_email.Enabled = enabled;
            txt_address.Enabled = enabled;
            cb_job.Enabled = enabled;

            // Date Hired is controlled by System Admin.
            txt_salary.Enabled = enabled;

            txt_staffID.Enabled =
                string.IsNullOrWhiteSpace(selectedStaffID);
        }

        // =====================================================
        // EDIT STAFF
        // =====================================================

        private void btn_edit_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff record first.");
                return;
            }

            isEditing = true;

            txt_staffID.Enabled = false;
            btn_add_staff.Enabled = false;
            btn_ud.Enabled = true;

            SetFieldsEnabled(true);
        }

        // =====================================================
        // UPDATE STAFF
        // =====================================================

        private void btn_ud_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff record first.");
                return;
            }

            if (!isEditing)
            {
                MessageBox.Show(
                    "Please click Edit before updating.");
                return;
            }

            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(branchid))
            {
                MessageBox.Show("No Branch ID was found.");
                return;
            }

            if (!ValidateStaffInput())
                return;

            try
            {
                decimal? salary = null;

                if (!string.IsNullOrWhiteSpace(txt_salary.Text))
                {
                    if (!decimal.TryParse(
                        txt_salary.Text.Trim(),
                        NumberStyles.Number,
                        CultureInfo.CurrentCulture,
                        out decimal salaryValue))
                    {
                        MessageBox.Show(
                            "Please enter a valid Salary Rate.");

                        txt_salary.Focus();
                        return;
                    }

                    salary = salaryValue;
                }

                // Datehired is intentionally NOT updated here.
                const string query = @"
                    UPDATE dbo.StaffTbl
                    SET
                        FullName = @FullName,
                        ContactNumber = @ContactNumber,
                        Email = @Email,
                        Adress = @Adress,
                        Gender = @Gender,
                        JobPosition = @JobPosition,
                        SalaryRate = @SalaryRate
                    WHERE StaffID = @StaffID
                    AND BranchID = @BranchID;";

                var parameters = new
                {
                    StaffID = selectedStaffID,
                    BranchID = branchid.Trim(),
                    FullName = txt_fullname.Text.Trim(),
                    ContactNumber = GetContactDigits(),
                    Email = txt_email.Text.Trim(),
                    Adress = txt_address.Text.Trim(),
                    Gender = GetGenderValue(),
                    JobPosition = cb_job.Text.Trim(),
                    SalaryRate = salary
                };

                int affected = AppDB.Instance.CUD(query, parameters);

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff updated successfully!",
                        "Staff Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    LoadStaff();
                    LoadStaffCounts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "No staff record was updated.",
                        "Update Staff",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update staff:\n" + ex.Message,
                    "Update Staff Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // CLEAR FORM
        // =====================================================

        private void ClearFields()
        {
            selectedStaffID = "";
            selectedDateHired = null;

            attendanceid = "";
            timeIn = null;
            timeOut = null;

            isEditing = false;

            txt_staffID.Clear();
            txt_fullname.Clear();
            txt_contact.Clear();
            txt_email.Clear();
            txt_address.Clear();
            txt_salary.Clear();

            Male_RB.Checked = false;
            Female_RB.Checked = false;

            cb_job.SelectedIndex = -1;

            txt_staffID.Enabled = true;

            btn_add_staff.Enabled = true;
            btn_ud.Enabled = false;

            SetFieldsEnabled(true);

            lbl_name.Text = "";
            lbl_jbp.Text = "";
            lbl__contact.Text = "";
            lbl_email.Text = "";
            lbl_location.Text = "";
            lbl_gender.Text = "";
            lbl_status.Text = "";
            lbl_date_started.Text = "Not yet hired";
            lbl_salary.Text = "";

            lbl_timein.Text = "Not recorded";
            lbl_timeout.Text = "Not recorded";

            btn_timein.Enabled = false;
            btn_timeout.Enabled = false;

            dgv_staff.ClearSelection();
            DGV_Attendance.ClearSelection();
        }

        // =====================================================
        // SELECT ATTENDANCE RECORD
        // =====================================================

        private void attendance_celllClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = DGV_Attendance.Rows[e.RowIndex];

                if (row.IsNewRow)
                    return;

                attendanceid =
                    Convert.ToString(row.Cells["AttendanceID"].Value) ?? "";

                selectedStaffID =
                    Convert.ToString(row.Cells["StaffID"].Value) ?? "";

                DateTime attendanceDate =
                    Convert.ToDateTime(row.Cells["Date"].Value);

                timeIn = ConvertToTimeOnly(
                    row.Cells["Time_IN"].Value);

                timeOut = ConvertToTimeOnly(
                    row.Cells["Time_OUT"].Value);

                lbl_timein.Text =
                    timeIn?.ToString("hh:mm tt") ?? "Not recorded";

                lbl_timeout.Text =
                    timeOut?.ToString("hh:mm tt") ?? "Not recorded";

                // Historical attendance is view-only.
                if (attendanceDate.Date != DateTime.Today)
                {
                    btn_timein.Enabled = false;
                    btn_timeout.Enabled = false;
                    return;
                }

                UpdateAttendanceButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to display attendance details:\n" +
                    ex.Message,
                    "Attendance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // TIME IN
        // =====================================================

        private void btn_timein_Click(object sender, EventArgs e)
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff member first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(branchid))
            {
                MessageBox.Show("No Branch ID was found.");
                return;
            }

            try
            {
                DateTime now = DateTime.Now;

                const string query = @"
                    UPDATE A
                    SET A.Time_IN = @Time_IN
                    FROM dbo.AttendanceTbl A
                    INNER JOIN dbo.StaffTbl S
                        ON A.StaffID = S.StaffID
                    WHERE A.StaffID = @StaffID
                    AND S.BranchID = @BranchID
                    AND S.Status = 'Active'
                    AND A.[Date] = @Date
                    AND A.Time_IN IS NULL
                    AND A.Time_OUT IS NULL;";

                int affected = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        StaffID = selectedStaffID,
                        BranchID = branchid.Trim(),
                        Date = DateTime.Today,
                        Time_IN = now.TimeOfDay
                    }
                );

                if (affected > 0)
                {
                    LoadSelectedStaffAttendance();

                    MessageBox.Show(
                        "Time In recorded successfully!\n" +
                        "Time: " + now.ToString("hh:mm tt"),
                        "Attendance",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    loadattendance();
                    UpdateAttendanceButtons();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to record Time In.\n\n" +
                        "Please check the staff status and today's attendance record.",
                        "Time In",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    LoadSelectedStaffAttendance();
                    UpdateAttendanceButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error recording Time In:\n" + ex.Message,
                    "Time In Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // TIME OUT
        // =====================================================

        private void btn_timeout_Click(object sender, EventArgs e)
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show(
                    "Please select a staff member first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(branchid))
            {
                MessageBox.Show("No Branch ID was found.");
                return;
            }

            try
            {
                DateTime now = DateTime.Now;

                const string query = @"
                    UPDATE A
                    SET A.Time_OUT = @Time_OUT
                    FROM dbo.AttendanceTbl A
                    INNER JOIN dbo.StaffTbl S
                        ON A.StaffID = S.StaffID
                    WHERE A.StaffID = @StaffID
                    AND S.BranchID = @BranchID
                    AND S.Status = 'Active'
                    AND A.[Date] = @Date
                    AND A.Time_IN IS NOT NULL
                    AND A.Time_OUT IS NULL;";

                int affected = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        StaffID = selectedStaffID,
                        BranchID = branchid.Trim(),
                        Date = DateTime.Today,
                        Time_OUT = now.TimeOfDay
                    }
                );

                if (affected > 0)
                {
                    LoadSelectedStaffAttendance();

                    MessageBox.Show(
                        "Time Out recorded successfully!\n" +
                        "Time: " + now.ToString("hh:mm tt"),
                        "Attendance",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    loadattendance();
                    UpdateAttendanceButtons();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to record Time Out.\n\n" +
                        "Make sure the staff member has already recorded Time In.",
                        "Time Out",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    LoadSelectedStaffAttendance();
                    UpdateAttendanceButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error recording Time Out:\n" + ex.Message,
                    "Time Out Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =====================================================
        // OTHER EVENTS
        // =====================================================

        private void guna2Panel11_Paint(
            object sender,
            PaintEventArgs e)
        {
        }

        private void dgv_staff_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
        }

        private void guna2Panel10_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}