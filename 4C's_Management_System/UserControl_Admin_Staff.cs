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
    public partial class UserControl_Admin_Staff : UserControl
    {
        public UserControl_Admin_Staff()
        {
            InitializeComponent();
        }

        private void Request_Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            btnApprove.Visible = true;
            btnReject.Visible = true;
            //btnEdit.Visible = true;
        }

        private void Staff_Directory_Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            btnApprove.Visible = false;
            btnReject.Visible = false;
           // btnEdit.Visible = true;
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {

        }

        private void UserControl_Admin_Staff_Load(object sender, EventArgs e)
        {

        }
    }
}