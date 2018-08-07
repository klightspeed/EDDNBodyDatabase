using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public static class SystemBodyComparer
    {
        public class ComparerClass : IEqualityComparer<SystemBody>
        {
            public bool Equals(SystemBody x, SystemBody y)
            {
                return x.SystemId == y.SystemId &&
                       x.Stars == y.Stars &&
                       x.IsBelt == y.IsBelt &&
                       x.Planet == y.Planet &&
                       x.Moon1 == y.Moon1 &&
                       x.Moon2 == y.Moon2 &&
                       x.Moon3 == y.Moon3 &&
                       x.CustomNameId == y.CustomNameId &&
                       x.ScanBaseHash == y.ScanBaseHash &&
                       (x.BodyID == -1 || y.BodyID == -1 || x.BodyID == y.BodyID);
            }

            public int GetHashCode(SystemBody obj)
            {
                return obj.GetHashCode();
            }
        }

        public class IgnoreCustomNameComparerClass : IEqualityComparer<SystemBody>
        {
            public bool Equals(SystemBody x, SystemBody y)
            {
                return x.SystemId == y.SystemId &&
                       x.Stars == y.Stars &&
                       x.IsBelt == y.IsBelt &&
                       x.Planet == y.Planet &&
                       x.Moon1 == y.Moon1 &&
                       x.Moon2 == y.Moon2 &&
                       x.Moon3 == y.Moon3 &&
                       x.ScanBaseHash == y.ScanBaseHash &&
                       (x.BodyID == -1 || y.BodyID == -1 || x.BodyID == y.BodyID);
            }

            public int GetHashCode(SystemBody obj)
            {
                return obj.GetHashCode();
            }
        }

        public static IEqualityComparer<SystemBody> Normal => new ComparerClass();
        public static IEqualityComparer<SystemBody> IgnoreCustomName => new IgnoreCustomNameComparerClass();
    }

    public class SystemBody : IEquatable<SystemBody>
    {

        public int Id { get; set; }
        public int SystemId { get; set; }
        public short BodyID { get; set; } = -1;
        public byte Stars { get; set; }
        public byte Planet { get; set; }
        public byte Moon1 { get; set; }
        public byte Moon2 { get; set; }
        public byte Moon3 { get; set; }
        public bool IsBelt { get; set; }
        public int ScanBaseHash { get; set; }
        public short CustomNameId { get; set; }

        public SystemBodyCustomName CustomName { get; set; }
        public SystemBodyDuplicate DuplicateOf { get; set; }
        public virtual System SystemRef { get; set; }

        public static SystemBody From(XModels.XScanClass scan, string customname, System sys)
        {
            var body = scan.Body;

            return new SystemBody
            {
                Id = body.DbId,
                SystemId = sys.Id,
                BodyID = body.BodyID,
                Stars = body.Stars,
                IsBelt = body.IsBelt,
                Planet = body.Planet,
                Moon1 = body.Moon1,
                Moon2 = body.Moon2,
                Moon3 = body.Moon3,
                SystemRef = sys,
                ScanBaseHash = scan.Scan.GetHashCode(),
                CustomNameId = BodyDatabase.BodyCustomName.GetId(customname) ?? 0,
                CustomName = customname == null ? null : new SystemBodyCustomName
                {
                    Id = body.DbId,
                    SystemId = sys.Id,
                    BodyID = body.BodyID,
                    CustomName = customname
                }
            };
        }

        public bool Equals(SystemBody other)
        {
            return this.SystemId == other.SystemId &&
                   this.Stars == other.Stars &&
                   this.IsBelt == other.IsBelt &&
                   this.Planet == other.Planet &&
                   this.Moon1 == other.Moon1 &&
                   this.Moon2 == other.Moon2 &&
                   this.Moon3 == other.Moon3 &&
                   this.BodyID == other.BodyID &&
                   this.CustomNameId == other.CustomNameId &&
                   this.ScanBaseHash == other.ScanBaseHash;
        }

        public override bool Equals(object obj)
        {
            return obj is SystemBody other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            long pghc = (IsBelt ? 13 : 41) * ((((Stars * 19 + Planet) * 17 + Moon1) * 11 + Moon2) * 5 + Moon3);
            long hc = (pghc % 509) + (SystemId * 509);
            return unchecked((int)hc ^ (int)(hc >> 31));
        }
    }
}
