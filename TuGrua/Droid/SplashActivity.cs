
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace TuGrua.Droid
{
	[Activity(
		Theme = "@style/Theme.Splash",
		Icon = "@drawable/icon",
		MainLauncher = true, 
		NoHistory = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, 
		ScreenOrientation = ScreenOrientation.Portrait)]		
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			StartActivity(typeof(MainActivity));
		}
	}
}

