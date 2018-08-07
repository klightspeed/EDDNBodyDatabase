using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 72 bytes per entry
    public struct XScan : IEquatable<XScan>, IWithParentId, IStructEquatable<XScan>, IWithNextId
    {
        [Flags]
        public enum ScanFlags : byte
        {
            None = 0x00,
            HasAxialTilt = 0x01,
            HasTidalLock = 0x02,
            HasRings = 0x04,
            HasOrbit = 0x08,
            IsPlanet = 0x10,
            IsStar = 0x20,
            TidalLock = 0x40,
            HasParents = 0x80,
            ClusterMask = HasAxialTilt | HasTidalLock | HasRings | HasOrbit | IsPlanet | IsStar | TidalLock,
            HashMask = HasOrbit | IsPlanet | IsStar,
        }

        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public int DbId { get; set; }
        public int ParentSetId;
        public int PlanetScanId;
        public int StarScanId;
        public int RingScanId;
        public int BodyId;
        public float Eccentricity;
        public float OrbitalInclination;
        public float Periapsis;
        public float Radius;
        public float AxialTilt;
        public float SemiMajorAxis;
        public float OrbitalPeriod;
        public float RotationPeriod;
        public float SurfaceTemperature;
        public byte ReserveLevelId;
        public ScanFlags Flags;

        public bool HasAxialTilt => Flags.HasFlag(ScanFlags.HasAxialTilt);
        public bool HasTidalLock => Flags.HasFlag(ScanFlags.HasTidalLock);
        public bool HasRings => Flags.HasFlag(ScanFlags.HasRings);
        public bool HasOrbit => Flags.HasFlag(ScanFlags.HasOrbit);
        public bool IsPlanet => Flags.HasFlag(ScanFlags.IsPlanet);
        public bool IsStar => Flags.HasFlag(ScanFlags.IsStar);
        public bool TidalLock => Flags.HasFlag(ScanFlags.TidalLock);
        public bool HasParents => Flags.HasFlag(ScanFlags.HasParents);


        public static XScan From(in XScanData body)
        {
            return new XScan
            {
                AxialTilt = body.AxialTilt,
                Eccentricity = body.Eccentricity,
                OrbitalInclination = body.OrbitalInclination,
                Periapsis = body.Periapsis,
                Radius = body.Radius,
                SemiMajorAxis = body.SemiMajorAxis,
                OrbitalPeriod = body.OrbitalPeriod,
                RotationPeriod = body.RotationPeriod,
                SurfaceTemperature = body.SurfaceTemperature,
                ReserveLevelId = body.ReserveLevelId,
                Flags = (body.HasAxialTilt ? ScanFlags.HasAxialTilt : ScanFlags.None) |
                        (body.HasTidalLock ? ScanFlags.HasTidalLock : ScanFlags.None) |
                        (body.HasRings ? ScanFlags.HasRings : ScanFlags.None) |
                        (body.HasOrbit ? ScanFlags.HasOrbit : ScanFlags.None) |
                        (body.IsStar ? ScanFlags.IsStar : ScanFlags.None) |
                        (body.IsPlanet ? ScanFlags.IsPlanet : ScanFlags.None) |
                        (body.TidalLock ? ScanFlags.TidalLock : ScanFlags.None) |
                        (body.HasParents ? ScanFlags.HasParents : ScanFlags.None),
            };
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScan other &&
                   this.Equals(in other);
        }

        public bool Equals(XScan other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScan other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScan x, in XScan y)
        {
            return x.ParentId == y.ParentId &&
                   x.BodyId == y.BodyId &&
                   x.ParentSetId == y.ParentSetId &&
                   x.Flags == y.Flags &&
                   x.AxialTilt == y.AxialTilt &&
                   x.SemiMajorAxis == y.SemiMajorAxis &&
                   x.OrbitalPeriod == y.OrbitalPeriod &&
                   x.RotationPeriod == y.RotationPeriod &&
                   x.SurfaceTemperature == y.SurfaceTemperature &&
                   x.ReserveLevelId == y.ReserveLevelId &&
                   x.PlanetScanId == y.PlanetScanId &&
                   x.StarScanId == y.StarScanId &&
                   x.RingScanId == y.RingScanId;
        }

        public static int GetHashCode(in XScan x)
        {
            int hc = (int)(x.Flags & ScanFlags.HashMask) ^
                     x.Eccentricity.GetHashCode() ^
                     x.OrbitalInclination.GetHashCode() ^
                     x.Periapsis.GetHashCode() ^
                     x.Radius.GetHashCode() ^
                     x.SemiMajorAxis.GetHashCode() ^
                     x.OrbitalPeriod.GetHashCode() ^
                     x.RotationPeriod.GetHashCode() ^
                     x.SurfaceTemperature.GetHashCode();

            return (hc % 19) + ((x.Flags & ScanFlags.ClusterMask) == ScanFlags.None ? -x.BodyId : x.ParentId) * 19;
        }

        public static bool UpdateIfEqual(ref XScan x, in XScan y)
        {
            bool match =
                   x.ParentId == y.ParentId &&
                   x.BodyId == y.BodyId &&
                   (x.ParentSetId == 0 || y.ParentSetId == 0 || x.ParentSetId == y.ParentSetId) &&
                   (!x.Flags.HasFlag(ScanFlags.HasTidalLock) || !y.Flags.HasFlag(ScanFlags.HasTidalLock) || x.Flags.HasFlag(ScanFlags.TidalLock) == y.Flags.HasFlag(ScanFlags.TidalLock)) &&
                   (!x.Flags.HasFlag(ScanFlags.HasAxialTilt) || !y.Flags.HasFlag(ScanFlags.HasAxialTilt) || x.AxialTilt == y.AxialTilt) &&
                   x.SemiMajorAxis == y.SemiMajorAxis &&
                   x.OrbitalPeriod == y.OrbitalPeriod &&
                   x.RotationPeriod == y.RotationPeriod &&
                   x.SurfaceTemperature == y.SurfaceTemperature &&
                   (!x.Flags.HasFlag(ScanFlags.HasRings) || !y.Flags.HasFlag(ScanFlags.HasRings) || (
                       x.ReserveLevelId == y.ReserveLevelId &&
                       x.RingScanId == y.RingScanId
                   )) &&
                   x.PlanetScanId == y.PlanetScanId &&
                   x.StarScanId == y.StarScanId;

            if (match)
            {
                if (!x.Flags.HasFlag(ScanFlags.HasTidalLock) && y.Flags.HasFlag(ScanFlags.HasTidalLock))
                {
                    x.Flags = (x.Flags & ~ScanFlags.TidalLock) | ScanFlags.HasTidalLock | (y.Flags & ScanFlags.TidalLock);
                }

                if (!x.Flags.HasFlag(ScanFlags.HasAxialTilt) && y.Flags.HasFlag(ScanFlags.HasAxialTilt))
                {
                    x.AxialTilt = y.AxialTilt;
                    x.Flags |= ScanFlags.HasAxialTilt;
                }

                if (!x.Flags.HasFlag(ScanFlags.HasRings) && y.Flags.HasFlag(ScanFlags.HasRings))
                {
                    x.ReserveLevelId = y.ReserveLevelId;
                    x.RingScanId = y.RingScanId;
                    x.Flags |= ScanFlags.HasRings;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
