using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using TuGrua.Core.Services.Geolocation;
using TuGrua.Droid.Services.Geolocation;
using Newtonsoft.Json.Linq;
using Android.Gms.Maps;

[assembly:ExportRenderer(typeof(TuGrua.DriverView), typeof(TuGrua.Droid.DriverView))]
namespace TuGrua.Droid
{
	[Activity (Label = "DriverView", Theme = "@style/CustomActionBarTheme")]			
	public class DriverView : PageRenderer
	{
		private IGeolocator _geolocator;

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
			
			this._geolocator.PositionError += OnListeningError;
			this._geolocator.PositionChanged += OnPositionChanged;

			this._geolocator.StartListening (500, 1);
		}

		/// <summary>
		/// Handles the PositionChanged event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">All information about position.</param>
		private void OnPositionChanged(object sender, PositionEventArgs e)
		{
            if (TuGrua.DriverView.ThisPage != null)
            {
                if (TuGrua.DriverView.ThisPage._currentUserStatus == 1)
                {
                    object position = new
                    {
                        Id = TuGrua.DriverView.ThisPage.Driver._id,
                        Position = e.Position
                    };
                    JObject jsonObject = JObject.FromObject(position);

                    App.io.Emit("SetPositionCrane", jsonObject);
                }

                TuGrua.DriverView.ThisPage._listenStatus.Text = "GPS Listening";
                TuGrua.DriverView.ThisPage._listenLatitude.Text = "Latitud: " + e.Position.Latitude.ToString("N4");
                TuGrua.DriverView.ThisPage._listenLongitude.Text = "Longitud: " + e.Position.Longitude.ToString("N4");
            }
		}

		/// <summary>
		/// Handles the <see cref="E:ListeningError" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PositionErrorEventArgs"/> instance containing the event data.</param>
		private void OnListeningError(object sender, PositionErrorEventArgs e)
		{

            TuGrua.DriverView.ThisPage._listenStatus.Text = "GPS Error: " + e.Error.ToString();
        }
	}
}

