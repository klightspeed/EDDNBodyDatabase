using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Collections;

namespace EDDNBodyDatabase.Models
{
    [DebuggerDisplay("{SystemBody.BodyName} {(Star.StarType ?? Planet.PlanetClass)}")]
    public struct BodyScanStruct
    {
        public class ComparerClass : IEqualityComparer<BodyScanStruct>
        {
            public bool Equals(BodyScanStruct x, BodyScanStruct y)
            {
                return BodyScanBaseStruct.Comparer.Equals(x.BaseData, y.BaseData) &&
                       x.Data.SemiMajorAxis == y.Data.SemiMajorAxis &&
                       x.Data.OrbitalPeriod == y.Data.OrbitalPeriod &&
                       x.Data.RotationPeriod == y.Data.RotationPeriod &&
                       x.Data.SurfaceTemperature == y.Data.SurfaceTemperature &&
                       x.IsStar == y.IsStar &&
                       x.IsPlanet == y.IsPlanet &&
                       x.Star.Equals(y.Star) &&
                       x.Planet.Equals(y.Planet) &&
                       (x.Data.HasAxialTilt == false || y.Data.HasAxialTilt == false || x.Data.AxialTilt == y.Data.AxialTilt) &&
                       (x.Data.HasTidalLock == false || y.Data.HasTidalLock == false || x.Data.TidalLock == y.Data.TidalLock) &&
                       (x.HasParents == false || y.HasParents == false || x.Parents.Equals(y.Parents)) &&
                       (x.HasRings == false || y.HasRings == false || x.Rings.Equals(y.Rings));
            }

            public int GetHashCode(BodyScanStruct obj)
            {
                return BodyScanBaseStruct.Comparer.GetHashCode(obj.BaseData) ^
                       obj.Data.SemiMajorAxis.GetHashCode() ^
                       obj.Data.OrbitalPeriod.GetHashCode() ^
                       obj.Data.RotationPeriod.GetHashCode() ^
                       obj.Data.SurfaceTemperature.GetHashCode() ^
                       obj.IsStar.GetHashCode() ^
                       obj.IsPlanet.GetHashCode() ^
                       obj.Star.GetHashCode() ^
                       obj.Planet.GetHashCode();
            }
        }

        public static readonly ComparerClass Comparer = new ComparerClass();

        public struct DataStruct : IEquatable<DataStruct>, IStructEquatable<DataStruct>
        {
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
            public bool HasOrbit;
            public bool HasAxialTilt;
            public bool TidalLock;
            public bool HasTidalLock;
            public string ReserveLevel { get { return BodyDatabase.ReserveLevel.GetName(ReserveLevelId); } set { ReserveLevelId = BodyDatabase.ReserveLevel.GetId(value) ?? 0; } }

            public override int GetHashCode()
            {
                return ((HasTidalLock ? 0 : 1) |
                        (TidalLock ? 0 : 2) |
                        (HasAxialTilt ? 0 : 4) |
                        (HasOrbit ? 0 : 8)) ^
                        Eccentricity.GetHashCode() ^
                        OrbitalInclination.GetHashCode() ^
                        Periapsis.GetHashCode() ^
                        Radius.GetHashCode() ^
                        AxialTilt.GetHashCode() ^
                        SemiMajorAxis.GetHashCode() ^
                        OrbitalPeriod.GetHashCode() ^
                        RotationPeriod.GetHashCode() ^
                        SurfaceTemperature.GetHashCode();
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
                return x.HasTidalLock == y.HasTidalLock &&
                       x.TidalLock == y.TidalLock &&
                       x.HasAxialTilt == y.HasAxialTilt &&
                       x.HasOrbit == y.HasOrbit &&
                       x.Eccentricity == y.Eccentricity &&
                       x.OrbitalInclination == y.OrbitalInclination &&
                       x.Periapsis == y.Periapsis &&
                       x.Radius == y.Radius &&
                       x.AxialTilt == y.AxialTilt &&
                       x.SemiMajorAxis == y.SemiMajorAxis &&
                       x.OrbitalPeriod == y.OrbitalPeriod &&
                       x.RotationPeriod == y.RotationPeriod &&
                       x.SurfaceTemperature == y.SurfaceTemperature &&
                       x.ReserveLevelId == y.ReserveLevelId;
            }

