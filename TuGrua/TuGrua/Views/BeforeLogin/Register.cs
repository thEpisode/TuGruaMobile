using System;
using System.Collections.Generic;
using Xamarin.Forms;
using TuGrua.Extensions;

namespace TuGrua
{
	public class Register : ContentPage
	{
		private Dictionary<string, int> roles = new Dictionary<string, int>
		{
			{"Conductor", 1}, {"Usuario", 2}
		};

		private int _roleSelected = -1;
		
		private CustomButton _registerButton;
		private Label _emailLabel;
		private Label _passLabel;
		private Label _nameLabel;
		private Label _lastNameLabel;
		private CustomEntry _emailText;
		private CustomEntry _passText;
		private CustomEntry _nameText;
		private CustomEntry _lastNameText;
		private Picker _rolesDropDown;

		public Register ()
		{
			SetElements();

			_registerButton.Clicked += _registerButton_Clicked;
			_rolesDropDown.SelectedIndexChanged += _rolesDropDown_SelectedIndexChanged;
		}

		private void _rolesDropDown_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_rolesDropDown.SelectedIndex > -1) {
				string roleText = _rolesDropDown.Items [_rolesDropDown.SelectedIndex];

				_roleSelected = roles [roleText];
			}
		}

		private void _registerButton_Clicked(object sender, EventArgs e)
		{
			string email = string.Empty;
			string pass = string.Empty;
			string name = string.Empty;
			string lastname = string.Empty;

			email = _emailText.Text;
			pass = _passText.Text;
			name = _nameText.Text;
			lastname = _lastNameText.Text;

			if (email.Length > 0 && pass.Length > 0 && name.Length > 0 && lastname.Length > 0 && _roleSelected > 0) {
				RegisterUser (email,pass,name,lastname);
			}

		}

		private void RegisterUser(string email, string pass, string name, string lastname)
		{
		}

		private void SetElements()
		{
			this.Title = "Registrarse";

			_registerButton = new CustomButton
			{
				Text = "Registrarse"
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
			_nameText = new CustomEntry {
				Name = "nameText",
				Placeholder = "Nombre"
			};
			_lastNameText = new CustomEntry {
				Name = "lastNameText",
				Placeholder = "Apellidos"
			};
			_emailLabel = new Label {
				Text = "Email:"
			};
			_passLabel = new Label {
				Text = "Contraseña:"
			};
			_nameLabel = new Label {
				Text = "Nombre:"
			};
			_lastNameLabel = new Label {
				Text = "Apellidos:"
			};

			_rolesDropDown = new Picker {
				Title = "Rol",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			foreach (string roleName in roles.Keys) {
				_rolesDropDown.Items.Add (roleName);
			}

			Content = new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 15,
				BackgroundColor = Color.FromRgba(0,0,0,230),
				Padding = new Thickness (20, 0, 20, 20),
				VerticalOptions = LayoutOptions.StartAndExpand,
				Children = {					
					_emailText,
					_passText,
					_nameText,
					_lastNameText,
					_rolesDropDown,
					_registerButton
				}
			};

			this.BackgroundImage = "gruaBack.jpg";
		}
	}
}


