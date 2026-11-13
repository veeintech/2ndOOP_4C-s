using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlRoomReserv : UserControl
    {
        public string branchid = AccessBranch_ByForm.BranchId;

        public UserControlRoomReserv()
        {
            InitializeComponent();

            RoomReserveDGV.CellClick += RoomReserveDGV_CellClick;

            // Update Customer ID and Tenant ID whenever accommodation changes.
            RR_resTypetxt.SelectedIndexChanged -= AccomodationTXT_selectedIndexChange;
            RR_resTypetxt.SelectedIndexChanged += AccomodationTXT_selectedIndexChange;
        }

        private bool IsApartment()
        {
            return string.Equals(
                RR_resTypetxt.Text.Trim(),
                "Apartment",
                StringComparison.OrdinalIgnoreCase);
        }

        private void ToggleCustomerTenantInputs()
        {
            bool apartment = IsApartment();
            bool hasAccommodation = !string.IsNullOrWhiteSpace(RR_resTypetxt.Text);

            if (!hasAccommodation)
            {
                RR_customertxt.Enabled = false;
                RR_tenantidTxt.Enabled = false;

                RR_customertxt.Clear();
                RR_tenantidTxt.Clear();
                return;
            }

            if (apartment)
            {
                RR_customertxt.Enabled = false;
                RR_customertxt.Clear();

                RR_tenantidTxt.Enabled = true;
            }
            else
            {
                RR_customertxt.Enabled = true;

                RR_tenantidTxt.Enabled = false;
                RR_tenantidTxt.Clear();
            }
        }

        private void UserControlRoomReserv_Load(object sender, EventArgs e)
        {
            RRes_AccommodationCB.SelectedIndex = 0;
            RR_RoomTypesCB.SelectedIndex = 0;

            RR_Timecheckin_TXT.Format = DateTimePickerFormat.Custom;
            RR_Timecheckin_TXT.CustomFormat = "hh:mm tt";
            RR_Timecheckin_TXT.ShowUpDown = true;

            RR_Timecheckout_TXT.Format = DateTimePickerFormat.Custom;
            RR_Timecheckout_TXT.CustomFormat = "hh:mm tt";
            RR_Timecheckout_TXT.ShowUpDown = true;

            ToggleCustomerTenantInputs();

            LoadReservationData();
            loadDisplay_label();
        }

        public void LoadReservationData()
        {
            try
            {
                const string query =
                    "SELECT ReservationId, AccomodationType, ReservationDate, ResStartDate, ResEndDate " +
                    "FROM ReservationTbl " +
                    "WHERE BranchId = @BranchId " +
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

        public void SearchData()
        {
            ApplyCustomerFilters();
        }

        private void RR_searchtxtx_textchanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void RRes_AccommodationCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void RR_RoomTypesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void ApplyCustomerFilters()
        {
            try
            {
                var conditions = new List<string>
                {
                    "R.BranchId = @BranchId"
                };

                var parameters = new Dictionary<string, object>
                {
                    ["BranchId"] = branchid
                };

                if (RRes_AccommodationCB.SelectedIndex > 0 &&
                    RRes_AccommodationCB.SelectedItem != null)
                {
                    conditions.Add("R.AccomodationType = @AccommodationType");

                    parameters["AccommodationType"] =
                        RRes_AccommodationCB.SelectedItem.ToString()!;
                }

                // RoomType belongs to TBLRoom, not ReservationTbl.
                if (RR_RoomTypesCB.SelectedIndex > 0 &&
                    RR_RoomTypesCB.SelectedItem != null)
                {
                    conditions.Add(
                        "R.RoomId IN (SELECT RoomId FROM TBLRoom WHERE RoomType = @RoomType AND BranchId = @BranchId)");

                    parameters["RoomType"] =
                        RR_RoomTypesCB.SelectedItem.ToString()!;
                }

                string reservationId = RR_searchresID.Text.Trim();

                if (!string.IsNullOrWhiteSpace(reservationId))
                {
                    conditions.Add("R.ReservationId LIKE @ReservationId");
                    parameters["ReservationId"] = $"%{reservationId}%";
                }

                const string queryStart =
                    "SELECT R.ReservationId, R.AccomodationType, " +
                    "R.ReservationDate, R.ResStartDate, R.ResEndDate " +
                    "FROM ReservationTbl R WHERE ";

                string query =
                    queryStart +
                    string.Join(" AND ", conditions) +
                    " ORDER BY R.ReservationId ASC";

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

        private void AccomodationTXT_selectedIndexChange(object sender, EventArgs e)
        {
            ToggleCustomerTenantInputs();
        }

        private void accomodationtype_selectedIndexChange(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void RR_AddCusBtn_Click(object sender, EventArgs e)
        {
            string customerId = RR_customertxt.Text.Trim();
            string tenantId = RR_tenantidTxt.Text.Trim();
            string roomId = RR_RoomIdTxt.Text.Trim();
            string accommodationType = RR_resTypetxt.Text.Trim();

            bool apartment = IsApartment();

            // Only the correct ID is used for the selected accommodation.
            if (apartment)
                customerId = "";
            else
                tenantId = "";

            DateTime startDate =
                RR_checkInDateTXT.Value.Date.Add(
                    RR_Timecheckin_TXT.Value.TimeOfDay);

            DateTime endDate =
                RR_checkOutDateTxt.Value.Date.Add(
                    RR_Timecheckout_TXT.Value.TimeOfDay);

            if (string.IsNullOrWhiteSpace(roomId) ||
                string.IsNullOrWhiteSpace(accommodationType))
            {
                MessageBox.Show(
                    "Please fill in all required fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (apartment && string.IsNullOrWhiteSpace(tenantId))
            {
                MessageBox.Show(
                    "Please enter a Tenant ID for an Apartment reservation.",
                    "Missing Tenant ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!apartment && string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show(
                    "Please enter a Customer ID for this reservation.",
                    "Missing Customer ID",
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

            if (!string.IsNullOrWhiteSpace(RR_ReserIDTxt.Text))
            {
                MessageBox.Show(
                    "Reservation cannot be added because a reservation is already selected.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check the room and retrieve its rate.
                const string roomQuery = @"
                    SELECT RoomType, Status, Rate
                    FROM TBLRoom
                    WHERE BranchId = @BranchId
                      AND RoomId = @RoomId;";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new
                    {
                        BranchId = branchid,
                        RoomId = roomId
                    });

                if (roomData.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "The selected room was not found in this branch.",
                        "Invalid Room",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DataRow room = roomData.Rows[0];

                string roomStatus =
                    Convert.ToString(room["Status"]) ?? "";

                if (string.Equals(
                    roomStatus,
                    "Maintenance",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "The room is under maintenance and cannot be reserved.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Check for overlapping reservations.
                const string reservationOverlapQuery = @"
                    SELECT COUNT(*)
                    FROM ReservationTbl
                    WHERE BranchId = @BranchId
                      AND RoomId = @RoomId
                      AND ResStartDate < @RequestedEnd
                      AND ResEndDate > @RequestedStart;";

                int activeReservations = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        reservationOverlapQuery,
                        new
                        {
                            BranchId = branchid,
                            RoomId = roomId,
                            RequestedStart = startDate,
                            RequestedEnd = endDate
                        }));

                if (activeReservations > 0)
                {
                    MessageBox.Show(
                        "The room is already reserved during the selected date and time.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Generate reservation ID.
                string reservationId =
                    IDGenerator.GenerateNextID("RES", branchid);

                // Save only one ID, depending on accommodation type.
                const string insertReservationQuery = @"
                    INSERT INTO ReservationTbl
                    (
                        ReservationId,
                        BranchId,
                        CustomerId,
                        TenantId,
                        RoomId,
                        AccomodationType,
                        ReservationDate,
                        ResStartDate,
                        ResEndDate
                    )
                    VALUES
                    (
                        @ReservationId,
                        @BranchId,
                        @CustomerId,
                        @TenantId,
                        @RoomId,
                        @AccomodationType,
                        @ReservationDate,
                        @ResStartDate,
                        @ResEndDate
                    );";

                int reservationResult = AppDB.Instance.CUD(
                    insertReservationQuery,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid,
                        CustomerId = apartment ? null : customerId,
                        TenantId = apartment ? tenantId : null,
                        RoomId = roomId,
                        AccomodationType = accommodationType,
                        ReservationDate = DateTime.Today,
                        ResStartDate = startDate,
                        ResEndDate = endDate
                    });

                if (reservationResult <= 0)
                {
                    MessageBox.Show(
                        "The reservation could not be saved.",
                        "Reservation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                RR_ReserIDTxt.Text = reservationId;

                if (!apartment)
                {
                    customerstatus("re");
                }

                // Update room status.
                const string updateRoomQuery = @"
                    UPDATE TBLRoom
                    SET Status = @Status
                    WHERE BranchId = @BranchId
                      AND RoomId = @RoomId;";

                AppDB.Instance.CUD(
                    updateRoomQuery,
                    new
                    {
                        BranchId = branchid,
                        RoomId = roomId,
                        Status = "Reserved"
                    });

                // Create the payment record.
                if (!apartment)
                {
                    double roomRate = Convert.ToDouble(room["Rate"]);

                    decimal amountPaid = Convert.ToDecimal(
                        (endDate - startDate).TotalDays * roomRate);

                    const string paymentQuery = @"
                        INSERT INTO PaymentTbl
                        (
                            PaymentID,
                            BranchID,
                            ReservationID,
                            CustomerID,
                            AmountPaid,
                            PaymentStatus
                        )
                        VALUES
                        (
                            @PaymentID,
                            @BranchID,
                            @ReservationID,
                            @CustomerID,
                            @AmountPaid,
                            @PaymentStatus
                        );";

                    AppDB.Instance.CUD(
                        paymentQuery,
                        new
                        {
                            PaymentID = IDGenerator.GenerateNextID("PAY", branchid),
                            BranchID = branchid,
                            ReservationID = reservationId,
                            CustomerID = customerId,
                            AmountPaid = amountPaid,
                            PaymentStatus = "Pending"
                        });
                }
                else
                {
                    decimal amountPaid = 0;

                    decimal.TryParse(
                        RR_totalpriceLBL.Text
                            .Replace("₱", "")
                            .Replace(",", "")
                            .Trim(),
                        out amountPaid);

                    const string paymentQuery = @"
                        INSERT INTO PaymentTbl
                        (
                            PaymentID,
                            BranchID,
                            TenantID,
                            AmountPaid,
                            PaymentStatus
                        )
                        VALUES
                        (
                            @PaymentID,
                            @BranchID,
                            @TenantID,
                            @AmountPaid,
                            @PaymentStatus
                        );";

                    AppDB.Instance.CUD(
                        paymentQuery,
                        new
                        {
                            PaymentID = IDGenerator.GenerateNextID("PAY", branchid),
                            BranchID = branchid,
                            TenantID = tenantId,
                            AmountPaid = amountPaid,
                            PaymentStatus = "Pending"
                        });
                }

                MessageBox.Show(
                    "Reservation added successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadReservationData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

            // Keep the selected accommodation type, but update the controls.
            ToggleCustomerTenantInputs();
        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e)
        {
        }

        private void RR_UpdateBtn_Click(object sender, EventArgs e)
        {
            string reservationId = RR_ReserIDTxt.Text.Trim();
            string customerId = RR_customertxt.Text.Trim();
            string tenantId = RR_tenantidTxt.Text.Trim();
            string roomId = RR_RoomIdTxt.Text.Trim();
            string accommodationType = RR_resTypetxt.Text.Trim();

            bool apartment = IsApartment();

            if (apartment)
                customerId = "";
            else
                tenantId = "";

            DateTime startDate =
                RR_checkInDateTXT.Value.Date.Add(
                    RR_Timecheckin_TXT.Value.TimeOfDay);

            DateTime endDate =
                RR_checkOutDateTxt.Value.Date.Add(
                    RR_Timecheckout_TXT.Value.TimeOfDay);

            if (string.IsNullOrWhiteSpace(reservationId) ||
                string.IsNullOrWhiteSpace(roomId) ||
                string.IsNullOrWhiteSpace(accommodationType))
            {
                MessageBox.Show(
                    "Please select a reservation and fill in all required fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (apartment && string.IsNullOrWhiteSpace(tenantId))
            {
                MessageBox.Show(
                    "Please enter a Tenant ID for an Apartment reservation.",
                    "Missing Tenant ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!apartment && string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show(
                    "Please enter a Customer ID for this reservation.",
                    "Missing Customer ID",
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

            try
            {
                // Check that the room exists in the current branch.
                const string roomQuery = @"
                    SELECT RoomId
                    FROM TBLRoom
                    WHERE BranchId = @BranchId
                      AND RoomId = @RoomId;";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new
                    {
                        BranchId = branchid,
                        RoomId = roomId
                    });

                if (roomData.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "The selected room was not found in this branch.",
                        "Invalid Room",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Update reservation.
                const string query = @"
                    UPDATE ReservationTbl
                    SET
                        CustomerId = @CustomerId,
                        TenantId = @TenantId,
                        RoomId = @RoomId,
                        AccomodationType = @AccomodationType,
                        ReservationDate = @ReservationDate,
                        ResStartDate = @ResStartDate,
                        ResEndDate = @ResEndDate
                    WHERE ReservationId = @ReservationId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid,
                        CustomerId = apartment ? null : customerId,
                        TenantId = apartment ? tenantId : null,
                        RoomId = roomId,
                        AccomodationType = accommodationType,
                        ReservationDate = DateTime.Today,
                        ResStartDate = startDate,
                        ResEndDate = endDate
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

            if (row.IsNewRow || row.Cells.Count < 5)
                return;

            RR_ReserIDTxt.Text =
                Convert.ToString(row.Cells[0].Value) ?? "";

            RR_resTypetxt.Text =
                Convert.ToString(row.Cells[1].Value) ?? "";

            // Update the enabled/disabled state after loading the type.
            ToggleCustomerTenantInputs();

            string reservationId =
                Convert.ToString(row.Cells[0].Value) ?? "";

            const string query = @"
                SELECT CustomerId, TenantId, RoomId
                FROM ReservationTbl
                WHERE ReservationId = @ReservationID
                  AND BranchId = @BranchId;";

            DataTable relatedData = AppDB.Instance.TableData(
                query,
                new
                {
                    ReservationID = reservationId,
                    BranchId = branchid
                });

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
                RR_customertxt.Clear();
                RR_tenantidTxt.Clear();
                RR_RoomIdTxt.Clear();
            }

            // Reapply the control state after loading the IDs.
            ToggleCustomerTenantInputs();

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
            string reservationId = RR_ReserIDTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(reservationId))
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
                const string query = @"
                    DELETE FROM ReservationTbl
                    WHERE ReservationId = @ReservationId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid
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
                    MessageBoxIcon.Error);
            }
        }

        public void LoadDisplay(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RoomReserveDGV.Rows.Count)
                return;

            DataGridViewRow row = RoomReserveDGV.Rows[rowIndex];

            if (row.IsNewRow || row.Cells.Count == 0)
                return;

            string reservationId =
                Convert.ToString(row.Cells[0].Value) ?? "";

            if (string.IsNullOrWhiteSpace(reservationId))
                return;

            const string reservationQuery = @"
                SELECT
                    CustomerId,
                    TenantId,
                    RoomId,
                    ReservationDate,
                    ResStartDate,
                    ResEndDate,
                    AccomodationType
                FROM ReservationTbl
                WHERE ReservationId = @ReservationId
                  AND BranchId = @BranchId;";

            DataTable reservationData = AppDB.Instance.TableData(
                reservationQuery,
                new
                {
                    ReservationId = reservationId,
                    BranchId = branchid
                });

            if (reservationData.Rows.Count == 0)
                return;

            DataRow reservationRow = reservationData.Rows[0];

            string customerId =
                Convert.ToString(reservationRow["CustomerId"]) ?? "";

            string tenantId =
                Convert.ToString(reservationRow["TenantId"]) ?? "";

            string roomId =
                Convert.ToString(reservationRow["RoomId"]) ?? "";

            DateTime startDate =
                Convert.ToDateTime(reservationRow["ResStartDate"]);

            DateTime endDate =
                Convert.ToDateTime(reservationRow["ResEndDate"]);

            string accommodationType =
                Convert.ToString(reservationRow["AccomodationType"]) ?? "";

            string capacity = "";
            double rate = 0;

            // Get room information.
            if (!string.IsNullOrWhiteSpace(roomId))
            {
                const string roomQuery = @"
                    SELECT Capacity, Rate
                    FROM TBLRoom
                    WHERE BranchId = @BranchId
                      AND RoomId = @RoomId;";

                DataTable roomData = AppDB.Instance.TableData(
                    roomQuery,
                    new
                    {
                        BranchId = branchid,
                        RoomId = roomId
                    });

                if (roomData.Rows.Count > 0)
                {
                    capacity =
                        Convert.ToString(roomData.Rows[0]["Capacity"]) ?? "";

                    rate =
                        Convert.ToDouble(roomData.Rows[0]["Rate"]);
                }
            }

            string customerName = "";
            string contactNumber = "";
            string displayStatus = "";

            // Get customer information for Hotel and Transient.
            if (!string.IsNullOrWhiteSpace(customerId))
            {
                const string customerQuery = @"
                    SELECT Fullname, ContactNumber, CustomerStatus
                    FROM CustomerTbl
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                DataTable customerData = AppDB.Instance.TableData(
                    customerQuery,
                    new
                    {
                        CustomerId = customerId,
                        BranchId = branchid
                    });

                if (customerData.Rows.Count > 0)
                {
                    customerName =
                        Convert.ToString(customerData.Rows[0]["Fullname"]) ?? "";

                    contactNumber =
                        Convert.ToString(customerData.Rows[0]["ContactNumber"]) ?? "";

                    displayStatus =
                        Convert.ToString(customerData.Rows[0]["CustomerStatus"]) ?? "";
                }
            }

            // Get tenant information for Apartment.
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                const string tenantQuery = @"
                    SELECT FullName, ContactNumber, Status
                    FROM TenantTbl
                    WHERE TenantId = @TenantId
                      AND BranchId = @BranchId;";

                DataTable tenantData = AppDB.Instance.TableData(
                    tenantQuery,
                    new
                    {
                        TenantId = tenantId,
                        BranchId = branchid
                    });

                if (tenantData.Rows.Count > 0)
                {
                    customerName =
                        Convert.ToString(tenantData.Rows[0]["FullName"]) ?? "";

                    contactNumber =
                        Convert.ToString(tenantData.Rows[0]["ContactNumber"]) ?? "";

                    displayStatus =
                        Convert.ToString(tenantData.Rows[0]["Status"]) ?? "";
                }
            }

            RR_ResIDLBL.Text = reservationId;
            RR_nameLBL.Text = customerName;
            RR_ContactLBL.Text = contactNumber;
            RR_Resstatuslbl.Text = displayStatus;
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

            // Calculate the total price.
            double hourlyRate = rate / 24;
            TimeSpan duration = endDate - startDate;

            double totalPrice =
                Math.Round(duration.TotalHours * hourlyRate, 2);

            RR_totalpriceLBL.Text = totalPrice.ToString("C2");
        }

        private void reservation_cellContent_Click(
            object sender,
            DataGridViewCellEventArgs e)
        {
            LoadDisplay(e.RowIndex);
        }

        private void RR_checkinBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsApartment())
                {
                    MessageBox.Show(
                        "Check-in using Customer ID is not available for Apartment reservations.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string customerId = RR_customertxt.Text.Trim();
                string roomId = RR_RoomIdTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(customerId) ||
                    string.IsNullOrWhiteSpace(roomId))
                {
                    MessageBox.Show(
                        "Please select a customer reservation first.",
                        "Missing Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                const string customerQuery = @"
                    SELECT CustomerStatus
                    FROM CustomerTbl
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new
                        {
                            CustomerId = customerId,
                            BranchId = branchid
                        }));

                if (!string.Equals(
                    status,
                    "Reserved",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently reserved. Cannot perform check-in.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                customerstatus("ci");

                const string query = @"
                    UPDATE TBLRoom
                    SET Status = @Status
                    WHERE RoomId = @RoomId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        RoomId = roomId,
                        BranchId = branchid,
                        Status = "Occupied"
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomId} is now Occupied.",
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
                string customerId = RR_customertxt.Text.Trim();
                string status = "";

                if (string.IsNullOrWhiteSpace(customerId))
                    return;

                if (s == "ci")
                    status = "Checked In";
                else if (s == "co")
                    status = "Checked Out";
                else if (s == "cn")
                    status = "Cancelled";
                else if (s == "re")
                    status = "Reserved";
                else if (s == "ns")
                    status = "No Show";
                else
                {
                    MessageBox.Show(
                        "Invalid status provided.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                const string query = @"
                    UPDATE CustomerTbl
                    SET CustomerStatus = @Status
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerId = customerId,
                        BranchId = branchid,
                        Status = status
                    });

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
                // Apartment reservations do not use CustomerTbl status.
                if (IsApartment())
                    return;

                string customerId = RR_customertxt.Text.Trim();
                string reservationId = RR_ReserIDTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(customerId) ||
                    string.IsNullOrWhiteSpace(reservationId))
                    return;

                const string customerQuery = @"
                    SELECT CustomerStatus
                    FROM CustomerTbl
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new
                        {
                            CustomerId = customerId,
                            BranchId = branchid
                        }));

                const string reservationQuery = @"
                    SELECT ResStartDate, ResEndDate
                    FROM ReservationTbl
                    WHERE ReservationId = @ReservationId
                      AND BranchId = @BranchId;";

                DataTable reservationData = AppDB.Instance.TableData(
                    reservationQuery,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid
                    });

                if (reservationData.Rows.Count == 0)
                    return;

                DataRow row = reservationData.Rows[0];

                if (row.IsNull("ResStartDate") ||
                    row.IsNull("ResEndDate"))
                    return;

                DateTime endDate =
                    Convert.ToDateTime(row["ResEndDate"]);

                if (DateTime.Now >= endDate &&
                    string.Equals(
                        status,
                        "Reserved",
                        StringComparison.OrdinalIgnoreCase))
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
                if (IsApartment())
                {
                    MessageBox.Show(
                        "Check-out using Customer ID is not available for Apartment reservations.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string customerId = RR_customertxt.Text.Trim();
                string roomId = RR_RoomIdTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(customerId) ||
                    string.IsNullOrWhiteSpace(roomId))
                    return;

                const string customerQuery = @"
                    SELECT CustomerStatus
                    FROM CustomerTbl
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new
                        {
                            CustomerId = customerId,
                            BranchId = branchid
                        }));

                if (!string.Equals(
                    status,
                    "Checked In",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently checked in. Cannot perform check-out.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                customerstatus("co");

                const string query = @"
                    UPDATE TBLRoom
                    SET Status = @Status
                    WHERE RoomId = @RoomId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        RoomId = roomId,
                        BranchId = branchid,
                        Status = "Available"
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomId} is now Available.",
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
                if (IsApartment())
                {
                    MessageBox.Show(
                        "Cancellation through Customer ID is not available for Apartment reservations.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                DateTime now = DateTime.Now;

                DateTime startDate =
                    RR_checkInDateTXT.Value.Date.Add(
                        RR_Timecheckin_TXT.Value.TimeOfDay);

                string customerId = RR_customertxt.Text.Trim();
                string roomId = RR_RoomIdTxt.Text.Trim();

                if (string.IsNullOrWhiteSpace(customerId) ||
                    string.IsNullOrWhiteSpace(roomId))
                    return;

                const string customerQuery = @"
                    SELECT CustomerStatus
                    FROM CustomerTbl
                    WHERE CustomerId = @CustomerId
                      AND BranchId = @BranchId;";

                string status = Convert.ToString(
                    AppDB.Instance.Scalar(
                        customerQuery,
                        new
                        {
                            CustomerId = customerId,
                            BranchId = branchid
                        }));

                if (now >= startDate)
                {
                    MessageBox.Show(
                        "Cannot cancel the reservation as the check-in date has already passed.",
                        "Cancellation Not Allowed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!string.Equals(
                    status,
                    "Reserved",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Customer is not currently reserved. Cannot perform cancellation.",
                        "Invalid Operation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                customerstatus("cn");

                const string query = @"
                    UPDATE TBLRoom
                    SET Status = @Status
                    WHERE RoomId = @RoomId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        RoomId = roomId,
                        BranchId = branchid,
                        Status = "Available"
                    });

                MessageBox.Show(
                    "Reservation cancelled successfully. Balance will be refunded.",
                    "Cancellation Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                if (result > 0)
                {
                    MessageBox.Show(
                        $"{roomId} is now Available.",
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

        public void loadDisplay_label()
        {
            const string labelDisplayQuery = @"
                SELECT
                    COUNT(CASE WHEN CustomerStatus = 'Reserved' THEN 1 END) AS TotalReserved,
                    COUNT(CASE WHEN CustomerStatus = 'Checked In' THEN 1 END) AS TotalCheckIn,
                    COUNT(CASE WHEN CustomerStatus = 'Checked Out' THEN 1 END) AS TotalCheckOut,
                    COUNT(CASE WHEN CustomerStatus = 'Cancelled' THEN 1 END) AS TotalCancelled
                FROM CustomerTbl
                WHERE BranchId = @BranchId;";

            DataTable reservationData = AppDB.Instance.TableData(
                labelDisplayQuery,
                new { BranchId = branchid });

            if (reservationData.Rows.Count == 0)
                return;

            DataRow reservationRow = reservationData.Rows[0];

            RM_totalResLBL.Text =
                Convert.ToString(reservationRow["TotalReserved"]) ?? "0";

            RR_checkedInLBL.Text =
                Convert.ToString(reservationRow["TotalCheckIn"]) ?? "0";

            RR_checkedOutLBL.Text =
                Convert.ToString(reservationRow["TotalCheckOut"]) ?? "0";

            RR_CancelledLBL.Text =
                Convert.ToString(reservationRow["TotalCancelled"]) ?? "0";
        }
    }
}