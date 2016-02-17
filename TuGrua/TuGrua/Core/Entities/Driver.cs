using System;
using System.Collections.Generic;
using System.Text;
using TuGrua.Core.Enums;
using System.Threading.Tasks;
using System.Net;
using System.Json;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.IO;

namespace TuGrua.Core.Entities
{
	public class Driver : DetailedUser
	{
		public List<Crane> Cranes { get; set; }

		private byte[] _dataToGet;

        private Driver ()
		{
		}
        
		public static async Task<Driver> Create(Authentication auth, bool isTestUser = false)
        {
			//Console.WriteLine (auth);
			return await CreateDriver(auth);
        }

		private static async Task<Driver> CreateDriver(Authentication authUser)
		{
			Driver driver = null;
			TaskCompletionSource<Driver> completion = new TaskCompletionSource<Driver>();
			System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(
				TuGrua.Core.Backend.Constants.UriServer +
				TuGrua.Core.Backend.Constants.UriApi +
				TuGrua.Core.Backend.Constants.GetDetailedUser + 
				"?id=" + authUser.DetailedUserId._id);

			request.Method = "GET";
			request.Accept = "application/json";
			request.Headers["x-access-token"] = authUser.Token;

			try {
				request.BeginGetResponse(ar =>
					{
						try
						{
							using (var response = request.EndGetResponse(ar))
							{
								using (var reader = new StreamReader(response.GetResponseStream()))
								{
									var responseObject = JsonConvert.DeserializeObject<Driver>(reader.ReadToEnd());
									driver = responseObject;
								}
							}
						}

						catch (Exception)
						{

						}
						completion.SetResult(driver);
					}, null);
			} catch (Exception) 
			{
				
			}


			
			return (completion.Task).Result;
			/* = new Driver()
            {
                _id = authUser.DetailedUserId._id,
                PhoneNumber = authUser.DetailedUserId.PhoneNumber,
                Name = authUser.DetailedUserId.Name,
                LastName = authUser.DetailedUserId.LastName,
                GeneralCalification = authUser.DetailedUserId.GeneralCalification
            };    */
		}


    }
}

