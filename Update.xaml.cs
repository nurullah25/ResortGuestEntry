using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GuestInfoEntry
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : Window
    {
        Show Shows = new Show();
        public string fileName = @"GuestsInformation.json";
        public FileInfo TempImageFile { get; set; }
        public FileInfo OldImageFile { get; set; }
        public BitmapImage DefaultImage => new BitmapImage(new Uri(GetImagePath() + "default.jpg"));
        public string DefaultImagePath => GetImagePath() + "default.jpg";

        public Update()
        {
            InitializeComponent();
            string[] roomType = new string[] { "Economy", "Business", "Suite" };
            this.cmbRoomType.ItemsSource = roomType;
            

            var path = System.IO.Path.GetDirectoryName(GetImagePath());
            if (!File.Exists(@"GuestsInformation.json"))
            {
                File.CreateText(fileName);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            ImgDisplay.Source = DefaultImage;
            
        }
        private string GetImagePath()
        {
            var AddinAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var AddinFolder = System.IO.Path.GetDirectoryName(AddinAssembly.Location);
            string ImagePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AddinFolder, @"..\..\Images\"));

            return ImagePath;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
           
            var JasonD = File.ReadAllText(fileName);
            var JasonObj = JObject.Parse(JasonD);
            JArray GuestUpdateArry = (JArray)JasonObj["GuestsInfos"];
            var ID = int.Parse(txtID.Text);
            var FirstName = txtFirstName.Text;
            var LastName = txtLastName.Text;
            var Gender = rdMale.IsChecked;
            var Age = txtAge.Text;
            var Email = txtEmail.Text;
            var ContactNo = txtContactNo.Text;
            var RoomType = cmbRoomType.SelectedItem.ToString();
            var CheckIn = DateTime.Parse(dpCheckIn.Text);
            var CheckOut = DateTime.Parse(dpCheckOut.Text);


            var ImageInformation = (TempImageFile != null) ? $"{int.Parse(txtID.Text) + TempImageFile.Extension}" : "default.jpg";

            foreach (var guest in GuestUpdateArry.Where(obj => obj["ID"].Value<int>() == ID))
            {
                
                guest["FirstName"] = !string.IsNullOrEmpty(FirstName) ? FirstName : "";
                guest["LastName"] = !string.IsNullOrEmpty(LastName) ? LastName : "";
               
                guest["Age"] = !string.IsNullOrEmpty(Age) ? Age : "";
                guest["Email"] = !string.IsNullOrEmpty(Email) ? Email : "";
                guest["ContactNo"] = !string.IsNullOrEmpty(ContactNo) ? ContactNo : "";
                guest["RoomType"] = !string.IsNullOrEmpty(RoomType) ? RoomType : "";
               
                OldImageFile = (guest["ImageInformation"].ToString() != "default.png") ? new FileInfo(GetImagePath() + guest["ImageInformation"].ToString()) : null;

                if (TempImageFile != null && OldImageFile == null)  
                {
                    TempImageFile.CopyTo(GetImagePath() + guest["ID"] + TempImageFile.Extension);
                    guest["ImageInformation"] = guest["ID"] + TempImageFile.Extension;
                    TempImageFile = null;
                }
                if (OldImageFile != null && TempImageFile != null && File.Exists(OldImageFile.FullName)) 
                {
                    
                    OldImageFile.Delete();      
                    TempImageFile.CopyTo(GetImagePath() + guest["ID"] + TempImageFile.Extension);
                    guest["ImageInformation"] = guest["ID"] + TempImageFile.Extension;
                    TempImageFile = null;
                }

            }
            JasonObj["GuestsInfos"] = GuestUpdateArry;
            string output = JsonConvert.SerializeObject(JasonObj, Formatting.Indented);
            File.WriteAllText(@"GuestsInformation.json", output);
            this.Close();
            Show ShowS = new Show();
            ShowS.Show();
            ShowS.Showdata();
            MessageBox.Show("Data Updated Successfully !!");
        }

        private void btnImageUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png;";
            fd.Title = "Select an Image";
            if (fd.ShowDialog().Value == true)
            {
                ImgDisplay.Source = new BitmapImage(new Uri(fd.FileName));
                TempImageFile = new FileInfo(fd.FileName);
            }
        }

        private void backList_Click(object sender, RoutedEventArgs e)
        {
            Show sw = new Show();
            this.Hide();
            sw.Show();
            sw.Showdata();
        }
    }
}
