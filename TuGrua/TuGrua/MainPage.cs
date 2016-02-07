using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TuGrua.Extensions;
using Xamarin.Forms;

using System.IO;
using System.Json;
using System.Net;
using System.Threading.Tasks;
using TuGrua.Core.Entities;
using TuGrua.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TuGrua
{
    public class MainPage : ContentPage
    {
        public ContentPage NavigationPage { get; set; }

        private CustomButton _loginButton;
        private CustomEntry _emailText;
        private CustomEntry _passText;
        public MainPage()
        {
            SetElements();

            // The root page of your application
            NavigationPage = new ContentPage
            {
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 15,
                    BackgroundColor = Color.Black,
                    Padding = new Thickness(20, 0, 20, 20),
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Children = {
                        _emailText,
                        _passText,
                        _loginButton
                    }
                }
            };

            _loginButton.Clicked += _loginButton_Clicked;
        }

        private async void _loginButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_emailText.Text) && !string.IsNullOrWhiteSpace(_passText.Text))
            {
                Authentication auth = null;
                if (Device.OS == TargetPlatform.Android)
                {
                    auth = await LoginCross(_emailText.Text, _passText.Text);
                }
                else if (Device.OS == TargetPlatform.WinPhone)
                {
                    auth = await LoginWindowsPhone(_emailText.Text, _passText.Text);
                }

                if (auth != null)
                {
                    switch (auth.Role)
                    {
                        case Role.Admin:
                            {
                                var page = new TuGrua.AdminApp(auth);
                                Navigation.InsertPageBefore(page, this);
                                await Navigation.PopAsync().ConfigureAwait(false);
                                break;
                            }
                        case Role.Driver:
                            {
                                var page = new TuGrua.DriverView(auth);
                                Navigation.InsertPageBefore(page, this);
                                await Navigation.PopAsync().ConfigureAwait(false);
                                break;
                            }
                        case Role.Requester:
                            {
                                var page = new TuGrua.RequestService(auth);
                                Navigation.InsertPageBefore(page, this);
                                await Navigation.PopAsync().ConfigureAwait(false);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }

        private void SetElements()
        {
            _loginButton = new CustomButton
            {
                Text = "Entrar"
            };
            _emailText = new CustomEntry
            {
                Name = "emailText",
                Placeholder = "Email",
                Keyboard = Keyboard.Email
            };
            _passText = new CustomEntry
            {
                Name = "passText",
                Placeholder = "Contraseña",
                IsPassword = true
            };
        }

        async Task<Authentication> LoginWindowsPhone(string email, string password)
        {
            User user = new User(true);

            System.Net.WebRequest request = await Task.Run(() => {
                return System.Net.WebRequest.Create(
                    TuGrua.Core.Backend.Constants.UriServer +
                    TuGrua.Core.Backend.Constants.UriApi +
                    TuGrua.Core.Backend.Constants.UriAuthenticate);
            });

            StringBuilder postData = new StringBuilder();
            postData.Append("email=" + System.Uri.EscapeDataString(email) + "&");
            postData.Append("password=" + System.Uri.EscapeDataString(password) + "&");
            var data = Encoding.UTF8.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;
            request.Headers["Content-Length"] = data.Length.ToString();
            try
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var rawJson = new StreamReader(stream).ReadToEnd();
                        var result = JObject.Parse(rawJson);

                        bool success = result["success"].ToObject<bool>();
                        if (!success)
                        {
                            await DisplayAlert("Aviso", (string)result["message"], "OK");
                        }
                        else {
                            return new Authentication()
                            {
                                Token = result["token"].ToObject<string>(),
                                UserId = result["userId"].ToObject<string>(),
                                Role = (Role)(result["role"].ToObject<int>()),
                                Email = email,
                                Status = result["status"].ToObject<int>(),
                                DetailedUserId = result["detailedUserId"].ToObject<DetailedUser>()
                            };
                        }
                    }
                }
            }
            catch (System.Net.WebException)
            {

            }

            return null;
        }

        async Task<Authentication> LoginCross(string email, string password)
        {
            User user = new User(true);

            var request = await Task.Run(() => {
                return System.Net.WebRequest.Create(
                    TuGrua.Core.Backend.Constants.UriServer +
                    TuGrua.Core.Backend.Constants.UriApi +
                    TuGrua.Core.Backend.Constants.UriAuthenticate);
            });

            StringBuilder postData = new StringBuilder();
            postData.Append("email=" + System.Uri.EscapeDataString(email) + "&");
            postData.Append("password=" + System.Uri.EscapeDataString(password) + "&");
            var data = Encoding.UTF8.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            try
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                        //Console.Out.WriteLine("Response: {0}", jsonDoc.ToString ());

                        JsonObject result = jsonDoc as JsonObject;
                        bool success = (bool)result["success"];
                        if (!success)
                        {
                            await DisplayAlert("Aviso", (string)result["message"], "OK");
                        }
                        else {
                            return new Authentication()
                            {
                                Token = (string)result["token"],
                                UserId = (string)result["userId"],
                                Role = (Role)((int)result["role"]),
                                Email = email,
                                Status = (int)result["status"],
                                DetailedUserId = JsonConvert.DeserializeObject<DetailedUser>(((object)result["detailedUserId"]).ToString())
                            };
                        }
                    }
                }
            }
            catch (System.Net.WebException)
            {

            }

            return null;
        }
    }
}
