using System;
using System.Collections.Generic;
using System.Text;
using TuGrua.Core.Enums;
using System.Threading.Tasks;
using System.Net;
using System.Json;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TuGrua.Core.Entities
{
	public class Driver : DetailedUser
	{
        public List<Crane> Cranes { get; set; }
        private Driver ()
		{
		}
        
		public static async Task<Driver> Create(Authentication auth, bool isTestUser = false)
        {
			Console.WriteLine (auth);
			return await CreateDriver(auth);
        }

		private static async Task<Driver> CreateDriver(Authentication authUser)
		{
			string uriParams = "?id=" + System.Uri.EscapeDataString (authUser.DetailedUserId._id);

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (
				TuGrua.Core.Backend.Constants.UriServer +
				TuGrua.Core.Backend.Constants.UriApi +
				TuGrua.Core.Backend.Constants.GetDetailedUser + 
				uriParams));

			request.ContentType = "application/json";
			request.Method = "GET";
            //request.Headers["x-access-token"] = authUser.Token;
			request.Headers.Add("x-access-token", authUser.Token);
            try 
			{

				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (var stream = response.GetResponseStream ()) 
					{
						// Use this stream to build a JSON document object:
						try 
						{
							JsonValue jsonDoc = await Task.Run (() => JsonObject.Load (stream));
							if (jsonDoc != null) {
                               
                                JsonObject result = jsonDoc as JsonObject;
								return new Driver () 
								{
									_id = (string)result["_id"],
									PhoneNumber = (string)result["PhoneNumber"],
									Name = (string)result["Name"],
									LastName = (string)result["LastName"],
									GeneralCalification = (int)result["GeneralCalification"]
								};
							}

						} 
						catch (Exception)
						{
							return null;
						}

					}
				}
			} 
			catch (System.Net.WebException ex) 
			{
				//if (ex.Status == WebExceptionStatus.ProtocolError) {
				//	var response = ex.Response as HttpWebResponse;
				//	if (response != null) {
				//		switch ((int)response.StatusCode) {
				//		case 404:
				//			//DisplayAlert ("Aviso", "La aplicación no está actualizada, ya que está accediendo a rutas inexistentes", "OK");
				//			break;
				//		default:
				//			break;
				//		}

				//	}
				//} 
				//else if (ex.Status == WebExceptionStatus.ConnectFailure) 
				//{
				//	//DisplayAlert ("Aviso", "La aplicación no está disponible.", "OK");
				//}
			}
			return null;
		}
    }
}

