using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanPlanetStruct : IEquatable<BodyScanPlanetStruct>, IStructEquatable<BodyScanPlanetStruct>
    {
        public struct DataStruct : IEquatable<DataStruct>, IStructEquatable<DataStruct>
        {
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
            public string PlanetClass { get { return BodyDatabase.PlanetClass.GetName(PlanetClassId); } set { PlanetClassId = BodyDatabase.PlanetClass.GetId(value) ?? 0; } }
            public string Volcanism { get { return BodyDatabase.Volcanism.GetName(VolcanismId); } set { VolcanismId = BodyDatabase.Volcanism.GetId(value) ?? 0; } }
            public string TerraformState { get { return BodyDatabase.TerraformState.GetName(TerraformStateId); } set { TerraformStateId = BodyDatabase.TerraformState.GetId(value) ?? 0; } }

            public override int GetHashCode()
            {
                return (PlanetClassId * 2269733) +
                       (MassEM.GetHashCode() % 2269733);
            }

            public override bool Equals(object obj)
            {
                return obj != null &&
                       obj is DataStruct other &&
                       this.Equals(other);
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
                return x.MassEM == y.MassEM &&
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
                       x.HasComposition == y.HasComposition;
            }
        }

        public DataStruct Data;
        public bool HasMaterials;
        public BodyScanMaterialsStruct Materials;
        public bool HasAtmosphere;
        public BodyScanAtmosphereStruct Atmosphere;

        public bool Equals(BodyScanPlanetStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanPlanetStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanPlanetStruct x, in BodyScanPlanetStruct y)
        {
            return DataStruct.Equals(in x.Data, in y.Data) &&
                   x.HasAtmosphere == y.HasAtmosphere &&
                   BodyScanAtmosphereStruct.Equals(in x.Atmosphere, in y.Atmosphere) &&
                   x.HasMaterials == y.HasMaterials &&
                   BodyScanMaterialsStruct.Equals(in x.Materials, in y.Materials);
        }

        public static BodyScanPlanetStruct FromJSON(JObject jo)
        {
            JObject components = jo["Components"] as JObject;
            string volcanism = jo.Value<string>("Volcanism");
            bool volcanismMinor = false;
            bool volcanismMajor = false;

            if (volcanism != null)
            {
                if (volcanism.StartsWith("minor "))
                {
                    volcanismMinor = true;
                    volcanism = volcanism.Substring(6);
                }
                if (volcanism.StartsWith("major "))
                {
                    volcanismMajor = true;
                    volcanism = volcanism.Substring(6);
                }
            }

            BodyScanMaterialsStruct mats = BodyScanMaterialsStruct.FromJSON(jo["Materials"]);

            return new BodyScanPlanetStruct
            {
                Data = new DataStruct
                {
                    PlanetClass = BodyDatabase.PlanetClass.Intern(jo.Value<string>("PlanetClass")),
                    MassEM = jo.Value<float?>("MassEM") ?? 0,
                    SurfaceGravity = jo.Value<float?>("SurfaceGravity") ?? 0,
                    HasDetailedScan = jo["Landable"] != null,
                    IsLandable = jo.Value<bool?>("Landable") ?? false,
                    TerraformState = BodyDatabase.TerraformState.Intern(jo.Value<string>("TerraformState")),
                    Volcanism = BodyDatabase.Volcanism.Intern(volcanism),
                    VolcanismMinor = volcanismMinor,
                    VolcanismMajor = volcanismMajor,
                    HasComposition = components != null,
                    CompositionMetal = components?.Value<float>("Metal") ?? 0,
                    CompositionRock = components?.Value<float>("Rock") ?? 0,
                    CompositionIce = components?.Value<float>("Ice") ?? 0,
                },
                HasAtmosphere = (jo.Value<float?>("SurfacePressure") ?? 0) != 0 || !String.IsNullOrEmpty(jo.Value<string>("Atmosphere")) || jo["AtmosphereComposition"] != null,
                Atmosphere = BodyScanAtmosphereStruct.FromJSON(jo),
                HasMaterials = mats.IsValid,
                Materials = mats,
            };
        }
    }
}
