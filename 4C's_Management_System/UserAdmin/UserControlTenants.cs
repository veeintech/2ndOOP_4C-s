
using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlTenants : UserControl
    {
        public string branchid = AccessBranch_ByForm.BranchId;

        public UserControlTenants()
        {
            InitializeComponent();
        }

        // =====================================================
        // LOAD USERCONTROL
        // =====================================================

        private void UserControlTenants_Load(object sender, EventArgs e)
        {
            TM_tenantstatusCB.SelectedIndex = 0;

            ApplyTenantFilters();
            LoadRoomsdisplay();
        }

        // =====================================================
        // LOAD TENANT DATAGRIDVIEW
        // =====================================================

        public void LoadRoomData()
        {
            ApplyTenantFilters();
        }

        // =====================================================
        // DISPLAY SELECTED TENANT
        // =====================================================

        public void loaddisplay()
        {
            if (TM_TenantDGV.CurrentRow == null ||
                TM_TenantDGV.CurrentRow.IsNewRow)
                return;

            DataGridViewRow row = TM_TenantDGV.CurrentRow;

            TM_tenantidtxt.Text =
                Convert.ToString(row.Cells[0].Value) ?? "";

            TM_fullnametxt.Text =
                Convert.ToString(row.Cells[1].Value) ?? "";

            TM_contacttxt.Text =
                Convert.ToString(row.Cells[2].Value) ?? "";

            TM_AdressTxt.Text =
                Convert.ToString(row.Cells[3].Value) ?? "";

            if (DateTime.TryParse(
                Convert.ToString(row.Cells[4].Value),
                out DateTime agreementStartDate))
            {
                TM_agreementstartTXT.Value = agreementStartDate;
            }

            if (DateTime.TryParse(
                Convert.ToString(row.Cells[5].Value),
                out DateTime agreementEndDate))
            {
                TM_agreementendtxt.Value = agreementEndDate;
            }

            TM_monthlyrenttxt.Text =
                Convert.ToString(row.Cells[6].Value) ?? "";

            TM_statustxt.Text =
                Convert.ToString(row.Cells[7].Value) ?? "";
        }

        // =====================================================
        // DATAGRIDVIEW CELL CLICK
        // =====================================================

        private void tenantDGV_clickcontent(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            TM_TenantDGV.ClearSelection();

            TM_TenantDGV.Rows[e.RowIndex].Selected = true;

            loaddisplay();
        }

        // =====================================================
        // TENANT STATUS FILTER
        // =====================================================

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

        // =====================================================
        // SEARCH TENANT
        // =====================================================

        private void TM_searchtxtTextChanged(
            object sender,
            EventArgs e)
        {
            ApplyTenantFilters();
        }

        // =====================================================
        // ADD TENANT
        // =====================================================

        private void TM_AddCusBtn_Click(object sender, EventArgs e)
        {
            string fullname = TM_fullnametxt.Text.Trim();
            string contactnumber = TM_contacttxt.Text.Trim();
            string address = TM_AdressTxt.Text.Trim();

            DateTime agreementStartDate =
                TM_agreementstartTXT.Value.Date;

            DateTime agreementEndDate =
                TM_agreementendtxt.Value.Date;

            string monthlyRent = TM_monthlyrenttxt.Text.Trim();

            string status =
                TM_statustxt.SelectedItem?.ToString() ?? "";

            // CHECK IF TENANT IS ALREADY SELECTED
            if (!string.IsNullOrWhiteSpace(TM_tenantidtxt.Text))
            {
                MessageBox.Show(
                    "A tenant is already selected. Clear the fields before adding a new tenant.",
                    "Information",
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
                    "Please fill in all fields.",
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

            try
            {
                string tenantid =
                    IDGenerator.GenerateNextID("TEN", branchid);

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
                    )";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantid,
                        BranchId = branchid,
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
                        "Tenant added successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
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
                    "Error adding tenant: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CLEAR FIELDS
        // =====================================================

        public void ClearFields()
        {
            TM_tenantidtxt.Clear();
            TM_fullnametxt.Clear();
            TM_contacttxt.Clear();
            TM_AdressTxt.Clear();

            TM_monthlyrenttxt.Clear();

            TM_agreementstartTXT.Value = DateTime.Today;
            TM_agreementendtxt.Value = DateTime.Today;

            TM_statustxt.SelectedIndex = -1;

            TM_TenantDGV.ClearSelection();
        }

        // =====================================================
        // UPDATE TENANT
        // =====================================================

        private void TM_UpdateBtn_Click(object sender, EventArgs e)
        {
            string tenantid = TM_tenantidtxt.Text.Trim();
            string fullname = TM_fullnametxt.Text.Trim();
            string contactnumber = TM_contacttxt.Text.Trim();
            string address = TM_AdressTxt.Text.Trim();

            DateTime agreementStartDate =
                TM_agreementstartTXT.Value.Date;

            DateTime agreementEndDate =
                TM_agreementendtxt.Value.Date;

            string monthlyRent = TM_monthlyrenttxt.Text.Trim();

            string status =
                TM_statustxt.SelectedItem?.ToString() ?? "";

            // VALIDATE REQUIRED FIELDS
            if (string.IsNullOrWhiteSpace(tenantid) ||
                string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(monthlyRent) ||
                string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show(
                    "Please select a tenant and fill in all fields.",
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
                    WHERE TenantId = @TenantId
                    AND BranchId = @BranchId";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantid,
                        BranchId = branchid,
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

                    ClearFields();
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
                    "Error updating tenant: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DELETE TENANT
        // =====================================================

        private void TM_deleteBtn_Click(object sender, EventArgs e)
        {
            string tenantid = TM_tenantidtxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenantid))
            {
                MessageBox.Show(
                    "Please select a tenant first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this tenant?\n\nTenant ID: "
                + tenantid,
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    DELETE FROM TenantTbl
                    WHERE TenantId = @TenantId
                    AND BranchId = @BranchId";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        TenantId = tenantid,
                        BranchId = branchid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
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

        // =====================================================
        // TENANT STATISTICS
        // =====================================================

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

                    FROM TenantTbl
                    WHERE BranchId = @BranchId";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchid });

                if (data.Rows.Count == 0)
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
                    "Error loading tenant statistics: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // TENANT STATUS FILTERING AND SEARCH
        // =====================================================

        private void ApplyTenantFilters()
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

                // FILTER TENANT STATUS
                if (TM_tenantstatusCB.SelectedIndex > 0 &&
                    TM_tenantstatusCB.SelectedItem != null)
                {
                    conditions.Add("Status = @Status");

                    parameters["Status"] =
                        TM_tenantstatusCB.SelectedItem.ToString()!;
                }

                // SEARCH TENANT ID
                string tenantid = TM_searchTxt.Text.Trim();

                if (!string.IsNullOrWhiteSpace(tenantid))
                {
                    conditions.Add("TenantId LIKE @TenantId");

                    parameters["TenantId"] =
                        "%" + tenantid + "%";
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
                        Status
                    FROM TenantTbl
                    WHERE " + string.Join(" AND ", conditions) + @"
                    ORDER BY TenantId ASC";

                AppDB.Instance.Table(
                    query,
                    parameters,
                    TM_TenantDGV,
                    header: new[]
                    {
                        "Tenant ID",
                        "Full Name",
                        "Contact Number",
                        "Address",
                        "Agreement Start",
                        "Agreement End",
                        "Monthly Rent",
                        "Status"
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading tenant data: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
