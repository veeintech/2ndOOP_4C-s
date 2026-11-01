using Guna.UI2.WinForms;
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
    public partial class UserNavigation : Form
    {
        public UserNavigation()
        {
            InitializeComponent();
        }

        public void designBTN(
            Guna2Button BWC1, Guna2Button BWTC2, Guna2Button BWTC3,
            Guna2Button BWTC4, Guna2Button BWTC5, Guna2Button BWTC6,
            Guna2Button BWTC7, Guna2Button BWTC8, Guna2Button BWTC9
            )
        {
            BWC1.FillColor = Color.FromArgb(212, 167, 44);

            BWTC2.FillColor = Color.Transparent;
            BWTC3.FillColor = Color.Transparent;
            BWTC4.FillColor = Color.Transparent;
            BWTC5.FillColor = Color.Transparent;
            BWTC6.FillColor = Color.Transparent;
            BWTC7.FillColor = Color.Transparent;
            BWTC8.FillColor = Color.Transparent;
            BWTC9.FillColor = Color.Transparent;
        }

        private void UserNavigation_Load(object sender, EventArgs e)
        {
            UserControlDB DB = new UserControlDB();
            MainControlClass.ShowControl(DB, UserNav_Panel_shift);
            designBTN(
                userNav_DashboardBtn,
                userNav_RoomsBtn,
                UserNav_ReservationBtn,
                UserNav_PaymentBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn,
                UserNav_LogoutBtn
                );
        }

        private void userNav_DashboardBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                userNav_DashboardBtn,
                userNav_RoomsBtn,
                UserNav_ReservationBtn,
                UserNav_PaymentBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn,
                UserNav_LogoutBtn
                );

            UserControlDB db = new UserControlDB();
            MainControlClass.ShowControl(db, UserNav_Panel_shift);
        }

        private void userNav_RoomsBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_ReservationBtn,
                UserNav_PaymentBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn,
                UserNav_LogoutBtn
                );

            UserControlRoom rm = new UserControlRoom();
            MainControlClass.ShowControl(rm, UserNav_Panel_shift);
        }

        private void dashboard_Panel_nav_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_PaymentBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn,
                UserNav_LogoutBtn
                );

            UserControlRoomReserv rs = new UserControlRoomReserv();
            MainControlClass.ShowControl(rs, UserNav_Panel_shift);
        }

        private void guna2CustomGradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UserNav_PaymentBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn,
                UserNav_LogoutBtn
                );

            UserControlPayment py = new UserControlPayment();
            MainControlClass.ShowControl(py, UserNav_Panel_shift);
        }

        private void UserNav_LogoutBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn
                );

            FrontPage fp = new FrontPage();
            Form? FrontPage = FindForm();

            if (FrontPage != null)
            {
                fp.FormClosed += (_, _) => FrontPage.Close();
            }

            fp.Show();
            FrontPage?.Hide();
        }

        private void UserNav_LogoutBtn_Click_1(object sender, EventArgs e)
        {
            designBTN(
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn
                );

            FrontPage fp = new FrontPage();
            Form? FrontPage = FindForm();

            if (FrontPage != null)
            {
                fp.FormClosed += (_, _) => FrontPage.Close();
            }

            fp.Show();
            FrontPage?.Hide();
        }

        private void UserNav_LogoutBtn_Click_2(object sender, EventArgs e)
        {
            designBTN(
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_CustomerBtn,
                UserNav_TenantsBtn,
                UserNav_MaintenanceBtn,
                UserNav_ReportsBtn
                );

            FrontPage fp = new FrontPage();
            Form? FrontPage = FindForm();

            if (FrontPage != null)
            {
                fp.FormClosed += (_, _) => FrontPage.Close();
            }

            fp.Show();
            FrontPage?.Hide();
        }
    }
}
