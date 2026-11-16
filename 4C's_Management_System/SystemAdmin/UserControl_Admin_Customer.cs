
using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Customer : UserControl
    {
        // ==========================================
        // VARIABLES
        // ==========================================

        private string customerBranchId = "";

        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public UserControl_Admin_Customer()
        {
            InitializeComponent();

            this.Load += UserControlCustomer_Load;
        
            // CLEAR BUTTON EVENT
            guna2Button2.Click += guna2Button2_Click;
        }

        // ==========================================
        // LOAD USERCONTROL
        // ==========================================

        private void UserControlCustomer_Load(object sender, EventArgs e)
        {
            ClearFields();

            // DEFAULT STATUS FILTER
            if (CM_CustomerstatCB.Items.Count > 0)
                CM_CustomerstatCB.SelectedIndex = 0;

            // DEFAULT CUSTOMER TYPE FILTER
            if (CM_custypeCB.Items.Count > 0)
                CM_custypeCB.SelectedIndex = 0;

            ApplyCustomerFilters();
            LoadCustomerDisplay();
            loadtable();
        }

        // ==========================================
        // LOAD CUSTOMER TABLE
        // ==========================================
        public void loadtable()
        {
            try
            {
                const string rquery = @"
                    SELECT
                        CustomerID,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus,
                        BranchId
                    FROM CustomerTbl
                    ORDER BY CustomerID ASC;";

                DataTable data = AppDB.Instance.TableData(
                    rquery,
                    new { });

                CM_CustomerDGV.DataSource = null;
                CM_CustomerDGV.AutoGenerateColumns = true;
                CM_CustomerDGV.DataSource = data;

                if (CM_CustomerDGV.Columns.Contains("BranchId"))
                    CM_CustomerDGV.Columns["BranchId"].Visible = false;

                if (CM_CustomerDGV.Columns.Contains("CustomerID"))
                    CM_CustomerDGV.Columns["CustomerID"].HeaderText = "Customer ID";

                if (CM_CustomerDGV.Columns.Contains("Fullname"))
                    CM_CustomerDGV.Columns["Fullname"].HeaderText = "Full Name";

                if (CM_CustomerDGV.Columns.Contains("ContactNumber"))
                    CM_CustomerDGV.Columns["ContactNumber"].HeaderText = "Contact Number";

                if (CM_CustomerDGV.Columns.Contains("Address"))
                    CM_CustomerDGV.Columns["Address"].HeaderText = "Address";

                if (CM_CustomerDGV.Columns.Contains("CustomerType"))
                    CM_CustomerDGV.Columns["CustomerType"].HeaderText = "Customer Type";

                if (CM_CustomerDGV.Columns.Contains("CustomerStatus"))
                    CM_CustomerDGV.Columns["CustomerStatus"].HeaderText = "Status";

                if (CM_CustomerDGV.Rows.Count > 0)
                {
                    CM_CustomerDGV.ClearSelection();

                    CM_CustomerDGV.Rows[0].Selected = true;

                    CM_CustomerDGV.CurrentCell =
                        CM_CustomerDGV.Rows[0].Cells["CustomerID"];

                    loaddisplay();
                }
                else
                {
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading customer table:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // LOAD CUSTOMER DATA
        // ==========================================

        public void LoadCustomerData()
        {
            ApplyCustomerFilters();
            LoadCustomerDisplay();
            loadtable();
        }

        // ==========================================
        // DISPLAY SELECTED CUSTOMER
        // ==========================================

        public void loaddisplay()
        {
            try
            {
                if (CM_CustomerDGV.CurrentRow == null ||
                    CM_CustomerDGV.CurrentRow.IsNewRow)
                    return;

                DataRowView row =
                    CM_CustomerDGV.CurrentRow.DataBoundItem as DataRowView;

                if (row == null)
                    return;

                CM_CustomeridTxt.Text =
                    Convert.ToString(row["CustomerID"]) ?? "";

                customerBranchId =
                    Convert.ToString(row["BranchId"]) ?? "";

                CM_fullnameTxt.Text =
                    Convert.ToString(row["Fullname"]) ?? "";

                CM_conManTxt.Text =
                    Convert.ToString(row["ContactNumber"]) ?? "";

                CM_AddressTxt.Text =
                    Convert.ToString(row["Address"]) ?? "";

                string customerType =
                    Convert.ToString(row["CustomerType"]) ?? "";

                string customerStatus =
                    Convert.ToString(row["CustomerStatus"]) ?? "";

                // SELECT CUSTOMER TYPE
                CM_AccomTypeTxt.SelectedIndex =
                    CM_AccomTypeTxt.FindStringExact(customerType);

                // SELECT CUSTOMER STATUS
                CM_statustxt.SelectedIndex =
                    CM_statustxt.FindStringExact(customerStatus);

                CM_branchidtxt.Text = customerBranchId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying customer details:\n" + ex.Message,
                    "Display Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DATAGRIDVIEW CELL CLICK
        // ==========================================

        private void CM_CustomerDGV_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                CM_CustomerDGV.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            CM_CustomerDGV.ClearSelection();

            row.Selected = true;

            if (CM_CustomerDGV.Columns.Contains("CustomerID"))
            {
                CM_CustomerDGV.CurrentCell =
                    row.Cells["CustomerID"];
            }
            else if (CM_CustomerDGV.Columns.Count > 0)
            {
                CM_CustomerDGV.CurrentCell =
                    row.Cells[0];
            }

            loaddisplay();
        }

        // ==========================================
        // CUSTOMER STATUS FILTER
        // ==========================================

        private void Custype_SIC(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        private void custype_SIC(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        // ==========================================
        // SEARCH CUSTOMER
        // ==========================================

        private void search_textChange(object sender, EventArgs e)
        {
            ApplyCustomerFilters();
        }

        // ==========================================
        // ADD CUSTOMER
        // ==========================================

        private void CM_AddCustomerBtn_Click(object sender, EventArgs e)
        {
            string branchId =
                CM_branchidtxt.Text.Trim();

            string fullname =
                CM_fullnameTxt.Text.Trim();

            string contactnumber =
                CM_conManTxt.Text.Trim();

            string address =
                CM_AddressTxt.Text.Trim();

            string customerType =
                CM_AccomTypeTxt.SelectedItem?.ToString()?.Trim() ?? "";

            string customerStatus =
                CM_statustxt.SelectedItem?.ToString()?.Trim() ?? "";

            // VALIDATE BRANCH ID

            if (string.IsNullOrWhiteSpace(branchId))
            {
                MessageBox.Show(
                    "Please enter the Branch ID for this customer.",
                    "Missing Branch ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE REQUIRED FIELDS

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType) ||
                string.IsNullOrWhiteSpace(customerStatus))
            {
                MessageBox.Show(
                    "Please complete all customer information.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to add this customer?",
                "Confirm Add Customer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string customerId =
                    IDGenerator.GenerateNextID("CUS", branchId);

                const string query = @"
                    INSERT INTO CustomerTbl
                    (
                        CustomerID,
                        BranchId,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus
                    )
                    VALUES
                    (
                        @CustomerID,
                        @BranchId,
                        @Fullname,
                        @ContactNumber,
                        @Address,
                        @CustomerType,
                        @CustomerStatus
                    );";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerID = customerId,
                        BranchId = branchId,
                        Fullname = fullname,
                        ContactNumber = contactnumber,
                        Address = address,
                        CustomerType = customerType,
                        CustomerStatus = customerStatus
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Customer added successfully!\nCustomer ID: " + customerId,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    ApplyCustomerFilters();
                    LoadCustomerDisplay();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to add customer.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error adding customer:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // CLEAR BUTTON
        // BUTTON: guna2Button2
        // ==========================================

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // ==========================================
        // CLEAR FIELDS
        // ==========================================

        public void ClearFields()
        {
            customerBranchId = "";

            CM_CustomeridTxt.Clear();
            CM_fullnameTxt.Clear();
            CM_conManTxt.Clear();
            CM_AddressTxt.Clear();
            CM_branchidtxt.Clear();

            CM_AccomTypeTxt.SelectedIndex = -1;
            CM_statustxt.SelectedIndex = -1;

            CM_CustomerDGV.ClearSelection();
        }

        // ==========================================
        // UPDATE CUSTOMER
        // ==========================================

        private void CM_UpdateCustomerBtn_Click(object sender, EventArgs e)
        {
            string customerId =
                CM_CustomeridTxt.Text.Trim();

            string fullname =
                CM_fullnameTxt.Text.Trim();

            string contactnumber =
                CM_conManTxt.Text.Trim();

            string address =
                CM_AddressTxt.Text.Trim();

            string customerType =
                CM_AccomTypeTxt.SelectedItem?.ToString()?.Trim() ?? "";

            string customerStatus =
                CM_statustxt.SelectedItem?.ToString()?.Trim() ?? "";

            // VALIDATE SELECTED CUSTOMER

            if (string.IsNullOrWhiteSpace(customerId) ||
                string.IsNullOrWhiteSpace(customerBranchId))
            {
                MessageBox.Show(
                    "Please select a customer from the table first.",
                    "No Customer Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE REQUIRED FIELDS

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(contactnumber) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(customerType) ||
                string.IsNullOrWhiteSpace(customerStatus))
            {
                MessageBox.Show(
                    "Please complete all customer information.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to update this customer?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    UPDATE CustomerTbl
                    SET
                        Fullname = @Fullname,
                        ContactNumber = @ContactNumber,
                        Address = @Address,
                        CustomerType = @CustomerType,
                        CustomerStatus = @CustomerStatus
                    WHERE
                        CustomerID = @CustomerID
                        AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerID = customerId,
                        BranchId = customerBranchId,
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

                    ApplyCustomerFilters();
                    LoadCustomerDisplay();
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
                    "Error updating customer:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DELETE CUSTOMER
        // ==========================================

        private void CM_DeleteCustomerBtn_Click(object sender, EventArgs e)
        {
            string customerId =
                CM_CustomeridTxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerId) ||
                string.IsNullOrWhiteSpace(customerBranchId))
            {
                MessageBox.Show(
                    "Please select a customer from the table first.",
                    "No Customer Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?\n\nCustomer ID: "
                + customerId,
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                const string query = @"
                    DELETE FROM CustomerTbl
                    WHERE
                        CustomerID = @CustomerID
                        AND BranchId = @BranchId;";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        CustomerID = customerId,
                        BranchId = customerBranchId
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
                    LoadCustomerDisplay();
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

        // ==========================================
        // CUSTOMER STATISTICS
        // ==========================================

        public void LoadCustomerDisplay()
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

                    FROM CustomerTbl;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    new { });

                if (data == null || data.Rows.Count == 0)
                    return;

                DataRow row = data.Rows[0];

                CM_totalcusLbl.Text =
                    Convert.ToInt32(row["TotalCustomers"]).ToString();

                CM_ActiveLbl.Text =
                    Convert.ToInt32(row["ActiveCustomers"]).ToString();

                CM_PendingLbl.Text =
                    Convert.ToInt32(row["PendingCustomers"]).ToString();

                CM_InactiveLbl.Text =
                    Convert.ToInt32(row["InactiveCustomers"]).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading customer statistics:\n" + ex.Message,
                    "Statistics Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // CUSTOMER FILTERING AND SEARCH
        // ==========================================

        private void ApplyCustomerFilters()
        {
            try
            {
                List<string> conditions = new List<string>();

                Dictionary<string, object> parameters =
                    new Dictionary<string, object>();

                // ==========================================
                // STATUS FILTER
                // ==========================================

                string selectedStatus =
                    CM_CustomerstatCB.SelectedItem?.ToString()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(selectedStatus) &&
                    !selectedStatus.StartsWith(
                        "All",
                        StringComparison.OrdinalIgnoreCase))
                {
                    conditions.Add("CustomerStatus = @CustomerStatus");

                    parameters["CustomerStatus"] = selectedStatus;
                }

                // ==========================================
                // CUSTOMER TYPE FILTER
                // ==========================================

                string selectedType =
                    CM_custypeCB.SelectedItem?.ToString()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(selectedType) &&
                    !selectedType.StartsWith(
                        "All",
                        StringComparison.OrdinalIgnoreCase))
                {
                    conditions.Add("CustomerType = @CustomerType");

                    parameters["CustomerType"] = selectedType;
                }

                // ==========================================
                // SEARCH FILTER
                // ==========================================

                string search =
                    Cm_searchLbl.Text.Trim();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    conditions.Add(@"
                        (
                            CustomerID LIKE @Search
                            OR Fullname LIKE @Search
                            OR ContactNumber LIKE @Search
                            OR Address LIKE @Search
                            OR BranchId LIKE @Search
                            OR CustomerType LIKE @Search
                            OR CustomerStatus LIKE @Search
                        )");

                    parameters["Search"] =
                        "%" + search + "%";
                }

                // ==========================================
                // CUSTOMER QUERY
                // ==========================================

                string query = @"
                    SELECT
                        CustomerID,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus,
                        BranchId
                    FROM CustomerTbl
                    " + (conditions.Count > 0
                        ? "WHERE " + string.Join(" AND ", conditions)
                        : "") + @"
                    ORDER BY CustomerID ASC;";

                DataTable data = AppDB.Instance.TableData(
                    query,
                    parameters);

                if (data == null)
                {
                    CM_CustomerDGV.DataSource = null;
                    ClearFields();
                    return;
                }

                // ==========================================
                // DATAGRIDVIEW DISPLAY
                // ==========================================

                CM_CustomerDGV.DataSource = null;
                CM_CustomerDGV.AutoGenerateColumns = true;
                CM_CustomerDGV.DataSource = data;

                // HIDE BRANCH ID

                if (CM_CustomerDGV.Columns.Contains("BranchId"))
                {
                    CM_CustomerDGV.Columns["BranchId"].Visible = false;
                }

                // COLUMN HEADERS

                if (CM_CustomerDGV.Columns.Contains("CustomerID"))
                    CM_CustomerDGV.Columns["CustomerID"].HeaderText = "Customer ID";

                if (CM_CustomerDGV.Columns.Contains("Fullname"))
                    CM_CustomerDGV.Columns["Fullname"].HeaderText = "Full Name";

                if (CM_CustomerDGV.Columns.Contains("ContactNumber"))
                    CM_CustomerDGV.Columns["ContactNumber"].HeaderText = "Contact Number";

                if (CM_CustomerDGV.Columns.Contains("Address"))
                    CM_CustomerDGV.Columns["Address"].HeaderText = "Address";

                if (CM_CustomerDGV.Columns.Contains("CustomerType"))
                    CM_CustomerDGV.Columns["CustomerType"].HeaderText = "Customer Type";

                if (CM_CustomerDGV.Columns.Contains("CustomerStatus"))
                    CM_CustomerDGV.Columns["CustomerStatus"].HeaderText = "Status";

                // ==========================================
                // DISPLAY FIRST CUSTOMER
                // ==========================================

                if (data.Rows.Count > 0 &&
                    CM_CustomerDGV.Rows.Count > 0)
                {
                    CM_CustomerDGV.ClearSelection();

                    DataGridViewRow firstRow =
                        CM_CustomerDGV.Rows[0];

                    firstRow.Selected = true;

                    if (CM_CustomerDGV.Columns.Contains("CustomerID"))
                    {
                        CM_CustomerDGV.CurrentCell =
                            firstRow.Cells["CustomerID"];
                    }
                    else if (CM_CustomerDGV.Columns.Count > 0)
                    {
                        CM_CustomerDGV.CurrentCell =
                            firstRow.Cells[0];
                    }

                    loaddisplay();
                }
                else
                {
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading customers:\n" + ex.Message,
                    "Customer Display Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // OTHER EVENTS
        // ==========================================

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }
    }
}
