
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Branch : UserControl
    {
        public UserControl_Admin_Branch()
        {
            InitializeComponent();

            BR_AddBtn.Click += BR_AddBtn_Click;
            BR_UpdateBtn.Click += BR_UpdateBtn_Click;
            BR_DeleteBtn.Click += BR_DeleteBtn_Click;
            ClearBtn.Click += ClearBtn_Click;

            BR_BranchDGV.CellClick += BR_BranchDGV_CellClick;
            BR_filterCB.SelectedIndexChanged += BR_filterCB_SelectedIndexChanged;
            BR_searchTxt.TextChanged += BR_searchTxt_TextChanged;
        }

        // =====================================================
        // USERCONTROL LOAD
        // =====================================================

        private void UserControl_Admin_Branch_Load(object sender, EventArgs e)
        {
            BR_branchidtxt.ReadOnly = true;

            BR_BranchDGV.AutoGenerateColumns = true;
            BR_BranchDGV.ReadOnly = true;
            BR_BranchDGV.AllowUserToAddRows = false;
            BR_BranchDGV.AllowUserToDeleteRows = false;
            BR_BranchDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            BR_BranchDGV.MultiSelect = false;
            BR_BranchDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            BR_filterCB.Items.Clear();
            BR_filterCB.Items.Add("All");
            BR_filterCB.Items.Add("Active");
            BR_filterCB.Items.Add("Inactive");
            BR_filterCB.SelectedIndex = 0;

            ClearFields();
            LoadBranches();
            LoadBranchSummary();
        }

        // =====================================================
        // LOAD BRANCHES
        // =====================================================

        private void LoadBranches()
        {
            try
            {
                string query = @"
                    SELECT BranchId, BranchName, Address, Status
                    FROM dbo.BranchTbl
                    WHERE
                        (@Search = '' OR BranchId LIKE @Search OR BranchName LIKE @Search)
                        AND (@Status = 'All' OR Status = @Status)
                    ORDER BY BranchId";

                DataTable dt = AppDB.Instance.TableData(query, new
                {
                    Search = "%" + BR_searchTxt.Text.Trim() + "%",
                    Status = string.IsNullOrWhiteSpace(BR_filterCB.Text)
                        ? "All"
                        : BR_filterCB.Text
                });

                BR_BranchDGV.DataSource = dt;
                BR_BranchDGV.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load branches.\n" + ex.Message,
                    "Branch Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // BRANCH SUMMARY
        // =====================================================

        private void LoadBranchSummary()
        {
            try
            {
                string query = @"
                    SELECT
                        COUNT(*) AS TotalBranches,
                        SUM(CASE WHEN Status = 'Active' THEN 1 ELSE 0 END) AS ActiveBranches,
                        SUM(CASE WHEN Status = 'Inactive' THEN 1 ELSE 0 END) AS InactiveBranches
                    FROM dbo.BranchTbl";

                DataTable dt = AppDB.Instance.TableData(query);

                if (dt.Rows.Count > 0)
                {
                    BR_totalLbl.Text =
                        dt.Rows[0]["TotalBranches"].ToString();

                    BR_activeLbl.Text =
                        dt.Rows[0]["ActiveBranches"].ToString();

                    BR_inactiveLbl.Text =
                        dt.Rows[0]["InactiveBranches"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load branch summary.\n" + ex.Message,
                    "Branch Summary Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // GENERATE BRANCH ID
        // =====================================================

        private string GenerateBranchID()
        {
            string query = @"
                SELECT ISNULL(
                    MAX(TRY_CONVERT(INT, SUBSTRING(BranchId, 5, 10))), 0
                ) + 1
                FROM dbo.BranchTbl
                WHERE BranchId LIKE 'BRC-%'";

            int nextNumber =
                Convert.ToInt32(AppDB.Instance.Scalar(query));

            return "BRC-" + nextNumber.ToString("D3");
        }

        // =====================================================
        // ADD BRANCH
        // =====================================================

        private void BR_AddBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BR_branchnametxt.Text) ||
                string.IsNullOrWhiteSpace(BR_addresstxt.Text))
            {
                MessageBox.Show(
                    "Please complete the branch details.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string branchID = GenerateBranchID();

                string query = @"
                    INSERT INTO dbo.BranchTbl
                    (BranchId, BranchName, Address, Status)
                    VALUES
                    (@BranchId, @BranchName, @Address, @Status)";

                int result = AppDB.Instance.CUD(query, new
                {
                    BranchId = branchID,
                    BranchName = BR_branchnametxt.Text.Trim(),
                    Address = BR_addresstxt.Text.Trim(),
                    Status = "Active"
                });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Branch added successfully!\nBranch ID: " + branchID,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    LoadBranches();
                    LoadBranchSummary();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to add branch.\n" + ex.Message,
                    "Add Branch Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // UPDATE BRANCH
        // =====================================================

        private void BR_UpdateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BR_branchidtxt.Text))
            {
                MessageBox.Show(
                    "Please select a branch first.",
                    "No Branch Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BR_branchnametxt.Text) ||
                string.IsNullOrWhiteSpace(BR_addresstxt.Text))
            {
                MessageBox.Show(
                    "Please complete the branch details.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (BR_statustxt.Text != "Active" &&
                BR_statustxt.Text != "Inactive")
            {
                MessageBox.Show(
                    "Status must be Active or Inactive.",
                    "Invalid Status",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = @"
                    UPDATE dbo.BranchTbl
                    SET BranchName = @BranchName,
                        Address = @Address,
                        Status = @Status
                    WHERE BranchId = @BranchId";

                int result = AppDB.Instance.CUD(query, new
                {
                    BranchId = BR_branchidtxt.Text,
                    BranchName = BR_branchnametxt.Text.Trim(),
                    Address = BR_addresstxt.Text.Trim(),
                    Status = BR_statustxt.Text
                });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Branch updated successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    LoadBranches();
                    LoadBranchSummary();
                }
                else
                {
                    MessageBox.Show("No branch record was updated.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update branch.\n" + ex.Message,
                    "Update Branch Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DELETE BRANCH
        // =====================================================

        private void BR_DeleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BR_branchidtxt.Text))
            {
                MessageBox.Show(
                    "Please select a branch first.",
                    "No Branch Selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this branch?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string query = @"
                    DELETE FROM dbo.BranchTbl
                    WHERE BranchId = @BranchId";

                int result = AppDB.Instance.CUD(query, new
                {
                    BranchId = BR_branchidtxt.Text
                });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Branch deleted successfully!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();
                    LoadBranches();
                    LoadBranchSummary();
                }
                else
                {
                    MessageBox.Show("No branch record was deleted.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete branch. It may still have related records.\n" + ex.Message,
                    "Delete Branch Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CLEAR BUTTON
        // BRANCH DETAILS ONLY
        // =====================================================

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            ClearBranchDetails();
        }

        // =====================================================
        // CLEAR BRANCH DETAILS ONLY
        // =====================================================

        private void ClearBranchDetails()
        {
            BR_branchidtxt.Clear();
            BR_branchnametxt.Clear();
            BR_addresstxt.Clear();
            BR_statustxt.Clear();
        }

        // =====================================================
        // DATAGRIDVIEW CELL CLICK
        // =====================================================

        private void BR_BranchDGV_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row =
                BR_BranchDGV.Rows[e.RowIndex];

            BR_branchidtxt.Text =
                row.Cells["BranchId"].Value?.ToString() ?? "";

            BR_branchnametxt.Text =
                row.Cells["BranchName"].Value?.ToString() ?? "";

            BR_addresstxt.Text =
                row.Cells["Address"].Value?.ToString() ?? "";

            BR_statustxt.Text =
                row.Cells["Status"].Value?.ToString() ?? "";
        }

        // =====================================================
        // FILTER BRANCHES
        // =====================================================

        private void BR_filterCB_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            LoadBranches();
        }

        // =====================================================
        // SEARCH BRANCHES
        // =====================================================

        private void BR_searchTxt_TextChanged(
            object sender, EventArgs e)
        {
            LoadBranches();
        }

        // =====================================================
        // CLEAR FIELDS
        // USED BY ADD, UPDATE, DELETE AND LOAD
        // =====================================================

        private void ClearFields()
        {
            ClearBranchDetails();

            BR_BranchDGV.ClearSelection();
        }
    }
}
