
using _4C_s_Management_System.Classess;
using Guna.Charts.WinForms;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace _4C_s_Management_System
{
    public partial class UserControlDB : UserControl
    {
        public UserControlDB()
        {
            InitializeComponent();

            // DATE AND TIME TIMER
            RM_Timer.Interval = 1000;
            RM_Timer.Tick += RM_Timer_Tick;
        }

        public string branchId = AccessBranch_ByForm.BranchId;

        private void UCDash_NewReservationBtn_Click(object sender, EventArgs e)
        {
            UserNavigation? nav = FindForm() as UserNavigation;
            Control? parent = Parent;

            if (parent == null)
            {
                return;
            }

            UserControlRoomReserv rs = new UserControlRoomReserv();
            MainControlClass.ShowControl(rs, parent);
        }

        private void UserControlDB_Load(object sender, EventArgs e)
        {
            RefreshData();

            // DATE AND TIME COLORS
            RM_Panel_DateTime.FillColor =
                ColorTranslator.FromHtml("#34465A");

            RM_DateTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");

            RM_TimeTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");

            RM_DateLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            RM_TimeLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            // INITIAL DATE AND TIME
            UpdateDateTime();

            // START TIMER
            RM_Timer.Start();
        }

        // DATE AND TIME
        private void RM_Timer_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            RM_DateLbl.Text = DateTime.Now.ToString("MMM dd, yyyy");
            RM_TimeLbl.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        public void RefreshData()
        {
            displayLabels();
            LoadDoughnutChart();
            LoadPropertyOccupancy();
            roomstatusBytypeDGV();
            RcentreservationData();
        }

        public void displayLabels()
        {
            try
            {
                const string query = @"
                SELECT
                    COUNT(CASE WHEN Status = 'Occupied' THEN 1 END) AS Occupied,
                    COUNT(CASE WHEN Status = 'Available' THEN 1 END) AS Available,
                    COUNT(CASE WHEN Status = 'Maintenance' THEN 1 END) AS Maintenance,
                    COUNT(CASE WHEN Status = 'Reserved' THEN 1 END) AS Reserved
                FROM TBLRoom
                WHERE BranchId = @BranchId;
                ";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                if (data.Rows.Count > 0)
                {
                    DataRow row = data.Rows[0];

                    int occupied = Convert.ToInt32(row["Occupied"]);
                    int available = Convert.ToInt32(row["Available"]);
                    int maintenance = Convert.ToInt32(row["Maintenance"]);
                    int reserved = Convert.ToInt32(row["Reserved"]);

                    // TOTAL ROOMS
                    DB_totalRoomLBL.Text = (
                        occupied + available + maintenance + reserved
                    ).ToString();

                    // ROOM STATUS
                    DB_OccupiedRoomLBL.Text = occupied.ToString();
                    DB_availableRoomLBLL.Text = available.ToString();
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

        private void LoadDoughnutChart()
        {
            DB_RoomOcc_donat.Datasets.Clear();

            var dataset = new GunaDoughnutDataset()
            {
                Label = "Room Occupancy"
            };

            const string query = @"
            SELECT
                COUNT(CASE WHEN Status = 'Occupied' THEN 1 END) AS Occupied,
                COUNT(CASE WHEN Status = 'Available' THEN 1 END) AS Available,
                COUNT(CASE WHEN Status = 'Maintenance' THEN 1 END) AS Maintenance,
                COUNT(CASE WHEN Status = 'Reserved' THEN 1 END) AS Reserved
            FROM TBLRoom
            WHERE BranchId = @BranchId;
            ";

            var data = AppDB.Instance.TableData(
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

                occupiedRooms = Convert.ToInt32(row["Occupied"]);
                availableRooms = Convert.ToInt32(row["Available"]);
                maintenanceRooms = Convert.ToInt32(row["Maintenance"]);
                reservedRooms = Convert.ToInt32(row["Reserved"]);

                totalRooms = occupiedRooms + availableRooms +
                             maintenanceRooms + reservedRooms;
            }

            // ADD ROOM COUNTS
            dataset.DataPoints.Add("Occupied", occupiedRooms);
            dataset.DataPoints.Add("Available", availableRooms);
            dataset.DataPoints.Add("Maintenance", maintenanceRooms);
            dataset.DataPoints.Add("Reserved", reservedRooms);

            double occupancyPercentage = totalRooms > 0
                ? (occupiedRooms / (double)totalRooms) * 100 : 0;

            double availablePercentage = totalRooms > 0
                ? (availableRooms / (double)totalRooms) * 100 : 0;

            double maintenancePercentage = totalRooms > 0
                ? (maintenanceRooms / (double)totalRooms) * 100 : 0;

            double reservedPercentage = totalRooms > 0
                ? (reservedRooms / (double)totalRooms) * 100 : 0;

            ROO_TotalRoomLBL.Text = totalRooms.ToString();

            ROO_OccupiedLBL.Text =
                occupiedRooms.ToString() + " (" +
                occupancyPercentage.ToString("F1") + "%)";

            ROO_AvailableLBL.Text =
                availableRooms.ToString() + " (" +
                availablePercentage.ToString("F1") + "%)";

            ROO_maintenanceLBL.Text =
                maintenanceRooms.ToString() + " (" +
                maintenancePercentage.ToString("F1") + "%)";

            ROO_ReservedLBL.Text =
                reservedRooms.ToString() + " (" +
                reservedPercentage.ToString("F1") + "%)";

            ROO_occupancyPerLBL.Text =
                occupancyPercentage.ToString("F1") + "%";

            // CHART COLORS
            dataset.FillColors.Add(Color.FromArgb(54, 162, 235));
            dataset.FillColors.Add(Color.FromArgb(75, 192, 192));
            dataset.FillColors.Add(Color.FromArgb(255, 206, 86));
            dataset.FillColors.Add(Color.FromArgb(153, 102, 255));

            DB_RoomOcc_donat.Datasets.Add(dataset);

            DB_RoomOcc_donat.Update();
        }

        public void LoadPropertyOccupancy()
        {
            try
            {
                const string query = @"
                SELECT
                    RoomType,
                    COUNT(*) AS TotalRooms,
                    SUM(CASE WHEN Status = 'Occupied' THEN 1 ELSE 0 END) AS OccupiedRooms
                FROM TBLRoom
                WHERE BranchId = @BranchId
                GROUP BY RoomType;";

                var data = AppDB.Instance.TableData(
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
                totalRooms = Convert.ToInt32(rows[0]["TotalRooms"]);
                occupiedRooms = Convert.ToInt32(rows[0]["OccupiedRooms"]);
            }

            int percentage = totalRooms > 0
                ? (int)Math.Round(
                    (double)occupiedRooms / totalRooms * 100
                  )
                : 0;

            progress.Value = percentage;
            percentLabel.Text = $"{percentage}%";
            countLabel.Text = $"{occupiedRooms}/{totalRooms} rooms";
        }

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
                    Status ASC;";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                // ADD COLUMNS IF NONE EXIST
                if (Room_statusBy_typeDGV.Columns.Count == 0)
                {
                    Room_statusBy_typeDGV.Columns.Add(
                        "RoomType", "Room Type"
                    );

                    Room_statusBy_typeDGV.Columns.Add(
                        "Status", "Status"
                    );

                    Room_statusBy_typeDGV.Columns.Add(
                        "Count", "Count"
                    );
                }

                // CLEAR EXISTING ROWS
                Room_statusBy_typeDGV.Rows.Clear();

                // POPULATE DATAGRIDVIEW
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
                ORDER BY ReservationDate DESC;";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId }
                );

                // CONFIGURE DATAGRIDVIEW
                Recenr_reservationDGV.Visible = true;
                Recenr_reservationDGV.AutoGenerateColumns = true;
                Recenr_reservationDGV.DataSource = null;

                // DISPLAY RETRIEVED RECORDS
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

        private void guna2HtmlLabel24_Click(object sender, EventArgs e)
        {

        }
    }
}
