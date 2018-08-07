using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanRingCustomName
    {
        public int ScanId { get; set; }
        public byte RingNum { get; set; }
        public string Name { get; set; }
    }
}
