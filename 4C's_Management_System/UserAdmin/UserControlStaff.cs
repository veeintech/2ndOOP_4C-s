
using System;
using System.Data;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControlStaff : UserControl
    {
        private string selectedStaffID = "";

        public UserControlStaff()
        {
            InitializeComponent();

            this.Load -= UserControlStaff_Load;
            this.Load += UserControlStaff_Load;

            btn_add_staff.Click -= btn_add_staff_Click;
            btn_add_staff.Click += btn_add_staff_Click;

            btn_edit.Click -= btn_edit_Click;
            btn_edit.Click += btn_edit_Click;

            btn_ud.Click -= btn_ud_Click;
            btn_ud.Click += btn_ud_Click;

            dgv_staff.CellClick -= dgv_staff_CellClick;
            dgv_staff.CellClick += dgv_staff_CellClick;
        }

        // =========================
        // FORM LOAD
        // =========================
        private void UserControlStaff_Load(object sender, EventArgs e)
        {
            cb_gender.Items.Clear();
            cb_gender.Items.AddRange(new object[]
            {
                "Male", "Female"
            });

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

            cb_empstat.Items.Clear();
            cb_empstat.Items.AddRange(new object[]
            {
                "Active", "Inactive"
            });

            ClearFields();
            LoadStaff();
            LoadStaffCounts();
        }

        // =========================
        // LOAD STAFF INTO DGV
        // =========================
        private void LoadStaff()
        {
            try
            {
                string query = @"
            SELECT StaffID, FullName, Gender, Contact,
                   Email, Address, JobPosition, DateHired,
                   SalaryRate, EmploymentStatus
            FROM dbo.StaffTbl
            ORDER BY FullName ASC";

                DataTable dt = AppDB.Instance.TableData(query);

                dgv_staff.DataSource = null;
                dgv_staff.AutoGenerateColumns = true;
                dgv_staff.DataSource = dt;

                // Show only five columns
                dgv_staff.Columns["StaffID"].Visible = true;
                dgv_staff.Columns["FullName"].Visible = true;
                dgv_staff.Columns["DateHired"].Visible = true;
                dgv_staff.Columns["JobPosition"].Visible = true;
                dgv_staff.Columns["EmploymentStatus"].Visible = true;

                // Hide other columns
                dgv_staff.Columns["Gender"].Visible = false;
                dgv_staff.Columns["Contact"].Visible = false;
                dgv_staff.Columns["Email"].Visible = false;
                dgv_staff.Columns["Address"].Visible = false;
                dgv_staff.Columns["SalaryRate"].Visible = false;

                // Header names
                dgv_staff.Columns["StaffID"].HeaderText = "Staff ID";
                dgv_staff.Columns["FullName"].HeaderText = "Name";
                dgv_staff.Columns["DateHired"].HeaderText = "Date Hired";
                dgv_staff.Columns["JobPosition"].HeaderText = "Position";
                dgv_staff.Columns["EmploymentStatus"].HeaderText = "Status";

                // Column order
                dgv_staff.Columns["StaffID"].DisplayIndex = 0;
                dgv_staff.Columns["FullName"].DisplayIndex = 1;
                dgv_staff.Columns["DateHired"].DisplayIndex = 2;
                dgv_staff.Columns["JobPosition"].DisplayIndex = 3;
                dgv_staff.Columns["EmploymentStatus"].DisplayIndex = 4;

                dgv_staff.Columns["DateHired"].DefaultCellStyle.Format = "MMM dd, yyyy";

                dgv_staff.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;



                // DISABLE RESIZING
                dgv_staff.AllowUserToResizeColumns = false;
                dgv_staff.AllowUserToResizeRows = false;

                // COLUMN HEADERS
                dgv_staff.EnableHeadersVisualStyles = false;
                dgv_staff.ColumnHeadersVisible = true;
                dgv_staff.ColumnHeadersHeight = 35;
                dgv_staff.ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                // RESIZING
                dgv_staff.AllowUserToResizeColumns = false;
                dgv_staff.AllowUserToResizeRows = false;

                // HEADER TEXT
                dgv_staff.ColumnHeadersDefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;

                dgv_staff.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load staff:\n" + ex.Message);
            }
        }

        // =========================
        // DASHBOARD COUNTS
        // =========================
        private void LoadStaffCounts()
        {
            try
            {
                Display_TotaoRoom.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM dbo.StaffTbl"
                    )
                );

                Display_Active.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM dbo.StaffTbl WHERE EmploymentStatus = 'Active'"
                    )
                );

                Display_Inactive.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM dbo.StaffTbl WHERE EmploymentStatus = 'Inactive'"
                    )
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Count Error: " + ex.Message);
            }
        }

        // =========================
        // ADD STAFF
        // =========================
        private void btn_add_staff_Click(object sender, EventArgs e)
        {
            string staffID = txt_staffID.Text.Trim();
            string fullName = txt_fullname.Text.Trim();

            if (string.IsNullOrWhiteSpace(staffID))
            {
                MessageBox.Show("Please enter Staff ID.");
                txt_staffID.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Please enter Full Name.");
                txt_fullname.Focus();
                return;
            }

            decimal? salary = null;

            if (!string.IsNullOrWhiteSpace(txt_salary.Text))
            {
                if (!decimal.TryParse(txt_salary.Text, out decimal parsedSalary))
                {
                    MessageBox.Show("Please enter a valid Salary Rate.");
                    txt_salary.Focus();
                    return;
                }

                salary = parsedSalary;
            }

            string employmentStatus = string.IsNullOrWhiteSpace(cb_empstat.Text)
                ? "Active"
                : cb_empstat.Text;

            try
            {
                string query = @"
                    INSERT INTO dbo.StaffTbl
                    (
                        StaffID,
                        FullName,
                        Gender,
                        Contact,
                        Email,
                        Address,
                        JobPosition,
                        DateHired,
                        SalaryRate,
                        EmploymentStatus
                    )
                    VALUES
                    (
                        @StaffID,
                        @FullName,
                        @Gender,
                        @Contact,
                        @Email,
                        @Address,
                        @JobPosition,
                        @DateHired,
                        @SalaryRate,
                        @EmploymentStatus
                    )";

                var parameters = new
                {
                    StaffID = staffID,
                    FullName = fullName,
                    Gender = cb_gender.Text,
                    Contact = txt_contact.Text.Trim(),
                    Email = txt_email.Text.Trim(),
                    Address = txt_address.Text.Trim(),
                    JobPosition = cb_job.Text,
                    DateHired = dtp_date_hired.Value,
                    SalaryRate = salary,
                    EmploymentStatus = employmentStatus
                };

                int affected = AppDB.Instance.CUD(query, parameters);

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Staff added successfully!",
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
                    MessageBox.Show("Staff was not added.");
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

        // =========================
        // SELECT STAFF FROM DGV
        // =========================
        private void dgv_staff_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgv_staff.Rows[e.RowIndex];

            selectedStaffID = Convert.ToString(
                row.Cells["StaffID"].Value
            ) ?? "";

            // BOTTOM PANEL
            txt_staffID.Text = selectedStaffID;
            txt_fullname.Text = Convert.ToString(row.Cells["FullName"].Value);
            cb_gender.Text = Convert.ToString(row.Cells["Gender"].Value);
            txt_contact.Text = Convert.ToString(row.Cells["Contact"].Value);
            txt_email.Text = Convert.ToString(row.Cells["Email"].Value);
            txt_address.Text = Convert.ToString(row.Cells["Address"].Value);
            cb_job.Text = Convert.ToString(row.Cells["JobPosition"].Value);
            cb_empstat.Text = Convert.ToString(row.Cells["EmploymentStatus"].Value);
            txt_salary.Text = Convert.ToString(row.Cells["SalaryRate"].Value);

            if (row.Cells["DateHired"].Value != DBNull.Value &&
                DateTime.TryParse(
                    Convert.ToString(row.Cells["DateHired"].Value),
                    out DateTime hiredDate))
            {
                dtp_date_hired.Value = hiredDate;
            }

            // RIGHT PANEL
            DisplayRightPanel();

            txt_staffID.Enabled = false;
            btn_add_staff.Enabled = false;
            btn_ud.Enabled = false;
        }

        // =========================
        // RIGHT PANEL DETAILS
        // =========================
        private void DisplayRightPanel()
        {
            lbl_name.Text = txt_fullname.Text;
            lbl_jbp.Text = cb_job.Text;
            lbl__contact.Text = txt_contact.Text;
            lbl_email.Text = txt_email.Text;
            lbl_location.Text = txt_address.Text;
            lbl_gender.Text = cb_gender.Text;
            lbl_status.Text = cb_empstat.Text;

            lbl_date_started.Text =
                dtp_date_hired.Value.ToString("MMMM dd, yyyy");

            lbl_salary.Text = txt_salary.Text;
        }

        // =========================
        // EDIT STAFF
        // =========================
        private void btn_edit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a staff record first.");
                return;
            }

            txt_staffID.Enabled = false;
            btn_add_staff.Enabled = false;
            btn_ud.Enabled = true;

            txt_fullname.Enabled = true;
            cb_gender.Enabled = true;
            txt_contact.Enabled = true;
            txt_email.Enabled = true;
            txt_address.Enabled = true;
            cb_job.Enabled = true;
            dtp_date_hired.Enabled = true;
            txt_salary.Enabled = true;
            cb_empstat.Enabled = true;
        }

        // =========================
        // UPDATE STAFF
        // =========================
        private void btn_ud_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedStaffID))
            {
                MessageBox.Show("Please select a staff record first.");
                return;
            }

            decimal? salary = null;

            if (!string.IsNullOrWhiteSpace(txt_salary.Text))
            {
                if (!decimal.TryParse(txt_salary.Text, out decimal parsedSalary))
                {
                    MessageBox.Show("Please enter a valid Salary Rate.");
                    txt_salary.Focus();
                    return;
                }

                salary = parsedSalary;
            }

            try
            {
                string query = @"
                    UPDATE dbo.StaffTbl
                    SET FullName = @FullName,
                        Gender = @Gender,
                        Contact = @Contact,
                        Email = @Email,
                        Address = @Address,
                        JobPosition = @JobPosition,
                        DateHired = @DateHired,
                        SalaryRate = @SalaryRate,
                        EmploymentStatus = @EmploymentStatus
                    WHERE StaffID = @StaffID";

                var parameters = new
                {
                    StaffID = selectedStaffID,
                    FullName = txt_fullname.Text.Trim(),
                    Gender = cb_gender.Text,
                    Contact = txt_contact.Text.Trim(),
                    Email = txt_email.Text.Trim(),
                    Address = txt_address.Text.Trim(),
                    JobPosition = cb_job.Text,
                    DateHired = dtp_date_hired.Value,
                    SalaryRate = salary,
                    EmploymentStatus = cb_empstat.Text
                };

                int affected = AppDB.Instance.CUD(query, parameters);

                if (affected > 0)
                {
                    MessageBox.Show("Staff updated successfully!");

                    LoadStaff();
                    LoadStaffCounts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("No staff record was updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update staff:\n" + ex.Message
                );
            }
        }

        // =========================
        // CLEAR FORM
        // =========================
        private void ClearFields()
        {
            selectedStaffID = "";

            txt_staffID.Clear();
            txt_fullname.Clear();
            txt_contact.Clear();
            txt_email.Clear();
            txt_address.Clear();
            txt_salary.Clear();

            cb_gender.SelectedIndex = -1;
            cb_job.SelectedIndex = -1;
            cb_empstat.SelectedIndex = -1;

            dtp_date_hired.Value = DateTime.Today;

            txt_staffID.Enabled = true;
            btn_add_staff.Enabled = true;
            btn_ud.Enabled = false;

            txt_fullname.Enabled = true;
            cb_gender.Enabled = true;
            txt_contact.Enabled = true;
            txt_email.Enabled = true;
            txt_address.Enabled = true;
            cb_job.Enabled = true;
            dtp_date_hired.Enabled = true;
            txt_salary.Enabled = true;
            cb_empstat.Enabled = true;

            lbl_name.Text = "";
            lbl_jbp.Text = "";
            lbl__contact.Text = "";
            lbl_email.Text = "";
            lbl_location.Text = "";
            lbl_gender.Text = "";
            lbl_status.Text = "";
            lbl_date_started.Text = "";
            lbl_salary.Text = "";
        }

        private void guna2Panel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgv_staff_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
