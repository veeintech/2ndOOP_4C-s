
using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControlCustomers : UserControl
    {
        public string branchId = AccessBranch_ByForm.BranchId;

        public UserControlCustomers()
        {
            InitializeComponent();
        }

        // =====================================================
        // LOAD USERCONTROL
        // =====================================================

        private void UserControlCustomers_Load(object sender, EventArgs e)
        {
            branchId = AccessBranch_ByForm.BranchId;

            if (CM_custypeCB.Items.Count > 0)
                CM_custypeCB.SelectedIndex = 0;

            if (CM_CustomerstatCB.Items.Count > 0)
                CM_CustomerstatCB.SelectedIndex = 0;

            ApplyCustomerFilters();
            LoadRoomsdisplay();
        }

        // =====================================================
        // LOAD CUSTOMER DATAGRIDVIEW
        // =====================================================

        public void LoadRoomData()
        {
            ApplyCustomerFilters();
        }

        // =====================================================
        // DISPLAY SELECTED CUSTOMER
        // =====================================================

        public void loaddisplay()
        {
            if (CM_CustomerDGV.CurrentRow == null ||
                CM_CustomerDGV.CurrentRow.IsNewRow)
                return;

            DataGridViewRow row = CM_CustomerDGV.CurrentRow;

            CM_CustomeridTxt.Text =
                Convert.ToString(row.Cells[0].Value) ?? "";

            CM_fullnameTxt.Text =
                Convert.ToString(row.Cells[1].Value) ?? "";

            CM_conManTxt.Text =
                Convert.ToString(row.Cells[2].Value) ?? "";

            CM_AddressTxt.Text =
                Convert.ToString(row.Cells[3].Value) ?? "";

            CM_AccomTypeTxt.Text =
                Convert.ToString(row.Cells[4].Value) ?? "";
        }

        // =====================================================
        // SEARCH CUSTOMER
        // =====================================================

        private void CMsearchtxtTextchanged(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        // =====================================================
        // ADD CUSTOMER
        // =====================================================

        private void CM_AddCusBtn_Click(object sender, EventArgs e)
        {
            string fullname = CM_fullnameTxt.Text.Trim();
            string contactnumber = CM_conManTxt.Text.Trim();
            string address = CM_AddressTxt.Text.Trim();
            string customerType = CM_AccomTypeTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType))
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
                if (!string.IsNullOrWhiteSpace(CM_CustomeridTxt.Text))
                {
                    MessageBox.Show(
                        "Customer cannot be added because a customer is already selected. Please clear the fields first.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string customerid =
                    IDGenerator.GenerateNextID("CUS", branchId);

                const string query = @"
                    INSERT INTO CustomerTbl
                    (
                        CustomerID,
                        BranchID,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus
                    )
                    VALUES
                    (
                        @CustomerID,
                        @BranchID,
                        @Fullname,
                        @ContactNumber,
                        @Address,
                        @CustomerType,
                        @CustomerStatus
                    )";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerID = customerid,
                        BranchID = branchId,
                        Fullname = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        CustomerType = customerType,
                        CustomerStatus = "Inactive"
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Customer added successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    ApplyCustomerFilters();
                    LoadRoomsdisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding customer: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // UPDATE CUSTOMER
        // =====================================================

        private void CM_UpdateBtn_Click(object sender, EventArgs e)
        {
            string customerid = CM_CustomeridTxt.Text.Trim();
            string fullname = CM_fullnameTxt.Text.Trim();
            string contactnumber = CM_conManTxt.Text.Trim();
            string address = CM_AddressTxt.Text.Trim();
            string customerType = CM_AccomTypeTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerid) ||
                string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType))
            {
                MessageBox.Show(
                    "Please select a customer and fill in all fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                const string query = @"
                    UPDATE CustomerTbl
                    SET
                        Fullname = @Fullname,
                        ContactNumber = @ContactNumber,
                        Address = @Address,
                        CustomerType = @CustomerType
                    WHERE CustomerID = @CustomerID
                    AND BranchID = @BranchID";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerID = customerid,
                        BranchID = branchId,
                        Fullname = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        CustomerType = customerType
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Customer updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    ApplyCustomerFilters();
                    LoadRoomsdisplay();
                }
                else
                {
                    MessageBox.Show(
                        "No customer record was updated.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating customer: " + ex.Message,
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
            CM_CustomeridTxt.Clear();
            CM_fullnameTxt.Clear();
            CM_conManTxt.Clear();
            CM_AddressTxt.Clear();
            CM_AccomTypeTxt.SelectedIndex = -1;

            CM_CustomerDGV.ClearSelection();
        }

        // =====================================================
        // CUSTOMER STATISTICS
        // =====================================================

        public void LoadRoomsdisplay()
        {
            try
            {
                const string query = @"
                    SELECT
                        COUNT(*) AS TotalCustomers,

                        COALESCE(SUM(
                            CASE WHEN CustomerStatus = 'Active'
                            THEN 1 ELSE 0 END
                        ), 0) AS ActiveCustomers,

                        COALESCE(SUM(
                            CASE WHEN CustomerStatus = 'Inactive'
                            THEN 1 ELSE 0 END
                        ), 0) AS InactiveCustomers,

                        COALESCE(SUM(
                            CASE WHEN CustomerStatus = 'Pending'
                            THEN 1 ELSE 0 END
                        ), 0) AS PendingCustomers

                    FROM CustomerTbl
                    WHERE BranchID = @BranchID";

                var data = AppDB.Instance.TableData(
                    query,
                    new { BranchID = branchId });

                if (data.Rows.Count == 0)
                    return;

                DataRow row = data.Rows[0];

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
                    "Error loading customer statistics: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DELETE CUSTOMER
        // =====================================================

        private void CM_deleteBtn_Click(object sender, EventArgs e)
        {
            string customerid = CM_CustomeridTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerid))
            {
                MessageBox.Show(
                    "Please select a customer first.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?\n\nCustomer ID: "
                + customerid,
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    DELETE FROM CustomerTbl
                    WHERE CustomerID = @CustomerID
                    AND BranchID = @BranchID";

                int result = AppDB.Instance.CUD(
                    query,
                    new
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

                    ClearFields();
                    ApplyCustomerFilters();
                    LoadRoomsdisplay();
                }
                else
                {
                    MessageBox.Show(
                        "Customer record was not found.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete customer. The customer may have existing reservation or payment records.\n\n"
                    + ex.Message,
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // DATAGRIDVIEW CELL CLICK
        // =====================================================

        private void Cellcontent_click_roomDgv(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            CM_CustomerDGV.ClearSelection();

            CM_CustomerDGV.Rows[e.RowIndex].Selected = true;

            loaddisplay();
        }

        // =====================================================
        // CUSTOMER STATUS AND TYPE FILTERING
        // =====================================================

        private void ApplyCustomerFilters()
        {
            try
            {
                branchId = AccessBranch_ByForm.BranchId;

                var conditions = new List<string>
                {
                    "BranchID = @BranchID"
                };

                var parameters = new Dictionary<string, object>
                {
                    ["BranchID"] = branchId
                };

                // FILTER CUSTOMER TYPE
                if (CM_custypeCB.SelectedIndex > 0 &&
                    CM_custypeCB.SelectedItem != null)
                {
                    conditions.Add(
                        "CustomerType = @CustomerType");

                    parameters["CustomerType"] =
                        CM_custypeCB.SelectedItem.ToString();
                }

                // FILTER CUSTOMER STATUS
                if (CM_CustomerstatCB.SelectedIndex > 0 &&
                    CM_CustomerstatCB.SelectedItem != null)
                {
                    conditions.Add(
                        "CustomerStatus = @CustomerStatus");

                    parameters["CustomerStatus"] =
                        CM_CustomerstatCB.SelectedItem.ToString();
                }

                // SEARCH CUSTOMER DETAILS
                string searchText = Cm_searchLbl.Text.Trim();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    conditions.Add(@"
                        (
                            CustomerID LIKE @Search
                            OR Fullname LIKE @Search
                            OR ContactNumber LIKE @Search
                            OR Address LIKE @Search
                        )");

                    parameters["Search"] = "%" + searchText + "%";
                }

                // FINAL QUERY
                string query = @"
                    SELECT
                        CustomerID,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus
                    FROM CustomerTbl
                    WHERE " + string.Join(" AND ", conditions) + @"
                    ORDER BY CustomerID ASC";

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
                    "Error loading customer data: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CUSTOMER STATUS FILTER EVENT
        // =====================================================

        private void CM_selectedindexchange_stat(
            object sender,
            EventArgs e)
        {
            ApplyCustomerFilters();
        }

        // =====================================================
        // CUSTOMER TYPE FILTER EVENT
        // =====================================================

        private void CM_CustomerType_indexChanged(
            object sender,
            EventArgs e)
        {
            ApplyCustomerFilters();
        }
    }
}
