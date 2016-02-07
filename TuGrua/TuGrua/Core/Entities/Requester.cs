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
    public class Requester : DetailedUser
    {
        private Requester()
        {
            
        }

		public static async Task<Requester> Create(Authentication auth, bool isTestUser = false)
        {
			return await CreateRequester(auth);
        }

		private static async Task<Requester> CreateRequester(Authentication authUser)
		{
            return new Requester()
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
