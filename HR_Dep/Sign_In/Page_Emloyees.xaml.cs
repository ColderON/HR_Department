using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
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

namespace Sign_In
{
    /// <summary>
    /// Interaction logic for Page_Emloyees.xaml
    /// </summary>
    public partial class Page_Emloyees : Page
    {
        IDbConnection? conn;

        public Page_Emloyees()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            rbEmloyees.IsChecked= true;
            btnChangeData.IsEnabled = false;
            btnAddChangeImage.IsEnabled = false;
            btnDismiss.IsEnabled = false;
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

                tbEmloyeesCount.Text = BIG_Helper.employeesList.Count(e => e.StatusId == 1).ToString();
                tbDismissedCount.Text = BIG_Helper.employeesList.Count(e => e.StatusId == 2).ToString();
                lbListOf.ItemsSource = BIG_Helper.employeesList.Where(e => e.StatusId == 1);

                rbEmloyees.IsChecked = true;
                rbVacancies.IsChecked = false;
                rbDismissed.IsChecked = false;
                LoadDefaultPageEmloyees();
            }
            catch (DbException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //public ImageSource LoadImageSourceFromBytes(byte[] imageBytes)
        //{
        //    BitmapImage bitmapImage = new BitmapImage();

        //    using (MemoryStream imageStream = new MemoryStream(imageBytes))
        //    {
        //        bitmapImage.BeginInit();
        //        bitmapImage.StreamSource = imageStream;
        //        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapImage.EndInit();
        //    }

        //    bitmapImage.Freeze();

        //    return bitmapImage;
        //}

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            BitmapImage? image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                ms.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void LoadDefaultPageEmloyees()
        {            
            imagePicture.Source = LoadImage(Resource_Images.noPhoto);
            //tbShowPosition.Text = "Does not exist";
            spEnterData.IsEnabled= false;            
        }

        private async void btnChangeData_Click(object sender, RoutedEventArgs e)
        {
            if (!spEnterData.IsEnabled)
            {
                spEnterData.IsEnabled = true;
                btnAddChangeImage.IsEnabled = true;
                lbListOf.IsEnabled = false;
                btnChangeData.Foreground = Brushes.Green;
                btnChangeData.Content = "Save";
            }
            else
            {
                spEnterData.IsEnabled = false;
                btnAddChangeImage.IsEnabled = false;               

                SaveChangedData();

                lbListOf.ItemsSource = BIG_Helper.employeesList;
                btnChangeData.Foreground = Brushes.Green;
                btnChangeData.Content = "√ Successfully";
                await Task.Delay(1000);
                btnChangeData.Foreground = Brushes.Black;
                btnChangeData.Content = "Change Data";
                lbListOf.IsEnabled = true;
            }
        }

        private void btnAddChangeImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files|*.jpg;*png;*.bmp";
            openDialog.FilterIndex = 1;
            if (openDialog.ShowDialog() == true)
            {
                imagePicture.Source = new BitmapImage(new Uri(openDialog.FileName));
                AddNewImage();
            }
        }

