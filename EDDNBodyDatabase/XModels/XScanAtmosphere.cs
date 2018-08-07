using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 36 bytes per entry
    public struct XScanAtmosphere : IEquatable<XScanAtmosphere>, IWithParentId, IStructEquatable<XScanAtmosphere>, IWithNextId
    {
        [Flags]
        public enum ScanFlags : byte
        {
            None = 0x00,
            AtmosphereHot = 0x01,
            AtmosphereThin = 0x02,
            AtmosphereThick = 0x04,
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public float SurfacePressure;
        public float Component1Amt;
        public float Component2Amt;
        public float Component3Amt;
        public byte Component1Id;
        public byte Component2Id;
        public byte Component3Id;
        public byte AtmosphereId;
        public byte AtmosphereTypeId;
        public ScanFlags Flags;

        public string Atmosphere { get { return BodyDatabase.Atmosphere.GetName(AtmosphereId); } set { AtmosphereId = BodyDatabase.Atmosphere.GetId(value) ?? 0; } }
        public string AtmosphereType { get { return BodyDatabase.AtmosphereType.GetName(AtmosphereTypeId); } set { AtmosphereTypeId = BodyDatabase.AtmosphereType.GetId(value) ?? 0; } }
        public string Component1Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component1Id); } set { Component1Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }
        public string Component2Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component2Id); } set { Component2Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }
        public string Component3Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component3Id); } set { Component3Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanAtmosphere other &&
                   this.Equals(in other);
        }

        public override int GetHashCode()
        {
            return SurfacePressure.GetHashCode() ^
                   Component1Amt.GetHashCode() ^
                   Component2Amt.GetHashCode() ^
                   Component3Amt.GetHashCode() ^
                   (int)Flags ^
                   (AtmosphereId << 8) ^
                   (AtmosphereTypeId << 12) ^
                   (Component1Id << 16) ^
                   (Component2Id << 20) ^
                   (Component3Id << 24);
        }

        public bool Equals(XScanAtmosphere other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanAtmosphere other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanAtmosphere x, in XScanAtmosphere y)
        {
            return x.ParentId == y.ParentId &&
                   x.SurfacePressure == y.SurfacePressure &&
                   x.AtmosphereId == y.AtmosphereId &&
                   x.AtmosphereTypeId == y.AtmosphereTypeId &&
                   x.Component1Id == y.Component1Id &&
                   x.Component1Amt == y.Component1Amt &&
                   x.Component2Id == y.Component2Id &&
                   x.Component2Amt == y.Component2Amt &&
                   x.Component3Id == y.Component3Id &&
                   x.Component3Amt == y.Component3Amt &&
                   x.Flags == y.Flags;
        }

        public static bool UpdateIfEqual(ref XScanAtmosphere x, in XScanAtmosphere y)
        {
            bool match = x.ParentId == y.ParentId &&
                         x.SurfacePressure == y.SurfacePressure &&
                         x.AtmosphereId == y.AtmosphereId &&
                         (x.AtmosphereTypeId == 0 || y.AtmosphereTypeId == 0 || x.AtmosphereTypeId == y.AtmosphereTypeId) &&
                         (x.Component1Id == 0 || y.Component1Id == 0 || (x.Component1Id == y.Component1Id && x.Component1Amt == y.Component1Amt)) &&
                         (x.Component2Id == 0 || y.Component2Id == 0 || (x.Component2Id == y.Component2Id && x.Component2Amt == y.Component2Amt)) &&
                         (x.Component3Id == 0 || y.Component3Id == 0 || (x.Component3Id == y.Component3Id && x.Component3Amt == y.Component3Amt));

            if (match)
            {
                if (x.AtmosphereTypeId == 0 && y.AtmosphereTypeId != 0)
                {
                    x.AtmosphereTypeId = y.AtmosphereTypeId;
                }

                if (x.Component1Id == 0 && y.Component1Id != 0)
                {
                    x.Component1Id = y.Component1Id;
                    x.Component1Amt = y.Component1Amt;
                    x.Component2Id = y.Component2Id;
                    x.Component2Amt = y.Component2Amt;
                    x.Component3Id = y.Component3Id;
                    x.Component3Amt = y.Component3Amt;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static XScanAtmosphere From(in XScanData scan)
        {
            return new XScanAtmosphere
            {
                SurfacePressure = scan.SurfacePressure,
                Component1Amt = scan.AtmosphereComponent1Amt,
                Component2Amt = scan.AtmosphereComponent2Amt,
                Component3Amt = scan.AtmosphereComponent3Amt,
                Component1Id = scan.AtmosphereComponent1Id,
                Component2Id = scan.AtmosphereComponent2Id,
                Component3Id = scan.AtmosphereComponent3Id,
                AtmosphereId = scan.AtmosphereId,
                AtmosphereTypeId = scan.AtmosphereTypeId,
                Flags = (scan.AtmosphereHot ? ScanFlags.AtmosphereHot : ScanFlags.None) |
                        (scan.AtmosphereThin ? ScanFlags.AtmosphereThin : ScanFlags.None) |
                        (scan.AtmosphereThick ? ScanFlags.AtmosphereThick : ScanFlags.None)
            };
        }
    }
}
