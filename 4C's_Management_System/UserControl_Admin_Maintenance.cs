
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Maintenance : UserControl
    {
        public UserControl_Admin_Maintenance()
        {
            InitializeComponent();
        }

        // =========================================
        // LOAD DATAGRIDVIEW
        // =========================================

        public void loaddgv()
        {
            try
            {
                const string mquery = @"
                    SELECT
                        M.MaintenanceID,
                        M.BranchID,
                        M.RoomID,
                        R.RoomType,
                        M.Issue,
                        M.DateStarted,
                        M.DateCompleted,
                        M.Remarks,
                        M.Status
                    FROM MaintenanceTbl M
                    INNER JOIN TBLRoom R
                        ON M.RoomID = R.RoomId
                    ORDER BY M.DateStarted DESC";

                AppDB.Instance.Table(
                    mquery,
                    new { },
                    maintenanceDGV
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading maintenance records: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // USERCONTROL LOAD
        // =========================================

        private void UserControl_Admin_Maintenance_Load(
            object sender, EventArgs e)
        {
            loaddgv();
            displaylabel();
            roomtypelbl.SelectedIndex = 0;
            statuCB.SelectedIndex = 0;
        }

        // =========================================
        // DISPLAY SUMMARY LABELS
        // =========================================

        public void displaylabel()
        {
            try
            {
                const string dis = @"
                    SELECT
                        COUNT(*) AS TotalRequest,
                        SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS Completed,
                        SUM(CASE WHEN Status = 'In Progress' THEN 1 ELSE 0 END) AS InProgress,
                        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Pending
                    FROM MaintenanceTbl";

                var data = AppDB.Instance.TableData(dis, new { });

                if (data.Rows.Count > 0)
                {
                    totre_lbl.Text = data.Rows[0]["TotalRequest"].ToString();
                    comlbl.Text = data.Rows[0]["Completed"].ToString();
                    inproglbl.Text = data.Rows[0]["InProgress"].ToString();
                    penlbl.Text = data.Rows[0]["Pending"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying maintenance summary: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // SEARCH AND FILTER
        // =========================================

        public void ApplyMaintenanceFilters()
        {
            try
            {
                string search = searchlbl.Text.Trim();
                string roomType = roomtypelbl.Text;
                string status = statuCB.Text;

                string query = @"
                    SELECT
                        M.MaintenanceID,
                        M.BranchID,
                        M.RoomID,
                        R.RoomType,
                        M.Issue,
                        M.DateStarted,
                        M.DateCompleted,
                        M.Remarks,
                        M.Status
                    FROM MaintenanceTbl M
                    INNER JOIN TBLRoom R
                        ON M.RoomID = R.RoomId
                    WHERE
                    (
                        M.MaintenanceID LIKE @Search
                        OR M.RoomID LIKE @Search
                        OR M.Issue LIKE @Search
                        OR ISNULL(M.Remarks, '') LIKE @Search
                    )";

                if (!string.IsNullOrWhiteSpace(roomType) &&
                    roomType != "All Room")
                {
                    query += " AND R.RoomType = @RoomType";
                }

                if (!string.IsNullOrWhiteSpace(status) &&
                    status != "All Status")
                {
                    query += " AND M.Status = @Status";
                }

                query += " ORDER BY M.DateStarted DESC";

                var data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        Search = "%" + search + "%",
                        RoomType = roomType,
                        Status = status
                    });

                maintenanceDGV.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error filtering maintenance records: " + ex.Message,
                    "Maintenance Filter Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // SEARCH TEXTBOX
        // =========================================

        private void searchTC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }

        // =========================================
        // ROOM TYPE FILTER
        // =========================================

        private void roomtypeSIC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }

        // =========================================
        // STATUS FILTER
        // =========================================

        private void roomstatusSIC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }
    }
}
