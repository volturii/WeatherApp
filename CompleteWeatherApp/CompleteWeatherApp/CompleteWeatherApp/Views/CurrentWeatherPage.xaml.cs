using CompleteWeatherApp.Helper;
using CompleteWeatherApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompleteWeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetWeatherInfo("warsaw");
        }


        public void useNameOfLocation(object sender, System.EventArgs e)
        {
            string location = nameOfLocation.Text;
            GetWeatherInfo(location);
        }

        public async void useMyLocation(object sender, System.EventArgs e)
        {
            CancellationTokenSource cts;
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    string apiKey = "aa23cce64232a1301e2ecd7b24ebce5f";
                    string apiBase = "https://api.openweathermap.org/data/2.5/forecast?lat=";
                    string unit = "metric";

                    try
                    {
                        string url = apiBase + location.Latitude + "&lon=" + location.Longitude + "&appid=" + apiKey + "&units=" + unit;
                        var handler = new HttpClientHandler();
                        HttpClient client = new HttpClient(handler);
                        string result = await client.GetStringAsync(url);
                        var resultObject = JObject.Parse(result);

                        //Location

                        string locationName = resultObject["city"]["name"].ToString();

                        GetWeatherInfo(locationName);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Weather Info", ex.Message, "OK");

                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Location Info", ex.Message, "OK");
            }


        }

        private async void GetWeatherInfo(string location)
        {

            string url = $"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=aa23cce64232a1301e2ecd7b24ebce5f&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0") + "°";
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    //pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                    dateTxt.Text = "Today, " + dt.ToString("MMM dd").ToUpper();

                    GetForecast(location);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }

        private async void GetForecast(string location)
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={location}&appid=aa23cce64232a1301e2ecd7b24ebce5f&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {

                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 12 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }


                    date1Txt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");


                    date2Txt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");


                    date3Txt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");


                    date4Txt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

        private void useMyLocation_Clicked(object sender, EventArgs e)
        {

        }
    }
}