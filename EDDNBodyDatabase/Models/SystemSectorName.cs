using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class SystemSectorName
    {
        public int Id { get; set; }
        public short RegionId { get; set; }
        public string Region { get { return BodyDatabase.Region.GetName(RegionId); } set { RegionId = BodyDatabase.Region.GetId(value) ?? 0; } }
        public byte Mid1a { get; set; }
        public byte Mid1b { get; set; }
        public byte Mid2 { get; set; }
        public byte SizeClass { get; set; }
        public byte Mid3 { get; set; }
        public short Sequence { get; set; }
        public virtual Region RegionRef { get; set; }
    }
}
