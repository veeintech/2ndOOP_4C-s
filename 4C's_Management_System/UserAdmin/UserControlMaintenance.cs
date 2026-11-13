
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _4C_s_Management_System
{
    public partial class UserControlMaintenance : UserControl
    {
        private bool isLoading = false;
        private bool isNewRequest = false;

        public string branchid = AccessBranch_ByForm.BranchId;

        public UserControlMaintenance()
        {
            InitializeComponent();

            Load += UserControlMaintenance_Load;

            TB_SEARCH.TextChanged += TB_SEARCH_TextChanged;
            CB_ROOM_TYPE.SelectedIndexChanged += CB_ROOM_TYPE_SelectedIndexChanged;
            CB_STATUS.SelectedIndexChanged += CB_STATUS_SelectedIndexChanged;

            DGV_MAINTENANCE.CellClick += DGV_MAINTENANCE_CellClick;

            btn_add_req.Click += btn_add_req_Click;
            btn_update.Click += btn_update_Click;
            btn_cancel.Click += btn_cancel_Click;

            btn_start_task.Click += btn_start_task_Click;
            btn_end_task.Click += btn_end_task_Click;
        }

        // ==========================================
        // LOAD FORM
        // ==========================================

        private void UserControlMaintenance_Load(object sender, EventArgs e)
        {
            try
            {
                isLoading = true;

                // LOAD ROOM TYPES
                CB_ROOM_TYPE.Items.Clear();
                CB_ROOM_TYPE.Items.Add("All Room Types");

                DataTable roomTypes = AppDB.Instance.TableData(
                    "SELECT DISTINCT RoomType FROM TBLRoom ORDER BY RoomType");

                foreach (DataRow row in roomTypes.Rows)
                {
                    CB_ROOM_TYPE.Items.Add(row["RoomType"].ToString());
                }

                CB_ROOM_TYPE.SelectedIndex = 0;

                // LOAD STATUS OPTIONS
                CB_STATUS.Items.Clear();

                CB_STATUS.Items.AddRange(new object[]
                {
                    "All Status",
                    "Pending", 
                    "Approved",
                    "In Progress",
                    "Completed",
                    "Canceled",
                    "Rejected"
                   
                });

                CB_STATUS.SelectedIndex = 0;

                isLoading = false;

                LoadMaintenance();
                UpdateDashboard();
                ClearFields();
            }
            catch (Exception ex)
            {
                isLoading = false;

                MessageBox.Show(
                    "Error loading maintenance: " + ex.Message);
            }
        }

        // ==========================================
        // LOAD, SEARCH AND FILTER MAINTENANCE
        // ==========================================

        private void LoadMaintenance()
        {
            try
            {
                string query = @"
                    SELECT
                        M.MaintenanceID,
                        M.RoomID,
                        M.Issue,
                        M.Remarks,
                        M.DateStarted,
                        M.DateCompleted,
                        M.Status,
                        R.RoomType
                    FROM MaintenanceTbl M
                    LEFT JOIN TBLRoom R
                        ON M.RoomID = R.RoomId
                    WHERE
                        (
                            @search = ''
                            OR M.MaintenanceID LIKE '%' + @search + '%'
                            OR M.RoomID LIKE '%' + @search + '%'
                            OR M.Issue LIKE '%' + @search + '%'
                        )
                        AND
                        (
                            @roomType = 'All Room Types'
                            OR R.RoomType = @roomType
                        )
                        AND
                        (
                            @status = 'All Status'
                            OR M.Status = @status
                        )
                    ORDER BY M.MaintenanceID DESC";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        search = TB_SEARCH.Text.Trim(),
                        roomType = CB_ROOM_TYPE.Text,
                        status = CB_STATUS.Text
                    });

                // DISPLAY DATA
                DGV_MAINTENANCE.DataSource = null;
                DGV_MAINTENANCE.DataSource = dt;

                // DISPLAY COLUMNS
                DGV_MAINTENANCE.Columns["MaintenanceID"].Visible = true;
                DGV_MAINTENANCE.Columns["MaintenanceID"].HeaderText = "Maintenance ID";

                DGV_MAINTENANCE.Columns["RoomID"].Visible = true;
                DGV_MAINTENANCE.Columns["RoomID"].HeaderText = "Room";

                DGV_MAINTENANCE.Columns["Issue"].Visible = true;
                DGV_MAINTENANCE.Columns["Status"].Visible = true;

                DGV_MAINTENANCE.Columns["RoomType"].Visible = true;
                DGV_MAINTENANCE.Columns["RoomType"].HeaderText = "Room Type";

                // HIDE OTHER DETAILS
                DGV_MAINTENANCE.Columns["Remarks"].Visible = false;
                DGV_MAINTENANCE.Columns["DateStarted"].Visible = false;
                DGV_MAINTENANCE.Columns["DateCompleted"].Visible = false;

                // EQUAL COLUMN WIDTH
                DGV_MAINTENANCE.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                DGV_MAINTENANCE.Columns["MaintenanceID"].FillWeight = 100;
                DGV_MAINTENANCE.Columns["RoomID"].FillWeight = 100;
                DGV_MAINTENANCE.Columns["Issue"].FillWeight = 100;
                DGV_MAINTENANCE.Columns["Status"].FillWeight = 100;
                DGV_MAINTENANCE.Columns["RoomType"].FillWeight = 100;

                DGV_MAINTENANCE.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to refresh maintenance list: " + ex.Message);
            }
        }

        // ==========================================
        // SEARCH AND FILTER EVENTS
        // ==========================================

        private void TB_SEARCH_TextChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            LoadMaintenance();
        }

        private void CB_ROOM_TYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            LoadMaintenance();
        }

        private void CB_STATUS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            LoadMaintenance();
        }

        // ==========================================
        // SELECT ROW - DISPLAY DETAILS
        // ==========================================

        private void DGV_MAINTENANCE_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = DGV_MAINTENANCE.Rows[e.RowIndex];

                if (row.Cells["MaintenanceID"].Value == null)
                    return;

                isNewRequest = false;

                string maintenanceID =
                    row.Cells["MaintenanceID"].Value.ToString();

                // BOTTOM INPUT FIELDS
                tb_maintenance_id.Text = maintenanceID;

                txb_Room_id.Text =
                    row.Cells["RoomID"].Value?.ToString() ?? "";

                txb_issue.Text =
                    row.Cells["Issue"].Value?.ToString() ?? "";

                txb_remarks.Text =
                    row.Cells["Remarks"].Value?.ToString() ?? "";

                // RIGHT SIDE DETAILS
                maintenance_id_info.Text = maintenanceID;

                Rm_no_LBL.Text =
                    row.Cells["RoomID"].Value?.ToString() ?? "";

                mtn_issue_info.Text =
                    row.Cells["Issue"].Value?.ToString() ?? "";

                Date_std_info.Text =
                    row.Cells["DateStarted"].Value == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(row.Cells["DateStarted"].Value)
                        .ToString("MM/dd/yyyy hh:mm tt");

                date_ctd_info.Text =
                    row.Cells["DateCompleted"].Value == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(row.Cells["DateCompleted"].Value)
                        .ToString("MM/dd/yyyy hh:mm tt");

                remarks_info.Text =
                    row.Cells["Remarks"].Value?.ToString() ?? "";

                Maintenance_StatusLBL.Text =
                    row.Cells["Status"].Value?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying details: " + ex.Message);
            }
        }

        // ==========================================
        // ADD NEW MAINTENANCE REQUEST
        // ==========================================

        private void btn_add_req_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txb_Room_id.Text) ||
                string.IsNullOrWhiteSpace(txb_issue.Text))
            {
                MessageBox.Show("Please enter Room ID and Issue.");
                return;
            }

            try
            {
                object? roomExists = AppDB.Instance.Scalar(
                    "SELECT COUNT(*) FROM TBLRoom WHERE RoomId = @RoomID",
                    new { RoomID = txb_Room_id.Text.Trim() });

                if (roomExists == null || Convert.ToInt32(roomExists) == 0)
                {
                    MessageBox.Show("Room ID does not exist.");
                    return;
                }

                string query = @"
                    INSERT INTO MaintenanceTbl
                    (
                        MaintenanceID,
                        RoomID,
                        Issue,
                        Remarks,
                        Status
                    )
                    VALUES
                    (
                        @id,
                        @roomID,
                        @issue,
                        @remarks,
                        'Pending'
                    )";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = IDGenerator.GenerateNextID("MTN", branchid),
                        roomID = txb_Room_id.Text.Trim(),
                        issue = txb_issue.Text.Trim(),
                        remarks = txb_remarks.Text.Trim()
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Maintenance request added successfully!");

                    ClearFields();
                    LoadMaintenance();
                    UpdateDashboard();
                }
                else
                {
                    MessageBox.Show("Unable to add maintenance request.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding request: " + ex.Message);
            }
        }

        // ==========================================
        // UPDATE SELECTED REQUEST
        // ==========================================

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txb_Room_id.Text) ||
                string.IsNullOrWhiteSpace(txb_issue.Text))
            {
                MessageBox.Show(
                    "Room ID and Issue are required.");
                return;
            }

            try
            {
                object? roomExists = AppDB.Instance.Scalar(
                    "SELECT COUNT(*) FROM TBLRoom WHERE RoomId = @RoomID",
                    new { RoomID = txb_Room_id.Text.Trim() });

                if (roomExists == null || Convert.ToInt32(roomExists) == 0)
                {
                    MessageBox.Show("Room ID does not exist.");
                    return;
                }

                string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        RoomID = @roomID,
                        Issue = @issue,
                        Remarks = @remarks
                    WHERE MaintenanceID = @id";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = tb_maintenance_id.Text,
                        roomID = txb_Room_id.Text.Trim(),
                        issue = txb_issue.Text.Trim(),
                        remarks = txb_remarks.Text.Trim()
                    });

                if (result > 0)
                {
                    MessageBox.Show("Maintenance record updated!");

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord();
                }
                else
                {
                    MessageBox.Show("No record was updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating record: " + ex.Message);
            }
        }

        // ==========================================
        // CANCEL / CLEAR FIELDS
        // ==========================================

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            isNewRequest = true;

            tb_maintenance_id.Clear();
            txb_Room_id.Clear();
            txb_issue.Clear();
            txb_remarks.Clear();

            maintenance_id_info.Text = "";
            mtn_issue_info.Text = "";
            Rm_no_LBL.Text = "";

            Date_std_info.Text = "";
            date_ctd_info.Text = "";

            remarks_info.Text = "";
            Maintenance_StatusLBL.Text = "";

            DGV_MAINTENANCE.ClearSelection();
        }

        // ==========================================
        // START MAINTENANCE TASK
        // ==========================================

        private void btn_start_task_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.");
                return;
            }

            try
            {
                string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        Status = 'In Progress',
                        DateStarted = GETDATE()
                    WHERE MaintenanceID = @id
                    AND Status = 'Pending'";

                int result = AppDB.Instance.CUD(
                    query,
                    new { id = tb_maintenance_id.Text });

                if (result > 0)
                {
                    MessageBox.Show("Task started!");

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord();
                }
                else
                {
                    MessageBox.Show(
                        "Only Pending tasks can be started.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error starting task: " + ex.Message);
            }
        }

        // ==========================================
        // END MAINTENANCE TASK
        // ==========================================

        private void btn_end_task_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.");
                return;
            }

            try
            {
                string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        Status = 'Completed',
                        DateCompleted = GETDATE()
                    WHERE MaintenanceID = @id
                    AND Status = 'In Progress'";

                int result = AppDB.Instance.CUD(
                    query,
                    new { id = tb_maintenance_id.Text });

                if (result > 0)
                {
                    MessageBox.Show("Task completed!");

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord();
                }
                else
                {
                    MessageBox.Show(
                        "Only In Progress tasks can be completed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error completing task: " + ex.Message);
            }
        }

        // ==========================================
        // RESELECT UPDATED RECORD
        // ==========================================

        private void SelectUpdatedRecord()
        {
            string id = tb_maintenance_id.Text;

            foreach (DataGridViewRow row in DGV_MAINTENANCE.Rows)
            {
                if (row.Cells["MaintenanceID"].Value?.ToString() == id)
                {
                    row.Selected = true;

                    DGV_MAINTENANCE.CurrentCell =
                        row.Cells["MaintenanceID"];

                    DGV_MAINTENANCE_CellClick(
                        DGV_MAINTENANCE,
                        new DataGridViewCellEventArgs(
                            0, row.Index));

                    return;
                }
            }
        }

        // ==========================================
        // DASHBOARD COUNTS
        // ==========================================

        private void UpdateDashboard()
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(@"
                    SELECT
                        Status,
                        COUNT(*) AS Total
                    FROM MaintenanceTbl
                    GROUP BY Status");

                int total = 0;
                int completed = 0;
                int inProgress = 0;
                int pending = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int count = Convert.ToInt32(row["Total"]);
                    string status = row["Status"].ToString();

                    total += count;

                    if (status == "Completed")
                        completed = count;

                    else if (status == "In Progress")
                        inProgress = count;

                    else if (status == "Pending")
                        pending = count;
                }

                LBL_TOTAL_NO.Text = total.ToString();
                LBL_COMPLETED_NO.Text = completed.ToString();
                LBL_INPROGRESS_NO.Text = inProgress.ToString();
                LBL_PENDING_NO.Text = pending.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating dashboard: " + ex.Message);
            }
        }
    }
}
