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
            Bitmap bmp = new Bitmap(
                panelPaymentDetails.Width,
                panelPaymentDetails.Height
            );

            panelPaymentDetails.DrawToBitmap(
                bmp,
                new Rectangle(0, 0, bmp.Width, bmp.Height)
            );

            e.Graphics.DrawImage(bmp, 50, 50);
            bmp.Dispose();
        }
    }
}
