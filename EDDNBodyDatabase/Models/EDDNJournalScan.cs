using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class EDDNJournalScan
    {
        public static readonly DateTime ScanTimeZero = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public int Id { get; set; }
        public int BodyScanId { get; set; }
        public long GatewayTimestampTicks { get; set; }
        public int ScanTimestampSeconds { get; set; }
        public short SoftwareVersionId { get; set; }
        public byte? ScanTypeId { get; set; }
        public float DistanceFromArrivalLS { get; set; }
        public bool HasSystemAddress { get; set; }
        public bool HasBodyID { get; set; }
        public bool HasParents { get; set; }
        public bool HasComposition { get; set; }
        public bool HasAxialTilt { get; set; }
        public bool HasLuminosity { get; set; }
        public bool IsMaterialsDict { get; set; }
        public bool IsBasicScan { get; set; }
        public bool IsPos3SigFig { get; set; }
        public bool HasAtmosphereType { get; set; }
        public bool HasAtmosphereComposition { get; set; }

        public string JsonExtra { get { return JsonExtraRef?.JsonExtra; } set { JsonExtraRef = value == null ? null : new EDDNJournalScanJsonExtra { Id = this.Id, JsonExtra = value }; } }

        public string ScanType { get { return BodyDatabase.ScanType.GetName(ScanTypeId); } set { ScanTypeId = BodyDatabase.ScanType.GetId(value); } }

        public DateTime ScanTimestamp { get { return ScanTimeZero.AddSeconds(ScanTimestampSeconds); } set { ScanTimestampSeconds = (int)value.Subtract(ScanTimeZero).TotalSeconds; } }
        public DateTime GatewayTimestamp { get { return ScanTimeZero.AddTicks(GatewayTimestampTicks); } set { GatewayTimestampTicks = (long)value.Subtract(ScanTimeZero).Ticks; } }

        public SoftwareVersion SoftwareVersion { get; set; }
        public BodyScan ScanData { get; set; }
        public ScanType ScanTypeRef { get; set; }
        public EDDNJournalScanJsonExtra JsonExtraRef { get; set; }

        public static EDDNJournalScan From(XScanHeader jscanstruct, int scanid, BodyScan scan, string jsonextra)
        {
            return new EDDNJournalScan
            {
                BodyScanId = scanid,
                ScanData = scanid == 0 ? scan : null,
                GatewayTimestamp = jscanstruct.GatewayTimestamp,
                ScanTimestampSeconds = jscanstruct.ScanTimestampSeconds,
                SoftwareVersionId = jscanstruct.SoftwareVersionId,
                DistanceFromArrivalLS = jscanstruct.DistanceFromArrivalLS,
                ScanTypeId = jscanstruct.ScanTypeId == 0 ? (byte?)null : jscanstruct.ScanTypeId,
                HasBodyID = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasBodyID),
                HasParents = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasParents),
                HasSystemAddress = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasSystemAddress),
                HasAxialTilt = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasAxialTilt),
                HasComposition = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasComposition),
                HasLuminosity = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasLuminosity),
                IsBasicScan = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.IsBasicScan),
                IsMaterialsDict = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.IsMaterialsDict),
                HasAtmosphereComposition = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasAtmosphereComposition),
                HasAtmosphereType = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.HasAtmosphereType),
                IsPos3SigFig = jscanstruct.Flags.HasFlag(XScanHeader.ScanFlags.IsPos3SigFig),
                JsonExtra = jsonextra
            };
        }
    }
}
