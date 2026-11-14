
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

            RoomReserveDGV.CellClick -= RoomReserveDGV_CellClick;
            RoomReserveDGV.CellClick += RoomReserveDGV_CellClick;

            RR_resTypetxt.SelectedIndexChanged -= AccomodationTXT_selectedIndexChange;
            RR_resTypetxt.SelectedIndexChanged += AccomodationTXT_selectedIndexChange;

            RRes_AccommodationCB.SelectedIndexChanged -= RRes_AccommodationCB_SelectedIndexChanged;
            RRes_AccommodationCB.SelectedIndexChanged += RRes_AccommodationCB_SelectedIndexChanged;

            RR_searchresID.TextChanged -= RR_searchtxtx_textchanged;
            RR_searchresID.TextChanged += RR_searchtxtx_textchanged;
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
            bool hasAccommodation =
                !string.IsNullOrWhiteSpace(RR_resTypetxt.Text);

            RR_customertxt.Enabled = hasAccommodation && !apartment;
            RR_tenantidTxt.Enabled = hasAccommodation && apartment;

            if (!hasAccommodation || apartment)
                RR_customertxt.Clear();

            if (!hasAccommodation || !apartment)
                RR_tenantidTxt.Clear();
        }

        private void UserControlRoomReserv_Load(object sender, EventArgs e)
        {
            branchid = AccessBranch_ByForm.BranchId;

            if (RRes_AccommodationCB.Items.Count > 0 &&
                RRes_AccommodationCB.SelectedIndex < 0)
            {
                RRes_AccommodationCB.SelectedIndex = 0;
            }

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
            ApplyCustomerFilters();
        }

        public void SearchData()
        {
            ApplyCustomerFilters();
        }

        // Search reservation ID.
        private void RR_searchtxtx_textchanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        // Filter by accommodation type only.
        private void RRes_AccommodationCB_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void ApplyCustomerFilters()
        {
            try
            {
                branchid = AccessBranch_ByForm.BranchId;

                string accommodation =
                    RRes_AccommodationCB.Text.Trim();

                string searchId =
                    RR_searchresID.Text.Trim();

                var conditions = new List<string>
                {
                    "R.BranchId = @BranchId"
                };

                var parameters = new Dictionary<string, object>
                {
                    ["BranchId"] = branchid
                };

                // Ignore the accommodation filter when All accommodation is selected.
                if (!string.IsNullOrWhiteSpace(accommodation) &&
                    !accommodation.Equals(
                        "All accommodation",
                        StringComparison.OrdinalIgnoreCase))
                {
                    conditions.Add(
                        "R.AccomodationType = @AccommodationType");

                    parameters["AccommodationType"] = accommodation;
                }

                // Search by reservation ID.
                if (!string.IsNullOrWhiteSpace(searchId))
                {
                    conditions.Add(
                        "R.ReservationId LIKE @ReservationId");

                    parameters["ReservationId"] = "%" + searchId + "%";
                }

                string query = @"
                    SELECT
                        R.ReservationId,
                        R.AccomodationType,
                        R.ReservationDate,
                        R.ResStartDate,
                        R.ResEndDate
                    FROM ReservationTbl R
                    WHERE " + string.Join(" AND ", conditions) + @"
                    ORDER BY R.ReservationId ASC;";

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
                    "Error loading reservation data: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AccomodationTXT_selectedIndexChange(
            object sender, EventArgs e)
        {
            ToggleCustomerTenantInputs();
        }

        // Keep this method if it is connected to your Designer.
        private void accomodationtype_selectedIndexChange(
            object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private bool ValidateReservationInputs(
            string reservationId,
            string customerId,
            string tenantId,
            string roomId,
            string accommodationType,
            DateTime startDate,
            DateTime endDate,
            bool apartment,
            bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(roomId) ||
                string.IsNullOrWhiteSpace(accommodationType))
            {
                MessageBox.Show(
                    "Please fill in all required fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (apartment &&
                string.IsNullOrWhiteSpace(tenantId))
            {
                MessageBox.Show(
                    "Please enter a Tenant ID for an Apartment reservation.",
                    "Missing Tenant ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!apartment &&
                string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show(
                    "Please enter a Customer ID for this reservation.",
                    "Missing Customer ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (startDate >= endDate)
            {
                MessageBox.Show(
                    "Start date must be earlier than end date.",
                    "Invalid Date Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (isUpdate && string.IsNullOrWhiteSpace(reservationId))
            {
                MessageBox.Show(
                    "Please select a reservation first.",
                    "Missing Reservation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool IsRoomAvailable(
            string roomId,
            DateTime startDate,
            DateTime endDate,
            string reservationId = "")
        {
            const string query = @"
                SELECT COUNT(*)
                FROM ReservationTbl
                WHERE BranchId = @BranchId
                  AND RoomId = @RoomId
                  AND ResStartDate < @RequestedEnd
                  AND ResEndDate > @RequestedStart
                  AND ReservationId <> @ReservationId;";

            int count = Convert.ToInt32(
                AppDB.Instance.Scalar(
                    query,
                    new
                    {
                        BranchId = branchid,
                        RoomId = roomId,
                        RequestedStart = startDate,
                        RequestedEnd = endDate,
                        ReservationId = reservationId ?? ""
                    }));

            return count == 0;
        }

        private void RR_AddCusBtn_Click(object sender, EventArgs e)
        {
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

            if (!ValidateReservationInputs(
                "",
                customerId,
                tenantId,
                roomId,
                accommodationType,
                startDate,
                endDate,
                apartment,
                false))
                return;

            if (!string.IsNullOrWhiteSpace(RR_ReserIDTxt.Text))
            {
                MessageBox.Show(
                    "A reservation is already selected. Clear the fields before adding another.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string roomQuery = @"
                    SELECT Status, Rate
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

                if (string.Equals(
                    roomStatus,
                    "Occupied",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "The room is currently occupied.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!IsRoomAvailable(roomId, startDate, endDate))
                {
                    MessageBox.Show(
                        "The room is already reserved during the selected date and time.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string reservationId =
                    IDGenerator.GenerateNextID("RES", branchid);

                const string insertQuery = @"
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

                int result = AppDB.Instance.CUD(
                    insertQuery,
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

                if (result <= 0)
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
                    customerstatus("re");

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

                decimal roomRate = Convert.ToDecimal(room["Rate"]);

                decimal totalHours =
                    Convert.ToDecimal((endDate - startDate).TotalHours);

                decimal amountPaid =
                    decimal.Round((roomRate / 24m) * totalHours, 2);

                if (!apartment)
                {
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
                            PaymentID =
                                IDGenerator.GenerateNextID("PAY", branchid),
                            BranchID = branchid,
                            ReservationID = reservationId,
                            CustomerID = customerId,
                            AmountPaid = amountPaid,
                            PaymentStatus = "Pending"
                        });
                }
                else
                {
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
                            PaymentID =
                                IDGenerator.GenerateNextID("PAY", branchid),
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
                loadDisplay_label();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding reservation: " + ex.Message,
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

            ToggleCustomerTenantInputs();

            RoomReserveDGV.ClearSelection();

            RR_ResIDLBL.Text = "";
            RR_nameLBL.Text = "";
            RR_ContactLBL.Text = "";
            RR_Resstatuslbl.Text = "";
            RR_RoomIDLBL.Text = "";
            RR_checkIndateLBL.Text = "";
            RR_checkOutLBL.Text = "";
            RR_capacityLbl.Text = "";
            RR_priceLBL.Text = "";
            RR_totalpriceLBL.Text = "";
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

            if (!ValidateReservationInputs(
                reservationId,
                customerId,
                tenantId,
                roomId,
                accommodationType,
                startDate,
                endDate,
                apartment,
                true))
                return;

            try
            {
                const string roomQuery = @"
                    SELECT Status
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

                string roomStatus =
                    Convert.ToString(roomData.Rows[0]["Status"]) ?? "";

                if (string.Equals(
                    roomStatus,
                    "Maintenance",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "The selected room is under maintenance.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!IsRoomAvailable(
                    roomId,
                    startDate,
                    endDate,
                    reservationId))
                {
                    MessageBox.Show(
                        "The selected room already has another reservation during this date and time.",
                        "Room Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                const string updateQuery = @"
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
                    updateQuery,
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
                    loadDisplay_label();
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
                    "Error updating reservation: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadReservationIntoFields(int rowIndex)
        {
            if (rowIndex < 0 ||
                rowIndex >= RoomReserveDGV.Rows.Count)
                return;

            DataGridViewRow row = RoomReserveDGV.Rows[rowIndex];

            if (row.IsNewRow || row.Cells.Count < 5)
                return;

            string reservationId =
                Convert.ToString(row.Cells[0].Value) ?? "";

            if (string.IsNullOrWhiteSpace(reservationId))
                return;

            const string query = @"
                SELECT
                    CustomerId,
                    TenantId,
                    RoomId,
                    AccomodationType,
                    ResStartDate,
                    ResEndDate
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

            if (relatedData.Rows.Count == 0)
                return;

            DataRow relatedRow = relatedData.Rows[0];

            RR_ReserIDTxt.Text = reservationId;

            RR_resTypetxt.Text =
                Convert.ToString(relatedRow["AccomodationType"]) ?? "";

            ToggleCustomerTenantInputs();

            RR_customertxt.Text =
                Convert.ToString(relatedRow["CustomerId"]) ?? "";

            RR_tenantidTxt.Text =
                Convert.ToString(relatedRow["TenantId"]) ?? "";

            RR_RoomIdTxt.Text =
                Convert.ToString(relatedRow["RoomId"]) ?? "";

            DateTime startDate =
                Convert.ToDateTime(relatedRow["ResStartDate"]);

            DateTime endDate =
                Convert.ToDateTime(relatedRow["ResEndDate"]);

            RR_checkInDateTXT.Value = startDate;
            RR_Timecheckin_TXT.Value = startDate;

            RR_checkOutDateTxt.Value = endDate;
            RR_Timecheckout_TXT.Value = endDate;

            ToggleCustomerTenantInputs();

            checkstatus();
        }

        private void RoomReserveDGV_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.RowIndex >= RoomReserveDGV.Rows.Count)
                return;

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

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this reservation?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string getRoomQuery = @"
                    SELECT RoomId
                    FROM ReservationTbl
                    WHERE ReservationId = @ReservationId
                      AND BranchId = @BranchId;";

                DataTable reservationData = AppDB.Instance.TableData(
                    getRoomQuery,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid
                    });

                if (reservationData.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Reservation not found.",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string roomId =
                    Convert.ToString(reservationData.Rows[0]["RoomId"]) ?? "";

                const string deleteQuery = @"
                    DELETE FROM ReservationTbl
                    WHERE ReservationId = @ReservationId
                      AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    deleteQuery,
                    new
                    {
                        ReservationId = reservationId,
                        BranchId = branchid
                    });

                if (result > 0)
                {
                    const string updateRoomQuery = @"
                        UPDATE TBLRoom
                        SET Status =
                            CASE
                                WHEN EXISTS
                                (
                                    SELECT 1
                                    FROM ReservationTbl
                                    WHERE ReservationTbl.BranchId = @BranchId
                                      AND ReservationTbl.RoomId = @RoomId
                                      AND ReservationTbl.ResEndDate > GETDATE()
                                )
                                THEN 'Reserved'
                                ELSE 'Available'
                            END
                        WHERE BranchId = @BranchId
                          AND RoomId = @RoomId
                          AND Status <> 'Maintenance';";

                    AppDB.Instance.CUD(
                        updateRoomQuery,
                        new
                        {
                            BranchId = branchid,
                            RoomId = roomId
                        });

                    MessageBox.Show(
                        "Reservation deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                LoadReservationData();
                loadDisplay_label();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error deleting reservation: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void LoadDisplay(int rowIndex)
        {
            if (rowIndex < 0 ||
                rowIndex >= RoomReserveDGV.Rows.Count)
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

            string capacity = "";
            decimal rate = 0m;

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
                        Convert.ToDecimal(roomData.Rows[0]["Rate"]);
                }
            }

            string customerName = "";
            string contactNumber = "";
            string displayStatus = "";

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

            RR_capacityLbl.Text =
                string.IsNullOrWhiteSpace(capacity) ? "N/A" : capacity;

            RR_priceLBL.Text = rate.ToString("C2");

            decimal hourlyRate = rate / 24m;

            decimal totalHours =
                Convert.ToDecimal((endDate - startDate).TotalHours);

            decimal totalPrice =
                decimal.Round(totalHours * hourlyRate, 2);

            RR_totalpriceLBL.Text = totalPrice.ToString("C2");
        }

        private void reservation_cellContent_Click(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
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
                        })) ?? "";

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
                    customerstatus("ci");

                    MessageBox.Show(
                        roomId + " is now Occupied.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadReservationData();
                    loadDisplay_label();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error during check-in: " + ex.Message,
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

                switch (s)
                {
                    case "ci":
                        status = "Checked In";
                        break;

                    case "co":
                        status = "Checked Out";
                        break;

                    case "cn":
                        status = "Cancelled";
                        break;

                    case "re":
                        status = "Reserved";
                        break;

                    case "ns":
                        status = "No Show";
                        break;

                    default:
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

                if (result <= 0)
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
                    "Error updating customer status: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void checkstatus()
        {
            try
            {
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
                        })) ?? "";

                const string reservationQuery = @"
                    SELECT ResStartDate
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

                if (reservationData.Rows.Count == 0 ||
                    reservationData.Rows[0].IsNull("ResStartDate"))
                    return;

                DateTime startDate =
                    Convert.ToDateTime(
                        reservationData.Rows[0]["ResStartDate"]);

                if (DateTime.Now >= startDate &&
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
                    "Error checking customer status: " + ex.Message,
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
                        })) ?? "";

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
                    customerstatus("co");

                    MessageBox.Show(
                        roomId + " is now Available.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadReservationData();
                    loadDisplay_label();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error during check-out: " + ex.Message,
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

                string customerId = RR_customertxt.Text.Trim();
                string roomId = RR_RoomIdTxt.Text.Trim();

                DateTime startDate =
                    RR_checkInDateTXT.Value.Date.Add(
                        RR_Timecheckin_TXT.Value.TimeOfDay);

                if (string.IsNullOrWhiteSpace(customerId) ||
                    string.IsNullOrWhiteSpace(roomId))
                    return;

                if (DateTime.Now >= startDate)
                {
                    MessageBox.Show(
                        "Cannot cancel the reservation as the check-in date has already passed.",
                        "Cancellation Not Allowed",
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
                        })) ?? "";

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

                if (result > 0)
                {
                    MessageBox.Show(
                        "Reservation cancelled successfully. Please process any applicable refund separately.",
                        "Cancellation Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadReservationData();
                    loadDisplay_label();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error during cancellation: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void loadDisplay_label()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(CASE WHEN CustomerStatus = 'Reserved' THEN 1 END) AS TotalReserved,
                        COUNT(CASE WHEN CustomerStatus = 'Checked In' THEN 1 END) AS TotalCheckIn,
                        COUNT(CASE WHEN CustomerStatus = 'Checked Out' THEN 1 END) AS TotalCheckOut,
                        COUNT(CASE WHEN CustomerStatus = 'Cancelled' THEN 1 END) AS TotalCancelled
                    FROM CustomerTbl
                    WHERE BranchId = @BranchId;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchid });

                if (data.Rows.Count == 0)
                    return;

                DataRow row = data.Rows[0];

                RM_totalResLBL.Text =
                    Convert.ToString(row["TotalReserved"]) ?? "0";

                RR_checkedInLBL.Text =
                    Convert.ToString(row["TotalCheckIn"]) ?? "0";

                RR_checkedOutLBL.Text =
                    Convert.ToString(row["TotalCheckOut"]) ?? "0";

                RR_CancelledLBL.Text =
                    Convert.ToString(row["TotalCancelled"]) ?? "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading reservation summary: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
