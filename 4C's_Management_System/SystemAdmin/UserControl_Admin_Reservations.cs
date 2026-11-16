
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Reservations : UserControl
    {
        public string branchId = AccessBranch_ByForm.BranchId;

        public UserControl_Admin_Reservations()
        {
            InitializeComponent();

            this.Load += UserControl_Admin_Reservations_Load;

            dgv_reservation_req.CellClick +=
                dgv_reservation_req_CellClick;
        }

        // =====================================================
        // LOAD USERCONTROL
        // =====================================================

        private void UserControl_Admin_Reservations_Load(
            object sender, EventArgs e)
        {
            branchId = AccessBranch_ByForm.BranchId;

            ClearReservationDetails();
            LoadReservationSummary();
            LoadReservations();
        }

        // =====================================================
        // LOAD RESERVATION SUMMARY
        // =====================================================

        public void LoadReservationSummary()
        {
            try
            {
                branchId = AccessBranch_ByForm.BranchId;

                const string query = @"
                    SELECT
                        COUNT(*) AS TotalReservations,

                        SUM(CASE
                            WHEN ReservationStatus = 'Confirmed'
                            THEN 1 ELSE 0
                        END) AS ConfirmedReservations,

                        SUM(CASE
                            WHEN ReservationStatus = 'Pending'
                            THEN 1 ELSE 0
                        END) AS PendingReservations,

                        SUM(CASE
                            WHEN ReservationStatus = 'Cancelled'
                            THEN 1 ELSE 0
                        END) AS CancelledReservations

                    FROM ReservationTbl
                    WHERE branchId = @BranchId;";

                DataTable dt = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    RM_totalResLBL.Text =
                        Convert.ToString(row["TotalReservations"]) ?? "0";

                    RR_checkedInLBL.Text =
                        Convert.ToString(row["ConfirmedReservations"]) ?? "0";

                    RR_checkedOutLBL.Text =
                        Convert.ToString(row["PendingReservations"]) ?? "0";

                    RR_CancelledLBL.Text =
                        Convert.ToString(row["CancelledReservations"]) ?? "0";
                }
                else
                {
                    RM_totalResLBL.Text = "0";
                    RR_checkedInLBL.Text = "0";
                    RR_checkedOutLBL.Text = "0";
                    RR_CancelledLBL.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading reservation summary:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // LOAD RESERVATION TABLE
        // =====================================================

        public void LoadReservations()
        {
            try
            {
                branchId = AccessBranch_ByForm.BranchId;

                const string query = @"
                    SELECT
                        ReservationId,
                        CustomerId,
                        TenantId,
                        RoomId,
                        AccomodationType,
                        ReservationDate,
                        ResStartDate,
                        ResEndDate,
                        ReservationStatus
                    FROM ReservationTbl
                    WHERE branchId = @BranchId
                    ORDER BY ReservationDate DESC;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId });

                dgv_reservation_req.DataSource = null;
                dgv_reservation_req.AutoGenerateColumns = true;
                dgv_reservation_req.DataSource = data;

                // COLUMN HEADERS

                if (dgv_reservation_req.Columns.Contains("ReservationId"))
                    dgv_reservation_req.Columns["ReservationId"].HeaderText =
                        "Reservation ID";

                if (dgv_reservation_req.Columns.Contains("CustomerId"))
                    dgv_reservation_req.Columns["CustomerId"].HeaderText =
                        "Customer ID";

                if (dgv_reservation_req.Columns.Contains("TenantId"))
                    dgv_reservation_req.Columns["TenantId"].HeaderText =
                        "Tenant ID";

                if (dgv_reservation_req.Columns.Contains("RoomId"))
                    dgv_reservation_req.Columns["RoomId"].HeaderText =
                        "Room ID";

                if (dgv_reservation_req.Columns.Contains("AccomodationType"))
                    dgv_reservation_req.Columns["AccomodationType"].HeaderText =
                        "Accommodation Type";

                if (dgv_reservation_req.Columns.Contains("ReservationDate"))
                    dgv_reservation_req.Columns["ReservationDate"].HeaderText =
                        "Reservation Date";

                if (dgv_reservation_req.Columns.Contains("ResStartDate"))
                    dgv_reservation_req.Columns["ResStartDate"].HeaderText =
                        "Check In";

                if (dgv_reservation_req.Columns.Contains("ResEndDate"))
                    dgv_reservation_req.Columns["ResEndDate"].HeaderText =
                        "Check Out";

                if (dgv_reservation_req.Columns.Contains("ReservationStatus"))
                    dgv_reservation_req.Columns["ReservationStatus"].HeaderText =
                        "Status";

                dgv_reservation_req.ClearSelection();

                if (dgv_reservation_req.Rows.Count > 0)
                {
                    dgv_reservation_req.Rows[0].Selected = true;

                    if (dgv_reservation_req.Columns.Contains("ReservationId"))
                    {
                        dgv_reservation_req.CurrentCell =
                            dgv_reservation_req.Rows[0].Cells["ReservationId"];
                    }

                    DisplayReservationDetails(
                        dgv_reservation_req.Rows[0]);
                }
                else
                {
                    ClearReservationDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading reservations:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DATAGRIDVIEW CELL CLICK
        // =====================================================

        private void dgv_reservation_req_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow selectedRow =
                dgv_reservation_req.Rows[e.RowIndex];

            if (selectedRow.IsNewRow)
                return;

            DisplayReservationDetails(selectedRow);
        }

        // =====================================================
        // DISPLAY RESERVATION DETAILS
        // =====================================================

        private void DisplayReservationDetails(DataGridViewRow row)
        {
            try
            {
                string customerId =
                    Convert.ToString(row.Cells["CustomerId"].Value) ?? "";

                string tenantId =
                    Convert.ToString(row.Cells["TenantId"].Value) ?? "";

                string roomId =
                    Convert.ToString(row.Cells["RoomId"].Value) ?? "";

                string checkIn =
                    Convert.ToString(row.Cells["ResStartDate"].Value) ?? "";

                string checkOut =
                    Convert.ToString(row.Cells["ResEndDate"].Value) ?? "";

                string status =
                    Convert.ToString(row.Cells["ReservationStatus"].Value) ?? "";

                // ROOM
                RR_RoomIDLBL.Text = roomId;

                // CHECK IN
                if (DateTime.TryParse(checkIn, out DateTime checkInDate))
                    RR_checkIndateLBL.Text =
                        checkInDate.ToString("MMM dd, yyyy");
                else
                    RR_checkIndateLBL.Text = checkIn;

                // CHECK OUT
                if (DateTime.TryParse(checkOut, out DateTime checkOutDate))
                    RR_checkOutLBL.Text =
                        checkOutDate.ToString("MMM dd, yyyy");
                else
                    RR_checkOutLBL.Text = checkOut;

                // STATUS
                RR_Panelstatus.Text = status;

                // GUEST AND CONTACT
                // Requires CustomerTbl / TenantTbl column structure.
                RR_nameLBL.Text =
                    !string.IsNullOrWhiteSpace(customerId)
                    ? "Customer ID: " + customerId
                    : "Tenant ID: " + tenantId;

                RR_ContactLBL.Text = "Not available yet";

                // CAPACITY AND PRICE
                RR_capacityLbl.Text = "Not available yet";
                RR_priceLBL.Text = "Not available yet";
                RR_totalpriceLBL.Text = "Not available yet";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying reservation details:\n" + ex.Message,
                    "Display Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CLEAR RESERVATION DETAILS
        // =====================================================

        private void ClearReservationDetails()
        {
            RR_nameLBL.Text = "";
            RR_ContactLBL.Text = "";
            RR_RoomIDLBL.Text = "";
            RR_checkIndateLBL.Text = "";
            RR_checkOutLBL.Text = "";
            RR_capacityLbl.Text = "";
            RR_priceLBL.Text = "";
            RR_totalpriceLBL.Text = "";
            RR_Panelstatus.Text = "";
        }

        // =====================================================
        // REFRESH RESERVATIONS
        // =====================================================

        public void RefreshReservations()
        {
            LoadReservationSummary();
            LoadReservations();
        }

        // =====================================================
        // OTHER EVENTS
        // =====================================================

        private void guna2PictureBox8_Click(object sender, EventArgs e)
        {

        }
    }
}
