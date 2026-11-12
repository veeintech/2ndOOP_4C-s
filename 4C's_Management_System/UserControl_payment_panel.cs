using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4C_s_Management_System
{
    public partial class UserControl_payment_panel : UserControl
    {
        public UserControl_payment_panel()
        {
            InitializeComponent();
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void PD_CashBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_GCashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_BankransferBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_MayaBTN.FillColor = Color.FromArgb(255, 255, 255);
            Panel_CashPayment.Visible = true;
            Panel_Gcash_Payment.Visible = false;
            Panel_BankTransPayment.Visible = false;
            Panel_MayaPayment.Visible = false;
        }

        private void PD_GCashBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_GCashBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_BankransferBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_MayaBTN.FillColor = Color.FromArgb(255, 255, 255);
            Panel_CashPayment.Visible = false;
            Panel_Gcash_Payment.Visible = true;
            Panel_BankTransPayment.Visible = false;
            Panel_MayaPayment.Visible = false;
        }

        private void PD_MayaBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_GCashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_BankransferBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_MayaBTN.FillColor = Color.FromArgb(255, 218, 88);
            Panel_CashPayment.Visible = false;
            Panel_Gcash_Payment.Visible = false;
            Panel_BankTransPayment.Visible = false;
            Panel_MayaPayment.Visible = true;
        }

        private void PD_BankransferBTN_Click(object sender, EventArgs e)
        {
            PD_CashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_GCashBTN.FillColor = Color.FromArgb(255, 255, 255);
            PD_BankransferBTN.FillColor = Color.FromArgb(255, 218, 88);
            PD_MayaBTN.FillColor = Color.FromArgb(255, 255, 255);
            Panel_CashPayment.Visible = false;
            Panel_Gcash_Payment.Visible = false;
            Panel_BankTransPayment.Visible = true;
            Panel_MayaPayment.Visible = false;
        }
    }
}
