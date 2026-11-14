
using _4C_s_Management_System.Classess;
using System;
using System.Data;
using System.Windows.Forms;

namespace _4C_s_Management_System.UserAdmin
{
    public partial class UserControl_Admin_Payment : UserControl
    {
        public UserControl_Admin_Payment()
        {
            InitializeComponent();
        }

        // =====================================================
        // LOAD USERCONTROL
        // =====================================================

        private void UserControl_Admin_Payment_Load(object sender, EventArgs e)
        {
            loadDGV();
            dislplayLabel();
            StatusCB.SelectedIndex = 0;

            if (PaymentDGV.Rows.Count > 0)
            {
                PaymentDGV.ClearSelection();
                PaymentDGV.Rows[0].Selected = true;
                PaymentDGV.CurrentCell = PaymentDGV.Rows[0].Cells[0];

                loaddisplay(PaymentDGV);
            }
        }

        // =====================================================
        // LOAD PAYMENT DATAGRIDVIEW
        // =====================================================

        public void loadDGV()
        {
            try
            {
                const string query = @"
                    SELECT
                        PaymentID,
                        ReservationID,
                        AmountPaid,
                        PaymentDate,
                        PaymentMethod,
                        PaymentStatus
                    FROM PaymentTbl
                    ORDER BY PaymentDate DESC";

                AppDB.Instance.Table(
                    query,
                    new { },
                    PaymentDGV,
                    header: new[]
                    {
                        "Payment ID",
                        "Reservation ID",
                        "Amount",
                        "Date",
                        "Method",
                        "Status"
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading payments: " + ex.Message,
                    "Payment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // PAYMENT SUMMARY LABELS
        // =====================================================

        public void dislplayLabel()
        {
            try
            {
                const string query = @"
                    SELECT
                        PaymentStatus,
                        COUNT(*) AS TotalCount,
                        ISNULL(SUM(AmountPaid), 0) AS TotalAmount
                    FROM PaymentTbl
                    GROUP BY PaymentStatus";

                var statusData = AppDB.Instance.TableData(
                    query,
                    new { });

                int pending = 0;
                int paid = 0;
                int refunded = 0;

                decimal paidAmount = 0;

                foreach (DataRow row in statusData.Rows)
                {
                    string status =
                        Convert.ToString(row["PaymentStatus"]) ?? "";

                    int total =
                        Convert.ToInt32(row["TotalCount"]);

                    decimal amount =
                        Convert.ToDecimal(row["TotalAmount"]);

                    if (status == "Pending")
                    {
                        pending = total;
                    }
                    else if (status == "Paid")
                    {
                        paid = total;
                        paidAmount = amount;
                    }
                    else if (status == "Refunded")
                    {
                        refunded = total;
                    }
                }

                PendingLbl.Text = pending.ToString();
                PaidLbl.Text = paid.ToString();
                RefundedLBL.Text = refunded.ToString();

                TotalPaymentLbl.Text = paidAmount.ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading payment summary: " + ex.Message,
                    "Payment Summary Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // LOAD SELECTED PAYMENT DETAILS
        // =====================================================

        public void loaddisplay(DataGridView dgv)
        {
            try
            {
                if (dgv.CurrentRow == null ||
                    dgv.CurrentRow.IsNewRow)
                    return;

                DataGridViewRow row = dgv.CurrentRow;

                string paymentid =
                    Convert.ToString(row.Cells[0].Value) ?? "";

                if (string.IsNullOrWhiteSpace(paymentid))
                    return;

                PaymentIdLBL.Text = paymentid;

                totalamlbl.Text =
                    row.Cells[2].Value?.ToString() ?? "";

                const string idquery = @"
                    SELECT
                        ReservationID,
                        TenantID,
                        CustomerID,
                        PaymentDate,
                        PaymentMethod,
                        PaymentStatus
                    FROM PaymentTbl
                    WHERE PaymentID = @PaymentID";

                var paymentData = AppDB.Instance.TableData(
                    idquery,
                    new
                    {
                        PaymentID = paymentid
                    });

                if (paymentData.Rows.Count == 0)
                {
                    MessageBox.Show("Payment record not found.");
                    return;
                }

                string tenantid =
                    Convert.ToString(paymentData.Rows[0]["TenantID"]) ?? "";

                string customerid =
                    Convert.ToString(paymentData.Rows[0]["CustomerID"]) ?? "";

                string paymentdate =
                    paymentData.Rows[0]["PaymentDate"] == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(
                        paymentData.Rows[0]["PaymentDate"])
                        .ToString("MM/dd/yyyy");

                string paymentmethod =
                    Convert.ToString(paymentData.Rows[0]["PaymentMethod"]) ?? "";

                string status =
                    Convert.ToString(paymentData.Rows[0]["PaymentStatus"]) ?? "";

                statLBL.Text = status;

                string fullname = "";
                string address = "";
                string contactnumber = "";

                // =========================================
                // CUSTOMER PAYMENT
                // =========================================

                if (!string.IsNullOrWhiteSpace(customerid))
                {
                    const string query = @"
                        SELECT
                            Fullname,
                            Address,
                            ContactNumber
                        FROM CustomerTbl
                        WHERE CustomerID = @CustomerID";

                    var customerData = AppDB.Instance.TableData(
                        query,
                        new
                        {
                            CustomerID = customerid
                        });

                    if (customerData.Rows.Count > 0)
                    {
                        fullname = Convert.ToString(
                            customerData.Rows[0]["Fullname"]) ?? "";

                        address = Convert.ToString(
                            customerData.Rows[0]["Address"]) ?? "";

                        contactnumber = Convert.ToString(
                            customerData.Rows[0]["ContactNumber"]) ?? "";
                    }
                    else
                    {
                        MessageBox.Show("Customer record not found.");
                    }
                }

                // =========================================
                // TENANT PAYMENT
                // =========================================

                else if (!string.IsNullOrWhiteSpace(tenantid))
                {
                    const string query = @"
                        SELECT
                            Fullname,
                            Address,
                            ContactNumber
                        FROM TenantTbl
                        WHERE TenantID = @TenantID";

                    var tenantData = AppDB.Instance.TableData(
                        query,
                        new
                        {
                            TenantID = tenantid
                        });

                    if (tenantData.Rows.Count > 0)
                    {
                        fullname = Convert.ToString(
                            tenantData.Rows[0]["Fullname"]) ?? "";

                        address = Convert.ToString(
                            tenantData.Rows[0]["Address"]) ?? "";

                        contactnumber = Convert.ToString(
                            tenantData.Rows[0]["ContactNumber"]) ?? "";
                    }
                    else
                    {
                        MessageBox.Show("Tenant record not found.");
                    }
                }
                else
                {
                    MessageBox.Show(
                        "No Customer ID or Tenant ID found for this payment.");
                }

                // =========================================
                // DISPLAY PAYMENT DETAILS
                // =========================================

                methodlbl.Text = paymentmethod;
                datelbl.Text = paymentdate;
                PaymentDateLBL.Text = paymentdate;

                fnlbl.Text = fullname;
                addlbl.Text = address;
                cllnumlbl.Text = contactnumber;

                StatusLbl.Text = status;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error displaying payment details: " + ex.Message,
                    "Payment Details Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // DATAGRIDVIEW CELL CLICK
        // =====================================================

        private void paymentDGV_cellConClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            loaddisplay(PaymentDGV);
        }

        // =====================================================
        // SEARCH PAYMENT
        // =====================================================

        private void searchtextChange(object sender, EventArgs e)
        {
            try
            {
                string search = searchTXT.Text.Trim();

                const string query = @"
                    SELECT
                        PaymentID,
                        ReservationID,
                        AmountPaid,
                        PaymentDate,
                        PaymentMethod,
                        PaymentStatus
                    FROM PaymentTbl
                    WHERE
                        PaymentID LIKE @Search
                        OR ISNULL(CustomerID, '') LIKE @Search
                        OR ISNULL(TenantID, '') LIKE @Search
                        OR ISNULL(PaymentStatus, '') LIKE @Search
                        OR ISNULL(PaymentMethod, '') LIKE @Search
                    ORDER BY PaymentDate DESC";

                var data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        Search = "%" + search + "%"
                    });

                PaymentDGV.DataSource = data;

                if (PaymentDGV.Rows.Count > 0)
                {
                    PaymentDGV.ClearSelection();

                    PaymentDGV.Rows[0].Selected = true;

                    PaymentDGV.CurrentCell =
                        PaymentDGV.Rows[0].Cells[0];

                    loaddisplay(PaymentDGV);
                }
                else
                {
                    ClearPaymentDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Search Error: " + ex.Message,
                    "Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // SORT PAYMENT STATUS
        // =====================================================

        public void SortPaymentStatus()
        {
            try
            {
                string status = StatusCB.Text;

                string query = @"
                    SELECT
                        PaymentID,
                        ReservationID,
                        AmountPaid,
                        PaymentDate,
                        PaymentMethod,
                        PaymentStatus
                    FROM PaymentTbl";

                if (!string.IsNullOrWhiteSpace(status) &&
                    status != "All Status")
                {
                    query += " WHERE PaymentStatus = @PaymentStatus";
                }

                query += " ORDER BY PaymentDate DESC";

                var data = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        PaymentStatus = status
                    });

                PaymentDGV.DataSource = data;

                if (PaymentDGV.Rows.Count > 0)
                {
                    PaymentDGV.ClearSelection();

                    PaymentDGV.Rows[0].Selected = true;

                    PaymentDGV.CurrentCell =
                        PaymentDGV.Rows[0].Cells[0];

                    loaddisplay(PaymentDGV);
                }
                else
                {
                    ClearPaymentDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Sorting Error: " + ex.Message,
                    "Payment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CLEAR PAYMENT DETAILS
        // =====================================================

        public void ClearPaymentDetails()
        {
            PM_PaymentIdLBL.Text = "";
            SUM_totalamLbl.Text = "";

            SUM_paymentMethodLBL.Text = "";
            SUM_paymentDateLBL.Text = "";
            PaymentDateLBL.Text = "";

            FullnameLbl.Text = "";
            AdreessLBL.Text = "";
            CallNumLBL.Text = "";

            StatusLbl.Text = "";
            SUM_PaymentLBL.Text = "";
        }

        // =====================================================
        // STATUS COMBOBOX
        // =====================================================

        private void status_SIC(object sender, EventArgs e)
        {
            SortPaymentStatus();
        }
    }
}
