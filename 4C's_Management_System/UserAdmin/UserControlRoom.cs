
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlRoom : UserControl
    {
        private bool _isInitializing;
        private string roomId = "";
        private string branchId = AccessBranch_ByForm.BranchId;

        public UserControlRoom()
        {
            InitializeComponent();
        }

        private void UserControlRoom_Load(object sender, EventArgs e)
        {
            _isInitializing = true;

            branchId = AccessBranch_ByForm.BranchId;

            if (RM_roomtypeCB.Items.Count > 0)
                RM_roomtypeCB.SelectedIndex = 0;

            if (RM_capacityCB.Items.Count > 0)
                RM_capacityCB.SelectedIndex = 0;

            if (RM_statusCB.Items.Count > 0)
                RM_statusCB.SelectedIndex = 0;

            _isInitializing = false;

            LoadRoomData();
            LoadRoomsdisplay();
        }

        // ==========================================
        // LOAD ROOM DATA WITH LATEST MAINTENANCE
        // ==========================================

        public void LoadRoomData()
        {
            LoadRoomDataFiltered();
        }

        private void LoadRoomDataFiltered()
        {
            try
            {
                string search = RM_searchtxt.Text.Trim();

                string roomType =
                    RM_roomtypeCB.SelectedIndex > 0
                    ? RM_roomtypeCB.SelectedItem?.ToString() ?? ""
                    : "";

                string capacity =
                    RM_capacityCB.SelectedIndex > 0
                    ? RM_capacityCB.SelectedItem?.ToString() ?? ""
                    : "";

                string status =
                    RM_statusCB.SelectedIndex > 0
                    ? RM_statusCB.SelectedItem?.ToString() ?? ""
                    : "";

                const string query = @"
                    SELECT
                        R.RoomId,
                        R.RoomType,
                        R.Capacity,
                        R.Rate,
                        R.Status
                    FROM TBLRoom R

                    OUTER APPLY
                    (
                        SELECT TOP 1
                            M.Issue,
                            COALESCE(M.DateCompleted, M.DateStarted) AS LastUpdated
                        FROM MaintenanceTbl M
                        WHERE M.RoomID = R.RoomId
                          AND M.BranchID = R.BranchId
                        ORDER BY
                            COALESCE(M.DateCompleted, M.DateStarted) DESC,
                            M.DateStarted DESC
                    ) LatestMaintenance

                    WHERE R.BranchId = @BranchId
                      AND (@Search = '' OR R.RoomId LIKE '%' + @Search + '%')
                      AND (@RoomType = '' OR R.RoomType = @RoomType)
                      AND (@Capacity = '' OR R.Capacity = @Capacity)
                      AND (@Status = '' OR R.Status = @Status)

                    ORDER BY R.RoomId ASC;";

                AppDB.Instance.Table(
                    query,
                    new
                    {
                        BranchId = branchId,
                        Search = search,
                        RoomType = roomType,
                        Capacity = capacity,
                        Status = status
                    },
                    RM_roomDGv,
                    header: new[]
                    {
                        "Room ID",
                        "Room Type",
                        "Capacity",
                        "Rate",
                        "Status"
                    });

                // Keep the selected room after refreshing.
                SelectRoomAfterRefresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading room data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // SELECT ROOM AFTER REFRESH
        // ==========================================

        private void SelectRoomAfterRefresh()
        {
            if (RM_roomDGv.Rows.Count == 0)
            {
                roomId = "";
                ClearRoomDetails();
                return;
            }

            RM_roomDGv.ClearSelection();

            bool found = false;

            foreach (DataGridViewRow row in RM_roomDGv.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string currentId =
                    Convert.ToString(row.Cells[0].Value) ?? "";

                if (currentId == roomId && !string.IsNullOrWhiteSpace(roomId))
                {
                    row.Selected = true;

                    if (RM_roomDGv.Columns.Count > 0)
                        RM_roomDGv.CurrentCell = row.Cells[0];

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                DataGridViewRow firstRow = RM_roomDGv.Rows[0];

                firstRow.Selected = true;

                if (RM_roomDGv.Columns.Count > 0)
                    RM_roomDGv.CurrentCell = firstRow.Cells[0];
            }

            loaddisplay();
        }

        // ==========================================
        // CLEAR ROOM DETAILS
        // ==========================================

        private void ClearRoomDetails()
        {
            RM_RoomIdLbl.Text = "";
            RM_RoomTypeLbl.Text = "";
            RM_CapacityLbl.Text = "";
            RM_RateLbl.Text = "";
            RM_StatusLbl.Text = "";

            RM_DescriptionLbl.Text = "";

            RM_MaintenanceLbl.Text = "No Issue";
            RM_LastUpdatedLbl.Text = "N/A";

            RM_Panel_details_status.FillColor = Color.Gray;
        }

        // ==========================================
        // SEARCH ROOM
        // ==========================================

        public void SearchData()
        {
            LoadRoomDataFiltered();
        }

        private void search_roomid_textchange(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;

            LoadRoomDataFiltered();
        }

        // ==========================================
        // FILTER ROOM TYPE
        // ==========================================

        public void Roomtypesort()
        {
            if (_isInitializing)
                return;

            LoadRoomDataFiltered();
        }

        private void RoomtypeSelected_Indexchanged(object sender, EventArgs e)
        {
            Roomtypesort();
        }

        // ==========================================
        // FILTER ROOM CAPACITY
        // ==========================================

        public void RoomCapacitysort()
        {
            if (_isInitializing)
                return;

            LoadRoomDataFiltered();
        }

        private void RoomStatusSelectedIndexchanged(object sender, EventArgs e)
        {
            RoomCapacitysort();
        }

        // ==========================================
        // FILTER ROOM STATUS
        // ==========================================

        private void RM_statusCB_Selected_indexChanged(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;

            LoadRoomDataFiltered();
        }

        // ==========================================
        // DISPLAY ROOM DETAILS
        // ==========================================

        public void loaddisplay()
        {
            if (RM_roomDGv.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = RM_roomDGv.SelectedRows[0];

            if (row.IsNewRow || row.Cells.Count < 5)
                return;

            roomId = Convert.ToString(row.Cells[0].Value) ?? "";

            string roomType =
                Convert.ToString(row.Cells[1].Value) ?? "";

            string capacity =
                Convert.ToString(row.Cells[2].Value) ?? "";

            string rate =
                Convert.ToString(row.Cells[3].Value) ?? "";

            string status =
                Convert.ToString(row.Cells[4].Value) ?? "";

            RM_RoomIdLbl.Text = roomId;
            RM_RoomTypeLbl.Text = roomType;
            RM_CapacityLbl.Text = capacity;
            RM_RateLbl.Text = rate;
            RM_StatusLbl.Text = status;

            RM_StatusLbl.ForeColor = Color.White;

            // Status colors
            switch (status)
            {
                case "Available":
                    RM_Panel_details_status.FillColor =
                        Color.FromArgb(95, 145, 100);
                    RM_StatusLbl.BackColor =
                        Color.FromArgb(95, 145, 100);
                    break;

                case "Occupied":
                    RM_Panel_details_status.FillColor =
                        Color.FromArgb(169, 121, 56);
                    RM_StatusLbl.BackColor =
                        Color.FromArgb(169, 121, 56);
                    break;

                case "Maintenance":
                    RM_Panel_details_status.FillColor =
                        Color.FromArgb(168, 93, 85);
                    RM_StatusLbl.BackColor =
                        Color.FromArgb(168, 93, 85);
                    break;

                case "Reserved":
                    RM_Panel_details_status.FillColor =
                        Color.FromArgb(112, 126, 145);
                    RM_StatusLbl.BackColor =
                        Color.FromArgb(112, 126, 145);
                    break;

                default:
                    RM_Panel_details_status.FillColor = Color.Gray;
                    RM_StatusLbl.BackColor = Color.Gray;
                    break;
            }

            RM_DescriptionLbl.Text =
                $"Room {roomId} is a {roomType} with a capacity of " +
                $"{capacity} and a rate of {rate}.";

            LoadMaintenanceDetails(roomId);
        }

        // ==========================================
        // LOAD LATEST MAINTENANCE DETAILS
        // ==========================================

        private void LoadMaintenanceDetails(string selectedRoomId)
        {
            try
            {
                const string query = @"
                    SELECT TOP 1
                        Issue,
                        DateStarted,
                        DateCompleted
                    FROM MaintenanceTbl
                    WHERE RoomID = @RoomID
                      AND BranchID = @BranchID
                    ORDER BY
                        COALESCE(DateCompleted, DateStarted) DESC,
                        DateStarted DESC;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        RoomID = selectedRoomId,
                        BranchID = branchId
                    });

                if (data.Rows.Count == 0)
                {
                    RM_MaintenanceLbl.Text = "No Issue";
                    RM_LastUpdatedLbl.Text = "N/A";
                    return;
                }

                DataRow maintenance = data.Rows[0];

                string issue =
                    Convert.ToString(maintenance["Issue"]) ?? "No Issue";

                object dateCompleted = maintenance["DateCompleted"];
                object dateStarted = maintenance["DateStarted"];

                DateTime lastUpdated;

                if (dateCompleted != DBNull.Value)
                {
                    lastUpdated = Convert.ToDateTime(dateCompleted);
                }
                else if (dateStarted != DBNull.Value)
                {
                    lastUpdated = Convert.ToDateTime(dateStarted);
                }
                else
                {
                    RM_MaintenanceLbl.Text = issue;
                    RM_LastUpdatedLbl.Text = "N/A";
                    return;
                }

                RM_MaintenanceLbl.Text = issue;
                RM_LastUpdatedLbl.Text =
                    lastUpdated.ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
                RM_MaintenanceLbl.Text = "Unable to load";
                RM_LastUpdatedLbl.Text = "N/A";

                MessageBox.Show(
                    $"Error loading maintenance details: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RM_roomDGv_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            loaddisplay();
        }

        // ==========================================
        // UPDATE ROOM STATUS
        // ==========================================

        private void RM_UpdateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(roomId) ||
                RM_statusCB.SelectedItem == null)
            {
                MessageBox.Show(
                    "Select a room and choose a status first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Do not allow manual status changes while maintenance
                // is currently in progress for this room.
                const string checkMaintenance = @"
                    SELECT COUNT(*)
                    FROM MaintenanceTbl
                    WHERE RoomID = @RoomID
                      AND BranchID = @BranchID
                      AND Status = 'In Progress';";

                int activeMaintenance = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        checkMaintenance,
                        new
                        {
                            RoomID = roomId,
                            BranchID = branchId
                        }));

                if (activeMaintenance > 0)
                {
                    MessageBox.Show(
                        "This room currently has an ongoing maintenance task. " +
                        "Complete the maintenance task before changing its room status.",
                        "Room Under Maintenance",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                const string query = @"
                    UPDATE TBLRoom
                    SET Status = @Status
                    WHERE RoomId = @RoomId
                      AND BranchId = @BranchId;";

                AppDB.Instance.CUD(
                    query,
                    new
                    {
                        Status = RM_statusCB.SelectedItem.ToString(),
                        RoomId = roomId,
                        BranchId = branchId
                    });

                MessageBox.Show(
                    "Room status updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RefreshRoomData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating room status: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // REFRESH ROOM DATA AND SUMMARY
        // ==========================================

        public void RefreshRoomData()
        {
            LoadRoomDataFiltered();
            LoadRoomsdisplay();
        }

        public void LoadRoomsdisplay()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(*) AS TotalRooms,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Occupied'
                            THEN 1 ELSE 0 END
                        ), 0) AS OccupiedRooms,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Available'
                            THEN 1 ELSE 0 END
                        ), 0) AS AvailableRooms,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Maintenance'
                            THEN 1 ELSE 0 END
                        ), 0) AS MaintenanceRooms

                    FROM TBLRoom
                    WHERE BranchId = @BranchId;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId });

                if (data.Rows.Count == 0)
                {
                    Display_TotaoRoom.Text = "0";
                    Display_Occupied.Text = "0";
                    Display_Available.Text = "0";
                    return;
                }

                DataRow row = data.Rows[0];

                Display_TotaoRoom.Text =
                    Convert.ToInt32(row["TotalRooms"]).ToString();

                Display_Occupied.Text =
                    Convert.ToInt32(row["OccupiedRooms"]).ToString();

                Display_Available.Text =
                    Convert.ToInt32(row["AvailableRooms"]).ToString();

                // If you have a Maintenance count label,
                // assign it here:
                // Display_Maintenance.Text =
                //     Convert.ToInt32(row["MaintenanceRooms"]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading room summary: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // EXISTING EMPTY DESIGNER EVENTS
        // ==========================================

        private void guna2vSeparator1_Click(object sender, EventArgs e)
        {
        }

        private void RM_Panel_details_status_Paint(
            object sender,
            PaintEventArgs e)
        {
        }

        private void RM_StatusLbl_Click(object sender, EventArgs e)
        {
        }
    }
}
