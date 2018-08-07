using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class SystemBodyDuplicate
    {
        public int Id { get; set; }
        public int DuplicateOfBodyId { get; set; }
        public short BodyID { get; set; }
        public string Name { get; set; }
        public int ScanBaseHash { get; set; }

        public SystemBody Duplicate { get; set; }
        public SystemBody Body { get; set; }
    }
}
