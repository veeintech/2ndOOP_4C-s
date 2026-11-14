using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControlReports : UserControl
    {
        private static readonly Color Maroon =
            Color.FromArgb(128, 0, 32);

        private static readonly Color DarkMaroon =
            Color.FromArgb(92, 16, 35);

        private static readonly Color Mustard =
            Color.FromArgb(212, 167, 44);

        private const string ZeroText = "0 (00.00%)";

        // Summary card labels (names from the designer)
        private const string LblTotalRevenue = "guna2HtmlLabel5";
        private const string LblTotalStaff = "guna2HtmlLabel23";
        private const string LblOccupancyRate = "guna2HtmlLabel3";
        private const string LblMaintenance = "guna2HtmlLabel7";

        // =========================================
        // OCCUPANCY LEGEND
        // =========================================

        private static readonly Dictionary<string, string> OccupancyLegend =
            new Dictionary<string, string>
            {
                { "Occupied", "guna2HtmlLabel74" },
                { "Available", "guna2HtmlLabel72" },
                { "Reserved", "guna2HtmlLabel70" },
                { "Under Maintenance", "guna2HtmlLabel68" },
                { "Maintenance", "guna2HtmlLabel68" }
            };

        // =========================================
        // MAINTENANCE LEGEND
        // =========================================

        private static readonly Dictionary<string, string> MaintenanceLegend =
            new Dictionary<string, string>
            {
                { "Completed", "guna2HtmlLabel58" },
                { "Ongoing", "guna2HtmlLabel56" },
                { "Pending", "guna2HtmlLabel54" },
                { "Cancelled", "guna2HtmlLabel52" }
            };

        // =========================================
        // RESERVATION LEGEND
        // =========================================

        private static readonly Dictionary<string, string> ReservationLegend =
            new Dictionary<string, string>
            {
                { "Confirmed", "guna2HtmlLabel25" },
                { "Pending", "guna2HtmlLabel21" },
                { "Check-In", "guna2HtmlLabel19" },
                { "Check-Out", "guna2HtmlLabel27" },
                { "Cancelled", "guna2HtmlLabel29" }
            };

        // =========================================
        // TENANT LEGEND
        // =========================================

        private static readonly Dictionary<string, string> TenantLegend =
            new Dictionary<string, string>
            {
                { "Active", "ROO_OccupiedLBL" },
                { "Inactive", "guna2HtmlLabel13" },
                { "Expired", "guna2HtmlLabel15" }
            };

        public UserControlReports()
        {
            InitializeComponent();
        }

        // =========================================
        // FORM LOAD
        // =========================================

        private void UserControlReports_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultsToZero();
                LoadAllReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load reports.\n\n" + ex.Message,
                    "Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // LOAD ALL REPORTS
        // =========================================

        private void LoadAllReports()
        {
            LoadSummaryCards();

            // Financial Reports
            LoadFinancialReports();

            // Occupancy / Rooms Reports
            LoadOccupancyReports();

            // Maintenance Reports
            LoadMaintenanceReports();

            // Reservation Reports
            LoadReservationReports();

            // Tenant Reports
            LoadTenantReports();

            // Revenue by Property Type
            LoadPropertyProgressBars();

            // Payroll data is not yet available.
            SetGunaChart5ToZero();

            // Attendance data is not yet available.
            SetGunaChart7ToZero();
            SetGunaChart8ToZero();
        }

        // =========================================
        // DEFAULT VALUES
        // =========================================

        private void SetDefaultsToZero()
        {
            SetText(LblTotalRevenue, "0.00");
            SetText(LblTotalStaff, "0");
            SetText(LblOccupancyRate, "0%");
            SetText(LblMaintenance, "0");

            foreach (var name in OccupancyLegend.Values.Distinct())
                SetText(name, ZeroText);

            foreach (var name in MaintenanceLegend.Values.Distinct())
                SetText(name, ZeroText);

            foreach (var name in ReservationLegend.Values.Distinct())
                SetText(name, ZeroText);

            foreach (var name in TenantLegend.Values.Distinct())
                SetText(name, ZeroText);

            // Payroll labels
            guna2HtmlLabel82.Text = ZeroText;
            guna2HtmlLabel80.Text = ZeroText;
            guna2HtmlLabel78.Text = ZeroText;
            guna2HtmlLabel76.Text = ZeroText;

            // Attendance labels
            guna2HtmlLabel66.Text = ZeroText;
            guna2HtmlLabel64.Text = ZeroText;
            guna2HtmlLabel62.Text = ZeroText;
            guna2HtmlLabel60.Text = ZeroText;

            // Apartment revenue
            guna2ProgressBar3.Value = 0;
            guna2HtmlLabel33.Text = "0.00";
            guna2HtmlLabel32.Text = "0.00%";

            // Hotel revenue
            guna2ProgressBar5.Value = 0;
            guna2HtmlLabel35.Text = "0.00";
            guna2HtmlLabel37.Text = "0.00%";

            // Transient revenue
            guna2ProgressBar6.Value = 0;
            guna2HtmlLabel34.Text = "0.00";
            guna2HtmlLabel36.Text = "0.00%";

            ResetPlaceholderLabels(this);
        }

        private void ResetPlaceholderLabels(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                bool isPlaceholder =
                    (child is Guna2HtmlLabel || child is Label) &&
                    child.Text != null &&
                    child.Text.Contains("_") &&
                    child.Text.Contains("%");

                if (isPlaceholder)
                    child.Text = ZeroText;

                if (child.HasChildren)
                    ResetPlaceholderLabels(child);
            }
        }

        // =========================================
        // SAFE HELPERS
        // =========================================

        private static double SafeDouble(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return double.TryParse(
                Convert.ToString(value),
                out double result)
                ? result
                : 0;
        }

        private static string Normalize(string value)
        {
            return (value ?? "")
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("_", "")
                .ToLowerInvariant();
        }

        private Control FindControlByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            Control[] found = Controls.Find(name, true);

            return found.Length > 0 ? found[0] : null;
        }

        private void SetText(string controlName, string text)
        {
            Control control = FindControlByName(controlName);

            if (control != null)
                control.Text = text;
        }

        // =========================================
        // BRANCH FILTER
        // =========================================

        private string BranchFilter(string column)
        {
            return $" AND {column} = @BranchId";
        }

        private object BranchParameters()
        {
            return new
            {
                BranchId = AccessBranch_ByForm.BranchId
            };
        }

        private DataTable GetReportData(string query)
        {
            return AppDB.Instance.TableData(
                query,
                BranchParameters());
        }

        // =========================================
        // DOUGHNUT CHART
        // =========================================

        private void LoadDoughnutChart(
            GunaChart chart,
            string query,
            Dictionary<string, string> legendMap = null,
            GunaDoughnutDataset existingDataset = null)
        {
            chart.Datasets.Clear();

            if (legendMap != null)
            {
                foreach (string controlName in legendMap.Values.Distinct())
                    SetText(controlName, ZeroText);
            }

            DataTable data = GetReportData(query);

            GunaDoughnutDataset dataset =
                existingDataset ?? new GunaDoughnutDataset();

            dataset.DataPoints.Clear();
            dataset.FillColors.Clear();

            dataset.Label = "Report Data";
            dataset.TargetChart = chart;

            if (data.Rows.Count == 0)
            {
                if (existingDataset != null)
                    chart.Datasets.Add(dataset);

                chart.Update();
                return;
            }

            Color[] colors =
            {
                Maroon,
                DarkMaroon,
                Mustard,
                Color.IndianRed,
                Color.Brown,
                Color.Gray
            };

            double grandTotal = 0;

            foreach (DataRow row in data.Rows)
            {
                grandTotal += SafeDouble(row["Total"]);
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                string label =
                    Convert.ToString(data.Rows[i]["Label"])
                    ?? "Unknown";

                double value =
                    SafeDouble(data.Rows[i]["Total"]);

                dataset.DataPoints.Add(
                    new LPoint
                    {
                        Label = label,
                        Y = value
                    });

                dataset.FillColors.Add(
                    colors[i % colors.Length]);

                if (legendMap != null)
                {
                    string key = legendMap.Keys.FirstOrDefault(
                        k => Normalize(k) == Normalize(label));

                    if (key != null)
                    {
                        double percentage = grandTotal > 0
                            ? value / grandTotal * 100
                            : 0;

                        SetText(
                            legendMap[key],
                            $"{value:0} ({percentage:00.00}%)");
                    }
                }
            }

            if (dataset.DataPoints.Count > 0)
                chart.Datasets.Add(dataset);

            chart.Update();
        }

        // =========================================
        // BAR CHART
        // =========================================

        private void LoadBarChart(
            GunaChart chart,
            string query,
            string datasetLabel,
            GunaBarDataset existingDataset = null)
        {
            chart.Datasets.Clear();

            DataTable data = GetReportData(query);

            GunaBarDataset dataset =
                existingDataset ?? new GunaBarDataset();

            dataset.DataPoints.Clear();
            dataset.FillColors.Clear();

            dataset.Label = datasetLabel;
            dataset.TargetChart = chart;

            if (data.Rows.Count == 0)
            {
                if (existingDataset != null)
                    chart.Datasets.Add(dataset);

                chart.Update();
                return;
            }

            foreach (DataRow row in data.Rows)
            {
                string label =
                    Convert.ToString(row["Label"])
                    ?? "Unknown";

                double value =
                    SafeDouble(row["Total"]);

                dataset.DataPoints.Add(
                    new LPoint
                    {
                        Label = label,
                        Y = value
                    });

                dataset.FillColors.Add(Maroon);
            }

            if (dataset.DataPoints.Count > 0)
                chart.Datasets.Add(dataset);

            chart.Update();
        }

        // =========================================
        // 0. SUMMARY CARDS
        // =========================================

        private void LoadSummaryCards()
        {
            // Total Revenue
            DataTable rev = GetReportData(@"
                SELECT SUM(ISNULL(AmountPaid, 0)) AS Total
                FROM PaymentTbl
                WHERE 1 = 1"
                + BranchFilter("BranchID") + ";");

            double totalRevenue =
                rev.Rows.Count > 0
                ? SafeDouble(rev.Rows[0]["Total"])
                : 0;

            SetText(
                LblTotalRevenue,
                totalRevenue.ToString("N2"));

            // Total Staff
            DataTable staff = GetReportData(@"
                SELECT COUNT(*) AS Total
                FROM UserTbl
                WHERE Role <> 'MasterAdmin'"
                + BranchFilter("BranchId") + ";");

            double totalStaff =
                staff.Rows.Count > 0
                ? SafeDouble(staff.Rows[0]["Total"])
                : 0;

            SetText(LblTotalStaff, totalStaff.ToString("0"));

            // Overall Occupancy Rate
            DataTable occ = GetReportData(@"
                SELECT
                    CAST(
                        100.0 *
                        SUM(
                            CASE WHEN Status = 'Occupied'
                            THEN 1 ELSE 0 END
                        )
                        /
                        NULLIF(
                            SUM(
                                CASE WHEN Status <> 'Maintenance'
                                THEN 1 ELSE 0 END
                            ),
                            0
                        )
                    AS DECIMAL(10,2)) AS Total
                FROM TBLRoom
                WHERE 1 = 1"
                + BranchFilter("BranchId") + ";");

            double rate =
                occ.Rows.Count > 0
                ? SafeDouble(occ.Rows[0]["Total"])
                : 0;

            SetText(
                LblOccupancyRate,
                rate.ToString("0.##") + "%");

            // Pending and ongoing maintenance
            DataTable maint = GetReportData(@"
                SELECT COUNT(*) AS Total
                FROM MaintenanceTbl
                WHERE Status IN ('Pending', 'Ongoing')"
                + BranchFilter("BranchID") + ";");

            double maintCount =
                maint.Rows.Count > 0
                ? SafeDouble(maint.Rows[0]["Total"])
                : 0;

            SetText(
                LblMaintenance,
                maintCount.ToString("0"));
        }

        // =========================================
        // 1. FINANCIAL REPORTS
        // CHART 4 = MONTHLY TOTAL REVENUE
        // gunaBarDataset1
        // =========================================

        private void LoadFinancialReports()
        {
            string revenueQuery = @"
                SELECT
                    FORMAT(PaymentDate, 'MMM yyyy') AS Label,
                    SUM(ISNULL(AmountPaid, 0)) AS Total,
                    YEAR(PaymentDate) AS SortYear,
                    MONTH(PaymentDate) AS SortMonth
                FROM PaymentTbl
                WHERE PaymentDate IS NOT NULL"
                + BranchFilter("BranchID") + @"
                GROUP BY
                    YEAR(PaymentDate),
                    MONTH(PaymentDate),
                    FORMAT(PaymentDate, 'MMM yyyy')
                ORDER BY SortYear, SortMonth;";

            LoadBarChart(
                gunaChart4,
                revenueQuery,
                "Monthly Total Revenue",
                gunaBarDataset1);
        }

        // =========================================
        // GUNACHART5 - PAYROLL BREAKDOWN
        // gunaDoughnutDataset1
        // =========================================

        private void SetGunaChart5ToZero()
        {
            gunaDoughnutDataset1.DataPoints.Clear();
            gunaDoughnutDataset1.FillColors.Clear();

            gunaDoughnutDataset1.Label = "Payroll Breakdown";
            gunaDoughnutDataset1.TargetChart = gunaChart5;

            gunaChart5.Datasets.Clear();
            gunaChart5.Datasets.Add(gunaDoughnutDataset1);

            string[] labels =
            {
                "Basic Salary",
                "Overtime Pay",
                "Deductions",
                "Net Salary"
            };

            Color[] colors =
            {
                Maroon,
                Mustard,
                Color.IndianRed,
                Color.SeaGreen
            };

            for (int i = 0; i < labels.Length; i++)
            {
                gunaDoughnutDataset1.DataPoints.Add(
                    new LPoint
                    {
                        Label = labels[i],
                        Y = 0
                    });

                gunaDoughnutDataset1.FillColors.Add(colors[i]);
            }

            gunaChart5.Update();

            guna2HtmlLabel82.Text = ZeroText;
            guna2HtmlLabel80.Text = ZeroText;
            guna2HtmlLabel78.Text = ZeroText;
            guna2HtmlLabel76.Text = ZeroText;
        }

        // =========================================
        // GUNACHART7 - ATTENDANCE ZERO VALUES
        // gunaDoughnutDataset3
        // =========================================

        private void SetGunaChart7ToZero()
        {
            gunaChart7.Datasets.Clear();

            gunaDoughnutDataset3.DataPoints.Clear();
            gunaDoughnutDataset3.FillColors.Clear();

            gunaDoughnutDataset3.TargetChart = gunaChart7;
            gunaDoughnutDataset3.Label = "Attendance";

            string[] labels =
            {
                "Present",
                "Absent",
                "Late",
                "On Leave"
            };

            Color[] colors =
            {
                Maroon,
                Color.IndianRed,
                Color.Brown,
                Color.FromArgb(205, 92, 92)
            };

            for (int i = 0; i < labels.Length; i++)
            {
                gunaDoughnutDataset3.DataPoints.Add(
                    new LPoint
                    {
                        Label = labels[i],
                        Y = 0
                    });

                gunaDoughnutDataset3.FillColors.Add(colors[i]);
            }

            gunaChart7.Datasets.Add(gunaDoughnutDataset3);
            gunaChart7.Update();

            guna2HtmlLabel66.Text = ZeroText;
            guna2HtmlLabel64.Text = ZeroText;
            guna2HtmlLabel62.Text = ZeroText;
            guna2HtmlLabel60.Text = ZeroText;
        }

        // =========================================
        // GUNACHART8 - WORKING HOURS ZERO VALUES
        // gunaBarDataset2
        // =========================================

        private void SetGunaChart8ToZero()
        {
            gunaChart8.Datasets.Clear();

            gunaBarDataset2.DataPoints.Clear();
            gunaBarDataset2.FillColors.Clear();

            gunaBarDataset2.TargetChart = gunaChart8;
            gunaBarDataset2.Label = "Working Hours";

            string[] categories =
            {
                "Regular",
                "Overtime",
                "Total Hours per Staff"
            };

            foreach (string category in categories)
            {
                gunaBarDataset2.DataPoints.Add(
                    new LPoint
                    {
                        Label = category,
                        Y = 0
                    });

                gunaBarDataset2.FillColors.Add(Maroon);
            }

            gunaChart8.Datasets.Add(gunaBarDataset2);
            gunaChart8.Update();
        }

        // =========================================
        // 2. OCCUPANCY / ROOMS REPORTS
        // CHART 6 = ROOM OCCUPANCY STATUS
        // gunaDoughnutDataset2
        //
        // CHART 10 = OCCUPANCY RATE PER ROOM TYPE
        // gunaBarDataset6
        // =========================================

        private void LoadOccupancyReports()
        {
            // Room Occupancy Status
            string occupancyQuery = @"
                SELECT
                    CASE
                        WHEN Status = 'Maintenance'
                        THEN 'Under Maintenance'
                        ELSE ISNULL(Status, 'Unknown')
                    END AS Label,
                    COUNT(*) AS Total
                FROM TBLRoom
                WHERE 1 = 1"
                + BranchFilter("BranchId") + @"
                GROUP BY
                    CASE
                        WHEN Status = 'Maintenance'
                        THEN 'Under Maintenance'
                        ELSE ISNULL(Status, 'Unknown')
                    END
                ORDER BY Label;";

            LoadDoughnutChart(
                gunaChart6,
                occupancyQuery,
                OccupancyLegend,
                gunaDoughnutDataset2);

            // Occupancy Rate Per Room Type
            string occupancyRateQuery = @"
                SELECT
                    RoomType AS Label,
                    CAST(
                        CASE
                            WHEN SUM(
                                CASE
                                    WHEN Status <> 'Maintenance'
                                    THEN 1 ELSE 0
                                END
                            ) = 0
                            THEN 0
                            ELSE
                                SUM(
                                    CASE
                                        WHEN Status = 'Occupied'
                                        THEN 1 ELSE 0
                                    END
                                ) * 100.0 /
                                SUM(
                                    CASE
                                        WHEN Status <> 'Maintenance'
                                        THEN 1 ELSE 0
                                    END
                                )
                        END
                    AS DECIMAL(10,2)) AS Total
                FROM TBLRoom
                WHERE 1 = 1"
                + BranchFilter("BranchId") + @"
                GROUP BY RoomType
                ORDER BY RoomType;";

            LoadBarChart(
                gunaChart10,
                occupancyRateQuery,
                "Occupancy Rate (%)",
                gunaBarDataset6);
        }

        // =========================================
        // PROPERTY TYPE REVENUE PROGRESS BARS
        // =========================================

        private void LoadPropertyProgressBars()
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
                WHERE P.ReservationID IS NOT NULL"
                + BranchFilter("P.BranchID") + @"
                GROUP BY R.RoomType;";

            DataTable data = GetReportData(query);

            double grandTotal = 0;

            foreach (DataRow row in data.Rows)
            {
                grandTotal += SafeDouble(row["TotalRevenue"]);
            }

            UpdatePropertyRevenue(
                data, "Apartment",
                guna2ProgressBar3,
                guna2HtmlLabel33,
                guna2HtmlLabel32,
                grandTotal);

            UpdatePropertyRevenue(
                data, "Hotel",
                guna2ProgressBar5,
                guna2HtmlLabel35,
                guna2HtmlLabel37,
                grandTotal);

            UpdatePropertyRevenue(
                data, "Transient",
                guna2ProgressBar6,
                guna2HtmlLabel34,
                guna2HtmlLabel36,
                grandTotal);
        }

        private void UpdatePropertyRevenue(
            DataTable data,
            string roomType,
            Guna2ProgressBar progressBar,
            Guna2HtmlLabel revenueLabel,
            Guna2HtmlLabel percentageLabel,
            double grandTotal)
        {
            DataRow[] rows = data.Select(
                $"RoomType = '{roomType.Replace("'", "''")}'");

            double revenue = 0;

            if (rows.Length > 0)
                revenue = SafeDouble(rows[0]["TotalRevenue"]);

            double percentage = grandTotal > 0
                ? revenue / grandTotal * 100
                : 0;

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = Math.Max(
                0,
                Math.Min(100, (int)Math.Round(percentage)));

            revenueLabel.Text = revenue.ToString("N2");
            percentageLabel.Text = $"{percentage:0.00}%";
        }

        // =========================================
        // 3. MAINTENANCE REPORTS
        // CHART 9 = MAINTENANCE STATUS
        // gunaDoughnutDataset6
        //
        // CHART 3 = TOTAL COMPLETED MAINTENANCE
        // PER MONTH
        // gunaBarDataset3
        // =========================================

        private void LoadMaintenanceReports()
        {
            // Maintenance Status
            string maintenanceQuery = @"
                SELECT
                    ISNULL(Status, 'Unknown') AS Label,
                    COUNT(*) AS Total
                FROM MaintenanceTbl
                WHERE 1 = 1"
                + BranchFilter("BranchID") + @"
                GROUP BY Status
                ORDER BY Status;";

            LoadDoughnutChart(
                gunaChart9,
                maintenanceQuery,
                MaintenanceLegend,
                gunaDoughnutDataset6);

            // Total Completed Maintenance per Month
            string monthlyMaintenanceQuery = @"
                SELECT
                    FORMAT(DateCompleted, 'MMM yyyy') AS Label,
                    COUNT(*) AS Total,
                    YEAR(DateCompleted) AS SortYear,
                    MONTH(DateCompleted) AS SortMonth
                FROM MaintenanceTbl
                WHERE DateCompleted IS NOT NULL
                    AND Status = 'Completed'"
                + BranchFilter("BranchID") + @"
                GROUP BY
                    YEAR(DateCompleted),
                    MONTH(DateCompleted),
                    FORMAT(DateCompleted, 'MMM yyyy')
                ORDER BY SortYear, SortMonth;";

            // gunaChart3 CONNECTED TO gunaBarDataset3
            LoadBarChart(
                gunaChart3,
                monthlyMaintenanceQuery,
                "Total Maintenance Done per Month",
                gunaBarDataset3);
        }

        // =========================================
        // 4. RESERVATION REPORTS
        // =========================================

        private void LoadReservationReports()
        {
            string reservationStatusQuery = @"
                SELECT
                    ISNULL(ReservationStatus, 'Unknown') AS Label,
                    COUNT(*) AS Total
                FROM ReservationTbl
                WHERE 1 = 1"
                + BranchFilter("branchId") + @"
                GROUP BY ReservationStatus
                ORDER BY ReservationStatus;";

            LoadDoughnutChart(
                gunaChart1,
                reservationStatusQuery,
                ReservationLegend,
                gunaDoughnutDataset5);

            string monthlyReservationQuery = @"
                SELECT
                    FORMAT(ReservationDate, 'MMM yyyy') AS Label,
                    COUNT(*) AS Total,
                    YEAR(ReservationDate) AS SortYear,
                    MONTH(ReservationDate) AS SortMonth
                FROM ReservationTbl
                WHERE 1 = 1"
                + BranchFilter("branchId") + @"
                GROUP BY
                    YEAR(ReservationDate),
                    MONTH(ReservationDate),
                    FORMAT(ReservationDate, 'MMM yyyy')
                ORDER BY SortYear, SortMonth;";

            LoadBarChart(
                gunaChart2,
                monthlyReservationQuery,
                "Reservations",
                gunaBarDataset5);
        }

        // =========================================
        // 5. TENANT REPORTS
        // =========================================

        private void LoadTenantReports()
        {
            string tenantStatusQuery = @"
                SELECT
                    CASE
                        WHEN AgreementEndDate <
                            CAST(GETDATE() AS DATE)
                        THEN 'Expired'
                        ELSE ISNULL(Status, 'Unknown')
                    END AS Label,
                    COUNT(*) AS Total
                FROM TenantTbl
                WHERE 1 = 1"
                + BranchFilter("BranchId") + @"
                GROUP BY
                    CASE
                        WHEN AgreementEndDate <
                            CAST(GETDATE() AS DATE)
                        THEN 'Expired'
                        ELSE ISNULL(Status, 'Unknown')
                    END
                ORDER BY Label;";

            LoadDoughnutChart(
                gunaChart11,
                tenantStatusQuery,
                TenantLegend,
                gunaDoughnutDataset4);

            string rentalIncomeQuery = @"
                SELECT
                    FORMAT(PaymentDate, 'MMM yyyy') AS Label,
                    SUM(ISNULL(AmountPaid, 0)) AS Total,
                    YEAR(PaymentDate) AS SortYear,
                    MONTH(PaymentDate) AS SortMonth
                FROM PaymentTbl
                WHERE TenantID IS NOT NULL
                    AND PaymentDate IS NOT NULL"
                + BranchFilter("BranchID") + @"
                GROUP BY
                    YEAR(PaymentDate),
                    MONTH(PaymentDate),
                    FORMAT(PaymentDate, 'MMM yyyy')
                ORDER BY SortYear, SortMonth;";

            LoadBarChart(
                gunaChart12,
                rentalIncomeQuery,
                "Rental Income",
                gunaBarDataset4);
        }

        // =========================================
        // REFRESH REPORTS
        // =========================================

        public void RefreshReports()
        {
            try
            {
                SetDefaultsToZero();
                LoadAllReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to refresh reports.\n\n" + ex.Message,
                    "Reports Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================
        // PRINT REPORT
        // Connect PrintIcon_Click to the print icon's Click event
        // (Properties > Events > Click)
        // =========================================

        private class ReportSection
        {
            public string Title;
            public string Col1;
            public string Col2;
            public List<string[]> Rows = new List<string[]>();
        }

        private List<ReportSection> _printSections;
        private int _printSection;
        private int _printRow;
        private int _printPage;

        private string GetText(string controlName)
        {
            Control c = FindControlByName(controlName);
            return c != null ? c.Text : "-";
        }

        private void AddValueSection(
            List<ReportSection> list,
            string title,
            string col1,
            string col2,
            System.Collections.IEnumerable points,
            Func<double, string> format)
        {
            ReportSection s = new ReportSection
            {
                Title = title,
                Col1 = col1,
                Col2 = col2
            };

            foreach (LPoint p in points)
                s.Rows.Add(new[] { p.Label, format(p.Y) });

            if (s.Rows.Count == 0)
                s.Rows.Add(new[] { "No data", "-" });

            list.Add(s);
        }

        private void AddStatusSection(
            List<ReportSection> list,
            string title,
            string col1,
            System.Collections.IEnumerable points)
        {
            ReportSection s = new ReportSection
            {
                Title = title,
                Col1 = col1,
                Col2 = "Count (Percentage)"
            };

            double total = 0;
            foreach (LPoint p in points)
                total += p.Y;

            foreach (LPoint p in points)
            {
                double pct = total > 0 ? p.Y / total * 100 : 0;
                s.Rows.Add(new[] { p.Label, $"{p.Y:0} ({pct:0.00}%)" });
            }

            if (s.Rows.Count == 0)
                s.Rows.Add(new[] { "No data", "-" });

            list.Add(s);
        }

        private List<ReportSection> BuildReportSections()
        {
            var list = new List<ReportSection>();

            // Summary
            var summary = new ReportSection
            {
                Title = "Summary",
                Col1 = "Metric",
                Col2 = "Value"
            };
            summary.Rows.Add(new[] { "Total Revenue", GetText(LblTotalRevenue) });
            summary.Rows.Add(new[] { "Total Staff", GetText(LblTotalStaff) });
            summary.Rows.Add(new[] { "Occupancy Rate", GetText(LblOccupancyRate) });
            summary.Rows.Add(new[] { "Pending / Ongoing Maintenance", GetText(LblMaintenance) });
            list.Add(summary);

            // Financial
            AddValueSection(list, "Monthly Total Revenue",
                "Month", "Revenue",
                gunaBarDataset1.DataPoints,
                v => v.ToString("N2"));

            var property = new ReportSection
            {
                Title = "Revenue by Property Type",
                Col1 = "Property Type",
                Col2 = "Revenue (Share)"
            };
            property.Rows.Add(new[] { "Apartment", guna2HtmlLabel33.Text + " (" + guna2HtmlLabel32.Text + ")" });
            property.Rows.Add(new[] { "Hotel", guna2HtmlLabel35.Text + " (" + guna2HtmlLabel37.Text + ")" });
            property.Rows.Add(new[] { "Transient", guna2HtmlLabel34.Text + " (" + guna2HtmlLabel36.Text + ")" });
            list.Add(property);

            // Rooms
            AddStatusSection(list, "Room Occupancy Status",
                "Status", gunaDoughnutDataset2.DataPoints);

            AddValueSection(list, "Occupancy Rate per Room Type",
                "Room Type", "Occupancy Rate",
                gunaBarDataset6.DataPoints,
                v => v.ToString("0.##") + "%");

            // Maintenance
            AddStatusSection(list, "Maintenance Status",
                "Status", gunaDoughnutDataset6.DataPoints);

            AddValueSection(list, "Completed Maintenance per Month",
                "Month", "Completed",
                gunaBarDataset3.DataPoints,
                v => v.ToString("0"));

            // Reservations
            AddStatusSection(list, "Reservation Status",
                "Status", gunaDoughnutDataset5.DataPoints);

            AddValueSection(list, "Reservations per Month",
                "Month", "Reservations",
                gunaBarDataset5.DataPoints,
                v => v.ToString("0"));

            // Tenants
            AddStatusSection(list, "Tenant Status",
                "Status", gunaDoughnutDataset4.DataPoints);

            AddValueSection(list, "Rental Income per Month",
                "Month", "Income",
                gunaBarDataset4.DataPoints,
                v => v.ToString("N2"));

            return list;
        }

        private void PrintIcon_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.DocumentName = "4C's Management System - Reports";
                pd.DefaultPageSettings.Margins = new Margins(60, 60, 60, 60);

                pd.BeginPrint += (s, ev) =>
                {
                    _printSections = BuildReportSections();
                    _printSection = 0;
                    _printRow = 0;
                    _printPage = 0;
                };

                pd.PrintPage += ReportPrintPage;

                using (PrintPreviewDialog preview = new PrintPreviewDialog())
                {
                    preview.Document = pd;
                    preview.WindowState = FormWindowState.Maximized;
                    preview.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to print the report.\n\n" + ex.Message,
                    "Print Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ReportPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle m = e.MarginBounds;

            e.HasMorePages = false;
            _printPage++;

            const float rowH = 24f;
            float bottom = m.Bottom - 25f;

            using (Font titleFont = new Font("Georgia", 22, FontStyle.Bold))
            using (Font smallTitleFont = new Font("Georgia", 12, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 9))
            using (Font secFont = new Font("Georgia", 13, FontStyle.Bold))
            using (Font headFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Font rowFont = new Font("Segoe UI", 10))
            using (Brush maroonBrush = new SolidBrush(Maroon))
            using (Brush grayBrush = new SolidBrush(Color.Gray))
            using (Brush blackBrush = new SolidBrush(Color.Black))
            using (Brush whiteBrush = new SolidBrush(Color.White))
            using (Brush stripeBrush = new SolidBrush(Color.FromArgb(250, 244, 236)))
            using (Pen mustardPen = new Pen(Mustard, 2f))
            using (Pen linePen = new Pen(Color.LightGray))
            {
                float y = m.Top;

                // ---------- PAGE HEADER ----------
                if (_printPage == 1)
                {
                    g.DrawString("Reports", titleFont, maroonBrush, m.Left, y);
                    y += 40;

                    g.DrawString(
                        "4C's Management System",
                        subFont, blackBrush, m.Left, y);
                    y += 16;

                    g.DrawString(
                        "Generated: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt")
                        + "    |    Branch ID: " + AccessBranch_ByForm.BranchId,
                        subFont, grayBrush, m.Left, y);
                    y += 22;
                }
                else
                {
                    g.DrawString(
                        "4C's Management System - Reports",
                        smallTitleFont, maroonBrush, m.Left, y);
                    y += 24;
                }

                g.DrawLine(mustardPen, m.Left, y, m.Right, y);
                y += 16;

                float yStart = y;

                // ---------- SECTIONS ----------
                while (_printSection < _printSections.Count)
                {
                    ReportSection s = _printSections[_printSection];

                    // Keep title + header + at least one row together
                    float needed = 28 + rowH + rowH;

                    if (y + needed > bottom && y > yStart)
                    {
                        e.HasMorePages = true;
                        break;
                    }

                    string title = _printRow == 0
                        ? s.Title
                        : s.Title + " (continued)";

                    g.DrawString(title, secFont, maroonBrush, m.Left, y);
                    y += 28;

                    // Table header
                    g.FillRectangle(maroonBrush, m.Left, y, m.Width, rowH);
                    g.DrawString(s.Col1, headFont, whiteBrush, m.Left + 8, y + 4);

                    StringFormat right = new StringFormat { Alignment = StringAlignment.Far };
                    g.DrawString(
                        s.Col2, headFont, whiteBrush,
                        new RectangleF(m.Left, y + 4, m.Width - 8, rowH),
                        right);
                    y += rowH;

                    // Rows
                    while (_printRow < s.Rows.Count)
                    {
                        if (y + rowH > bottom)
                            break;

                        if (_printRow % 2 == 1)
                            g.FillRectangle(stripeBrush, m.Left, y, m.Width, rowH);

                        g.DrawString(
                            s.Rows[_printRow][0], rowFont, blackBrush,
                            m.Left + 8, y + 4);

                        g.DrawString(
                            s.Rows[_printRow][1], rowFont, blackBrush,
                            new RectangleF(m.Left, y + 4, m.Width - 8, rowH),
                            right);

                        g.DrawLine(linePen, m.Left, y + rowH, m.Right, y + rowH);

                        y += rowH;
                        _printRow++;
                    }

                    right.Dispose();

                    if (_printRow < s.Rows.Count)
                    {
                        e.HasMorePages = true;
                        break;
                    }

                    _printSection++;
                    _printRow = 0;
                    y += 18;
                }

                // ---------- FOOTER ----------
                using (StringFormat center = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString(
                        "Page " + _printPage,
                        subFont, grayBrush,
                        new RectangleF(m.Left, m.Bottom - 10, m.Width, 20),
                        center);
                }
            }
        }

        private void guna2TextBox1_TextChanged(
            object sender,
            EventArgs e)
        {
        }
    }
}