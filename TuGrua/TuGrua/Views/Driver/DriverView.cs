using System;
using System.Collections.Generic;
using Xamarin.Forms;
using TuGrua.Core.Entities;
using Xamarin.Forms.Maps;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TuGrua
{
	public class DriverView : ContentPage
	{
		public static TuGrua.DriverView ThisPage { get; set; }

		private Authentication _auth;
		private Driver _driver;

		public Driver Driver {
			get {
				return _driver;
			}
			set
			{
				_driver = value;
			}
		}

		public int _currentUserStatus;

		// Elements
		public Label _listenStatus;
		public Label _listenLatitude;
		public Label _listenLongitude;
		public Label _status;
		public Label _name;
		public Label _confirmedJob;
		public Button _changeStatus;
		public Button _confirmJob;

        public DriverView (Authentication auth)
		{
            App.OnSocketEvent += App_OnSocketEvent;
			_auth = auth;
			validateUser ();

			SetUI ();

			_currentUserStatus = 1;
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
			string command = (string)data.SelectToken ("Command");
			switch (command) {
			case "RequestJob":
				values = (JToken)data.SelectToken ("Values");
				if (values != null) {
					string craneId = (String)values.SelectToken ("CraneId");
					Crane crane = _driver.Cranes.Find (x => x.CraneId.Equals (craneId));
					if (crane != null) {
						JToken details = (JToken)values.SelectToken ("Details");
						string jobType = (String)values.SelectToken ("RequestType");
						Device.BeginInvokeOnMainThread (async() => {
							bool accept = await DisplayAlert ("Nuevo trabajo", "Tienes un nuevo trabajo en asistencia de " + jobType, "Aceptar trabajo", "Rechazar");

							if (accept) {
								_confirmedJob.Text = (string)details.SelectToken ("Details");
								object response = new
								{
									Requester = details.SelectToken ("RequesterId"),
									Driver = _driver
								};
								JObject jsonObject = JObject.FromObject (response);

								App.io.Emit ("ConfirmedJob", jsonObject);

								_changeStatus.IsEnabled = false;
								_changeStatus.Text = "Ocupado";
								_changeStatus.TextColor = Color.Black;
								_changeStatus.BackgroundColor = Color.Red;
							}
						});
					}
				}
				break;
			default:
				break;
			}
		}

        private void UpdateStatus(string socketStatus)
        {
			Device.BeginInvokeOnMainThread(() =>
				{
					if (_status != null) {
						_status.Text = "Estado: " + socketStatus;
					}
				});
        }

        private void SetInitialStatus()
        {
            string status = String.Empty;
            switch (App.SocketStatus)
            {
                case Core.Enums.SocketStatus.Connected:
                    status = "Conectado";
                    break;
                case Core.Enums.SocketStatus.Connecting:
                    status = "Conectando...";
                    break;
                case Core.Enums.SocketStatus.ConnectionError:
                    status = "Error en la conexión";
                    break;
                case Core.Enums.SocketStatus.ConnectionTimeout:
                    status = "No se puede conectar por el tiempo de espera";
                    break;
                case Core.Enums.SocketStatus.Disconnected:
                    status = "Desconectado";
                    break;
                case Core.Enums.SocketStatus.FailedToConnect:
                    status = "Falla al conectar";
                    break;
                case Core.Enums.SocketStatus.ReconnectFailed:
                    status = "Reconexión fallida";
                    break;
                case Core.Enums.SocketStatus.Reconnecting:
                    status = "Reconectando...";
                    break;
                default:
                    break;
            }

            UpdateStatus(status);
        }

        private void SetUI()
		{
			// Setting up the navigation bar

			NavigationPage navigationPage = new NavigationPage(this);
			navigationPage.BarBackgroundColor = Color.FromRgb(255, 255, 255);
			navigationPage.BarTextColor = Color.FromRgb(255, 255, 255);

			// Creating labels
			_listenStatus = new Label();
			_listenLatitude = new Label ();
			_listenLongitude = new Label ();
			_status = new Label ();
			_name = new Label ();
			_confirmedJob = new Label ();

			_name.Text = "Comienza poniendo un estado en el siguiente botón, en verde diciendo que estás dispuesto a trabajar; de lo contrario, en rojo.";
            SetInitialStatus();

			// Set Buttons
			_changeStatus = new Button();
			_confirmJob = new Button ();

			_changeStatus.Text = "Estoy disponible";
			_changeStatus.BackgroundColor = Color.FromRgb(10, 214, 0);
			_changeStatus.TextColor = Color.FromRgb (0, 0, 0);
			_confirmJob.Text = "Hay un nuevo trabajo, ver detalles";
			_confirmJob.IsVisible = false;

			_changeStatus.Clicked += _changeStatus_Clicked;
			_confirmJob.Clicked += _confirmJob_Clicked;

			// Setting layout
			var stack = new StackLayout { Spacing = 0 };

			stack.Children.Add (_status);
			stack.Children.Add (_name);
			stack.Children.Add (_changeStatus);
			stack.Children.Add (_confirmJob);
			stack.Children.Add (_confirmedJob);

			Content = stack;

			ThisPage.Title = "  TuGrua.co";
		}

		void _changeStatus_Clicked (object sender, EventArgs e)
		{
			if (_currentUserStatus == (int)TuGrua.Core.Enums.DriverStatus.Busy) {
				object changeStatus = new { 
					Email = _auth.Email,
					Status = 1
				};
				JObject jsonObject = JObject.FromObject(changeStatus);

				TuGrua.App.io.Emit ("ChangeStatus", (data)=>{
					if ((bool)data == true) {
						Device.BeginInvokeOnMainThread(() =>
							{
								ThisPage._changeStatus.Text = "Estoy disponible";
								ThisPage._changeStatus.BackgroundColor = Color.FromRgb(10, 214, 0);
								ThisPage._changeStatus.TextColor = Color.FromRgb (0, 0, 0);
							});
						_currentUserStatus = 1;
					}
				}, jsonObject);
			}
			else if (_currentUserStatus == (int)TuGrua.Core.Enums.DriverStatus.Available) {
				object changeStatus = new { 
					Email = _auth.Email,
					Status = 0
				};
				JObject jsonObject = JObject.FromObject(changeStatus);

				TuGrua.App.io.Emit ("ChangeStatus", (data)=>{
					if ((bool)data == true){
						Device.BeginInvokeOnMainThread(() =>
							{
								ThisPage._changeStatus.Text = "Estoy ocupado";
								ThisPage._changeStatus.BackgroundColor = Color.FromRgb(219, 0, 0);
								ThisPage._changeStatus.TextColor = Color.FromRgb (255, 255, 255);
							});
						_currentUserStatus = 0;
					}
				}, jsonObject);
			}

		}

		void _confirmJob_Clicked(object sender, EventArgs e)
		{

		}

		private async void validateUser()
		{
			if (_auth == null) 
			{
				await Navigation.PopToRootAsync ();
			} 
			else
			{
				// Request for detailed user
				_driver = await Driver.Create(_auth);

				if (_driver == null) 
				{
					await Navigation.PopToRootAsync ();
				} 
				else 
				{
					if (_name != null) {
						//_name.Text = "Bienvenido " + _driver.Name + " " + _driver.LastName;
						this.Title = "Bienvenido " + _driver.Name + " " + _driver.LastName;
						_currentUserStatus = _auth.Status;
					}

					ThisPage = this;
				}
			}
		}
	}
}