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
            Guna2Button BWTC7, Guna2Button BWTC8, Guna2Button BWTC9,
            Guna2Button BWTC10
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
            BWTC10.FillColor = Color.Transparent;
        }


        public void reservationBTN()
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );
        }

        private void UserNavigation_Load(object sender, EventArgs e)
        {

            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlDB DB = new UserControlDB();
                MainControlClass.ShowControl(DB, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void userNav_DashboardBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlDB db = new UserControlDB();
                MainControlClass.ShowControl(db, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void userNav_RoomsBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlRoom rm = new UserControlRoom();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void guna2Button9_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );
            try
            {
                UserControlRoomReserv rs = new UserControlRoomReserv();
                MainControlClass.ShowControl(rs, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void UserNav_PaymentBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );


            try
            {
                UserControlPayment py = new UserControlPayment();
                MainControlClass.ShowControl(py, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_LogoutBtn_Click_2(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
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
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while logging out: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_CustomerBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlCustomers cr = new UserControlCustomers();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void UserNav_TenantsBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlTenants cr = new UserControlTenants();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_MaintenanceBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlMaintenance cr = new UserControlMaintenance();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_ReportsBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlReports cr = new UserControlReports();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_StaffBtn_Click(object sender, EventArgs e)
        {
            designBTN(
                UserNav_ReportsBtn,
                UserNav_TenantsBtn,
                UserNav_CustomerBtn,
                UserNav_LogoutBtn,
                UserNav_PaymentBtn,
                UserNav_ReservationBtn,
                userNav_RoomsBtn,
                userNav_DashboardBtn,
                UserNav_MaintenanceBtn,
                UserNav_StaffBtn
                );

            try
            {
                UserControlStaff cr = new UserControlStaff();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
