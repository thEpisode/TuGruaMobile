using System;
using System.Collections.Generic;
using System.Text;

namespace TuGrua.Core.Entities
{
    public class Crane
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public bool Active { get; set; }
    }
}
