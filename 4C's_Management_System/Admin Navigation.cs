using _4C_s_Management_System.UserAdmin;
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
    public partial class Admin_Navigation : Form
    {
        public Admin_Navigation()
        {
            InitializeComponent();
        }

        public void designBTN(
          Guna2Button BWC1, Guna2Button BWTC2, Guna2Button BWTC3,
          Guna2Button BWTC4, Guna2Button BWTC5, Guna2Button BWTC6,
          Guna2Button BWTC7, Guna2Button BWTC8/*, Guna2Button BWTC9,
          Guna2Button BWTC10, Guna2Button BWTC11*/
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
            //BWTC9.FillColor = Color.Transparent;
            //BWTC10.FillColor = Color.Transparent;
            //BWTC11.FillColor = Color.Transparent;

        }

        private void UserNav_Panel_shift_Paint(object sender, PaintEventArgs e)
        {

        }


        private void userNav_RoomsBtn_Click_1(object sender, EventArgs e)
        {
            designBTN(
              userNav_RoomsBtn,
              //UserNav_Staff_Btn,
              UserNav_TenantsBtn,
              UserNav_CustomerBtn,
              UserNav_LogoutBtn,
              UserNav_PaymentBtn,
              UserNav_ReservationBtn,
              userNav_DashboardBtn,
              UserNav_MaintenanceBtn
              //User_Control_ReportsBtn,
              //UserNav_AdminBtn
              );

            try
            {
                UserControl_Admin_Room rm = new UserControl_Admin_Room();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_Staff_Btn_Click(object sender, EventArgs e)
        {
            designBTN(
           UserNav_Staff_Btn,
           UserNav_TenantsBtn,
           UserNav_CustomerBtn,
           //UserNav_LogoutBtn,
           UserNav_PaymentBtn,
           UserNav_ReservationBtn,
           userNav_RoomsBtn,
           userNav_DashboardBtn,
           UserNav_MaintenanceBtn
           //User_Control_ReportsBtn
           //UserNav_AdminBtn
           );

            try
            {
                UserControl_Admin_Staff cr = new UserControl_Admin_Staff();
                MainControlClass.ShowControl(cr, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void userNav_DashboardBtn_Click(object sender, EventArgs e)
        {
            designBTN(
              userNav_DashboardBtn,
              userNav_RoomsBtn,
              //UserNav_Staff_Btn,
              UserNav_TenantsBtn,
              UserNav_CustomerBtn,
              UserNav_LogoutBtn,
              UserNav_PaymentBtn,
              UserNav_ReservationBtn,
              UserNav_MaintenanceBtn
              //User_Control_ReportsBtn,
              //UserNav_AdminBtn
              );

            try
            {
                UserControl_Admin_DB rm = new UserControl_Admin_DB();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_ReservationBtn_Click(object sender, EventArgs e)
        {
            designBTN(
              UserNav_ReservationBtn,
              userNav_DashboardBtn,
              userNav_RoomsBtn,
              //UserNav_Staff_Btn,
              UserNav_TenantsBtn,
              UserNav_CustomerBtn,
              UserNav_LogoutBtn,
              UserNav_PaymentBtn,
              UserNav_MaintenanceBtn
              //User_Control_ReportsBtn,
              //UserNav_AdminBtn
              );

            try
            {
                UserControl_Admin_Reservations rm = new UserControl_Admin_Reservations();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_PaymentBtn_Click(object sender, EventArgs e)
        {
            designBTN(
              UserNav_PaymentBtn,
              UserNav_ReservationBtn,
              userNav_DashboardBtn,
              userNav_RoomsBtn,
              //UserNav_Staff_Btn,
              UserNav_TenantsBtn,
              UserNav_CustomerBtn,
              UserNav_LogoutBtn,
              UserNav_MaintenanceBtn
              //User_Control_ReportsBtn,
              //UserNav_AdminBtn
              );

            try
            {
                UserControl_Admin_Payment rm = new UserControl_Admin_Payment();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_TenantsBtn_Click(object sender, EventArgs e)
        {
            designBTN(
             UserNav_TenantsBtn,
             UserNav_PaymentBtn,
             UserNav_ReservationBtn,
             userNav_DashboardBtn,
             userNav_RoomsBtn,
             //UserNav_Staff_Btn,
             UserNav_CustomerBtn,
             UserNav_LogoutBtn,
             UserNav_MaintenanceBtn
             //User_Control_ReportsBtn,
             //UserNav_AdminBtn
             );

            try
            {
                UserControl_Admin_Tenants rm = new UserControl_Admin_Tenants();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_MaintenanceBtn_Click(object sender, EventArgs e)
        {
            designBTN(
             UserNav_MaintenanceBtn,
             UserNav_TenantsBtn,
             UserNav_PaymentBtn,
             UserNav_ReservationBtn,
             userNav_DashboardBtn,
             userNav_RoomsBtn,
             //UserNav_Staff_Btn,
             UserNav_CustomerBtn,
             UserNav_LogoutBtn
             //User_Control_ReportsBtn,
             //UserNav_AdminBtn
             );

            try
            {
                UserControl_Admin_Maintenance rm = new UserControl_Admin_Maintenance();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
