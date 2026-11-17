
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Room : UserControl
    {
        // ==========================================
        // VARIABLES
        // ==========================================

        private bool _isInitializing = false;
        private string roomId = "";

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public UserControl_Admin_Room()
        {
            InitializeComponent();

            guna2Button1.Click -= guna2Button1_Click;
            guna2Button1.Click += guna2Button1_Click;
        }

        // ========================================
        // ROOM STATISTICS
        // ========================================

        public void rommstatlabel()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(*) AS TotalRooms,
                        SUM(CASE WHEN Status = 'Occupied' THEN 1 ELSE 0 END) AS OccupiedRooms,
                        SUM(CASE WHEN Status = 'Available' THEN 1 ELSE 0 END) AS AvailableRooms,
                        SUM(CASE WHEN Status = 'Maintenance' THEN 1 ELSE 0 END) AS MaintenanceRooms
                    FROM TBLRoom;";

                DataTable result = AppDB.Instance.TableData(query, null);

                if (result != null && result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];

                    total_roomLBL.Text =
                        Convert.ToString(row["TotalRooms"]);

                    Occupied_LBL.Text =
                        Convert.ToString(row["OccupiedRooms"]);

                    Available_LBL.Text =
                        Convert.ToString(row["AvailableRooms"]);

                    Maintenance_LBL.Text =
                        Convert.ToString(row["MaintenanceRooms"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading room statistics: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // LOAD ROOM DATA
        // ========================================

        public void loaddata()
        {
            try
            {
                const string query = @"
                    SELECT
                        RoomId,
                        RoomType,
                        Capacity,
                        Rate,
                        Status,
                        BranchId
                    FROM TBLRoom
                    ORDER BY RoomId ASC;";

                AppDB.Instance.Table(
                    query,
                    null,
                    RoomDGV,
                    header: new[]
                    {
                        "Room ID",
                        "Room Type",
                        "Capacity",
                        "Rate",
                        "Status",
                        "Branch ID"
                    });

                HideBranchColumn();

                if (RoomDGV.Rows.Count > 0)
                {
                    RoomDGV.ClearSelection();

                    DataGridViewRow row = RoomDGV.Rows[0];

                    if (!row.IsNewRow)
                    {
                        row.Selected = true;
                        RoomDGV.CurrentCell = row.Cells[0];

                        loaddisplayLabel(RoomDGV);
                    }
                    else
                    {
                        ClearRoomDetails();
                    }
                }
                else
                {
                    ClearRoomDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading rooms: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // HIDE BRANCH ID COLUMN
        // ========================================

        private void HideBranchColumn()
        {
            if (RoomDGV.Columns.Count > 5)
            {
                RoomDGV.Columns[5].Visible = false;
            }
        }

        // ========================================
        // DATAGRIDVIEW CELL CLICK EVENT
        // ========================================

        private void DGV_room_cellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = RoomDGV.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            RoomDGV.ClearSelection();

            row.Selected = true;
            RoomDGV.CurrentCell = row.Cells[0];

            loaddisplayLabel(RoomDGV);
        }

        // ========================================
        // DISPLAY SELECTED ROOM DETAILS
        // ========================================

        public void loaddisplayLabel(DataGridView e)
        {
            try
            {
                if (e == null ||
                    e.CurrentRow == null ||
                    e.CurrentRow.IsNewRow)
                {
                    ClearRoomDetails();
                    return;
                }

                DataGridViewRow row = e.CurrentRow;

                if (row.Cells.Count < 6)
                {
                    ClearRoomDetails();
                    return;
                }

                roomId =
                    Convert.ToString(row.Cells[0].Value) ?? "";

                string roomType =
                    Convert.ToString(row.Cells[1].Value) ?? "";

                string capacity =
                    Convert.ToString(row.Cells[2].Value) ?? "";

                string rate =
                    Convert.ToString(row.Cells[3].Value) ?? "";

                string status =
                    Convert.ToString(row.Cells[4].Value) ?? "";

                string branchId =
                    Convert.ToString(row.Cells[5].Value) ?? "";

                // SELECTED ROOM INFORMATION

                BranchIdTXT.Text = branchId;

                RoomIdLBL.Text = roomId;
                RoomtypeLBL.Text = roomType;
                capacityLBL.Text = capacity;
                PriceLBl.Text = rate;
                statusLBLL.Text = status;

                // FILL INPUT CONTROLS

                RoomidTXT.Text = roomId;

                rd_roomtype.Text = roomType;
                rd_capacity.Text = capacity;
                rateTxt.Text = rate;
                statusTxt.Text = status;

                DescriptionLBL.Text =
                    $"Room {roomId} is a {roomType} with a capacity of " +
                    $"{capacity} and a rate of {rate}.";

                // LOAD MAINTENANCE DETAILS

                LoadMaintenanceDetails(roomId, branchId);

                // STATUS COLORS

                statusLBLL.ForeColor = Color.White;

                switch (status.Trim().ToLower())
                {
                    case "available":

                        panel_status.FillColor =
                            Color.FromArgb(95, 145, 100);

                        statusLBLL.BackColor =
                            Color.FromArgb(95, 145, 100);

                        break;

                    case "occupied":

                        panel_status.FillColor =
                            Color.FromArgb(169, 121, 56);

                        statusLBLL.BackColor =
                            Color.FromArgb(169, 121, 56);

                        break;

                    case "maintenance":

                        panel_status.FillColor =
                            Color.FromArgb(168, 93, 85);

                        statusLBLL.BackColor =
                            Color.FromArgb(168, 93, 85);

                        break;

                    case "reserved":

                        panel_status.FillColor =
                            Color.FromArgb(112, 126, 145);

                        statusLBLL.BackColor =
                            Color.FromArgb(112, 126, 145);

                        break;

                    default:

                        panel_status.FillColor = Color.Gray;
                        statusLBLL.BackColor = Color.Gray;

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying room details: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // LOAD MAINTENANCE DETAILS
        // ========================================

        private void LoadMaintenanceDetails(
            string selectedRoomId,
            string selectedBranchId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedRoomId) ||
                    string.IsNullOrWhiteSpace(selectedBranchId))
                {
                    MaintenanceLBL.Text = "No Maintenance Issue";
                    LastUpdateLBL.Text = "N/A";
                    return;
                }

                const string query = @"
                    SELECT TOP 1
                        Issue,
                        DateStarted,
                        DateCompleted
                    FROM MaintenanceTbl
                    WHERE RoomID = @RoomID
                      AND BranchID = @BranchID
                    ORDER BY DateStarted DESC;";

                DataTable result = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        RoomID = selectedRoomId,
                        BranchID = selectedBranchId
                    });

                if (result == null || result.Rows.Count == 0)
                {
                    MaintenanceLBL.Text = "No Maintenance Issue";
                    LastUpdateLBL.Text = "N/A";
                    return;
                }

                DataRow maintenance = result.Rows[0];

                string issue =
                    Convert.ToString(maintenance["Issue"]) ?? "";

                MaintenanceLBL.Text =
                    string.IsNullOrWhiteSpace(issue)
                        ? "No Description"
                        : issue;

                DateTime? latestDate = null;

                if (maintenance["DateCompleted"] != DBNull.Value)
                {
                    latestDate = Convert.ToDateTime(
                        maintenance["DateCompleted"]);
                }
                else if (maintenance["DateStarted"] != DBNull.Value)
                {
                    latestDate = Convert.ToDateTime(
                        maintenance["DateStarted"]);
                }

                LastUpdateLBL.Text = latestDate.HasValue
                    ? latestDate.Value.ToString("MMM dd, yyyy")
                    : "N/A";
            }
            catch (Exception ex)
            {
                MaintenanceLBL.Text = "Unable to load maintenance";
                LastUpdateLBL.Text = "N/A";

                MessageBox.Show(
                    "Error loading maintenance details: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // CLEAR ROOM DETAILS
        // ========================================

        private void ClearRoomDetails()
        {
            roomId = "";

            BranchIdTXT.Clear();

            RoomIdLBL.Text = "";
            RoomtypeLBL.Text = "";
            capacityLBL.Text = "";
            PriceLBl.Text = "";
            statusLBLL.Text = "";

            DescriptionLBL.Text = "";

            MaintenanceLBL.Text = "No Maintenance Issue";
            LastUpdateLBL.Text = "N/A";

            panel_status.FillColor = Color.Gray;
            statusLBLL.BackColor = Color.Gray;
        }

        // ========================================
        // USER CONTROL LOAD EVENT
        // ========================================

        private void UserControl_Admin_Room_Load(
            object sender,
            EventArgs e)
        {
            _isInitializing = true;

            try
            {
                if (CB_RM_Type.Items.Count > 0)
                    CB_RM_Type.SelectedIndex = 0;

                if (cb_capacity.Items.Count > 0)
                    cb_capacity.SelectedIndex = 0;

                if (cb_status.Items.Count > 0)
                    cb_status.SelectedIndex = 0;
            }
            finally
            {
                _isInitializing = false;
            }

            loaddata();
            rommstatlabel();
        }

        // ========================================
        // FILTER ROOMS
        // ========================================

        public void FilterRooms()
        {
            try
            {
                string roomType = CB_RM_Type.Text.Trim();
                string capacity = cb_capacity.Text.Trim();
                string status = cb_status.Text.Trim();
                string search = SearchTXT.Text.Trim();

                const string query = @"
                    SELECT
                        RoomId,
                        RoomType,
                        Capacity,
                        Rate,
                        Status,
                        BranchId
                    FROM TBLRoom
                    WHERE
                        (
                            @RoomType = 'All type'
                            OR RoomType = @RoomType
                        )
                        AND
                        (
                            @Capacity = 'All type'
                            OR Capacity = @Capacity
                        )
                        AND
                        (
                            @Status = 'All status'
                            OR Status = @Status
                        )
                        AND
                        (
                            @Search = ''
                            OR RoomId LIKE '%' + @Search + '%'
                            OR RoomType LIKE '%' + @Search + '%'
                            OR CAST(Capacity AS VARCHAR(50)) LIKE '%' + @Search + '%'
                            OR Status LIKE '%' + @Search + '%'
                        )
                    ORDER BY RoomId ASC;";

                AppDB.Instance.Table(
                    query,
                    new
                    {
                        RoomType = roomType,
                        Capacity = capacity,
                        Status = status,
                        Search = search
                    },
                    RoomDGV,
                    header: new[]
                    {
                        "Room ID",
                        "Room Type",
                        "Capacity",
                        "Rate",
                        "Status",
                        "Branch ID"
                    });

                HideBranchColumn();

                if (RoomDGV.Rows.Count > 0)
                {
                    RoomDGV.ClearSelection();

                    DataGridViewRow row = RoomDGV.Rows[0];

                    if (!row.IsNewRow)
                    {
                        row.Selected = true;
                        RoomDGV.CurrentCell = row.Cells[0];

                        loaddisplayLabel(RoomDGV);
                    }
                    else
                    {
                        ClearRoomDetails();
                    }
                }
                else
                {
                    ClearRoomDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error filtering rooms: " + ex.Message,
                    "Filter Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // SEARCH AND FILTER EVENTS
        // ========================================

        private void search_TC(object sender, EventArgs e)
        {
            if (!_isInitializing)
                FilterRooms();
        }

        private void Roomtype_SIC(object sender, EventArgs e)
        {
            if (!_isInitializing)
                FilterRooms();
        }

        private void RoomCapacity_SIC(object sender, EventArgs e)
        {
            if (!_isInitializing)
                FilterRooms();
        }

        private void Roomstat_SIC(object sender, EventArgs e)
        {
            if (!_isInitializing)
                FilterRooms();
        }

        // ========================================
        // ADD ROOM
        // ========================================

        private void adddroom_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string branchId = BranchIdTXT.Text.Trim();

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    MessageBox.Show(
                        "Please enter a Branch ID.",
                        "Missing Branch ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string newRoomId =
                    IDGenerator.GenerateNextID("RM", branchId);

                RoomidTXT.Text = newRoomId;

                string roomType = rd_roomtype.Text.Trim();
                string capacity = rd_capacity.Text.Trim();
                string status = statusTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(newRoomId) ||
                    string.IsNullOrWhiteSpace(roomType) ||
                    string.IsNullOrWhiteSpace(capacity) ||
                    string.IsNullOrWhiteSpace(status) ||
                    string.IsNullOrWhiteSpace(rateTxt.Text))
                {
                    MessageBox.Show(
                        "Please complete all room information.",
                        "Missing Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(
                    rateTxt.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal rate) || rate < 0)
                {
                    MessageBox.Show(
                        "Please enter a valid room rate.",
                        "Invalid Rate",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to add this room?",
                    "Confirm Add Room",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                const string query = @"
                    INSERT INTO TBLRoom
                    (
                        RoomId,
                        BranchId,
                        RoomType,
                        Capacity,
                        Rate,
                        Status
                    )
                    VALUES
                    (
                        @RoomId,
                        @BranchId,
                        @RoomType,
                        @Capacity,
                        @Rate,
                        @Status
                    );";

                AppDB.Instance.CUD(
                    query,
                    new
                    {
                        RoomId = newRoomId,
                        BranchId = branchId,
                        RoomType = roomType,
                        Capacity = capacity,
                        Rate = rate,
                        Status = status
                    });

                MessageBox.Show(
                    "Room successfully added!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearRoomInputFields();

                FilterRooms();
                rommstatlabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding room: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // UPDATE ROOM
        // ========================================

        private void upromm_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string oldRoomId = roomId;
                string newRoomId = RoomidTXT.Text.Trim();
                string branchId = BranchIdTXT.Text.Trim();

                string roomType = rd_roomtype.Text.Trim();
                string capacity = rd_capacity.Text.Trim();
                string status = statusTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(oldRoomId))
                {
                    MessageBox.Show(
                        "Please select a room from the table first.",
                        "No Room Selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(newRoomId) ||
                    string.IsNullOrWhiteSpace(branchId) ||
                    string.IsNullOrWhiteSpace(roomType) ||
                    string.IsNullOrWhiteSpace(capacity) ||
                    string.IsNullOrWhiteSpace(status) ||
                    string.IsNullOrWhiteSpace(rateTxt.Text))
                {
                    MessageBox.Show(
                        "Please complete all room information.",
                        "Missing Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(
                    rateTxt.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal rate) || rate < 0)
                {
                    MessageBox.Show(
                        "Please enter a valid room rate.",
                        "Invalid Rate",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to update this room?",
                    "Confirm Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                const string query = @"
                    UPDATE TBLRoom
                    SET
                        RoomId = @NewRoomId,
                        RoomType = @RoomType,
                        Capacity = @Capacity,
                        Rate = @Rate,
                        Status = @Status
                    WHERE
                        RoomId = @OldRoomId
                        AND BranchId = @BranchId;";

                AppDB.Instance.CUD(
                    query,
                    new
                    {
                        NewRoomId = newRoomId,
                        OldRoomId = oldRoomId,
                        BranchId = branchId,
                        RoomType = roomType,
                        Capacity = capacity,
                        Rate = rate,
                        Status = status
                    });

                MessageBox.Show(
                    "Room successfully updated!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearRoomInputFields();

                FilterRooms();
                rommstatlabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating room: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // DELETE ROOM
        // ========================================

        private void _deleteBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedRoomId = roomId;
                string branchId = BranchIdTXT.Text.Trim();

                if (string.IsNullOrWhiteSpace(selectedRoomId))
                {
                    MessageBox.Show(
                        "Please select a room from the table first.",
                        "No Room Selected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(branchId))
                {
                    MessageBox.Show(
                        "The selected room has no Branch ID.",
                        "Missing Branch ID",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "Are you sure you want to delete Room " + selectedRoomId + "?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

                const string query = @"
                    DELETE FROM TBLRoom
                    WHERE RoomId = @RoomId
                      AND BranchId = @BranchId;";

                AppDB.Instance.CUD(
                    query,
                    new
                    {
                        RoomId = selectedRoomId,
                        BranchId = branchId
                    });

                MessageBox.Show(
                    "Room successfully deleted!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearRoomInputFields();

                FilterRooms();
                rommstatlabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error deleting room. It may have existing reservations or maintenance records.\n\n"
                    + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ========================================
        // CLEAR BUTTON
        // Button Name: guna2Button1
        // ROOM DETAILS ONLY
        // ========================================

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ClearRoomInputFields();
            ClearRoomDetails();

            // HUWAG I-CLEAR ANG RoomDGV.
            // Mananatili ang records at current selection.
        }

        // ========================================
        // CLEAR INPUT FIELDS
        // ========================================

        private void ClearRoomInputFields()
        {
            RoomidTXT.Clear();
            BranchIdTXT.Clear();

            rd_roomtype.SelectedIndex = -1;
            rd_capacity.SelectedIndex = -1;

            rateTxt.Clear();

            statusTxt.SelectedIndex = -1;

            roomId = "";
        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
