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
using System.Threading;

namespace TuGrua
{
    public class MainPage : ContentPage
    {
        private ManualResetEvent allDone = new ManualResetEvent(false);

        private byte[] _dataToPost;

        public ContentPage ContentPage { get; set; }

        private CustomButton _loginButton;
        private CustomEntry _emailText;
        private CustomEntry _passText;
        public MainPage()
        {
            SetElements();

            // The root page of your application
            ContentPage = new ContentPage
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
                await LoginCross(_emailText.Text, _passText.Text);
            }
        }

        private async Task AuthenticationProcess(Authentication auth)
        {
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

        async Task<Authentication> LoginCross(string email, string password)
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
            byte[] data = Encoding.UTF8.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;
            request.Headers["Content-Length"] = data.Length.ToString();

            // start the asynchronous operation
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

            return null;
        }

        private async void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream stream = request.EndGetRequestStream(asynchronousResult);

            await stream.WriteAsync(_dataToPost, 0, _dataToPost.Length);

            stream.Dispose();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
        }

        private async void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            
            var rawJson = new StreamReader(streamResponse).ReadToEnd();
            var result = JObject.Parse(rawJson);

            bool success = result["success"].ToObject<bool>();
            if (!success)
            {
                await AuthenticationProcess(null);
                await DisplayAlert("Aviso", (string)result["message"], "OK");
            }
            else {
                await AuthenticationProcess(new Authentication()
                {
                    Token = result["token"].ToObject<string>(),
                    UserId = result["userId"].ToObject<string>(),
                    Role = (Role)(result["role"].ToObject<int>()),
                    Email = _emailText.Text,
                    Status = result["status"].ToObject<int>(),
                    DetailedUserId = result["detailedUserId"].ToObject<DetailedUser>()
                });
            }

            // Close the stream object
            streamResponse.Dispose();
            
            // Release the HttpWebResponse
            response.Dispose();
            
        }
    }
}
