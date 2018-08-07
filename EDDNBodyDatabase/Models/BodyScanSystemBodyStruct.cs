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
    [DebuggerDisplay("{System.StarSystem}/{BodyName} [{BodyID}]")]
    public struct BodyScanSystemBodyStruct
    {
        public class ComparerClass : IEqualityComparer<BodyScanSystemBodyStruct>, IComparer<BodyScanSystemBodyStruct>
        {
            public int Compare(BodyScanSystemBodyStruct x, BodyScanSystemBodyStruct y)
            {
                int namecompare = StringComparer.OrdinalIgnoreCase.Compare(x.BodyName, y.BodyName);

                if (!BodyScanSystemStruct.Comparer.Equals(x.System, y.System))
                {
                    return BodyScanSystemStruct.Comparer.Compare(x.System, y.System);
                }
                else if (namecompare != 0)
                {
                    return namecompare;
                }
                else if (x.BodyID != y.BodyID)
                {
                    return x.BodyID.CompareTo(y.BodyID);
                }
                else if (x.BodyName != y.BodyName)
                {
                    return String.CompareOrdinal(x.BodyName, y.BodyName);
                }
                else
                {
                    return BodyScanSystemStruct.Comparer.Compare(x.System, y.System);
                }
            }

            public bool Equals(BodyScanSystemBodyStruct x, BodyScanSystemBodyStruct y)
            {
                return BodyScanSystemStruct.Comparer.Equals(x.System, y.System) &&
                       x.PgBody.Equals(y.PgBody) &&
                       String.Equals(x.CustomBodyName, y.CustomBodyName, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(BodyScanSystemBodyStruct obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly ComparerClass Comparer = new ComparerClass();

        public struct DataStruct : IComparable<DataStruct>, IEquatable<DataStruct>, IStructEquatable<DataStruct>
        {
            public SystemBodyStruct PgBody;
            public short BodyID;

            public override int GetHashCode()
            {
                return PgBody.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj != null &&
                       obj is DataStruct other &&
                       this.PgBody.Equals(in other.PgBody) &&
                       this.BodyID == other.BodyID;
            }

            public bool Equals(DataStruct other)
            {
                return this.PgBody.Equals(in other.PgBody) &&
                       this.BodyID == other.BodyID;
            }

            public bool Equals(in DataStruct other)
            {
                return this.PgBody.Equals(in other.PgBody) &&
                       this.BodyID == other.BodyID;
            }

            public int CompareTo(DataStruct other)
            {
                if (!this.PgBody.Equals(other.PgBody))
                {
                    return this.PgBody.CompareTo(other.PgBody);
                }
                else
                {
                    return (this.BodyID).CompareTo(other.BodyID);
                }
            }
        }

        public BodyScanSystemStruct System;
        public short BodyID;
        public SystemBodyStruct PgBody;
        public string CustomBodyName;

        public DataStruct BaseData
        {
            get
            {
                return new DataStruct
                {
                    PgBody = this.PgBody,
                    BodyID = this.BodyID
                };
            }
        }

        public string BodyName
        {
            get
            {
                if (CustomBodyName != null)
                {
                    return CustomBodyName;
                }
                else
                {
                    return PgBody.GetName(System.StarSystem);
                }
            }
            set
            {
                if (SystemBodyStruct.TryParse(value, System.StarSystem, out PgBody))
                {
                    CustomBodyName = null;
                }
                else
                {
                    PgBody = new SystemBodyStruct();
                    CustomBodyName = value;
                }
            }
        }

        public static BodyScanSystemBodyStruct FromJSON(JObject jo)
        {
            return new BodyScanSystemBodyStruct
            {
                System = BodyScanSystemStruct.FromJSON(jo),
                BodyID = jo.Value<short?>("BodyID") ?? -1,
                BodyName = jo.Value<string>("BodyName"),
            };
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is BodyScanSystemBodyStruct other &&
                   this.System.Equals(other.System) &&
                   this.PgBody.Equals(other.PgBody) &&
                   this.CustomBodyName == other.CustomBodyName &&
                   this.BodyID == other.BodyID;
        }

        public override int GetHashCode()
        {
            long hc = (System.GetHashCode() * 174440041L) ^
                      (CustomBodyName?.ToLowerInvariant().GetHashCode() ?? 0) ^
                      PgBody.GetHashCode();
            return unchecked((int)hc ^ (int)(hc >> 31));
        }
    }
}
