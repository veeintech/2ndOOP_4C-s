namespace _4C_s_Management_System
{
    partial class DashboardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            dashboard_Panel_nav = new Guna.UI2.WinForms.Guna2Panel();
            dashboard_panel_data = new Guna.UI2.WinForms.Guna2Panel();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            dashboard_panel_data.SuspendLayout();
            SuspendLayout();
            // 
            // dashboard_Panel_nav
            // 
            dashboard_Panel_nav.CustomizableEdges = customizableEdges1;
            dashboard_Panel_nav.Dock = DockStyle.Left;
            dashboard_Panel_nav.FillColor = Color.FromArgb(128, 0, 32);
            dashboard_Panel_nav.Location = new Point(0, 0);
            dashboard_Panel_nav.Name = "dashboard_Panel_nav";
            dashboard_Panel_nav.ShadowDecoration.CustomizableEdges = customizableEdges2;
            dashboard_Panel_nav.Size = new Size(250, 713);
            dashboard_Panel_nav.TabIndex = 0;
            // 
            // dashboard_panel_data
            // 
            dashboard_panel_data.Controls.Add(guna2Panel1);
            dashboard_panel_data.CustomizableEdges = customizableEdges5;
            dashboard_panel_data.Dock = DockStyle.Fill;
            dashboard_panel_data.FillColor = Color.FromArgb(255, 248, 231);
            dashboard_panel_data.Location = new Point(250, 0);
            dashboard_panel_data.Name = "dashboard_panel_data";
            dashboard_panel_data.ShadowDecoration.CustomizableEdges = customizableEdges6;
            dashboard_panel_data.Size = new Size(932, 713);
            dashboard_panel_data.TabIndex = 1;
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.Transparent;
            guna2Panel1.BorderColor = Color.FromArgb(58, 41, 37);
            guna2Panel1.BorderRadius = 15;
            guna2Panel1.BorderThickness = 2;
            guna2Panel1.CustomizableEdges = customizableEdges3;
            guna2Panel1.Location = new Point(38, 12);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2Panel1.Size = new Size(188, 139);
            guna2Panel1.TabIndex = 0;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 713);
            Controls.Add(dashboard_panel_data);
            Controls.Add(dashboard_Panel_nav);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "4C's Management System";
            dashboard_panel_data.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel dashboard_Panel_nav;
        private Guna.UI2.WinForms.Guna2Panel dashboard_panel_data;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
    }
}