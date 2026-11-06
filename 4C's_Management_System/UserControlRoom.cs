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
        string branchId = "brc-001";

        private void UserControlRoom_Load(object sender, EventArgs e)
        {
            _isInitializing = true;
            RM_roomtypeCB.SelectedIndex = 0;
            RM_capacityCB.SelectedIndex = 0;
            _isInitializing = false;

            LoadRoomData();
            LoadRoomsdisplay();
        }

        public void LoadRoomData()
        {
            try
            {
                const string query =
                    "SELECT RoomId, RoomType, Capacity, Rate, Status " +
                    "FROM TBLRoom " +
                    "ORDER BY RoomId ASC";

                AppDB.Instance.Table(
                    query,
                    RM_roomDGv,
                    header: new[]
                    {
                    "Room ID",
                    "Room Type",
                    "Capacity",
                    "Rate",
                    "Status"
                    });
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
                    "SELECT RoomId, RoomType, Capacity, Rate, Status " +
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
                    "Room Type",
                    "Capacity",
                    "Rate",
                    "Status"
                    });
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
                    "SELECT RoomId, RoomType, Capacity, Rate, Status " +
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
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
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
                        "SELECT RoomId, RoomType, Capacity, Rate, Status " +
                        "FROM TBLRoom " +
                        "ORDER BY RoomId ASC";

                    AppDB.Instance.Table(
                        query1,
                        RM_roomDGv,
                        header: new[]
                        {
                        "Room ID",
                        "Room Type",
                        "Capacity",
                        "Rate",
                        "Status"
                        });

                    return;
                }
                string capacity = RM_capacityCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT RoomId, RoomType, Capacity, Rate, Status " +
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
                "Room Type",
                "Capacity",
                "Rate",
                "Status"
                    });
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

                RM_statusCB.SelectedItem = row.Cells[4].Value?.ToString() ?? "";

                RM_RoomIdLbl.Text = row.Cells[0].Value?.ToString() ?? "";
                RM_RoomTypeLbl.Text = row.Cells[1].Value?.ToString() ?? "";
                RM_CapacityLbl.Text = row.Cells[2].Value?.ToString() ?? "";
                RM_RateLbl.Text = row.Cells[3].Value?.ToString() ?? "";
                RM_StatusLbl.Text = row.Cells[4].Value?.ToString() ?? "";
                RM_DescriptionLbl.Text = $"Room {row.Cells[0].Value?.ToString() ?? ""} is a " +
                                         $"{row.Cells[2].Value?.ToString() ?? ""} with a capacity of " +
                                         $"{row.Cells[3].Value?.ToString() ?? ""} and a rate of " +
                                         $"{row.Cells[4].Value?.ToString() ?? ""}.";            
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
        }

        private void RoomStatusSelectedIndexchanged(object sender, EventArgs e)
        {
            RoomCapacitysort();
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

                var data = AppDB.Instance.TableData(query, new { BranchId = branchId });

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
