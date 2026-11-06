using _4C_s_Management_System;
using System;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _4C_s_Management_System
{
    public partial class UserControlRoom : UserControl
    {
        private bool _isInitializing;

        public UserControlRoom()
        {
            InitializeComponent();
        }

        string roomId = "";

        private void UserControlRoom_Load(object sender, EventArgs e)
        {
            _isInitializing = true;
            RM_roomtypeCB.SelectedIndex = 0;
            RM_capacityCB.SelectedIndex = 0;
            _isInitializing = false;

            LoadRoomData();
            ApplyGridStyle();
            LoadRoomsdisplay();
        }

        public void LoadRoomData()
        {
            try
            {
                const string query =
                    "SELECT RoomId, BranchId, RoomType, Capacity, Rate, Status " +
                    "FROM TBLRoom " +
                    "ORDER BY RoomId ASC";

                AppDB.Instance.Table(
                    query,
                    RM_roomDGv,
                    header: new[]
                    {
                "Room ID",
                "Branch ID",
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
                ApplyGridStyle();
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

        public void SearchData()
        {
            string roomId = RM_searchtxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(roomId))
            {
                LoadRoomData();
                return;
            }

            try
            {
                const string query =
                    "SELECT RoomId, BranchId, RoomType, Capacity, Rate, Status " +
                    "FROM TBLRoom " +
                    "WHERE RoomId Like @RoomId " +
                    "ORDER BY RoomId ASC";

                AppDB.Instance.Table(
                    query,
                    new { RoomId = $"{roomId}%" },
                    RM_roomDGv,
                    header: new[]
                    {
                "Room ID",
                "Branch ID",
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
                ApplyGridStyle();
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

        public void Roomtypesort()
        {
            if (_isInitializing || RM_roomtypeCB.SelectedItem is null)
                return;

            try
            {
                if (RM_roomtypeCB.SelectedIndex == 0)
                {
                    LoadRoomData();
                    return;
                }

                string roomType = RM_roomtypeCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT RoomId, BranchId, RoomType, Capacity, Rate, Status " +
                    "FROM TBLRoom " +
                    "WHERE RoomType = @RoomType " +
                    "ORDER BY RoomId ASC";

                AppDB.Instance.Table(
                    query,
                    new { RoomType = roomType },
                    RM_roomDGv,
                    header: new[]
                    {
                "Room ID",
                "Branch ID",
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
                ApplyGridStyle();
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

        public void RoomCapacitysort()
        {
            if (_isInitializing || RM_capacityCB.SelectedItem is null)
                return;

            try
            {
                if (RM_capacityCB.SelectedIndex == 0)
                {
                    string roomType1 = RM_capacityCB.SelectedItem.ToString()!;
                    const string query1 =
                        "SELECT RoomId, BranchId, RoomType, Capacity, Rate, Status " +
                        "FROM TBLRoom " +
                        "ORDER BY RoomId ASC";

                    AppDB.Instance.Table(
                        query1,
                        RM_roomDGv,
                        header: new[]
                        {
                        "Room ID",
                        "Branch ID",
                        "Room Type",
                        "Capacity",
                        "Rate",
                        "Status"
                        });

                    ApplyGridStyle();
                    return;
                }
                string capacity = RM_capacityCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT RoomId, BranchId, RoomType, Capacity, Rate, Status " +
                    "FROM TBLRoom " +
                    "WHERE Capacity = @Capacity " +
                    "ORDER BY RoomId ASC";

                AppDB.Instance.Table(
                    query,
                    new { Capacity = capacity },
                    RM_roomDGv,
                    header: new[]
                    {
                "Room ID",
                "Branch ID",
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
                ApplyGridStyle();
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

        public void loaddisplay()
        {

            DataGridViewRow row = RM_roomDGv.SelectedRows[0];

                roomId = row.Cells[0].Value?.ToString() ?? "";

                RM_statusCB.SelectedItem = row.Cells[5].Value?.ToString() ?? "";
                RM_RoomIdLbl.Text = row.Cells[0].Value?.ToString() ?? "";
                RM_StatusLbl.Text = row.Cells[5].Value?.ToString() ?? "";
                RM_RoomTypeLbl.Text = row.Cells[2].Value?.ToString() ?? "";
                RM_CapacityLbl.Text = row.Cells[3].Value?.ToString() ?? "";
                RM_RateLbl.Text = row.Cells[4].Value?.ToString() ?? "";
                RM_DescriptionLbl.Text = $"Room {row.Cells[0].Value?.ToString() ?? ""} is a " +
                                         $"{row.Cells[2].Value?.ToString() ?? ""} with a capacity of " +
                                         $"{row.Cells[3].Value?.ToString() ?? ""} and a rate of " +
                                         $"{row.Cells[4].Value?.ToString() ?? ""}.";
                ApplyGridStyle();
            
        }
        private void RM_roomDGv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            loaddisplay();
        }

        private void search_roomid_textchange(object sender, EventArgs e)
        {
            if (_isInitializing)
                return;

            SearchData();
        }

        private void RoomtypeSelected_Indexchanged(object sender, EventArgs e)
        {
            Roomtypesort();
            ApplyGridStyle();
        }

        private void RoomStatusSelectedIndexchanged(object sender, EventArgs e)
        {
            RoomCapacitysort();
            ApplyGridStyle();
        }

        private void ApplyGridStyle()
        {
            RM_roomDGv.EnableHeadersVisualStyles = false;

            // Normal rows
            RM_roomDGv.DefaultCellStyle.BackColor = Color.White;
            RM_roomDGv.DefaultCellStyle.ForeColor = Color.FromArgb(92, 16, 35);

            // Selected rows
            RM_roomDGv.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(212, 167, 44);

            RM_roomDGv.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(92, 16, 35);

            // Alternating rows
            RM_roomDGv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(255, 248, 231);

            RM_roomDGv.AlternatingRowsDefaultCellStyle.ForeColor =
                Color.FromArgb(92, 16, 35);

            RM_roomDGv.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                Color.FromArgb(212, 167, 44);

            RM_roomDGv.AlternatingRowsDefaultCellStyle.SelectionForeColor =
                Color.FromArgb(92, 16, 35);

            if (RM_roomDGv.Rows.Count > 0)
            {
                RM_roomDGv.ClearSelection();
                RM_roomDGv.Rows[0].Selected = true;
                RM_roomDGv.CurrentCell = RM_roomDGv.Rows[0].Cells[0];
            }
        }

        private void RM_UpdateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                const string query =
                   "Update TBLRoom " +
                   "SET Status = @Status " +
                   "WHERE RoomId = @RoomId";

                AppDB.Instance.CUD(query, new
                {
                    Status = RM_statusCB.SelectedItem?.ToString(),
                    RoomId = roomId
                });
                LoadRoomData();
                LoadRoomsdisplay();
                loaddisplay();
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


        public void LoadRoomsdisplay()
        {
            try
            {
                const string query =
                    "SELECT " +
                    "COUNT(*) AS TotalRooms, " +
                    "SUM(CASE WHEN Status = 'Occupied' THEN 1 ELSE 0 END) AS OccupiedRooms, " +
                    "SUM(CASE WHEN Status = 'Available' THEN 1 ELSE 0 END) AS AvailableRooms " +
                    "FROM TBLRoom WHERE BranchId = @BranchId";

                var data = AppDB.Instance.TableData(query, new { BranchId = "BRC-001" });

                if (data.Rows.Count == 0)
                {
                    Display_TotaoRoom.Text = "0";
                    Display_Occupied.Text = "0";
                    Display_Available.Text = "0";
                    return;
                }

                var row = data.Rows[0];
                Display_TotaoRoom.Text = Convert.ToInt32(row["TotalRooms"]).ToString();
                Display_Occupied.Text = Convert.ToInt32(row["OccupiedRooms"]).ToString();
                Display_Available.Text = Convert.ToInt32(row["AvailableRooms"]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading rooms: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