        private void AddNewImage()
        {             
            BitmapImage? img = imagePicture.Source as BitmapImage;
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            
            MemoryStream stream = new MemoryStream();
            //BinaryReader br = new BinaryReader(stream);
            encoder.Save(stream);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            
            IDbCommand cmd = conn.CreateCommand();
            string FileName = $"{tbFirstName.Text}{tbLastName.Text}_Photo_{DateTime.Now}";
            long emplId = (lbListOf.SelectedItem as Employee).Id;

            cmd.CommandText = $"Update Employees Set PhotoName = @fileName, PhotoImage = @image where Id = {emplId}";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "fileName",
                DbType = DbType.String,
                Size = 64,
                Direction = ParameterDirection.Input,
                Value = FileName
            });            
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "image",
                SqlDbType = SqlDbType.Image,
                Size = (int)stream.Length,
                Direction = ParameterDirection.Input,
                Value = stream
            }); 
            cmd.ExecuteNonQuery();

            (lbListOf.SelectedItem as Employee).ImageFileName = $"{tbFirstName.Text}{tbLastName.Text}_Photo_{DateTime.Now}";
            (lbListOf.SelectedItem as Employee).ImageBytes = stream.ToArray();
        }
                
        private void btnDismiss_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Sure?\nDismiss this Emloyee","Confirm dismissal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"Update Employees Set Status_Id = 2 where Id = {(lbListOf.SelectedItem as Employee).Id}";
                cmd.ExecuteNonQuery();
                //TODO
                lbListOf.ItemsSource = BIG_Helper.employeesList.Where(e=> e.StatusId == 1);
                LoadDefaultPageEmloyees();
            }
        }

        private void lbListOf_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {        
            //TODO
            if(!btnChangeData.IsEnabled) {
                btnChangeData.IsEnabled = true;
                btnDismiss.IsEnabled = true;
            }
            
            if (lbListOf.SelectedItem == null){

                imagePicture.Source = LoadImage(Resource_Images.noPhoto);
                tbFirstName.Text = String.Empty;
                tbLastName.Text = String.Empty;
                dpDateOfBirth.Text = String.Empty;

                cbDepartment.SelectedIndex = -1;
                cbPosition.SelectedIndex = -1;
            }

            else if (lbListOf.SelectedItem != null && (lbListOf.SelectedItem as Employee).ImageBytes == null)
            {
                imagePicture.Source = LoadImage(Resource_Images.noPhoto);
                tbFirstName.Text = (lbListOf.SelectedItem as Employee).FirstName;
                tbLastName.Text = (lbListOf.SelectedItem as Employee).LastName;
                dpDateOfBirth.Text = (lbListOf.SelectedItem as Employee).DateOfBirth.ToString();

                cbDepartment.SelectedIndex = ((int)(lbListOf.SelectedItem as Employee).DepId - 1);

                foreach (var item in cbPosition.Items)
                {
                    if ((item as Position).Id == ((int)(lbListOf.SelectedItem as Employee).PosId))
                    {
                        cbPosition.SelectedItem = item;
                        break;
                    }
                }
            }

            else {
                imagePicture.Source = LoadImage((lbListOf.SelectedItem as Employee).ImageBytes);
                tbFirstName.Text = (lbListOf.SelectedItem as Employee).FirstName;
                tbLastName.Text = (lbListOf.SelectedItem as Employee).LastName;
                dpDateOfBirth.Text = (lbListOf.SelectedItem as Employee).DateOfBirth.ToString();

                cbDepartment.SelectedIndex = ((int)(lbListOf.SelectedItem as Employee).DepId - 1);

                foreach (var item in cbPosition.Items)
                {
                    if ((item as Position).Id == ((int)(lbListOf.SelectedItem as Employee).PosId))
                    {
                        cbPosition.SelectedItem = item;
                        break;
                    }
                }
            }
            //GC.Collect(2, GCCollectionMode.Forced);
        }
        private void cbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbListOf.SelectedItem != null) {
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

        private void SaveChangedData()
        {
            if(IsEnabled)
            {
                if ((lbListOf.SelectedItem as Employee).FirstName != tbFirstName.Text) { BIG_Helper.ChangeFName(conn, tbFirstName.Text, (lbListOf.SelectedItem as Employee).Id); }
                if ((lbListOf.SelectedItem as Employee).LastName != tbLastName.Text) { BIG_Helper.ChangeLName(conn, tbLastName.Text, (lbListOf.SelectedItem as Employee).Id); }
                if ((lbListOf.SelectedItem as Employee).DateOfBirth != DateTime.Parse(dpDateOfBirth.Text))
                {
                    BIG_Helper.ChangeBirthday(conn, DateTime.Parse(dpDateOfBirth.Text), (lbListOf.SelectedItem as Employee).Id);
                }

                if ((lbListOf.SelectedItem as Employee).DepId != (cbDepartment.SelectedItem as Department).Id ||
                    (lbListOf.SelectedItem as Employee).PosId != (cbPosition.SelectedItem as Position).Id &&
                    cbDepartment.SelectedIndex >= 0 && cbPosition.SelectedIndex >= 0)
                {
                    BIG_Helper.ChangeDepartmentAndOrPosition(conn, (cbDepartment.SelectedItem as Department).Id,
                        (cbPosition.SelectedItem as Position).Id, (lbListOf.SelectedItem as Employee).Id);
                }
            }           
            else { MessageBox.Show("Please, fill in all fileds", "Error!"); }
        }
        private bool IsAddable()
        {
            if (tbFirstName.Text != String.Empty && tbLastName.Text != String.Empty
                && dpDateOfBirth.Text != String.Empty
                && cbDepartment.SelectedIndex >= 0 && cbPosition.SelectedIndex >= 0) return true;
            else return false;
        }
        public bool HasNoPhoto(long emloyeeId)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "HasNoPhoto";

            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "persId",
                Direction = ParameterDirection.Input,
                DbType = DbType.String,
                Size = 32,
                Value = emloyeeId,
            });

            SqlParameter counter = (new SqlParameter
            {
                ParameterName = "count",
                Direction = ParameterDirection.Output,
                DbType = DbType.Int32,
            });

            cmd.Parameters.Add(counter);
            cmd.ExecuteNonQuery();

            //if user PhotoId == NULL, return true
            return (int)counter.Value == 1;
        }

        private void rbEmloyees_Click(object sender, RoutedEventArgs e)
        {
            btnChangeData.Visibility = Visibility.Visible;
            btnAddChangeImage.Visibility = Visibility.Visible;
            btnDismiss.Visibility = Visibility.Visible;

            lbListOf.SelectedItem = null;
            lbListOf.ItemsSource = BIG_Helper.employeesList.Where(e => e.StatusId == 1);

            rbEmloyees.IsChecked = true;
            rbVacancies.IsChecked = false;
            rbDismissed.IsChecked = false;
            LoadDefaultPageEmloyees();
        }

        private void rbDismissed_Click(object sender, RoutedEventArgs e)
        {
            btnChangeData.Visibility = Visibility.Hidden;
            btnAddChangeImage.Visibility = Visibility.Hidden;
            btnDismiss.Visibility = Visibility.Hidden;

            lbListOf.SelectedItem = null;
            lbListOf.ItemsSource = BIG_Helper.employeesList.Where(e => e.StatusId == 2);

            rbEmloyees.IsChecked = false;
            rbVacancies.IsChecked = false;
            rbDismissed.IsChecked = true;
            LoadDefaultPageEmloyees();
        }
    }
}
