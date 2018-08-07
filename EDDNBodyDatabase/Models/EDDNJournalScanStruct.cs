using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace EDDNBodyDatabase.Models
{
    public struct EDDNJournalScanStruct
    {
        [Flags]
        public enum ScanFlags : short
        {
            None = 0x00000000,
            HasParents = 0x00000001,
            HasAxialTilt = 0x00000002,
            HasBodyID = 0x00000004,
            HasSystemAddress = 0x00000008,
            HasLuminosity = 0x00000010,
            HasComposition = 0x00000020,
            IsMaterialsDict = 0x00000040,
            IsBasicScan = 0x00000080,
            IsPos3SigFig = 0x00000100,
            HasAtmosphereType = 0x00000200,
            HasAtmosphereComposition = 0x00000400,
        };

        public struct DataStruct : IEquatable<DataStruct>
        {
            public string JsonExtra;
            public DateTime GatewayTimestamp;
            public DateTime ScanTimestamp;
            public float DistanceFromArrivalLS;
            public short SoftwareVersionId;
            public byte ScanTypeId;
            public bool HasParents;
            public bool HasAxialTilt;
            public bool HasBodyID;
            public bool HasSystemAddress;
            public bool HasLuminosity;
            public bool HasComposition;
            public bool IsMaterialsDict;
            public bool IsBasicScan;
            public bool IsPos3SigFig;
            public bool HasAtmosphereType;
            public bool HasAtmosphereComposition;

            public ScanFlags Flags
            {
                get
                {
                    return (HasParents ? ScanFlags.HasParents : ScanFlags.None) |
                           (HasAxialTilt ? ScanFlags.HasAxialTilt : ScanFlags.None) |
                           (HasBodyID ? ScanFlags.HasBodyID : ScanFlags.None) |
                           (HasSystemAddress ? ScanFlags.HasSystemAddress : ScanFlags.None) |
                           (HasLuminosity ? ScanFlags.HasLuminosity : ScanFlags.None) |
                           (HasComposition ? ScanFlags.HasComposition : ScanFlags.None) |
                           (IsMaterialsDict ? ScanFlags.IsMaterialsDict : ScanFlags.None) |
                           (IsBasicScan ? ScanFlags.IsBasicScan : ScanFlags.None) |
                           (IsPos3SigFig ? ScanFlags.IsPos3SigFig : ScanFlags.None) |
                           (HasAtmosphereType ? ScanFlags.HasAtmosphereType : ScanFlags.None) |
                           (HasAtmosphereComposition ? ScanFlags.HasAtmosphereComposition : ScanFlags.None);
                }
                set
                {
                    HasParents = value.HasFlag(ScanFlags.HasParents);
                    HasAxialTilt = value.HasFlag(ScanFlags.HasAxialTilt);
                    HasBodyID = value.HasFlag(ScanFlags.HasBodyID);
                    HasSystemAddress = value.HasFlag(ScanFlags.HasSystemAddress);
                    HasLuminosity = value.HasFlag(ScanFlags.HasLuminosity);
                    HasComposition = value.HasFlag(ScanFlags.HasComposition);
                    IsMaterialsDict = value.HasFlag(ScanFlags.IsMaterialsDict);
                    IsBasicScan = value.HasFlag(ScanFlags.IsBasicScan);
                    IsPos3SigFig = value.HasFlag(ScanFlags.IsPos3SigFig);
                    HasAtmosphereType = value.HasFlag(ScanFlags.HasAtmosphereType);
                    HasAtmosphereComposition = value.HasFlag(ScanFlags.HasAtmosphereComposition);
                }
            }

            public string ScanType { get { return BodyDatabase.ScanType.GetName(ScanTypeId); } set { ScanTypeId = BodyDatabase.ScanType.GetId(value) ?? 0; } }

            public SoftwareVersion SoftwareVersion
            {
                get
                {
                    return BodyDatabase.GetSoftwareVersion(SoftwareVersionId);
                }
                set
                {
                    SoftwareVersionId = BodyDatabase.GetOrAddSoftwareVersion(value.SoftwareName, value.Version).Id;
                }
            }

            public override int GetHashCode()
            {
                return GatewayTimestamp.GetHashCode();
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
                return x.GatewayTimestamp == y.GatewayTimestamp &&
                       x.SoftwareVersionId == y.SoftwareVersionId &&
                       x.ScanTimestamp == y.ScanTimestamp &&
                       x.DistanceFromArrivalLS == y.DistanceFromArrivalLS &&
                       x.ScanTypeId == y.ScanTypeId &&
                       x.HasParents == y.HasParents &&
                       x.HasAxialTilt == y.HasAxialTilt &&
                       x.HasBodyID == y.HasBodyID &&
                       x.HasSystemAddress == y.HasSystemAddress &&
                       x.HasLuminosity == y.HasLuminosity &&
                       x.HasComposition == y.HasComposition &&
                       x.IsMaterialsDict == y.IsMaterialsDict &&
                       x.IsBasicScan == y.IsBasicScan &&
                       x.JsonExtra == y.JsonExtra;
            }

            public static int GetHashCode(in DataStruct x)
            {
                return x.GatewayTimestamp.GetHashCode();
            }
        }

        public DataStruct Header;
        public BodyScanStruct Scan;

        private static HashSet<string> RemoveMessageJsonExtraKeys = new HashSet<string>
        {
            "timestamp",
            "event",
            "StarSystem",
            "StarPos",
            "SystemAddress",
            "BodyName",
            "BodyID",
            "ScanType",
            "AxialTilt",
            "Eccentricity",
            "OrbitalInclination",
            "Periapsis",
            "SemiMajorAxis",
            "OrbitalPeriod",
            "Radius",
            "RotationPeriod",
            "SurfaceTemperature",
            "SemiMajorAxis",
            "StarType",
            "AbsoluteMagnitude",
            "Age_MY",
            "StellarMass",
            "Luminosity",
            "Volcanism",
            "PlanetClass",
            "MassEM",
            "SurfaceGravity",
            "Landable",
            "TerraformState",
            "SurfacePressure",
            "AtmosphereType",
            "Atmosphere",
            "TidalLock",
            "DistanceFromArrivalLS",
            "ReserveLevel",
            "_marketId",
            "_systemCoordinates",
            "_systemName",
            "_shipId",
            "_systemAddress",
            "_stationName",
        };
        private static string[] RemoveCompositionKeys = new[] { "Metal", "Rock", "Ice" };
        private static string[] VeryCommonMaterials = new[] { "carbon", "iron", "nickel", "sulphur", "phosphorus" };

        private static void GetJsonExtraAddEntry<T>(ref T token, string propname, Dictionary<string, JToken> ret, Action<T> addent)
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
        }

        private static void GetJsonExtraAddEntry(ref JArray token, string propname, Dictionary<string, JToken> ret, JsonReader rdr)
        {
            if (token == null)
            {
                token = new JArray();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            token.Add(JToken.Load(rdr));
        }

        private static void GetJsonExtraAddEntry(ref JArray token, string propname, Dictionary<string, JToken> ret, JToken val)
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
        }

        private static void GetJsonExtraAddEntry(ref JObject token, string propname, Dictionary<string, JToken> ret, string name, JsonReader rdr)
        {
            if (token == null)
            {
                token = new JObject();
            }

            if (!ret.ContainsKey(propname))
            {
                ret[propname] = token;
            }

            token[name] = JToken.Load(rdr);
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

        private static void GetJsonExtraComposition(string propname, JsonReader reader, BodyScanStruct scan, Dictionary<string, JToken> ret)
        {
            if (reader.TokenType == JsonToken.StartObject && scan.Planet.Data.HasComposition)
            {
                JObject comp = null;

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {reader.TokenType}");
                    }

                    string component = (string)reader.Value;

                    reader.Read();
                    JToken val = JToken.Load(reader);

                    if (!RemoveCompositionKeys.Contains(component) || (val.Type != JTokenType.Float && val.Type != JTokenType.Integer))
                    {
                        GetJsonExtraAddEntry(ref comp, propname, ret, t => t[component] = val);
                    }
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                ret[propname] = JToken.Load(reader);
            }
        }

        private static void GetJsonExtraAtmosphereComposition(string propname, JsonReader reader, BodyScanStruct scan, Dictionary<string, JToken> ret)
        {
            if (reader.TokenType == JsonToken.StartArray && scan.Planet.HasAtmosphere)
            {
                JArray comp = null;
                HashSet<string> acomps = new HashSet<string>
                {
                    scan.Planet.Atmosphere.Component1Name,
                    scan.Planet.Atmosphere.Component2Name,
                    scan.Planet.Atmosphere.Component3Name,
                };

                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject c = JObject.Load(reader);
                        if (!acomps.Contains(c.Value<string>("Name")))
                        {
                            GetJsonExtraAddEntry(ref comp, propname, ret, c);
                        }
                    }
                    else
                    {
                        GetJsonExtraAddEntry(ref comp, propname, ret, reader);
                    }
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                ret[propname] = JToken.Load(reader);
            }
        }

        private static void GetJsonExtraMaterials(string propname, JsonReader reader, BodyScanStruct scan, Dictionary<string, JToken> ret)
        {
            var pmats = scan.Planet.Materials;
            HashSet<string> matnames = new HashSet<string>(new[]
            {
                "carbon", "iron", "nickel", "phosphorus", "sulphur",
                pmats.Material1Name,
                pmats.Material2Name,
                pmats.Material3Name,
                pmats.Material4Name,
                pmats.Material5Name,
                pmats.Material6Name,
            }.Where(a => a != null));

            if (reader.TokenType == JsonToken.StartObject && scan.Planet.HasMaterials)
            {
                JObject mats = null;

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {reader.TokenType}");
                    }

                    string matname = (string)reader.Value;

                    reader.Read();

                    JToken mat = JToken.Load(reader);

                    if (!matnames.Contains(matname) || (mat.Type != JTokenType.Float && mat.Type != JTokenType.Integer && mat.Type != JTokenType.Null))
                    {
                        GetJsonExtraAddEntry(ref mats, propname, ret, matname, mat);
                    }
                }
            }
            else if (reader.TokenType == JsonToken.StartArray && scan.Planet.HasMaterials)
            {
                JArray mats = null;

                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject c = JObject.Load(reader);
                        if (!matnames.Contains(c.Value<string>("Name")))
                        {
                            GetJsonExtraAddEntry(ref mats, propname, ret, c);
                        }
                    }
                    else
                    {
                        GetJsonExtraAddEntry(ref mats, propname, ret, reader);
                    }
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                ret[propname] = JToken.Load(reader);
            }
        }

        private static void GetJsonExtraRings(string propname, JsonReader reader, BodyScanStruct scan, Dictionary<string, JToken> ret)
        {
            if (reader.TokenType == JsonToken.StartArray && scan.HasRings)
            {
                JArray rings = null;
                string[] ringnames = new string[]
                {
                    scan.Rings.RingA.Name,
                    scan.Rings.RingB.Name,
                    scan.Rings.RingC.Name,
                    scan.Rings.RingD.Name
                };

                int i = 0;
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject c = JObject.Load(reader);
                        if (i >= 4 || (ringnames[i] != null && c.Value<string>("Name") != ringnames[i]))
                        {
                            GetJsonExtraAddEntry(ref rings, propname, ret, c);
                        }
                    }
                    else
                    {
                        GetJsonExtraAddEntry(ref rings, propname, ret, reader);
                    }

                    i++;
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                ret[propname] = JToken.Load(reader);
            }
        }

        private static void GetJsonExtraParents(string propname, JsonReader reader, BodyScanStruct scan, Dictionary<string, JToken> ret)
        {
            if (reader.TokenType == JsonToken.StartArray && scan.HasRings)
            {
                JArray parents = null;
                ParentSetStruct.ParentEntry[] psets = new ParentSetStruct.ParentEntry[]
                {
                    scan.Parents.Parent0,
                    scan.Parents.Parent1,
                    scan.Parents.Parent2,
                    scan.Parents.Parent3,
                    scan.Parents.Parent4,
                    scan.Parents.Parent5,
                    scan.Parents.Parent6,
                    scan.Parents.Parent7
                };

                int i = 0;
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JObject c = JObject.Load(reader);
                        if (i >= 8 || c.Count != 1)
                        {
                            GetJsonExtraAddEntry(ref parents, propname, ret, c);
                        }
                        else
                        {
                            JProperty prop = c.Properties().First();

                            if (prop.Name != psets[i].Type || prop.Value.Type != JTokenType.Integer || prop.Value.Value<int>() != psets[i].BodyID)
                            {
                                GetJsonExtraAddEntry(ref parents, propname, ret, c);
                            }
                        }
                    }
                    else
                    {
                        GetJsonExtraAddEntry(ref parents, propname, ret, reader);
                    }

                    i++;
                }
            }
            else if (reader.TokenType != JsonToken.Null)
            {
                ret[propname] = JToken.Load(reader);
            }
        }

        private static string GetJsonExtra(JObject message, BodyScanStruct scan)
        {
            Dictionary<string, JToken> ret = new Dictionary<string, JToken>();

            using (var reader = message.CreateReader())
            {
                reader.Read();
                if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new InvalidOperationException($"Expected StartObject; got {reader.TokenType}");
                }

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {reader.TokenType}");
                    }

                    string propname = (string)reader.Value;
                    reader.Read();

                    switch (propname)
                    {
                        case "Composition":
                            GetJsonExtraComposition(propname, reader, scan, ret);
                            break;
                        case "AtmosphereComposition":
                            GetJsonExtraAtmosphereComposition(propname, reader, scan, ret);
                            break;
                        case "Materials":
                            GetJsonExtraMaterials(propname, reader, scan, ret);
                            break;
                        case "Rings":
                            GetJsonExtraRings(propname, reader, scan, ret);
                            break;
                        case "Parents":
                            GetJsonExtraParents(propname, reader, scan, ret);
                            break;
                        default:
                            {
                                JToken val = JToken.Load(reader);
                                if (!RemoveMessageJsonExtraKeys.Contains(propname))
                                {
                                    ret[propname] = val;
                                }
                            }
                            break;
                    }
                }
            }

            if (ret.Count == 0)
            {
                return null;
            }
            else
            {
                JObject retjo = new JObject();

                foreach (var kvp in ret)
                {
                    retjo[kvp.Key] = kvp.Value;
                }

                global::System.Diagnostics.Trace.WriteLine($"Extra JSON: {retjo.ToString()}");
                return retjo.ToString();
            }
        }

        public static EDDNJournalScanStruct FromJSON(JObject jo)
        {
            JObject header = jo["header"] as JObject;
            JObject message = jo["message"] as JObject;
            var scan = BodyScanStruct.FromJson(message);

            return new EDDNJournalScanStruct
            {
                Header = new DataStruct
                {
                    GatewayTimestamp = header.Value<DateTime>("gatewayTimestamp"),
                    SoftwareVersion = BodyDatabase.GetOrAddSoftwareVersion(header.Value<string>("softwareName"), header.Value<string>("softwareVersion")),
                    DistanceFromArrivalLS = message.Value<float>("DistanceFromArrivalLS"),
                    ScanTimestamp = message.Value<DateTime>("timestamp"),
                    ScanType = BodyDatabase.ScanType.Intern(message.Value<string>("ScanType")),
                    HasAxialTilt = scan.Data.HasAxialTilt,
                    HasLuminosity = scan.Star.Luminosity != null,
                    HasParents = scan.HasParents,
                    HasBodyID = scan.BaseData.SystemBody.BodyID >= 0,
                    HasSystemAddress = scan.BaseData.SystemBody.System.Data.SystemAddress != 0,
                    HasComposition = scan.Planet.Data.HasComposition,
                    IsMaterialsDict = message["Materials"] is JObject,
                    IsBasicScan = !scan.HasRings && !scan.Data.HasTidalLock && !scan.Planet.Data.HasDetailedScan,
                    JsonExtra = GetJsonExtra(message, scan)
                },
                Scan = scan,
            };
        }

        private static void ReadHeader(ref EDDNJournalScanStruct scan, JsonReader rdr)
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

                switch (propname)
                {
                    case "gatewayTimestamp":
                        scan.Header.GatewayTimestamp = (DateTime)rdr.Value;
                        break;
                    case "softwareName":
                        softwareName = (string)rdr.Value;
                        break;
                    case "softwareVersion":
                        softwareVersion = (string)rdr.Value;
                        break;
                    case "uploaderId":
                        break;
                }
            }

            if (softwareName != null && softwareVersion != null)
            {
                scan.Header.SoftwareVersion = BodyDatabase.GetOrAddSoftwareVersion(softwareName, softwareVersion);
            }
        }

        private static void ReadStarPos(ref EDDNJournalScanStruct scan, JsonReader rdr)
        {
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

            rdr.Read();

            if (rdr.TokenType != JsonToken.Float && rdr.TokenType != JsonToken.Integer)
            {
                throw new InvalidOperationException($"Expected Float; got {rdr.TokenType}");
            }

            double y = Convert.ToDouble(rdr.Value);

            rdr.Read();

            if (rdr.TokenType != JsonToken.Float && rdr.TokenType != JsonToken.Integer)
            {
                throw new InvalidOperationException($"Expected Float; got {rdr.TokenType}");
            }

            double z = Convert.ToDouble(rdr.Value);

            rdr.Read();

            if (rdr.TokenType != JsonToken.EndArray)
            {
                throw new InvalidOperationException($"Expected EndArray; got {rdr.TokenType}");
            }

            float x32 = (float)(Math.Floor(x * 32 + 0.5) / 32.0);
            float y32 = (float)(Math.Floor(y * 32 + 0.5) / 32.0);
            float z32 = (float)(Math.Floor(z * 32 + 0.5) / 32.0);

            scan.Header.IsPos3SigFig = x32 != x || y32 != y || z32 != z;
            scan.Scan.Body.System.Data.StarPosX = x32;
            scan.Scan.Body.System.Data.StarPosY = y32;
            scan.Scan.Body.System.Data.StarPosZ = z32;
        }

        private static void ReadVolcanism(ref EDDNJournalScanStruct scan, string val)
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

            scan.Scan.Planet.Data.Volcanism = volcanism;
            scan.Scan.Planet.Data.VolcanismMinor = volcanismMinor;
            scan.Scan.Planet.Data.VolcanismMajor = volcanismMajor;
        }

        private static void ReadComposition(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            JObject jecomp = null;

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
                    case "Metal":
                        scan.Scan.Planet.Data.CompositionMetal = Convert.ToSingle(rdr.Value);
                        scan.Scan.Planet.Data.HasComposition = true;
                        scan.Header.HasComposition = true;
                        break;
                    case "Rock":
                        scan.Scan.Planet.Data.CompositionRock = Convert.ToSingle(rdr.Value);
                        scan.Scan.Planet.Data.HasComposition = true;
                        scan.Header.HasComposition = true;
                        break;
                    case "Ice":
                        scan.Scan.Planet.Data.CompositionIce = Convert.ToSingle(rdr.Value);
                        scan.Scan.Planet.Data.HasComposition = true;
                        scan.Header.HasComposition = true;
                        break;
                    default:
                        GetJsonExtraAddEntry(ref jecomp, "Composition", jsonextra, propname, rdr);
                        break;
                }
            }
        }

        private static void ReadAtmosphere(ref EDDNJournalScanStruct scan, string val)
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

            scan.Scan.Planet.Atmosphere.Atmosphere = atmosphere;
            scan.Scan.Planet.Atmosphere.AtmosphereHot = atmosphereHot;
            scan.Scan.Planet.Atmosphere.AtmosphereThick = atmosphereThick;
            scan.Scan.Planet.Atmosphere.AtmosphereThin = atmosphereThin;

            if (atmosphere != "")
            {
                scan.Scan.Planet.HasAtmosphere = true;
            }
        }

        private static void ReadAtmosphereComposition(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            JArray jecomp = null;

            if (rdr.TokenType != JsonToken.StartArray)
            {
                jsonextra["AtmosphereComposition"] = JToken.Load(rdr);
                return;
            }

            int i = 0;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
            {
                if (rdr.TokenType == JsonToken.StartObject && i < 3)
                {
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

                    scan.Header.HasAtmosphereComposition = true;
                    scan.Scan.Planet.HasAtmosphere = true;

                    switch (i)
                    {
                        case 0:
                            scan.Scan.Planet.Atmosphere.Component1Name = name;
                            scan.Scan.Planet.Atmosphere.Component1Amt = amt;
                            break;
                        case 1:
                            scan.Scan.Planet.Atmosphere.Component2Name = name;
                            scan.Scan.Planet.Atmosphere.Component2Amt = amt;
                            break;
                        case 2:
                            scan.Scan.Planet.Atmosphere.Component3Name = name;
                            scan.Scan.Planet.Atmosphere.Component3Amt = amt;
                            break;
                    }
                }
                else
                {
                    GetJsonExtraAddEntry(ref jecomp, "AtmosphereComposition", jsonextra, rdr);
                }

                i++;
            }
        }

        private static void ReadMaterials(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            if (rdr.TokenType == JsonToken.StartObject)
            {
                JObject mats = null;

                int i = 0;
                while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                {
                    if (rdr.TokenType != JsonToken.PropertyName)
                    {
                        throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                    }

                    string matname = (string)rdr.Value;

                    rdr.Read();

                    switch (matname)
                    {
                        case "carbon":
                            scan.Scan.Planet.Materials.MaterialCarbon = Convert.ToSingle(rdr.Value);
                            break;
                        case "iron":
                            scan.Scan.Planet.Materials.MaterialIron = Convert.ToSingle(rdr.Value);
                            scan.Scan.Planet.HasMaterials = true;
                            scan.Header.IsMaterialsDict = true;
                            break;
                        case "nickel":
                            scan.Scan.Planet.Materials.MaterialNickel = Convert.ToSingle(rdr.Value);
                            break;
                        case "phosphorus":
                            scan.Scan.Planet.Materials.MaterialPhosphorus = Convert.ToSingle(rdr.Value);
                            break;
                        case "sulphur":
                            scan.Scan.Planet.Materials.MaterialSulphur = Convert.ToSingle(rdr.Value);
                            break;
                        default:
                            if (i < 6)
                            {
                                float amt = Convert.ToSingle(rdr.Value);

                                switch (i)
                                {
                                    case 0:
                                        scan.Scan.Planet.Materials.Material1Name = matname;
                                        scan.Scan.Planet.Materials.Material1Amt = amt;
                                        break;
                                    case 1:
                                        scan.Scan.Planet.Materials.Material2Name = matname;
                                        scan.Scan.Planet.Materials.Material2Amt = amt;
                                        break;
                                    case 2:
                                        scan.Scan.Planet.Materials.Material3Name = matname;
                                        scan.Scan.Planet.Materials.Material3Amt = amt;
                                        break;
                                    case 3:
                                        scan.Scan.Planet.Materials.Material4Name = matname;
                                        scan.Scan.Planet.Materials.Material4Amt = amt;
                                        break;
                                    case 4:
                                        scan.Scan.Planet.Materials.Material5Name = matname;
                                        scan.Scan.Planet.Materials.Material5Amt = amt;
                                        break;
                                    case 5:
                                        scan.Scan.Planet.Materials.Material6Name = matname;
                                        scan.Scan.Planet.Materials.Material6Amt = amt;
                                        break;
                                }
                            }
                            else
                            {
                                GetJsonExtraAddEntry(ref mats, "Materials", jsonextra, matname, rdr);
                            }

                            i++;
                            break;
                    }
                }
            }
            else if (rdr.TokenType == JsonToken.StartArray)
            {
                JArray mats = null;
                int i = 0;

                while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
                {
                    if (rdr.TokenType == JsonToken.StartObject && i < 6)
                    {
                        string matname = null;
                        float amt = 0;

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
                                case "Name":
                                    matname = (string)rdr.Value;
                                    break;
                                case "Percent":
                                    amt = Convert.ToSingle(rdr.Value);
                                    break;
                            }
                        }

                        switch (matname)
                        {
                            case "carbon":
                                scan.Scan.Planet.Materials.MaterialCarbon = amt;
                                break;
                            case "iron":
                                scan.Scan.Planet.Materials.MaterialIron = amt;
                                scan.Scan.Planet.HasMaterials = true;
                                scan.Header.IsMaterialsDict = false;
                                break;
                            case "nickel":
                                scan.Scan.Planet.Materials.MaterialNickel = amt;
                                break;
                            case "phosphorus":
                                scan.Scan.Planet.Materials.MaterialPhosphorus = amt;
                                break;
                            case "sulphur":
                                scan.Scan.Planet.Materials.MaterialSulphur = amt;
                                break;
                            default:
                                switch (i)
                                {
                                    case 0:
                                        scan.Scan.Planet.Materials.Material1Name = matname;
                                        scan.Scan.Planet.Materials.Material1Amt = amt;
                                        break;
                                    case 1:
                                        scan.Scan.Planet.Materials.Material2Name = matname;
                                        scan.Scan.Planet.Materials.Material2Amt = amt;
                                        break;
                                    case 2:
                                        scan.Scan.Planet.Materials.Material3Name = matname;
                                        scan.Scan.Planet.Materials.Material3Amt = amt;
                                        break;
                                    case 3:
                                        scan.Scan.Planet.Materials.Material4Name = matname;
                                        scan.Scan.Planet.Materials.Material4Amt = amt;
                                        break;
                                    case 4:
                                        scan.Scan.Planet.Materials.Material5Name = matname;
                                        scan.Scan.Planet.Materials.Material5Amt = amt;
                                        break;
                                    case 5:
                                        scan.Scan.Planet.Materials.Material6Name = matname;
                                        scan.Scan.Planet.Materials.Material6Amt = amt;
                                        break;
                                }

                                i++;
                                break;

                        }
                    }
                    else
                    {
                        JToken tok = JToken.Load(rdr);

                        if (tok is JObject jo && jo["Name"] != null && VeryCommonMaterials.Contains(jo.Value<string>("Name")))
                        {
                            switch (jo.Value<string>("Name"))
                            {
                                case "carbon":
                                    scan.Scan.Planet.Materials.MaterialCarbon = jo.Value<float>("Percent");
                                    break;
                                case "iron":
                                    scan.Scan.Planet.Materials.MaterialIron = jo.Value<float>("Percent");
                                    scan.Scan.Planet.HasMaterials = true;
                                    scan.Header.IsMaterialsDict = false;
                                    break;
                                case "nickel":
                                    scan.Scan.Planet.Materials.MaterialNickel = jo.Value<float>("Percent");
                                    break;
                                case "phosphorus":
                                    scan.Scan.Planet.Materials.MaterialPhosphorus = jo.Value<float>("Percent");
                                    break;
                                case "sulphur":
                                    scan.Scan.Planet.Materials.MaterialSulphur = jo.Value<float>("Percent");
                                    break;
                            }
                        }
                        else
                        {
                            GetJsonExtraAddEntry(ref mats, "Materials", jsonextra, rdr);
                        }
                    }
                }
            }
            else if (rdr.TokenType != JsonToken.Null)
            {
                jsonextra["Materials"] = JToken.Load(rdr);
            }
        }

        private static void ReadRings(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            JArray jerings = null;

            if (rdr.TokenType != JsonToken.StartArray)
            {
                jsonextra["Rings"] = JToken.Load(rdr);
                return;
            }

            int i = 0;

            while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
            {
                if (rdr.TokenType == JsonToken.StartObject && i < 4)
                {
                    BodyScanRingStruct ring = new BodyScanRingStruct();

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

                    scan.Scan.HasRings = true;

                    switch (i)
                    {
                        case 0:
                            scan.Scan.Rings.RingA = ring;
                            break;
                        case 1:
                            scan.Scan.Rings.RingB = ring;
                            break;
                        case 2:
                            scan.Scan.Rings.RingC = ring;
                            break;
                        case 3:
                            scan.Scan.Rings.RingD = ring;
                            break;
                    }
                }
                else
                {
                    GetJsonExtraAddEntry(ref jerings, "Rings", jsonextra, rdr);
                }

                i++;
            }

        }

        private static void ReadParents(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            JArray jeparents = null;

            if (rdr.TokenType != JsonToken.StartArray)
            {
                jsonextra["Parents"] = JToken.Load(rdr);
                return;
            }

            List<ParentSetStruct.ParentEntry> parents = new List<ParentSetStruct.ParentEntry>();

            while (rdr.Read() && rdr.TokenType != JsonToken.EndArray)
            {
                if (rdr.TokenType == JsonToken.StartObject && parents.Count < 8)
                {
                    ParentSetStruct.ParentEntry parent = default;
                    JObject jo = null;

                    while (rdr.Read() && rdr.TokenType != JsonToken.EndObject)
                    {
                        if (rdr.TokenType != JsonToken.PropertyName)
                        {
                            throw new InvalidOperationException($"Expected PropertyName; got {rdr.TokenType}");
                        }

                        string propname = (string)rdr.Value;
                        rdr.Read();
                        if (rdr.TokenType == JsonToken.Integer && jo == null && parent.Type == null)
                        {
                            parent.Type = propname;
                            parent.BodyID = Convert.ToInt16(rdr.Value);
                        }
                        else
                        {
                            if (jo == null)
                            {
                                jo = new JObject();
                                if (parent.Type != null)
                                {
                                    jo[parent.Type] = parent.BodyID;
                                    parent.Type = null;
                                }
                            }

                            jo[propname] = JToken.Load(rdr);
                        }
                    }

                    if (jo != null)
                    {
                        jeparents.Add(jo);
                    }
                    else
                    {
                        scan.Scan.HasParents = true;
                        scan.Header.HasParents = true;

                        parents.Add(parent);
                    }
                }
                else
                {
                    GetJsonExtraAddEntry(ref jeparents, "Parents", jsonextra, rdr);
                }
            }

            parents.Reverse();
            
            while (parents.Count < 8)
            {
                parents.Add(new ParentSetStruct.ParentEntry());
            }

            scan.Scan.Parents.Parent0 = parents[0];
            scan.Scan.Parents.Parent1 = parents[1];
            scan.Scan.Parents.Parent2 = parents[2];
            scan.Scan.Parents.Parent3 = parents[3];
            scan.Scan.Parents.Parent4 = parents[4];
            scan.Scan.Parents.Parent5 = parents[5];
            scan.Scan.Parents.Parent6 = parents[6];
            scan.Scan.Parents.Parent7 = parents[7];
        }

        private static void CheckRingName(ref BodyScanRingStruct ring, char n, string bodyname)
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
            }
        }

        private static void ReadMessage(ref EDDNJournalScanStruct scan, JsonReader rdr, Dictionary<string, JToken> jsonextra)
        {
            if (rdr.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException($"Expected StartObject; got {rdr.TokenType}");
            }

            string bodyname = null;
            string starsystem = null;

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
                    case "event":
                    case "_marketId":
                    case "_systemName":
                    case "_shipId":
                    case "_systemAddress":
                    case "_stationName":
                        break;
                    case "_systemCoordinates":
                        JToken.Load(rdr);
                        break;
                    /*
                     * Header
                     */
                    case "timestamp":
                        scan.Header.ScanTimestamp = (DateTime)rdr.Value;
                        break;
                    case "DistanceFromArrivalLS":
                        scan.Header.DistanceFromArrivalLS = Convert.ToSingle(rdr.Value);
                        break;
                    case "ScanType":
                        scan.Header.ScanType = (string)rdr.Value;
                        break;
                    /*
                     * Scan.Body.System
                     */
                    case "StarSystem":
                        starsystem = (string)rdr.Value;
                        break;
                    case "SystemAddress":
                        scan.Scan.Body.System.Data.SystemAddress = (long)rdr.Value;
                        scan.Header.HasSystemAddress = true;
                        break;
                    case "StarPos":
                        ReadStarPos(ref scan, rdr);
                        break;
                    /*
                     * Scan.Body
                     */
                    case "BodyName":
                        bodyname = (string)rdr.Value;
                        break;
                    case "BodyID":
                        scan.Scan.Body.BodyID = Convert.ToInt16(rdr.Value);
                        scan.Header.HasBodyID = true;
                        break;
                    /*
                     * Scan.Data
                     */
                    case "AxialTilt":
                        scan.Scan.Data.AxialTilt = Convert.ToSingle(rdr.Value);
                        scan.Scan.Data.HasAxialTilt = true;
                        scan.Header.HasAxialTilt = true;
                        break;
                    case "Eccentricity":
                        scan.Scan.Data.Eccentricity = Convert.ToSingle(rdr.Value);
                        scan.Scan.Data.HasOrbit = true;
                        break;
                    case "OrbitalInclination":
                        scan.Scan.Data.OrbitalInclination = Convert.ToSingle(rdr.Value);
                        break;
                    case "OrbitalPeriod":
                        scan.Scan.Data.OrbitalPeriod = Convert.ToSingle(rdr.Value);
                        break;
                    case "Periapsis":
                        scan.Scan.Data.Periapsis = Convert.ToSingle(rdr.Value);
                        break;
                    case "SemiMajorAxis":
                        scan.Scan.Data.SemiMajorAxis = Convert.ToSingle(rdr.Value);
                        break;
                    case "Radius":
                        scan.Scan.Data.Radius = Convert.ToSingle(rdr.Value);
                        break;
                    case "RotationPeriod":
                        scan.Scan.Data.RotationPeriod = Convert.ToSingle(rdr.Value);
                        break;
                    case "SurfaceTemperature":
                        scan.Scan.Data.SurfaceTemperature = Convert.ToSingle(rdr.Value);
                        break;
                    case "TidalLock":
                        scan.Scan.Data.TidalLock = (bool)rdr.Value;
                        scan.Scan.Data.HasTidalLock = true;
                        break;
                    /*
                     * Scan.Planet
                     */
                    case "PlanetClass":
                        scan.Scan.Planet.Data.PlanetClass = (string)rdr.Value;
                        scan.Scan.IsPlanet = true;
                        break;
                    case "MassEM":
                        scan.Scan.Planet.Data.MassEM = Convert.ToSingle(rdr.Value);
                        break;
                    case "SurfaceGravity":
                        scan.Scan.Planet.Data.SurfaceGravity = Convert.ToSingle(rdr.Value);
                        break;
                    case "TerraformState":
                        scan.Scan.Planet.Data.TerraformState = (string)rdr.Value;
                        scan.Scan.Planet.Data.HasDetailedScan = true;
                        break;
                    case "Volcanism":
                        ReadVolcanism(ref scan, (string)rdr.Value);
                        scan.Scan.Planet.Data.HasDetailedScan = true;
                        break;
                    case "Landable":
                        scan.Scan.Planet.Data.IsLandable = (bool)rdr.Value;
                        scan.Scan.Planet.Data.HasLandable = true;
                        scan.Scan.Planet.Data.HasDetailedScan = true;
                        break;
                    case "Composition":
                        ReadComposition(ref scan, rdr, jsonextra);
                        break;
                    /*
                     * Scan.Planet.Atmosphere
                     */
                    case "Atmosphere":
                        ReadAtmosphere(ref scan, (string)rdr.Value);
                        break;
                    case "AtmosphereType":
                        scan.Scan.Planet.Atmosphere.AtmosphereType = (string)rdr.Value;
                        scan.Header.HasAtmosphereType = true;
                        break;
                    case "SurfacePressure":
                        scan.Scan.Planet.Atmosphere.SurfacePressure = Convert.ToSingle(rdr.Value);
                        if (scan.Scan.Planet.Atmosphere.SurfacePressure != 0)
                        {
                            scan.Scan.Planet.HasAtmosphere = true;
                        }
                        break;
                    case "AtmosphereComposition":
                        ReadAtmosphereComposition(ref scan, rdr, jsonextra);
                        break;
                    /*
                     * Scan.Planet.Materials
                     */
                    case "Materials":
                        ReadMaterials(ref scan, rdr, jsonextra);
                        break;
                    /*
                     * Scan.Star
                     */
                    case "StarType":
                        scan.Scan.Star.StarType = (string)rdr.Value;
                        scan.Scan.IsStar = true;
                        break;
                    case "Luminosity":
                        scan.Scan.Star.Luminosity = (string)rdr.Value;
                        scan.Header.HasLuminosity = true;
                        break;
                    case "StellarMass":
                        scan.Scan.Star.StellarMass = Convert.ToSingle(rdr.Value);
                        break;
                    case "Age_MY":
                        scan.Scan.Star.Age_MY = Convert.ToInt16(rdr.Value);
                        break;
                    case "AbsoluteMagnitude":
                        scan.Scan.Star.AbsoluteMagnitude = Convert.ToSingle(rdr.Value);
                        break;
                    /*
                     * Scan.Rings
                     */
                    case "Rings":
                        ReadRings(ref scan, rdr, jsonextra);
                        break;
                    case "ReserveLevel":
                        scan.Scan.Data.ReserveLevel = (string)rdr.Value;
                        break;
                    case "Parents":
                        ReadParents(ref scan, rdr, jsonextra);
                        break;
                    default:
                        jsonextra[propname] = JToken.Load(rdr);
                        break;
                }
            }

            scan.Scan.Body.System.StarSystem = starsystem;
            if (scan.Scan.Body.System.Data.PgSystem.RegionId != 0 && scan.Scan.Body.System.Data.SystemAddress == 0)
            {
                scan.Scan.Body.System.Data.SystemAddress = scan.Scan.Body.System.Data.PgSystem.ToSystemAddress();
            }
            scan.Scan.Body.BodyName = bodyname;
            CheckRingName(ref scan.Scan.Rings.RingA, 'A', bodyname);
            CheckRingName(ref scan.Scan.Rings.RingB, 'B', bodyname);
            CheckRingName(ref scan.Scan.Rings.RingC, 'C', bodyname);
            CheckRingName(ref scan.Scan.Rings.RingD, 'D', bodyname);
        }

        public static Tuple<EDDNJournalScanStruct> Read(JsonReader rdr)
        {
            EDDNJournalScanStruct scan = new EDDNJournalScanStruct();
            scan.Scan.Body.BodyID = -1;
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
                        ReadHeader(ref scan, rdr);
                        break;
                    case "message":
                        ReadMessage(ref scan, rdr, jsonextra);
                        break;
                    default:
                        break;
                }
            }

            if (jsonextra.Count != 0)
            {
                JObject retjo = new JObject();

                foreach (var kvp in jsonextra)
                {
                    retjo[kvp.Key] = kvp.Value;
                }

                global::System.Diagnostics.Trace.WriteLine($"Extra JSON: {retjo.ToString()}");
                scan.Header.JsonExtra = retjo.ToString();
            }

            return new Tuple<EDDNJournalScanStruct>(scan);
        }
    }
}
