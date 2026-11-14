
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlMaintenance : UserControl
    {
        private bool isLoading = false;

        public string branchid = AccessBranch_ByForm.BranchId;

        public UserControlMaintenance()
        {
            InitializeComponent();

            Load -= UserControlMaintenance_Load;
            Load += UserControlMaintenance_Load;

            TB_SEARCH.TextChanged -= TB_SEARCH_TextChanged;
            TB_SEARCH.TextChanged += TB_SEARCH_TextChanged;

            CB_ROOM_TYPE.SelectedIndexChanged -= CB_ROOM_TYPE_SelectedIndexChanged;
            CB_ROOM_TYPE.SelectedIndexChanged += CB_ROOM_TYPE_SelectedIndexChanged;

            CB_STATUS.SelectedIndexChanged -= CB_STATUS_SelectedIndexChanged;
            CB_STATUS.SelectedIndexChanged += CB_STATUS_SelectedIndexChanged;

            DGV_MAINTENANCE.CellClick -= DGV_MAINTENANCE_CellClick;
            DGV_MAINTENANCE.CellClick += DGV_MAINTENANCE_CellClick;

            btn_add_req.Click -= btn_add_req_Click;
            btn_add_req.Click += btn_add_req_Click;

            btn_update.Click -= btn_update_Click;
            btn_update.Click += btn_update_Click;

            btn_cancel.Click -= btn_cancel_Click;
            btn_cancel.Click += btn_cancel_Click;

            btn_start_task.Click -= btn_start_task_Click;
            btn_start_task.Click += btn_start_task_Click;

            btn_end_task.Click -= btn_end_task_Click;
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

                branchid = AccessBranch_ByForm.BranchId;

                // LOAD ROOM TYPES
                CB_ROOM_TYPE.Items.Clear();
                CB_ROOM_TYPE.Items.Add("All Room Types");

                DataTable roomTypes = AppDB.Instance.TableData(
                    @"SELECT DISTINCT RoomType
                      FROM TBLRoom
                      WHERE BranchID = @BranchID
                      ORDER BY RoomType",
                    new { BranchID = branchid });

                foreach (DataRow row in roomTypes.Rows)
                {
                    CB_ROOM_TYPE.Items.Add(
                        Convert.ToString(row["RoomType"]));
                }

                CB_ROOM_TYPE.SelectedIndex = 0;

                // LOAD MAINTENANCE STATUSES
                CB_STATUS.Items.Clear();

                CB_STATUS.Items.AddRange(new object[]
                {
                    "All Status",
                    "Pending",
                    "Approved",
                    "In Progress",
                    "Completed",
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
                    "Error loading maintenance: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // LOAD MAINTENANCE
        // ==========================================

        private void LoadMaintenance()
        {
            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                const string query = @"
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
                        AND M.BranchID = R.BranchID
                    WHERE
                        M.BranchID = @BranchID
                        AND
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
                    ORDER BY M.MaintenanceID DESC;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        BranchID = branchid,
                        search = TB_SEARCH.Text.Trim(),
                        roomType = CB_ROOM_TYPE.Text,
                        status = CB_STATUS.Text
                    });

                DGV_MAINTENANCE.DataSource = null;
                DGV_MAINTENANCE.DataSource = dt;

                if (DGV_MAINTENANCE.Columns.Count > 0)
                {
                    DGV_MAINTENANCE.Columns["MaintenanceID"].HeaderText =
                        "Maintenance ID";

                    DGV_MAINTENANCE.Columns["RoomID"].HeaderText =
                        "Room";

                    DGV_MAINTENANCE.Columns["Issue"].HeaderText =
                        "Issue";

                    DGV_MAINTENANCE.Columns["Status"].HeaderText =
                        "Status";

                    DGV_MAINTENANCE.Columns["RoomType"].HeaderText =
                        "Room Type";

                    DGV_MAINTENANCE.Columns["Remarks"].Visible = false;
                    DGV_MAINTENANCE.Columns["DateStarted"].Visible = false;
                    DGV_MAINTENANCE.Columns["DateCompleted"].Visible = false;

                    DGV_MAINTENANCE.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;

                    DGV_MAINTENANCE.Columns["MaintenanceID"].FillWeight = 100;
                    DGV_MAINTENANCE.Columns["RoomID"].FillWeight = 100;
                    DGV_MAINTENANCE.Columns["Issue"].FillWeight = 150;
                    DGV_MAINTENANCE.Columns["Status"].FillWeight = 100;
                    DGV_MAINTENANCE.Columns["RoomType"].FillWeight = 100;
                }

                DGV_MAINTENANCE.ClearSelection();
                DGV_MAINTENANCE.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to refresh maintenance list: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // SEARCH AND FILTER EVENTS
        // ==========================================

        private void TB_SEARCH_TextChanged(object sender, EventArgs e)
        {
            if (isLoading)
                return;

            LoadMaintenance();
        }

        private void CB_ROOM_TYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading)
                return;

            LoadMaintenance();
        }

        private void CB_STATUS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading)
                return;

            LoadMaintenance();
        }

        // ==========================================
        // SELECT ROW
        // ==========================================

        private void DGV_MAINTENANCE_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= DGV_MAINTENANCE.Rows.Count)
                return;

            try
            {
                DataGridViewRow row = DGV_MAINTENANCE.Rows[e.RowIndex];

                if (row.IsNewRow ||
                    row.Cells["MaintenanceID"].Value == null)
                    return;

                string maintenanceID =
                    Convert.ToString(row.Cells["MaintenanceID"].Value) ?? "";

                tb_maintenance_id.Text = maintenanceID;

                txb_Room_id.Text =
                    Convert.ToString(row.Cells["RoomID"].Value) ?? "";

                txb_issue.Text =
                    Convert.ToString(row.Cells["Issue"].Value) ?? "";

                txb_remarks.Text =
                    Convert.ToString(row.Cells["Remarks"].Value) ?? "";

                maintenance_id_info.Text = maintenanceID;

                Rm_no_LBL.Text =
                    Convert.ToString(row.Cells["RoomID"].Value) ?? "";

                mtn_issue_info.Text =
                    Convert.ToString(row.Cells["Issue"].Value) ?? "";

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
                    Convert.ToString(row.Cells["Remarks"].Value) ?? "";

                Maintenance_StatusLBL.Text =
                    Convert.ToString(row.Cells["Status"].Value) ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying details: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // VALIDATE ROOM
        // ==========================================

        private bool ValidateRoom()
        {
            object roomExists = AppDB.Instance.Scalar(
                @"SELECT COUNT(*)
                  FROM TBLRoom
                  WHERE RoomId = @RoomID
                    AND BranchID = @BranchID",
                new
                {
                    RoomID = txb_Room_id.Text.Trim(),
                    BranchID = branchid
                });

            if (roomExists == null ||
                Convert.ToInt32(roomExists) == 0)
            {
                MessageBox.Show(
                    "Room ID does not exist in this branch.",
                    "Invalid Room",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        // ==========================================
        // SET ROOM TO MAINTENANCE
        // ==========================================

        private void SetRoomToMaintenance(string roomID)
        {
            const string query = @"
                UPDATE TBLRoom
                SET Status = 'Maintenance'
                WHERE BranchID = @BranchID
                  AND RoomId = @RoomID;";

            int result = AppDB.Instance.CUD(
                query,
                new
                {
                    BranchID = branchid,
                    RoomID = roomID
                });

            if (result <= 0)
            {
                throw new Exception(
                    "The task was started, but the room status could not be updated.");
            }
        }

        // ==========================================
        // RESTORE ROOM STATUS AFTER MAINTENANCE
        // ==========================================

        private void UpdateRoomStatusAfterMaintenance(string roomID)
        {
            if (string.IsNullOrWhiteSpace(roomID))
                return;

            const string query = @"
                UPDATE R
                SET R.Status =
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1
                            FROM MaintenanceTbl M
                            WHERE M.BranchID = R.BranchID
                              AND M.RoomID = R.RoomId
                              AND M.Status = 'In Progress'
                        )
                        THEN 'Maintenance'

                        WHEN EXISTS
                        (
                            SELECT 1
                            FROM ReservationTbl Res
                            WHERE Res.BranchId = R.BranchID
                              AND Res.RoomId = R.RoomId
                              AND Res.ResStartDate <= GETDATE()
                              AND Res.ResEndDate > GETDATE()
                        )
                        THEN 'Occupied'

                        WHEN EXISTS
                        (
                            SELECT 1
                            FROM ReservationTbl Res
                            WHERE Res.BranchId = R.BranchID
                              AND Res.RoomId = R.RoomId
                              AND Res.ResStartDate > GETDATE()
                              AND Res.ResEndDate > GETDATE()
                        )
                        THEN 'Reserved'

                        ELSE 'Available'
                    END
                FROM TBLRoom R
                WHERE R.BranchID = @BranchID
                  AND R.RoomId = @RoomID;";

            AppDB.Instance.CUD(
                query,
                new
                {
                    BranchID = branchid,
                    RoomID = roomID
                });
        }

        // ==========================================
        // ADD MAINTENANCE REQUEST
        // DEFAULT STATUS: PENDING
        // ==========================================

        private void btn_add_req_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txb_Room_id.Text) ||
                string.IsNullOrWhiteSpace(txb_issue.Text))
            {
                MessageBox.Show(
                    "Please enter Room ID and Issue.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                if (!ValidateRoom())
                    return;

                string maintenanceID =
                    IDGenerator.GenerateNextID("MTN", branchid);

                const string query = @"
                    INSERT INTO MaintenanceTbl
                    (
                        MaintenanceID,
                        BranchID,
                        RoomID,
                        Issue,
                        Remarks,
                        Status
                    )
                    VALUES
                    (
                        @id,
                        @BranchID,
                        @roomID,
                        @issue,
                        @remarks,
                        'Pending'
                    );";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        BranchID = branchid,
                        roomID = txb_Room_id.Text.Trim(),
                        issue = txb_issue.Text.Trim(),
                        remarks = txb_remarks.Text.Trim()
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Maintenance request submitted successfully! Waiting for System Admin approval.",
                        "Request Submitted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    LoadMaintenance();
                    UpdateDashboard();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to submit maintenance request.",
                        "Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error submitting request: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // UPDATE MAINTENANCE REQUEST
        // ONLY PENDING REQUESTS CAN BE EDITED
        // ==========================================

        private void btn_update_Click(object sender, EventArgs e)
        {
            string maintenanceID = tb_maintenance_id.Text.Trim();

            if (string.IsNullOrWhiteSpace(maintenanceID))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txb_Room_id.Text) ||
                string.IsNullOrWhiteSpace(txb_issue.Text))
            {
                MessageBox.Show(
                    "Room ID and Issue are required.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!ValidateRoom())
                    return;

                const string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        RoomID = @roomID,
                        Issue = @issue,
                        Remarks = @remarks
                    WHERE MaintenanceID = @id
                      AND BranchID = @BranchID
                      AND Status = 'Pending';";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        roomID = txb_Room_id.Text.Trim(),
                        issue = txb_issue.Text.Trim(),
                        remarks = txb_remarks.Text.Trim(),
                        BranchID = branchid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Maintenance request updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord(maintenanceID);
                }
                else
                {
                    MessageBox.Show(
                        "Only Pending requests can be edited.",
                        "Update Not Allowed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating record: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // APPROVE MAINTENANCE REQUEST
        // SYSTEM ADMIN ONLY
        // ==========================================

        private void btn_approve_req_Click(object sender, EventArgs e)
        {
            string maintenanceID = tb_maintenance_id.Text.Trim();

            if (string.IsNullOrWhiteSpace(maintenanceID))
            {
                MessageBox.Show(
                    "Please select a maintenance request first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET Status = 'Approved'
                    WHERE MaintenanceID = @id
                      AND BranchID = @BranchID
                      AND Status = 'Pending';";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        BranchID = branchid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Maintenance request approved successfully!",
                        "Request Approved",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord(maintenanceID);
                }
                else
                {
                    MessageBox.Show(
                        "Only Pending requests can be approved.",
                        "Unable to Approve",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error approving request: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // REJECT MAINTENANCE REQUEST
        // SYSTEM ADMIN ONLY
        // ==========================================

        private void btn_reject_req_Click(object sender, EventArgs e)
        {
            string maintenanceID = tb_maintenance_id.Text.Trim();

            if (string.IsNullOrWhiteSpace(maintenanceID))
            {
                MessageBox.Show(
                    "Please select a maintenance request first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET Status = 'Rejected'
                    WHERE MaintenanceID = @id
                      AND BranchID = @BranchID
                      AND Status = 'Pending';";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        BranchID = branchid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Maintenance request rejected.",
                        "Request Rejected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadMaintenance();
                    UpdateDashboard();
                    SelectUpdatedRecord(maintenanceID);
                }
                else
                {
                    MessageBox.Show(
                        "Only Pending requests can be rejected.",
                        "Unable to Reject",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error rejecting request: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // CLEAR FIELDS
        // ==========================================

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
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
        // ONLY APPROVED REQUESTS CAN START
        // ==========================================

        private void btn_start_task_Click(object sender, EventArgs e)
        {
            string maintenanceID = tb_maintenance_id.Text.Trim();
            string roomID = txb_Room_id.Text.Trim();

            if (string.IsNullOrWhiteSpace(maintenanceID))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        Status = 'In Progress',
                        DateStarted = GETDATE()
                    WHERE MaintenanceID = @id
                      AND BranchID = @BranchID
                      AND Status = 'Approved';";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        BranchID = branchid
                    });

                if (result <= 0)
                {
                    MessageBox.Show(
                        "Only Approved requests can be started.",
                        "Unable to Start",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                SetRoomToMaintenance(roomID);

                MessageBox.Show(
                    "Maintenance task started successfully. Room status is now Maintenance.",
                    "Task Started",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadMaintenance();
                UpdateDashboard();
                SelectUpdatedRecord(maintenanceID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error starting task: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // COMPLETE MAINTENANCE TASK
        // ONLY IN PROGRESS TASKS CAN BE COMPLETED
        // ==========================================

        private void btn_end_task_Click(object sender, EventArgs e)
        {
            string maintenanceID = tb_maintenance_id.Text.Trim();
            string roomID = txb_Room_id.Text.Trim();

            if (string.IsNullOrWhiteSpace(maintenanceID))
            {
                MessageBox.Show(
                    "Please select a maintenance record first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET
                        Status = 'Completed',
                        DateCompleted = GETDATE()
                    WHERE MaintenanceID = @id
                      AND BranchID = @BranchID
                      AND Status = 'In Progress';";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        id = maintenanceID,
                        BranchID = branchid
                    });

                if (result <= 0)
                {
                    MessageBox.Show(
                        "Only In Progress tasks can be completed.",
                        "Unable to Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                UpdateRoomStatusAfterMaintenance(roomID);

                MessageBox.Show(
                    "Maintenance task completed successfully. Room status has been updated.",
                    "Task Completed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadMaintenance();
                UpdateDashboard();
                SelectUpdatedRecord(maintenanceID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error completing task: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // RESELECT UPDATED RECORD
        // ==========================================

        private void SelectUpdatedRecord(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return;

            foreach (DataGridViewRow row in DGV_MAINTENANCE.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (Convert.ToString(
                    row.Cells["MaintenanceID"].Value) == id)
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
                const string query = @"
                    SELECT
                        Status,
                        COUNT(*) AS Total
                    FROM MaintenanceTbl
                    WHERE BranchID = @BranchID
                    GROUP BY Status;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new { BranchID = branchid });

                int total = 0;
                int completed = 0;
                int inProgress = 0;
                int pending = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int count = Convert.ToInt32(row["Total"]);

                    string status =
                        Convert.ToString(row["Status"]) ?? "";

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
                    "Error updating dashboard: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
