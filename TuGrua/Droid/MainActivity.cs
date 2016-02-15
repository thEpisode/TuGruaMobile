using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TuGrua.Droid
{
	[Activity ( 
		Icon = "@drawable/icon", 
		/*MainLauncher = true, */
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
		ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		public App MainApp;
		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			Xamarin.FormsMaps.Init(this, bundle);

			MainApp = new App ();

			LoadApplication (MainApp);

			await MainApp.InitializeSocket ();
		}
	}
}