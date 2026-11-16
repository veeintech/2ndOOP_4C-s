
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

            Load -= UserControl_Admin_Maintenance_Load;
            Load += UserControl_Admin_Maintenance_Load;

            MM_searchTxt.TextChanged -= searchTC;
            MM_searchTxt.TextChanged += searchTC;

            MM_roomTypeCB.SelectedIndexChanged -= roomtypeSIC;
            MM_roomTypeCB.SelectedIndexChanged += roomtypeSIC;

            MM_statusCB.SelectedIndexChanged -= roomstatusSIC;
            MM_statusCB.SelectedIndexChanged += roomstatusSIC;

            MM_MaintenanceDGV.CellClick -= MM_MaintenanceDGV_CellClick;
            MM_MaintenanceDGV.CellClick += MM_MaintenanceDGV_CellClick;

            MM_acceptBtn.Click -= MM_acceptBtn_Click;
            MM_acceptBtn.Click += MM_acceptBtn_Click;

            MM_rejectBtn.Click -= MM_rejectBtn_Click;
            MM_rejectBtn.Click += MM_rejectBtn_Click;
        }

        // =========================================
        // USERCONTROL LOAD
        // =========================================

        private void UserControl_Admin_Maintenance_Load(
            object sender, EventArgs e)
        {
            MM_roomTypeCB.Items.Clear();
            MM_roomTypeCB.Items.Add("All Room Types");
            MM_roomTypeCB.Items.Add("Hotel");
            MM_roomTypeCB.Items.Add("Apartment");
            MM_roomTypeCB.Items.Add("Transient");
            MM_roomTypeCB.SelectedIndex = 0;

            MM_statusCB.Items.Clear();
            MM_statusCB.Items.Add("All Status");
            MM_statusCB.Items.Add("Pending");
            MM_statusCB.Items.Add("Approved");
            MM_statusCB.Items.Add("In Progress");
            MM_statusCB.Items.Add("Completed");
            MM_statusCB.Items.Add("Cancelled");
            MM_statusCB.Items.Add("Rejected");
            MM_statusCB.SelectedIndex = 0;

            MM_MaintenanceDGV.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            MM_MaintenanceDGV.MultiSelect = false;
            MM_MaintenanceDGV.ReadOnly = true;
            MM_MaintenanceDGV.AutoGenerateColumns = true;

            ClearDetails();

            displaylabel();
            loaddgv();
        }

        // =========================================
        // LOAD ALL MAINTENANCE RECORDS
        // =========================================

        public void loaddgv()
        {
            ApplyMaintenanceFilters();
        }

        public void ApplyMaintenanceFilters()
        {
            try
            {
                string search = MM_searchTxt.Text.Trim();
                string roomType = MM_roomTypeCB.Text;
                string status = MM_statusCB.Text;

                string query = @"
                    SELECT
                        M.MaintenanceID,
                        M.RoomID,
                        R.RoomType,
                        M.Issue,
                        M.Status
                    FROM MaintenanceTbl M
                    LEFT JOIN TBLRoom R
                        ON M.RoomID = R.RoomId
                    WHERE
                    (
                        M.MaintenanceID LIKE @Search
                        OR M.RoomID LIKE @Search
                        OR ISNULL(R.RoomType, '') LIKE @Search
                        OR M.Issue LIKE @Search
                    )";

                if (roomType != "All Room Types" &&
                    !string.IsNullOrWhiteSpace(roomType))
                {
                    query += " AND R.RoomType = @RoomType";
                }

                if (status != "All Status" &&
                    !string.IsNullOrWhiteSpace(status))
                {
                    query += " AND M.Status = @Status";
                }

                query += " ORDER BY M.DateStarted DESC";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        Search = "%" + search + "%",
                        RoomType = roomType,
                        Status = status
                    });

                MM_MaintenanceDGV.DataSource = data;

                MM_MaintenanceDGV.ClearSelection();
                ClearDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading maintenance records: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // SUMMARY LABELS
        // =========================================

        public void displaylabel()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(*) AS TotalRequest,
                        SUM(CASE WHEN Status = 'Completed'
                            THEN 1 ELSE 0 END) AS Completed,
                        SUM(CASE WHEN Status = 'In Progress'
                            THEN 1 ELSE 0 END) AS InProgress,
                        SUM(CASE WHEN Status = 'Pending'
                            THEN 1 ELSE 0 END) AS Pending
                    FROM MaintenanceTbl";

                DataTable data = AppDB.Instance.TableData(query);

                if (data.Rows.Count > 0)
                {
                    totre_lbl.Text =
                        data.Rows[0]["TotalRequest"].ToString();

                    comlbl.Text =
                        data.Rows[0]["Completed"].ToString();

                    inproglbl.Text =
                        data.Rows[0]["InProgress"].ToString();

                    penlbl.Text =
                        data.Rows[0]["Pending"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying maintenance summary: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // SEARCH AND FILTER EVENTS
        // =========================================

        private void searchTC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }

        private void roomtypeSIC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }

        private void roomstatusSIC(object sender, EventArgs e)
        {
            ApplyMaintenanceFilters();
        }

        // =========================================
        // DGV ROW CLICK
        // =========================================

        private void MM_MaintenanceDGV_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                MM_MaintenanceDGV.Rows[e.RowIndex];

            MM_maintenanceIDtxt.Text =
                row.Cells["MaintenanceID"].Value?.ToString() ?? "";

            MM_roomIDtxt.Text =
                row.Cells["RoomID"].Value?.ToString() ?? "";

            MM_descriptiontxt.Text =
                row.Cells["Issue"].Value?.ToString() ?? "";

            string status =
                row.Cells["Status"].Value?.ToString() ?? "";

            bool isPending = status == "Pending";

            MM_acceptBtn.Enabled = isPending;
            MM_rejectBtn.Enabled = isPending;
        }

        // =========================================
        // CLEAR DETAILS
        // =========================================

        private void ClearDetails()
        {
            MM_maintenanceIDtxt.Clear();
            MM_roomIDtxt.Clear();
            MM_descriptiontxt.Clear();

            MM_acceptBtn.Enabled = false;
            MM_rejectBtn.Enabled = false;
        }

        // =========================================
        // ACCEPT REQUEST
        // Pending -> Approved
        // =========================================

        private void MM_acceptBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MM_maintenanceIDtxt.Text))
                return;

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to accept this maintenance request?",
                "Confirm Acceptance",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET Status = 'Approved'
                    WHERE MaintenanceID = @MaintenanceID
                    AND Status = 'Pending'";

                int affected = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        MaintenanceID = MM_maintenanceIDtxt.Text
                    });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Maintenance request successfully approved.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "This request is no longer pending.",
                        "Notice",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                displaylabel();
                ApplyMaintenanceFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error approving request: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // REJECT REQUEST
        // Pending -> Rejected
        // =========================================

        private void MM_rejectBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MM_maintenanceIDtxt.Text))
                return;

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to reject this maintenance request?",
                "Confirm Rejection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    UPDATE MaintenanceTbl
                    SET Status = 'Rejected'
                    WHERE MaintenanceID = @MaintenanceID
                    AND Status = 'Pending'";

                int affected = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        MaintenanceID = MM_maintenanceIDtxt.Text
                    });

                if (affected > 0)
                {
                    MessageBox.Show(
                        "Maintenance request successfully rejected.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "This request is no longer pending.",
                        "Notice",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                displaylabel();
                ApplyMaintenanceFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error rejecting request: " + ex.Message,
                    "Maintenance Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
