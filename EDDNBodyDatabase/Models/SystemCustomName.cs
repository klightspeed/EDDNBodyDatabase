using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class SystemCustomName
    {
        public int Id { get; set; }
        public long SystemAddress { get; set; }
        public string CustomName { get; set; }
    }
}
