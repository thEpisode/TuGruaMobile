using System;
using Xamarin.Forms;
using Android.App;
using Xamarin.Forms.Platform.Android;
using TuGrua.Core.Services.Geolocation;
using TuGrua.Droid.Services.Geolocation;
using Android.Widget;
using Newtonsoft.Json.Linq;
using Android.Gms.Maps;

[assembly:ExportRenderer(typeof(TuGrua.RequestService), typeof(TuGrua.Droid.RequestService))]
namespace TuGrua.Droid
{
	[Activity (Label = "RequestService")]
	public class RequestService :  PageRenderer
	{
		private IGeolocator _geolocator;

		public RequestService ()
		{
			
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged (e);

			SetupGeolocator ();
		}


		void SetupGeolocator()
		{
			if (this._geolocator != null)
				return;
			else {
				this._geolocator = new Geolocator ();
			}
			//this._geolocator = DependencyService.Get<IGeolocator> ();
			this._geolocator.PositionError += OnListeningError;
			this._geolocator.PositionChanged += OnPositionChanged;

			this._geolocator.StartListening (500, 1);
		}

		/// <summary>
		/// Handles the <see cref="E:PositionChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PositionEventArgs"/> instance containing the event data.</param>
		private void OnPositionChanged(object sender, PositionEventArgs e)
		{
			try {
				object position = new { 
					Latitude = e.Position.Latitude, 
					Longitude = e.Position.Longitude, 
					Timestamp = e.Position.Timestamp,
					Accuracy = e.Position.Accuracy,
					Speed = e.Position.Speed,
					Altitude = e.Position.Altitude
				};
				JObject jsonObject = JObject.FromObject(position);

				//TuGrua.App.io.Emit ("SetPositionCrane", jsonObject);

			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}

			try {
				Xamarin.Forms.Maps.Position pos = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
				var mapSpan = new Xamarin.Forms.Maps.MapSpan (pos, 0.01, 0.01);
				TuGrua.RequestService.ThisPage._map.MoveToRegion (mapSpan);
                TuGrua.RequestService.ThisPage.MyPosition = e.Position;
                TuGrua.RequestService.ThisPage.CanRequest = true;
                TuGrua.RequestService.ThisPage._requestServiceButton.IsEnabled = true;
			} 
			catch (Exception ex) {
				Console.WriteLine (ex.Message);
                //TuGrua.RequestService.ThisPage.CanRequest = false;
            }

			try {
				TuGrua.RequestService.ThisPage._listenStatus.Text = "Estado:" + e.Position.Timestamp.ToString("G");
				TuGrua.RequestService.ThisPage._listenLatitude.Text = "Latitud: " + e.Position.Latitude.ToString("N4");
				TuGrua.RequestService.ThisPage._listenLongitude.Text = "Longitud: " + e.Position.Longitude.ToString("N4");
			} catch (Exception) {
				Console.WriteLine ("Error with visual elements: RequestService.cs");
			}

		}

		/// <summary>
		/// Handles the <see cref="E:ListeningError" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PositionErrorEventArgs"/> instance containing the event data.</param>
		private void OnListeningError(object sender, PositionErrorEventArgs e)
		{
			TuGrua.RequestService.ThisPage._listenStatus.Text = "Estado:" + e.Error.ToString();
		}
	}
}