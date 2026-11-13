
using _4C_s_Management_System.Classess;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_payment_panel : UserControl
    {
        public string paymentMet = "Cash";
        public double totalam = 0;

        public UserControl_payment_panel()
        {
            InitializeComponent();

            // Default payment method
            paymentMet = "Cash";
            PaymentMethod.Text = "Cash Payment";

            // Calculate change whenever the received amount changes
            PD_CashRecievedTXT.TextChanged += PD_CashRecievedTXT_TextChanged;
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        // CASH PAYMENT
        private void PD_CashBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_GCashBTN.FillColor = Color.White;
            PD_BankransferBTN.FillColor = Color.White;
            PD_MayaBTN.FillColor = Color.White;

            paymentMet = "Cash";
            PaymentMethod.Text = "Cash Payment";

            refNumtxt.ReadOnly = true;
            refNumtxt.Text = "";

            CalculateChange();
        }

        // GCASH PAYMENT
        private void PD_GCashBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.White;
            PD_GCashBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_BankransferBTN.FillColor = Color.White;
            PD_MayaBTN.FillColor = Color.White;

            paymentMet = "GCash";
            PaymentMethod.Text = "GCash Payment";

            refNumtxt.ReadOnly = false;
            PD_ChangeTXT.Text = "0.00";
        }

        // MAYA PAYMENT
        private void PD_MayaBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.White;
            PD_GCashBTN.FillColor = Color.White;
            PD_BankransferBTN.FillColor = Color.White;
            PD_MayaBTN.FillColor = Color.FromArgb(255, 218, 88);

            paymentMet = "Maya";
            PaymentMethod.Text = "Maya Payment";

            refNumtxt.ReadOnly = false;
            PD_ChangeTXT.Text = "0.00";
        }

        // BANK TRANSFER PAYMENT
        private void PD_BankransferBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.White;
            PD_GCashBTN.FillColor = Color.White;
            PD_BankransferBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_MayaBTN.FillColor = Color.White;

            paymentMet = "Bank Transfer";
            PaymentMethod.Text = "Bank Transfer Payment";

            refNumtxt.ReadOnly = false;
            PD_ChangeTXT.Text = "0.00";
        }

        // CANCEL PAYMENT
        private void PD_cancelBTN_Click(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                this.Parent.Visible = false;
            }
        }

        // LOAD EVENT
        private void UserControl_payment_panel_Load(object sender, EventArgs e)
        {
            // Prevent database operations in Visual Studio Designer
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                DesignMode)
            {
                return;
            }

            loadlabel();
        }

        // LOAD PAYMENT AMOUNT
        public void loadlabel()
        {
            try
            {
                string pid = AccessBranch_ByForm.PaymentId;

                if (string.IsNullOrWhiteSpace(pid))
                {
                    PD_AmountLBL.Text = "0.00";
                    MessageBox.Show("Payment ID is missing.");
                    return;
                }

                const string tot =
                    "SELECT AmountPaid " +
                    "FROM PaymentTbl " +
                    "WHERE PaymentID = @PaymentID";

                object result = AppDB.Instance.Scalar(
                    tot,
                    new
                    {
                        PaymentID = pid
                    });

                if (result != null && result != DBNull.Value)
                {
                    totalam = Convert.ToDouble(result);

                    PD_AmountLBL.Text = totalam.ToString("N2");
                }
                else
                {
                    totalam = 0;
                    PD_AmountLBL.Text = "0.00";

                    MessageBox.Show("Payment record not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load payment details: " + ex.Message);
            }
        }

        // AUTOMATIC CHANGE CALCULATION
        private void PD_CashRecievedTXT_TextChanged(object sender, EventArgs e)
        {
            CalculateChange();
        }

        private void CalculateChange()
        {
            decimal received;
            decimal amountDue;

            if (!decimal.TryParse(PD_CashRecievedTXT.Text, out received))
            {
                PD_ChangeTXT.Text = "0.00";
                return;
            }

            if (!decimal.TryParse(PD_AmountLBL.Text, out amountDue))
            {
                PD_ChangeTXT.Text = "0.00";
                return;
            }

            if (paymentMet == "Cash")
            {
                decimal change = received - amountDue;

                if (change >= 0)
                {
                    PD_ChangeTXT.Text = change.ToString("N2");
                }
                else
                {
                    PD_ChangeTXT.Text = "0.00";
                }
            }
            else
            {
                PD_ChangeTXT.Text = "0.00";
            }
        }

        // CONFIRM PAYMENT
        private void PD_ConfirmPaymenBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // Check payment method
                if (string.IsNullOrWhiteSpace(paymentMet))
                {
                    MessageBox.Show("Please select a payment method.");
                    return;
                }

                // Check received amount
                decimal totpaid;

                if (!decimal.TryParse(PD_CashRecievedTXT.Text, out totpaid))
                {
                    MessageBox.Show("Please enter a valid payment amount.");
                    PD_CashRecievedTXT.Focus();
                    return;
                }

                if (totpaid <= 0)
                {
                    MessageBox.Show("Payment amount must be greater than zero.");
                    PD_CashRecievedTXT.Focus();
                    return;
                }

                // Get total amount due
                decimal totalAmount;

                if (!decimal.TryParse(PD_AmountLBL.Text, out totalAmount) ||
                    totalAmount <= 0)
                {
                    MessageBox.Show("Invalid payment amount.");
                    return;
                }

                // Validate payment amount
                if (totpaid < totalAmount)
                {
                    MessageBox.Show(
                        "Insufficient payment amount.\n" +
                        "Please enter the full amount.");

                    PD_CashRecievedTXT.Focus();
                    return;
                }

                // Get reference number
                string refn = refNumtxt.Text.Trim();

                // Reference number is required for online payments
                if (paymentMet != "Cash" && string.IsNullOrWhiteSpace(refn))
                {
                    MessageBox.Show("Please enter the reference number.");
                    refNumtxt.Focus();
                    return;
                }

                // Calculate change
                decimal change = 0;

                if (paymentMet == "Cash")
                {
                    change = totpaid - totalAmount;
                }

                PD_ChangeTXT.Text = change.ToString("N2");

                // Actual amount applied to the payment
                decimal amountPaid = totalAmount;

                // Remaining balance
                decimal balance = totalAmount - amountPaid;

                // Payment status
                string paymentStatus = balance <= 0 ? "Paid" : "Pending";

                // Payment date
                DateTime dateNom = DateTime.Now;

                // Payment ID
                string pid = AccessBranch_ByForm.PaymentId;

                if (string.IsNullOrWhiteSpace(pid))
                {
                    MessageBox.Show("Payment ID is missing.");
                    return;
                }

                // Update PaymentTbl
                const string paymentq =
                    "UPDATE PaymentTbl SET " +
                    "AmountPaid = @AmountPaid, " +
                    "PaymentDate = @datenow, " +
                    "Balance = @Balance, " +
                    "PaymentMethod = @PaymentMethod, " +
                    "ReferenceNumber = @ReferenceNumber, " +
                    "PaymentStatus = @PaymentStatus " +
                    "WHERE PaymentID = @PaymentID";

                int result = AppDB.Instance.CUD(
                    paymentq,
                    new
                    {
                        AmountPaid = amountPaid,
                        datenow = dateNom,
                        Balance = balance,
                        PaymentMethod = paymentMet,
                        ReferenceNumber = refn,
                        PaymentStatus = paymentStatus,
                        PaymentID = pid
                    });

                if (result > 0)
                {
                    MessageBox.Show(
                        "Payment successfully processed!",
                        "Payment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    if (this.Parent != null)
                    {
                        this.Parent.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show("Payment failed. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UserControl_payment_panel_Load_1(object sender, EventArgs e)
        {
            // Prevent database operations in Visual Studio Designer
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                DesignMode)
            {
                return;
            }

            loadlabel();
        }
    }
}
