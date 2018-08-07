using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 32 bytes per entry
    public struct XScanBase : IEquatable<XScanBase>, IWithId, IStructEquatable<XScanBase>, IWithNextId
    {
        [Flags]
        public enum ScanFlags : byte
        {
            None = 0x00,
            HasOrbit = 0x01,
            IsStar = 0x02,
            IsPlanet = 0x04,
        }

        public int Id { get; set; }
        public int NextId { get; set; }
        public float EccentricityAge;
        public float OrbitalInclinationStarType;
        public float Periapsis;
        public float Radius;
        public float Mass;
        public ScanFlags Flags;
        public uint RadiusBin { get { return BitConverter.ToUInt32(BitConverter.GetBytes(Radius), 0); } }
        public uint MassBin { get { return BitConverter.ToUInt32(BitConverter.GetBytes(Mass), 0); } }
        public string RadiusHex { get { return RadiusBin.ToString("X8"); } }
        public string MassHex { get { return MassBin.ToString("X8"); } }

        public float Eccentricity
        {
            get
            {
                return Flags.HasFlag(ScanFlags.HasOrbit) ? EccentricityAge : 0;
            }
        }

        public float OrbitalInclination
        {
            get
            {
                return Flags.HasFlag(ScanFlags.HasOrbit) ? OrbitalInclinationStarType : 0;
            }
        }

        private static XScanBase From(float eccage, float inctemp, float mass, float radius, float periapsis, bool hasorbit, short age, byte startypeid, bool isstar, bool isplanet)
        {
            if (!hasorbit)
            {
                eccage = age + 1;
                inctemp = startypeid + 360;
            }

            //mass = (float)Math.Floor(mass * 1024.0F) / 1024.0F;
            mass = BitConverter.ToSingle(BitConverter.GetBytes(BitConverter.ToUInt32(BitConverter.GetBytes(mass), 0) & 0xFFFC0000U), 0);
            radius = BitConverter.ToSingle(BitConverter.GetBytes(BitConverter.ToUInt32(BitConverter.GetBytes(radius), 0) & 0xFFFC0000U), 0);
            periapsis = (float)Math.Floor(periapsis);
            eccage = (float)Math.Floor(eccage * 1024.0 + 0.5) / 1024.0F;
            inctemp = (float)Math.Floor(inctemp * 16.0 + 0.5) / 16.0F;

            return new XScanBase
            {
                EccentricityAge = eccage,
                OrbitalInclinationStarType = inctemp,
                Periapsis = periapsis,
                Radius = radius,
                Mass = mass,
                Flags = (isstar ? ScanFlags.IsStar : ScanFlags.None) |
                        (isplanet ? ScanFlags.IsPlanet : ScanFlags.None) |
                        (hasorbit ? ScanFlags.HasOrbit : ScanFlags.None)
            };

        }

        public static XScanBase From(in XScanData scan)
        {
            float eccage = scan.Eccentricity;
            float inctemp = scan.OrbitalInclination;
            float mass = scan.IsStar ? scan.StellarMass : scan.MassEM;
            float radius = scan.Radius;
            float periapsis = scan.Periapsis;
            bool hasorbit = scan.Periapsis != 0 ||
                            scan.Eccentricity != 0 ||
                            scan.OrbitalInclination != 0 ||
                            scan.OrbitalPeriod != 0 ||
                            scan.SemiMajorAxis != 0 ||
                            !scan.IsStar ||
                            scan.BodyID > 0 ||
                            scan.CustomBodyName != null ||
                            scan.PgBody.Stars != 0 ||
                            scan.PgBody.Planet != 0;

            return From(eccage, inctemp, mass, radius, periapsis, hasorbit, scan.Age_MY, scan.StarTypeId, scan.IsStar, scan.IsPlanet);
        }

        public static XScanBase From(Models.BodyScan scan)
        {
            float eccage = scan.Eccentricity;
            float inctemp = scan.OrbitalInclination;
            var star = scan as Models.BodyScanStar;
            float mass = scan is Models.BodyScanStar ? star.StellarMass : (scan is Models.BodyScanPlanet planet ? planet.MassEM : 0);
            float radius = scan.Radius;
            float periapsis = scan.Periapsis;
            bool hasorbit = scan.Periapsis != 0 ||
                            scan.Eccentricity != 0 ||
                            scan.OrbitalInclination != 0 ||
                            scan.OrbitalPeriod != 0 ||
                            scan.SemiMajorAxis != 0 ||
                            star == null ||
                            (scan.SystemBody?.BodyID ?? -1) > 0 ||
                            (scan.SystemBody?.CustomNameId ?? 0) > 0 ||
                            (scan.SystemBody?.Stars ?? 0) != 0 ||
                            (scan.SystemBody?.Planet ?? 0) != 0;

            return From(eccage, inctemp, mass, radius, periapsis, hasorbit, star?.Age_MY ?? 0, star?.StarTypeId ?? 0, scan is Models.BodyScanStar, scan is Models.BodyScanPlanet);
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanBase other &&
                   this.Equals(in other);
        }

        public bool Equals(XScanBase other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanBase other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanBase x, in XScanBase y)
        {
            return x.EccentricityAge == y.EccentricityAge &&
                   x.OrbitalInclinationStarType == y.OrbitalInclinationStarType &&
                   x.Periapsis == y.Periapsis &&
                   x.Radius == y.Radius &&
                   x.Mass == y.Mass &&
                   x.Flags == y.Flags;
        }

        public static int GetHashCode(in XScanBase x)
        {
            long hc = (long)x.Flags * 37139213L +
                      (long)(BitConverter.ToUInt32(BitConverter.GetBytes(x.Mass), 0) / 32768) * 2269733L +
                      (long)Math.Floor(x.EccentricityAge * 1024 + 0.5) * 167449L +
                      (long)Math.Floor(x.OrbitalInclinationStarType * 16 + 0.5) * 1787L +
                      (long)Math.Floor(x.Periapsis) * 59L +
                      (long)(BitConverter.ToUInt32(BitConverter.GetBytes(x.Radius), 0) / 32768) * 5L;
            return unchecked((int)hc ^ (int)(hc >> 31));
        }
    }
}
