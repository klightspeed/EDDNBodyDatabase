using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 52 bytes per entry
    public struct XScanPlanet : IEquatable<XScanPlanet>, IWithParentId, IStructEquatable<XScanPlanet>, IWithNextId
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public int AtmosphereId { get; set; }
        public int MaterialsId { get; set; }
        public float MassEM;
        public float SurfaceGravity;
        public float CompositionMetal;
        public float CompositionRock;
        public float CompositionIce;
        public byte PlanetClassId;
        public byte VolcanismId;
        public byte TerraformStateId;
        public bool HasDetailedScan;
        public bool VolcanismMinor;
        public bool VolcanismMajor;
        public bool IsLandable;
        public bool HasLandable;
        public bool HasComposition;
        public bool HasAtmosphere { get; set; }
        public bool HasMaterials { get; set; }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanPlanet other &&
                   Equals(in this, in other);
        }

        public override int GetHashCode()
        {
            return (PlanetClassId * 2269733) +
                   (MassEM.GetHashCode() % 2269733);
        }

        public bool Equals(XScanPlanet other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanPlanet other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanPlanet x, in XScanPlanet y)
        {
            return x.ParentId == y.ParentId &&
                   x.MassEM == y.MassEM &&
                   x.SurfaceGravity == y.SurfaceGravity &&
                   x.CompositionMetal == y.CompositionMetal &&
                   x.CompositionRock == y.CompositionRock &&
                   x.CompositionIce == y.CompositionIce &&
                   x.PlanetClassId == y.PlanetClassId &&
                   x.VolcanismId == y.VolcanismId &&
                   x.TerraformStateId == y.TerraformStateId &&
                   x.HasDetailedScan == y.HasDetailedScan &&
                   x.VolcanismMinor == y.VolcanismMinor &&
                   x.VolcanismMajor == y.VolcanismMajor &&
                   x.IsLandable == y.IsLandable &&
                   x.HasLandable == y.HasLandable &&
                   x.HasComposition == y.HasComposition &&
                   x.HasAtmosphere == y.HasAtmosphere &&
                   x.HasMaterials == y.HasMaterials &&
                   x.AtmosphereId == y.AtmosphereId &&
                   x.MaterialsId == y.MaterialsId;
        }

        public static bool UpdateIfEqual(ref XScanPlanet x, in XScanPlanet y)
        {
            bool match = x.ParentId == y.ParentId &&
                         x.MassEM == y.MassEM &&
                         x.SurfaceGravity == y.SurfaceGravity &&
                         x.PlanetClassId == y.PlanetClassId &&
                         (!x.HasComposition || !y.HasComposition || (
                             x.CompositionMetal == y.CompositionMetal &&
                             x.CompositionRock == y.CompositionRock &&
                             x.CompositionIce == y.CompositionIce
                         )) &&
                         (!x.HasDetailedScan || !y.HasDetailedScan || (
                            x.AtmosphereId == y.AtmosphereId &&
                            x.MaterialsId == y.MaterialsId &&
                            x.VolcanismId == y.VolcanismId &&
                            x.TerraformStateId == y.TerraformStateId &&
                            x.VolcanismMinor == y.VolcanismMinor &&
                            x.VolcanismMajor == y.VolcanismMajor &&
                            x.IsLandable == y.IsLandable
                         ));

            if (match)
            {
                if (!x.HasDetailedScan && y.HasDetailedScan)
                {
                    x.HasAtmosphere = y.HasAtmosphere;
                    x.AtmosphereId = y.AtmosphereId;
                    x.HasMaterials = y.HasMaterials;
                    x.MaterialsId = y.MaterialsId;
                    x.VolcanismId = y.VolcanismId;
                    x.VolcanismMinor = y.VolcanismMinor;
                    x.VolcanismMajor = y.VolcanismMajor;
                    x.HasLandable = y.HasLandable;
                    x.IsLandable = y.IsLandable;
                    x.HasDetailedScan = true;
                }

                if (!x.HasComposition && y.HasComposition)
                {
                    x.CompositionMetal = y.CompositionMetal;
                    x.CompositionRock = y.CompositionRock;
                    x.CompositionIce = y.CompositionIce;
                    x.HasComposition = true;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static XScanPlanet From(in XScanData scan)
        {
            return new XScanPlanet
            {
                MassEM = scan.MassEM,
                SurfaceGravity = scan.SurfaceGravity,
                CompositionMetal = scan.CompositionMetal,
                CompositionRock = scan.CompositionRock,
                CompositionIce = scan.CompositionIce,
                PlanetClassId = scan.PlanetClassId,
                VolcanismId = scan.VolcanismId,
                TerraformStateId = scan.TerraformStateId,
                HasDetailedScan = scan.HasDetailedScan,
                VolcanismMinor = scan.VolcanismMinor,
                VolcanismMajor = scan.VolcanismMajor,
                IsLandable = scan.IsLandable,
                HasLandable = scan.HasLandable,
                HasComposition = scan.HasComposition,
                HasAtmosphere = scan.HasAtmosphere,
                HasMaterials = scan.HasMaterials,
            };
        }
    }
}
