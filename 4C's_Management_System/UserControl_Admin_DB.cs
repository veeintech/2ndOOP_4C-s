using Guna.Charts.WinForms;
using System.Data;
using System.Globalization;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_DB : UserControl
    {

        private static readonly Color Maroon =Color.FromArgb(128, 0, 32);
        private static readonly Color DarkMaroon = Color.FromArgb(92, 16, 35);
        private static readonly Color Mustard = Color.FromArgb(212, 167, 44);
        private static readonly Color SandyBrown = Color.SandyBrown;
        private static readonly Color LightGray =
            Color.LightGray;

        private bool reportsLoaded = false;

        private System.Windows.Forms.Timer dashboardTimer;

        private Guna.UI2.WinForms.Guna2HtmlLabel apartmentRevenueLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel hotelRevenueLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel transientRevenueLabel;

        private Guna.UI2.WinForms.Guna2HtmlLabel activeTenantLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel inactiveTenantLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel expiredTenantLabel;

        // Constructor
        public UserControl_Admin_DB()
        {
            InitializeComponent();

            this.Load += UserControl_Admin_DB_Load;

            dashboardTimer = new System.Windows.Forms.Timer();
            dashboardTimer.Interval = 1000;
            dashboardTimer.Tick += DashboardTimer_Tick;

            this.Disposed += (s, e) =>
            {
                if (dashboardTimer != null)
                {
                    dashboardTimer.Stop();
                    dashboardTimer.Dispose();
                }
            };
        }

        private void UserControl_Admin_DB_Load(
            object sender,
            EventArgs e)
        {
            if (reportsLoaded)
                return;

            try
            {
                UpdateDashboardDateTime();
                dashboardTimer.Start();

                SetDefaultValues();

                LoadMonthlyRevenue();
                LoadRoomStatus();
                LoadPropertyRevenue();
                LoadReservationsPerMonth();
                LoadTenantStatus();

                LoadContractsEndingSoon();
                LoadPendingMaintenance();
                LoadRecentPayments();
                LoadStaffAttendance();

                reportsLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load Admin Dashboard.\n\n" +
                    ex.Message,
                    "Dashboard Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // DISPLAY CURRENT DATE AND TIME
        private void UpdateDashboardDateTime()
        {
            DateTime now = DateTime.Now;

            DB_DateLbl.Text = now.ToString(
                "MMMM dd, yyyy",
                CultureInfo.InvariantCulture);

            DB_TimeLbl.Text = now.ToString(
                "hh:mm:ss tt",
                CultureInfo.InvariantCulture);
        }

        // TIMER TICK
        private void DashboardTimer_Tick(
            object sender,
            EventArgs e)
        {
            UpdateDashboardDateTime();
        }

        private Guna.UI2.WinForms.Guna2HtmlLabel CreateProgressResultLabel(
            Control alignmentLabel,
            string labelName,
            string initialText)
        {
            Guna.UI2.WinForms.Guna2HtmlLabel resultLabel =
                new Guna.UI2.WinForms.Guna2HtmlLabel();

            resultLabel.Name = labelName;
            resultLabel.Text = initialText;

            resultLabel.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold);

            resultLabel.ForeColor = DarkMaroon;
            resultLabel.BackColor = Color.Transparent;

            resultLabel.AutoSize = false;

            resultLabel.Size = new Size(
                120,
                alignmentLabel.Height);

            resultLabel.TextAlignment =
                ContentAlignment.MiddleRight;

            Control parent = alignmentLabel.Parent;

            if (parent != null)
            {
                parent.Controls.Add(resultLabel);

                resultLabel.Location = new Point(
                    Math.Max(
                        0,
                        parent.ClientSize.Width - resultLabel.Width - 25),
                    alignmentLabel.Top);

                resultLabel.Anchor =
                    AnchorStyles.Top | AnchorStyles.Right;

                resultLabel.BringToFront();

                parent.SizeChanged += (s, e) =>
                {
                    if (!resultLabel.IsDisposed)
                    {
                        resultLabel.Location = new Point(
                            Math.Max(
                                0,
                                parent.ClientSize.Width -
                                resultLabel.Width - 25),
                            alignmentLabel.Top);
                    }
                };
            }
            return resultLabel;
        }

        // INITIALIZE RESULT LABELS
        private void InitializeProgressResultLabels()
        {
            if (apartmentRevenueLabel == null)
            {
                apartmentRevenueLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel39,
                        "ApartmentRevenueResult",
                        "₱ 0.00");
            }

            if (hotelRevenueLabel == null)
            {
                hotelRevenueLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel40,
                        "HotelRevenueResult",
                        "₱ 0.00");
            }

            if (transientRevenueLabel == null)
            {
                transientRevenueLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel41,
                        "TransientRevenueResult",
                        "₱ 0.00");
            }

            if (activeTenantLabel == null)
            {
                activeTenantLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel12,
                        "ActiveTenantResult",
                        "0%");
            }

            if (inactiveTenantLabel == null)
            {
                inactiveTenantLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel10,
                        "InactiveTenantResult",
                        "0%");
            }

            if (expiredTenantLabel == null)
            {
                expiredTenantLabel =
                    CreateProgressResultLabel(
                        guna2HtmlLabel9,
                        "ExpiredTenantResult",
                        "0%");
            }
        }

        private void SetDefaultValues()
        {
            DB_totalRoomLBL.Text = "0";
            DB_availableRoomLBLL.Text = "0";
            DB_OccupiedRoomLBL.Text = "0";

            // TOTAL COLLECTION WITH PESO SIGN
            DB_TotalCollection_LBL.Text = "₱ 0.00";

            ROO_TotalRoomLBL.Text = "0";
            ROO_AvailableLBL.Text = "0 (0.00%)";
            ROO_OccupiedLBL.Text = "0 (0.00%)";
            ROO_ReservedLBL.Text = "0 (0.00%)";

            // PROPERTY REVENUE PROGRESS BARS
            DB_ApartmentProgBar.Minimum = 0;
            DB_ApartmentProgBar.Maximum = 100;
            DB_ApartmentProgBar.Value = 0;

            DB_HotelProgBar.Minimum = 0;
            DB_HotelProgBar.Maximum = 100;
            DB_HotelProgBar.Value = 0;

            DB_TransientProgBar.Minimum = 0;
            DB_TransientProgBar.Maximum = 100;
            DB_TransientProgBar.Value = 0;

            // TENANT STATUS PROGRESS BARS
            DB_ActiveProgBar.Minimum = 0;
            DB_ActiveProgBar.Maximum = 100;
            DB_ActiveProgBar.Value = 0;

            DB_InactiveProgBar.Minimum = 0;
            DB_InactiveProgBar.Maximum = 100;
            DB_InactiveProgBar.Value = 0;

            DB_ExpiredProgBar.Minimum = 0;
            DB_ExpiredProgBar.Maximum = 100;
            DB_ExpiredProgBar.Value = 0;

            InitializeProgressResultLabels();

            apartmentRevenueLabel.Text = "₱ 0.00";
            hotelRevenueLabel.Text = "₱ 0.00";
            transientRevenueLabel.Text = "₱ 0.00";

            activeTenantLabel.Text = "0%";
            inactiveTenantLabel.Text = "0%";
            expiredTenantLabel.Text = "0%";
        }
        private DataTable GetReportData(string query)
        {
            return AppDB.Instance.TableData(query);
        }

        private static double SafeDouble(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            double result;

            return double.TryParse(
                Convert.ToString(value),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result)
                ? result
                : 0;
        }

        private static int SafeInt(object value)
        {
            return Convert.ToInt32(SafeDouble(value));
        }

        // 1. MONTHLY REVENUE OVERVIEW
        private void LoadMonthlyRevenue()
        {
            string query = @"
                SELECT
                    FORMAT(PaymentDate, 'MMM yyyy') AS Label,
                    SUM(ISNULL(AmountPaid, 0)) AS Total,
                    YEAR(PaymentDate) AS SortYear,
                    MONTH(PaymentDate) AS SortMonth
                FROM PaymentTbl
                WHERE PaymentDate IS NOT NULL
                GROUP BY
                    YEAR(PaymentDate),
                    MONTH(PaymentDate),
                    FORMAT(PaymentDate, 'MMM yyyy')
                ORDER BY
                    SortYear,
                    SortMonth;";

            DataTable data = GetReportData(query);

            DB_MonthlyRevenue.Datasets.Clear();
            LineMonthlyRev.DataPoints.Clear();

            LineMonthlyRev.Label = "Monthly Revenue";
            LineMonthlyRev.TargetChart = DB_MonthlyRevenue;

            foreach (DataRow row in data.Rows)
            {
                string month =
                    Convert.ToString(row["Label"]) ?? "Unknown";

                double revenue =
                    SafeDouble(row["Total"]);

                LineMonthlyRev.DataPoints.Add(
                    new LPoint
                    {
                        Label = month,
                        Y = revenue
                    });
            }

            DB_MonthlyRevenue.Datasets.Add(LineMonthlyRev);
            DB_MonthlyRevenue.Update();

            // TOTAL COLLECTION
            string totalQuery = @"
                SELECT ISNULL(SUM(AmountPaid), 0)
                FROM PaymentTbl;";

            object totalResult =
                AppDB.Instance.Scalar(totalQuery);

            double totalCollection =
                SafeDouble(totalResult);

            // DISPLAY TOTAL COLLECTION WITH PESO SIGN
            DB_TotalCollection_LBL.Text =
                "₱ " + totalCollection.ToString("N2");
        }

        // 2. ROOM STATUS
        private void LoadRoomStatus()
        {
            string query = @"
                SELECT
                    CASE
                        WHEN Status = 'Maintenance'
                            THEN 'Under Maintenance'
                        ELSE ISNULL(Status, 'Unknown')
                    END AS Label,
                    COUNT(*) AS Total
                FROM TBLRoom
                GROUP BY
                    CASE
                        WHEN Status = 'Maintenance'
                            THEN 'Under Maintenance'
                        ELSE ISNULL(Status, 'Unknown')
                    END;";

            DataTable data = GetReportData(query);

            DB_StatusChart.Datasets.Clear();

            gunaDoughnutRoomStatus.DataPoints.Clear();
            gunaDoughnutRoomStatus.FillColors.Clear();

            gunaDoughnutRoomStatus.Label = "Room Status";
            gunaDoughnutRoomStatus.TargetChart = DB_StatusChart;

            int available = 0;
            int occupied = 0;
            int reserved = 0;
            int maintenance = 0;

            foreach (DataRow row in data.Rows)
            {
                string label =
                    Convert.ToString(row["Label"]) ?? "";

                int total = SafeInt(row["Total"]);

                if (label.Equals(
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
                {
                    available = total;
                }
                else if (label.Equals(
                    "Occupied",
                    StringComparison.OrdinalIgnoreCase))
                {
                    occupied = total;
                }
                else if (label.Equals(
                    "Reserved",
                    StringComparison.OrdinalIgnoreCase))
                {
                    reserved = total;
                }
                else if (
                    label.Equals(
                        "Under Maintenance",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    label.Equals(
                        "Maintenance",
                        StringComparison.OrdinalIgnoreCase))
                {
                    maintenance = total;
                }
            }

            string totalRoomQuery = @"
                SELECT COUNT(*)
                FROM TBLRoom;";

            int grandTotal = SafeInt(
                AppDB.Instance.Scalar(totalRoomQuery));

            gunaDoughnutRoomStatus.DataPoints.Add(
                new LPoint
                {
                    Label = "Available",
                    Y = available
                });

            gunaDoughnutRoomStatus.DataPoints.Add(
                new LPoint
                {
                    Label = "Occupied",
                    Y = occupied
                });

            gunaDoughnutRoomStatus.DataPoints.Add(
                new LPoint
                {
                    Label = "Reserved",
                    Y = reserved
                });

            gunaDoughnutRoomStatus.DataPoints.Add(
                new LPoint
                {
                    Label = "Under Maintenance",
                    Y = maintenance
                });

            gunaDoughnutRoomStatus.FillColors.Add(Maroon);
            gunaDoughnutRoomStatus.FillColors.Add(Mustard);
            gunaDoughnutRoomStatus.FillColors.Add(SandyBrown);
            gunaDoughnutRoomStatus.FillColors.Add(LightGray);

            DB_StatusChart.Datasets.Add(gunaDoughnutRoomStatus);
            DB_StatusChart.Update();

            DB_totalRoomLBL.Text =
                grandTotal.ToString("N0");

            DB_availableRoomLBLL.Text =
                available.ToString("N0");

            DB_OccupiedRoomLBL.Text =
                occupied.ToString("N0");

            ROO_TotalRoomLBL.Text =
                grandTotal.ToString("N0");

            ROO_AvailableLBL.Text =
                FormatRoomPercentage(available, grandTotal);

            ROO_OccupiedLBL.Text =
                FormatRoomPercentage(occupied, grandTotal);

            ROO_ReservedLBL.Text =
                FormatRoomPercentage(reserved, grandTotal);
        }

        // ROOM STATUS PERCENTAGE
        private string FormatRoomPercentage(
            int count,
            int grandTotal)
        {
            double percentage = grandTotal > 0
                ? count * 100.0 / grandTotal
                : 0;

            return $"{count:N0} ({percentage:0.00}%)";
        }

        // 3. REVENUE BY PROPERTY TYPE
        private void LoadPropertyRevenue()
        {
            string query = @"
                SELECT
                    R.RoomType,
                    SUM(ISNULL(P.AmountPaid, 0)) AS TotalRevenue
                FROM PaymentTbl P
                INNER JOIN ReservationTbl Res
                    ON P.ReservationID = Res.ReservationId
                INNER JOIN TBLRoom R
                    ON Res.RoomId = R.RoomId
                WHERE P.ReservationID IS NOT NULL
                GROUP BY R.RoomType;";

            DataTable data = GetReportData(query);

            double apartmentRevenue = 0;
            double hotelRevenue = 0;
            double transientRevenue = 0;

            foreach (DataRow row in data.Rows)
            {
                string roomType =
                    Convert.ToString(row["RoomType"]) ?? "";

                double revenue =
                    SafeDouble(row["TotalRevenue"]);

                if (roomType.Equals(
                    "Apartment",
                    StringComparison.OrdinalIgnoreCase))
                {
                    apartmentRevenue = revenue;
                }
                else if (roomType.Equals(
                    "Hotel",
                    StringComparison.OrdinalIgnoreCase))
                {
                    hotelRevenue = revenue;
                }
                else if (roomType.Equals(
                    "Transient",
                    StringComparison.OrdinalIgnoreCase))
                {
                    transientRevenue = revenue;
                }
            }

            double grandTotal =
                apartmentRevenue +
                hotelRevenue +
                transientRevenue;

            UpdatePropertyRevenue(
                apartmentRevenue,
                grandTotal,
                DB_ApartmentProgBar,
                apartmentRevenueLabel);

            UpdatePropertyRevenue(
                hotelRevenue,
                grandTotal,
                DB_HotelProgBar,
                hotelRevenueLabel);

            UpdatePropertyRevenue(
                transientRevenue,
                grandTotal,
                DB_TransientProgBar,
                transientRevenueLabel);
        }

        // =========================================
        // UPDATE PROPERTY REVENUE PROGRESS BAR
        // =========================================

        private void UpdatePropertyRevenue(
            double revenue,
            double grandTotal,
            Guna.UI2.WinForms.Guna2ProgressBar progressBar,
            Guna.UI2.WinForms.Guna2HtmlLabel resultLabel)
        {
            double percentage = grandTotal > 0
                ? revenue / grandTotal * 100
                : 0;

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            progressBar.Value = Math.Max(
                0,
                Math.Min(
                    100,
                    (int)Math.Round(percentage)));

            resultLabel.Text =
                "₱ " + revenue.ToString("N2");
        }

        // =========================================
        // 4. RESERVATIONS PER MONTH
        // CHART: DB_Reservations
        // DATASET: BarReservations
        // TABLE: ReservationTbl
        // =========================================

        private void LoadReservationsPerMonth()
        {
            string query = @"
                SELECT
                    FORMAT(ReservationDate, 'MMM yyyy') AS MonthName,
                    YEAR(ReservationDate) AS SortYear,
                    MONTH(ReservationDate) AS SortMonth,
                    COUNT(*) AS TotalReservations
                FROM ReservationTbl
                WHERE ReservationDate >= DATEADD(
                    MONTH,
                    -11,
                    DATEFROMPARTS(
                        YEAR(GETDATE()),
                        MONTH(GETDATE()),
                        1
                    )
                )
                GROUP BY
                    YEAR(ReservationDate),
                    MONTH(ReservationDate),
                    FORMAT(ReservationDate, 'MMM yyyy')
                ORDER BY
                    SortYear,
                    SortMonth;";

            DataTable data = GetReportData(query);

            DB_Reservations.Datasets.Clear();
            BarReservations.DataPoints.Clear();

            BarReservations.Label = "Reservations";
            BarReservations.TargetChart = DB_Reservations;

            foreach (DataRow row in data.Rows)
            {
                string month =
                    Convert.ToString(row["MonthName"]) ?? "Unknown";

                double totalReservations =
                    SafeDouble(row["TotalReservations"]);

                BarReservations.DataPoints.Add(
                    new LPoint
                    {
                        Label = month,
                        Y = totalReservations
                    });
            }

            DB_Reservations.Datasets.Add(BarReservations);
            DB_Reservations.Update();
        }

        // =========================================
        // 5. TENANT STATUS
        // TABLE: TenantTbl
        // =========================================

        private void LoadTenantStatus()
        {
            string query = @"
                SELECT
                    SUM(
                        CASE
                            WHEN UPPER(LTRIM(RTRIM(Status))) = 'ACTIVE'
                            THEN 1 ELSE 0
                        END
                    ) AS ActiveCount,

                    SUM(
                        CASE
                            WHEN UPPER(LTRIM(RTRIM(Status))) = 'INACTIVE'
                            THEN 1 ELSE 0
                        END
                    ) AS InactiveCount,

                    SUM(
                        CASE
                            WHEN UPPER(LTRIM(RTRIM(Status))) = 'EXPIRED'
                            THEN 1 ELSE 0
                        END
                    ) AS ExpiredCount,

                    COUNT(*) AS TotalTenants

                FROM TenantTbl;";

            DataTable data = GetReportData(query);

            int active = 0;
            int inactive = 0;
            int expired = 0;
            int total = 0;

            if (data.Rows.Count > 0)
            {
                DataRow row = data.Rows[0];

                active = SafeInt(row["ActiveCount"]);
                inactive = SafeInt(row["InactiveCount"]);
                expired = SafeInt(row["ExpiredCount"]);
                total = SafeInt(row["TotalTenants"]);
            }

            int activePercentage = total > 0
                ? (int)Math.Round(active * 100.0 / total)
                : 0;

            int inactivePercentage = total > 0
                ? (int)Math.Round(inactive * 100.0 / total)
                : 0;

            int expiredPercentage = total > 0
                ? (int)Math.Round(expired * 100.0 / total)
                : 0;

            DB_ActiveProgBar.Minimum = 0;
            DB_ActiveProgBar.Maximum = 100;
            DB_ActiveProgBar.Value = activePercentage;

            DB_InactiveProgBar.Minimum = 0;
            DB_InactiveProgBar.Maximum = 100;
            DB_InactiveProgBar.Value = inactivePercentage;

            DB_ExpiredProgBar.Minimum = 0;
            DB_ExpiredProgBar.Maximum = 100;
            DB_ExpiredProgBar.Value = expiredPercentage;

            // DISPLAY PERCENTAGES AT UPPER RIGHT

            activeTenantLabel.Text =
                activePercentage + "%";

            inactiveTenantLabel.Text =
                inactivePercentage + "%";

            expiredTenantLabel.Text =
                expiredPercentage + "%";
        }

        // =========================================
        // 6. CONTRACTS ENDING SOON
        // PANEL: guna2Panel7
        // =========================================

        private void LoadContractsEndingSoon()
        {
            string query = @"
                SELECT TOP 3
                    T.TenantId,
                    T.FullName,
                    T.AgreementStartDate,
                    T.AgreementEndDate,
                    R.RoomId,
                    DATEDIFF(
                        DAY,
                        CAST(GETDATE() AS DATE),
                        CAST(T.AgreementEndDate AS DATE)
                    ) AS DaysLeft

                FROM TenantTbl T

                OUTER APPLY
                (
                    SELECT TOP 1
                        Res.RoomId
                    FROM ReservationTbl Res
                    WHERE Res.TenantId = T.TenantId
                    ORDER BY Res.ResEndDate DESC
                ) R

                WHERE
                    T.AgreementEndDate >= CAST(GETDATE() AS DATE)
                    AND T.AgreementEndDate < DATEADD(
                        DAY,
                        31,
                        CAST(GETDATE() AS DATE)
                    )

                ORDER BY
                    T.AgreementEndDate ASC;";

            DataTable data = GetReportData(query);

            // REMOVE PREVIOUSLY GENERATED CONTROLS

            for (int i = guna2Panel7.Controls.Count - 1; i >= 0; i--)
            {
                Control control = guna2Panel7.Controls[i];

                if (control.Tag != null &&
                    control.Tag.ToString() == "ContractRow")
                {
                    guna2Panel7.Controls.Remove(control);
                    control.Dispose();
                }
            }

            guna2HtmlLabel22.Text = "Contracts Ending Soon";

            // DYNAMIC NEXT 30 DAYS SUBTITLE

            Guna.UI2.WinForms.Guna2HtmlLabel next30DaysLabel =
                new Guna.UI2.WinForms.Guna2HtmlLabel();

            next30DaysLabel.Text = "Next 30 Days";

            next30DaysLabel.Font =
                new Font("Segoe UI", 10F, FontStyle.Regular);

            next30DaysLabel.ForeColor =
                Color.FromArgb(90, 90, 90);

            next30DaysLabel.BackColor = Color.Transparent;
            next30DaysLabel.AutoSize = true;

            next30DaysLabel.Location = new Point(
                Math.Max(25, guna2Panel7.Width - 130),
                38);

            next30DaysLabel.Tag = "ContractRow";

            guna2Panel7.Controls.Add(next30DaysLabel);

            if (data.Rows.Count == 0)
            {
                Guna.UI2.WinForms.Guna2HtmlLabel emptyLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                emptyLabel.Text =
                    "No contracts ending in the next 30 days.";

                emptyLabel.Font = new Font("Segoe UI", 10F);
                emptyLabel.ForeColor = Color.Gray;
                emptyLabel.BackColor = Color.Transparent;
                emptyLabel.Location = new Point(25, 100);
                emptyLabel.AutoSize = true;
                emptyLabel.Tag = "ContractRow";

                guna2Panel7.Controls.Add(emptyLabel);

                return;
            }

            int startY = 82;
            int rowSpacing = 71;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];

                string tenantName =
                    Convert.ToString(row["FullName"]) ?? "Unknown Tenant";

                string roomId =
                    row["RoomId"] == DBNull.Value
                        ? "No Room"
                        : Convert.ToString(row["RoomId"]) ?? "No Room";

                DateTime endDate =
                    Convert.ToDateTime(row["AgreementEndDate"]);

                int daysLeft =
                    Convert.ToInt32(row["DaysLeft"]);

                int y = startY + (i * rowSpacing);

                Guna.UI2.WinForms.Guna2HtmlLabel tenantLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                tenantLabel.Text =
                    $"{roomId} · {tenantName}";

                tenantLabel.Font =
                    new Font("Segoe UI", 11F, FontStyle.Regular);

                tenantLabel.ForeColor =
                    Color.FromArgb(35, 35, 35);

                tenantLabel.BackColor = Color.Transparent;
                tenantLabel.Location = new Point(25, y);
                tenantLabel.AutoSize = true;
                tenantLabel.Tag = "ContractRow";

                Guna.UI2.WinForms.Guna2HtmlLabel dateLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                dateLabel.Text =
                    "Ends " + endDate.ToString("MMM dd, yyyy");

                dateLabel.Font =
                    new Font("Segoe UI", 9.5F);

                dateLabel.ForeColor = Color.Gray;
                dateLabel.BackColor = Color.Transparent;
                dateLabel.Location = new Point(25, y + 26);
                dateLabel.AutoSize = true;
                dateLabel.Tag = "ContractRow";

                Guna.UI2.WinForms.Guna2Panel daysPanel =
                    new Guna.UI2.WinForms.Guna2Panel();

                daysPanel.Size = new Size(74, 30);
                daysPanel.Location = new Point(510, y + 5);
                daysPanel.BorderRadius = 15;
                daysPanel.Tag = "ContractRow";

                if (daysLeft <= 10)
                {
                    daysPanel.FillColor =
                        Color.FromArgb(250, 220, 220);
                }
                else
                {
                    daysPanel.FillColor =
                        Color.FromArgb(255, 240, 195);
                }

                Guna.UI2.WinForms.Guna2HtmlLabel daysLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                daysLabel.Text =
                    daysLeft + " days";

                daysLabel.Font =
                    new Font("Segoe UI", 9.5F, FontStyle.Bold);

                daysLabel.ForeColor = daysLeft <= 10
                    ? Color.FromArgb(190, 35, 35)
                    : Color.FromArgb(145, 100, 0);

                daysLabel.BackColor = Color.Transparent;
                daysLabel.Dock = DockStyle.Fill;

                daysLabel.TextAlignment =
                    ContentAlignment.MiddleCenter;

                daysPanel.Controls.Add(daysLabel);

                guna2Panel7.Controls.Add(tenantLabel);
                guna2Panel7.Controls.Add(dateLabel);
                guna2Panel7.Controls.Add(daysPanel);

                if (i < data.Rows.Count - 1)
                {
                    Panel divider = new Panel();

                    divider.BackColor =
                        Color.FromArgb(235, 220, 205);

                    divider.Size = new Size(566, 1);
                    divider.Location = new Point(25, y + 54);
                    divider.Tag = "ContractRow";

                    guna2Panel7.Controls.Add(divider);
                }
            }
        }

        // =========================================
        // 7. PENDING MAINTENANCE
        // PANEL: guna2Panel8
        // =========================================

        private void LoadPendingMaintenance()
        {
            string query = @"
                SELECT TOP 3
                    Issue,
                    RoomID,
                    DateStarted,
                    Status
                FROM MaintenanceTbl
                WHERE
                    UPPER(LTRIM(RTRIM(Status))) IN
                    ('PENDING', 'ONGOING')
                ORDER BY
                    CASE
                        WHEN DateStarted IS NULL THEN 1
                        ELSE 0
                    END,
                    DateStarted DESC;";

            DataTable data = GetReportData(query);

            string countQuery = @"
                SELECT COUNT(*)
                FROM MaintenanceTbl
                WHERE
                    UPPER(LTRIM(RTRIM(Status))) IN
                    ('PENDING', 'ONGOING');";

            int openCount = SafeInt(
                AppDB.Instance.Scalar(countQuery));

            // REMOVE PREVIOUSLY GENERATED CONTROLS

            for (int i = guna2Panel8.Controls.Count - 1; i >= 0; i--)
            {
                Control control = guna2Panel8.Controls[i];

                if (control.Tag != null &&
                    control.Tag.ToString() == "MaintenanceRow")
                {
                    guna2Panel8.Controls.Remove(control);
                    control.Dispose();
                }
            }

            // COUNT LABEL ONLY
            // TITLE IS ALREADY IN DESIGNER

            Guna.UI2.WinForms.Guna2HtmlLabel countLabel =
                new Guna.UI2.WinForms.Guna2HtmlLabel();

            countLabel.Text = openCount + " open";

            countLabel.Font =
                new Font("Segoe UI", 10F);

            countLabel.ForeColor =
                Color.FromArgb(90, 90, 90);

            countLabel.BackColor = Color.Transparent;
            countLabel.AutoSize = true;
            countLabel.Tag = "MaintenanceRow";

            countLabel.Location = new Point(
                Math.Max(25, guna2Panel8.Width - 75),
                38);

            guna2Panel8.Controls.Add(countLabel);

            if (data.Rows.Count == 0)
            {
                Guna.UI2.WinForms.Guna2HtmlLabel emptyLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                emptyLabel.Text =
                    "No pending maintenance records.";

                emptyLabel.Font =
                    new Font("Segoe UI", 10F);

                emptyLabel.ForeColor = Color.Gray;
                emptyLabel.BackColor = Color.Transparent;
                emptyLabel.Location = new Point(25, 105);
                emptyLabel.AutoSize = true;
                emptyLabel.Tag = "MaintenanceRow";

                guna2Panel8.Controls.Add(emptyLabel);

                return;
            }

            int startY = 94;
            int rowSpacing = 71;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];

                string issue =
                    row["Issue"] == DBNull.Value
                        ? "Maintenance issue"
                        : Convert.ToString(row["Issue"]) ?? "Maintenance issue";

                string roomId =
                    row["RoomID"] == DBNull.Value
                        ? "Unknown Room"
                        : Convert.ToString(row["RoomID"]) ?? "Unknown Room";

                string status =
                    row["Status"] == DBNull.Value
                        ? "Pending"
                        : Convert.ToString(row["Status"]) ?? "Pending";

                string normalizedStatus = status.Trim();

                string reportedDate = "Date not available";

                if (row["DateStarted"] != DBNull.Value)
                {
                    DateTime dateStarted =
                        Convert.ToDateTime(row["DateStarted"]);

                    reportedDate =
                        "Reported " + dateStarted.ToString("MMM dd");
                }

                int y = startY + (i * rowSpacing);

                Guna.UI2.WinForms.Guna2HtmlLabel issueLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                issueLabel.Text =
                    $"{issue} · {roomId}";

                issueLabel.Font =
                    new Font("Segoe UI", 11F, FontStyle.Regular);

                issueLabel.ForeColor =
                    Color.FromArgb(35, 35, 35);

                issueLabel.BackColor = Color.Transparent;
                issueLabel.Location = new Point(25, y);
                issueLabel.AutoSize = true;
                issueLabel.MaximumSize =
                    new Size(440, 0);

                issueLabel.Tag = "MaintenanceRow";

                Guna.UI2.WinForms.Guna2HtmlLabel dateLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                dateLabel.Text = reportedDate;

                dateLabel.Font =
                    new Font("Segoe UI", 9.5F);

                dateLabel.ForeColor = Color.Gray;
                dateLabel.BackColor = Color.Transparent;
                dateLabel.Location = new Point(25, y + 25);
                dateLabel.AutoSize = true;
                dateLabel.Tag = "MaintenanceRow";

                Guna.UI2.WinForms.Guna2Panel statusPanel =
                    new Guna.UI2.WinForms.Guna2Panel();

                statusPanel.Size = new Size(84, 30);

                statusPanel.Location = new Point(
                    guna2Panel8.Width - 110,
                    y + 4);

                statusPanel.BorderRadius = 15;
                statusPanel.Tag = "MaintenanceRow";

                bool isPending =
                    normalizedStatus.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase);

                statusPanel.FillColor = isPending
                    ? Color.FromArgb(250, 220, 220)
                    : Color.FromArgb(255, 240, 195);

                Guna.UI2.WinForms.Guna2HtmlLabel statusLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                statusLabel.Text = normalizedStatus;

                statusLabel.Font =
                    new Font("Segoe UI", 9.5F, FontStyle.Bold);

                statusLabel.ForeColor = isPending
                    ? Color.FromArgb(190, 35, 35)
                    : Color.FromArgb(145, 100, 0);

                statusLabel.BackColor = Color.Transparent;
                statusLabel.Dock = DockStyle.Fill;

                statusLabel.TextAlignment =
                    ContentAlignment.MiddleCenter;

                statusPanel.Controls.Add(statusLabel);

                guna2Panel8.Controls.Add(issueLabel);
                guna2Panel8.Controls.Add(dateLabel);
                guna2Panel8.Controls.Add(statusPanel);

                if (i < data.Rows.Count - 1)
                {
                    Panel divider = new Panel();

                    divider.BackColor =
                        Color.FromArgb(235, 220, 205);

                    divider.Size = new Size(
                        Math.Max(100, guna2Panel8.Width - 52),
                        1);

                    divider.Location =
                        new Point(25, y + 54);

                    divider.Tag = "MaintenanceRow";

                    guna2Panel8.Controls.Add(divider);
                }
            }
        }

        // =========================================
        // 8. RECENT PAYMENTS
        // PANEL: guna2Panel12
        // =========================================

        private void LoadRecentPayments()
        {
            string query = @"
                SELECT TOP 3
                    P.PaymentID,
                    P.PaymentDate,
                    P.AmountPaid,

                    COALESCE(
                        T.FullName,
                        C.Fullname,
                        'Unknown Customer'
                    ) AS CustomerName,

                    CASE
                        WHEN P.TenantID IS NOT NULL
                            THEN 'Rent'
                        WHEN P.CustomerID IS NOT NULL
                            THEN 'Reservation'
                        ELSE 'Payment'
                    END AS PaymentType

                FROM PaymentTbl P

                LEFT JOIN TenantTbl T
                    ON P.TenantID = T.TenantId

                LEFT JOIN CustomerTbl C
                    ON P.CustomerID = C.CustomerID

                ORDER BY
                    P.PaymentDate DESC,
                    P.PaymentID DESC;";

            DataTable data = GetReportData(query);

            // REMOVE PREVIOUSLY GENERATED CONTROLS

            for (int i = guna2Panel12.Controls.Count - 1; i >= 0; i--)
            {
                Control control = guna2Panel12.Controls[i];

                if (control.Tag != null &&
                    control.Tag.ToString() == "RecentPaymentRow")
                {
                    guna2Panel12.Controls.Remove(control);
                    control.Dispose();
                }
            }

            if (data.Rows.Count == 0)
            {
                Guna.UI2.WinForms.Guna2HtmlLabel emptyLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                emptyLabel.Text = "No recent payments found.";
                emptyLabel.Font = new Font("Segoe UI", 10F);
                emptyLabel.ForeColor = Color.Gray;
                emptyLabel.BackColor = Color.Transparent;
                emptyLabel.Location = new Point(25, 85);
                emptyLabel.AutoSize = true;
                emptyLabel.Tag = "RecentPaymentRow";

                guna2Panel12.Controls.Add(emptyLabel);

                return;
            }

            int startY = 75;
            int rowSpacing = 70;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];

                string customerName =
                    Convert.ToString(row["CustomerName"])
                    ?? "Unknown Customer";

                string paymentType =
                    Convert.ToString(row["PaymentType"])
                    ?? "Payment";

                string paymentDate = "Date unavailable";

                if (row["PaymentDate"] != DBNull.Value)
                {
                    DateTime date =
                        Convert.ToDateTime(row["PaymentDate"]);

                    paymentDate = date.ToString("MMM dd, yyyy");
                }

                double amount =
                    SafeDouble(row["AmountPaid"]);

                int y = startY + (i * rowSpacing);

                Guna.UI2.WinForms.Guna2HtmlLabel nameLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                nameLabel.Text =
                    $"{customerName} · {paymentType}";

                nameLabel.Font =
                    new Font("Segoe UI", 11F, FontStyle.Regular);

                nameLabel.ForeColor =
                    Color.FromArgb(35, 35, 35);

                nameLabel.BackColor = Color.Transparent;
                nameLabel.Location = new Point(25, y);
                nameLabel.AutoSize = true;
                nameLabel.MaximumSize = new Size(350, 0);
                nameLabel.Tag = "RecentPaymentRow";

                Guna.UI2.WinForms.Guna2HtmlLabel dateLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                dateLabel.Text = paymentDate;

                dateLabel.Font =
                    new Font("Segoe UI", 9.5F);

                dateLabel.ForeColor = Color.Gray;
                dateLabel.BackColor = Color.Transparent;
                dateLabel.Location = new Point(25, y + 25);
                dateLabel.AutoSize = true;
                dateLabel.Tag = "RecentPaymentRow";

                Guna.UI2.WinForms.Guna2HtmlLabel amountLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                amountLabel.Text =
                    "₱ " + amount.ToString("N2");

                amountLabel.Font =
                    new Font("Segoe UI", 10F, FontStyle.Bold);

                amountLabel.ForeColor = Maroon;
                amountLabel.BackColor = Color.Transparent;
                amountLabel.AutoSize = true;
                amountLabel.Tag = "RecentPaymentRow";

                amountLabel.Location = new Point(
                    Math.Max(25, guna2Panel12.Width - 150),
                    y + 8);

                guna2Panel12.Controls.Add(nameLabel);
                guna2Panel12.Controls.Add(dateLabel);
                guna2Panel12.Controls.Add(amountLabel);

                if (i < data.Rows.Count - 1)
                {
                    Panel divider = new Panel();

                    divider.BackColor =
                        Color.FromArgb(235, 220, 205);

                    divider.Size = new Size(
                        Math.Max(100, guna2Panel12.Width - 52),
                        1);

                    divider.Location =
                        new Point(25, y + 54);

                    divider.Tag = "RecentPaymentRow";

                    guna2Panel12.Controls.Add(divider);
                }
            }
        }

        // =========================================
        // 9. STAFF ATTENDANCE TODAY
        // PANEL: guna2Panel13
        // TABLES: StaffTbl, AttendanceTbl
        // =========================================

        private void LoadStaffAttendance()
        {
            string query = @"
                SELECT
                    COUNT(
                        CASE
                            WHEN A.Time_IN IS NOT NULL
                            THEN 1
                        END
                    ) AS PresentCount,

                    COUNT(
                        CASE
                            WHEN A.AttendanceID IS NULL
                                 OR A.Time_IN IS NULL
                            THEN 1
                        END
                    ) AS AbsentCount

                FROM StaffTbl S

                LEFT JOIN AttendanceTbl A
                    ON S.StaffID = A.StaffID
                    AND A.[Date] = CAST(GETDATE() AS DATE);";

            int totalStaff = SafeInt(
                AppDB.Instance.Scalar(
                    "SELECT COUNT(*) FROM StaffTbl;"));

            DataTable data = GetReportData(query);

            int present = 0;
            int absent = 0;

            if (data.Rows.Count > 0)
            {
                DataRow row = data.Rows[0];

                present = SafeInt(row["PresentCount"]);
                absent = SafeInt(row["AbsentCount"]);
            }

            Guna.UI2.WinForms.Guna2Panel panel = guna2Panel13;

            // REMOVE PREVIOUSLY GENERATED CONTROLS

            for (int i = panel.Controls.Count - 1; i >= 0; i--)
            {
                Control control = panel.Controls[i];

                if (control.Tag != null &&
                    control.Tag.ToString() == "StaffRow")
                {
                    panel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            // UPDATE EXISTING DESIGNER SUBTITLE

            Guna.UI2.WinForms.Guna2HtmlLabel designerSubHeader = null;

            foreach (Control control in panel.Controls)
            {
                Guna.UI2.WinForms.Guna2HtmlLabel label =
                    control as Guna.UI2.WinForms.Guna2HtmlLabel;

                if (label != null &&
                    label.Text != null &&
                    label.Text.IndexOf(
                        "Next 30 Days",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    designerSubHeader = label;
                    break;
                }
            }

            if (designerSubHeader != null)
            {
                designerSubHeader.Text = totalStaff + " staff";
            }
            else
            {
                Guna.UI2.WinForms.Guna2HtmlLabel countHeader =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                countHeader.Text = totalStaff + " staff";
                countHeader.Font = new Font("Segoe UI", 10F);
                countHeader.ForeColor = Color.FromArgb(90, 90, 90);
                countHeader.BackColor = Color.Transparent;
                countHeader.AutoSize = true;
                countHeader.Tag = "StaffRow";

                countHeader.Location = new Point(
                    Math.Max(25, panel.Width - 90),
                    38);

                panel.Controls.Add(countHeader);
            }

            // PRESENT AND ABSENT ONLY

            var items = new[]
            {
                new
                {
                    Name = "Present",
                    Count = present
                },
                new
                {
                    Name = "Absent",
                    Count = absent
                }
            };

            int startY = 85;
            int rowSpacing = 85;

            for (int i = 0; i < items.Length; i++)
            {
                int y = startY + (i * rowSpacing);

                // STATUS LABEL

                Guna.UI2.WinForms.Guna2HtmlLabel nameLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                nameLabel.Text = items[i].Name;

                nameLabel.Font =
                    new Font("Segoe UI", 11F, FontStyle.Bold);

                nameLabel.ForeColor =
                    Color.FromArgb(35, 35, 35);

                nameLabel.BackColor = Color.Transparent;
                nameLabel.AutoSize = false;

                nameLabel.Size = new Size(
                    Math.Max(100, panel.Width - 50),
                    25);

                nameLabel.Location = new Point(25, y);

                nameLabel.TextAlignment =
                    ContentAlignment.MiddleCenter;

                nameLabel.Tag = "StaffRow";

                // YELLOW COUNT PILL

                Guna.UI2.WinForms.Guna2Panel pill =
                    new Guna.UI2.WinForms.Guna2Panel();

                pill.Size = new Size(65, 32);
                pill.BorderRadius = 16;

                pill.FillColor =
                    Color.FromArgb(255, 240, 195);

                pill.Location = new Point(
                    (panel.Width - pill.Width) / 2,
                    y + 29);

                pill.Tag = "StaffRow";

                Guna.UI2.WinForms.Guna2HtmlLabel countLabel =
                    new Guna.UI2.WinForms.Guna2HtmlLabel();

                countLabel.Text = items[i].Count.ToString();

                countLabel.Font =
                    new Font("Segoe UI", 10F, FontStyle.Bold);

                countLabel.ForeColor =
                    Color.FromArgb(145, 100, 0);

                countLabel.BackColor = Color.Transparent;
                countLabel.Dock = DockStyle.Fill;

                countLabel.TextAlignment =
                    ContentAlignment.MiddleCenter;

                pill.Controls.Add(countLabel);

                panel.Controls.Add(nameLabel);
                panel.Controls.Add(pill);

                // DIVIDER

                if (i < items.Length - 1)
                {
                    Panel divider = new Panel();

                    divider.BackColor =
                        Color.FromArgb(235, 220, 205);

                    divider.Size = new Size(
                        Math.Max(100, panel.Width - 50),
                        1);

                    divider.Location =
                        new Point(25, y + 72);

                    divider.Anchor =
                        AnchorStyles.Top |
                        AnchorStyles.Left |
                        AnchorStyles.Right;

                    divider.Tag = "StaffRow";

                    panel.Controls.Add(divider);
                }
            }
        }

        // =========================================
        // REFRESH ADMIN DASHBOARD
        // =========================================

        public void RefreshAdminDashboard()
        {
            try
            {
                UpdateDashboardDateTime();

                if (dashboardTimer != null &&
                    !dashboardTimer.Enabled)
                {
                    dashboardTimer.Start();
                }

                SetDefaultValues();

                LoadMonthlyRevenue();
                LoadRoomStatus();
                LoadPropertyRevenue();
                LoadReservationsPerMonth();
                LoadTenantStatus();

                LoadContractsEndingSoon();
                LoadPendingMaintenance();
                LoadRecentPayments();
                LoadStaffAttendance();

                reportsLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to refresh Admin Dashboard.\n\n" +
                    ex.Message,
                    "Refresh Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}