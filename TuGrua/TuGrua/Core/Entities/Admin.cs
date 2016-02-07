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
	public class Admin : DetailedUser
	{
		public Admin ()
		{
		}

		public static async Task<Admin> Create(Authentication auth, bool isTestUser = false)
		{
			return await CreateAdmin(auth);
		}

		private static async Task<Admin> CreateAdmin(Authentication authUser)
		{
            return new Admin()
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

