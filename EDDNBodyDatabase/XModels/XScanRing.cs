using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 32 bytes per entry
    public struct XScanRing : IEquatable<XScanRing>, IWithParentId, IStructEquatable<XScanRing>, IWithNextId
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public int NextRingId;
        public float InnerRad;
        public float OuterRad;
        public float MassMT;
        public byte ClassId;
        public byte RingNumber;
        public bool IsBelt;

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanRing other &&
                   Equals(in this, in other);
        }

        public override int GetHashCode()
        {
            return InnerRad.GetHashCode() ^
                   OuterRad.GetHashCode() ^
                   MassMT.GetHashCode() ^
                   (ClassId * 19) ^
                   (RingNumber * 5) ^
                   (IsBelt ? 0 : 1);
        }

        public bool Equals(XScanRing other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanRing other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanRing x, in XScanRing y)
        {
            return x.ParentId == y.ParentId &&
                   x.RingNumber == y.RingNumber &&
                   x.ClassId == y.ClassId &&
                   x.InnerRad == y.InnerRad &&
                   x.OuterRad == y.OuterRad &&
                   x.MassMT == y.MassMT &&
                   x.IsBelt == y.IsBelt;
        }

        public static XScanRing From(in XBodyScanRingStruct scan, int ringnum)
        {
            return new XScanRing
            {
                RingNumber = (byte)ringnum,
                InnerRad = scan.InnerRad,
                OuterRad = scan.OuterRad,
                MassMT = scan.MassMT,
                ClassId = scan.ClassId,
                IsBelt = scan.IsBelt,
            };
        }

        public static XScanRing[] From(in XScanData ent, out string[] names)
        {
            names = new string[]
            {
                ent.RingA.Name,
                ent.RingB.Name,
                ent.RingC.Name,
                ent.RingD.Name
            };

            return new XScanRing[]
            {
                From(ent.RingA, 0),
                From(ent.RingB, 1),
                From(ent.RingC, 2),
                From(ent.RingD, 3)
            };
        }
    }
}
