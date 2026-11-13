
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
    public partial class UserControl_Admin_PROFILE : UserControl
    {
        public UserControl_Admin_PROFILE()
        {
            InitializeComponent();

            // DATE AND TIME TIMER
            RM_Timer.Interval = 1000;
            RM_Timer.Tick += RM_Timer_Tick;
        }

        private void UserControl_Admin_PROFILE_Load(object sender, EventArgs e)
        {
            // DATE AND TIME COLORS
            RM_Panel_DateTime.FillColor =
                ColorTranslator.FromHtml("#34465A");

            RM_DateTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");

            RM_TimeTitleLbl.ForeColor =
                ColorTranslator.FromHtml("#8ECAE6");

            RM_DateLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            RM_TimeLbl.ForeColor =
                ColorTranslator.FromHtml("#F8FAFC");

            // INITIAL DATE AND TIME
            UpdateDateTime();

            // START TIMER
            RM_Timer.Start();
        }

        // DATE AND TIME TIMER
        private void RM_Timer_Tick(object sender, EventArgs e)
        {
            UpdateDateTime();
        }

        // UPDATE DATE AND TIME
        private void UpdateDateTime()
        {
            RM_DateLbl.Text = DateTime.Now.ToString("MMM dd, yyyy");
            RM_TimeLbl.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

       
    }
}
