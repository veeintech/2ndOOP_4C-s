
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlCustomers : UserControl
    {
        public UserControlCustomers()
        {
            InitializeComponent();
        }
        public int idnum = 0;
        public string branchId = "brc-001";
        public string tempid = "";

        private void UserControlCustomers_Load(object sender, EventArgs e)
        {
            LoadRoomData();
            CM_custypeCB.SelectedIndex = 0;
            CM_CustomerstatCB.SelectedIndex = 0;
            LoadRoomsdisplay();
        }

        public void LoadRoomData()
        {
            try
            {
                const string query =
                    "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE BranchId = @BranchId " +
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchId },
                    CM_CustomerDGV,
                    header: new[]
                    {
                    "Customer ID",
                    "Full Name",
                    "Contact Number",
                    "Address",
                    "Customer Type",
                    "Customer Status"

                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading Customerdata: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void loaddisplay()
        {
            if (CM_CustomerDGV.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = CM_CustomerDGV.SelectedRows[0];

            CM_CustomeridTxt.Text = row.Cells[0].Value?.ToString() ?? "";

            CM_fullnameTxt.Text = row.Cells[1].Value?.ToString() ?? "";
            CM_conManTxt.Text = row.Cells[2].Value?.ToString() ?? "";
            CM_AddressTxt.Text = row.Cells[3].Value?.ToString() ?? "";
            CM_AccomTypeTxt.Text = row.Cells[4].Value?.ToString() ?? "";
            CM_CustomerstatTxt.Text = row.Cells[5].Value?.ToString() ?? "";

        }

        private void CMsearchtxtTextchanged(object sender, EventArgs e)
        {
            string customerId = Cm_searchLbl.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                LoadRoomData();
                return;
            }

            try
            {
                const string query =
                    "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE BranchId = @BranchId AND CustomerID LIKE @CustomerID " +
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchId, CustomerID = $"%{customerId}%" },
                    CM_CustomerDGV,
                    header: new[]
                    {
                    "Customer ID",
                    "Full Name",
                    "Contact Number",
                    "Address",
                    "Customer Type",
                    "Customer Status"
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading customer data: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CM_AddCusBtn_Click(object sender, EventArgs e)
        {
            string fullname = CM_fullnameTxt.Text.Trim();
            string contactnumber = CM_conManTxt.Text.Trim();
            string address = CM_AddressTxt.Text.Trim();
            string customerType = CM_AccomTypeTxt.Text.Trim();
            string customerStatus = CM_CustomerstatTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType) ||
                string.IsNullOrWhiteSpace(customerStatus))
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
                if (CM_CustomeridTxt.Text == "")
                {
                    string customerid = IDGenerator.GenerateNextCustomerID("CUS");
                    CM_CustomeridTxt.Text = customerid;
                    tempid = customerid;

                    const string query =
                        "INSERT INTO CustomerTbl (CustomerID,BranchID, Fullname, ContactNumber, Address, CustomerType, CustomerStatus) " +
                        "VALUES (@CustomerID, @BranchID, @Fullname, @ContactNumber, @Address, @CustomerType, @CustomerStatus)";

                    var result = AppDB.Instance.CUD(query, new
                    {
                        CustomerID = customerid,
                        BranchID = branchId,
                        Fullname = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        CustomerType = customerType,
                        CustomerStatus = customerStatus
                    });
                    if (result > 0)
                    {
                        MessageBox.Show(
                            "Customer added successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Customer cannot be added, already exists.",
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

        private void CM_UpdateBtn_Click(object sender, EventArgs e)
        {
            string customerid = CM_CustomeridTxt.Text.Trim();
            string fullname = CM_fullnameTxt.Text.Trim();
            string contactnumber = CM_conManTxt.Text.Trim();
            string address = CM_AddressTxt.Text.Trim();
            string customerType = CM_AccomTypeTxt.Text.Trim();
            string customerStatus = CM_CustomerstatTxt.Text.Trim();
            CM_CustomeridTxt.Text = customerid;

            if (string.IsNullOrWhiteSpace(customerid) ||
                string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType) ||
                string.IsNullOrWhiteSpace(customerStatus))
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
                    "UPDATE CustomerTbl SET BranchID = @BranchID, Fullname = @Fullname, ContactNumber = @ContactNumber," +
                    " Address = @Address, CustomerType = @CustomerType, CustomerStatus = @CustomerStatus " +
                    "WHERE CustomerID = @CustomerID AND BranchID = @BranchID";

                var result = AppDB.Instance.CUD(query, new
                {
                    CustomerID = customerid,
                    BranchID = branchId,
                    Fullname = fullname,
                    ContactNumber = contactnumber,
                    Address = address,
                    CustomerType = customerType,
                    CustomerStatus = customerStatus
                });
                if (result > 0)
                {
                    MessageBox.Show(
                        "Customer updated successfully.",
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

        public void ClearFields()
        {
            CM_CustomeridTxt.Clear();
            CM_fullnameTxt.Clear();
            CM_conManTxt.Clear();
            CM_AddressTxt.Clear();
            CM_AccomTypeTxt.SelectedIndex = -1;
            CM_CustomerstatTxt.SelectedIndex = -1;
        }

        public void LoadRoomsdisplay()
        {
            try
            {
                const string query =
                    "SELECT " +
                    "COUNT(*) AS TotalCustomers, " +
                    "COALESCE(SUM(CASE WHEN CustomerStatus = 'Active' THEN 1 ELSE 0 END), 0) AS ActiveCustomers, " +
                    "COALESCE(SUM(CASE WHEN CustomerStatus = 'Inactive' THEN 1 ELSE 0 END), 0) AS InactiveCustomers, " +
                    "COALESCE(SUM(CASE WHEN CustomerStatus = 'Pending' THEN 1 ELSE 0 END), 0) AS PendingCustomers " +
                    "FROM CustomerTbl " +
                    "WHERE BranchId = @BranchId";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchId = branchId });

                var row = data.Rows[0];

                CM_totalcusLbl.Text =
                    Convert.ToInt32(row["TotalCustomers"]).ToString();

                CM_ActiveLbl.Text =
                    Convert.ToInt32(row["ActiveCustomers"]).ToString();

                CM_InactiveLbl.Text =
                    Convert.ToInt32(row["InactiveCustomers"]).ToString();

                CM_PendingLbl.Text =
                    Convert.ToInt32(row["PendingCustomers"]).ToString();
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

        private void CM_deleteBtn_Click(object sender, EventArgs e)
        {
            string customerid = CM_CustomeridTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerid))
            {
                MessageBox.Show(
                    "Please select Data",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            try
            {

                const string query =
                    "DELETE FROM CustomerTbl " +
                    "WHERE CustomerID = @CustomerID AND BranchID = @BranchID";

                var result = AppDB.Instance.CUD(query, new
                {
                    CustomerID = customerid,
                    BranchID = branchId
                });
                if (result > 0)
                {
                    MessageBox.Show(
                        "Customer deleted successfully.",
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

        private void Cellcontent_click_roomDgv(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            loaddisplay();
        }
       #region "Customer Status and Type Filtering"

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
            ["BranchId"] = branchId
        };

        if (CM_custypeCB.SelectedIndex > 0 &&
            CM_custypeCB.SelectedItem is not null)
        {
            conditions.Add("CustomerType = @CustomerType");
            parameters["CustomerType"] =
                CM_custypeCB.SelectedItem.ToString()!;
        }

        if (CM_CustomerstatCB.SelectedIndex > 0 &&
            CM_CustomerstatCB.SelectedItem is not null)
        {
            conditions.Add("CustomerStatus = @CustomerStatus");
            parameters["CustomerStatus"] =
                CM_CustomerstatCB.SelectedItem.ToString()!;
        }

        string customerId = Cm_searchLbl.Text.Trim();

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            conditions.Add("CustomerID LIKE @CustomerID");
            parameters["CustomerID"] = $"%{customerId}%";
        }

        string query =
            "SELECT CustomerID, Fullname, ContactNumber, Address, " +
            "CustomerType, CustomerStatus " +
            "FROM CustomerTbl " +
            $"WHERE {string.Join(" AND ", conditions)} " +
            "ORDER BY CustomerID ASC";

        AppDB.Instance.Table(
            query,
            parameters,
            CM_CustomerDGV,
            header: new[]
            {
                "Customer ID",
                "Full Name",
                "Contact Number",
                "Address",
                "Customer Type",
                "Customer Status"
            });
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"Error loading customer data: {ex.Message}",
            "Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}

private void CM_selectedindexchange_stat(object sender, EventArgs e)
{
    ApplyCustomerFilters();
}

private void CM_CustomerType_indexChanged(object sender, EventArgs e)
{
    ApplyCustomerFilters();
}

#endregion
    }
}

