
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
    public partial class UserControlStaff : UserControl
    {
        private DateTime? timeIn = null;
        private DateTime? timeOut = null;

        private DateTime attendanceDate = DateTime.Today;

        public UserControlStaff()
        {
            InitializeComponent();

            UpdateAttendanceUI();
        }

        private void btnTimeAction_Click(object sender, EventArgs e)
        {
            if (attendanceDate.Date != DateTime.Today)
            {
                attendanceDate = DateTime.Today;
                timeIn = null;
                timeOut = null;
            }

            // FIRST CLICK: TIME IN
            if (timeIn == null)
            {
                timeIn = DateTime.Now;
            }
            // SECOND CLICK: TIME OUT
            else if (timeOut == null)
            {
                timeOut = DateTime.Now;
            }
            // Already completed
            else
            {
                return;
            }

            UpdateAttendanceUI();
        }

        private void UpdateAttendanceUI()
        {
            //display date
            lblDate.Text = "Today · " +
                attendanceDate.ToString("MMM d, yyyy");

            //dili time in
            if (timeIn == null)
            {

                btnTimeAction.Text = "Time in";
                btnTimeAction.Enabled = true;
            }

            //time in
            else if (timeOut == null)
            {
                    timeIn.Value.ToString("h:mm tt");

                btnTimeAction.Text = "Time out";
                btnTimeAction.Enabled = true;
            }

            //time out
            else
            {

                btnTimeAction.Text = "Done for today";
                btnTimeAction.Enabled = false;

                btnTimeAction.Font = new Font("Baskerville Old Face", 8F, FontStyle.Bold);
            }
        }

        private void guna2Panel1_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel1.Location = new Point(
                guna2Panel1.Location.X,
                guna2Panel1.Location.Y - 5
            );

            guna2Panel1.ShadowDecoration.Enabled = true;
            guna2Panel1.ShadowDecoration.Depth = 15;
        }

        private void guna2Panel1_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel1.Location = new Point(
                guna2Panel1.Location.X,
                guna2Panel1.Location.Y + 5
            );

            guna2Panel1.ShadowDecoration.Depth = 0;
        }

        private void guna2Panel3_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel3.Location = new Point(
                guna2Panel3.Location.X,
                guna2Panel3.Location.Y - 5
            );

            guna2Panel3.ShadowDecoration.Enabled = true;
            guna2Panel3.ShadowDecoration.Depth = 15;
        }

        private void guna2Panel3_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel3.Location = new Point(
                guna2Panel3.Location.X,
                guna2Panel3.Location.Y + 5
            );

            guna2Panel3.ShadowDecoration.Depth = 0;
        }

        private void guna2Panel6_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel6.Location = new Point(
                guna2Panel6.Location.X,
                guna2Panel6.Location.Y - 5
            );

            guna2Panel6.ShadowDecoration.Enabled = true;
            guna2Panel6.ShadowDecoration.Depth = 15;
        }

        private void guna2Panel6_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel6.Location = new Point(
                guna2Panel6.Location.X,
                guna2Panel6.Location.Y + 5
            );

            guna2Panel6.ShadowDecoration.Depth = 0;
        }

        private void guna2Panel7_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel7.Location = new Point(
                guna2Panel7.Location.X,
                guna2Panel7.Location.Y - 5
            );

            guna2Panel7.ShadowDecoration.Enabled = true;
            guna2Panel7.ShadowDecoration.Depth = 15;
        }

        private void guna2Panel7_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel7.Location = new Point(
                guna2Panel7.Location.X,
                guna2Panel7.Location.Y + 5
            );

            guna2Panel7.ShadowDecoration.Depth = 0;
        }
        private void guna2Panel8_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel8.Location = new Point(
                guna2Panel8.Location.X,
                guna2Panel8.Location.Y - 5
            );

            guna2Panel8.ShadowDecoration.Enabled = true;
            guna2Panel8.ShadowDecoration.Depth = 15;
        }

        private void guna2Panel8_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel8.Location = new Point(
                guna2Panel8.Location.X,
                guna2Panel8.Location.Y + 5
            );

            guna2Panel8.ShadowDecoration.Depth = 0;
        }
    }
}
