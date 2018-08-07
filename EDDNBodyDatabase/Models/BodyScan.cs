using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScan
    {
        public int Id { get; set; }
        public int SystemBodyId { get; set; }
        public int? ParentSetId { get; set; }
        public int ScanBaseHash { get; set; }
        public float Eccentricity { get; set; }
        public float OrbitalInclination { get; set; }
        public float Periapsis { get; set; }
        public bool HasOrbit { get; set; }
        public float? AxialTilt { get; set; }
        public float Radius { get; set; }
        public float SemiMajorAxis { get; set; }
        public float OrbitalPeriod { get; set; }
        public float RotationPeriod { get; set; }
        public float SurfaceTemperature { get; set; }
        public bool? TidalLock { get; set; }
        public byte? ReserveLevelId { get; set; }

        public string ReserveLevel { get { return BodyDatabase.ReserveLevel.GetName(ReserveLevelId); } set { ReserveLevelId = BodyDatabase.ReserveLevel.GetId(value); } }

        public ParentSet Parents { get; set; }
        public List<BodyScanRing> Rings { get; set; }
        public virtual SystemBody SystemBody { get; set; }
        public virtual ReserveLevel ReserveLevelRef { get; set; }

        public virtual bool Equals(XModels.XScanClass scandata, bool ignoremats = false)
        {
            var scan = scandata.Scan;
            var rings = this.Rings?.OrderBy(r => r.RingNum).ToArray();
            var basehash = scandata.ScanBase.GetHashCode();
            return this.ScanBaseHash == basehash &&
                   (scandata.DbBody == null || this.SystemBodyId == scandata.DbBody.Id) &&
                   this.Eccentricity == scan.Eccentricity &&
                   this.OrbitalInclination == scan.OrbitalInclination &&
                   this.Periapsis == scan.Periapsis &&
                   this.Radius == scan.Radius &&
                   this.HasOrbit == scan.HasOrbit &&
                   this.SemiMajorAxis == scan.SemiMajorAxis &&
                   this.OrbitalPeriod == scan.OrbitalPeriod &&
                   this.RotationPeriod == scan.RotationPeriod &&
                   this.SurfaceTemperature == scan.SurfaceTemperature &&
                   (this.ParentSetId == null || scandata.DbParents == null || this.ParentSetId == scandata.DbParents.Id) &&
                   (!scan.HasAxialTilt || this.AxialTilt == null || this.AxialTilt == scan.AxialTilt) &&
                   (!scan.HasTidalLock || this.TidalLock == null || this.TidalLock == scan.TidalLock) &&
                   (this is BodyScanPlanet == scan.IsPlanet) &&
                   (this is BodyScanStar == scan.IsStar) &&
                   (!scan.HasRings || rings == null || rings.Length == 0 || (
                       ((rings.Length >= 1 && rings[0].Equals(scandata.RingA) && rings[0].Name == scandata.RingAName) || (rings.Length < 1 && scandata.RingA.ClassId == 0)) &&
                       ((rings.Length >= 2 && rings[1].Equals(scandata.RingB) && rings[1].Name == scandata.RingBName) || (rings.Length < 2 && scandata.RingB.ClassId == 0)) &&
                       ((rings.Length >= 3 && rings[2].Equals(scandata.RingC) && rings[2].Name == scandata.RingCName) || (rings.Length < 3 && scandata.RingC.ClassId == 0)) &&
                       ((rings.Length >= 4 && rings[3].Equals(scandata.RingD) && rings[3].Name == scandata.RingDName) || (rings.Length < 4 && scandata.RingD.ClassId == 0))
                   ));
        }

        public class CompareResult
        {
            private static float TweakMaxDelta(string name)
            {
                if (name == "AbsoluteMagnitude")
                {
                    return 1.0F;
                }
                else if (name == "SurfaceTemperature")
                {
                    return 1000000F;
                }
                else if (name == "Periapsis")
                {
                    return 0.00005F;
                }
                else if (name == "OrbitalInclination")
                {
                    return 0.00005F;
                }
                else
                {
                    return 0.0F;
                }
            }

            private static float TweakMaxMult(string name)
            {
                if (name == "OrbitalPeriod")
                {
                    return 1.25F;
                }
                else if (name == "SemiMajorAxis")
                {
                    return 0.5F;
                }
                else if (name == "Rings[0].MassMT" || name == "Rings[1].MassMT")
                {
                    return 0.00005F;
                }
                else if (name.StartsWith("Rings[") && name.EndsWith("erRad"))
                {
                    return 0.05F;
                }
                else if (name == "RotationPeriod")
                {
                    return 0.5F;
                }
                else if (name == "SurfacePressure")
                {
                    return 0.00005F;
                }
                else if (name == "SurfaceGravity")
                {
                    return 0.00005F;
                }
                else
                {
                    return 0.0F;
                }
            }

            private static float MinorMaxDelta(string name)
            {
                if (name.StartsWith("Material"))
                {
                    return 0.05F;
                }

                return 0.0000015F;
            }

            private static bool TweakedProperty(string name)
            {
                if (name == "LuminosityId")
                {
                    return true;
                }

                return false;
            }

            public BodyScan Scan { get; set; }
            public string Property { get; set; }
            public object Left { get; set; }
            public object Right { get; set; }
            public int LSB { get { return Left is float left && Right is float right ? Math.Abs(BitConverter.ToInt32(BitConverter.GetBytes(left), 0) - BitConverter.ToInt32(BitConverter.GetBytes(right), 0)) : -1; } }
            public float Delta { get { return Left is float left && Right is float right ? Math.Abs(left - right) : -1; } }
            public float DeltaMult { get { return Left is float left && Right is float right && left * right >= 1e-10 ? Math.Abs(left - right) / (float)Math.Sqrt(Math.Abs(left * right)) : -1; } }

            public bool IsTweakedProperty
            {
                get
                {
                    if (Left is float left && Right is float right)
                    {
                        var diff = Math.Abs(left - right);
                        var div = Math.Min(Math.Abs(left), Math.Abs(right));
                        var tdelta = TweakMaxDelta(Property);
                        var tmult = TweakMaxMult(Property) * div;
                        var star = Scan as BodyScanStar;

                        if (diff < tdelta || diff < tmult)
                        {
                            switch (Property)
                            {
                                case "AbsoluteMagnitude":
                                    return star?.StarTypeId == BodyDatabase.StarType.GetId("N");
                                case "Periapsis":
                                case "OrbitalInclination":
                                case "OrbitalPeriod":
                                case "SemiMajorAxis":
                                case "SurfacePressure":
                                case "SurfaceGravity":
                                case "RotationPeriod":
                                case "Rings[0].InnerRad":
                                case "Rings[1].InnerRad":
                                case "Rings[2].InnerRad":
                                case "Rings[3].InnerRad":
                                case "Rings[0].OuterRad":
                                case "Rings[1].OuterRad":
                                case "Rings[2].OuterRad":
                                case "Rings[3].OuterRad":
                                    return true;
                                case "Rings[0].MassMT":
                                case "Rings[1].MassMT":
                                case "Rings[2].MassMT":
                                case "Rings[3].MassMT":
                                    return star != null;
                                case "SurfaceTemperature":
                                    return left == 0 || right == 0;
                            }
                        }
                    }
                    else if (Left is byte bleft && Right is byte bright)
                    {
                        switch (Property)
                        {
                            case "LuminosityId":
                                return bleft == 0 || bright == 0;
                        }
                    }

                    return false;
                }
            }

            public bool IsMinorDifference
            {
                get
                {
                    if (Left is float left && Right is float right)
                    {
                        var diff = Math.Abs(left - right);
                        var bitleft = BitConverter.ToInt32(BitConverter.GetBytes(left), 0);
                        var bitright = BitConverter.ToInt32(BitConverter.GetBytes(right), 0);
                        var bitdiff = Math.Abs(bitleft - bitright);
                        var maxdelta = MinorMaxDelta(Property);
                        var maxbits = 8;

                        if (diff < maxdelta || bitdiff <= maxbits)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            public static CompareResult Create<T>(string propname, T left, T right, BodyScan scan)
            {
                return new CompareResult { Property = propname, Left = left, Right = right, Scan = scan };
            }

            public static void AddIfUnequal<T>(List<CompareResult> diffs, BodyScan scan, string name, bool isnull, T left, T right)
                where T : IEquatable<T>
            {
                if (!isnull && ((left == null) != (right == null) || (left != null && !left.Equals(right)))) diffs.Add(Create(name, left, right, scan));
            }

            public static void AddIfUnequal<T>(List<CompareResult> diffs, BodyScan scan, string name, T left, T right)
                where T : IEquatable<T>
            {
                if (left != null && right != null && !left.Equals(right)) diffs.Add(Create(name, left, right, scan));
            }

            public static void AddIfUnequal<T>(List<CompareResult> diffs, BodyScan scan, string name, bool isnull, T? left, T? right)
                where T : struct, IEquatable<T>
            {
                if (!isnull && (left.HasValue != right.HasValue || (left.HasValue && !left.Value.Equals(right.Value)))) diffs.Add(Create(name, left, right, scan));
            }

            public static void AddIfUnequal<T>(List<CompareResult> diffs, BodyScan scan, string name, T? left, T? right)
                where T : struct, IEquatable<T>
            {
                if (left != null && right != null && !left.Value.Equals(right.Value)) diffs.Add(Create(name, left, right, scan));
            }
        }

        public virtual List<CompareResult> GetDifferingProps(XModels.XScanClass scandata, List<CompareResult> diffs)
        {
            var scan = scandata.Scan;
            var rings = this.Rings?.OrderBy(r => r.RingNum).ToArray();
            var basehash = scandata.ScanBase.GetHashCode();

            CompareResult.AddIfUnequal(diffs, this, "ScanBaseHash", this.ScanBaseHash, basehash);
            CompareResult.AddIfUnequal(diffs, this, "SystemBodyId", this.SystemBodyId, scandata.DbBody?.Id);
            CompareResult.AddIfUnequal(diffs, this, "Eccentricity", this.Eccentricity, scan.Eccentricity);
            CompareResult.AddIfUnequal(diffs, this, "OrbitalInclination", this.OrbitalInclination, scan.OrbitalInclination);
            CompareResult.AddIfUnequal(diffs, this, "Periapsis", this.Periapsis, scan.Periapsis);
            CompareResult.AddIfUnequal(diffs, this, "Radius", this.Radius, scan.Radius);
            CompareResult.AddIfUnequal(diffs, this, "HasOrbit", this.HasOrbit, scan.HasOrbit);
            CompareResult.AddIfUnequal(diffs, this, "SemiMajorAxis", this.SemiMajorAxis, scan.SemiMajorAxis);
            CompareResult.AddIfUnequal(diffs, this, "OrbitalPeriod", this.OrbitalPeriod, scan.OrbitalPeriod);
            CompareResult.AddIfUnequal(diffs, this, "RotationPeriod", this.RotationPeriod, scan.RotationPeriod);
            CompareResult.AddIfUnequal(diffs, this, "SurfaceTemperature", this.SurfaceTemperature, scan.SurfaceTemperature);
            CompareResult.AddIfUnequal(diffs, this, "ParentSet", this.ParentSetId, scandata.DbParents?.Id);
            CompareResult.AddIfUnequal(diffs, this, "AxialTilt", this.AxialTilt, scan.HasAxialTilt ? (float?)scan.AxialTilt : null);
            CompareResult.AddIfUnequal(diffs, this, "TidalLock", this.TidalLock, scan.HasTidalLock ? (bool?)scan.TidalLock : null);
            CompareResult.AddIfUnequal(diffs, this, "IsPlanet", this is BodyScanPlanet, scan.IsPlanet);
            CompareResult.AddIfUnequal(diffs, this, "IsStar", this is BodyScanStar, scan.IsStar);
            if (rings != null)
            {
                if (rings.Length >= 1 && scandata.RingA.ClassId != 0) rings[0].GetDifferingProps(scandata.RingA, diffs, this);
                if (rings.Length >= 2 && scandata.RingB.ClassId != 0) rings[1].GetDifferingProps(scandata.RingB, diffs, this);
                if (rings.Length >= 3 && scandata.RingC.ClassId != 0) rings[2].GetDifferingProps(scandata.RingC, diffs, this);
                if (rings.Length >= 4 && scandata.RingD.ClassId != 0) rings[3].GetDifferingProps(scandata.RingD, diffs, this);
            }

            return diffs;
        }

        public virtual bool Equals(XModels.XScanClass scandata, float epsilon)
        {
            var scan = scandata.Scan;
            var rings = this.Rings?.OrderBy(r => r.RingNum).ToArray();
            var basehash = scandata.ScanBase.GetHashCode();
            return this.ScanBaseHash == basehash &&
                   (scandata.DbBody == null || this.SystemBodyId == scandata.DbBody.Id) &&
                   this.Eccentricity == scan.Eccentricity &&
                   this.OrbitalInclination == scan.OrbitalInclination &&
                   this.Periapsis == scan.Periapsis &&
                   this.Radius == scan.Radius &&
                   this.HasOrbit == scan.HasOrbit &&
                   this.SemiMajorAxis == scan.SemiMajorAxis &&
                   this.OrbitalPeriod == scan.OrbitalPeriod &&
                   this.RotationPeriod == scan.RotationPeriod &&
                   this.SurfaceTemperature == scan.SurfaceTemperature &&
                   (this.ParentSetId == null || scandata.DbParents == null || this.ParentSetId == scandata.DbParents.Id) &&
                   (!scan.HasAxialTilt || this.AxialTilt == null || this.AxialTilt == scan.AxialTilt) &&
                   (!scan.HasTidalLock || this.TidalLock == null || this.TidalLock == scan.TidalLock) &&
                   (this is BodyScanPlanet == scan.IsPlanet) &&
                   (this is BodyScanStar == scan.IsStar) &&
                   (!scan.HasRings || rings == null || rings.Length == 0 || (
                       ((rings.Length >= 1 && rings[0].Equals(scandata.RingA) && rings[0].Name == scandata.RingAName) || (rings.Length < 1 && scandata.RingA.ClassId == 0)) &&
                       ((rings.Length >= 2 && rings[1].Equals(scandata.RingB) && rings[1].Name == scandata.RingBName) || (rings.Length < 2 && scandata.RingB.ClassId == 0)) &&
                       ((rings.Length >= 3 && rings[2].Equals(scandata.RingC) && rings[2].Name == scandata.RingCName) || (rings.Length < 3 && scandata.RingC.ClassId == 0)) &&
                       ((rings.Length >= 4 && rings[3].Equals(scandata.RingD) && rings[3].Name == scandata.RingDName) || (rings.Length < 4 && scandata.RingD.ClassId == 0))
                   ));
        }

        public BodyScan()
        {
        }

        public BodyScan(XModels.XScanClass scan, SystemBody sysbody, int sysbodyid, int id = 0)
        {
            Id = id;
            SystemBodyId = sysbodyid;
            SystemBody = sysbody;
            ScanBaseHash = scan.ScanBase.GetHashCode();
            Eccentricity = scan.Scan.Eccentricity;
            OrbitalInclination = scan.Scan.OrbitalInclination;
            Periapsis = scan.Scan.Periapsis;
            HasOrbit = scan.Scan.Flags.HasFlag(XScan.ScanFlags.HasOrbit);
            AxialTilt = scan.Scan.AxialTilt;
            SemiMajorAxis = scan.Scan.SemiMajorAxis;
            OrbitalPeriod = scan.Scan.OrbitalPeriod;
            Radius = scan.Scan.Radius;
            RotationPeriod = scan.Scan.RotationPeriod;
            SurfaceTemperature = scan.Scan.SurfaceTemperature;
            TidalLock = scan.Scan.Flags.HasFlag(XScan.ScanFlags.HasTidalLock) ? scan.Scan.Flags.HasFlag(XScan.ScanFlags.TidalLock) : (bool?)null;
            Rings = scan.RingA.ClassId == 0 ? null : BodyScanRing.GetRings(scan);
            ParentSetId = scan.Parents.Parent0TypeId == 0 ? (int?)null : (scan.DbParents?.Id ?? BodyDatabase.GetOrAddParentSet(scan.Parents).Id);
            ReserveLevelId = scan.Scan.ReserveLevelId == 0 ? (byte?)null : scan.Scan.ReserveLevelId;
        }

        public static BodyScan FromStruct(XScanClass scan, SystemBody sysbody, int systembodyid, int id = 0)
        {
            if (scan.Scan.Flags.HasFlag(XScan.ScanFlags.IsPlanet))
            {
                return new BodyScanPlanet(scan, sysbody, systembodyid, id);
            }
            else if (scan.Scan.Flags.HasFlag(XScan.ScanFlags.IsStar))
            {
                return new BodyScanStar(scan, sysbody, systembodyid, id);
            }
            else
            {
                return new BodyScan(scan, sysbody, systembodyid, id);
            }
        }
    }
}
