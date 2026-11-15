
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
    public partial class UserControl_Admin_Task_Manager : UserControl
    {
        private bool loading = false;
        private string currentMode = "";
        private string selectedTaskID = "";
        private string newTaskID = "";

        public UserControl_Admin_Task_Manager()
        {
            InitializeComponent();

            Load += UserControl_Admin_Task_Manager_Load;

            TM_searchTxt.TextChanged += TM_searchTxt_TextChanged;
            TM_statusCB.SelectedIndexChanged += TM_FilterChanged;
            TM_branchCB.SelectedIndexChanged += TM_FilterChanged;

            TM_TaskDGV.SelectionChanged += TM_TaskDGV_SelectionChanged;

            TM_branchDetailsCB.SelectedIndexChanged += TM_branchDetailsCB_SelectedIndexChanged;

            TM_addBtn.Click += TM_addBtn_Click;
            TM_editBtn.Click += TM_editBtn_Click;
            TM_deleteBtn.Click += TM_deleteBtn_Click;
        }

        // ==========================================
        // LOAD
        // ==========================================
        private void UserControl_Admin_Task_Manager_Load(object? sender, EventArgs e)
        {
            try
            {
                loading = true;

                TM_TaskDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                TM_TaskDGV.MultiSelect = false;
                TM_TaskDGV.ReadOnly = true;
                TM_TaskDGV.AutoGenerateColumns = true;

                LoadBranchFilter();
                LoadStatusFilter();
                LoadDetailsBranches();
                LoadDetailsStatus();

                loading = false;

                SetDetailsEditable(false);
                LoadTasks();
            }
            catch (Exception ex)
            {
                loading = false;
                MessageBox.Show("Unable to initialize Task Manager.\n" + ex.Message);
            }
        }

        // ==========================================
        // BRANCH FILTER
        // ==========================================
        private void LoadBranchFilter()
        {
            DataTable dt = AppDB.Instance.TableData(
                "SELECT BranchId, BranchName FROM BranchTbl ORDER BY BranchName"
            );

            DataRow all = dt.NewRow();
            all["BranchId"] = "ALL";
            all["BranchName"] = "All Branches";
            dt.Rows.InsertAt(all, 0);

            TM_branchCB.DataSource = dt;
            TM_branchCB.DisplayMember = "BranchName";
            TM_branchCB.ValueMember = "BranchId";
            TM_branchCB.SelectedIndex = 0;
        }

        // ==========================================
        // STATUS FILTER
        // ==========================================
        private void LoadStatusFilter()
        {
            TM_statusCB.Items.Clear();

            TM_statusCB.Items.AddRange(new object[]
            {
                "All Statuses",
                "Pending",
                "In Progress",
                "Completed",
                "Cancelled"
            });

            TM_statusCB.SelectedIndex = 0;
        }

        // ==========================================
        // DETAILS BRANCH COMBOBOX
        // ==========================================
        private void LoadDetailsBranches()
        {
            DataTable dt = AppDB.Instance.TableData(
                "SELECT BranchId, BranchName FROM BranchTbl ORDER BY BranchName"
            );

            TM_branchDetailsCB.DataSource = dt;
            TM_branchDetailsCB.DisplayMember = "BranchName";
            TM_branchDetailsCB.ValueMember = "BranchId";
            TM_branchDetailsCB.SelectedIndex = -1;
        }

        // ==========================================
        // DETAILS STATUS COMBOBOX
        // ==========================================
        private void LoadDetailsStatus()
        {
            TM_statusDetailsCB.Items.Clear();

            TM_statusDetailsCB.Items.AddRange(new object[]
            {
                "Pending",
                "In Progress",
                "Completed",
                "Cancelled"
            });

            TM_statusDetailsCB.SelectedIndex = -1;
        }

        // ==========================================
        // LOAD STAFF BY BRANCH
        // ==========================================
        private void LoadAssignedStaff(string branchID, string? staffID = null)
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(
                    @"SELECT StaffID, FullName
                      FROM StaffTbl
                      WHERE BranchID = @BranchID
                      ORDER BY FullName",
                    new { BranchID = branchID }
                );

                TM_assignedStaffCB.DataSource = null;
                TM_assignedStaffCB.DisplayMember = "FullName";
                TM_assignedStaffCB.ValueMember = "StaffID";
                TM_assignedStaffCB.DataSource = dt;

                if (!string.IsNullOrWhiteSpace(staffID))
                    TM_assignedStaffCB.SelectedValue = staffID;
                else
                    TM_assignedStaffCB.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load staff.\n" + ex.Message);
            }
        }

        private void TM_branchDetailsCB_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (loading || currentMode == "")
                return;

            if (TM_branchDetailsCB.SelectedValue == null)
                return;

            if (TM_branchDetailsCB.SelectedValue is DataRowView)
                return;

            string branchID = TM_branchDetailsCB.SelectedValue.ToString()!;

            if (!string.IsNullOrWhiteSpace(branchID))
                LoadAssignedStaff(branchID);
        }

        // ==========================================
        // LOAD TASKS
        // ==========================================
        private void LoadTasks()
        {
            try
            {
                string branch = TM_branchCB.SelectedValue?.ToString() ?? "ALL";
                string status = TM_statusCB.Text;
                string search = TM_searchTxt.Text.Trim();

                DataTable dt = AppDB.Instance.TableData(
                    @"SELECT
                        t.TaskID,
                        t.TaskName,
                        s.FullName AS AssignedStaff,
                        b.BranchName AS Branch,
                        t.Deadline,
                        t.Status,
                        t.Description,
                        t.StaffID,
                        t.BranchID,
                        t.DateAssigned
                    FROM TaskTbl t
                    INNER JOIN StaffTbl s ON t.StaffID = s.StaffID
                    INNER JOIN BranchTbl b ON t.BranchID = b.BranchId
                    WHERE
                        (@Branch = 'ALL' OR t.BranchID = @Branch)
                        AND (@Status = 'All Statuses' OR t.Status = @Status)
                        AND (
                            t.TaskID LIKE @Search
                            OR t.TaskName LIKE @Search
                            OR s.FullName LIKE @Search
                            OR b.BranchName LIKE @Search
                        )
                    ORDER BY t.DateAssigned DESC",
                    new
                    {
                        Branch = branch,
                        Status = status,
                        Search = "%" + search + "%"
                    }
                );

                TM_TaskDGV.DataSource = dt;

                TM_TaskDGV.Columns["Description"].Visible = false;
                TM_TaskDGV.Columns["StaffID"].Visible = false;
                TM_TaskDGV.Columns["BranchID"].Visible = false;
                TM_TaskDGV.Columns["DateAssigned"].Visible = false;

                TM_TaskDGV.Columns["TaskID"].HeaderText = "Task ID";
                TM_TaskDGV.Columns["TaskName"].HeaderText = "Task Name";
                TM_TaskDGV.Columns["AssignedStaff"].HeaderText = "Assigned Staff";
                TM_TaskDGV.Columns["Branch"].HeaderText = "Branch";
                TM_TaskDGV.Columns["Deadline"].HeaderText = "Deadline";
                TM_TaskDGV.Columns["Status"].HeaderText = "Status";

                TM_TaskDGV.Columns["Deadline"].DefaultCellStyle.Format = "MMM dd, yyyy";

                TM_TaskDGV.ClearSelection();

                selectedTaskID = "";
                currentMode = "";
                newTaskID = "";

                ClearDetails();
                SetDetailsEditable(false);

                TM_addBtn.Text = "Add";
                TM_editBtn.Text = "Edit";

                UpdateSummary();
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load tasks.\n" + ex.Message);
            }
        }

        // ==========================================
        // SUMMARY CARDS
        // ==========================================
        private void UpdateSummary()
        {
            try
            {
                TM_totalLbl.Text = Convert.ToString(
                    AppDB.Instance.Scalar("SELECT COUNT(*) FROM TaskTbl")
                ) ?? "0";

                TM_pendingLbl.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM TaskTbl WHERE Status = 'Pending'"
                    )
                ) ?? "0";

                TM_inProgressLbl.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM TaskTbl WHERE Status = 'In Progress'"
                    )
                ) ?? "0";

                TM_completedLbl.Text = Convert.ToString(
                    AppDB.Instance.Scalar(
                        "SELECT COUNT(*) FROM TaskTbl WHERE Status = 'Completed'"
                    )
                ) ?? "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update summary.\n" + ex.Message);
            }
        }

        // ==========================================
        // SEARCH AND FILTER
        // ==========================================
        private void TM_searchTxt_TextChanged(object? sender, EventArgs e)
        {
            if (!loading && currentMode == "")
                LoadTasks();
        }

        private void TM_FilterChanged(object? sender, EventArgs e)
        {
            if (!loading && currentMode == "")
                LoadTasks();
        }

        // ==========================================
        // SELECT TASK
        // ==========================================
        private void TM_TaskDGV_SelectionChanged(object? sender, EventArgs e)
        {
            if (loading || currentMode != "")
                return;

            if (TM_TaskDGV.SelectedRows.Count == 0)
            {
                selectedTaskID = "";
                ClearDetails();
                UpdateButtons();
                return;
            }

            DataGridViewRow row = TM_TaskDGV.SelectedRows[0];

            selectedTaskID = row.Cells["TaskID"].Value?.ToString() ?? "";

            TM_taskIDlbl.Text = selectedTaskID;
            TM_taskNametxt.Text = row.Cells["TaskName"].Value?.ToString() ?? "";
            TM_descriptiontxt.Text = row.Cells["Description"].Value?.ToString() ?? "";

            string branchID = row.Cells["BranchID"].Value?.ToString() ?? "";
            string staffID = row.Cells["StaffID"].Value?.ToString() ?? "";

            loading = true;

            TM_branchDetailsCB.SelectedValue = branchID;
            LoadAssignedStaff(branchID, staffID);

            TM_dateAssignedDTP.Value =
                Convert.ToDateTime(row.Cells["DateAssigned"].Value);

            TM_deadlineDTP.Value =
                Convert.ToDateTime(row.Cells["Deadline"].Value);

            TM_statusDetailsCB.SelectedItem =
                row.Cells["Status"].Value?.ToString();

            loading = false;

            SetDetailsEditable(false);
            UpdateButtons();
        }

        // ==========================================
        // CLEAR DETAILS
        // ==========================================
        private void ClearDetails()
        {
            TM_taskIDlbl.Text = "---";
            TM_taskNametxt.Clear();
            TM_descriptiontxt.Clear();

            TM_branchDetailsCB.SelectedIndex = -1;
            TM_assignedStaffCB.DataSource = null;

            TM_dateAssignedDTP.Value = DateTime.Now;
            TM_deadlineDTP.Value = DateTime.Now.AddDays(1);

            TM_statusDetailsCB.SelectedIndex = -1;
        }

        // ==========================================
        // ENABLE / DISABLE EDITING
        // ==========================================
        private void SetDetailsEditable(bool editable)
        {
            TM_taskNametxt.ReadOnly = !editable;
            TM_descriptiontxt.ReadOnly = !editable;

            TM_branchDetailsCB.Enabled = editable;
            TM_assignedStaffCB.Enabled = editable;
            TM_dateAssignedDTP.Enabled = editable;
            TM_deadlineDTP.Enabled = editable;
            TM_statusDetailsCB.Enabled = editable;
        }

        // ==========================================
        // BUTTON STATES
        // ==========================================
        private void UpdateButtons()
        {
            bool hasSelection = TM_TaskDGV.SelectedRows.Count > 0;

            TM_editBtn.Enabled = hasSelection || currentMode == "Edit";
            TM_deleteBtn.Enabled = hasSelection && currentMode == "";
            TM_addBtn.Enabled = true;
        }

        // ==========================================
        // ADD TASK
        // ==========================================
        private void TM_addBtn_Click(object? sender, EventArgs e)
        {
            if (currentMode == "Add")
            {
                SaveTask();
                return;
            }

            currentMode = "Add";
            selectedTaskID = "";

            newTaskID = "TSK" +
                Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            TM_TaskDGV.ClearSelection();

            loading = true;
            ClearDetails();
            loading = false;

            TM_taskIDlbl.Text = newTaskID;

            TM_dateAssignedDTP.Value = DateTime.Now;
            TM_deadlineDTP.Value = DateTime.Now.AddDays(1);
            TM_statusDetailsCB.SelectedItem = "Pending";

            SetDetailsEditable(true);

            TM_addBtn.Text = "Save";
            TM_editBtn.Enabled = false;
            TM_deleteBtn.Enabled = false;

            TM_taskNametxt.Focus();
        }

        // ==========================================
        // EDIT TASK
        // ==========================================
        private void TM_editBtn_Click(object? sender, EventArgs e)
        {
            if (currentMode == "Edit")
            {
                SaveTask();
                return;
            }

            if (TM_TaskDGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task first.");
                return;
            }

            currentMode = "Edit";

            SetDetailsEditable(true);

            TM_editBtn.Text = "Save";
            TM_addBtn.Enabled = false;
            TM_deleteBtn.Enabled = false;
        }

        // ==========================================
        // SAVE TASK
        // ==========================================
        private void SaveTask()
        {
            if (string.IsNullOrWhiteSpace(TM_taskNametxt.Text))
            {
                MessageBox.Show("Please enter the task name.");
                TM_taskNametxt.Focus();
                return;
            }

            if (TM_branchDetailsCB.SelectedValue == null ||
                TM_branchDetailsCB.SelectedValue is DataRowView)
            {
                MessageBox.Show("Please select a branch.");
                return;
            }

            if (TM_assignedStaffCB.SelectedValue == null ||
                TM_assignedStaffCB.SelectedValue is DataRowView)
            {
                MessageBox.Show("Please select an assigned staff.");
                return;
            }

            if (TM_statusDetailsCB.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.");
                return;
            }

            if (TM_deadlineDTP.Value.Date < TM_dateAssignedDTP.Value.Date)
            {
                MessageBox.Show("Deadline cannot be earlier than Date Assigned.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Do you want to save this task?",
                "Confirm Save",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string taskName = TM_taskNametxt.Text.Trim();
                string description = TM_descriptiontxt.Text.Trim();
                string branchID = TM_branchDetailsCB.SelectedValue.ToString()!;
                string staffID = TM_assignedStaffCB.SelectedValue.ToString()!;
                DateTime dateAssigned = TM_dateAssignedDTP.Value;
                DateTime deadline = TM_deadlineDTP.Value;
                string status = TM_statusDetailsCB.SelectedItem.ToString()!;

                int result;

                if (currentMode == "Add")
                {
                    result = AppDB.Instance.CUD(
                        @"INSERT INTO TaskTbl
                          (TaskID, TaskName, Description, StaffID, BranchID,
                           DateAssigned, Deadline, Status)
                          VALUES
                          (@TaskID, @TaskName, @Description, @StaffID, @BranchID,
                           @DateAssigned, @Deadline, @Status)",
                        new
                        {
                            TaskID = newTaskID,
                            TaskName = taskName,
                            Description = description,
                            StaffID = staffID,
                            BranchID = branchID,
                            DateAssigned = dateAssigned,
                            Deadline = deadline,
                            Status = status
                        }
                    );
                }
                else
                {
                    result = AppDB.Instance.CUD(
                        @"UPDATE TaskTbl
                          SET TaskName = @TaskName,
                              Description = @Description,
                              StaffID = @StaffID,
                              BranchID = @BranchID,
                              DateAssigned = @DateAssigned,
                              Deadline = @Deadline,
                              Status = @Status
                          WHERE TaskID = @TaskID",
                        new
                        {
                            TaskID = selectedTaskID,
                            TaskName = taskName,
                            Description = description,
                            StaffID = staffID,
                            BranchID = branchID,
                            DateAssigned = dateAssigned,
                            Deadline = deadline,
                            Status = status
                        }
                    );
                }

                if (result > 0)
                {
                    MessageBox.Show("Task saved successfully!");

                    currentMode = "";
                    newTaskID = "";

                    TM_addBtn.Text = "Add";
                    TM_editBtn.Text = "Edit";

                    TM_addBtn.Enabled = true;
                    TM_editBtn.Enabled = true;

                    LoadTasks();
                }
                else
                {
                    MessageBox.Show("No changes were saved.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save task.\n" + ex.Message);
            }
        }

        // ==========================================
        // DELETE TASK
        // ==========================================
        private void TM_deleteBtn_Click(object? sender, EventArgs e)
        {
            if (TM_TaskDGV.SelectedRows.Count == 0)
                return;

            string taskID = TM_TaskDGV.SelectedRows[0]
                .Cells["TaskID"].Value?.ToString() ?? "";

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this task?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                int result = AppDB.Instance.CUD(
                    "DELETE FROM TaskTbl WHERE TaskID = @TaskID",
                    new { TaskID = taskID }
                );

                if (result > 0)
                {
                    MessageBox.Show("Task deleted successfully!");
                    LoadTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete task.\n" + ex.Message);
            }
        }
    }
}
