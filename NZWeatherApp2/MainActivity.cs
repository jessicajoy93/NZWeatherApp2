using System;
using System.Net;
using Android.App;
using Android.Widget;
using Android.OS;

namespace NZWeatherApp2
{
    [Activity(Label = "NZWeatherApp", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ImageView myImageView;
        private string StrMetService;
        public string URL { get; set; }
        private Button btnGetWeather;
        private string TempText;
        public string City;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            //tie in the ImageView
            //myImageView = FindViewById<ImageView>(Resource.Id.Image);
            //Tie in the Button, and create a Click event for using a delegate (Not the old fashioned way, but MS likes it. )
            btnGetWeather = FindViewById<Button>(Resource.Id.GetWeatherButton);
            //When you click the button
            btnGetWeather.Click += btnGetWeather_Click;
            SpinnerSetup();
        }

        private void btnGetWeather_Click(object sender, EventArgs e)
        {
            //Create the URL - we can change this later for other places
            //URL = "http://m.metservice.com/towns/christchurch";
            URL = "http://m.metservice.com/towns/" + City;
            //run the method that dl's the temp
            ConnectToNetAndDLTemp();
            //change the text on the button, so that you know something has happened

            //btnGetWeather.Text = "Christchurch";
            btnGetWeather.Text = City.ToUpper();
        }

        private void ConnectToNetAndDLTemp()
        {
            //downloads the string and returns it
            var webaddress = new Uri(URL); //Get the URL change it to a Uniform Resource Identifier
            var webclient = new WebClient(); //Make a webclient to dl stuff
            webclient.DownloadStringAsync(webaddress); //dl the website as a string
            //Pink color means its an event
            webclient.DownloadStringCompleted += webclient_DownloadStringCompleted; //Connect a method to the run when the DL is finished,
        }

        private void webclient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //http://stackoverflow.com/questions/30634329/how-to-download-image-from-url-in-xamarin-android
            StrMetService = e.Result; // Result is a property that holds the DL'ed string
            //get rid of single quotes in the string, its a pain otherwise. Always do this first
            StrMetService = StrMetService.Replace("\"", string.Empty);
            //get rid of everything in the header, you don't need it
            StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body"));
            //get the left hand side of where the temp is, add 30 to get to the end of this string and the beginning of the number
            var intTempLeft = StrMetService.IndexOf("<span class=actual-temp>") + 24;
            //get the length of the temp string you want. To do that find the text after the Temp and subtract the length BEFORE the temp from it.
            var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;
            //Pass all the text to the textView in the Scroll bar so you can see the text

            //Pass the Temp to the TempText TextView
            var Temp = StrMetService.Substring(intTempLeft, intTempRight);
            TempText = Temp + " " + "°C";
            FindViewById<TextView>(Resource.Id.TempText).Text = TempText;
            //FindViewById<TextView>(Resource.Id.AllText).Text = StrMetService;
            //Run the Image code, and pass the image to the ImageView
            //var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath());
            //myImageView.SetImageBitmap(imageBitmap);
        }

        private void SpinnerSetup()
        {
            //https://developer.xamarin.com/guides/android/user_interface/spinner/
            //tie in the spinner 
            var spinner = FindViewById<Spinner>(Resource.Id.spCity);
            //tie it to the method. 
            spinner.ItemSelected += spinner_ItemSelected;
            //The CreateFromResource() method then creates a new ArrayAdapter, which binds each item in the string array to the initial appearance for the Spinner (which is how each item will appear in the spinner when selected).
            var arrayadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.place_array, Android.Resource.Layout.SimpleSpinnerItem);
            //SetDropDownViewResource is called to define the appearance for each item when the widget is opened (SimpleSpinItem is another standard layout defined by the platform)
            arrayadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //Finally, the ArrayAdapter is set to associate all of its items with the Spinner by setting the Adapter property 
            spinner.Adapter = arrayadapter;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender; //make a fake spinner and send through the data to it
            City = spinner.GetItemAtPosition(e.Position).ToString();
            City = City.ToLower(); //make it lower case for the URL to work
            //make a string to hold the city name so it appears in the toast message
            var toast = string.Format("The city is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}

