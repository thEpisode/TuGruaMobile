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
			//Console.WriteLine (auth);
			return await CreateDriver(auth);
        }

		private static async Task<Driver> CreateDriver(Authentication authUser)
		{

            return new Driver()
            {
                _id = authUser.DetailedUserId._id,
                PhoneNumber = authUser.DetailedUserId.PhoneNumber,
                Name = authUser.DetailedUserId.Name,
                LastName = authUser.DetailedUserId.LastName,
                GeneralCalification = authUser.DetailedUserId.GeneralCalification
            };
        }
    }
}

