using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Sign_In
{
    /// <summary>
    /// Interaction logic for Page_NewEmloyee.xaml
    /// </summary>
    public partial class Page_NewEmloyee : Page
    {
        IDbConnection? conn;

        public Page_NewEmloyee()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            conn = new SqlConnection(new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = "HR_Department_SQL",
                IntegratedSecurity = true,
                MultipleActiveResultSets = true,
                TrustServerCertificate = true,
            }.ConnectionString);

            try
            {
                conn.Open();
                BIG_Helper.FillDepartments(conn);
                BIG_Helper.FillPositions(conn);
                BIG_Helper.FillEmployees(conn);
                BIG_Helper.FillJobOrdersList(conn);
                cbDepartment.ItemsSource = BIG_Helper.departmentsList;
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            clicktoupload.Visibility = Visibility.Hidden;
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files|*.jpg;*png;*.bmp";
            openDialog.FilterIndex = 1;
            if (openDialog.ShowDialog() == true)
            {
                imagePicture.Source = new BitmapImage(new Uri(openDialog.FileName));               
            }
        }

        private void cbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDepartment.Items.Count > 0)
            {               
                cbPosition.Items.Clear();
                foreach (var item in BIG_Helper.positionsList)
                {
                    if (item.DepId == (cbDepartment.SelectedItem as Department).Id)
                    {
                        cbPosition.Items.Add(item);
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (IsAddable())
            {
                AddNewEmployee();
                CreateJobOrderAutom(conn);
                MakePageClear();
            }
            else { MessageBox.Show("Please, fill in all fileds", "Error!"); }
        }

        private void AddNewEmployee()
        {
            btnAdd.IsEnabled = false;
            IDbCommand cmd = conn.CreateCommand();

            string firstName = tbFirstName.Text;
            string lastName = tbLastName.Text;
            DateTime dateOfbirth = DateTime.Parse(dpDateOfBirth.Text);
            long department = (cbDepartment.SelectedItem as Department).Id;
            long position = (cbPosition.SelectedItem as Position).Id;
            string photoName = $"{tbFirstName.Text}{tbLastName.Text}_Photo_{DateTime.Now}";
            long status;
            if(checkTrainee.IsChecked == true) { status= 3; }
            else { status= 1; }

            if(imagePicture.Source != null)
            {
                cmd.CommandText = $"Insert into Employees (FirstName, LastName, Date_of_birth, Position_Id, Department_Id, PhotoName, PhotoImage, Status_Id)" +
                $"Values(@fname, @lname, @dateBirth, @Position, @DepartmentId, @PhotoName, @PhotoImage, @StatusId)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "fname",
                    DbType = DbType.String,
                    Size = 64,
                    Direction = ParameterDirection.Input,
                    Value = firstName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "lname",
                    DbType = DbType.String,
                    Size = 64,
                    Direction = ParameterDirection.Input,
                    Value = lastName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "dateBirth",
                    DbType = DbType.Date,
                    Direction = ParameterDirection.Input,
                    Value = dateOfbirth
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Position",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = position
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DepartmentId",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = department
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StatusId",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = status
                });                
                BitmapImage? img = imagePicture.Source as BitmapImage;
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));

                MemoryStream stream = new MemoryStream();
                BinaryReader br = new BinaryReader(stream);
                encoder.Save(stream);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "PhotoName",
                    DbType = DbType.String,
                    Size = 64,
                    Direction = ParameterDirection.Input,
                    Value = photoName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "PhotoImage",
                    SqlDbType = SqlDbType.Image,
                    Size = (int)stream.Length,
                    Direction = ParameterDirection.Input,
                    Value = stream
                });
            }
            else
            {
                cmd.CommandText = $"Insert into Employees (FirstName, LastName, Date_of_birth, Position_Id, Department_Id, Status_Id)" +
                $"Values(@fname, @lname, @dateBirth, @Position, @DepartmentId, @StatusId)";

                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "fname",
                    DbType = DbType.String,
                    Size = 64,
                    Direction = ParameterDirection.Input,
                    Value = firstName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "lname",
                    DbType = DbType.String,
                    Size = 64,
                    Direction = ParameterDirection.Input,
                    Value = lastName
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "dateBirth",
                    DbType = DbType.Date,
                    Direction = ParameterDirection.Input,
                    Value = dateOfbirth
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Position",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = position
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "DepartmentId",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = department
                });
                cmd.Parameters.Add(new SqlParameter
                {
                    ParameterName = "StatusId",
                    DbType = DbType.Int64,
                    Direction = ParameterDirection.Input,
                    Value = status
                });
            }
            cmd.ExecuteNonQuery();
        }

        //private void AddImage()
        //{
        //    if (imagePicture.Source != null)
        //    {
        //        BitmapImage? img = imagePicture.Source as BitmapImage;
        //        if (imagePicture != null)
        //        {
        //            BitmapEncoder encoder = new PngBitmapEncoder();
        //            encoder.Frames.Add(BitmapFrame.Create(img));

        //            MemoryStream stream = new MemoryStream();
        //            BinaryReader br = new BinaryReader(stream);
        //            encoder.Save(stream);
        //            stream.Flush();
        //            stream.Seek(0, SeekOrigin.Begin);
                    
        //            IDbCommand cmd = conn.CreateCommand();
        //            string FileName = $"{tbFirstName.Text}{tbLastName.Text}_Photo_{DateTime.Now}";
                   
        //            cmd.CommandText = $"Insert into Photos (PhotoName, PhotoImage)" +
        //                $" Values(@fileName), (@image)";

        //            cmd.Parameters.Add(new SqlParameter
        //            {
        //                ParameterName = "fileName",
        //                DbType = DbType.String,
        //                Size = 64,
        //                Direction = ParameterDirection.Input,
        //                Value = FileName
        //            });

        //            cmd.Parameters.Add(new SqlParameter
        //            {
        //                ParameterName = "image",
        //                SqlDbType = SqlDbType.Image,
        //                Size = (int)stream.Length,
        //                Direction = ParameterDirection.Input,
        //                Value = stream
        //            });                    
        //            cmd.ExecuteNonQuery();                   
        //        }
        //    }
        //    else {
        //        MakePageClear();
        //        return;
        //    }
        //}

        private void CreateJobOrderAutom(IDbConnection conn)
        {            
            BIG_Helper.AddNewEmployeeToList(conn);
            long orderNumber = 10;
            long personId = BIG_Helper.employeesList.LastOrDefault().Id;
            long department = (cbDepartment.SelectedItem as Department).Id;          
            long position = (cbPosition.SelectedItem as Position).Id;
            if (BIG_Helper.job_OrdersList.Count > 0) { orderNumber += BIG_Helper.employeesList.LastOrDefault().Id;}

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"Insert into Job_Order (OrderNumber, Person_Id, Position_Id, Department_Id)" +
                $"Values(@orderNum, @persId, @posId, @departId)";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "orderNum",
                DbType = DbType.Int64,
                Size = 64,
                Direction = ParameterDirection.Input,
                Value = orderNumber
            });            
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "persId",
                DbType = DbType.String,
                Size = 64,
                Direction = ParameterDirection.Input,
                Value = personId
            });           
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "posId",
                DbType = DbType.Int64,
                Direction = ParameterDirection.Input,
                Value = position
            });            
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "departId",
                DbType = DbType.Int64,
                Direction = ParameterDirection.Input,
                Value = department
            });
            cmd.ExecuteNonQuery();
        }

        private bool IsAddable()
        {           
            if (tbFirstName.Text != String.Empty && tbLastName.Text != String.Empty
                && dpDateOfBirth.Text != String.Empty
                && cbDepartment.SelectedIndex >= 0 && cbPosition.SelectedIndex >= 0) return true;
            else return false;
        }
        private void MakePageClear()
        {
            ShowSuccessAdded();
            btnAdd.IsEnabled = true;
            imagePicture.Source = null;
            clicktoupload.Visibility = Visibility.Visible;
            tbFirstName.Text = string.Empty;
            tbLastName.Text = string.Empty;
            cbPosition.Items.Clear();
            cbDepartment.SelectedIndex = 0;
        }
        private async void ShowSuccessAdded()
        {
            tbSuccessfullyAdded.Content = "√ Successfully added";
            await Task.Delay(1000);
            tbSuccessfullyAdded.Content = "";
        }
              
        private void tbComment_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
