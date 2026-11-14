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
                UserControlRoom rm = new UserControlRoom();
                MainControlClass.ShowControl(rm, UserNav_Panel_shift);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UserNav_Staff_Btn_Click(object sender, EventArgs e)
        {

            // designBTN(
            /*UserNav_Staff_Btn,
            UserNav_TenantsBtn,
            UserNav_CustomerBtn,
            UserNav_LogoutBtn,
            UserNav_PaymentBtn,
            UserNav_ReservationBtn,
            userNav_RoomsBtn,
            userNav_DashboardBtn,
            UserNav_MaintenanceBtn,
            User_Control_ReportsBtn,
            UserNav_AdminBtn
            );

        try
        {
            UserControlStaff cr = new UserControlStaff();
            MainControlClass.ShowControl(cr, UserNav_Panel_shift);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }*/


        }

        private void User_Control_PayrollBtn(object sender, EventArgs e)
        {

        }
    }
}
