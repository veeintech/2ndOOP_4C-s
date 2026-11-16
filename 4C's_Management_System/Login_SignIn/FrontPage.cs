namespace _4C_s_Management_System
{
    public partial class FrontPage : Form
    {
        public FrontPage()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            LogInForm lg = new LogInForm();
            this.Hide();
            lg.logIn();
            lg.Show();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            LogInForm lg = new LogInForm();
            this.Hide();
            lg.SignUp();
            lg.Show();
        }
    }
}
