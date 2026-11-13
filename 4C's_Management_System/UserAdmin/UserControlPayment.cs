using _4C_s_Management_System.Classess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace _4C_s_Management_System
{
    public partial class UserControlPayment : UserControl
    {
        PrintDocument printDocument = new PrintDocument();
        public UserControlPayment()
        {
            InitializeComponent();
            printDocument.PrintPage += printDocument_PrintPage;
        }


        public string branchid = AccessBranch_ByForm.BranchId;

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PM_PaymentIdLBL.Text))
            {
                MessageBox.Show("Please select a payment first.");
                return;
            }

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = printDocument;
                preview.Width = 900;
                preview.Height = 700;

                preview.ShowDialog();
            }
        }

        //print 



        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // FONTS
            using (Font titleFont = new Font("Arial", 18, FontStyle.Bold))
            using (Font headerFont = new Font("Arial", 11, FontStyle.Bold))
            using (Font normalFont = new Font("Arial", 10, FontStyle.Regular))
            using (Font smallFont = new Font("Arial", 8, FontStyle.Regular))
            using (Font totalFont = new Font("Arial", 13, FontStyle.Bold))
            using (Pen linePen = new Pen(Color.Black, 1))
            using (StringFormat center = new StringFormat())
            {
                // CENTER RECEIPT
                float contentWidth = 450;

                float left = e.MarginBounds.Left +
                    (e.MarginBounds.Width - contentWidth) / 2f;

                float right = left + contentWidth;
                float y = e.MarginBounds.Top + 30;
                float width = contentWidth;

                float valueX = left + 135;
                float totalX = left + 270;

                center.Alignment = StringAlignment.Center;

                // HEADER
                g.DrawString(
                    "4C's MANAGEMENT SYSTEM",
                    titleFont,
                    Brushes.Black,
                    new RectangleF(left, y, width, 35),
                    center);

                y += 35;

                g.DrawString(
                    "PAYMENT RECEIPT",
                    headerFont,
                    Brushes.Black,
                    new RectangleF(left, y, width, 25),
                    center);

                y += 40;

                g.DrawLine(linePen, left, y, right, y);
                y += 15;

                // PAYMENT INFORMATION
                g.DrawString("PAYMENT INFORMATION", headerFont,
                    Brushes.Black, left, y);

                y += 30;

                g.DrawString("Payment ID:", normalFont, Brushes.Black, left, y);
                g.DrawString(PM_PaymentIdLBL.Text, normalFont, Brushes.Black, valueX, y);

                y += 25;

                g.DrawString("Payment Date:", normalFont, Brushes.Black, left, y);
                g.DrawString(PaymentDateLBL.Text, normalFont, Brushes.Black, valueX, y);

                y += 25;

                g.DrawString("Payment Method:", normalFont, Brushes.Black, left, y);
                g.DrawString(SUM_paymentMethodLBL.Text, normalFont, Brushes.Black, valueX, y);

                y += 25;

                g.DrawString("Payment Status:", normalFont, Brushes.Black, left, y);
                g.DrawString(StatusLbl.Text, normalFont, Brushes.Black, valueX, y);

                y += 35;

                g.DrawLine(linePen, left, y, right, y);
                y += 15;

                // CUSTOMER INFORMATION
                g.DrawString("CUSTOMER INFORMATION", headerFont,
                    Brushes.Black, left, y);

                y += 30;

                g.DrawString("Full Name:", normalFont, Brushes.Black, left, y);
                g.DrawString(FullnameLbl.Text, normalFont, Brushes.Black, valueX, y);

                y += 25;

                g.DrawString("Address:", normalFont, Brushes.Black, left, y);
                g.DrawString(AdreessLBL.Text, normalFont, Brushes.Black, valueX, y);

                y += 25;

                g.DrawString("Contact Number:", normalFont, Brushes.Black, left, y);
                g.DrawString(CallNumLBL.Text, normalFont, Brushes.Black, valueX, y);

                y += 35;

                g.DrawLine(linePen, left, y, right, y);
                y += 20;

                // TOTAL PAYMENT
                g.DrawString("TOTAL AMOUNT:", headerFont,
                    Brushes.Black, left, y);

                g.DrawString(
                    SUM_totalamLbl.Text,
                    totalFont,
                    Brushes.Black,
                    totalX,
                    y);

                y += 45;

                g.DrawLine(linePen, left, y, right, y);
                y += 20;

                // FOOTER
                g.DrawString(
                    "Thank you for your payment!",
                    headerFont,
                    Brushes.Black,
                    new RectangleF(left, y, width, 25),
                    center);

                y += 30;

                g.DrawString(
                    "This is a computer-generated receipt.",
                    smallFont,
                    Brushes.Black,
                    new RectangleF(left, y, width, 20),
                    center);
            }

            e.HasMorePages = false;
        }




        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PM_PaymentIdLBL.Text))
            {
                MessageBox.Show("Please select a payment first.");
                return;
            }

            using (PrintDialog dialog = new PrintDialog())
            {
                dialog.Document = printDocument;
                dialog.AllowSomePages = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Pen pen = new Pen(Color.Goldenrod, 3))
            {
                e.Graphics.DrawRectangle(
                    pen,
                    1,
                    1,
                    this.Width - 3,
                    this.Height - 3
                );
            }
        }

        private void payM_payBTN_Click(object sender, EventArgs e)
        {
            try
            {
                string id = PM_PaymentIdLBL.Text.Trim();

                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("Please select a payment first.");
                    return;
                }

                AccessBranch_ByForm.SetPayment(id);

                // Configure shell border
                shift_panel.BorderThickness = 3;
                shift_panel.BorderColor = Color.Goldenrod;
                shift_panel.Padding = new Padding(3);

                // Show shell
                shift_panel.Visible = true;
                shift_panel.BringToFront();

                // Load UserControl inside shell
                UserControl_payment_panel paymentControl =
                    new UserControl_payment_panel();

                MainControlClass.ShowControl(paymentControl, shift_panel);

                // Refresh shell
                shift_panel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void HidePaymentShell()
        {
            shift_panel.Visible = false;
        }

        private void UserControlPayment_Load(object sender, EventArgs e)
        {
            shift_panel.Visible = false;
            loadDGV();
            loaddisplay();
            dislplayLabel();
        }

        public void loadDGV()
        {
            try
            {
                const string query =
                       "SELECT PaymentID, ReservationID, AmountPaid, PaymentDate, " +
                       "PaymentMethod, PaymentStatus " +
                       "FROM PaymentTbl " +
                       "WHERE BranchID = @BranchId " +
                       "ORDER BY PaymentDate DESC";

                AppDB.Instance.Table(
                    query,
                    new { BranchId = branchid },
                    PaymentDGV,
                    header: new[]
                                {
                        "Payment ID",
                        "Reservation ID",
                        "Amount",
                        "Date",
                        "Method",
                        "Status",
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        public void dislplayLabel()
        {
            const string query =
                "SELECT PaymentStatus, " +
                "COUNT(*) AS TotalCount, " +
                "ISNULL(SUM(AmountPaid), 0) AS TotalAmount " +
                "FROM PaymentTbl " +
                "GROUP BY PaymentStatus";

            var statusData = AppDB.Instance.TableData(query, new { });

            int pending = 0;
            int paid = 0;
            int refunded = 0;

            decimal pendingAmount = 0;
            decimal paidAmount = 0;
            decimal refundedAmount = 0;

            foreach (DataRow row in statusData.Rows)
            {
                string status = Convert.ToString(row["PaymentStatus"]) ?? "";

                int total = Convert.ToInt32(row["TotalCount"]);

                decimal amount = Convert.ToDecimal(row["TotalAmount"]);

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

            // PAYMENT COUNTS
            PendingLbl.Text = pending.ToString();
            PaidLbl.Text = paid.ToString();
            RefundedLBL.Text = refunded.ToString();

            // PAYMENT AMOUNTS
            TotalPaymentLbl.Text = paidAmount.ToString("C2");
        }


        public void loaddisplay()
        {
            if (PaymentDGV.CurrentRow == null)
                return;

            DataGridViewRow row = PaymentDGV.CurrentRow;

            string paymentid = Convert.ToString(row.Cells[0].Value) ?? "";

            if (string.IsNullOrWhiteSpace(paymentid))
                return;

            PM_PaymentIdLBL.Text = paymentid;
            SUM_totalamLbl.Text = row.Cells[2].Value?.ToString() ?? "";
            SUM_PaymentLBL.Text = row.Cells[4].Value?.ToString() ?? "";


            const string idquery =
            "SELECT ReservationID, TenantID, CustomerID, PaymentDate, PaymentMethod, PaymentStatus " +
            "FROM PaymentTbl " +
            "WHERE PaymentID = @PaymentID";

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

            string reservationid = paymentData.Rows[0]["ReservationID"] == DBNull.Value
                ? ""
                : paymentData.Rows[0]["ReservationID"].ToString();

            string tenantid = paymentData.Rows[0]["TenantID"] == DBNull.Value
                ? ""
                : paymentData.Rows[0]["TenantID"].ToString();

            string customerid = paymentData.Rows[0]["CustomerID"] == DBNull.Value
                ? ""
                : paymentData.Rows[0]["CustomerID"].ToString();

            string paymentdate = paymentData.Rows[0]["PaymentDate"] == DBNull.Value
                ? ""
                : Convert.ToDateTime(paymentData.Rows[0]["PaymentDate"]).ToString("MM/dd/yyyy");

            string paymentmethod = paymentData.Rows[0]["PaymentMethod"] == DBNull.Value
                ? ""
                : paymentData.Rows[0]["PaymentMethod"].ToString();

            string status = paymentData.Rows[0]["PaymentStatus"] == DBNull.Value
                ? ""
                : paymentData.Rows[0]["PaymentStatus"].ToString();

            StatusLbl.Text = status;

            string fullname = "";
            string address = "";
            string contactnumber = "";

            // CUSTOMER PAYMENT
            if (!string.IsNullOrWhiteSpace(customerid))
            {
                const string query =
                    "SELECT Fullname, Address, ContactNumber " +
                    "FROM CustomerTbl " +
                    "WHERE CustomerID = @CustomerID";

                var customerData = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        CustomerID = customerid
                    });

                if (customerData.Rows.Count > 0)
                {
                    fullname = Convert.ToString(customerData.Rows[0]["Fullname"]) ?? "";
                    address = Convert.ToString(customerData.Rows[0]["Address"]) ?? "";
                    contactnumber = Convert.ToString(customerData.Rows[0]["ContactNumber"]) ?? "";
                }
                else
                {
                    MessageBox.Show("Customer record not found.");
                }
            }

            // TENANT PAYMENT
            else if (!string.IsNullOrWhiteSpace(tenantid))
            {
                const string query =
                    "SELECT Fullname, Address, ContactNumber " +
                    "FROM TenantTbl " +
                    "WHERE TenantID = @TenantID";

                var tenantData = AppDB.Instance.TableData(
                    query,
                    new
                    {
                        TenantID = tenantid
                    });

                if (tenantData.Rows.Count > 0)
                {
                    fullname = Convert.ToString(tenantData.Rows[0]["Fullname"]) ?? "";
                    address = Convert.ToString(tenantData.Rows[0]["Address"]) ?? "";
                    contactnumber = Convert.ToString(tenantData.Rows[0]["ContactNumber"]) ?? "";
                }
                else
                {
                    MessageBox.Show("Tenant record not found.");
                }
            }
            else
            {
                MessageBox.Show("No Customer ID or Tenant ID found for this payment.");
            }

            // DISPLAY CUSTOMER OR TENANT INFORMATION

            SUM_paymentMethodLBL.Text = paymentmethod;
            SUM_paymentDateLBL.Text = paymentdate;
            PaymentDateLBL.Text = paymentdate;
            FullnameLbl.Text = fullname;
            AdreessLBL.Text = address;
            CallNumLBL.Text = contactnumber;
            SUM_PaymentLBL.Text = status;
        }

        // PAYMENT DATAGRIDVIEW CELL CLICK
        private void PaymentDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            loaddisplay();
        }

        private void PM_refundBTN_Click(object sender, EventArgs e)
        {
            string paymentid = PM_PaymentIdLBL.Text.Trim();
            string status = StatusLbl.Text.Trim();

            if (string.IsNullOrWhiteSpace(paymentid))
            {
                MessageBox.Show("Please select a payment first.");
                return;
            }

            if (status != "Paid")
            {
                MessageBox.Show("Only paid transactions can be refunded.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to refund this payment?",
                "Confirm Refund",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                const string query =
                    "UPDATE PaymentTbl " +
                    "SET PaymentStatus = @PaymentStatus " +
                    "WHERE PaymentID = @PaymentID " +
                    "AND BranchID = @BranchID " +
                    "AND PaymentStatus = 'Paid'";

                int result = AppDB.Instance.CUD(
                    query,
                    new
                    {
                        PaymentStatus = "Refunded",
                        PaymentID = paymentid,
                        BranchID = branchid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Payment successfully refunded!",
                        "Refund",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    StatusLbl.Text = "Refunded";

                    loadDGV(); // Replace with your actual payment DataGridView loading method.

                    dislplayLabel();
                }
                else
                {
                    MessageBox.Show("Refund failed. Please check the payment record.");
                }
            }
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            string search = searchTXT.Text.Trim();

            const string query =
                "SELECT * FROM PaymentTbl " +
                "WHERE BranchID = @BranchID " +
                "AND (" +
                "PaymentID LIKE @Search OR " +
                "ISNULL(CustomerID, '') LIKE @Search OR " +
                "ISNULL(TenantID, '') LIKE @Search OR " +
                "PaymentStatus LIKE @Search OR " +
                "PaymentMethod LIKE @Search" +
                ")";

            var data = AppDB.Instance.TableData(
                query,
                new
                {
                    BranchID = branchid,
                    Search = "%" + search + "%"
                });

            PaymentDGV.DataSource = data;
        }

        public void SortPaymentStatus()
        {
            string status = StatusCB.Text;

            string query =
                "SELECT * FROM PaymentTbl " +
                "WHERE BranchID = @BranchID ";

            if (status != "All")
            {
                query += "AND PaymentStatus = @PaymentStatus ";
            }

            query += "ORDER BY PaymentDate DESC";

            var data = AppDB.Instance.TableData(
                query,
                new
                {
                    BranchID = branchid,
                    PaymentStatus = status
                });

            PaymentDGV.DataSource = data;
        }

        private void Status_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortPaymentStatus();
        }
    }
}
