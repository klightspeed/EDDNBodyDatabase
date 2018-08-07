using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanBaseStruct
    {
        public class ComparerClass : IEqualityComparer<BodyScanBaseStruct>, IComparer<BodyScanBaseStruct>
        {
            public int Compare(BodyScanBaseStruct x, BodyScanBaseStruct y)
            {
                int bodycompare = BodyScanSystemBodyStruct.Comparer.Compare(x.SystemBody, y.SystemBody);

                if (!BodyScanSystemBodyStruct.Comparer.Equals(x.SystemBody, y.SystemBody))
                {
                    return bodycompare;
                }
                else if (x.Data.HasOrbit != y.Data.HasOrbit)
                {
                    return x.Data.HasOrbit.CompareTo(y.Data.HasOrbit);
                }
                else if (x.Data.Eccentricity != y.Data.Eccentricity)
                {
                    return x.Data.Eccentricity.CompareTo(y.Data.Eccentricity);
                }
                else if (x.Data.OrbitalInclination != y.Data.OrbitalInclination)
                {
                    return x.Data.OrbitalInclination.CompareTo(y.Data.OrbitalInclination);
                }
                else if (x.Data.Periapsis != y.Data.Periapsis)
                {
                    return x.Data.Periapsis.CompareTo(y.Data.Periapsis);
                }
                else if (x.Data.Radius != y.Data.Radius)
                {
                    return x.Data.Radius.CompareTo(y.Data.Radius);
                }
                else
                {
                    return bodycompare;
                }
            }

            public bool Equals(BodyScanBaseStruct x, BodyScanBaseStruct y)
            {
                return BodyScanSystemBodyStruct.Comparer.Equals(x.SystemBody, y.SystemBody) &&
                       x.Data.Eccentricity == y.Data.Eccentricity &&
                       x.Data.OrbitalInclination == y.Data.OrbitalInclination &&
                       x.Data.Periapsis == y.Data.Periapsis &&
                       x.Data.Radius == y.Data.Radius &&
                       x.Data.HasOrbit == y.Data.HasOrbit;
            }

            public int GetHashCode(BodyScanBaseStruct obj)
            {
                return BodyScanSystemBodyStruct.Comparer.GetHashCode(obj.SystemBody) ^
                       obj.Data.HasOrbit.GetHashCode() ^
                       obj.Data.Eccentricity.GetHashCode() ^
                       obj.Data.OrbitalInclination.GetHashCode() ^
                       obj.Data.Periapsis.GetHashCode() ^
                       obj.Data.Radius.GetHashCode();
            }
        }

        public static readonly ComparerClass Comparer = new ComparerClass();

        public struct DataStruct : IComparable<DataStruct>, IEquatable<DataStruct>, IStructEquatable<DataStruct>
        {
            public float Eccentricity;
            public float OrbitalInclination;
            public float Periapsis;
            public float Radius;
            public bool HasOrbit;

            public override bool Equals(object obj)
            {
                return obj != null &&
                       obj is DataStruct other &&
                       this.Equals(other);
            }

            public override int GetHashCode()
            {
                long hc = (Eccentricity.GetHashCode() * 13L) ^
                          (OrbitalInclination.GetHashCode() * 41L) ^
                          (Periapsis.GetHashCode() * 179L) ^
                          (Radius.GetHashCode() * 1063) ^
                          (HasOrbit ? 0 : 326851121);
                return unchecked((int)hc ^ (int)(hc >> 31));
            }

            public bool Equals(DataStruct other)
            {
                return Equals(in this, in other);
            }

            public bool Equals(in DataStruct other)
            {
                return Equals(in this, in other);
            }

            public static bool Equals(in DataStruct x, in DataStruct y)
            {
                return x.Eccentricity == y.Eccentricity &&
                       x.OrbitalInclination == y.OrbitalInclination &&
                       x.Periapsis == y.Periapsis &&
                       x.Radius == y.Radius &&
                       x.HasOrbit == y.HasOrbit;
            }

            public int CompareTo(DataStruct other)
            {
                if (this.HasOrbit != other.HasOrbit)
                {
                    return this.HasOrbit.CompareTo(other.HasOrbit);
                }
                else if (this.Radius != other.Radius)
                {
                    return this.Radius.CompareTo(other.Radius);
                }
                else if (this.Eccentricity != other.Eccentricity)
                {
                    return this.Eccentricity.CompareTo(other.Eccentricity);
                }
                else if (this.OrbitalInclination != other.OrbitalInclination)
                {
                    return this.OrbitalInclination.CompareTo(other.OrbitalInclination);
                }
                else
                {
                    return this.Periapsis.CompareTo(other.Periapsis);
                }
            }
        }

        public BodyScanSystemBodyStruct SystemBody;
        public DataStruct Data;

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is BodyScanBaseStruct other &&
                   this.Data.Equals(other.Data) &&
                   this.SystemBody.Equals(other.SystemBody);
        }

        public override int GetHashCode()
        {
            return this.SystemBody.GetHashCode() ^ this.Data.GetHashCode();
        }
    }
}
