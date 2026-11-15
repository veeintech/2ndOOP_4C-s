
using System;
using System.Data;
using System.Windows.Forms;
using _4C_s_Management_System.Classess;

namespace _4C_s_Management_System
{
    public partial class UserControl_Admin_Customer : UserControl
    {
        public UserControl_Admin_Customer()
        {
            InitializeComponent();
        }

        private void UserControl_Admin_Customer_Load(object sender, EventArgs e)
        {
            loaddgv();
        }

        // =========================================
        // LOAD CUSTOMER DATAGRIDVIEW
        // =========================================

        public void loaddgv()
        {
            try
            {
                const string cusq = @"
                    SELECT
                        CustomerID,
                        BranchId,
                        Fullname,
                        ContactNumber,
                        Address,
                        CustomerType,
                        CustomerStatus
                    FROM CustomerTbl
                    ORDER BY Fullname ASC;";

                AppDB.Instance.Table(
                    cusq,
                    null,
                    CM_CustomerDGV,
                    header: new string[]
                    {
                        "Customer ID",
                        "Branch ID",
                        "Full Name",
                        "Contact Number",
                        "Address",
                        "Customer Type",
                        "Status"
                    }
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading customer records: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
