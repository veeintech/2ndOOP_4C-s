
using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Tenants : UserControl
    {
        // ==========================================
        // VARIABLES
        // ==========================================

        private string tenantBranchId = "";

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public UserControl_Admin_Tenants()
        {
            InitializeComponent();

            this.Load += UserControlTenants_Load;

            guna2Button2.Click -= guna2Button2_Click;
            guna2Button2.Click += guna2Button2_Click;
        }

        // ==========================================
        // LOAD USERCONTROL
        // ==========================================

        private void UserControlTenants_Load(object sender, EventArgs e)
        {
            if (TM_tenantstatusCB.Items.Count > 0)
                TM_tenantstatusCB.SelectedIndex = 0;

            ClearTenantDetails();

            ApplyTenantFilters();
            LoadRoomsdisplay();
        }

        // ==========================================
        // LOAD TENANT DATA
        // ==========================================

        public void LoadRoomData()
        {
            ApplyTenantFilters();
            LoadRoomsdisplay();
        }

        // ==========================================
        // DISPLAY SELECTED TENANT
        // ==========================================

        public void loaddisplay()
        {
            try
            {
                if (TM_TenantDGV.CurrentRow == null ||
                    TM_TenantDGV.CurrentRow.IsNewRow)
                    return;

                DataGridViewRow row = TM_TenantDGV.CurrentRow;

                TM_tenantidtxt.Text =
                    Convert.ToString(row.Cells["TenantId"].Value) ?? "";

                TM_fullnametxt.Text =
                    Convert.ToString(row.Cells["FullName"].Value) ?? "";

                TM_contacttxt.Text =
                    Convert.ToString(row.Cells["ContactNumber"].Value) ?? "";

                TM_AdressTxt.Text =
                    Convert.ToString(row.Cells["Address"].Value) ?? "";

                if (DateTime.TryParse(
                    Convert.ToString(row.Cells["AgreementStartDate"].Value),
                    out DateTime startDate))
                {
                    TM_agreementstartTXT.Value = startDate;
                }

                if (DateTime.TryParse(
                    Convert.ToString(row.Cells["AgreementEndDate"].Value),
                    out DateTime endDate))
                {
                    TM_agreementendtxt.Value = endDate;
                }

                TM_monthlyrenttxt.Text =
                    Convert.ToString(row.Cells["MonthlyRent"].Value) ?? "";

                TM_statustxt.Text =
                    Convert.ToString(row.Cells["Status"].Value) ?? "";

                tenantBranchId =
                    Convert.ToString(row.Cells["BranchId"].Value) ?? "";

                TM_branchidtxt.Text = tenantBranchId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying tenant details:\n" + ex.Message,
                    "Display Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DATAGRIDVIEW CELL CLICK
        // ==========================================

        private void tenantDGV_clickcontent(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                TM_TenantDGV.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            TM_TenantDGV.ClearSelection();

            row.Selected = true;

            TM_TenantDGV.CurrentCell =
                row.Cells["TenantId"];

            loaddisplay();
        }

        // ==========================================
        // STATUS FILTER
        // ==========================================

        public void statuscombobox()
        {
            ApplyTenantFilters();
        }

        private void TM_tenantselectedindexchanged(
            object sender,
            EventArgs e)
        {
            ApplyTenantFilters();
        }

        // ==========================================
        // SEARCH TENANT
        // ==========================================

        private void TM_searchtxtTextChanged(
            object sender,
            EventArgs e)
        {
            ApplyTenantFilters();
        }

        // ==========================================
        // ADD TENANT
        // ==========================================

        private void TM_AddCusBtn_Click(object sender, EventArgs e)
        {
            string branchId =
                TM_branchidtxt.Text.Trim();

            string fullname =
                TM_fullnametxt.Text.Trim();

            string contactnumber =
                TM_contacttxt.Text.Trim();

            string address =
                TM_AdressTxt.Text.Trim();

            DateTime agreementStartDate =
                TM_agreementstartTXT.Value.Date;

            DateTime agreementEndDate =
                TM_agreementendtxt.Value.Date;

            string monthlyRent =
                TM_monthlyrenttxt.Text.Trim();

            string status =
                TM_statustxt.SelectedItem?.ToString() ?? "";

            // VALIDATE BRANCH ID

            if (string.IsNullOrWhiteSpace(branchId))
            {
                MessageBox.Show(
                    "Please enter the Branch ID for this tenant.",
                    "Missing Branch ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE REQUIRED FIELDS

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(monthlyRent) ||
                string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show(
                    "Please complete all tenant information.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE MONTHLY RENT

            if (!decimal.TryParse(
                monthlyRent,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal rent) || rent <= 0)
            {
                MessageBox.Show(
                    "Please enter a valid monthly rent greater than zero.",
                    "Invalid Monthly Rent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE AGREEMENT DATES

            if (agreementEndDate < agreementStartDate)
            {
                MessageBox.Show(
                    "Agreement end date cannot be earlier than the start date.",
                    "Invalid Agreement Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to add this tenant?",
                "Confirm Add Tenant",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string tenantId =
                    IDGenerator.GenerateNextID("TEN", branchId);

                const string query = @"
                    INSERT INTO TenantTbl
                    (
                        TenantId,
                        BranchId,
                        FullName,
                        ContactNumber,
                        Address,
                        AgreementStartDate,
                        AgreementEndDate,
                        MonthlyRent,
                        Status
                    )
                    VALUES
                    (
                        @TenantId,
                        @BranchId,
                        @FullName,
                        @ContactNumber,
                        @Address,
                        @AgreementStartDate,
                        @AgreementEndDate,
                        @MonthlyRent,
                        @Status
                    );";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantId,
                        BranchId = branchId,
                        FullName = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        AgreementStartDate = agreementStartDate,
                        AgreementEndDate = agreementEndDate,
                        MonthlyRent = rent,
                        Status = status
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant added successfully!\nTenant ID: " + tenantId,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearTenantDetails();

                    ApplyTenantFilters();
                    LoadRoomsdisplay();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to add tenant.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding tenant:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // CLEAR BUTTON
        // Button Name: guna2Button2
        // TENANT DETAILS ONLY
        // ==========================================

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ClearTenantDetails();
        }

        // ==========================================
        // CLEAR TENANT DETAILS ONLY
        // ==========================================

        private void ClearTenantDetails()
        {
            tenantBranchId = "";

            TM_tenantidtxt.Clear();
            TM_fullnametxt.Clear();
            TM_contacttxt.Clear();
            TM_AdressTxt.Clear();
            TM_monthlyrenttxt.Clear();

            TM_branchidtxt.Clear();

            TM_agreementstartTXT.Value = DateTime.Today;
            TM_agreementendtxt.Value = DateTime.Today;

            TM_statustxt.SelectedIndex = -1;
        }

        // ==========================================
        // UPDATE TENANT
        // ==========================================

        private void TM_UpdateBtn_Click(object sender, EventArgs e)
        {
            string tenantid =
                TM_tenantidtxt.Text.Trim();

            string fullname =
                TM_fullnametxt.Text.Trim();

            string contactnumber =
                TM_contacttxt.Text.Trim();

            string address =
                TM_AdressTxt.Text.Trim();

            DateTime agreementStartDate =
                TM_agreementstartTXT.Value.Date;

            DateTime agreementEndDate =
                TM_agreementendtxt.Value.Date;

            string monthlyRent =
                TM_monthlyrenttxt.Text.Trim();

            string status =
                TM_statustxt.SelectedItem?.ToString() ?? "";

            // VALIDATE SELECTED TENANT

            if (string.IsNullOrWhiteSpace(tenantid) ||
                string.IsNullOrWhiteSpace(tenantBranchId))
            {
                MessageBox.Show(
                    "Please select a tenant from the table first.",
                    "No Tenant Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE REQUIRED FIELDS

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(monthlyRent) ||
                string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show(
                    "Please complete all tenant information.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE MONTHLY RENT

            if (!decimal.TryParse(
                monthlyRent,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal rent) || rent <= 0)
            {
                MessageBox.Show(
                    "Please enter a valid monthly rent greater than zero.",
                    "Invalid Monthly Rent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE AGREEMENT DATES

            if (agreementEndDate < agreementStartDate)
            {
                MessageBox.Show(
                    "Agreement end date cannot be earlier than the start date.",
                    "Invalid Agreement Date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to update this tenant?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    UPDATE TenantTbl
                    SET
                        FullName = @FullName,
                        ContactNumber = @ContactNumber,
                        Address = @Address,
                        AgreementStartDate = @AgreementStartDate,
                        AgreementEndDate = @AgreementEndDate,
                        MonthlyRent = @MonthlyRent,
                        Status = @Status
                    WHERE
                        TenantId = @TenantId
                        AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantid,
                        BranchId = tenantBranchId,
                        FullName = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        AgreementStartDate = agreementStartDate,
                        AgreementEndDate = agreementEndDate,
                        MonthlyRent = rent,
                        Status = status
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ApplyTenantFilters();
                    LoadRoomsdisplay();
                }
                else
                {
                    MessageBox.Show(
                        "No tenant record was updated.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating tenant:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DELETE TENANT
        // ==========================================

        private void TM_deleteBtn_Click(object sender, EventArgs e)
        {
            string tenantid =
                TM_tenantidtxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenantid) ||
                string.IsNullOrWhiteSpace(tenantBranchId))
            {
                MessageBox.Show(
                    "Please select a tenant from the table first.",
                    "No Tenant Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this tenant?\n\nTenant ID: "
                + tenantid,
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    DELETE FROM TenantTbl
                    WHERE
                        TenantId = @TenantId
                        AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantid,
                        BranchId = tenantBranchId
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearTenantDetails();

                    ApplyTenantFilters();
                    LoadRoomsdisplay();
                }
                else
                {
                    MessageBox.Show(
                        "Tenant record was not found.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete tenant. The tenant may have existing reservation or payment records.\n\n"
                    + ex.Message,
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // TENANT STATISTICS
        // ==========================================

        public void LoadRoomsdisplay()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(*) AS TotalTenants,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Active'
                            THEN 1 ELSE 0 END
                        ), 0) AS ActiveTenants,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Inactive'
                            THEN 1 ELSE 0 END
                        ), 0) AS InactiveTenants,

                        COALESCE(SUM(
                            CASE WHEN Status = 'Pending'
                            THEN 1 ELSE 0 END
                        ), 0) AS PendingTenants

                    FROM TenantTbl;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { });

                if (data == null || data.Rows.Count == 0)
                    return;

                DataRow row = data.Rows[0];

                TM_totaltenLbl.Text =
                    Convert.ToInt32(row["TotalTenants"]).ToString();

                TM_activeLbl.Text =
                    Convert.ToInt32(row["ActiveTenants"]).ToString();

                TM_InactiveLbl.Text =
                    Convert.ToInt32(row["InactiveTenants"]).ToString();

                TM_pendingLbl.Text =
                    Convert.ToInt32(row["PendingTenants"]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading tenant statistics:\n" + ex.Message,
                    "Statistics Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // TENANT FILTERING AND SEARCH
        // ==========================================

        private void ApplyTenantFilters()
        {
            try
            {
                List<string> conditions = new List<string>();

                Dictionary<string, object> parameters =
                    new Dictionary<string, object>();

                // STATUS FILTER

                if (TM_tenantstatusCB.SelectedIndex > 0 &&
                    TM_tenantstatusCB.SelectedItem != null)
                {
                    conditions.Add("Status = @Status");

                    parameters["Status"] =
                        TM_tenantstatusCB.SelectedItem.ToString();
                }

                // SEARCH FILTER

                string search =
                    TM_searchTxt.Text.Trim();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    conditions.Add(@"
                        (
                            TenantId LIKE @Search
                            OR FullName LIKE @Search
                            OR ContactNumber LIKE @Search
                            OR Address LIKE @Search
                            OR BranchId LIKE @Search
                        )");

                    parameters["Search"] =
                        "%" + search + "%";
                }

                string query = @"
                    SELECT
                        TenantId,
                        FullName,
                        ContactNumber,
                        Address,
                        AgreementStartDate,
                        AgreementEndDate,
                        MonthlyRent,
                        Status,
                        BranchId
                    FROM TenantTbl
                    " + (conditions.Count > 0
                        ? "WHERE " + string.Join(" AND ", conditions)
                        : "") + @"
                    ORDER BY TenantId ASC;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    parameters);

                if (data == null)
                {
                    TM_TenantDGV.DataSource = null;
                    ClearTenantDetails();
                    return;
                }

                TM_TenantDGV.AutoGenerateColumns = true;
                TM_TenantDGV.DataSource = null;
                TM_TenantDGV.DataSource = data;

                // HIDE BRANCH ID

                if (TM_TenantDGV.Columns.Contains("BranchId"))
                {
                    TM_TenantDGV.Columns["BranchId"].Visible = false;
                }

                // COLUMN HEADERS

                TM_TenantDGV.Columns["TenantId"].HeaderText = "Tenant ID";
                TM_TenantDGV.Columns["FullName"].HeaderText = "Full Name";
                TM_TenantDGV.Columns["ContactNumber"].HeaderText = "Contact Number";
                TM_TenantDGV.Columns["Address"].HeaderText = "Address";
                TM_TenantDGV.Columns["AgreementStartDate"].HeaderText = "Agreement Start";
                TM_TenantDGV.Columns["AgreementEndDate"].HeaderText = "Agreement End";
                TM_TenantDGV.Columns["MonthlyRent"].HeaderText = "Monthly Rent";
                TM_TenantDGV.Columns["Status"].HeaderText = "Status";

                // DISPLAY FIRST TENANT

                if (data.Rows.Count > 0)
                {
                    TM_TenantDGV.ClearSelection();

                    TM_TenantDGV.Rows[0].Selected = true;

                    TM_TenantDGV.CurrentCell =
                        TM_TenantDGV.Rows[0].Cells["TenantId"];

                    loaddisplay();
                }
                else
                {
                    ClearTenantDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading tenants:\n" + ex.Message,
                    "Tenant Display Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
