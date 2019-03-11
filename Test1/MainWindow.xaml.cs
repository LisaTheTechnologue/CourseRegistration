using Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SMSEntities studentdb = new SMSEntities();
        public string studentid ;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string password = new System.Net.NetworkCredential(string.Empty, loginControl.Password).Password;

            if (loginControl.Username.ToString() == "" || password == "")
            {
                MessageBox.Show("Please input username and password");
            }
            else 
            {
                var query = from s in studentdb.Logins
                            where s.LoginName == loginControl.Username && s.Password == password
                            select new { id = s.Student.StudentID };
               
                if (query.Count() > 0)
                {
                    foreach (var el in query)
                    {
                        studentid = el.id;
                    }
                    MessageBox.Show("You've just entered:\n\n- Username: " + loginControl.Username + "\n- Password: " + password.ToString());
                    var w = new RegistrationWindow(studentid);
                    w.ShowDialog();
                }
                else MessageBox.Show("Invalid. Please try again. ");
            }

        }
    }
}
