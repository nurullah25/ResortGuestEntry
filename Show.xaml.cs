using GuestInfoEntry.Models;
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
    /// Interaction logic for Show.xaml
    /// </summary>
    public partial class Show : Window
    {
        public string fileName = "GuestsInformation.json";
        public FileInfo TempImageFile { get; set; }
        public BitmapImage DefaultImage => new BitmapImage(new Uri(GetImagePath() + "default.jpg"));
        public string DefaultImagePath => GetImagePath() + "default.jpg";
        public Show()
        {
            InitializeComponent();
        }
        public void Showdata()
        {
            var Json = File.ReadAllText(fileName);
            var JsonObj = JObject.Parse(Json);
            if (JsonObj != null)
            {
                JArray GuestArry = (JArray)JsonObj["GuestsInfos"];
                List<GuestsInfo> guestInformation = new List<GuestsInfo>();
                if (GuestArry.Count == 0)
                {
                    lstAll.ItemsSource = "";
                    MessageBox.Show("Database Is Empty!!!!");
                    return;
                }
                foreach (var item in GuestArry)
                {
                    guestInformation.Add(new GuestsInfo()
                    {
                        ID = Convert.ToInt32(item["ID"]),                        
                        FirstName = item["FirstName"].ToString(),
                        LastName = item["LastName"].ToString(),
                        Gender = item["Gender"].ToString(),
                        Age = Convert.ToInt32(item["Age"]),
                        Email = item["Email"].ToString(),
                        ContactNo = item["ContactNo"].ToString(),                        
                        CheckIn = Convert.ToDateTime(item["CheckIn"]),
                        CheckOut = Convert.ToDateTime(item["CheckOut"]),
                        RoomType = item["RoomType"].ToString(),
                        ImageInformation = File.Exists(GetImagePath() + item["ImageInformation"]) ? GetImagePath() + item["ImageInformation"] : DefaultImagePath,

                        ImageShow= ImageInstance(new Uri(GetImagePath()+item["ImageInformation"]))
                        

                    });
                    lstAll.ItemsSource = guestInformation;
                }
                lstAll.Items.Refresh();
            }
        }
        private string GetImagePath()
        {
            var AddinAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var AddinFolder = System.IO.Path.GetDirectoryName(AddinAssembly.Location);
            string ImagePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AddinFolder, @"..\..\Images\"));

            return ImagePath;
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
            return bitmap;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
           
            Update edit = new Update();
            edit.Show();
            this.Hide();
            edit.Owner = this;
            Button b = sender as Button;
            GuestsInfo information = b.CommandParameter as GuestsInfo;
            edit.txtID.IsEnabled = false;
            edit.txtID.Text = information.ID.ToString();            
            edit.txtFirstName.Text = information.FirstName;
            edit.txtLastName.Text = information.LastName;
            if (information.Gender == "Male")
            {
                edit.rdMale.IsChecked = true;
            }
            else if (information.Gender == "Female")
            {
                edit.rdFemale.IsChecked = true;
            }
            edit.txtAge.Text = information.Age.ToString(); ;
            edit.txtEmail.Text = information.Email;
            edit.txtContactNo.Text = information.ContactNo;
            edit.dpCheckIn.Text = information.CheckIn.ToString();
            edit.dpCheckOut.Text = information.CheckOut.ToString();
            edit.cmbRoomType.Text = information.RoomType;
            edit.ImgDisplay.Source = information.ImageShow;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var JasonD = File.ReadAllText(fileName);
            var JasonObj = JObject.Parse(JasonD);
            JArray GuestDeleteArry = (JArray)JasonObj["GuestsInfos"];
            Button b = sender as Button;
            GuestsInfo gst = b.CommandParameter as GuestsInfo;
            int guestId = gst.ID;
            if (guestId > 0)
            {
                var informationToDeleted = GuestDeleteArry.FirstOrDefault(obj => obj["ID"].Value<int>() == guestId);
                GuestDeleteArry.Remove(informationToDeleted);
                string output = JsonConvert.SerializeObject(JasonObj, Formatting.Indented);

                MessageBoxResult result = MessageBox.Show($"Are you want to delete {informationToDeleted["ID"].Value<string>()}", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    FileInfo thisFile = new FileInfo(GetImagePath() + informationToDeleted["ImageInformation"]);
                    if (thisFile.Name != "default.jpg") 
                    {
                        thisFile.Delete();
                    }
                    File.WriteAllText(fileName, output);
                    MessageBox.Show("Data Deleted Successfully !!", "Delete", MessageBoxButton.OK, MessageBoxImage.Question);
                    Showdata();
                    Guests gs = new Guests();
                    gs.AllClear();
                }
                else
                {
                    return;
                }
            }
        }

        private void backList_Click(object sender, RoutedEventArgs e)
        {
            Guests gst = new Guests();
            gst.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            ViewInformation viewinfo = new ViewInformation();
            
            Button b = sender as Button;
            GuestsInfo g = b.CommandParameter as GuestsInfo;

            viewinfo.txttext.Text = $" Id Number\t:  {g.ID}\n Name\t\t:  {  g.FirstName } {g.LastName} \n Gender\t\t:  {g.Gender}  \n Age\t\t:  {g.Age} \n Email\t\t:  {g.Email} \n ContactNo\t:  {g.ContactNo} \n RoomType\t:  {g.RoomType} \n CheckIn\t\t:  {g.CheckIn}\n CheckOut\t:  {g.CheckOut}";
            viewinfo.image.Source = g.ImageShow;
            this.Close();
            viewinfo.Show();
        }
    }
}
