using _4C_s_Management_System.Classess;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlDB : UserControl
    {
        public string branchId = AccessBranch_ByForm.BranchId;

        private decimal totalMonthlyIncome = 0;

        // =========================================
        // RECENT PAYMENTS COLORS
        // =========================================

        private readonly Color RecentGreen =
            Color.FromArgb(34, 160, 95);

        private readonly Color RecentDark =
            Color.FromArgb(45, 45, 45);

        private readonly Color RecentGray =
            Color.FromArgb(125, 125, 125);

        // =========================================
        // MAINTENANCE REPORTS COLORS
        // =========================================

        private readonly Color MaintenanceDark =
            Color.FromArgb(45, 45, 45);

        private readonly Color MaintenanceGray =
            Color.FromArgb(125, 125, 125);

        private readonly Color MaintenanceGreen =
            Color.FromArgb(34, 160, 95);

        private readonly Color MaintenanceOrange =
            Color.FromArgb(220, 145, 35);

        private readonly Color MaintenanceRed =
            Color.FromArgb(200, 65, 65);

        public UserControlDB()
        {
            InitializeComponent();

            // DATE AND TIME TIMER
            RM_Timer.Interval = 1000;
            RM_Timer.Tick += RM_Timer_Tick;
        }

        // =========================================
        // NAVIGATION: NEW RESERVATION
        // =========================================

        private void UCDash_NewReservationBtn_Click(
            object sender,
            EventArgs e)
        {
            if (FindForm() is not UserNavigation nav ||
                Parent is not Control parent)
            {
                return;
            }

            nav.reservation();

            UserControlRoomReserv reservationControl =
                new UserControlRoomReserv();

            MainControlClass.ShowControl(
                reservationControl,
                parent
            );
        }

        // =========================================
        // FORM LOAD
        // =========================================

        private void UserControlDB_Load(
            object sender,
            EventArgs e)
        {
            RefreshData();

            RM_Panel_DateTime.FillColor =
                ColorTranslator.FromHtml("#34465A");

            RM_DateTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");

            DB_DateLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            DB_TimeLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            UpdateDateTime();

            RM_Timer.Start();
        }

        // =========================================
        // DATE AND TIME
        // =========================================

        private void RM_Timer_Tick(
            object sender,
            EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            DB_DateLbl.Text =
                DateTime.Now.ToString("MMM dd, yyyy");

            DB_TimeLbl.Text =
                DateTime.Now.ToString("hh:mm:ss tt");
        }

        // =========================================
        // REFRESH ALL DASHBOARD DATA
        // =========================================

        public void RefreshData()
        {
            displayLabels();

            LoadDoughnutChart();

            LoadPropertyOccupancy();

            roomstatusBytypeDGV();

            RcentreservationData();

            LoadMonthlyIncome();

            // TOTAL COLLECTION
            LoadTotalCollection();

            LoadTodaysActivity();

            // RECENT PAYMENTS
            LoadRecentPayments();

            // MAINTENANCE REPORTS
            LoadMaintenanceReports();
        }

        // =========================================
        // ROOM STATUS LABELS
        // =========================================

        public void displayLabels()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(CASE
                            WHEN Status = 'Occupied'
                            THEN 1 END) AS Occupied,

                        COUNT(CASE
                            WHEN Status = 'Available'
                            THEN 1 END) AS Available,

                        COUNT(CASE
                            WHEN Status = 'Maintenance'
                            THEN 1 END) AS Maintenance,

                        COUNT(CASE
                            WHEN Status = 'Reserved'
                            THEN 1 END) AS Reserved

                    FROM TBLRoom
                    WHERE BranchId = @BranchId;
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                if (data.Rows.Count > 0)
                {
                    DataRow row = data.Rows[0];

                    int occupied =
                        Convert.ToInt32(row["Occupied"]);

                    int available =
                        Convert.ToInt32(row["Available"]);

                    int maintenance =
                        Convert.ToInt32(row["Maintenance"]);

                    int reserved =
                        Convert.ToInt32(row["Reserved"]);

                    DB_totalRoomLBL.Text =
                        (occupied + available +
                         maintenance + reserved).ToString();

                    DB_OccupiedRoomLBL.Text =
                        occupied.ToString();

                    DB_availableRoomLBLL.Text =
                        available.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // DOUGHNUT CHART
        // =========================================

        private void LoadDoughnutChart()
        {
            try
            {
                DB_RoomOcc_donat.Datasets.Clear();

                var dataset = new GunaDoughnutDataset()
                {
                    Label = "Room Occupancy"
                };

                const string query = @"
                    SELECT
                        COUNT(CASE
                            WHEN Status = 'Occupied'
                            THEN 1 END) AS Occupied,

                        COUNT(CASE
                            WHEN Status = 'Available'
                            THEN 1 END) AS Available,

                        COUNT(CASE
                            WHEN Status = 'Maintenance'
                            THEN 1 END) AS Maintenance,

                        COUNT(CASE
                            WHEN Status = 'Reserved'
                            THEN 1 END) AS Reserved

                    FROM TBLRoom
                    WHERE BranchId = @BranchId;
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                int totalRooms = 0;
                int occupiedRooms = 0;
                int availableRooms = 0;
                int maintenanceRooms = 0;
                int reservedRooms = 0;

                if (data.Rows.Count > 0)
                {
                    DataRow row = data.Rows[0];

                    occupiedRooms =
                        Convert.ToInt32(row["Occupied"]);

                    availableRooms =
                        Convert.ToInt32(row["Available"]);

                    maintenanceRooms =
                        Convert.ToInt32(row["Maintenance"]);

                    reservedRooms =
                        Convert.ToInt32(row["Reserved"]);

                    totalRooms =
                        occupiedRooms +
                        availableRooms +
                        maintenanceRooms +
                        reservedRooms;
                }

                dataset.DataPoints.Add(
                    "Occupied", occupiedRooms);

                dataset.DataPoints.Add(
                    "Available", availableRooms);

                dataset.DataPoints.Add(
                    "Maintenance", maintenanceRooms);

                dataset.DataPoints.Add(
                    "Reserved", reservedRooms);

                double occupancyPercentage = totalRooms > 0
                    ? (occupiedRooms / (double)totalRooms) * 100
                    : 0;

                double availablePercentage = totalRooms > 0
                    ? (availableRooms / (double)totalRooms) * 100
                    : 0;

                double maintenancePercentage = totalRooms > 0
                    ? (maintenanceRooms / (double)totalRooms) * 100
                    : 0;

                double reservedPercentage = totalRooms > 0
                    ? (reservedRooms / (double)totalRooms) * 100
                    : 0;

                ROO_TotalRoomLBL.Text =
                    totalRooms.ToString();

                ROO_OccupiedLBL.Text =
                    occupiedRooms + " (" +
                    occupancyPercentage.ToString("F1") + "%)";

                ROO_AvailableLBL.Text =
                    availableRooms + " (" +
                    availablePercentage.ToString("F1") + "%)";

                ROO_maintenanceLBL.Text =
                    maintenanceRooms + " (" +
                    maintenancePercentage.ToString("F1") + "%)";

                ROO_ReservedLBL.Text =
                    reservedRooms + " (" +
                    reservedPercentage.ToString("F1") + "%)";

                ROO_occupancyPerLBL.Text =
                    occupancyPercentage.ToString("F1") + "%";

                dataset.FillColors.Add(Color.Maroon);
                dataset.FillColors.Add(Color.Firebrick);
                dataset.FillColors.Add(Color.IndianRed);
                dataset.FillColors.Add(Color.Brown);

                DB_RoomOcc_donat.Datasets.Add(dataset);

                DB_RoomOcc_donat.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading room occupancy chart: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // MONTHLY INCOME CHART
        // =========================================

        private void LoadMonthlyIncome()
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                DateTime startDate =
                    new DateTime(currentYear, 1, 1);

                DateTime endDate =
                    startDate.AddYears(1);

                const string query = @"
                    SELECT
                        MONTH(PaymentDate) AS MonthNumber,
                        ISNULL(SUM(AmountPaid), 0) AS TotalIncome

                    FROM PaymentTbl

                    WHERE BranchID = @BranchID
                      AND PaymentStatus = 'Paid'
                      AND PaymentDate >= @StartDate
                      AND PaymentDate < @EndDate

                    GROUP BY MONTH(PaymentDate)

                    ORDER BY MONTH(PaymentDate);
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        BranchID = branchId,
                        StartDate = startDate,
                        EndDate = endDate
                    }
                );

                string[] monthNames =
                {
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June",
                    "July",
                    "August",
                    "September",
                    "October",
                    "November",
                    "December"
                };

                decimal[] monthlyIncome = new decimal[12];

                foreach (DataRow row in data.Rows)
                {
                    int monthNumber =
                        Convert.ToInt32(row["MonthNumber"]);

                    decimal income =
                        Convert.ToDecimal(row["TotalIncome"]);

                    if (monthNumber >= 1 &&
                        monthNumber <= 12)
                    {
                        monthlyIncome[monthNumber - 1] =
                            income;
                    }
                }

                BarMonthlyIncome.DataPoints.Clear();

                totalMonthlyIncome = 0;

                for (int i = 0; i < 12; i++)
                {
                    BarMonthlyIncome.DataPoints.Add(
                        monthNames[i],
                        (double)monthlyIncome[i]
                    );

                    totalMonthlyIncome += monthlyIncome[i];
                }

                DB_MonthlyIncomeOver.Datasets.Clear();

                DB_MonthlyIncomeOver.Datasets.Add(
                    BarMonthlyIncome
                );

                DB_MonthlyIncomeOver.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading monthly income: {ex.Message}",
                    "Monthly Income Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // TOTAL COLLECTION
        // LABEL: DB_TotalCollection_LBL
        // TABLE: PaymentTbl
        // =========================================

        private void LoadTotalCollection()
        {
            try
            {
                const string query = @"
                    SELECT
                        ISNULL(SUM(AmountPaid), 0)
                    FROM PaymentTbl
                    WHERE BranchID = @BranchID
                      AND PaymentStatus = 'Paid';
                ";

                object result = AppDB.Instance.Scalar(
                    query,
                    new
                    {
                        BranchID = branchId
                    }
                );

                decimal totalCollection =
                    result == null || result == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(result);

                DB_TotalCollection_LBL.Text =
                    "₱ " + totalCollection.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading Total Collection:\n\n" +
                    ex.Message,
                    "Total Collection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // PROPERTY OCCUPANCY
        // =========================================

        public void LoadPropertyOccupancy()
        {
            try
            {
                const string query = @"
                    SELECT
                        RoomType,
                        COUNT(*) AS TotalRooms,

                        SUM(CASE
                            WHEN Status = 'Occupied'
                            THEN 1 ELSE 0 END) AS OccupiedRooms

                    FROM TBLRoom
                    WHERE BranchId = @BranchId
                    GROUP BY RoomType;
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                UpdatePropertyRow(
                    data,
                    "Apartment",
                    OPT_AppartmentProgBar,
                    OPT_appartmenPerLBL,
                    OPT_appartmentstatLBL
                );

                UpdatePropertyRow(
                    data,
                    "Hotel",
                    OPT_HotelProgBar,
                    OPT_HotelPerLBL,
                    OPT_HotelStatLBL
                );

                UpdatePropertyRow(
                    data,
                    "Transient",
                    OPT_TransientProgBar,
                    OPT_transientPerLBL,
                    OPT_transientStatLBL
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading property occupancy: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void UpdatePropertyRow(
            DataTable data,
            string roomType,
            Guna2ProgressBar progress,
            Guna2HtmlLabel percentLabel,
            Guna2HtmlLabel countLabel)
        {
            DataRow[] rows = data.Select(
                $"RoomType = '{roomType.Replace("'", "''")}'"
            );

            int totalRooms = 0;
            int occupiedRooms = 0;

            if (rows.Length > 0)
            {
                totalRooms =
                    Convert.ToInt32(rows[0]["TotalRooms"]);

                occupiedRooms =
                    Convert.ToInt32(rows[0]["OccupiedRooms"]);
            }

            int percentage = totalRooms > 0
                ? (int)Math.Round(
                    (double)occupiedRooms / totalRooms * 100
                  )
                : 0;

            progress.Value = percentage;

            percentLabel.Text =
                $"{percentage}%";

            countLabel.Text =
                $"{occupiedRooms}/{totalRooms} rooms";
        }

        // =========================================
        // ROOM STATUS BY TYPE DATAGRIDVIEW
        // =========================================

        public void roomstatusBytypeDGV()
        {
            try
            {
                const string query = @"
                    SELECT
                        RoomType,
                        Status,
                        COUNT(*) AS Count

                    FROM TBLRoom
                    WHERE BranchId = @BranchId

                    GROUP BY RoomType, Status

                    ORDER BY
                        CASE RoomType
                            WHEN 'Hotel' THEN 1
                            WHEN 'Transient' THEN 2
                            WHEN 'Apartment' THEN 3
                            ELSE 4
                        END,
                        Status ASC;
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                if (Room_statusBy_typeDGV.Columns.Count == 0)
                {
                    Room_statusBy_typeDGV.Columns.Add(
                        "RoomType", "Room Type");

                    Room_statusBy_typeDGV.Columns.Add(
                        "Status", "Status");

                    Room_statusBy_typeDGV.Columns.Add(
                        "Count", "Count");
                }

                Room_statusBy_typeDGV.Rows.Clear();

                foreach (DataRow row in data.Rows)
                {
                    Room_statusBy_typeDGV.Rows.Add(
                        row["RoomType"],
                        row["Status"],
                        row["Count"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading room status by type: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // RECENT RESERVATIONS
        // =========================================

        public void RcentreservationData()
        {
            try
            {
                const string query = @"
                    SELECT TOP 5
                        ReservationId,
                        RoomId,
                        ReservationDate

                    FROM ReservationTbl
                    WHERE BranchId = @BranchId
                    ORDER BY ReservationDate DESC;
                ";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                Recenr_reservationDGV.Visible = true;

                Recenr_reservationDGV.AutoGenerateColumns = true;

                Recenr_reservationDGV.DataSource = null;

                Recenr_reservationDGV.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading recent reservations: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // TODAY'S ACTIVITY
        // =========================================

        public void LoadTodaysActivity()
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // 1. NEW RESERVATIONS TODAY

                const string reservationQuery = @"
                    SELECT COUNT(*)
                    FROM ReservationTbl
                    WHERE BranchId = @BranchId
                      AND ReservationDate >= @Today
                      AND ReservationDate < @Tomorrow;
                ";

                int newReservations = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        reservationQuery,
                        new
                        {
                            BranchId = branchId,
                            Today = today,
                            Tomorrow = tomorrow
                        }
                    )
                );

                guna2HtmlLabel32.Text =
                    newReservations.ToString("N0");

                // 2. CHECK IN TODAY

                const string checkInQuery = @"
                    SELECT COUNT(*)
                    FROM ReservationTbl
                    WHERE BranchId = @BranchId
                      AND ResStartDate >= @Today
                      AND ResStartDate < @Tomorrow;
                ";

                int checkIns = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        checkInQuery,
                        new
                        {
                            BranchId = branchId,
                            Today = today,
                            Tomorrow = tomorrow
                        }
                    )
                );

                guna2HtmlLabel33.Text =
                    checkIns.ToString("N0");

                // 3. CHECK OUT TODAY

                const string checkOutQuery = @"
                    SELECT COUNT(*)
                    FROM ReservationTbl
                    WHERE BranchId = @BranchId
                      AND ResEndDate >= @Today
                      AND ResEndDate < @Tomorrow;
                ";

                int checkOuts = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        checkOutQuery,
                        new
                        {
                            BranchId = branchId,
                            Today = today,
                            Tomorrow = tomorrow
                        }
                    )
                );

                guna2HtmlLabel34.Text =
                    checkOuts.ToString("N0");

                // 4. PAYMENTS RECEIVED TODAY

                const string paymentQuery = @"
                    SELECT ISNULL(SUM(AmountPaid), 0)
                    FROM PaymentTbl
                    WHERE BranchID = @BranchId
                      AND PaymentStatus = 'Paid'
                      AND PaymentDate >= @Today
                      AND PaymentDate < @Tomorrow;
                ";

                object paymentResult = AppDB.Instance.Scalar(
                    paymentQuery,
                    new
                    {
                        BranchId = branchId,
                        Today = today,
                        Tomorrow = tomorrow
                    }
                );

                decimal paymentsReceived =
                    paymentResult == null ||
                    paymentResult == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(paymentResult);

                guna2HtmlLabel35.Text =
                    "₱ " + paymentsReceived.ToString("N2");

                // 5. MAINTENANCE REQUESTS TODAY

                const string maintenanceQuery = @"
                    SELECT COUNT(*)
                    FROM MaintenanceTbl
                    WHERE BranchID = @BranchId
                      AND DateStarted >= @Today
                      AND DateStarted < @Tomorrow;
                ";

                int maintenanceRequests = Convert.ToInt32(
                    AppDB.Instance.Scalar(
                        maintenanceQuery,
                        new
                        {
                            BranchId = branchId,
                            Today = today,
                            Tomorrow = tomorrow
                        }
                    )
                );

                guna2HtmlLabel36.Text =
                    maintenanceRequests.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading Today's Activity:\n\n" +
                    ex.Message,
                    "Today's Activity Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // RECENT PAYMENTS
        // PANEL: guna2Panel13
        // =========================================

        private void LoadRecentPayments()
        {
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                const string query = @"
                    SELECT TOP 4
                        COALESCE(
                            C.Fullname,
                            T.Fullname,
                            'Unknown'
                        ) AS CustomerName,

                        P.PaymentMethod,
                        P.PaymentDate,
                        P.AmountPaid

                    FROM PaymentTbl P

                    LEFT JOIN CustomerTbl C
                        ON P.CustomerID = C.CustomerID

                    LEFT JOIN TenantTbl T
                        ON P.TenantID = T.TenantID

                    WHERE P.BranchID = @BranchID
                      AND P.PaymentStatus = 'Paid'
                      AND P.PaymentDate >= @Today
                      AND P.PaymentDate < @Tomorrow

                    ORDER BY P.PaymentDate DESC;
                ";

                DataTable payments = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        BranchID = branchId,
                        Today = today,
                        Tomorrow = tomorrow
                    }
                );

                const string totalQuery = @"
                    SELECT ISNULL(SUM(AmountPaid), 0)
                    FROM PaymentTbl
                    WHERE BranchID = @BranchID
                      AND PaymentStatus = 'Paid'
                      AND PaymentDate >= @Today
                      AND PaymentDate < @Tomorrow;
                ";

                object totalResult = AppDB.Instance.Scalar(
                    totalQuery,
                    new
                    {
                        BranchID = branchId,
                        Today = today,
                        Tomorrow = tomorrow
                    }
                );

                decimal todayTotal =
                    totalResult == null ||
                    totalResult == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(totalResult);

                CreateRecentPaymentsUI(
                    payments,
                    todayTotal
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading Recent Payments:\n\n" +
                    ex.Message,
                    "Recent Payments Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // CREATE RECENT PAYMENTS UI
        // =========================================

        private void CreateRecentPaymentsUI(
            DataTable payments,
            decimal todayTotal)
        {
            Control oldContent =
                guna2Panel13.Controls["RecentPaymentsContent"];

            if (oldContent != null)
            {
                guna2Panel13.Controls.Remove(oldContent);
                oldContent.Dispose();
            }

            Guna2Panel contentPanel = new Guna2Panel
            {
                Name = "RecentPaymentsContent",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                Padding = new Padding(12)
            };

            Guna2HtmlLabel title = new Guna2HtmlLabel
            {
                Text = "Recent Payments",
                Font = new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Bold
                ),
                ForeColor = RecentDark,
                Location = new Point(12, 10),
                AutoSize = true
            };

            contentPanel.Controls.Add(title);

            FlowLayoutPanel paymentList =
                new FlowLayoutPanel
                {
                    Name = "RecentPaymentList",
                    Location = new Point(12, 43),
                    Size = new Size(
                        Math.Max(100, contentPanel.ClientSize.Width - 24),
                        Math.Max(50, contentPanel.ClientSize.Height - 112)
                    ),
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Bottom |
                        AnchorStyles.Left |
                        AnchorStyles.Right,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    BackColor = Color.White,
                    Padding = new Padding(0, 3, 0, 3)
                };

            contentPanel.Controls.Add(paymentList);

            if (payments.Rows.Count == 0)
            {
                Guna2HtmlLabel emptyLabel =
                    new Guna2HtmlLabel
                    {
                        Text = "No payments received today.",
                        Font = new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Regular
                        ),
                        ForeColor = RecentGray,
                        AutoSize = true,
                        Margin = new Padding(3, 15, 3, 3)
                    };

                paymentList.Controls.Add(emptyLabel);
            }
            else
            {
                foreach (DataRow row in payments.Rows)
                {
                    string customerName =
                        row["CustomerName"] == DBNull.Value
                        ? "Unknown Customer"
                        : Convert.ToString(row["CustomerName"]);

                    string paymentMethod =
                        row["PaymentMethod"] == DBNull.Value
                        ? "Unknown Method"
                        : Convert.ToString(row["PaymentMethod"]);

                    DateTime paymentDate =
                        Convert.ToDateTime(row["PaymentDate"]);

                    decimal amount =
                        Convert.ToDecimal(row["AmountPaid"]);

                    AddRecentPaymentRow(
                        paymentList,
                        customerName,
                        paymentMethod,
                        paymentDate,
                        amount
                    );
                }
            }

            Panel divider = new Panel
            {
                BackColor = Color.FromArgb(230, 230, 230),
                Height = 1,
                Location = new Point(
                    12,
                    Math.Max(45, contentPanel.ClientSize.Height - 58)
                ),
                Width = Math.Max(
                    100,
                    contentPanel.ClientSize.Width - 24
                ),
                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            contentPanel.Controls.Add(divider);

            Guna2HtmlLabel todayLabel =
                new Guna2HtmlLabel
                {
                    Text = "Today's Total:",
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold
                    ),
                    ForeColor = RecentDark,
                    AutoSize = true,
                    Location = new Point(
                        12,
                        contentPanel.ClientSize.Height - 43
                    ),
                    Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Left
                };

            contentPanel.Controls.Add(todayLabel);

            Guna2HtmlLabel totalLabel =
                new Guna2HtmlLabel
                {
                    Text = "₱ " + todayTotal.ToString("N2"),
                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold
                    ),
                    ForeColor = RecentGreen,
                    AutoSize = true,
                    Location = new Point(
                        Math.Max(
                            12,
                            contentPanel.ClientSize.Width - 125
                        ),
                        contentPanel.ClientSize.Height - 44
                    ),
                    Anchor =
                        AnchorStyles.Bottom |
                        AnchorStyles.Right
                };

            contentPanel.Controls.Add(totalLabel);

            guna2Panel13.Controls.Add(contentPanel);

            contentPanel.BringToFront();
        }

        // =========================================
        // ADD ONE RECENT PAYMENT ROW
        // =========================================

        private void AddRecentPaymentRow(
            FlowLayoutPanel parent,
            string customerName,
            string paymentMethod,
            DateTime paymentDate,
            decimal amount)
        {
            int rowWidth = Math.Max(
                180,
                parent.ClientSize.Width - 25
            );

            Panel row = new Panel
            {
                Width = rowWidth,
                Height = 58,
                BackColor = Color.White,
                Margin = new Padding(0, 2, 0, 5)
            };

            Guna2HtmlLabel nameLabel =
                new Guna2HtmlLabel
                {
                    Text = customerName,
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold
                    ),
                    ForeColor = RecentDark,
                    Location = new Point(3, 5),
                    AutoSize = true,
                    MaximumSize = new Size(
                        Math.Max(100, rowWidth - 125),
                        22
                    )
                };

            row.Controls.Add(nameLabel);

            Guna2HtmlLabel detailsLabel =
                new Guna2HtmlLabel
                {
                    Text =
                        paymentMethod + " · " +
                        paymentDate.ToString("h:mm tt"),

                    Font = new Font(
                        "Segoe UI",
                        8,
                        FontStyle.Regular
                    ),

                    ForeColor = RecentGray,

                    Location = new Point(3, 29),

                    AutoSize = true,

                    MaximumSize = new Size(
                        Math.Max(100, rowWidth - 125),
                        20
                    )
                };

            row.Controls.Add(detailsLabel);

            Guna2HtmlLabel amountLabel =
                new Guna2HtmlLabel
                {
                    Text = "+₱" + amount.ToString("N2"),

                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold
                    ),

                    ForeColor = RecentGreen,

                    AutoSize = true,

                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right
                };

            amountLabel.Location = new Point(
                Math.Max(
                    3,
                    rowWidth - amountLabel.PreferredSize.Width - 5
                ),
                17
            );

            row.Controls.Add(amountLabel);

            parent.Controls.Add(row);
        }

        // =========================================
        // MAINTENANCE REPORTS
        // PANEL: guna2Panel14
        // =========================================

        private void LoadMaintenanceReports()
        {
            try
            {
                // GET TOTAL AND RESOLVED MAINTENANCE

                const string summaryQuery = @"
                    SELECT
                        COUNT(*) AS TotalMaintenance,

                        SUM(
                            CASE
                                WHEN DateCompleted IS NOT NULL
                                  OR LOWER(LTRIM(RTRIM(Status)))
                                     IN ('done', 'completed', 'resolved')
                                THEN 1
                                ELSE 0
                            END
                        ) AS ResolvedMaintenance

                    FROM MaintenanceTbl

                    WHERE BranchID = @BranchID;
                ";

                DataTable summaryData =
                    AppDB.Instance.TableData(
                        summaryQuery,
                        new { BranchID = branchId }
                    );

                int totalMaintenance = 0;
                int resolvedMaintenance = 0;

                if (summaryData.Rows.Count > 0)
                {
                    DataRow summaryRow = summaryData.Rows[0];

                    totalMaintenance =
                        Convert.ToInt32(
                            summaryRow["TotalMaintenance"]
                        );

                    resolvedMaintenance =
                        summaryRow["ResolvedMaintenance"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            summaryRow["ResolvedMaintenance"]
                        );
                }

                // GET LATEST 10 MAINTENANCE RECORDS

                const string recordsQuery = @"
                    SELECT TOP 10
                        M.MaintenanceID,
                        R.RoomId AS RoomName,
                        M.Issue,
                        M.DateStarted,
                        M.DateCompleted,
                        M.Status,
                        M.Remarks

                    FROM MaintenanceTbl M

                    INNER JOIN TBLRoom R
                        ON M.RoomID = R.RoomId

                    WHERE M.BranchID = @BranchID
                      AND R.BranchId = @BranchID

                    ORDER BY
                        M.DateStarted DESC,
                        M.MaintenanceID DESC;
                ";

                DataTable maintenanceData =
                    AppDB.Instance.TableData(
                        recordsQuery,
                        new { BranchID = branchId }
                    );

                CreateMaintenanceReportsUI(
                    maintenanceData,
                    totalMaintenance,
                    resolvedMaintenance
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading Maintenance Reports:\n\n" +
                    ex.Message,
                    "Maintenance Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================
        // CREATE MAINTENANCE REPORTS UI
        // =========================================

        private void CreateMaintenanceReportsUI(
            DataTable maintenanceData,
            int totalMaintenance,
            int resolvedMaintenance)
        {
            // REMOVE PREVIOUS DYNAMIC CONTENT

            Control oldContent =
                guna2Panel14.Controls["MaintenanceReportsContent"];

            if (oldContent != null)
            {
                guna2Panel14.Controls.Remove(oldContent);
                oldContent.Dispose();
            }

            // MAIN PANEL

            Guna2Panel mainPanel = new Guna2Panel
            {
                Name = "MaintenanceReportsContent",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                Padding = new Padding(12)
            };

            // HEADER TITLE

            Guna2HtmlLabel titleLabel =
                new Guna2HtmlLabel
                {
                    Text = "Maintenance Reports",
                    Font = new Font(
                        "Segoe UI",
                        12,
                        FontStyle.Bold
                    ),
                    ForeColor = MaintenanceDark,
                    Location = new Point(12, 10),
                    AutoSize = true
                };

            mainPanel.Controls.Add(titleLabel);

            // RESOLVED COUNT

            Guna2HtmlLabel resolvedLabel =
                new Guna2HtmlLabel
                {
                    Text =
                        $"{resolvedMaintenance} of {totalMaintenance} resolved",

                    Font = new Font(
                        "Segoe UI",
                        8,
                        FontStyle.Regular
                    ),

                    ForeColor = MaintenanceGray,

                    Location = new Point(12, 38),

                    AutoSize = true
                };

            mainPanel.Controls.Add(resolvedLabel);

            // PROGRESS PERCENTAGE

            int percentage = totalMaintenance > 0
                ? (int)Math.Round(
                    (double)resolvedMaintenance /
                    totalMaintenance * 100
                  )
                : 0;

            percentage = Math.Max(0, Math.Min(100, percentage));

            Guna2HtmlLabel percentageLabel =
                new Guna2HtmlLabel
                {
                    Text = percentage + "%",
                    Font = new Font(
                        "Segoe UI",
                        8,
                        FontStyle.Bold
                    ),
                    ForeColor = MaintenanceGreen,
                    AutoSize = true,
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right
                };

            percentageLabel.Location = new Point(
                Math.Max(
                    12,
                    mainPanel.ClientSize.Width -
                    percentageLabel.PreferredSize.Width - 15
                ),
                38
            );

            mainPanel.Controls.Add(percentageLabel);

            // PROGRESS BAR

            Guna2ProgressBar progressBar =
                new Guna2ProgressBar
                {
                    Name = "MaintenanceProgressBar",
                    Minimum = 0,
                    Maximum = 100,
                    Value = percentage,
                    BorderRadius = 5,
                    FillColor = Color.FromArgb(235, 235, 235),
                    ProgressColor = MaintenanceGreen,
                    ProgressColor2 = MaintenanceGreen,
                    Height = 8,
                    Location = new Point(12, 61),
                    Width = Math.Max(
                        100,
                        mainPanel.ClientSize.Width - 24
                    ),
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left |
                        AnchorStyles.Right
                };

            mainPanel.Controls.Add(progressBar);

            // MAINTENANCE LIST

            FlowLayoutPanel maintenanceList =
                new FlowLayoutPanel
                {
                    Name = "MaintenanceReportsList",
                    Location = new Point(12, 82),
                    Size = new Size(
                        Math.Max(100, mainPanel.ClientSize.Width - 24),
                        Math.Max(50, mainPanel.ClientSize.Height - 94)
                    ),
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Bottom |
                        AnchorStyles.Left |
                        AnchorStyles.Right,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    BackColor = Color.White,
                    Padding = new Padding(0, 3, 0, 3)
                };

            mainPanel.Controls.Add(maintenanceList);

            // EMPTY STATE

            if (maintenanceData.Rows.Count == 0)
            {
                Guna2HtmlLabel emptyLabel =
                    new Guna2HtmlLabel
                    {
                        Text = "No maintenance records found.",
                        Font = new Font(
                            "Segoe UI",
                            9,
                            FontStyle.Regular
                        ),
                        ForeColor = MaintenanceGray,
                        AutoSize = true,
                        Margin = new Padding(3, 15, 3, 3)
                    };

                maintenanceList.Controls.Add(emptyLabel);
            }
            else
            {
                foreach (DataRow row in maintenanceData.Rows)
                {
                    string roomName =
                        row["RoomName"] == DBNull.Value
                        ? "Unknown Room"
                        : Convert.ToString(row["RoomName"]);

                    string issue =
                        row["Issue"] == DBNull.Value
                        ? "No issue description"
                        : Convert.ToString(row["Issue"]);

                    string status =
                        row["Status"] == DBNull.Value
                        ? "Pending"
                        : Convert.ToString(row["Status"]);

                    string timeText = "Time unavailable";

                    if (row["DateStarted"] != DBNull.Value)
                    {
                        DateTime dateStarted =
                            Convert.ToDateTime(row["DateStarted"]);

                        timeText =
                            dateStarted.ToString("MMM dd, yyyy · h:mm tt");
                    }

                    AddMaintenanceReportRow(
                        maintenanceList,
                        roomName,
                        issue,
                        timeText,
                        status
                    );
                }
            }

            // ADD TO DESIGNER PANEL

            guna2Panel14.Controls.Add(mainPanel);

            mainPanel.BringToFront();
        }

        // =========================================
        // ADD ONE MAINTENANCE REPORT ROW
        // =========================================

        private void AddMaintenanceReportRow(
            FlowLayoutPanel parent,
            string roomName,
            string issue,
            string timeText,
            string status)
        {
            int rowWidth = Math.Max(
                180,
                parent.ClientSize.Width - 25
            );

            Panel row = new Panel
            {
                Width = rowWidth,
                Height = 68,
                BackColor = Color.White,
                Margin = new Padding(0, 2, 0, 3)
            };

            // ROOM NAME

            Guna2HtmlLabel roomLabel =
                new Guna2HtmlLabel
                {
                    Text = roomName,
                    Font = new Font(
                        "Segoe UI",
                        9,
                        FontStyle.Bold
                    ),
                    ForeColor = MaintenanceDark,
                    Location = new Point(3, 3),
                    AutoSize = true,
                    MaximumSize = new Size(
                        Math.Max(80, rowWidth - 115),
                        20
                    )
                };

            row.Controls.Add(roomLabel);

            // ISSUE DESCRIPTION

            Guna2HtmlLabel issueLabel =
                new Guna2HtmlLabel
                {
                    Text = issue,
                    Font = new Font(
                        "Segoe UI",
                        8,
                        FontStyle.Regular
                    ),
                    ForeColor = MaintenanceDark,
                    Location = new Point(3, 24),
                    AutoSize = true,
                    MaximumSize = new Size(
                        Math.Max(80, rowWidth - 115),
                        18
                    )
                };

            row.Controls.Add(issueLabel);

            // DATE AND TIME

            Guna2HtmlLabel timeLabel =
                new Guna2HtmlLabel
                {
                    Text = timeText,
                    Font = new Font(
                        "Segoe UI",
                        7,
                        FontStyle.Regular
                    ),
                    ForeColor = MaintenanceGray,
                    Location = new Point(3, 44),
                    AutoSize = true,
                    MaximumSize = new Size(
                        Math.Max(80, rowWidth - 115),
                        17
                    )
                };

            row.Controls.Add(timeLabel);

            // STATUS COLOR

            string normalizedStatus =
                status.Trim().ToLowerInvariant();

            Color statusColor;

            if (normalizedStatus == "done" ||
                normalizedStatus == "completed" ||
                normalizedStatus == "resolved")
            {
                statusColor = MaintenanceGreen;
            }
            else if (normalizedStatus == "in progress" ||
                     normalizedStatus == "in-progress" ||
                     normalizedStatus == "ongoing")
            {
                statusColor = MaintenanceOrange;
            }
            else
            {
                statusColor = MaintenanceRed;
            }

            // STATUS LABEL

            Guna2HtmlLabel statusLabel =
                new Guna2HtmlLabel
                {
                    Text = status,
                    Font = new Font(
                        "Segoe UI",
                        8,
                        FontStyle.Bold
                    ),
                    ForeColor = statusColor,
                    AutoSize = true,
                    Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Right
                };

            statusLabel.Location = new Point(
                Math.Max(
                    3,
                    rowWidth -
                    statusLabel.PreferredSize.Width - 5
                ),
                25
            );

            row.Controls.Add(statusLabel);

            // DIVIDER

            Panel divider = new Panel
            {
                BackColor = Color.FromArgb(235, 235, 235),
                Height = 1,
                Width = rowWidth - 5,
                Location = new Point(3, 66),
                Anchor =
                    AnchorStyles.Bottom |
                    AnchorStyles.Left |
                    AnchorStyles.Right
            };

            row.Controls.Add(divider);

            parent.Controls.Add(row);
        }
    }
}
