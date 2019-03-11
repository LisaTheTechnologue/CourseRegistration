using Library;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace Test1
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private SMSEntities studentdb = new SMSEntities();
        CollectionViewSource courseViewSource;
        EnrolledCourse enrolledCourse;
        string studentid, courseid;
        public RegistrationWindow(string stuid)
        {
            InitializeComponent();
            studentid = stuid;
            txtStudentID.Content = stuid;
        }

        private void courseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            courseid = courseComboBox.SelectedValue.ToString();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            EnrolledCourseList courses = new EnrolledCourseList();

            var query = from c in studentdb.Courses
                        from s in c.Students
                        where s.StudentID == studentid 
                        select new { code = c.CourseCode, title = c.CourseTitle };
            //check if query returns null
            if (query.Count() > 0)
            {
                foreach (var el in query)
                {
                    enrolledCourse = new EnrolledCourse(el.code, el.title);
                    courses.RegisterCourse(enrolledCourse);
                }
                //check if the course chosen is in student's list of course 
                if (courses.Any(c => c.CourseCode.Contains(courseid)))
                {
                    MessageBox.Show("Student " + studentid + " already enrolled " + courseid);
                }
                else
                {
                    var existingStudent = studentdb.Students.Include("Courses")
                .Where(s => s.StudentID == studentid).FirstOrDefault<Student>();

                    var addedCourses = studentdb.Courses.Where(c => c.CourseCode == courseid).ToList<Course>();

                    foreach (Course c in addedCourses)
                    {
                        studentdb.Courses.Attach(c);
                        existingStudent.Courses.Add(c);
                    }
                    studentdb.SaveChanges();
                    MessageBox.Show("Successfully registered course " + courseid);
                }
                
            }

            courseViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("courseViewSource")));

            studentdb.Courses.Load();

            courseViewSource.Source = studentdb.Courses.Local.Where(c => c.Students.Any(s => s.StudentID == studentid));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            courseViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("courseViewSource")));
            
            studentdb.Courses.Load();

            courseViewSource.Source = studentdb.Courses.Local.Where(c => c.Students.Any(s => s.StudentID == studentid));
            courseComboBox.ItemsSource = studentdb.Courses.Local;
            courseComboBox.DisplayMemberPath = "CourseTitle";
            courseComboBox.SelectedValuePath = "CourseCode";

        }

    }
}
