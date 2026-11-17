
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_PROFILE : UserControl
    {
        private FlowLayoutPanel taskListPanel;

        public UserControl_Admin_PROFILE()
        {
            InitializeComponent();

            // DATE AND TIME TIMER
            RM_Timer.Interval = 1000;
            RM_Timer.Tick -= RM_Timer_Tick;
            RM_Timer.Tick += RM_Timer_Tick;
        }

        // ==========================================
        // USER CONTROL LOAD
        // ==========================================

        private void UserControl_Admin_PROFILE_Load(object sender, EventArgs e)
        {
            // DATE AND TIME COLORS
            RM_Panel_DateTime.FillColor =
                ColorTranslator.FromHtml("#34465A");

            RM_DateTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");


            RM_DateLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            RM_TimeLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            UpdateDateTime();
            RM_Timer.Start();

            // LOAD ADMIN AND BRANCH INFORMATION
            LoadAdminInformation();
            LoadBranchOperations();
            LoadTaskManager();
        }

        // ==========================================
        // DATE AND TIME
        // ==========================================

        private void RM_Timer_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            RM_DateLbl.Text = DateTime.Now.ToString("MMM dd, yyyy");
            RM_TimeLbl.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        // ==========================================
        // ADMIN INFORMATION
        // ==========================================

        private void LoadAdminInformation()
        {
            try
            {
                string staffId = AccessBranch_ByForm.StaffId;
                string branchId = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(staffId) ||
                    string.IsNullOrWhiteSpace(branchId))
                {
                    MessageBox.Show(
                        "Admin session information is missing. Please log in again.",
                        "Session Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                const string query = @"
                    SELECT
                        S.StaffID,
                        S.FullName,
                        S.Gender,
                        S.ContactNumber,
                        S.Email,
                        S.Adress,
                        S.JobPosition,
                        S.Datehired,
                        S.SalaryRate,
                        S.Status,
                        U.Status AS AccountStatus
                    FROM StaffTbl S
                    INNER JOIN UserTbl U
                        ON S.StaffID = U.StaffId
                    WHERE S.StaffID = @StaffId
                    AND S.BranchID = @BranchId
                    AND U.BranchId = @BranchId";

                DataTable result = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        StaffId = staffId,
                        BranchId = branchId
                    }
                );

                if (result.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Admin information was not found.",
                        "No Record",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    return;
                }

                DataRow row = result.Rows[0];

                // PERSONAL INFORMATION

                guna2HtmlLabel41.Text =
                    Convert.ToString(row["FullName"]) ?? "";

                guna2HtmlLabel42.Text =
                    Convert.ToString(row["StaffID"]) ?? "";

                string gender =
                    Convert.ToString(row["Gender"]) ?? "";

                guna2HtmlLabel43.Text =
                    gender.Equals("M", StringComparison.OrdinalIgnoreCase)
                        ? "Male"
                        : gender.Equals("F", StringComparison.OrdinalIgnoreCase)
                            ? "Female"
                            : gender;

                guna2HtmlLabel39.Text =
                    Convert.ToString(row["ContactNumber"]) ?? "";

                guna2HtmlLabel38.Text =
                    Convert.ToString(row["Email"]) ?? "";

                guna2HtmlLabel40.Text =
                    Convert.ToString(row["Adress"]) ?? "";

                // EMPLOYMENT INFORMATION

                guna2HtmlLabel45.Text =
                    Convert.ToString(row["JobPosition"]) ?? "";

                guna2HtmlLabel47.Text =
                    row["Datehired"] == DBNull.Value
                        ? "N/A"
                        : Convert.ToDateTime(row["Datehired"])
                            .ToString("MMM dd, yyyy");

                guna2HtmlLabel46.Text =
                    row["SalaryRate"] == DBNull.Value
                        ? "N/A"
                        : Convert.ToDecimal(row["SalaryRate"])
                            .ToString("₱#,##0.00");

                guna2HtmlLabel44.Text =
                    Convert.ToString(row["Status"]) ?? "";

                // ADMIN ACCOUNT STATUS

                string accountStatus =
                    Convert.ToString(row["AccountStatus"]) ?? "";

                UpdateAccountStatus(accountStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load admin information:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==========================================
        // ADMIN ACCOUNT STATUS
        // ==========================================

        private void UpdateAccountStatus(string status)
        {
            bool isActive = status.Equals(
                "Active",
                StringComparison.OrdinalIgnoreCase
            );

            guna2Panel12.FillColor = isActive
                ? Color.FromArgb(192, 255, 192)
                : Color.Firebrick;
        }

        // ==========================================
        // BRANCH OPERATIONS
        // ==========================================

        private void LoadBranchOperations()
        {
            try
            {
                string branchId = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(branchId))
                    return;

                // TOTAL ROOMS
                guna2HtmlLabel48.Text = GetCount(@"
                    SELECT COUNT(*)
                    FROM TBLRoom
                    WHERE BranchId = @BranchId",
                    branchId
                ).ToString();

                // AVAILABLE ROOMS
                guna2HtmlLabel50.Text = GetCount(@"
                    SELECT COUNT(*)
                    FROM TBLRoom
                    WHERE BranchId = @BranchId
                    AND Status = 'Available'",
                    branchId
                ).ToString();

                // ACTIVE CUSTOMERS
                guna2HtmlLabel16.Text = GetCount(@"
                    SELECT COUNT(*)
                    FROM CustomerTbl
                    WHERE BranchId = @BranchId
                    AND CustomerStatus = 'Active'",
                    branchId
                ).ToString();

                // TOTAL RESERVATIONS
                guna2HtmlLabel14.Text = GetCount(@"
                    SELECT COUNT(*)
                    FROM ReservationTbl
                    WHERE branchId = @BranchId",
                    branchId
                ).ToString();

                // FINANCIAL OVERVIEW
                decimal totalPayments = GetDecimal(@"
                    SELECT ISNULL(SUM(AmountPaid), 0)
                    FROM PaymentTbl
                    WHERE BranchID = @BranchId",
                    branchId
                );

                guna2HtmlLabel60.Text =
                    totalPayments.ToString("₱#,##0.00");

                // STAFF OVERVIEW
                guna2HtmlLabel20.Text = GetCount(@"
                    SELECT COUNT(*)
                    FROM StaffTbl
                    WHERE BranchID = @BranchId",
                    branchId
                ).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load branch operations:\n" + ex.Message,
                    "Branch Information Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==========================================
        // GET COUNT
        // ==========================================

        private int GetCount(string query, string branchId)
        {
            DataTable result = AppDB.Instance.TableData(
                query,
                new { BranchId = branchId }
            );

            if (result.Rows.Count == 0)
                return 0;

            return Convert.ToInt32(result.Rows[0][0]);
        }

        // ==========================================
        // GET DECIMAL
        // ==========================================

        private decimal GetDecimal(string query, string branchId)
        {
            DataTable result = AppDB.Instance.TableData(
                query,
                new { BranchId = branchId }
            );

            if (result.Rows.Count == 0 ||
                result.Rows[0][0] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToDecimal(result.Rows[0][0]);
        }

        // ==========================================
        // TASK MANAGER
        // ==========================================

        private void LoadTaskManager()
        {
            try
            {
                string branchId = AccessBranch_ByForm.BranchId;

                if (string.IsNullOrWhiteSpace(branchId))
                    return;

                const string query = @"
                    SELECT TOP 10
                        TaskName,
                        Description,
                        Deadline,
                        Status
                    FROM TaskTbl
                    WHERE BranchID = @BranchId
                    ORDER BY DateAssigned DESC";

                DataTable tasks = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                // CREATE TASK LIST PANEL
                if (taskListPanel == null ||
                    taskListPanel.IsDisposed)
                {
                    taskListPanel = new FlowLayoutPanel
                    {
                        Name = "AdminTaskListPanel",
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        FlowDirection = FlowDirection.TopDown,
                        WrapContents = false,
                        BackColor = Color.Transparent,
                        Padding = new Padding(8)
                    };

                    guna2Panel2.Controls.Add(taskListPanel);
                    taskListPanel.BringToFront();
                }

                taskListPanel.Controls.Clear();

                // NO TASKS
                if (tasks.Rows.Count == 0)
                {
                    Label emptyLabel = new Label
                    {
                        Text = "No tasks available.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        Padding = new Padding(8)
                    };

                    taskListPanel.Controls.Add(emptyLabel);
                    return;
                }

                // DISPLAY TASKS
                foreach (DataRow row in tasks.Rows)
                {
                    string taskName =
                        Convert.ToString(row["TaskName"]) ?? "";

                    string description =
                        Convert.ToString(row["Description"]) ?? "";

                    string status =
                        Convert.ToString(row["Status"]) ?? "";

                    string deadline =
                        row["Deadline"] == DBNull.Value
                            ? "N/A"
                            : Convert.ToDateTime(row["Deadline"])
                                .ToString("MMM dd, yyyy");

                    Label taskLabel = new Label
                    {
                        AutoSize = false,
                        Width = Math.Max(
                            180,
                            taskListPanel.ClientSize.Width - 35
                        ),
                        Height = 85,
                        Margin = new Padding(4),
                        Padding = new Padding(8),
                        BackColor = Color.FromArgb(248, 248, 248),
                        ForeColor = Color.FromArgb(52, 70, 90),
                        Font = new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Regular
                        ),
                        Text =
                            "Task: " + taskName + Environment.NewLine +
                            "Description: " + description + Environment.NewLine +
                            "Deadline: " + deadline +
                            "     Status: " + status
                    };

                    taskListPanel.Controls.Add(taskLabel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load tasks:\n" + ex.Message,
                    "Task Manager Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==========================================
        // EXISTING EVENTS
        // ==========================================

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void RM_TimeLbl_Click(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel43_Click(object sender, EventArgs e)
        {
        }
    }
}
