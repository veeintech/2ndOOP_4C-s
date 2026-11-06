using System.Configuration;
using System.Text;
using System.Xml.Linq;

namespace _4C_s_Management_System
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new UserNavigation());
        }
    }
}

//mike's connection string ======================================================================
/*

<? xml version = "1.0" encoding = "utf-8" ?>
< configuration >
  < connectionStrings >
    < add name = "AppDB"
         connectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=&quot;C:\Users\mikel\OneDrive\文档\oop\2ndOOP_4C-s\4C's_Management_System\4C's_Database.mdf&quot;;Integrated Security=True" />
  </ connectionStrings >
</ configuration >

*/
//==============================================================================================

//jy's connection string 
//<? xml version = "1.0" encoding = "utf-8" ?>
//< configuration >

//    < connectionStrings >

//        < add name = "AppDB"

//             connectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Jyrha Mae Fababaer\OneDrive\Documents\oop\2ndOOP_4C-s\4C's_Management_System\4C's_Database.mdf;Integrated Security=True" />

//    </ connectionStrings >
//</ configuration >
//==============================================================================================