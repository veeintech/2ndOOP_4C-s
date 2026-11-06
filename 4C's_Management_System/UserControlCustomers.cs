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
        string branchId = "brc-001";

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
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
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

        private void CM_CustomerType_indexChanged(object sender, EventArgs e)
        {
            RoomCapacitysort();
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

        private void CM_dataselecteditemchanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            loaddisplay();
        }

        public void RoomCapacitysort()
        {
            if (CM_custypeCB.SelectedIndex == -1 || CM_custypeCB.SelectedItem is null)
                return;

            try
            {
                if (CM_custypeCB.SelectedIndex == 0)
                {
                    string roomType1 = CM_custypeCB.SelectedItem.ToString()!;
                    const string query1 =
                        "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                        "FROM CustomerTbl " +
                        "ORDER BY CustomerID ASC";

                    AppDB.Instance.Table(
                        query1,
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

                    return;
                }
                string capacity = CM_custypeCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE CustomerType = @CustomerType " +
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
                    new { CustomerType = capacity },
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
                    "WHERE CustomerID Like @CustomerID " +
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
                    new { CustomerID = $"%{customerId}%" },
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
            string customerid = IDGenerator.GenerateNextCustomerID();
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
                    " Address = @Address, CustomerType = @CustomerType, CustomerStatus = @CustomerStatus WHERE CustomerID = @CustomerID ";

                var result = AppDB.Instance.CUD(query, new
                {
                    CustomerID = customerid,
                    BranchID = "brc-001",
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

        private void CM_selectedindexchange_stat(object sender, EventArgs e)
        {
            if (CM_CustomerstatCB.SelectedIndex == -1 || CM_CustomerstatCB.SelectedItem is null)
                return;

            try
            {
                if (CM_CustomerstatCB.SelectedIndex == 0)
                {
                    string roomType1 = CM_CustomerstatCB.SelectedItem.ToString()!;
                    const string query1 =
                        "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                        "FROM CustomerTbl " +
                        "ORDER BY CustomerID ASC";

                    AppDB.Instance.Table(
                        query1,
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

                    return;
                }
                string capacity = CM_CustomerstatCB.SelectedItem.ToString()!;
                const string query =
                    "SELECT CustomerID, Fullname, ContactNumber, Address, CustomerType , CustomerStatus " +
                    "FROM CustomerTbl " +
                    "WHERE CustomerStatus = @CustomerStatus " +
                    "ORDER BY CustomerID ASC";

                AppDB.Instance.Table(
                    query,
                    new { CustomerStatus = capacity },
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
                    "SUM(CASE WHEN CustomerStatus = 'Active' THEN 1 ELSE 0 END) AS ActiveCustomers, " +
                    "SUM(CASE WHEN CustomerStatus = 'Inactive' THEN 1 ELSE 0 END) AS InactiveCustomers, " +
                    "SUM(CASE WHEN CustomerStatus = 'Pending' THEN 1 ELSE 0 END) AS PendingCustomers " +
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
                    "Please select Data",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            try
            {

                const string query = "DELETE FROM CustomerTbl WHERE CustomerID = @CustomerID";

                var result = AppDB.Instance.CUD(query, new
                {
                    CustomerID = customerid
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
    }
}

