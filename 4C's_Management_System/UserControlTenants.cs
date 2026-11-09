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
    public partial class UserControlTenants : UserControl
    {
        public UserControlTenants()
        {
            InitializeComponent();
        }
        public string branchid = "BRN-001";

        private void UserControlTenants_Load(object sender, EventArgs e)
        {
            LoadRoomData();
            LoadRoomsdisplay();
            TM_tenantstatusCB.SelectedIndex = 0;
        }

        public void LoadRoomData()
        {
            try
            {
                const string query =
                    "SELECT TenantId, FullName, ContactNumber, Address, AgreementStartDate, AgreementEndDate, MonthlyRent, Status " +
                    "FROM TenantTbl " +
                    "WHERE BranchId = @BranchId " +
                    "ORDER BY TenantId ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchid },
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
                    $"Error loading Tenant data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void loaddisplay()
        {
            if (TM_TenantDGV.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = TM_TenantDGV.SelectedRows[0];

            TM_tenantidtxt.Text = row.Cells[0].Value?.ToString() ?? "";

            TM_fullnametxt.Text = row.Cells[1].Value?.ToString() ?? "";
            TM_contacttxt.Text = row.Cells[2].Value?.ToString() ?? "";
            TM_AdressTxt.Text = row.Cells[3].Value?.ToString() ?? "";
            if (DateTime.TryParse(row.Cells[4].Value?.ToString(), out DateTime agreementStartDate))
                TM_agreementstartTXT.Value = agreementStartDate;

            if (DateTime.TryParse(row.Cells[5].Value?.ToString(), out DateTime agreementEndDate))
                TM_agreementendtxt.Value = agreementEndDate;
            TM_monthlyrenttxt.Text = row.Cells[6].Value?.ToString() ?? "";
            TM_statustxt.Text = row.Cells[7].Value?.ToString() ?? "";

        }

        private void tenantDGV_clickcontent(object sender, DataGridViewCellEventArgs e)
        {
            loaddisplay();
        }

        public void statuscombobox()
        {
            if (TM_tenantstatusCB.SelectedIndex == -1 || TM_tenantstatusCB.SelectedItem is null)
                return;

            try
            {
                if (TM_tenantstatusCB.SelectedIndex == 0)
                {
                    string roomType1 = TM_tenantstatusCB.SelectedItem.ToString()!;
                    const string query1 =
                        "SELECT TenantId, FullName, ContactNumber, Address, AgreementStartDate, AgreementEndDate, MonthlyRent, Status " +
                        "FROM TenantTbl " +
                        "WHERE BranchId = @BranchId " +
                        "ORDER BY TenantId ASC";

                    AppDB.Instance.Table(
                        query1,
                        new { BranchId = branchid },
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

                    return;
                }
                string capacity = TM_tenantstatusCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT TenantId, FullName, ContactNumber, Address, AgreementStartDate, AgreementEndDate, MonthlyRent, Status " +
                    "FROM TenantTbl " +
                    "WHERE BranchId = @BranchId AND Status = @Status " +
                    "ORDER BY TenantId ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchid, Status = capacity },
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
                    $"Error loading tenant data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }

        private void TM_tenantselectedindexchanged(object sender, EventArgs e)
        {
            statuscombobox();
        }

        private void TM_searchtxtTextChanged(object sender, EventArgs e)
        {
            string customerId = TM_searchTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                LoadRoomData();
                return;
            }

            try
            {
                const string query =
                    "SELECT TenantId, FullName, ContactNumber, Address, AgreementStartDate, AgreementEndDate, MonthlyRent, Status " +
                    "FROM TenantTbl " +
                    "WHERE BranchId = @BranchId AND TenantId LIKE @TenantId " +
                    "ORDER BY TenantId ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchid, TenantId = $"%{customerId}%" },
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
                    $"Error loading tenant data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void TM_AddCusBtn_Click(object sender, EventArgs e)
        {
            // Replace this with a tenant-specific sequence when one is created.
            string tenantid = IDGenerator.GenerateNextCustomerID("TEN");
            string fullname = TM_fullnametxt.Text.Trim();
            string contactnumber = TM_contacttxt.Text.Trim();
            string address = TM_AdressTxt.Text.Trim();
            DateTime agreementEndDate = TM_agreementendtxt.Value;
            DateTime agreementStartDate = TM_agreementstartTXT.Value;
            string monthlyRent = TM_monthlyrenttxt.Text.Trim();
            string status = TM_statustxt.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(tenantid) ||
                string.IsNullOrWhiteSpace(fullname) ||
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
            else
            {
                try
                {
                    if(TM_tenantidtxt.Text == "")
                    {
                        const string query =
                        "INSERT INTO TenantTbl " +
                        "(TenantId, BranchId, FullName, ContactNumber, Address, AgreementStartDate, AgreementEndDate, MonthlyRent, Status) " +
                        "VALUES (@TenantId, @BranchId, @FullName, @ContactNumber, @Address, @AgreementStartDate, @AgreementEndDate, @MonthlyRent, @Status)";

                        var result = AppDB.Instance.CUD(query, new
                        {
                            TenantId = tenantid,
                            BranchId = branchid,
                            FullName = fullname,
                            ContactNumber = contactnumber,
                            Address = address,
                            AgreementStartDate = agreementStartDate,
                            AgreementEndDate = agreementEndDate,
                            MonthlyRent = monthlyRent,
                            Status = status
                        });
                        if (result > 0)
                        {
                            MessageBox.Show(
                                "Tenant added successfully.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Tenant cannot be added, already exists.",
                            "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    LoadRoomData();
                    ClearFields();
                    LoadRoomsdisplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void ClearFields()
        {
            TM_tenantidtxt.Clear();
            TM_fullnametxt.Clear();
            TM_contacttxt.Clear();
            TM_AdressTxt.Clear();
            TM_monthlyrenttxt.Clear();
            TM_agreementendtxt.Value = DateTime.Today;
            TM_agreementstartTXT.Value = DateTime.Today;
            TM_statustxt.SelectedIndex = -1;
        }

        private void TM_UpdateBtn_Click(object sender, EventArgs e)
        {
            string tenantid = TM_tenantidtxt.Text.Trim();
            string fullname = TM_fullnametxt.Text.Trim();
            string contactnumber = TM_contacttxt.Text.Trim();
            string address = TM_AdressTxt.Text.Trim();
            DateTime agreementEndDate = TM_agreementendtxt.Value;
            DateTime agreementStartDate = TM_agreementstartTXT.Value;
            string month = TM_monthlyrenttxt.Text.Trim();
            string status = TM_statustxt.SelectedItem?.ToString();
            TM_tenantidtxt.Text = tenantid;

            if (string.IsNullOrWhiteSpace(tenantid) ||
                string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(month) ||
                string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            try
            {
                const string query =
                    "UPDATE TenantTbl SET BranchId = @BranchId, FullName = @FullName, ContactNumber = @ContactNumber, " +
                    "Address = @Address, AgreementStartDate = @AgreementStartDate, AgreementEndDate = @AgreementEndDate, " +
                    "MonthlyRent = @MonthlyRent, Status = @Status " +
                    "WHERE TenantId = @TenantId";

                var result = AppDB.Instance.CUD(query, new
                {
                    BranchId = branchid,
                    FullName = fullname,
                    ContactNumber = contactnumber,
                    Address = address,
                    AgreementStartDate = agreementStartDate,
                    AgreementEndDate = agreementEndDate,
                    MonthlyRent = month,
                    Status = status,
                    TenantId = tenantid,

                });
                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                LoadRoomData();
                ClearFields();
                LoadRoomsdisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TM_deleteBtn_Click(object sender, EventArgs e)
        {
            string tenantid = TM_tenantidtxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(tenantid))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            try
            {

                const string query = "DELETE FROM TenantTbl WHERE TenantId = @TenantId";

                var result = AppDB.Instance.CUD(query, new
                {
                    TenantId = tenantid
                });
                if (result > 0)
                {
                    MessageBox.Show(
                        "Tenant deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                LoadRoomData();
                ClearFields();
                LoadRoomsdisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading rooms: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public void LoadRoomsdisplay()
        {
            try
            {
                const string query =
                    "SELECT " +
                    "COUNT(*) AS TotalTenants, " +
                    "COALESCE(SUM(CASE WHEN Status = 'Active' THEN 1 ELSE 0 END), 0) AS ActiveTenants, " +
                    "COALESCE(SUM(CASE WHEN Status = 'Inactive' THEN 1 ELSE 0 END), 0) AS InactiveTenants, " +
                    "COALESCE(SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END), 0) AS PendingTenants " +
                    "FROM TenantTbl " +
                    "WHERE BranchId = @BranchId";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchid });

                var row = data.Rows[0];

                TM_totaltenLbl.Text =
                    Convert.ToInt32(row["TotalTenants"]).ToString();

                TM_activeLbl.Text =
                    Convert.ToInt32(row["ActiveTenants"]).ToString();

                TM_pendingLbl.Text =
                    Convert.ToInt32(row["PendingTenants"]).ToString();

                TM_InactiveLbl.Text =
                    Convert.ToInt32(row["InactiveTenants"]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading customer statistics: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
