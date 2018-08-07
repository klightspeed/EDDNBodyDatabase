using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanRingsStruct : IEquatable<BodyScanRingsStruct>, IStructEquatable<BodyScanRingsStruct>
    {
        public BodyScanRingStruct RingA;
        public BodyScanRingStruct RingB;
        public BodyScanRingStruct RingC;
        public BodyScanRingStruct RingD;

        public bool Equals(BodyScanRingsStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanRingsStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanRingsStruct x, in BodyScanRingsStruct y)
        {
            return BodyScanRingStruct.Equals(in x.RingA, in y.RingA) &&
                   BodyScanRingStruct.Equals(in x.RingB, in y.RingB) &&
                   BodyScanRingStruct.Equals(in x.RingC, in y.RingC) &&
                   BodyScanRingStruct.Equals(in x.RingD, in y.RingD);
        }

        public static BodyScanRingsStruct FromJSON(JObject jo)
        {
            BodyScanRingStruct[] rings = new BodyScanRingStruct[4];
            List<BodyScanRingStruct> ringlist = jo["Rings"]?.OfType<JObject>().Select((e, i) => BodyScanRingStruct.FromJSON(e, jo.Value<string>("BodyName"), i)).ToList();
            ringlist?.CopyTo(0, rings, 0, ringlist.Count < 4 ? ringlist.Count : 4);

            return new BodyScanRingsStruct
            {
                RingA = rings[0],
                RingB = rings[1],
                RingC = rings[2],
                RingD = rings[3],
            };
        }
    }
}
