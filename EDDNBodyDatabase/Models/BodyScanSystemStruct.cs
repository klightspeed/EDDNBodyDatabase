using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    [DebuggerDisplay("{StarSystem} @ ({StarPosX},{StarPosY},{StarPosZ})")]
    public struct BodyScanSystemStruct
    {
        public class ComparerClass : IEqualityComparer<BodyScanSystemStruct>, IComparer<BodyScanSystemStruct>
        {
            public int Compare(BodyScanSystemStruct x, BodyScanSystemStruct y)
            {
                int sysnamecompare = StringComparer.OrdinalIgnoreCase.Compare(x.CustomSystemName, y.CustomSystemName);

                if (sysnamecompare != 0)
                {
                    return sysnamecompare;
                }
                else if (!x.Data.Equals(y.Data))
                {
                    return x.Data.CompareTo(y.Data);
                }
                else
                {
                    return StringComparer.Ordinal.Compare(x.CustomSystemName, y.CustomSystemName);
                }
            }

            public bool Equals(BodyScanSystemStruct x, BodyScanSystemStruct y)
            {
                return (x.Data.SystemAddress == 0 || y.Data.SystemAddress == 0 || x.Data.SystemAddress == y.Data.SystemAddress) &&
                       String.Equals(x.CustomSystemName, y.CustomSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                       x.Data.PgSystem.Equals(y.Data.PgSystem) &&
                       x.Data.StarPosX == y.Data.StarPosX &&
                       x.Data.StarPosY == y.Data.StarPosY &&
                       x.Data.StarPosZ == y.Data.StarPosZ;
            }

            public int GetHashCode(BodyScanSystemStruct obj)
            {
                return obj.GetHashCode();
            }
        }

        public static ComparerClass Comparer { get; } = new ComparerClass();

        public struct DataStruct : IComparable<DataStruct>, IEquatable<DataStruct>, IComparable, IStructEquatable<DataStruct>
        {
            public long SystemAddress;
            public SystemStruct PgSystem;
            public float StarPosX;
            public float StarPosY;
            public float StarPosZ;

            public override bool Equals(object obj)
            {
                return obj != null &&
                       obj is DataStruct other &&
                       this.Equals(other);
            }

            public override int GetHashCode()
            {
                return GetHashCode(in this);
            }

            public bool Equals(DataStruct other)
            {
                return Equals(in this, in other);
            }

            public bool Equals(in DataStruct other)
            {
                return Equals(in this, in other);
            }

            public int CompareTo(DataStruct other)
            {
                if (!this.PgSystem.Equals(other.PgSystem))
                {
                    return this.PgSystem.CompareTo(other.PgSystem);
                }
                else if (this.StarPosZ != other.StarPosZ)
                {
                    return this.StarPosZ.CompareTo(other.StarPosZ);
                }
                else if (this.StarPosY != other.StarPosY)
                {
                    return this.StarPosY.CompareTo(other.StarPosY);
                }
                else if (this.StarPosX != other.StarPosX)
                {
                    return this.StarPosX.CompareTo(other.StarPosX);
                }
                else
                {
                    return this.SystemAddress.CompareTo(other.SystemAddress);
                }
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                {
                    return 1;
                }
                else if (obj is DataStruct other)
                {
                    return this.CompareTo(other);
                }
                else
                {
                    throw new ArgumentException("Bad object type", "obj");
                }
            }

            public static bool Equals(in DataStruct x, in DataStruct y)
            {
                return x.SystemAddress == y.SystemAddress &&
                       SystemStruct.Equals(in x.PgSystem, in y.PgSystem) &&
                       x.StarPosX == y.StarPosX &&
                       x.StarPosY == y.StarPosY &&
                       x.StarPosZ == y.StarPosZ;
            }

            public static int GetHashCode(in DataStruct x)
            {
                return SystemStruct.GetHashCode(in x.PgSystem) ^
                       ((double)x.StarPosX).GetHashCode() ^
                       ((double)x.StarPosY + 1 / 64).GetHashCode() ^
                       ((double)x.StarPosZ + 1 / 128).GetHashCode();
            }
        }

        public DataStruct Data;
        public string CustomSystemName;

        public string StarSystem
        {
            get
            {
                if (CustomSystemName != null)
                {
                    return CustomSystemName;
                }
                else
                {
                    return Data.PgSystem.Name;
                }
            }
            set
            {
                if (SystemStruct.TryParse(value, out Data.PgSystem))
                {
                    CustomSystemName = null;
                }
                else
                {
                    Data.PgSystem = new SystemStruct();
                    CustomSystemName = String.Intern(value);
                }
            }
        }

        public static BodyScanSystemStruct FromJSON(JObject jo)
        {
            BodyScanSystemStruct sys = new BodyScanSystemStruct
            {
                Data = new DataStruct
                {
                    SystemAddress = jo.Value<long?>("SystemAddress") ?? 0,
                    StarPosX = (float)(Math.Floor(jo["StarPos"].Value<float>(0) * 32.0 + 0.5) / 32.0),
                    StarPosY = (float)(Math.Floor(jo["StarPos"].Value<float>(1) * 32.0 + 0.5) / 32.0),
                    StarPosZ = (float)(Math.Floor(jo["StarPos"].Value<float>(2) * 32.0 + 0.5) / 32.0),
                }
            };

            sys.StarSystem = jo.Value<string>("StarSystem");

            return sys;
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is BodyScanSystemStruct other &&
                   this.CustomSystemName == other.CustomSystemName &&
                   this.Data.Equals(other.Data);
        }

        public override int GetHashCode()
        {
            return (CustomSystemName?.GetHashCode() ?? 0) ^ Data.GetHashCode();
        }
    }
}
