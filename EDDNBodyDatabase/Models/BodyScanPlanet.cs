using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanPlanet : BodyScan
    {
        public byte PlanetClassId { get; set; }
        public string PlanetClass { get { return BodyDatabase.PlanetClass.GetName(PlanetClassId); } set { PlanetClassId = (byte)BodyDatabase.PlanetClass.GetId(value); } }
        public float CompositionMetal { get; set; }
        public float CompositionRock { get; set; }
        public float CompositionIce { get; set; }
        public float MassEM { get; set; }
        public float SurfaceGravity { get; set; }
        public byte? VolcanismId { get; set; }
        public string Volcanism { get { return BodyDatabase.Volcanism.GetName(VolcanismId); } set { VolcanismId = (byte?)BodyDatabase.Volcanism.GetId(value); } }
        public bool VolcanismMinor { get; set; }
        public bool VolcanismMajor { get; set; }
        public bool? IsLandable { get; set; }
        public bool HasComposition { get; set; }
        public byte? TerraformStateId { get; set; }
        public string TerraformState { get { return BodyDatabase.TerraformState.GetName(TerraformStateId); } set { TerraformStateId = BodyDatabase.TerraformState.GetId(value); } }

        public BodyScanMaterials Materials { get; set; }
        public BodyScanAtmosphere Atmosphere { get; set; }
        public virtual Volcanism VolcanismRef { get; set; }
        public virtual PlanetClass PlanetClassRef { get; set; }
        public virtual TerraformState TerraformStateRef { get; set; }

        public override bool Equals(XModels.XScanClass scan, bool ignoremats = false)
        {
            XModels.XScanPlanet pscan = scan.Planet;
            return base.Equals(scan) &&
                   this.PlanetClassId == pscan.PlanetClassId &&
                   this.MassEM == pscan.MassEM &&
                   this.SurfaceGravity == pscan.SurfaceGravity &&
                   (!this.HasComposition || !pscan.HasComposition || (
                       this.CompositionMetal == pscan.CompositionMetal &&
                       this.CompositionRock == pscan.CompositionRock &&
                       this.CompositionIce == pscan.CompositionIce
                   )) &&
                   (this.VolcanismId == null || pscan.VolcanismId == 0 || (
                       this.VolcanismId == pscan.VolcanismId &&
                       this.VolcanismMajor == pscan.VolcanismMajor &&
                       this.VolcanismMinor == pscan.VolcanismMinor
                   )) &&
                   (this.IsLandable == null || !pscan.HasLandable || this.IsLandable == pscan.IsLandable) &&
                   (this.Atmosphere == null || !pscan.HasAtmosphere || this.Atmosphere.Equals(scan.Atmosphere)) &&
                   (ignoremats || this.Materials == null || !pscan.HasMaterials || this.Materials.Equals(scan.Materials));
        }

        public override List<CompareResult> GetDifferingProps(XScanClass scandata, List<CompareResult> diffs)
        {
            XModels.XScanPlanet pscan = scandata.Planet;
            base.GetDifferingProps(scandata, diffs);
            CompareResult.AddIfUnequal(diffs, this, "PlanetClassId", this.PlanetClassId, pscan.PlanetClassId);
            CompareResult.AddIfUnequal(diffs, this, "MassEM", this.MassEM, pscan.MassEM);
            CompareResult.AddIfUnequal(diffs, this, "SurfaceGravity", this.SurfaceGravity, pscan.SurfaceGravity);
            if (this.HasComposition && pscan.HasComposition)
            {
                CompareResult.AddIfUnequal(diffs, this, "CompositionMetal", this.CompositionMetal, pscan.CompositionMetal);
                CompareResult.AddIfUnequal(diffs, this, "CompositionRock", this.CompositionRock, pscan.CompositionRock);
                CompareResult.AddIfUnequal(diffs, this, "CompositionIce", this.CompositionIce, pscan.CompositionIce);
            }
            if (this.VolcanismId != null && pscan.VolcanismId != 0)
            {
                CompareResult.AddIfUnequal(diffs, this, "VolcanismId", this.VolcanismId, pscan.VolcanismId);
                CompareResult.AddIfUnequal(diffs, this, "VolcanismMajor", this.VolcanismMajor, pscan.VolcanismMajor);
                CompareResult.AddIfUnequal(diffs, this, "VolcanismMinor", this.VolcanismMinor, pscan.VolcanismMinor);
            }
            CompareResult.AddIfUnequal(diffs, this, "IsLandable", this.IsLandable, pscan.HasLandable ? (bool?)pscan.IsLandable : null);
            if (this.Atmosphere != null && pscan.HasAtmosphere) this.Atmosphere.GetDifferingProps(scandata.Atmosphere, diffs, this);
            if (this.Materials != null && pscan.HasMaterials) this.Materials.GetDifferingProps(scandata.Materials, diffs, this);

            return diffs;
        }

        public override bool Equals(XModels.XScanClass scan, float epsilon)
        {
            XModels.XScanPlanet pscan = scan.Planet;
            return base.Equals(scan) &&
                   this.PlanetClassId == pscan.PlanetClassId &&
                   this.MassEM == pscan.MassEM &&
                   this.SurfaceGravity == pscan.SurfaceGravity &&
                   (!this.HasComposition || !pscan.HasComposition || (
                       this.CompositionMetal == pscan.CompositionMetal &&
                       this.CompositionRock == pscan.CompositionRock &&
                       this.CompositionIce == pscan.CompositionIce
                   )) &&
                   (this.VolcanismId == null || pscan.VolcanismId == 0 || (
                       this.VolcanismId == pscan.VolcanismId &&
                       this.VolcanismMajor == pscan.VolcanismMajor &&
                       this.VolcanismMinor == pscan.VolcanismMinor
                   )) &&
                   (this.IsLandable == null || !pscan.HasLandable || this.IsLandable == pscan.IsLandable) &&
                   (this.Atmosphere == null || !pscan.HasAtmosphere || this.Atmosphere.Equals(scan.Atmosphere)) &&
                   (this.Materials == null || !pscan.HasMaterials || this.Materials.Equals(scan.Materials, epsilon));
        }

        public BodyScanPlanet()
        {
        }

        public BodyScanPlanet(XScanClass scan, SystemBody sysbody, int sysbodyid, int id = 0) : base(scan, sysbody, sysbodyid, id)
        {
            XScanPlanet pscan = scan.Planet;
            PlanetClassId = pscan.PlanetClassId;
            MassEM = pscan.MassEM;
            SurfaceGravity = pscan.SurfaceGravity;
            VolcanismId = pscan.VolcanismId == 0 ? (byte?)null : pscan.VolcanismId;
            VolcanismMinor = pscan.VolcanismMinor;
            VolcanismMajor = pscan.VolcanismMajor;
            IsLandable = pscan.HasLandable ? pscan.IsLandable : (bool?)null;
            HasComposition = pscan.HasComposition;
            CompositionMetal = pscan.CompositionMetal;
            CompositionRock = pscan.CompositionRock;
            CompositionIce = pscan.CompositionIce;

            if (pscan.HasAtmosphere)
            {
                Atmosphere = new BodyScanAtmosphere(scan.Atmosphere, id);
            }

            if (pscan.HasMaterials)
            {
                Materials = new BodyScanMaterials(scan.Materials, id);
            }
        }
    }
}
