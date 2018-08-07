using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 24 bytes per entry
    public struct XScanStar : IEquatable<XScanStar>, IWithParentId, IStructEquatable<XScanStar>, IWithNextId
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public float AbsoluteMagnitude;
        public float StellarMass;
        public short Age_MY;
        public byte StarTypeId;
        public byte LuminosityId;

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanStar other &&
                   Equals(in this, in other);
        }

        public override int GetHashCode()
        {
            return AbsoluteMagnitude.GetHashCode() ^
                   StellarMass.GetHashCode() ^
                   (Age_MY * 65539) ^
                   (StarTypeId * 277) ^
                   (LuminosityId * 19);
        }

        public bool Equals(XScanStar other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanStar other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanStar x, in XScanStar y)
        {
            return x.ParentId == y.ParentId &&
                   x.AbsoluteMagnitude == y.AbsoluteMagnitude &&
                   x.StellarMass == y.StellarMass &&
                   x.Age_MY == y.Age_MY &&
                   x.StarTypeId == y.StarTypeId &&
                   x.LuminosityId == y.LuminosityId;
        }

        public static bool UpdateIfEqual(ref XScanStar x, in XScanStar y)
        {
            bool match = x.ParentId == y.ParentId &&
                   x.AbsoluteMagnitude == y.AbsoluteMagnitude &&
                   x.StellarMass == y.StellarMass &&
                   x.Age_MY == y.Age_MY &&
                   x.StarTypeId == y.StarTypeId &&
                   (x.LuminosityId == 0 || y.LuminosityId == 0 || x.LuminosityId == y.LuminosityId);

            if (match)
            {
                if (x.LuminosityId == 0 || y.LuminosityId != 0)
                {
                    x.LuminosityId = y.LuminosityId;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static XScanStar From(in XScanData scan)
        {
            return new XScanStar
            {
                AbsoluteMagnitude = scan.AbsoluteMagnitude,
                StellarMass = scan.StellarMass,
                Age_MY = scan.Age_MY,
                StarTypeId = scan.StarTypeId,
                LuminosityId = scan.LuminosityId,
            };
        }
    }
}
