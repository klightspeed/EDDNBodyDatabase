using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 40 bytes per entry
    public struct XSystem : IEquatable<XSystem>, IWithId, IStructEquatable<XSystem>, IWithNextId
    {
        public long SystemAddress;
        private int _id;
        public int Id { get => _id; set => _id = value; }
        public int NextId { get; set; }
        public int DbId { get; set; }
        public float StarPosX;
        public float StarPosY;
        public float StarPosZ;
        public short RegionId { get; set; }
        public short Sequence { get; set; }
        public byte Mid1a { get; set; }
        public byte Mid1b { get; set; }
        public byte Mid2 { get; set; }
        public byte SizeClass { get; set; }
        public byte Mid3 { get; set; }

        public static XSystem From(in XScanData sys)
        {
            return new XSystem
            {
                SystemAddress = sys.SystemAddress,
                StarPosX = sys.StarPosX,
                StarPosY = sys.StarPosY,
                StarPosZ = sys.StarPosZ,
                RegionId = sys.PgSystemName.RegionId,
                Sequence = sys.PgSystemName.Sequence,
                Mid1a = sys.PgSystemName.Mid1a,
                Mid1b = sys.PgSystemName.Mid1b,
                Mid2 = sys.PgSystemName.Mid2,
                SizeClass = sys.PgSystemName.SizeClass,
                Mid3 = sys.PgSystemName.Mid3,
            };
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XSystem other &&
                   this.Equals(in other);
        }

        public bool Equals(XSystem other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XSystem other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XSystem x, in XSystem y)
        {
            if (x._id != 0 && y._id != 0)
            {
                return x._id == y._id;
            }
            else
            {
                return x.SystemAddress == y.SystemAddress &&
                       x.RegionId == y.RegionId &&
                       x.Mid1a == y.Mid1a &&
                       x.Mid1b == y.Mid1b &&
                       x.Mid2 == y.Mid2 &&
                       x.Mid3 == y.Mid3 &&
                       x.SizeClass == y.SizeClass &&
                       x.Sequence == y.Sequence &&
                       x.StarPosX == y.StarPosX &&
                       x.StarPosY == y.StarPosY &&
                       x.StarPosZ == y.StarPosZ;
            }
        }

        public static int GetHashCode(in XSystem x)
        {
            return (x.RegionId * 52711) ^
                   (((((x.Mid3 * 26 + x.Mid2) * 26 + x.Mid1b) * 26 + x.Mid1a) | (x.SizeClass << 21)) * 127) ^
                   (x.Sequence * 3) ^
                   ((double)x.StarPosX).GetHashCode() ^
                   ((double)x.StarPosY + 1 / 64).GetHashCode() ^
                   ((double)x.StarPosZ + 1 / 128).GetHashCode();
        }
    }
}
