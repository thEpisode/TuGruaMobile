using System;
using System.Collections.Generic;
using System.Text;
using TuGrua.Core.Enums;
using System.Threading.Tasks;

namespace TuGrua.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid DetailedUserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
		public int Status { get; set; }
        private static bool _test { get; set; }
        public User()
        {

        }
        public User(bool isTestUser)
        {
            _test = isTestUser;
        }

        public bool isTestUser()
        {
            return _test;
        }
    }
}
