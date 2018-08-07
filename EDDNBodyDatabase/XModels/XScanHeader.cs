using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 32 bytes per entry
    public struct XScanHeader : IEquatable<XScanHeader>, IWithParentId, IStructEquatable<XScanHeader>, IWithNextId
    {
        private static readonly DateTime ScanTimeZero = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Flags]
        public enum ScanFlags : short
        {
            None = 0x0000,
            HasParents = 0x0001,
            HasAxialTilt = 0x0002,
            HasBodyID = 0x0004,
            HasSystemAddress = 0x0008,
            HasLuminosity = 0x0010,
            HasComposition = 0x0020,
            IsMaterialsDict = 0x0040,
            IsBasicScan = 0x0080,
            IsPos3SigFig = 0x0100,
            HasAtmosphereType = 0x0200,
            HasAtmosphereComposition = 0x0400,
            ScanTypeNone = 0x1000,
            ScanTypeBasic = 0x2000,
            ScanTypeDetailed = 0x3000,
            ScanTypeNavBeaconDetail = 0x4000,
            ScanTypeMask = 0x7000
        };

        public DateTime GatewayTimestamp;
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public int DbId { get; set; }
        public int ScanTimestampSeconds;
        public float DistanceFromArrivalLS;
        public short SoftwareVersionId;
        public short FlagsData;

        public ScanFlags Flags
        {
            get
            {
                return (ScanFlags)FlagsData;
            }
            set
            {
                FlagsData = (short)value;
            }
        }

        public byte ScanTypeId
        {
            get
            {
                return (byte)((int)Flags >> 12);
            }
        }

        public DateTime ScanTimestamp
        {
            get
            {
                return ScanTimeZero.AddSeconds(ScanTimestampSeconds);
            }
            set
            {
                ScanTimestampSeconds = (int)value.Subtract(ScanTimeZero).TotalSeconds;
            }
        }

        public string SoftwareName
        {
            get
            {
                return BodyDatabase.GetSoftwareVersion(SoftwareVersionId)?.SoftwareName;
            }
        }

        public string SoftwareVersion
        {
            get
            {
                return BodyDatabase.GetSoftwareVersion(SoftwareVersionId)?.Version;
            }
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanHeader other &&
                   Equals(in this, in other);
        }

        public bool Equals(XScanHeader other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanHeader other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanHeader x, in XScanHeader y)
        {
            return x.ParentId == y.ParentId &&
                   x.GatewayTimestamp == y.GatewayTimestamp &&
                   x.SoftwareVersionId == y.SoftwareVersionId &&
                   x.ScanTimestampSeconds == y.ScanTimestampSeconds &&
                   x.DistanceFromArrivalLS == y.DistanceFromArrivalLS &&
                   x.Flags == y.Flags;
        }

        public static int GetHashCode(in XScanHeader x)
        {
            long hc = (x.GatewayTimestamp.GetHashCode() % 509) + x.ParentId * 509;
            return unchecked((int)hc ^ (int)(hc >> 31));
        }

        public static XScanHeader From(in XScanData scan)
        {
            return new XScanHeader
            {
                GatewayTimestamp = scan.GatewayTimestamp,
                ScanTimestampSeconds = (int)scan.ScanTimestamp.Subtract(ScanTimeZero).TotalSeconds,
                DistanceFromArrivalLS = scan.DistanceFromArrivalLS,
                SoftwareVersionId = scan.SoftwareVersionId,
                Flags = (scan.HasParents ? ScanFlags.HasParents : ScanFlags.None) |
                        (scan.HasAxialTilt ? ScanFlags.HasAxialTilt : ScanFlags.None) |
                        (scan.HasBodyID ? ScanFlags.HasBodyID : ScanFlags.None) |
                        (scan.HasSystemAddress ? ScanFlags.HasSystemAddress : ScanFlags.None) |
                        (scan.HasLuminosity ? ScanFlags.HasLuminosity : ScanFlags.None) |
                        (scan.HasComposition ? ScanFlags.HasComposition : ScanFlags.None) |
                        (scan.IsMaterialsDict ? ScanFlags.IsMaterialsDict : ScanFlags.None) |
                        (scan.IsBasicScan ? ScanFlags.IsBasicScan : ScanFlags.None) |
                        (scan.IsPos3SigFig ? ScanFlags.IsPos3SigFig : ScanFlags.None) |
                        (scan.HasAtmosphereType ? ScanFlags.HasAtmosphereType : ScanFlags.None) |
                        (scan.HasAtmosphereComposition ? ScanFlags.HasAtmosphereComposition : ScanFlags.None) |
                        (ScanFlags)(scan.ScanTypeId << 12)
            };
        }
    }
}
