using System;

using Xamarin.Forms;
using TuGrua.Core.Entities;
using Xamarin.Forms.Maps;

namespace TuGrua
{
	public class AdminApp : ContentPage
	{

		public static TuGrua.AdminApp ThisPage { get; set; }

		private Authentication _auth;
		private Admin _admin;

		// Elements
		public Label _title;
		public Label _name;
		public Button _retrieveAllUsers;
		public Button _deleteUser;

		public AdminApp (Authentication auth)
		{
			_auth = auth;
			validateUser ();

			SetUI ();
		}

		private void SetUI()
		{
			//Set Labels
			_title = new Label();
			_name = new Label ();
			_title.FontSize = 32;

			// Set Buttons
			_retrieveAllUsers = new Button();
			_retrieveAllUsers.Text = "Consultar todos los usuarios";
			_deleteUser = new Button ();
			_deleteUser.Text = "Borrar usuario";
			_deleteUser.BackgroundColor = new Color (201, 2, 2);
			_deleteUser.TextColor = new Color (255, 255, 255);

			// Setting layout
			var stack = new StackLayout { Spacing = 0 };

			stack.Children.Add (_title);
			stack.Children.Add (_name);
			stack.Children.Add (_retrieveAllUsers);

			Content = stack;
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
				_admin = await Admin.Create(_auth);
				if (_admin == null) 
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

