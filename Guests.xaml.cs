using Newtonsoft.Json;
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
using Path = System.IO.Path;
using GuestInfoEntry.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace GuestInfoEntry
{
    /// <summary>
    /// Interaction logic for Guests.xaml
    /// </summary>
    public static class JsonExtension
    {

        public static string Serialize(this object obj)
        {
            var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return data;
        }
    }
    public partial class Guests : Window
    {
        Show Shows = new Show();
        public string fileName = @"GuestsInformation.json";
        public FileInfo TempImageFile { get; set; }
        public BitmapImage DefaultImage => new BitmapImage(new Uri(GetImagePath() + "default.jpg"));
        public string DefaultImagePath => GetImagePath() + "default.jpg";
        public Guests()
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
            ShowSaveData();
        }
                
        
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string Gender = string.Empty;
            if (rdMale.IsChecked == true)
            {
                Gender = "Male";
            }
            else
            {
                Gender = "Female";
            }

            GuestsInfo gst = new GuestsInfo()
            {
                ImageInformation = (TempImageFile != null) ? $"{int.Parse(txtID.Text) + TempImageFile.Extension}" : "default.jpg",
                ID = int.Parse(txtID.Text),
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Gender = Gender,
                Age = int.Parse(txtAge.Text),
                Email = txtEmail.Text,
                ContactNo = txtContactNo.Text,
                RoomType = cmbRoomType.SelectedItem.ToString(),
                CheckIn = DateTime.Parse(dpCheckIn.Text),
                CheckOut = DateTime.Parse(dpCheckOut.Text),
            };

            string filedata = File.ReadAllText(fileName);
            if (IsValidJson(filedata) && IsExists("GuestsInfos") && !IsIdExists(gst.ID)) 
            {
                var data = JObject.Parse(filedata);
                var gstJson = data.GetValue("GuestsInfos").ToString();
                var gstList = JsonConvert.DeserializeObject<List<GuestsInfo>>(gstJson);
                gstList.Add(gst);
                JArray gstArray = JArray.FromObject(gstList);
                data["GuestsInfos"] = gstArray;
                var newJsonResult = JsonConvert.SerializeObject(data, Formatting.Indented);

                if (TempImageFile != null)
                {
                    TempImageFile.CopyTo(GetImagePath() + gst.ImageInformation);
                    TempImageFile = null;
                    ImgDisplay.Source = DefaultImage;
                }
                File.WriteAllText(fileName, newJsonResult);     
            }

            if (!IsValidJson(filedata))
            {
                var gs = new { GuestsInfos = new GuestsInfo[] { gst } };  
                string newJsonResult = JsonConvert.SerializeObject(gs, Formatting.Indented); 
                if (TempImageFile != null)
                {
                    TempImageFile.CopyTo(GetImagePath() + gst.ImageInformation);
                    TempImageFile = null;
                    ImgDisplay.Source = DefaultImage;
                }
                File.WriteAllText(fileName, newJsonResult);         
            }
            
            MessageBox.Show("Data Saved Successfully!!!", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            ShowSaveData();
        }


        private bool IsValidJson(string data)
        {
            try
            {
                var temdata = (JContainer)JToken.Parse(data);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        private bool IsExists(string data)
        {
            string filedata = File.ReadAllText(fileName);

            var root = (JContainer)JToken.Parse(filedata);

            var name = root.DescendantsAndSelf()
                .OfType<JProperty>()
                .Select(p => p.Name)
                .FirstOrDefault();
            if (name == data)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsIdExists(int inputId)
        {
            string filedata = File.ReadAllText(fileName);
            var fileObj = JObject.Parse((string)filedata);
            var GuestsJson = fileObj.GetValue("GuestsInfos").ToString();
            var ds = JsonConvert.DeserializeObject<List<GuestsInfo>>(GuestsJson);
            var exists = ds.Find(x => x.ID == inputId);
            if (exists != null)
            {
                MessageBox.Show($"ID - {exists.ID} exists\nTry with different Id", "Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {           
            Shows.Show();
            this.Hide();

            Shows.Showdata();
        }
        public void ShowSaveData()
        {
            var json = File.ReadAllText(fileName);

            if (!IsValidJson(json))
            {
                return;
            }

            var jsonObj = JObject.Parse(json);
            var gstJson = jsonObj.GetValue("GuestsInfos").ToString();
            var gstList = JsonConvert.DeserializeObject<List<GuestsInfo>>(gstJson);   
            gstList = gstList.OrderBy(x => x.ID).ToList();  

            foreach (var item in gstList)
            {
                item.ImageShow = ImageInstance(new Uri(GetImagePath() + item.ImageInformation));   
            }
            lstGuests.ItemsSource = gstList;
            lstGuests.Items.Refresh();

            GC.Collect();                  
            GC.WaitForPendingFinalizers();
        }
        public ImageSource ImageInstance(Uri path)  
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 300;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        private string GetImagePath()
        {
            var AddinAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var AddinFolder = Path.GetDirectoryName(AddinAssembly.Location);
            string ImagePath = Path.GetFullPath(Path.Combine(AddinFolder, @"..\..\Images\"));

            return ImagePath;
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
        public void AllClear()
        {
            txtID.Clear();
            cmbRoomType.SelectedIndex = -1;
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtContactNo.Clear();
            txtAge.Clear();      
            
            txtID.IsEnabled = true;
        }

        private void backList_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
