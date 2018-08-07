using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class SystemBodyCustomName
    {
        public int Id { get; set; }
        public int SystemId { get; set; }
        public short? BodyID { get; set; }
        public string CustomName { get; set; }
    }
}