            public int CompareTo(DataStruct other)
            {
                if (this.HasOrbit != other.HasOrbit)
                {
                    return this.HasOrbit.CompareTo(other.HasOrbit);
                }
                else if (this.OrbitalPeriod != other.OrbitalPeriod)
                {
                    return this.OrbitalPeriod.CompareTo(other.OrbitalPeriod);
                }
                else if (this.Periapsis != other.Periapsis)
                {
                    return this.Periapsis.CompareTo(other.Periapsis);
                }
                else if (this.SemiMajorAxis != other.SemiMajorAxis)
                {
                    return this.SemiMajorAxis.CompareTo(other.SemiMajorAxis);
                }
                else if (this.Radius != other.Radius)
                {
                    return this.Radius.CompareTo(other.Radius);
                }
                else if (this.Eccentricity != other.Eccentricity)
                {
                    return this.Eccentricity.CompareTo(other.Eccentricity);
                }
                else if (this.SurfaceTemperature != other.SurfaceTemperature)
                {
                    return this.SurfaceTemperature.CompareTo(other.SurfaceTemperature);
                }
                else if (this.RotationPeriod != other.RotationPeriod)
                {
                    return this.RotationPeriod.CompareTo(other.RotationPeriod);
                }
                else if (this.HasTidalLock != other.HasTidalLock)
                {
                    return this.HasTidalLock.CompareTo(other.HasTidalLock);
                }
                else if (this.TidalLock != other.TidalLock)
                {
                    return this.TidalLock.CompareTo(other.TidalLock);
                }
                else if (this.HasAxialTilt != other.HasAxialTilt)
                {
                    return this.HasAxialTilt.CompareTo(other.HasAxialTilt);
                }
                else
                {
                    return this.AxialTilt.CompareTo(other.AxialTilt);
                }
            }
        }

        public BodyScanSystemBodyStruct Body;
        public DataStruct Data;
        public bool IsStar;
        public BodyScanStarStruct Star;
        public bool IsPlanet;
        public BodyScanPlanetStruct Planet;
        public bool HasRings;
        public BodyScanRingsStruct Rings;
        public bool HasParents;
        public ParentSetStruct Parents;

        public BodyScanBaseStruct BaseData
        {
            get
            {
                return new BodyScanBaseStruct
                {
                    Data = new BodyScanBaseStruct.DataStruct
                    {
                        Eccentricity = Data.Eccentricity,
                        HasOrbit = Data.HasOrbit,
                        OrbitalInclination = Data.OrbitalInclination,
                        Periapsis = Data.Periapsis,
                        Radius = Data.Radius
                    },
                    SystemBody = Body
                };
            }
        }

        public static BodyScanStruct FromJson(JObject jo)
        {
            if (jo["PlanetClass"] == null && jo["StarType"] == null)
            {
                return new BodyScanStruct
                {
                    Body = BodyScanSystemBodyStruct.FromJSON(jo),
                    Parents = ParentSetStruct.FromJSON(jo["Parents"] as JArray)
                };
            }

            return new BodyScanStruct
            {
                Body = BodyScanSystemBodyStruct.FromJSON(jo),
                Data = new DataStruct
                {
                    Eccentricity = jo.Value<float?>("Eccentricity") ?? 0,
                    OrbitalInclination = jo.Value<float?>("OrbitalInclination") ?? 0,
                    Periapsis = jo.Value<float?>("Periapsis") ?? 0,
                    Radius = jo.Value<float>("Radius"),
                    HasOrbit = jo["SemiMajorAxis"] != null,
                    AxialTilt = jo.Value<float?>("AxialTilt") ?? 0,
                    HasAxialTilt = jo["AxialTilt"] != null,
                    SemiMajorAxis = jo.Value<float?>("SemiMajorAxis") ?? 0,
                    OrbitalPeriod = jo.Value<float?>("OrbitalPeriod") ?? 0,
                    RotationPeriod = jo.Value<float>("RotationPeriod"),
                    SurfaceTemperature = jo.Value<float>("SurfaceTemperature"),
                    TidalLock = jo.Value<bool?>("TidalLock") ?? false,
                    HasTidalLock = jo["TidalLock"] == null,
                    ReserveLevel = jo.Value<string>("ReserveLevel"),
                },
                IsStar = jo["StarType"] != null,
                Star = BodyScanStarStruct.FromJSON(jo),
                IsPlanet = jo["PlanetClass"] != null,
                Planet = BodyScanPlanetStruct.FromJSON(jo),
                HasParents = jo["Parents"] != null,
                Parents = ParentSetStruct.FromJSON(jo["Parents"] as JArray),
                HasRings = jo["Rings"] != null && jo["Rings"].Count() != 0,
                Rings = BodyScanRingsStruct.FromJSON(jo),
            };
        }
    }
}
