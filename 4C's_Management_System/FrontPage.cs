namespace _4C_s_Management_System
{
    public partial class FrontPage : Form
    {
        public FrontPage()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Shapes1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

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

        private void FrontPage_Load(object sender, EventArgs e)
        {

        }
    }
}
