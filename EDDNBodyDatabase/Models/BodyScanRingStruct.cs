using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanRingStruct : IEquatable<BodyScanRingStruct>, IStructEquatable<BodyScanRingStruct>
    {
        public byte ClassId;
        public float InnerRad;
        public float OuterRad;
        public float MassMT;
        public bool IsBelt;
        public string Name;
        public string Class { get { return BodyDatabase.RingClass.GetName(ClassId); } set { ClassId = BodyDatabase.RingClass.GetId(value) ?? 0; } }

        public bool Equals(BodyScanRingStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanRingStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanRingStruct x, in BodyScanRingStruct y)
        {
            return x.ClassId == y.ClassId &&
                   x.Name == y.Name &&
                   x.InnerRad == y.InnerRad &&
                   x.OuterRad == y.OuterRad &&
                   x.MassMT == y.MassMT &&
                   x.IsBelt == y.IsBelt;
        }

        public static BodyScanRingStruct FromJSON(JObject jo, string bodyname, int ringnum)
        {
            string ringname = jo.Value<string>("Name");
            byte ringclassid = BodyDatabase.RingClass.GetId(jo.Value<string>("RingClass")) ?? 0;
            float innerrad = jo.Value<float>("InnerRad");
            float outerrad = jo.Value<float>("OuterRad");
            float massmt = jo.Value<float>("MassMT");
            bool isbelt = ringname.EndsWith(" Belt", StringComparison.InvariantCultureIgnoreCase);

            if (ringname.Equals($"{bodyname} {(char)('A' + ringnum)} {(isbelt ? "Belt" : "Ring")}", StringComparison.InvariantCultureIgnoreCase))
            {
                ringname = null;
            }

            return new BodyScanRingStruct
            {
                Name = ringname,
                ClassId = ringclassid,
                InnerRad = innerrad,
                OuterRad = outerrad,
                MassMT = massmt,
                IsBelt = isbelt
            };
        }
    }
}
