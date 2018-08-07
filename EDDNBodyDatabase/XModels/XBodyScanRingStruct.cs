using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public struct XBodyScanRingStruct : IEquatable<XBodyScanRingStruct>, IStructEquatable<XBodyScanRingStruct>
    {
        public byte ClassId;
        public float InnerRad;
        public float OuterRad;
        public float MassMT;
        public bool IsBelt;
        public string Name;
        public string Class { get { return BodyDatabase.RingClass.GetName(ClassId); } set { ClassId = BodyDatabase.RingClass.GetId(value) ?? 0; } }

        public bool Equals(XBodyScanRingStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XBodyScanRingStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XBodyScanRingStruct x, in XBodyScanRingStruct y)
        {
            return x.ClassId == y.ClassId &&
                   x.Name == y.Name &&
                   x.InnerRad == y.InnerRad &&
                   x.OuterRad == y.OuterRad &&
                   x.MassMT == y.MassMT &&
                   x.IsBelt == y.IsBelt;
        }
    }
}
