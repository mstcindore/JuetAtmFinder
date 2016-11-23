using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BingMapsTry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            myMap.Loaded += MyMap_Loaded;
        }

        private Geoposition GPos;
        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            myMap.Center = new Geopoint(new BasicGeoposition { Latitude = 24, Longitude = 77 });

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 10 };
                    geolocator.StatusChanged += Geolocator_StatusChanged;
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    UpdateLocationData(pos);
                    break;
            }
        }

        private void UpdateLocationData(Geoposition pos)
        {
            myMap.Center = new Geopoint(new BasicGeoposition { Latitude = pos.Coordinate.Latitude, Longitude = pos.Coordinate.Longitude});
            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = myMap.Center;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = "Me";
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MapsIcon.png"));
            mapIcon.ZIndex = 0;
            myMap.MapElements.Add(mapIcon);
            GPos = pos;
        }


        private async void GetAtms(Geoposition pos)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://juetatmfinder.azurewebsites.net/Api/ATM");
            var param = new
            {
                latitude = pos.Coordinate.Latitude,
                longitude = pos.Coordinate.Longitude,
                radius = "10000"
            };

            string data = JsonConvert.SerializeObject(param);

            byte[] postData = Encoding.ASCII.GetBytes(data);

            request.Method = "POST";
            request.ContentType = "application/json";

            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(postData, 0, data.Length);
            }

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                       string output = sr.ReadToEnd();
                        showAtms(output);
                    }
                }
            }

        }

        private void Geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetAtms(GPos);
        }

        private void showAtms(string atms)
        {
            var AllAtms = new Parameters();
            AllAtms= JsonConvert.DeserializeObject<Parameters>(atms);

            for (int i = 0; i< AllAtms.Entries.Length;i++)
            {
                MapIcon mapIcon = new MapIcon();
                mapIcon.Location = new Geopoint(new BasicGeoposition { Latitude = AllAtms.Entries[i].latitude, Longitude = AllAtms.Entries[i].longitude }); ;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = AllAtms.Entries[i].name;
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Visa.png"));
                mapIcon.ZIndex = 0;
                mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                myMap.MapElements.Add(mapIcon);
                
            }
        }
    }
}
