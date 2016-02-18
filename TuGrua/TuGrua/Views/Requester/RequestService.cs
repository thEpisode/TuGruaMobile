using System;

using Xamarin.Forms;
using TuGrua.Core.Entities;
using Xamarin.Forms.Maps;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace TuGrua
{
	public class RequestService : ContentPage
	{
		public static TuGrua.RequestService ThisPage { get; set; }

		private Authentication _auth;
		private Requester _requester;

		// Elements
		public Map _map;
        public Label _status;
        public Label _listenStatus;
		public Label _listenLatitude;
		public Label _listenLongitude;
		public Button _requestServiceButton;
        public Button _cancelServiceButton;

        // Atributes
        private bool _canRequest;

        public bool CanRequest
        {
            get { return _canRequest; }
            set { _canRequest = value; }
        }

        private TuGrua.Core.Services.Geolocation.Position _myPosition;

        public TuGrua.Core.Services.Geolocation.Position MyPosition
        {
            get { return _myPosition; }
            set { _myPosition = value; }
        }

        public RequestService (Authentication auth)
		{
            App.OnSocketEvent += App_OnSocketEvent;
			_auth = auth;
			validateUser ();

			SetUI ();
		}

        private void App_OnSocketEvent(object sender, JToken e)
        {
            switch ((String)e.SelectToken("Command"))
            {
                case "EVENT_CONNECT":
                case "EVENT_DISCONNECT":
                case "EVENT_RECONNECTING":
                case "EVENT_RECONNECT_FAILED":
                case "EVENT_CONNECT_ERROR":
                    UpdateStatus((string)(e.SelectToken("Data")));
                    break;
                case "Backend.Message":
                    FromBackendMessage((JToken)e.SelectToken("Values"));
                    break;
                default:
                    break;
            }
        }

        private void FromBackendMessage(JToken data)
        {
            JToken values = null;
            switch ((string)data.SelectToken("Command"))
            {
                case "CurrentPositionCranes":
                    break;
                case "ConfirmedJob":
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        _requestServiceButton.IsVisible = false;
                        _cancelServiceButton.IsVisible = true;
                        DisplayAlert("Conductor confirmado", "La grúa con el servicio que solicitaste está en camino", "Ok");
                    });
                    break;
				case "NoCranesAvailable":
					Device.BeginInvokeOnMainThread (() => {
						_requestServiceButton.IsEnabled = false;
						DisplayAlert ("No hay vehículos disponibles", "En este momento no hay grúas disponibles, intenta más tarde.", "Ok");
					});
					break;
                default:
                    break;
            }
        }

        private void UpdateStatus(string socketStatus)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _status.Text = "Estado: " + socketStatus;
            });
        }

        private void SetUI()
		{
			// Creating Map
			_map = new Map(
				MapSpan.FromCenterAndRadius(
					new Position(37,-122), Distance.FromMiles(0.3))) {
				IsShowingUser = true,
				HeightRequest = 100,
				WidthRequest = 960,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			// Creating labels
			_listenStatus = new Label();
			_listenLatitude = new Label ();
			_listenLongitude = new Label ();
            _status = new Label();

            // Creating Buttons
            _requestServiceButton = new Button();
			_requestServiceButton.Text = "Tomar servicio";
			_requestServiceButton.BackgroundColor = Color.FromRgb(253, 194, 44);
            _requestServiceButton.TextColor = Color.FromRgb(0, 0, 0);
            _requestServiceButton.IsEnabled = false;

            _cancelServiceButton = new Button();
            _cancelServiceButton.Text = "Cancelar servicio";
            _cancelServiceButton.BackgroundColor = Color.Red;
            _cancelServiceButton.TextColor = Color.FromRgb(0, 0, 0);
            _cancelServiceButton.IsVisible = false;


            _requestServiceButton.Clicked += _requestServiceButton_Clicked;
            _cancelServiceButton.Clicked += _cancelServiceButton_Clicked;

            // Setting layout
            var stack = new StackLayout { Spacing = 0 };

			stack.Children.Add(_status);
			//stack.Children.Add (_listenStatus);
			//stack.Children.Add (_listenLatitude);
			//stack.Children.Add (_listenLongitude);
			stack.Children.Add(_map);
			stack.Children.Add (_requestServiceButton);
            stack.Children.Add(_cancelServiceButton);

			Content = stack;

			ThisPage.Title = "  TuGrua.co";
		}

        private void _cancelServiceButton_Clicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        async void _requestServiceButton_Clicked (object sender, EventArgs e)
		{
            if (_canRequest)
            {
                var popupRequest = await DisplayActionSheet("Asistencia solicitada", "Cancelar", null, "Gasolina", "Cerrajero", "Electricista", "Mecánico", "Despinchar");

                string details = "Nombres: " + _requester.Name + " " + _requester.LastName+
                                    "\nTeléfono: " + _requester.PhoneNumber+
                                    "\nCalificación de usuario: " + _requester.GeneralCalification+
                                    "\nAsistencia en: " + popupRequest;
                object assistence = new
                {
                    Id = _requester._id,
                    Details = details,
                    Position = _myPosition,
                    Type = popupRequest
                };
                JObject jsonObject = JObject.FromObject(assistence);
                
                App.io.Emit("RequestJob", jsonObject);
            }
        }

		private async void validateUser()
		{
			if (_auth == null) 
			{
				await Navigation.PopToRootAsync ();
			} else 
			{
				// Request for detailed user
				_requester = await Requester.Create(_auth);
				if (_requester == null) 
				{
					await Navigation.PopToRootAsync ();
				} 
				else 
				{
					ThisPage = this;
				}
			}
		}
	}
}