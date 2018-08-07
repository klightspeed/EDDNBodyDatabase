using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 20 bytes per entry
    public struct XBody : IEquatable<XBody>, IWithParentId, IStructEquatable<XBody>, IWithNextId
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public int DbId { get; set; }
        public short BodyID;
        public byte Stars;
        public byte Planet;
        public byte Moon1;
        public byte Moon2;
        public byte Moon3;
        public bool IsBelt;

        public static XBody From(in SystemBodyStruct body, short bodyid)
        {
            return new XBody
            {
                BodyID = bodyid,
                Stars = body.Stars,
                Planet = body.Planet,
                Moon1 = body.Moon1,
                Moon2 = body.Moon2,
                Moon3 = body.Moon3,
                IsBelt = body.IsBelt,
            };
        }

        public static XBody From(in XScanData body)
        {
            return new XBody
            {
                BodyID = body.BodyID,
                Stars = body.PgBody.Stars,
                Planet = body.PgBody.Planet,
                Moon1 = body.PgBody.Moon1,
                Moon2 = body.PgBody.Moon2,
                Moon3 = body.PgBody.Moon3,
                IsBelt = body.PgBody.IsBelt,
            };
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XBody other &&
                   this.Equals(in other);
        }

        public bool Equals(XBody other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XBody other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XBody x, in XBody y)
        {
            return x.ParentId == y.ParentId &&
                   x.BodyID == y.BodyID &&
                   x.Stars == y.Stars &&
                   x.Planet == y.Planet &&
                   x.Moon1 == y.Moon1 &&
                   x.Moon2 == y.Moon2 &&
                   x.Moon3 == y.Moon3 &&
                   x.IsBelt == y.IsBelt;
        }

        public static int GetHashCode(in XBody x)
        {
            if (x.ParentId == 0)
            {
                throw new InvalidOperationException("No parent");
            }

            long pghc = (x.IsBelt ? 13 : 41) * ((((x.Stars * 19 + x.Planet) * 17 + x.Moon1) * 11 + x.Moon2) * 5 + x.Moon3);
            long hc = (pghc % 509) + (x.ParentId * 509);
            return unchecked((int)hc ^ (int)(hc >> 31));
        }
    }
}
