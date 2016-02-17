using System;
using System.Collections.Generic;
using TuGrua.Core.Enums;

namespace TuGrua.Core.Entities
{
	public class Authentication
	{
		public string Token{ get; set; }
		public string UserId{ get; set; }
		public DetailedUser DetailedUserId{ get; set; }
		public Role Role{ get; set; }
		public string Email { get; set; }
		public int Status { get; set; }

		public Authentication ()
		{
			
		}
	}
}

