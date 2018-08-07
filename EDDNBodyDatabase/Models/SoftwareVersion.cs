using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class SoftwareVersion
    {
        public short Id { get; set; }
        public byte SoftwareId { get; set; }
        public string SoftwareName { get { return BodyDatabase.Software.GetName(SoftwareId); } set { SoftwareId = (byte)BodyDatabase.Software.GetId(value); } }
        public string Version { get; set; }

        public Software SoftwareRef { get; set; }
    }
}
