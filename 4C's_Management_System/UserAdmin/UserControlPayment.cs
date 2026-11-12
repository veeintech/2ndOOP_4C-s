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

namespace _4C_s_Management_System
{
    public partial class UserControlPayment : UserControl
    {
        public UserControlPayment()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            printDocument1.PrintPage += PrintDocument1_PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDocument1;
            preview.ShowDialog();

            printDocument1.PrintPage -= PrintDocument1_PrintPage;
        }


        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(
                panelPaymentDetails.Width,
                panelPaymentDetails.Height))
            {
                panelPaymentDetails.DrawToBitmap(
                    bmp,
                    new Rectangle(0, 0, bmp.Width, bmp.Height)
                );

                int newWidth = 750;
                int newHeight = (int)(
                    bmp.Height * ((float)newWidth / bmp.Width)
                );

                e.Graphics.DrawImage(
                    bmp,
                    new Rectangle(50, 50, newWidth, newHeight)
                );
            }
        }


        private void payM_payBTN_Click(object sender, EventArgs e)
        {

            try
            {
                panelPOPUP.Visible = true;
                UserControl_payment_panel paymentPanel = new UserControl_payment_panel();
                MainControlClass.ShowControl(paymentPanel, panelPOPUP);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void UserControlPayment_Load(object sender, EventArgs e)
        {
            panelPOPUP.Visible = false;
        }
    }
}
