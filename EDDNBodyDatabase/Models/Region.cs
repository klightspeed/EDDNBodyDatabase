using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EDDNBodyDatabase.Models
{
    [DebuggerDisplay("{Name}: ({X0},{Y0},{Z0})")]
    public class Region : INameIdMap<short>
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public float? X0 { get; set; }
        public float? Y0 { get; set; }
        public float? Z0 { get; set; }
        public int? SizeX { get; set; }
        public int? SizeY { get; set; }
        public int? SizeZ { get; set; }
    }
}
