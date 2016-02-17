using System;
using TuGrua.Core.Enums;
using System.Collections.Generic;

namespace TuGrua.Core.Entities
{
	public class DetailedUser
	{
        public string _id { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int GeneralCalification { get; set; }
        private bool _test { get; set; }

        public DetailedUser ()
		{
            
		}
        public DetailedUser(bool isTestUser)
        {
            _test = isTestUser;
        }
        
        public bool isTestUser()
        {
            return _test;
        }
    }
}

