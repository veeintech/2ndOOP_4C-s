using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace _4C_s_Management_System
{
    public partial class UserControlRoomReserv : UserControl
    {
        public UserControlRoomReserv()
        {
            InitializeComponent();
            RoomReserveDGV.CellClick += RoomReserveDGV_CellClick;
        }

        public string branchid = AccessBranch_ByForm.BranchId;
        private void UserControlRoomReserv_Load(object sender, EventArgs e)
        {
            RRes_AccommodationCB.SelectedIndex = 0;
            RR_RoomTypesCB.SelectedIndex = 0;
            LoadReservationData();

            RR_Timecheckin_TXT.Format = DateTimePickerFormat.Custom;
            RR_Timecheckin_TXT.CustomFormat = "hh:mm tt";
            RR_Timecheckin_TXT.ShowUpDown = true;

            RR_Timecheckout_TXT.Format = DateTimePickerFormat.Custom;
            RR_Timecheckout_TXT.CustomFormat = "hh:mm tt";
            RR_Timecheckout_TXT.ShowUpDown = true;
            loadDisplay_label();
        }

        public void LoadReservationData()
        {
            try
            {
                const string query =
                    "SELECT ReservationId, AccomodationType, ReservationDate, ResStartDate, ResEndDate " +
                    "FROM ReservationTbl " +
                    "WHERE branchId = @BranchId " +
                    "ORDER BY ReservationId ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchid },
                    RoomReserveDGV,
                    header: new[]
                    {
                    "Reservation ID",
                    "Accommodation Type",
                    "Reservation Date",
                    "Start Date",
                    "End Date",
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading reservation data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void SearchData()
        {
            string reservationId = RR_searchresID.Text.Trim();

            if (string.IsNullOrWhiteSpace(reservationId))
            {
                LoadReservationData();
                return;
            }

            try
            {
                const string query =
                    "SELECT ReservationId, AccomodationType, " +
                    "ReservationDate, ResStartDate, ResEndDate " +
                    "FROM ReservationTbl " +
                    "WHERE BranchId = @BranchId " +
                    "AND ReservationId LIKE @ReservationId OR " +
                    "AccomodationType LIKE @AccomodationType " +
                    "ORDER BY ReservationId ASC";

                AppDB.Instance.Table(
                    query,
                    new
                    {
                        BranchId = branchid,
                        ReservationId = $"%{reservationId}%",
                        AccomodationType = $"%{reservationId}%"
                    },
                    RoomReserveDGV,
                    header: new[]
                    {
                        "Reservation ID",
                        "Accommodation Type",
                        "Reservation Date",
                        "Start Date",
                        "End Date"
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading reservation data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RR_searchtxtx_textchanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }
        private void RRes_AccommodationCB_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void RR_RoomTypesCB_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void ApplyCustomerFilters()
        {
            try
            {
                var conditions = new List<string>
                 {
                     "BranchId = @BranchId"
                 };

                var parameters = new Dictionary<string, object>
                {
                    ["BranchId"] = branchid
                };

                if (RRes_AccommodationCB.SelectedIndex > 0 &&
                    RRes_AccommodationCB.SelectedItem is not null)
                {
                    conditions.Add("AccomodationType = @AccommodationType");
                    parameters["AccommodationType"] =
                        RRes_AccommodationCB.SelectedItem.ToString()!;
                }

                if (RR_RoomTypesCB.SelectedIndex > 0 &&
                    RR_RoomTypesCB.SelectedItem is not null)
                {
                    conditions.Add("AccomodationType = @RoomType");
                    parameters["RoomType"] =
                        RR_RoomTypesCB.SelectedItem.ToString()!;
                }

                string reservationId = RR_searchresID.Text.Trim();

                if (!string.IsNullOrWhiteSpace(reservationId))
                {
                    conditions.Add("ReservationId LIKE @ReservationId");
                    parameters["ReservationId"] = $"%{reservationId}%";
                }

                string query =
                    "SELECT ReservationId, AccomodationType, " +
                    "ReservationDate, ResStartDate, ResEndDate " +
                    "FROM ReservationTbl " +
                    $"WHERE {string.Join(" AND ", conditions)} " +
                    "ORDER BY ReservationId ASC";

                AppDB.Instance.Table(
                    query,
                    parameters,
                    RoomReserveDGV,
                    header: new[]
                    {
                        "Reservation ID",
                        "Accommodation",
                        "Date reserved",
                        "Start Date",
                        "End Date"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading reservation data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RR_AddCusBtn_Click(object sender, EventArgs e)
        {
            string reserId = RR_ReserIDTxt.Text.Trim();
            string customerid = RR_customertxt.Text.Trim();
            string tenantid = RR_tenantidTxt.Text.Trim();
            string roomid = RR_RoomIdTxt.Text.Trim();
            string accomodationtype = RR_resTypetxt.Text.Trim();
            string reservationdate = RR_ReserIDTxt.Text.Trim();

            DateTime startdate = RR_checkInDateTXT.Value.Date.Add(RR_Timecheckin_TXT.Value.TimeOfDay);

            DateTime enddate = RR_checkOutDateTxt.Value.Date.Add(RR_Timecheckout_TXT.Value.TimeOfDay);

            if (string.IsNullOrWhiteSpace(customerid) ||
                string.IsNullOrWhiteSpace(roomid) ||
                string.IsNullOrWhiteSpace(accomodationtype))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            if (startdate >= enddate)
            {
                MessageBox.Show(
                    "Start date must be earlier than end date.",
                    "Invalid Date Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (RR_resTypetxt.SelectedIndex != 2 && !string.IsNullOrWhiteSpace(tenantid))
            {
                MessageBox.Show(
                    "Tenant ID should only be provided for the selected accommodation type.",
                    "Invalid Tenant ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            const string customerQuery =
               "SELECT Status " +
               "FROM TBLRoom " +
               "WHERE RoomId = @roomid";

            string status = Convert.ToString(
                AppDB.Instance.Scalar(
                    customerQuery,
                    new { RoomId = roomid }));

            if (!string.Equals(status, "Available", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Room is not currently available. Cannot be reserved.",
                    "Invalid Operation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string roomType = "";

            if (!string.IsNullOrWhiteSpace(roomid))
            {
                const string roomQuery =
                    "SELECT RoomType " +
                    "FROM TBLRoom " +
                    "WHERE RoomId = @RoomId";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new { RoomId = roomid });

                if (roomData.Rows.Count > 0)
                {
                    roomType =
                        Convert.ToString(roomData.Rows[0]["RoomType"]) ?? "";
                }
            }
            if (roomType != accomodationtype)
            {
                MessageBox.Show(
                 $"The selected room is of type '{roomType}', but the accommodation type is '{accomodationtype}'.",
                 "Invalid RoomId",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(RR_ReserIDTxt.Text))
                {
                    string reservationId =
                        IDGenerator.GenerateNextID("RES", branchid);

                    RR_ReserIDTxt.Text = reservationId;

                    const string query =
                        "INSERT INTO ReservationTbl " +
                        "(ReservationId, BranchId, CustomerId, TenantId, RoomId, " +
                        "AccomodationType, ReservationDate, ResStartDate, ResEndDate) " +
                        "VALUES " +
                        "(@ReservationId, @BranchId, @CustomerId, @TenantId, @RoomId, " +
                        "@AccomodationType, @ReservationDate, @ResStartDate, @ResEndDate)";

                    int result = AppDB.Instance.CUD(
                        query,
                        new
                        {
                            ReservationId = reservationId,
                            BranchId = branchid,
                            CustomerId = customerid,
                            TenantId = tenantid,
                            RoomId = roomid,
                            AccomodationType = accomodationtype,
                            ReservationDate = DateTime.Today,
                            ResStartDate = startdate.Date,
                            ResEndDate = enddate.Date
                        });

                    if (result > 0)
                    {
                        MessageBox.Show(
                            "Reservation added successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    customerstatus("re");

                    const string roomquery =
                    "UPDATE TBLRoom SET " +
                    "Status = @Status " +
                    "WHERE RoomId = @RoomId";

                    int roomres = AppDB.Instance.CUD(roomquery, new { RoomId = roomid, Status = "Reserved" });

                    if (roomres > 0)
                    {
                        MessageBox.Show(
                            $"{roomid} is now Reserved.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    LoadReservationData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "Reservation cannot be added because it already exists.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearFields()
        {
            RR_customertxt.Clear();
            RR_RoomIdTxt.Clear();
            RR_tenantidTxt.Clear();
            RR_ReserIDTxt.Clear();
            RR_checkInDateTXT.Value = DateTime.Now;
            RR_checkOutDateTxt.Value = DateTime.Now;

            RR_Timecheckin_TXT.Value = DateTime.Now;
            RR_Timecheckout_TXT.Value = DateTime.Now;
        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AccomodationTXT_selectedIndexChange(object sender, EventArgs e)
        {
            if (RR_resTypetxt.SelectedIndex == 2)
            {
                RR_tenantidTxt.ReadOnly = false;
            }
            else
            {
                RR_tenantidTxt.ReadOnly = true;
                RR_tenantidTxt.Clear();
            }
        }

        private void RR_UpdateBtn_Click(object sender, EventArgs e)
        {
            string reservationId = RR_ReserIDTxt.Text.Trim();
            string customerId = RR_customertxt.Text.Trim();
            string tenantId = RR_tenantidTxt.Text.Trim();
            string roomId = RR_RoomIdTxt.Text.Trim();
            string accommodationType = RR_resTypetxt.Text.Trim();
            DateTime startDate =
                RR_checkInDateTXT.Value.Date.Add(
                    RR_Timecheckin_TXT.Value.TimeOfDay);

            DateTime endDate =
                RR_checkOutDateTxt.Value.Date.Add(
                    RR_Timecheckout_TXT.Value.TimeOfDay);

            if (string.IsNullOrWhiteSpace(reservationId) ||
                string.IsNullOrWhiteSpace(customerId) ||
                string.IsNullOrWhiteSpace(roomId) ||
                string.IsNullOrWhiteSpace(accommodationType))
            {
                MessageBox.Show(
                    "Please select a reservation and fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (startDate >= endDate)
            {
                MessageBox.Show(
                    "Start date must be earlier than end date.",
                    "Invalid Date Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (startDate < DateTime.Now)
            {
                MessageBox.Show(
                    "Start date cannot be in the past.",
                    "Invalid Start Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string roomType = "";

            if (!string.IsNullOrWhiteSpace(roomId))
            {
                const string roomQuery =
                    "SELECT RoomType " +
                    "FROM TBLRoom " +
                    "WHERE RoomId = @RoomId";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new { RoomId = roomId });

                if (roomData.Rows.Count > 0)
                {
                    roomType =
                        Convert.ToString(roomData.Rows[0]["RoomType"]) ?? "";
                }
            }
            if (roomType != accommodationType)
            {
                MessageBox.Show(
                 $"The selected room is of type '{roomType}', but the accommodation type is '{accommodationType}'.",
                 "Invalid RoomId",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning);
                return;
            }

            if (RR_resTypetxt.SelectedIndex != 2 && !string.IsNullOrWhiteSpace(tenantId))
            {
                MessageBox.Show(
                    "Tenant ID should only be provided for the selected accommodation type.",
                    "Invalid Tenant ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query =
                    "UPDATE ReservationTbl SET " +
                    "CustomerId = @CustomerId, " +
                    "TenantId = @TenantId, " +
                    "RoomId = @RoomId, " +
                    "AccomodationType = @AccomodationType, " +
                    "ReservationDate = @ReservationDate, " +
                    "ResStartDate = @ResStartDate, " +
                    "ResEndDate = @ResEndDate " +
                    "WHERE ReservationId = @ReservationId " +
                    "AND BranchId = @BranchId";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid,
                        CustomerId = customerId,
                        TenantId = tenantId,
                        RoomId = roomId,
                        AccomodationType = accommodationType,
                        ReservationDate = DateTime.Today,
                        ResStartDate = startDate.Date,
                        ResEndDate = endDate.Date
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Reservation updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadReservationData();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "No reservation was updated.",
                        "Not Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating reservation: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadReservationIntoFields(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RoomReserveDGV.Rows.Count)
                return;

            DataGridViewRow row = RoomReserveDGV.Rows[rowIndex];

            if (row.IsNewRow)
                return;

            // LoadReservationData and SearchData populate five columns:
            // ReservationId, AccommodationType, ReservationDate,
            // ResStartDate, and ResEndDate.
            if (row.Cells.Count < 5)
                return;

            RR_ReserIDTxt.Text =
                Convert.ToString(row.Cells[0].Value) ?? "";

            RR_resTypetxt.Text =
                Convert.ToString(row.Cells[1].Value) ?? "";

            string reservationId =
                Convert.ToString(row.Cells[0].Value) ?? "";

            const string query =
                "SELECT CustomerId, TenantId, RoomId " +
                "FROM ReservationTbl " +
                "WHERE ReservationId = @ReservationID";

            DataTable relatedData = AppDB.Instance.TableData(
                query,
                new { ReservationID = reservationId });

            if (relatedData.Rows.Count > 0)
            {
                DataRow relatedRow = relatedData.Rows[0];

                RR_customertxt.Text =
                    Convert.ToString(relatedRow["CustomerId"]) ?? "";

                RR_tenantidTxt.Text =
                    Convert.ToString(relatedRow["TenantId"]) ?? "";

                RR_RoomIdTxt.Text =
                    Convert.ToString(relatedRow["RoomId"]) ?? "";
            }
            else
            {
                RR_customertxt.Text = "";
                RR_tenantidTxt.Text = "";
                RR_RoomIdTxt.Text = "";
            }

            RR_checkInDateTXT.Value =
                Convert.ToDateTime(row.Cells[3].Value);

            RR_Timecheckin_TXT.Value =
                Convert.ToDateTime(row.Cells[3].Value);

            RR_checkOutDateTxt.Value =
                Convert.ToDateTime(row.Cells[4].Value);

            RR_Timecheckout_TXT.Value =
                Convert.ToDateTime(row.Cells[4].Value);


            checkstatus();
        }

        private void RoomReserveDGV_CellClick(
        object sender,
        DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= RoomReserveDGV.Rows.Count)
            {
                return;
            }

            LoadReservationIntoFields(e.RowIndex);
            LoadDisplay(e.RowIndex);
        }

        private void RR_deleteBtn_Click(object sender, EventArgs e)
        {
            string Reservationid = RR_ReserIDTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(Reservationid))
            {
                MessageBox.Show(
                    "Please select a reservation to delete.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            try
            {

                const string query =
                    "DELETE FROM ReservationTbl " +
                    "WHERE ReservationId = @ReservationId";

                var result = AppDB.Instance.CUD(query, new
                {
                    ReservationId = Reservationid
                });
                if (result > 0)
                {
                    MessageBox.Show(
                        "Reservation deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                LoadReservationData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting reservation: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        public void LoadDisplay(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RoomReserveDGV.Rows.Count)
                return;

            DataGridViewRow row = RoomReserveDGV.Rows[rowIndex];

            if (row.IsNewRow)
                return;

            if (row.IsNewRow || row.Cells.Count == 0)
                return;

            string reservationId =
                Convert.ToString(row.Cells[0].Value) ?? "";

            if (string.IsNullOrWhiteSpace(reservationId))
                return;

            // GET RESERVATION INFORMATION
            const string reservationQuery =
                "SELECT CustomerId, TenantId, RoomId , ReservationDate, ResStartDate, ResEndDate, AccomodationType " +
                "FROM ReservationTbl " +
                "WHERE ReservationId = @ReservationId";

            DataTable reservationData = AppDB.Instance.TableData(
                reservationQuery,
                new { ReservationId = reservationId });

            if (reservationData.Rows.Count == 0)
                return;

            DataRow reservationRow = reservationData.Rows[0];

            string customerId = Convert.ToString(reservationRow["CustomerId"]) ?? "";
            string tenantId = Convert.ToString(reservationRow["TenantId"]) ?? "";
            string roomId = Convert.ToString(reservationRow["RoomId"]) ?? "";
            DateTime starting = Convert.ToDateTime(reservationRow["ReservationDate"]);
            DateTime startDate = Convert.ToDateTime(reservationRow["ResStartDate"]);
            DateTime endDate = Convert.ToDateTime(reservationRow["ResEndDate"]);
            string accType = Convert.ToString(reservationRow["AccomodationType"]) ?? "";


            // GET ROOM INFORMATION
            string capacity = "";
            double rate = 0;

            if (!string.IsNullOrWhiteSpace(roomId))
            {
                const string roomQuery =
                    "SELECT Capacity, Rate " +
                    "FROM TBLRoom " +
                    "WHERE RoomId = @RoomId";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new { RoomId = roomId });

                if (roomData.Rows.Count > 0)
                {
                    capacity =
                        Convert.ToString(roomData.Rows[0]["Capacity"]) ?? "";

                    rate =
                        Convert.ToDouble(roomData.Rows[0]["Rate"]);
                }
            }

            // GET CUSTOMER INFORMATION
            string customerName = "";
            string cus_contact = "";
            string Customerstatus = "";

            if (!string.IsNullOrWhiteSpace(customerId))
            {
                const string customerQuery =
                    "SELECT Fullname, ContactNumber, CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE CustomerId = @CustomerId";

                DataTable customerData = AppDB.Instance.TableData(
                    customerQuery,
                    new { CustomerId = customerId });

                if (customerData.Rows.Count > 0)
                {
                    customerName =
                        Convert.ToString(customerData.Rows[0]["Fullname"]) ?? "";

                    cus_contact =
                        Convert.ToString(customerData.Rows[0]["ContactNumber"]) ?? "";

                    Customerstatus =
                        Convert.ToString(customerData.Rows[0]["CustomerStatus"]) ?? "";
                }
            }

            // DISPLAY CUSTOMER AND ROOM INFORMATION
            RR_ResIDLBL.Text = reservationId;

            RR_nameLBL.Text = customerName;
            RR_ContactLBL.Text = cus_contact;
            RR_Resstatuslbl.Text = Customerstatus;
            RR_RoomIDLBL.Text = roomId;

            RR_checkIndateLBL.Text =
                startDate.ToString("MM/dd/yyyy hh:mm tt");

            RR_checkOutLBL.Text =
                endDate.ToString("MM/dd/yyyy hh:mm tt");

            if (capacity == "Single Room")
            {
                RR_capacityLbl.Text = "1-2 Persons";
            }
            else if (capacity == "Double Room")
            {
                RR_capacityLbl.Text = "2-4 Persons";
            }
            else
            {
                RR_capacityLbl.Text = capacity;
            }

            RR_priceLBL.Text = rate.ToString("C2");

            // COMPUTE TOTAL PRICE
            double dailyRate = rate;
            double hourlyRate = dailyRate / 24;

            // Correct order: checkout minus check-in
            TimeSpan duration = endDate - startDate;

            double totalHours = duration.TotalHours;

            double totalPrice = totalHours * hourlyRate;

            // APPLY ROOM MULTIPLIER
            if (RR_RoomTypesCB.SelectedIndex != 2)
            {
                if (capacity == "Double Room")
                {
                    totalPrice *= 2;
                }
            }

            totalPrice = Math.Round(totalPrice, 2);

            RR_totalpriceLBL.Text = totalPrice.ToString("C2");
        }



        private void reservation_cellContent_Click(object sender, DataGridViewCellEventArgs e)
        {
            LoadDisplay(e.RowIndex);
        }

        private void RR_checkinBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string customerId = RR_customertxt.Text.Trim();
                string roomid = RR_RoomIdTxt.Text.Trim();

                const string customerQuery =
                   "SELECT CustomerStatus " +
                   "FROM CustomerTbl " +
                   "WHERE CustomerId = @CustomerId";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new { CustomerId = customerId }));

                if (!string.Equals(status, "Reserved", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently reserved. Cannot perform cancellation.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                customerstatus("ci");

                const string query =
                    "UPDATE TBLRoom SET " +
                    "Status = @Status " +
                    "WHERE RoomId = @RoomId";

                int result = AppDB.Instance.CUD(query, new { RoomId = roomid, Status = "Occupied" });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomid} is now Occupied.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during check-in: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void customerstatus(string s)
        {
            try
            {
                string Cusid = RR_customertxt.Text.Trim();
                string status = "";

                if (s == "ci")
                {
                    status = "Checked In";
                }
                else if (s == "co")
                {
                    status = "Checked Out";
                }
                else if (s == "cn")
                {
                    status = "Cancelled";
                }
                else if (s == "re")
                {
                    status = "Reserved";
                }
                else if (s == "ns")
                {
                    status = "No Show";
                }
                else
                {
                    MessageBox.Show(
                        "Invalid status provided.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                const string query =
                    "UPDATE CustomerTbl SET " +
                    $"CustomerStatus = @Status " +
                    "WHERE CustomerId = @CustomerId";

                int result = AppDB.Instance.CUD(query, new { CustomerId = Cusid, Status = status });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"Customer status updated to '{status}'.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "No customer status was updated.",
                        "Not Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error updating customer status: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }


        }
        public void checkstatus()
        {
            try
            {
                string customerId = RR_customertxt.Text.Trim();
                string reservationId = RR_ReserIDTxt.Text.Trim();

                const string customerQuery =
                    "SELECT CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE CustomerId = @CustomerId";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new { CustomerId = customerId }));

                const string reservationQuery =
                    "SELECT ResStartDate, ResEndDate " +
                    "FROM ReservationTbl " +
                    "WHERE ReservationId = @ReservationId";

                DataTable reservationData = AppDB.Instance.TableData(
                    reservationQuery,
                    new { ReservationId = reservationId });

                if (reservationData.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Reservation data not found.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                DataRow row = reservationData.Rows[0];

                if (row.IsNull("ResStartDate") || row.IsNull("ResEndDate"))
                {
                    MessageBox.Show(
                        "Reservation dates are incomplete.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                DateTime startDate = Convert.ToDateTime(row["ResStartDate"]);
                DateTime endDate = Convert.ToDateTime(row["ResEndDate"]);

                if (DateTime.Now >= endDate &&
                    string.Equals(status, "Reserved", StringComparison.OrdinalIgnoreCase))
                {
                    customerstatus("ns");

                    MessageBox.Show(
                        "Customer status updated to 'No Show'.",
                        "Status Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error checking customer status: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void RR_checkoutBTN_Click(object sender, EventArgs e)
        {
            try
            {
                string customerId = RR_customertxt.Text.Trim();
                string roomid = RR_RoomIdTxt.Text.Trim();

                const string customerQuery =
                   "SELECT CustomerStatus " +
                   "FROM CustomerTbl " +
                   "WHERE CustomerId = @CustomerId";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new { CustomerId = customerId }));

                if (!string.Equals(status, "Checked In", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently checked in. Cannot perform check-out.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                customerstatus("co");

                const string query =
                    "UPDATE TBLRoom SET " +
                    "Status = @Status " +
                    "WHERE RoomId = @RoomId";

                int result = AppDB.Instance.CUD(query, new { RoomId = roomid, Status = "Available" });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomid} is now Available.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during check-out: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RR_cnacelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;
                DateTime startDate = RR_checkInDateTXT.Value.Date.Add(RR_Timecheckin_TXT.Value.TimeOfDay);

                string customerId = RR_customertxt.Text.Trim();
                string roomid = RR_RoomIdTxt.Text.Trim();

                const string customerQuery =
                   "SELECT CustomerStatus " +
                   "FROM CustomerTbl " +
                   "WHERE CustomerId = @CustomerId";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new { CustomerId = customerId }));

                if (now >= startDate)
                {
                    MessageBox.Show(
                        "Cannot cancel the reservation as the check-in date has already passed.",
                        "Cancellation Not Allowed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!string.Equals(status, "Reserved", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently reserved. Cannot perform cancellation.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                customerstatus("cn");

                MessageBox.Show(
                "Reservation cancelled successfully. Balance will be refunded.",
                "Cancellation Successful",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                const string query =
                "UPDATE TBLRoom SET " +
                "Status = @Status " +
                "WHERE RoomId = @RoomId";

                int result = AppDB.Instance.CUD(query, new { RoomId = roomid, Status = "Available" });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomid} is now Available.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during cancellation: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void accomodationtype_selectedIndexChange(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        public void loadDisplay_label()
        {
            const string labelDisplayQuery =
                "SELECT COUNT(CASE WHEN CustomerStatus = 'Reserved' THEN 1 END) AS TotalReserved, " +
                "COUNT(CASE WHEN CustomerStatus = 'Checked In' THEN 1 END) AS TotalCheckIn, " +
                "COUNT(CASE WHEN CustomerStatus = 'Checked Out' THEN 1 END) AS TotalCheckOut, " +
                "COUNT(CASE WHEN CustomerStatus = 'Cancelled' THEN 1 END) AS TotalCancelled FROM CustomerTbl;";

            DataTable reservationData = AppDB.Instance.TableData(
                labelDisplayQuery,
                new { });


            if (reservationData.Rows.Count == 0)
                return;

            DataRow reservationRow = reservationData.Rows[0];

            string reserved = Convert.ToString(reservationRow["TotalReserved"]) ?? "";
            string ci = Convert.ToString(reservationRow["TotalCheckIn"]) ?? "";
            string co = Convert.ToString(reservationRow["TotalCheckOut"]) ?? "";
            string cl = Convert.ToString(reservationRow["TotalCancelled"]) ?? "";

            RM_totalResLBL.Text = reserved;
            RR_checkedInLBL.Text = ci;
            RR_checkedOutLBL.Text = co;
            RR_CancelledLBL.Text = cl;

        }
    }
}
