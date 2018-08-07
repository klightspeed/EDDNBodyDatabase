using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public class XScanData
    {
        [Flags]
        public enum CommonBodyFlags : short
        {
            HasParents = 0x01,
            HasBodyId = 0x02,
            HasSystemAddress = 0x04,
            HasScanTypeId = 0x08,
            HasRings = 0x10,
            HasReserveLevel = 0x20,
            HasDetailedScan = 0x40,
            IsBasicScan = 0x80,
            IsPos3SigFig = 0x100,
            IsPlanet = 0x200,
            IsStar = 0x400,
        }

        [Flags]
        public enum OrbitScanFlags : byte
        {
            HasEccentricity = 0x01,
            HasInclination = 0x02,
            HasPeriapsis = 0x04,
            HasSemiMajorAxis = 0x08,
            HasOrbitalPeriod = 0x10,
            HasDistanceFromArrivalLS = 0x20,
            HasTidalLock = 0x40,
            TidalLock = 0x80,
        }

        [Flags]
        public enum CommonScanFlags : byte
        {
            HasRadius = 0x01,
            HasAxialTilt = 0x02,
            HasRotationPeriod = 0x04,
            HasSurfaceTemperature = 0x08,
        }

        [Flags]
        public enum StarScanFlags : byte
        {
            HasAbsoluteMagnitude = 0x01,
            HasStellarMass = 0x02,
            HasLuminosity = 0x04,
            HasStarType = 0x08,
            HasAge = 0x10,
        }

        [Flags]
        public enum PlanetScanFlags : short
        {
            HasMass = 0x01,
            HasSurfaceGravity = 0x02,
            HasComposition = 0x04,
            HasPlanetClass = 0x08,
            HasVolcanism = 0x10,
            HasTerraformState = 0x20,
            HasLandable = 0x40,
            HasMaterials = 0x80,
            VolcanismMinor = 0x100,
            VolcanismMajor = 0x200,
            IsLandable = 0x400,
            IsMaterialsDict = 0x800,
            HasAtmosphere = 0x1000,
        }

        [Flags]
        public enum AtmosphereScanFlags : byte
        {
            HasSurfacePressure = 0x01,
            HasAtmosphereComposition = 0x02,
            HasAtmosphere = 0x04,
            HasAtmosphereType = 0x08,
            AtmosphereHot = 0x10,
            AtmosphereThin = 0x20,
            AtmosphereThick = 0x40,
        }

        public SystemStruct PgSystemName { get; private set; }
        public SystemBodyStruct PgBody { get; private set; }
        #region Parents
        public XParentEntry Parent0 { get; private set; }
        public XParentEntry Parent1 { get; private set; }
        public XParentEntry Parent2 { get; private set; }
        public XParentEntry Parent3 { get; private set; }
        public XParentEntry Parent4 { get; private set; }
        public XParentEntry Parent5 { get; private set; }
        public XParentEntry Parent6 { get; private set; }
        public XParentEntry Parent7 { get; private set; }
        #endregion
        #region Rings
        public XBodyScanRingStruct RingA { get; private set; }
        public XBodyScanRingStruct RingB { get; private set; }
        public XBodyScanRingStruct RingC { get; private set; }
        public XBodyScanRingStruct RingD { get; private set; }
        #endregion
        #region Common
        public string JsonSchema { get; private set; }
        public Dictionary<string, object> JsonHeader { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> JsonMessage { get; } = new Dictionary<string, object>();
        public string CustomSystemName { get; private set; }
        public string CustomBodyName { get; private set; }
        public string JsonExtra { get; private set; }
        public DateTime GatewayTimestamp { get; private set; }
        public DateTime ScanTimestamp { get; private set; }
        public long SystemAddress { get; private set; }
        public float StarPosX { get; private set; }
        public float StarPosY { get; private set; }
        public float StarPosZ { get; private set; }
        #endregion
        #region Orbit
        public float Eccentricity { get; private set; }
        public float OrbitalInclination { get; private set; }
        public float Periapsis { get; private set; }
        public float SemiMajorAxis { get; private set; }
        public float OrbitalPeriod { get; private set; }
        public float DistanceFromArrivalLS { get; private set; }
        #endregion
        #region Scan Common
        public float Radius { get; private set; }
        public float AxialTilt { get; private set; }
        public float RotationPeriod { get; private set; }
        public float SurfaceTemperature { get; private set; }
        #endregion
        #region Atmosphere float
        public float SurfacePressure { get; private set; }
        public float AtmosphereComponent1Amt { get; private set; }
        public float AtmosphereComponent2Amt { get; private set; }
        public float AtmosphereComponent3Amt { get; private set; }
        #endregion
        #region Materials float
        public float MaterialCarbon { get; private set; }
        public float MaterialIron { get; private set; }
        public float MaterialNickel { get; private set; }
        public float MaterialPhosphorus { get; private set; }
        public float MaterialSulphur { get; private set; }
        public float Material1Amt { get; private set; }
        public float Material2Amt { get; private set; }
        public float Material3Amt { get; private set; }
        public float Material4Amt { get; private set; }
        public float Material5Amt { get; private set; }
        public float Material6Amt { get; private set; }
        #endregion
        #region Planet float
        public float MassEM { get; private set; }
        public float SurfaceGravity { get; private set; }
        public float CompositionMetal { get; private set; }
        public float CompositionRock { get; private set; }
        public float CompositionIce { get; private set; }
        #endregion
        #region Star float
        public float AbsoluteMagnitude { get; private set; }
        public float StellarMass { get; private set; }
        #endregion
        public short BodyID { get; private set; }
        public short Age_MY { get; private set; }
        public short SoftwareVersionId { get; private set; }
        public byte ScanTypeId { get; private set; }
        public byte ReserveLevelId { get; private set; }
        public CommonBodyFlags BodyFlags { get; private set; }
        public CommonScanFlags ScanFlags { get; private set; }
        public OrbitScanFlags OrbitFlags { get; private set; }
        #region Atmosphere byte
        public byte AtmosphereComponent1Id { get; private set; }
        public byte AtmosphereComponent2Id { get; private set; }
        public byte AtmosphereComponent3Id { get; private set; }
        public byte AtmosphereId { get; private set; }
        public byte AtmosphereTypeId { get; private set; }
        public AtmosphereScanFlags AtmosphereFlags { get; private set; }
        #endregion
        #region Materials byte
        public byte Material1Id { get; private set; }
        public byte Material2Id { get; private set; }
        public byte Material3Id { get; private set; }
        public byte Material4Id { get; private set; }
        public byte Material5Id { get; private set; }
        public byte Material6Id { get; private set; }
        #endregion
        #region Planet byte
        public byte PlanetClassId { get; private set; }
        public byte VolcanismId { get; private set; }
        public byte TerraformStateId { get; private set; }
        public PlanetScanFlags PlanetFlags { get; private set; }
        #endregion
        #region Star byte
        public byte StarTypeId { get; private set; }
        public byte LuminosityId { get; private set; }
        public StarScanFlags StarFlags { get; private set; }
        #endregion
        public bool TidalLock { get { return OrbitFlags.HasFlag(OrbitScanFlags.TidalLock); } }
        public bool VolcanismMinor { get { return PlanetFlags.HasFlag(PlanetScanFlags.VolcanismMinor); } }
        public bool VolcanismMajor { get { return PlanetFlags.HasFlag(PlanetScanFlags.VolcanismMajor); } }
        public bool IsLandable { get { return PlanetFlags.HasFlag(PlanetScanFlags.IsLandable); } }
        public bool IsMaterialsDict { get { return PlanetFlags.HasFlag(PlanetScanFlags.IsMaterialsDict); } }
        public bool IsBasicScan { get { return BodyFlags.HasFlag(CommonBodyFlags.IsBasicScan); } }
        public bool IsPos3SigFig { get { return BodyFlags.HasFlag(CommonBodyFlags.IsPos3SigFig); } }
        public bool IsStar { get { return BodyFlags.HasFlag(CommonBodyFlags.IsStar); } }
        public bool IsPlanet { get { return BodyFlags.HasFlag(CommonBodyFlags.IsPlanet); } }
        public bool AtmosphereHot { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.AtmosphereHot); } }
        public bool AtmosphereThick { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.AtmosphereThick); } }
        public bool AtmosphereThin { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.AtmosphereThin); } }
        public bool HasAxialTilt { get { return ScanFlags.HasFlag(CommonScanFlags.HasAxialTilt); } }
        public bool HasTidalLock { get { return OrbitFlags.HasFlag(OrbitScanFlags.HasTidalLock); } }
        public bool HasRings { get { return BodyFlags.HasFlag(CommonBodyFlags.HasRings); } }
        public bool HasDetailedScan { get { return BodyFlags.HasFlag(CommonBodyFlags.HasDetailedScan); } }
        public bool HasLandable { get { return PlanetFlags.HasFlag(PlanetScanFlags.HasLandable); } }
        public bool HasComposition { get { return PlanetFlags.HasFlag(PlanetScanFlags.HasComposition); } }
        public bool HasOrbit { get { return OrbitFlags.HasFlag(OrbitScanFlags.HasSemiMajorAxis); } }
        public bool HasParents { get { return BodyFlags.HasFlag(CommonBodyFlags.HasParents); } }
        public bool HasBodyID { get { return BodyFlags.HasFlag(CommonBodyFlags.HasBodyId); } }
        public bool HasSystemAddress { get { return BodyFlags.HasFlag(CommonBodyFlags.HasSystemAddress); } }
        public bool HasLuminosity { get { return StarFlags.HasFlag(StarScanFlags.HasLuminosity); } }
        public bool HasAtmosphereType { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.HasAtmosphereType); } }
        public bool HasAtmosphereComposition { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.HasAtmosphereComposition); } }
        public bool HasAtmosphere { get { return AtmosphereFlags.HasFlag(AtmosphereScanFlags.HasAtmosphere); } }
        public bool HasMaterials { get { return PlanetFlags.HasFlag(PlanetScanFlags.HasMaterials); } }

        public bool IsPartial
        {
            get
            {
                return !PgBody.IsBelt && (
                       (!HasOrbit && (!IsStar || PgBody.Stars != 0 || PgBody.Planet != 0 || CustomBodyName != null)) ||
                       (IsStar && (!StarFlags.HasFlag(StarScanFlags.HasAge) || StellarMass == 0)) ||
                       (IsPlanet && SurfaceGravity != 0 && Radius != 0 && MassEM != 0 && SurfaceGravity * Radius * Radius / MassEM < 3.5e14));
            }
        }

        public Models.SoftwareVersion Software { get { return BodyDatabase.GetSoftwareVersion(SoftwareVersionId); } }
        public string StarType { get { return BodyDatabase.StarType.GetName(StarTypeId); } }
        public string PlanetClass { get { return BodyDatabase.PlanetClass.GetName(PlanetClassId); } }
        public string Luminosity { get { return BodyDatabase.Luminosity.GetName(LuminosityId); } }
        public string Volcanism { get { return BodyDatabase.Volcanism.GetName(VolcanismId); } }
        public string TerraformState { get { return BodyDatabase.TerraformState.GetName(TerraformStateId); } }

        private static readonly string[] VeryCommonMaterials = new[] { "carbon", "iron", "nickel", "sulphur", "phosphorus" };

        private static object GetJsonExtraAddEntry<T>(ref T token, string propname, Dictionary<string, JToken> ret, Action<T> addent)
            where T : JToken, new()
        {
            if (token == null)
            {
                token = new T();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            addent(token);

            return token;
        }

        private static object GetJsonExtraAddEntry(ref JArray token, string propname, Dictionary<string, JToken> ret, JsonReader rdr)
        {
            if (token == null)
            {
                token = new JArray();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            var jprop = JToken.Load(rdr);
            token.Add(jprop);
            return jprop;
        }

        private static object GetJsonExtraAddEntry(ref JArray token, string propname, Dictionary<string, JToken> ret, JToken val)
        {
            if (token == null)
            {
                token = new JArray();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            token.Add(val);

            return val;
        }

        private static object GetJsonExtraAddEntry(ref JObject token, string propname, Dictionary<string, JToken> ret, string name, JsonReader rdr)
        {
            if (token == null)
            {
                token = new JObject();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            JToken jprop = JToken.Load(rdr);
            token[name] = jprop;
            return jprop;
        }

        private static void GetJsonExtraAddEntry(ref JObject token, string propname, Dictionary<string, JToken> ret, string name, JToken val)
        {
            if (token == null)
            {
                token = new JObject();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            token[name] = val;
        }

        private void ReadHeader(JsonReader rdr)
        {
            if (rdr.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
            }

            string softwareName = null;
            string softwareVersion = null;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
            {
                if (rdr.TokenType != JsonToken.PropertyName)
                {
                    throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                }

                string propname = (string)rdr.Value;
                rdr.Read();
                JsonHeader[propname] = softwareVersion;

                switch (propname)
                {
                    case "gatewayTimestamp":
                        GatewayTimestamp = (DateTime)rdr.Value;
                        break;
                    case "softwareName":
                        softwareName = (string)rdr.Value;
                        break;
                    case "softwareVersion":
                        softwareVersion = (string)rdr.Value;
                        break;
                    case "uploaderID":
                    case "manuallyApproved":
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid header field: {propname}");
                }
            }

            if (softwareName == null || softwareVersion == null)
            {
                //throw new InvalidOperationException("No software version");
            }

            if (softwareName != null && softwareVersion != null)
            {
                SoftwareVersionId = BodyDatabase.GetOrAddSoftwareVersion(softwareName, softwareVersion).Id;
            }
        }

        private object ReadStarPos(JsonReader rdr)
        {
            double[] pos = new double[3];

            if (rdr.TokenType != JsonToken.StartArray)
            {
                throw new InvalidOperationException($"Expected StartArray; got {rdr.TokenType}");
            }

            rdr.Read();

            if (rdr.TokenType != JsonToken.Float && rdr.TokenType != JsonToken.Integer)
            {
                throw new InvalidOperationException($"Expected Float; got {rdr.TokenType}");
            }

            double x = Convert.ToDouble(rdr.Value);
            pos[0] = x;

            rdr.Read();

            if (rdr.TokenType != JsonToken.Float && rdr.TokenType != JsonToken.Integer)
            {
                throw new InvalidOperationException($"Expected Float; got {rdr.TokenType}");
            }

            double y = Convert.ToDouble(rdr.Value);
            pos[1] = y;

            rdr.Read();

            if (rdr.TokenType != JsonToken.Float && rdr.TokenType != JsonToken.Integer)
            {
                throw new InvalidOperationException($"Expected Float; got {rdr.TokenType}");
            }

            double z = Convert.ToDouble(rdr.Value);
            pos[2] = z;

            rdr.Read();

            if (rdr.TokenType != JsonToken.EndArray)
            {
                throw new InvalidOperationException($"Expected EndArray; got {rdr.TokenType}");
            }

            float x32 = (float)(Math.Floor(x * 32 + 0.5) / 32.0);
            float y32 = (float)(Math.Floor(y * 32 + 0.5) / 32.0);
            float z32 = (float)(Math.Floor(z * 32 + 0.5) / 32.0);

            BodyFlags |= (x32 != x || y32 != y || z32 != z) ? CommonBodyFlags.IsPos3SigFig : 0;
            StarPosX = x32;
            StarPosY = y32;
            StarPosZ = z32;

            return pos;
        }

        private void ReadVolcanism(string val)
        {
            string volcanism = val;
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

            VolcanismId = BodyDatabase.Volcanism.GetId(volcanism) ?? 0;
            PlanetFlags |=
                (volcanismMinor ? PlanetScanFlags.VolcanismMinor : 0) |
                (volcanismMajor ? PlanetScanFlags.VolcanismMajor : 0) |
                PlanetScanFlags.HasVolcanism;
        }

        private object ReadComposition(JsonReader rdr)
        {
            Dictionary<string, object> jprop = new Dictionary<string, object>();

            if (rdr.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (rdr.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
            }

            while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
            {
                if (rdr.TokenType != JsonToken.PropertyName)
                {
                    throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                }

                string propname = (string)rdr.Value;
                rdr.Read();
                jprop[propname] = rdr.Value;

                switch (propname)
                {
                    case "Metal":
                        CompositionMetal = Convert.ToSingle(rdr.Value);
                        PlanetFlags |= PlanetScanFlags.HasComposition;
                        break;
                    case "Rock":
                        CompositionRock = Convert.ToSingle(rdr.Value);
                        PlanetFlags |= PlanetScanFlags.HasComposition;
                        break;
                    case "Ice":
                        CompositionIce = Convert.ToSingle(rdr.Value);
                        PlanetFlags |= PlanetScanFlags.HasComposition;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid solid component: {propname}");
                }
            }

            return jprop;
        }

        private void ReadAtmosphere(string val)
        {
            string atmosphere = val;
            bool atmosphereHot = false;
            bool atmosphereThin = false;
            bool atmosphereThick = false;

            if (atmosphere != null)
            {
                if (atmosphere.StartsWith("hot "))
                {
                    atmosphereHot = true;
                    atmosphere = atmosphere.Substring(4);
                }
                if (atmosphere.StartsWith("thin "))
                {
                    atmosphereThin = true;
                    atmosphere = atmosphere.Substring(5);
                }
                if (atmosphere.StartsWith("thick "))
                {
                    atmosphereThick = true;
                    atmosphere = atmosphere.Substring(6);
                }
            }

            AtmosphereId = BodyDatabase.Atmosphere.GetId(atmosphere) ?? 0;
            AtmosphereFlags |=
                (atmosphereHot ? AtmosphereScanFlags.AtmosphereHot : 0) |
                (atmosphereThick ? AtmosphereScanFlags.AtmosphereThick : 0) |
                (atmosphereThin ? AtmosphereScanFlags.AtmosphereThin : 0);

            if (atmosphere != "")
            {
                AtmosphereFlags |= AtmosphereScanFlags.HasAtmosphere;
                PlanetFlags |= PlanetScanFlags.HasAtmosphere;
            }
        }

        private object ReadAtmosphereComposition(JsonReader rdr)
        {
            List<object> jprops = new List<object>();

            if (rdr.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (rdr.TokenType != JsonToken.StartArray)
            {
                throw new InvalidOperationException($"Expected StartArray; got {rdr.TokenType}");
            }

            int i = 0;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
            {
                if (i >= 3)
                {
                    throw new InvalidOperationException("AtmosphereComposition has too many items");
                }
                else if (rdr.TokenType != JsonToken.StartObject)
                {
                    throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
                }

                Dictionary<string, object> jprop = new Dictionary<string, object>();

                string name = null;
                float amt = 0;

                while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                {
                    if (rdr.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                    }

                    string propname = (string)rdr.Value;
                    rdr.Read();
                    jprop[propname] = rdr.Value;

                    switch (propname)
                    {
                        case "Name":
                            name = (string)rdr.Value;
                            break;
                        case "Percent":
                            amt = Convert.ToSingle(rdr.Value);
                            break;
                    }
                }

                AtmosphereFlags |= AtmosphereScanFlags.HasAtmosphereComposition;
                PlanetFlags |= PlanetScanFlags.HasAtmosphere;

                switch (i)
                {
                    case 0:
                        AtmosphereComponent1Id = BodyDatabase.AtmosphereComponent.GetId(name) ?? 0;
                        AtmosphereComponent1Amt = amt;
                        break;
                    case 1:
                        AtmosphereComponent2Id = BodyDatabase.AtmosphereComponent.GetId(name) ?? 0;
                        AtmosphereComponent2Amt = amt;
                        break;
                    case 2:
                        AtmosphereComponent3Id = BodyDatabase.AtmosphereComponent.GetId(name) ?? 0;
                        AtmosphereComponent3Amt = amt;
                        break;
                }

                jprops.Add(jprop);

                i++;
            }

            return jprops;
        }

        private object ReadMaterials(JsonReader rdr)
        {

            if (rdr.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (rdr.TokenType == JsonToken.StartObject)
            {
                List<Tuple<byte, float>> mats = new List<Tuple<byte, float>>();
                Dictionary<string, object> jprop = new Dictionary<string, object>();

                int i = 0;
                while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                {
                    if (rdr.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                    }

                    string matname = (string)rdr.Value;

                    rdr.Read();
                    jprop[matname] = rdr.Value;

                    switch (matname)
                    {
                        case "carbon":
                            MaterialCarbon = Convert.ToSingle(rdr.Value);
                            break;
                        case "iron":
                            MaterialIron = Convert.ToSingle(rdr.Value);
                            PlanetFlags |= PlanetScanFlags.HasMaterials;
                            break;
                        case "nickel":
                            MaterialNickel = Convert.ToSingle(rdr.Value);
                            break;
                        case "phosphorus":
                            MaterialPhosphorus = Convert.ToSingle(rdr.Value);
                            break;
                        case "sulphur":
                            MaterialSulphur = Convert.ToSingle(rdr.Value);
                            break;
                        default:
                            if (i >= 6)
                            {
                                throw new InvalidOperationException("Too many materials");
                            }

                            float amt = Convert.ToSingle(rdr.Value);
                            byte matid = BodyDatabase.MaterialName.GetId(matname) ?? 0;

                            mats.Add(new Tuple<byte, float>(matid, amt));

                            i++;
                            break;
                    }
                }

                if (mats.Count == 6)
                {
                    mats = mats.OrderByDescending(m => m.Item2).ToList();
                    Material1Id = mats[0].Item1;
                    Material1Amt = mats[0].Item2;
                    Material2Id = mats[1].Item1;
                    Material2Amt = mats[1].Item2;
                    Material3Id = mats[2].Item1;
                    Material3Amt = mats[2].Item2;
                    Material4Id = mats[3].Item1;
                    Material4Amt = mats[3].Item2;
                    Material5Id = mats[4].Item1;
                    Material5Amt = mats[4].Item2;
                    Material6Id = mats[5].Item1;
                    Material6Amt = mats[5].Item2;
                }
                else if (mats.Count != 0)
                {
                    throw new InvalidOperationException("Too few materials");
                }

                return jprop;
            }
            else if (rdr.TokenType == JsonToken.StartArray)
            {
                List<Tuple<byte, float>> mats = new List<Tuple<byte, float>>();
                List<object> jprops = new List<object>();

                int i = 0;

                while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
                {
                    if (rdr.TokenType != JsonToken.StartObject)
                    {
                        throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
                    }

                    Dictionary<string, object> jprop = new Dictionary<string, object>();

                    string matname = null;
                    byte matid = 0;
                    float amt = 0;

                    while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                    {
                        if (rdr.TokenType != JsonToken.PropertyName)
                        {
                            throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                        }

                        string propname = (string)rdr.Value;
                        rdr.Read();
                        jprop[propname] = rdr.Value;

                        switch (propname)
                        {
                            case "Name":
                                matname = (string)rdr.Value;
                                matid = BodyDatabase.MaterialName.GetId(matname) ?? 0;
                                break;
                            case "Percent":
                                amt = Convert.ToSingle(rdr.Value);
                                break;
                        }
                    }

                    switch (matname)
                    {
                        case "carbon":
                            MaterialCarbon = amt;
                            break;
                        case "iron":
                            MaterialIron = amt;
                            PlanetFlags |= PlanetScanFlags.HasMaterials;
                            break;
                        case "nickel":
                            MaterialNickel = amt;
                            break;
                        case "phosphorus":
                            MaterialPhosphorus = amt;
                            break;
                        case "sulphur":
                            MaterialSulphur = amt;
                            break;
                        default:
                            if (i >= 6)
                            {
                                throw new InvalidOperationException("Too many materials");
                            }

                            mats.Add(new Tuple<byte, float>(matid, amt));

                            i++;
                            break;

                    }

                    jprops.Add(jprop);
                }

                if (mats.Count == 6)
                {
                    mats = mats.OrderByDescending(m => m.Item2).ToList();
                    Material1Id = mats[0].Item1;
                    Material1Amt = mats[0].Item2;
                    Material2Id = mats[1].Item1;
                    Material2Amt = mats[1].Item2;
                    Material3Id = mats[2].Item1;
                    Material3Amt = mats[2].Item2;
                    Material4Id = mats[3].Item1;
                    Material4Amt = mats[3].Item2;
                    Material5Id = mats[4].Item1;
                    Material5Amt = mats[4].Item2;
                    Material6Id = mats[5].Item1;
                    Material6Amt = mats[5].Item2;
                }
                else if (mats.Count != 0)
                {
                    throw new InvalidOperationException("Too few materials");
                }

                return jprops;
            }
            else
            {
                throw new InvalidOperationException($"Expected StartObject or StartArray; got {rdr.TokenType}");
            }
        }

        private object ReadRings(JsonReader rdr, out XBodyScanRingStruct[] rings)
        {
            rings = new XBodyScanRingStruct[4];
            List<Dictionary<string, object>> jprops = new List<Dictionary<string, object>>();

            if (rdr.TokenType == JsonToken.Null)
            {
                rings = null;
                return null;
            }
            else if (rdr.TokenType != JsonToken.StartArray)
            {
                throw new InvalidOperationException($"Expected StartArray; got {rdr.TokenType}");
            }

            int i = 0;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
            {
                if (i >= 4)
                {
                    throw new InvalidOperationException("Too many rings");
                }
                else if (rdr.TokenType != JsonToken.StartObject)
                {
                    throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
                }

                Dictionary<string, object> jprop = new Dictionary<string, object>();
                XBodyScanRingStruct ring = new XBodyScanRingStruct();

                while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                {
                    if (rdr.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                    }

                    string propname = (string)rdr.Value;
                    rdr.Read();
                    jprop[propname] = rdr.Value;

                    switch (propname)
                    {
                        case "Name":
                            ring.Name = (string)rdr.Value;
                            break;
                        case "RingClass":
                            ring.Class = (string)rdr.Value;
                            break;
                        case "InnerRad":
                            ring.InnerRad = Convert.ToSingle(rdr.Value);
                            break;
                        case "OuterRad":
                            ring.OuterRad = Convert.ToSingle(rdr.Value);
                            break;
                        case "MassMT":
                            ring.MassMT = Convert.ToSingle(rdr.Value);
                            break;
                    }
                }

                BodyFlags |= CommonBodyFlags.HasRings;

                rings[i] = ring;
                jprops.Add(jprop);

                i++;
            }

            return jprops;
        }

        private void ProcessParents(JArray ja)
        {
            List<XParentEntry> parents = new List<XParentEntry>();

            foreach (JObject jo in ja)
            {
                if (jo.Count != 1)
                {
                    throw new InvalidOperationException("Expected single-entry dictionary");
                }

                XParentEntry parent = default;
                JProperty jp = jo.Properties().Single();
                parent.Type = jp.Name;
                parent.BodyID = Convert.ToInt16(jp.Value);
                BodyFlags |= CommonBodyFlags.HasParents;

                parents.Add(parent);
            }

            parents.Reverse();

            while (parents.Count < 8)
            {
                parents.Add(new XParentEntry());
            }

            Parent0 = parents[0];
            Parent1 = parents[1];
            Parent2 = parents[2];
            Parent3 = parents[3];
            Parent4 = parents[4];
            Parent5 = parents[5];
            Parent6 = parents[6];
            Parent7 = parents[7];
        }

        private static void CheckRingName(ref XBodyScanRingStruct ring, char n, string bodyname)
        {
            if (ring.Name != null)
            {
                if (ring.Name.EndsWith(" Belt"))
                {
                    ring.IsBelt = true;
                }

                if (ring.Name == $"{bodyname} {n} {(ring.IsBelt ? "Belt" : "Ring")}")
                {
                    ring.Name = null;
                }
                else
                {
                    //System.Diagnostics.Debugger.Break();
                }
            }
        }

        private void ReadMessage(JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            if (rdr.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
            }

            string bodyname = null;
            string starsystem = null;
            short bodyid = -1;
            JArray parents = null;
            XBodyScanRingStruct[] rings = null;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
            {
                if (rdr.TokenType != JsonToken.PropertyName)
                {
                    throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                }

                string propname = (string)rdr.Value;
                rdr.Read();

                object jprop = rdr.Value;

                switch (propname)
                {
                    case "event":
                        if ((string)rdr.Value != "Scan")
                        {
                            throw new InvalidOperationException($"Invalid event type: {rdr.Value}");
                        }
                        break;
                    case "_marketId":
                    case "_systemName":
                    case "_shipId":
                    case "_systemAddress":
                    case "_stationName":
                        break;
                    case "_systemCoordinates":
                        jprop = JToken.Load(rdr);
                        break;
                    /*
                     * Header
                     */
                    case "timestamp":
                        ScanTimestamp = (DateTime)rdr.Value;
                        jprop = ScanTimestamp.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
                        break;
                    case "DistanceFromArrivalLS":
                        DistanceFromArrivalLS = Convert.ToSingle(rdr.Value);
                        break;
                    case "ScanType":
                        ScanTypeId = BodyDatabase.ScanType.GetId((string)rdr.Value) ?? 0;
                        BodyFlags |= CommonBodyFlags.HasScanTypeId;
                        break;
                    /*
                     * Scan.Body.System
                     */
                    case "StarSystem":
                        starsystem = (string)rdr.Value;
                        break;
                    case "SystemAddress":
                        SystemAddress = (long)rdr.Value;
                        BodyFlags |= CommonBodyFlags.HasSystemAddress;
                        break;
                    case "StarPos":
                        jprop = ReadStarPos(rdr);
                        break;
                    /*
                     * Scan.Body
                     */
                    case "BodyName":
                        bodyname = (string)rdr.Value;
                        break;
                    case "BodyID":
                        bodyid = Convert.ToInt16(rdr.Value);
                        break;
                    /*
                     * Scan.Data
                     */
                    case "AxialTilt":
                        AxialTilt = Convert.ToSingle(rdr.Value);
                        ScanFlags |= CommonScanFlags.HasAxialTilt;
                        break;
                    case "Eccentricity":
                        Eccentricity = Convert.ToSingle(rdr.Value);
                        OrbitFlags |= OrbitScanFlags.HasEccentricity;
                        break;
                    case "OrbitalInclination":
                        OrbitalInclination = Convert.ToSingle(rdr.Value);
                        OrbitFlags |= OrbitScanFlags.HasInclination;
                        break;
                    case "OrbitalPeriod":
                        OrbitalPeriod = Convert.ToSingle(rdr.Value);
                        OrbitFlags |= OrbitScanFlags.HasOrbitalPeriod;
                        break;
                    case "Periapsis":
                        Periapsis = Convert.ToSingle(rdr.Value);
                        OrbitFlags |= OrbitScanFlags.HasPeriapsis;
                        break;
                    case "SemiMajorAxis":
                        SemiMajorAxis = Convert.ToSingle(rdr.Value);
                        OrbitFlags |= OrbitScanFlags.HasSemiMajorAxis;
                        break;
                    case "Radius":
                        Radius = Convert.ToSingle(rdr.Value);
                        ScanFlags |= CommonScanFlags.HasRadius;
                        break;
                    case "RotationPeriod":
                        RotationPeriod = Convert.ToSingle(rdr.Value);
                        ScanFlags |= CommonScanFlags.HasRotationPeriod;
                        break;
                    case "SurfaceTemperature":
                        SurfaceTemperature = Convert.ToSingle(rdr.Value);
                        ScanFlags |= CommonScanFlags.HasSurfaceTemperature;
                        break;
                    case "TidalLock":
                        OrbitFlags |= ((bool)rdr.Value ? OrbitScanFlags.TidalLock : 0) | OrbitScanFlags.HasTidalLock;
                        break;
                    /*
                     * Scan.Planet
                     */
                    case "PlanetClass":
                        PlanetClassId = BodyDatabase.PlanetClass.GetId((string)rdr.Value) ?? 0;
                        PlanetFlags |= PlanetScanFlags.HasPlanetClass;
                        BodyFlags |= CommonBodyFlags.IsPlanet;
                        break;
                    case "MassEM":
                        MassEM = Convert.ToSingle(rdr.Value);
                        PlanetFlags |= PlanetScanFlags.HasMass;
                        break;
                    case "SurfaceGravity":
                        SurfaceGravity = Convert.ToSingle(rdr.Value);
                        PlanetFlags |= PlanetScanFlags.HasSurfaceGravity;
                        break;
                    case "TerraformState":
                        TerraformStateId = BodyDatabase.TerraformState.GetId((string)rdr.Value) ?? 0;
                        PlanetFlags |= PlanetScanFlags.HasTerraformState;
                        BodyFlags |= CommonBodyFlags.HasDetailedScan;
                        break;
                    case "Volcanism":
                        ReadVolcanism((string)rdr.Value);
                        BodyFlags |= CommonBodyFlags.HasDetailedScan;
                        break;
                    case "Landable":
                        PlanetFlags |= ((bool)rdr.Value ? PlanetScanFlags.IsLandable : 0) | PlanetScanFlags.HasLandable;
                        BodyFlags |= CommonBodyFlags.HasDetailedScan;
                        break;
                    case "Composition":
                        jprop = ReadComposition(rdr);
                        break;
                    /*
                     * Scan.Planet.Atmosphere
                     */
                    case "Atmosphere":
                        ReadAtmosphere((string)rdr.Value);
                        break;
                    case "AtmosphereType":
                        AtmosphereTypeId = BodyDatabase.AtmosphereType.GetId((string)rdr.Value) ?? 0;
                        AtmosphereFlags |= AtmosphereScanFlags.HasAtmosphereType;
                        break;
                    case "SurfacePressure":
                        SurfacePressure = Convert.ToSingle(rdr.Value);
                        AtmosphereFlags |= AtmosphereScanFlags.HasSurfacePressure;
                        if (SurfacePressure != 0)
                        {
                            PlanetFlags |= PlanetScanFlags.HasAtmosphere;
                        }
                        break;
                    case "AtmosphereComposition":
                        jprop = ReadAtmosphereComposition(rdr);
                        break;
                    /*
                     * Scan.Planet.Materials
                     */
                    case "Materials":
                        jprop = ReadMaterials(rdr);
                        break;
                    /*
                     * Scan.Star
                     */
                    case "StarType":
                        StarTypeId = BodyDatabase.StarType.GetId((string)rdr.Value) ?? 0;
                        StarFlags |= StarScanFlags.HasStarType;
                        BodyFlags |= CommonBodyFlags.IsStar;
                        break;
                    case "Luminosity":
                        LuminosityId = BodyDatabase.Luminosity.GetId((string)rdr.Value) ?? 0;
                        StarFlags |= StarScanFlags.HasLuminosity;
                        break;
                    case "StellarMass":
                        StellarMass = Convert.ToSingle(rdr.Value);
                        StarFlags |= StarScanFlags.HasStellarMass;
                        break;
                    case "Age_MY":
                        Age_MY = Convert.ToInt16(rdr.Value);
                        StarFlags |= StarScanFlags.HasAge;
                        break;
                    case "AbsoluteMagnitude":
                        AbsoluteMagnitude = Convert.ToSingle(rdr.Value);
                        StarFlags |= StarScanFlags.HasAbsoluteMagnitude;
                        break;
                    /*
                     * Scan.Rings
                     */
                    case "Rings":
                        jprop = ReadRings(rdr, out rings);
                        break;
                    case "ReserveLevel":
                        ReserveLevelId = BodyDatabase.ReserveLevel.GetId((string)rdr.Value) ?? 0;
                        BodyFlags |= CommonBodyFlags.HasReserveLevel;
                        break;
                    case "Parents":
                        parents = JArray.Load(rdr);
                        jprop = parents;
                        break;
                    default:
                        Trace.WriteLine($"Unrecognised property {propname}");
                        var jextra = JToken.Load(rdr);
                        jsonextra[propname] = jextra;
                        jprop = jextra;
                        break;
                }

                JsonMessage[propname] = jprop;
            }

            if (ScanTimestamp >= XDatabase.ed303 && ScanTimestamp < XDatabase.ed304)
            {
                jsonextra["BodyID"] = bodyid;
                jsonextra["Parents"] = parents;
                bodyid = -1;
                parents = null;
            }

            if (bodyid >= 0)
            {
                BodyID = bodyid;
                BodyFlags |= CommonBodyFlags.HasBodyId;
            }

            if (parents != null)
            {
                ProcessParents(parents);
            }

            if (SystemStruct.TryParse(starsystem, out SystemStruct pgsystem))
            {
                PgSystemName = pgsystem;
                if (SystemAddress == 0)
                {
                    SystemAddress = PgSystemName.ToSystemAddress();
                }
            }
            else
            {
                PgSystemName = new SystemStruct();
                CustomSystemName = starsystem;
            }

            if (SystemBodyStruct.TryParse(bodyname, starsystem, out SystemBodyStruct pgbody))
            {
                PgBody = pgbody;
            }
            else
            {
                CustomBodyName = bodyname;
            }

            if (rings != null)
            {
                CheckRingName(ref rings[0], 'A', bodyname);
                CheckRingName(ref rings[1], 'B', bodyname);
                CheckRingName(ref rings[2], 'C', bodyname);
                CheckRingName(ref rings[3], 'D', bodyname);

                RingA = rings[0];
                RingB = rings[1];
                RingC = rings[2];
                RingD = rings[3];
            }
        }

        public static XScanData Read(JsonReader rdr)
        {
            XScanData scan = new XScanData
            {
                BodyID = -1
            };

            Dictionary<string, JToken> jsonextra = new Dictionary<string, JToken>();

            if (rdr.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
            }

            while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
            {
                if (rdr.TokenType != JsonToken.PropertyName)
                {
                    throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                }

                string propname = (string)rdr.Value;
                rdr.Read();

                switch (propname)
                {
                    case "header":
                        scan.ReadHeader(rdr);
                        break;
                    case "message":
                        scan.ReadMessage(rdr, jsonextra);
                        break;
                    case "$schemaRef":
                        scan.JsonSchema = (string)rdr.Value;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid root property: {propname}");
                }
            }

            if (jsonextra.Count != 0)
            {
                JObject retjo = new JObject();

                foreach (var kvp in jsonextra)
                {
                    retjo[kvp.Key] = kvp.Value;
                }

                //System.Diagnostics.Trace.WriteLine($"Extra JSON: {retjo.ToString()}");
                scan.JsonExtra = retjo.ToString();
            }

            return scan;
        }

    }
}
