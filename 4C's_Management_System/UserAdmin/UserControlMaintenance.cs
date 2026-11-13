
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlMaintenance : UserControl
    {
        private bool isLoading = false;
        private bool isNewRequest = false;

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

        public string branchid = AccessBranch_ByForm.BranchId;

        // LOAD
        private void UserControlMaintenance_Load(object sender, EventArgs e)
        {
            try
            {
                isLoading = true;

                CB_ROOM_TYPE.Items.Clear();
                CB_ROOM_TYPE.Items.Add("All Room Types");

                DataTable roomTypes = AppDB.Instance.TableData(
                    "SELECT DISTINCT RoomType FROM TBLRoom ORDER BY RoomType");

                foreach (DataRow row in roomTypes.Rows)
                {
                    CB_ROOM_TYPE.Items.Add(row["RoomType"].ToString());
                }

                CB_ROOM_TYPE.SelectedIndex = 0;

                CB_STATUS.Items.Clear();
                CB_STATUS.Items.AddRange(new object[]
                {
                    "All Status",
                    "Pending",
                    "In Progress",
                    "Completed"
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
                MessageBox.Show("Error loading maintenance: " + ex.Message);
            }
        }

        // LOAD AND FILTER DATA

        private void LoadMaintenance()
        {
            try
            {
                string query = @"SELECT MaintenanceID, RoomID, Issue, Status FROM MaintenanceTbl ORDER BY MaintenanceID DESC";

                DataTable dt = AppDB.Instance.TableData(query);

                DGV_MAINTENANCE.DataSource = null;
                DGV_MAINTENANCE.DataSource = dt;

                DGV_MAINTENANCE.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to refresh maintenance list: " + ex.Message);
            }
        }


        // SEARCH AND FILTER EVENTS
        private void TB_SEARCH_TextChanged(object sender, EventArgs e)
        {
            LoadMaintenance();
        }

        private void CB_ROOM_TYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMaintenance();
        }

        private void CB_STATUS_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMaintenance();
        }

        // SELECT ROW - DISPLAY DETAILS
        private void DGV_MAINTENANCE_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row = DGV_MAINTENANCE.Rows[e.RowIndex];

                if (row.Cells["MaintenanceID"].Value == null)
                    return;

                isNewRequest = false;

                maintenance_id_info.Text=
                    row.Cells["MaintenanceID"].Value.ToString();

                txb_Room_id.Text =
                    row.Cells["RoomID"].Value.ToString();

                txb_issue.Text =
                    row.Cells["Issue"].Value.ToString();

                txb_remarks.Text =
                    row.Cells["Remarks"].Value?.ToString();

                // RIGHT SIDE DETAILS
                maintenance_id_info.Text =
                    row.Cells["MaintenanceID"].Value.ToString();

                mtn_issue_info.Text =
                    row.Cells["Issue"].Value.ToString();

                description_info_txb.Text =
                    row.Cells["Description"].Value?.ToString();

                Date_std_info.Text =
                    row.Cells["DateStarted"].Value == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(row.Cells["DateStarted"].Value)
                        .ToString("MMM dd, yyyy");

                date_ctd_info.Text =
                    row.Cells["DateCompleted"].Value == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(row.Cells["DateCompleted"].Value)
                        .ToString("MMM dd, yyyy");

                remarks_info.Text =
                    row.Cells["Remarks"].Value?.ToString();

                Maintenance_StatusLBL.Text =
                    row.Cells["Status"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying details: " + ex.Message);
            }
        }

        // ADD NEW REQUEST
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
                string maintenanceID = "MNT" + DateTime.Now.ToString("yyyyMMddHHmmss");

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
                        id = IDGenerator.GenerateNextID("MTN",  branchid),
                        roomID = txb_Room_id.Text.Trim(),
                        issue = txb_issue.Text.Trim(),
                        remarks = txb_remarks.Text.Trim()
                    });

                if (result > 0)
                {
                    MessageBox.Show("Maintenance request added successfully!");

                    ClearFields();
                    LoadMaintenance();
                    UpdateDashboard();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding request: " + ex.Message);
            }
        }

        // UPDATE SELECTED REQUEST
        private void btn_update_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show("Please select a maintenance record first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txb_Room_id.Text) ||
                string.IsNullOrWhiteSpace(txb_issue.Text))
            {
                MessageBox.Show("Room ID and Issue are required.");
                return;
            }

            try
            {
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
                }
                else
                {
                    MessageBox.Show("No record was updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record: " + ex.Message);
            }
        }

        // CANCEL / CLEAR
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
            description_info_txb.Text = "";
            Date_std_info.Text = "";
            date_ctd_info.Text = "";
            remarks_info.Text = "";
            Maintenance_StatusLBL.Text = "";

            DGV_MAINTENANCE.ClearSelection();
        }

        // START TASK
        private void btn_start_task_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show("Please select a maintenance record first.");
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
                    MessageBox.Show("Only Pending tasks can be started.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting task: " + ex.Message);
            }
        }

        // END TASK
        private void btn_end_task_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_maintenance_id.Text))
            {
                MessageBox.Show("Please select a maintenance record first.");
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
                    MessageBox.Show("Only In Progress tasks can be completed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error completing task: " + ex.Message);
            }
        }

        // RESELECT UPDATED RECORD
        private void SelectUpdatedRecord()
        {
            string id = tb_maintenance_id.Text;

            foreach (DataGridViewRow row in DGV_MAINTENANCE.Rows)
            {
                if (row.Cells["MaintenanceID"].Value?.ToString() == id)
                {
                    row.Selected = true;
                    DGV_MAINTENANCE.CurrentCell = row.Cells[0];

                    DGV_MAINTENANCE_CellClick(
                        DGV_MAINTENANCE,
                        new DataGridViewCellEventArgs(0, row.Index));

                    return;
                }
            }
        }

        // DASHBOARD COUNTS
        private void UpdateDashboard()
        {
            try
            {
                DataTable dt = AppDB.Instance.TableData(@"
                    SELECT Status, COUNT(*) AS Total
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
                MessageBox.Show("Error updating dashboard: " + ex.Message);
            }
        }
    }
}
