using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using Ionic.BZip2;
using EDDNBodyDatabase.XModels;

namespace EDDNBodyDatabase
{
    public static class BodyDatabase
    {
        private static Dictionary<long, List<Models.ParentSet>> ParentSets { get; set; }
        private static Dictionary<string, Dictionary<string, Models.SoftwareVersion>> SoftwareVersions { get; set; }
        private static Dictionary<short, Models.SoftwareVersion> SoftwareVersionsById { get; set; }
        private static Dictionary<int, Dictionary<string, List<Models.SystemBody>>> NamedBodies { get; set; }
        private static Dictionary<int, string> NamedBodySystems { get; set; }

        public static NameIdMap<Models.Atmosphere> Atmosphere { get; private set; }
        public static NameIdMap<Models.AtmosphereComponent> AtmosphereComponent { get; private set; }
        public static NameIdMap<Models.AtmosphereType> AtmosphereType { get; private set; }
        public static NameIdMap<Models.BodyType> BodyType { get; private set; }
        public static NameIdMap<Models.Luminosity> Luminosity { get; private set; }
        public static NameIdMap<Models.MaterialName> MaterialName { get; private set; }
        public static NameIdMap<Models.PlanetClass> PlanetClass { get; private set; }
        public static NameIdMap<Models.ReserveLevel> ReserveLevel { get; private set; }
        public static NameIdMap<Models.RingClass> RingClass { get; private set; }
        public static NameIdMap<Models.ScanType> ScanType { get; private set; }
        public static NameIdMap<Models.Software> Software { get; private set; }
        public static NameIdMap<Models.StarType> StarType { get; private set; }
        public static NameIdMap<Models.TerraformState> TerraformState { get; private set; }
        public static NameIdMap<Models.Volcanism> Volcanism { get; private set; }
        public static NameIdMap<Models.Region, short> Region { get; private set; }
        public static NameIdMap<Models.Region, int> RegionByAddress { get; private set; }
        public static NameIdMap<Models.BodyCustomName, short> BodyCustomName { get; private set; }

        #region EDSM Body to journal conversion
        private static Dictionary<string, string> EDSM2PlanetNames = new Dictionary<string, string>()
        {
            // EDSM name    (lower case)            Journal name                  
            { "rocky ice world",                    "Rocky ice body" },
            { "high metal content world" ,          "High metal content body"},
            { "class i gas giant",                  "Sudarsky class I gas giant"},
            { "class ii gas giant",                 "Sudarsky class II gas giant"},
            { "class iii gas giant",                "Sudarsky class III gas giant"},
            { "class iv gas giant",                 "Sudarsky class IV gas giant"},
            { "class v gas giant",                  "Sudarsky class V gas giant"},
            { "earth-like world",                   "Earthlike body" },
        };

        private static Dictionary<string, string> EDSM2StarNames = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            // EDSM name (lower case)               Journal name
            { "a (blue-white super giant) star", "A_BlueWhiteSuperGiant" },
            { "f (white super giant) star", "F_WhiteSuperGiant" },
            { "k (yellow-orange giant) star", "K_OrangeGiant" },
            { "m (red giant) star", "M_RedGiant" },
            { "m (red super giant) star", "M_RedSuperGiant" },
            { "black hole", "H" },
            { "c star", "C" },
            { "cj star", "CJ" },
            { "cn star", "CN" },
            { "herbig ae/be star", "AeBe" },
            { "ms-type star", "MS" },
            { "neutron star", "N" },
            { "s-type star", "S" },
            { "t tauri star", "TTS" },
            { "wolf-rayet c star", "WC" },
            { "wolf-rayet n star", "WN" },
            { "wolf-rayet nc star", "WNC" },
            { "wolf-rayet o star", "WO" },
            { "wolf-rayet star", "W" },
        };

        private static Dictionary<string, string> EDSM2Atmosphere = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["Ammonia and Oxygen"] = "",
            ["Suitable for water-based life"] = "",
            ["No atmosphere"] = "",
            ["Hot Thick Ammonia and Oxygen"] = "hot thick  atmosphere",
            ["Hot Thick Suitable for water-based life"] = "hot thick  atmosphere",
            ["Hot Thick No atmosphere"] = "hot thick  atmosphere",
            ["Hot Thin Ammonia and Oxygen"] = "hot thin  atmosphere",
            ["Hot Thin Suitable for water-based life"] = "hot thin  atmosphere",
            ["Hot Thin No atmosphere"] = "hot thin  atmosphere",
            ["Thick Ammonia and Oxygen"] = "thick  atmosphere",
            ["Thick Suitable for water-based life"] = "thick  atmosphere",
            ["Thick No atmosphere"] = "thick  atmosphere",
            ["Thin Ammonia and Oxygen"] = "thin  atmosphere",
            ["Thin Suitable for water-based life"] = "thin  atmosphere",
            ["Thin No atmosphere"] = "thin  atmosphere",
        };

        private static Dictionary<string, string> EDSM2AtmosphereType = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["No atmosphere"] = null,
            ["Ammonia and Oxygen"] = "AmmoniaOxygen",
            ["Suitable for water-based life"] = "EarthLike",
        };

        private static Dictionary<string, string> EDSM2Volcanism = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
        };

        private static Dictionary<string, string> EDSM2TerraformingState = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["Candidate for terraforming"] = "Terraformable",
            ["Not terraformable"] = "",
        };

        private const double EDSM_solarRadius_m = 695700000;
        private const double EDSM_oneLS_m = 299792458;
        private const double EDSM_oneAU_m = 149597870700;
        private const double EDSM_oneAU_LS = EDSM_oneAU_m / EDSM_oneLS_m;
        private const double EDSM_oneDay_s = 86400;
        private const double EDSM_oneMoon_MT = 73420000000000;
        private const double EDSM_oneAtmosphere_Pa = 101325;
        private const double EDSM_oneGee_m_s2 = 9.80665;

        private static string EDSMPlanet2JournalName(string inname)
        {
            return EDSM2PlanetNames.ContainsKey(inname.ToLower()) ? EDSM2PlanetNames[inname.ToLower()] : inname;
        }

        private static string EDSMStar2JournalName(string startype)
        {
            if (startype == null)
                startype = "Unknown";
            else if (EDSM2StarNames.ContainsKey(startype))
                startype = EDSM2StarNames[startype];
            else if (startype.StartsWith("White Dwarf (", StringComparison.InvariantCultureIgnoreCase))
            {
                int start = startype.IndexOf("(") + 1;
                int len = startype.IndexOf(")") - start;
                if (len > 0)
                    startype = startype.Substring(start, len);
            }
            else   // Remove extra text from EDSM   ex  "F (White) Star" -> "F"
            {
                int index = startype.IndexOf("(");
                if (index > 0)
                    startype = startype.Substring(0, index).Trim();
            }
            return startype;
        }

        private static string EDSMAtmosphere2JournalName(string atmos)
        {
            if (atmos == null)
            {
                return null;
            }
            else if (EDSM2Atmosphere.ContainsKey(atmos))
            {
                return EDSM2Atmosphere[atmos];
            }
            else
            {
                return atmos.ToLowerInvariant() + " atmosphere";
            }
        }

        private static string EDSMAtmosphereType2JournalName(string atmos)
        {
            if (atmos == null)
            {
                return null;
            }
            else
            {
                if (atmos.Equals("No atmosphere", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "None";
                }

                if (atmos.StartsWith("Hot ", StringComparison.InvariantCultureIgnoreCase))
                {
                    atmos = atmos.Substring(4);
                }

                if (atmos.StartsWith("Thin ", StringComparison.InvariantCultureIgnoreCase))
                {
                    atmos = atmos.Substring(5);
                }
                else if (atmos.StartsWith("Thick ", StringComparison.InvariantCultureIgnoreCase))
                {
                    atmos = atmos.Substring(6);
                }

                if (EDSM2AtmosphereType.ContainsKey(atmos))
                {
                    return EDSM2AtmosphereType[atmos];
                }
                else
                {
                    string atmostype = "";

                    if (atmos.EndsWith("-rich", StringComparison.InvariantCultureIgnoreCase))
                    {
                        atmos = atmos.Substring(0, atmos.Length - 5);
                        atmostype = "Rich";
                    }

                    if (atmos.EndsWith(" vapour", StringComparison.InvariantCultureIgnoreCase))
                    {
                        atmos = atmos.Substring(0, atmos.Length - 7);
                        atmostype = "Vapour" + atmostype;
                    }

                    if (atmos.EndsWith(" dioxide", StringComparison.InvariantCultureIgnoreCase))
                    {
                        atmos = atmos.Substring(0, atmos.Length - 8);
                        atmostype = "Dioxide" + atmostype;
                    }

                    return atmos.Substring(0, 1).ToUpperInvariant() + atmos.Substring(1).ToLowerInvariant() + atmostype;
                }
            }
        }

        private static string EDSMVolcanism2JournalName(string volc)
        {
            if (volc == null)
            {
                return null;
            }
            else if (EDSM2Volcanism.ContainsKey(volc))
            {
                return EDSM2Volcanism[volc];
            }
            else
            {
                return volc.ToLowerInvariant() + " volcanism";
            }
        }

        private static string EDSMTerraformingState2JournalName(string tstate)
        {
            if (tstate == null)
            {
                return null;
            }
            else if (EDSM2TerraformingState.ContainsKey(tstate))
            {
                return EDSM2TerraformingState[tstate];
            }
            else
            {
                return tstate.Substring(0, 1).ToUpperInvariant() + tstate.Substring(1).ToLowerInvariant();
            }
        }

        private static JObject ConvertFromEDSMBodies(JObject jo, Models.System sys)
        {
            JObject jout = new JObject
            {
                ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                ["event"] = "Scan",
                ["BodyName"] = jo["name"],
                ["SystemAddress"] = jo["systemId64"],
                ["StarSystem"] = jo["systemName"],
                ["StarPos"] = new JArray
                {
                    (sys.X / 32.0) - 49985,
                    (sys.Y / 32.0) - 40985,
                    (sys.Z / 32.0) - 24105
                }
            };

            if (jo["orbitalInclination"] != null) jout["OrbitalInclination"] = jo["orbitalInclination"];
            if (jo["orbitalEccentricity"] != null) jout["Eccentricity"] = jo["orbitalEccentricity"];
            if (jo["argOfPeriapsis"] != null) jout["Periapsis"] = jo["argOfPeriapsis"];
            if ((jo.Value<double?>("semiMajorAxis") ?? 0) != 0) jout["SemiMajorAxis"] = jo.Value<double>("semiMajorAxis") * EDSM_oneAU_m; // AU -> metres
            if ((jo.Value<double?>("orbitalPeriod") ?? 0) != 0) jout["OrbitalPeriod"] = jo.Value<double>("orbitalPeriod") * EDSM_oneDay_s; // days -> seconds
            if (jo.Value<bool?>("rotationalPeriodTidallyLocked") != null) jout["TidalLock"] = jo.Value<bool>("rotationalPeriodTidallyLocked");
            if (jo.Value<double?>("axialTilt") != null) jout["AxialTilt"] = jo.Value<double>("axialTilt") * Math.PI / 180.0; // degrees -> radians
            if ((jo.Value<double?>("rotationalPeriod") ?? 0) != 0) jout["RotationalPeriod"] = jo.Value<double>("rotationalPeriod") * EDSM_oneDay_s; // days -> seconds
            if (jo["surfaceTemperature"] != null) jout["SurfaceTemperature"] = jo["surfaceTemperature"];
            if (jo["distanceToArrival"] != null) jout["DistanceFromArrivalLS"] = jo["distanceToArrival"];
            if (jo["parents"] != null) jout["Parents"] = jo["parents"];
            if (jo["bodyId"] != null) jout["BodyID"] = jo.Value<long>("bodyId");

            if (!String.IsNullOrEmpty(jo.Value<string>("type")))
            {
                if (jo["type"].Value<string>().Equals("Star"))
                {
                    jout["StarType"] = EDSMStar2JournalName(jo.Value<string>("subType"));           // pass thru null to code, it will cope with it
                    jout["Age_MY"] = jo["age"];
                    jout["StellarMass"] = jo["solarMasses"];
                    jout["Radius"] = jo.Value<double>("solarRadius") * EDSM_solarRadius_m; // solar-rad -> metres
                    jout["Luminosity"] = jo["luminosity"];
                }
                else if (jo["type"].Value<string>().Equals("Planet"))
                {
                    jout["Landable"] = jo["isLandable"];
                    jout["MassEM"] = jo["earthMasses"];
                    jout["Radius"] = jo.Value<double>("radius") * 1000.0; // km -> metres
                    jout["PlanetClass"] = EDSMPlanet2JournalName(jo.Value<string>("subType"));

                    string volcanism = EDSMVolcanism2JournalName(jo.Value<string>("volcanismType"));
                    if (volcanism != null) jout["Volcanism"] = volcanism;

                    string atmos = EDSMAtmosphere2JournalName(jo.Value<string>("atmosphereType"));
                    if (atmos != null) jout["Atmosphere"] = atmos;

                    var atmostype = EDSMAtmosphereType2JournalName(jo.Value<string>("atmosphereType"));
                    if (atmostype != null) jout["AtmosphereType"] = atmostype;

                    var terraform = EDSMTerraformingState2JournalName(jo.Value<string>("terraformingState"));
                    if (terraform != null) jout["TerraformState"] = terraform;

                    if (jo["surfacePressure"] != null) jout["SurfacePressure"] = jo.Value<double>("surfacePressure") * EDSM_oneAtmosphere_Pa; // atmospheres -> pascals


                    if (jo["atmosphereComposition"] is JObject atmospherecomposition)
                    {
                        JArray atmoscomp = new JArray();

                        foreach (JProperty comp in atmospherecomposition.Properties())
                        {
                        }
                    }
                }
            }

            if ((jo["belts"] ?? jo["rings"]) is JArray rings)
            {
                JArray jring = new JArray();

                foreach (JObject ring in rings)
                {
                    jring.Add(new JObject
                    {
                        ["InnerRad"] = ring.Value<double>("innerRadius") * 1000,
                        ["OuterRad"] = ring.Value<double>("outerRadius") * 1000,
                        ["MassMT"] = ring["mass"],
                        ["RingClass"] = ring["type"],
                        ["Name"] = ring["name"]
                    });
                }

                jout["Rings"] = jring;
            }

            if (jo["materials"] is JObject mats)  // Check if materials has null
            {
                JObject mats2 = new JObject();

                foreach (JProperty prop in mats.Properties())
                {
                    if (prop.Value == null)
                    {
                        mats2[prop.Name.ToLower()] = 0.0;
                    }
                    else
                    {
                        mats2[prop.Name.ToLower()] = prop.Value<double>();
                    }
                }

                jout["Materials"] = mats2;
            }

            return jout;
        }

        #endregion

        private static long GetParentSetId(Models.ParentSet pset)
        {
            long id = (long)(((ulong)pset.Parent7BodyId & 0x1FF) |
                             (((ulong)pset.Parent6BodyId & 0x1FF) << 9) |
                             (((ulong)pset.Parent5BodyId & 0x1FF) << 18) |
                             (((ulong)pset.Parent4BodyId & 0x1FF) << 27) |
                             (((ulong)pset.Parent3BodyId & 0x1FF) << 36) |
                             (((ulong)pset.Parent2BodyId & 0x1FF) << 45) |
                             (((ulong)pset.Parent1BodyId & 0x1FF) << 54));
            return id;
        }

        private static long GetParentSetId(XParentSet pset)
        {
            long id = (long)(((ulong)pset.Parent7BodyID & 0x1FF) |
                             (((ulong)pset.Parent6BodyID & 0x1FF) << 9) |
                             (((ulong)pset.Parent5BodyID & 0x1FF) << 18) |
                             (((ulong)pset.Parent4BodyID & 0x1FF) << 27) |
                             (((ulong)pset.Parent3BodyID & 0x1FF) << 36) |
                             (((ulong)pset.Parent2BodyID & 0x1FF) << 45) |
                             (((ulong)pset.Parent1BodyID & 0x1FF) << 54));
            return id;
        }

        private static Dictionary<long, List<Models.ParentSet>> GetParentSets()
        {
            Dictionary<long, List<Models.ParentSet>> parentSets = new Dictionary<long, List<Models.ParentSet>>();

            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                foreach (Models.ParentSet pset in ctx.Set<Models.ParentSet>())
                {
                    long id = GetParentSetId(pset);

                    if (!parentSets.ContainsKey(id))
                    {
                        parentSets[id] = new List<Models.ParentSet>();
                    }

                    parentSets[id].Add(pset);
                }
            }

            return parentSets;
        }

        private static Dictionary<string, Dictionary<string, Models.SoftwareVersion>> GetSoftwareVersions()
        {
            Dictionary<string, Dictionary<string, Models.SoftwareVersion>> versions = new Dictionary<string, Dictionary<string, Models.SoftwareVersion>>(StringComparer.InvariantCultureIgnoreCase);

            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                foreach (Models.SoftwareVersion vers in ctx.Set<Models.SoftwareVersion>())
                {
                    string name = vers.SoftwareName;

                    if (!versions.ContainsKey(name))
                    {
                        versions[name] = new Dictionary<string, Models.SoftwareVersion>(StringComparer.InvariantCultureIgnoreCase);
                    }

                    versions[name][vers.Version] = vers;
                }
            }

            return versions;
        }

        public static int SystemCount()
        {
            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                return ctx.Set<Models.System>().Count();
            }
        }

        public static void LoadNamedSystems(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            using (Stream s = File.OpenRead(filename))
            {
                using (TextReader tr = new StreamReader(s, Encoding.UTF8))
                {
                    using (JsonTextReader jr = new JsonTextReader(tr))
                    {
                        System.Diagnostics.Trace.WriteLine("Reading systems from json");
                        Console.WriteLine("Loading named systems from json");

                        Dictionary<long, JObject> systemsByIndex = new Dictionary<long, JObject>();

                        while (jr.Read())
                        {
                            if (jr.TokenType == JsonToken.StartObject)
                            {
                                JObject jo = JObject.Load(jr);
                                long index = jo.Value<long?>("index") ?? jo.Value<long>("id64");
                                systemsByIndex[index] = jo;
                            }
                        }

                        System.Diagnostics.Trace.WriteLine("Processing systems");
                        List<JObject> systems = systemsByIndex.OrderBy(k => k.Key).Select(k => k.Value).ToList();

                        for (int i = 0; i < systems.Count; i += 100)
                        {
                            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
                            {
                                List<Models.System> syslist = new List<Models.System>();

                                for (int j = i; j < i + 100 && j < systems.Count; j++)
                                {
                                    JObject jo = systems[j];
                                    long id = jo.Value<long>("id64");
                                    string pgname = SystemStruct.FromSystemAddress(id).Name;
                                    string name = jo.Value<string>("name");
                                    JObject coords = (JObject)jo["coords"];
                                    float x = coords.Value<float>("x");
                                    float y = coords.Value<float>("y");
                                    float z = coords.Value<float>("z");

                                    Models.System sys = ctx.GetOrAddSystem(name, pgname, id, x, y, z, true, true);

                                    if (sys.Id == 0)
                                    {
                                        syslist.Add(sys);
                                    }
                                    //Console.Write(".");
                                    //Console.Out.Flush();
                                }

                                if (syslist.Count != 0)
                                {
                                    ctx.InsertSystems(syslist);
                                    ctx.SaveChanges();
                                }
                                //Console.WriteLine();
                            }

                            System.Diagnostics.Trace.WriteLine($"{Math.Min(i + 100, systems.Count)} systems processed");
                            Console.Write(".");

                            if (((i / 100) % 64) == 63)
                            {
                                Console.WriteLine($" {Math.Min(i + 100, systems.Count)}");
                            }
                        }

                        Console.WriteLine($" {systems.Count}");
                    }
                }
            }
        }

        public static void Init()
        {
            System.Diagnostics.Trace.WriteLine("Initializing Database");
            Console.WriteLine("Initializing Database");
            Models.BodyDbContext.InitializeDatabase();

            System.Diagnostics.Trace.WriteLine("Loading lookup tables");
            Console.WriteLine("Loading lookup tables");
            Atmosphere = new NameIdMap<Models.Atmosphere>(Models.SeedValues.Atmosphere);
            AtmosphereComponent = new NameIdMap<Models.AtmosphereComponent>(Models.SeedValues.AtmosphereComponent);
            AtmosphereType = new NameIdMap<Models.AtmosphereType>(Models.SeedValues.AtmosphereType);
            BodyType = new NameIdMap<Models.BodyType>(Models.SeedValues.BodyType);
            Luminosity = new NameIdMap<Models.Luminosity>(Models.SeedValues.Luminosity);
            MaterialName = new NameIdMap<Models.MaterialName>(Models.SeedValues.MaterialName);
            PlanetClass = new NameIdMap<Models.PlanetClass>(Models.SeedValues.PlanetClass);
            Region = new NameIdMap<Models.Region, short>(Models.SeedValues.Region);
            RegionByAddress = new NameIdMap<Models.Region, int>(new Models.Region[0]);
            ReserveLevel = new NameIdMap<Models.ReserveLevel>(Models.SeedValues.ReserveLevel);
            RingClass = new NameIdMap<Models.RingClass>(Models.SeedValues.RingClass);
            ScanType = new NameIdMap<Models.ScanType>(Models.SeedValues.ScanType);
            Software = new NameIdMap<Models.Software>(Models.SeedValues.Software);
            StarType = new NameIdMap<Models.StarType>(Models.SeedValues.StarType);
            TerraformState = new NameIdMap<Models.TerraformState>(Models.SeedValues.TerraformState);
            Volcanism = new NameIdMap<Models.Volcanism>(Models.SeedValues.Volcanism);
            BodyCustomName = new NameIdMap<Models.BodyCustomName, short>(Models.SeedValues.BodyCustomNames);
            ParentSets = GetParentSets();
            SoftwareVersions = GetSoftwareVersions();
            SoftwareVersionsById = SoftwareVersions.Values.SelectMany(s => s.Values).ToDictionary(s => s.Id);
        }

        public static void LoadEDSMSystems(TextReader rdr)
        {
            Trace.WriteLine("Processing EDSM systems");
            Console.WriteLine("Processing EDSM systems");

            int i = 0;
            List<JObject> lines = new List<JObject>();
            DateTime lastupdate = DateTime.MinValue;
            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {

                var sys = ctx.Set<Models.System>().OrderByDescending(e => e.EdsmId).FirstOrDefault();
                if (sys != null && sys.EdsmId != 0)
                {
                    lastupdate = sys.EdsmLastModified;
                }
            }

            do
            {
                lines.Clear();

                while (lines.Count < 100)
                {
                    string line = rdr.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (line.StartsWith("[") || line.StartsWith("]"))
                    {
                        continue;
                    }

                    if (line.EndsWith(","))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    JObject jo = JObject.Parse(line);
                    lines.Add(jo);
                }

                if (lines.Count != 0)
                {
                    using (Models.BodyDbContext ctx = new Models.BodyDbContext())
                    {
                        List<Models.System> syslist = new List<Models.System>();
                        bool update = false;

                        foreach (JObject jo in lines)
                        {
                            DateTime lastmod = jo.Value<DateTime>("date");
                            if (lastmod >= lastupdate)
                            {
                                int edsmid = jo.Value<int>("id");
                                long? id64 = jo.Value<long?>("id64");
                                string name = jo.Value<string>("name");
                                JObject coords = (JObject)jo["coords"];
                                float x = coords.Value<float>("x");
                                float y = coords.Value<float>("y");
                                float z = coords.Value<float>("z");

                                Models.System sys = ctx.GetOrAddSystem(name, null, id64, x, y, z, true, true, edsmid);

                                if (sys.EdsmId == 0 || sys.EdsmId != edsmid)
                                {
                                    if (sys.Id != 0)
                                    {
                                        sys = ctx.Set<Models.System>().Find(sys.Id);
                                    }

                                    sys.EdsmId = edsmid;
                                    sys.EdsmLastModified = lastmod;
                                    update = true;
                                }
                                else if (edsmid != sys.EdsmId)
                                {
                                    sys.Id = 0;
                                    sys.EdsmId = edsmid;
                                    sys.EdsmLastModified = lastmod;
                                }
                                else if (lastmod > sys.EdsmLastModified)
                                {
                                    sys = ctx.Set<Models.System>().Find(sys.Id);
                                    sys.EdsmLastModified = lastmod;
                                    update = true;
                                }

                                if (sys.Id == 0)
                                {
                                    syslist.Add(sys);
                                    update = true;
                                }
                            }
                        }

                        if (update)
                        {
                            if (syslist.Count != 0)
                            {
                                ctx.InsertSystems(syslist);
                            }

                            ctx.SaveChanges();
                        }
                    }

                    i += lines.Count;

                    Trace.WriteLine($"{i} entries processed");
                    Console.Write(".");

                    if (((i / 100) % 64) == 0)
                    {
                        Console.WriteLine($" {i}");
                    }
                }
            }
            while (lines.Count != 0);
            Console.WriteLine($" {i}");

            Trace.WriteLine("Finished processing EDSM systems");
        }

        public static void LoadEDSMSystemsWeb()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://www.edsm.net/dump/systemsWithCoordinates.json");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (Stream stream = resp.GetResponseStream())
            {
                using (TextReader tr = new StreamReader(stream, Encoding.UTF8))
                {
                    LoadEDSMSystems(tr);
                }
            }
        }

        public static void LoadEDSMSystemsLocal(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Downloading systemsWithCoordinates.json");
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://www.edsm.net/dump/systemsWithCoordinates.json", filename);
                }
            }

            using (Stream stream = File.OpenRead(filename))
            {
                using (TextReader tr = new StreamReader(stream, Encoding.UTF8))
                {
                    LoadEDSMSystems(tr);
                }
            }
        }

        public static void LoadNamedBodies(string data)
        {
            System.Diagnostics.Trace.WriteLine("Loading named bodies");
            Console.WriteLine("Loading named bodies");
            string[] lines = data.Split('\n');
            NamedBodies = new Dictionary<int, Dictionary<string, List<Models.SystemBody>>>();
            NamedBodySystems = new Dictionary<int, string>();

            using (var ctx = new Models.BodyDbContext())
            {
                foreach (var line in lines)
                {
                    string[] fields = line.Split('\t');

                    if (fields.Length >= 7 && Int64.TryParse(fields[0], out long sysaddr))
                    {
                        if (!Int16.TryParse(fields[3], out short bodyid))
                        {
                            bodyid = -1;
                        }

                        Int32.TryParse(fields[1], out int edsmid);
                        string sysname = fields[2].Trim();
                        string bodycustomname = fields[4].Trim();
                        string designation = fields[6].Trim();

                        if (edsmid != 0 && sysname != "" && bodycustomname != "" && designation != "" && SystemBodyStruct.TryParse(designation, sysname, out SystemBodyStruct pgbody))
                        {
                            Trace.WriteLine($"Processing named body {bodycustomname} ({designation} [{bodyid}]) in system {sysname} ({sysaddr})");

                            var bysysaddr = ctx.GetSystemsByAddress(sysaddr);
                            var systems = bysysaddr.Where(s => s.Name.Equals(sysname, StringComparison.InvariantCultureIgnoreCase)).ToList();

                            if (systems.Count != 1)
                            {
                                Debugger.Break();
                            }

                            foreach (var sys in systems)
                            {
                                NamedBodySystems[sys.Id] = sys.Name;

                                if (!NamedBodies.ContainsKey(sys.Id))
                                {
                                    NamedBodies[sys.Id] = new Dictionary<string, List<Models.SystemBody>>(StringComparer.InvariantCultureIgnoreCase);
                                }

                                if (!NamedBodies[sys.Id].ContainsKey(bodycustomname))
                                {
                                    NamedBodies[sys.Id][bodycustomname] = new List<Models.SystemBody>();
                                }

                                List<Models.SystemBody> bodies = new List<Models.SystemBody>();

                                if (SystemBodyStruct.TryParse(bodycustomname, sysname, out SystemBodyStruct cnbody))
                                {
                                    bodies.AddRange(ctx.GetSystemBodiesByPgName(sys.Id, XBody.From(cnbody, bodyid)).Where(b => bodyid == -1 || b.BodyID == -1 || bodyid == b.BodyID));
                                }

                                bodies.AddRange(ctx.GetSystemBodiesByCustomName(sys.Id, bodycustomname).Where(b => bodyid == -1 || b.BodyID == -1 || bodyid == b.BodyID));

                                bodies = bodies.Where(b => b.CustomName == null || b.CustomName.CustomName == bodycustomname).GroupBy(b => b.Id).Select(g => g.First()).ToList();

                                var pgmatch = bodies.Where(b => b.Stars == pgbody.Stars && b.IsBelt == pgbody.IsBelt && b.Planet == pgbody.Planet && b.Moon1 == pgbody.Moon1 && b.Moon2 == pgbody.Moon2 && b.Moon3 == pgbody.Moon3).ToList();

                                if (pgmatch.Count == 1)
                                {
                                    bodies = pgmatch;
                                }

                                if (bodies.Count > 1)
                                {
                                    var scans = bodies.ToDictionary(b => b.Id, b => ctx.GetBodyScans(b.Id).ToList());
                                    var headers = scans.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.SelectMany(s => ctx.GetEDDNScansByScanId(s.Id)).ToList());
                                    List<int> ids = null;

                                    if (pgbody.Stars == 1 && pgbody.Planet == 0)
                                    {
                                        ids = headers.Where(kvp => kvp.Value.All(s => s.DistanceFromArrivalLS == 0)).Select(kvp => kvp.Key).ToList();
                                    }
                                    else if (pgbody.Stars != 0 && pgbody.Planet == 0)
                                    {
                                        ids = headers.Where(kvp => kvp.Value.All(s => s.DistanceFromArrivalLS != 0)).Select(kvp => kvp.Key).ToList();
                                    }

                                    if (ids != null && ids.Count == 1)
                                    {
                                        bodies = bodies.Where(b => b.Id == ids[0]).ToList();
                                    }
                                }

                                if (bodies.Count > 1)
                                {
                                    Debugger.Break();
                                }

                                if (bodies.Count == 1)
                                {
                                    var body = bodies[0];
                                    var customname = body.CustomName;

                                    if (bodycustomname != designation)
                                    {
                                        body.CustomNameId = BodyDatabase.BodyCustomName.GetId(bodycustomname) ?? 0;

                                        if (body.CustomName == null)
                                        {
                                            var dbcustomname = ctx.Set<Models.SystemBodyCustomName>().Find(body.Id);

                                            customname = new Models.SystemBodyCustomName
                                            {
                                                Id = body.Id,
                                                BodyID = body.BodyID,
                                                SystemId = sys.Id,
                                                CustomName = bodycustomname
                                            };

                                            ctx.Set<Models.SystemBodyCustomName>().Add(customname);
                                        }

                                        if (body.Stars != pgbody.Stars || body.Planet != pgbody.Planet || body.Moon1 != pgbody.Moon1 || body.Moon2 != pgbody.Moon2 || body.Moon3 != pgbody.Moon3 || body.IsBelt != pgbody.IsBelt)
                                        {
                                            var dbbody = ctx.Set<Models.SystemBody>().Find(body.Id);

                                            dbbody.Stars = pgbody.Stars;
                                            dbbody.Planet = pgbody.Planet;
                                            dbbody.Moon1 = pgbody.Moon1;
                                            dbbody.Moon2 = pgbody.Moon2;
                                            dbbody.Moon3 = pgbody.Moon3;
                                            dbbody.IsBelt = pgbody.IsBelt;
                                        }

                                        body.CustomName = customname;
                                        body.Stars = pgbody.Stars;
                                        body.Planet = pgbody.Planet;
                                        body.Moon1 = pgbody.Moon1;
                                        body.Moon2 = pgbody.Moon2;
                                        body.Moon3 = pgbody.Moon3;
                                        body.IsBelt = pgbody.IsBelt;
                                    }

                                    NamedBodies[sys.Id][bodycustomname].Add(body);
                                }

                                if (bodies.Count == 0)
                                {
                                    var body = new Models.SystemBody
                                    {
                                        SystemId = sys.Id,
                                        BodyID = bodyid,
                                        Stars = pgbody.Stars,
                                        Planet = pgbody.Planet,
                                        Moon1 = pgbody.Moon1,
                                        Moon2 = pgbody.Moon2,
                                        Moon3 = pgbody.Moon3,
                                        IsBelt = pgbody.IsBelt,
                                    };

                                    if (bodycustomname != designation)
                                    {
                                        body.CustomNameId = BodyDatabase.BodyCustomName.GetId(bodycustomname) ?? 0;
                                        body.CustomName = new Models.SystemBodyCustomName
                                        {
                                            SystemId = sys.Id,
                                            BodyID = bodyid,
                                            CustomName = bodycustomname
                                        };
                                    }

                                    NamedBodies[sys.Id][bodycustomname].Add(body);
                                }
                            }
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }

        public static void LoadNamedBodiesLocal(string filename)
        {
            if (!File.Exists(filename))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://docs.google.com/spreadsheets/d/e/2PACX-1vR9lEav_Bs8rZGRtwcwuOwQ2hIoiNJ_PWYAEgXk7E3Y-UD0r6uER04y4VoQxFAAdjMS4oipPyySoC3t/pub?gid=711269421&single=true&output=tsv", filename);
                }
            }

            using (Stream stream = File.OpenRead(filename))
            {
                using (TextReader tr = new StreamReader(stream, Encoding.UTF8))
                {
                    LoadNamedBodies(tr.ReadToEnd());
                }
            }
        }

        public static void LoadNamedBodiesWeb()
        {
            WebClient client = new WebClient();
            string data = client.DownloadString("https://docs.google.com/spreadsheets/d/e/2PACX-1vR9lEav_Bs8rZGRtwcwuOwQ2hIoiNJ_PWYAEgXk7E3Y-UD0r6uER04y4VoQxFAAdjMS4oipPyySoC3t/pub?gid=711269421&single=true&output=tsv");
            LoadNamedBodies(data);
        }

        public static void LoadEDSMBodies(TextReader rdr)
        {
            Trace.WriteLine("Processing EDSM bodies");

            int i = 0;
            List<JObject> lines = new List<JObject>();

            do
            {
                lines.Clear();

                while (lines.Count < 100)
                {
                    string line = rdr.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (line.EndsWith(","))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    JObject jo = JObject.Parse(line);
                    lines.Add(jo);
                }

                if (lines.Count != 0)
                {
                    using (Models.BodyDbContext ctx = new Models.BodyDbContext())
                    {
                        List<Models.BodyScan> scanlist = new List<Models.BodyScan>();

                        foreach (JObject line in lines)
                        {
                            Models.System system = ctx.GetSystemsByEdsmId(line.Value<int>("systemId")).FirstOrDefault();
                            if (system != null)
                            {
                                JObject scanobj = ConvertFromEDSMBodies(line, system);

                                XScanData scandata = XScanData.Read(scanobj.CreateReader());
                                XScanClass scancls = XScanClass.From(scandata);
                                Models.ParentSet parents = ctx.GetOrAddParentSet(scancls.Parents);
                                Models.SystemBody sysbody = ctx.GetOrAddBody(scanobj.Value<string>("BodyName"), system, bodyid: scanobj.Value<int?>("BodyID") ?? -1, scanbasehash: scancls.ScanBase.GetHashCode(), scandata: scancls);
                                Models.BodyScan scan = ctx.GetOrAddScan(sysbody, scancls);
                            }
                        }

                        ctx.SaveChanges();
                    }

                   i += lines.Count;

                   Trace.WriteLine($"{i} entries processed");
                }
            }
            while (lines.Count != 0);
        }

        public static Models.ParentSet GetOrAddParentSet(XParentSet scan)
        {
            long id = GetParentSetId(scan);

            if (!ParentSets.ContainsKey(id))
            {
                ParentSets[id] = new List<Models.ParentSet>();
            }

            foreach (Models.ParentSet pset in ParentSets[id])
            {
                if (pset.Equals(scan))
                {
                    return pset;
                }
            }

            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                Models.ParentSet pset = ctx.GetOrAddParentSet(scan);
                ParentSets[id].Add(pset);
                ctx.SaveChanges();
                return pset;
            }
        }

        public static Models.SoftwareVersion GetOrAddSoftwareVersion(string name, string version)
        {
            if (!SoftwareVersions.ContainsKey(name))
            {
                SoftwareVersions[name] = new Dictionary<string, Models.SoftwareVersion>(StringComparer.InvariantCultureIgnoreCase);
            }

            if (!SoftwareVersions[name].ContainsKey(version))
            {
                int swid = (int)Software.GetId(name);

                using (Models.BodyDbContext ctx = new Models.BodyDbContext())
                {
                    foreach (Models.SoftwareVersion vers in ctx.Set<Models.SoftwareVersion>().Where(s => s.SoftwareId == swid && s.Version == version))
                    {
                        SoftwareVersions[name][vers.Version] = vers;
                    }

                    if (!SoftwareVersions[name].ContainsKey(version))
                    {
                        Models.SoftwareVersion vers = new Models.SoftwareVersion
                        {
                            SoftwareId = (byte)swid,
                            Version = version
                        };

                        ctx.Set<Models.SoftwareVersion>().Add(vers);

                        ctx.SaveChanges();

                        SoftwareVersions[name][version] = vers;
                        SoftwareVersionsById[vers.Id] = vers;
                    }
                }
            }

            return SoftwareVersions[name][version];
        }

        public static Models.SoftwareVersion GetSoftwareVersion(short id)
        {
            return SoftwareVersionsById[id];
        }

        public static bool GetNamedBody(int sysid, XBody pgbody, int scanbasehash, string customname, out Models.SystemBody body)
        {
            return GetNamedBody(sysid, new SystemBodyStruct { Stars = pgbody.Stars, Planet = pgbody.Planet, Moon1 = pgbody.Moon1, Moon2 = pgbody.Moon2, Moon3 = pgbody.Moon3, IsBelt = pgbody.IsBelt }, pgbody.BodyID, scanbasehash, customname, out body);
        }

        public static bool GetNamedBody(int sysid, SystemBodyStruct pgbody, short bodyid, int scanbasehash, string customname, out Models.SystemBody retbody)
        {
            retbody = null;

            if (!NamedBodies.ContainsKey(sysid) || !NamedBodySystems.ContainsKey(sysid))
            {
                return false;
            }

            string sysname = NamedBodySystems[sysid];

            if (customname == null)
            {
                customname = pgbody.GetName(sysname);
            }

            if (!NamedBodies[sysid].ContainsKey(customname))
            {
                return false;
            }

            var bodies = NamedBodies[sysid][customname];
            var bodiesById = bodies.Where(b => (bodyid == -1 || b.BodyID == -1 || bodyid == b.BodyID)).ToList();

            if (bodiesById.Count == 1)
            {
                var body = bodiesById[0];
                if (body.ScanBaseHash != scanbasehash)
                {
                    //Debugger.Break();
                }

                retbody = bodiesById[0];
                return true;
            }

            foreach (var body in bodiesById)
            {
                if (body.ScanBaseHash == scanbasehash)
                {
                    retbody = body;
                    return true;
                }
            }

            foreach (var body in bodies)
            {
                if (body.ScanBaseHash == 0 && body.Id == 0 && bodyid != -1 && body.BodyID != -1 && bodyid == body.BodyID)
                {
                    body.ScanBaseHash = scanbasehash;
                    retbody = body;
                    return true;
                }
            }

            return true;
        }

        public static void AddNamedBody(int sysid, string name, Models.SystemBody body)
        {
            if (!NamedBodies.ContainsKey(sysid))
            {
                return;
            }

            if (name == null)
            {
                SystemBodyStruct pgbody = new SystemBodyStruct
                {
                    Stars = body.Stars,
                    Planet = body.Planet,
                    Moon1 = body.Moon1,
                    Moon2 = body.Moon2,
                    Moon3 = body.Moon3,
                    IsBelt = body.IsBelt
                };

                name = pgbody.GetName(NamedBodySystems[sysid]);
            }

            if (!NamedBodies[sysid].ContainsKey(name))
            {
                return;
            }

            var bodies = NamedBodies[sysid][name];

            foreach (var nbody in bodies)
            {
                if (nbody.ScanBaseHash == body.ScanBaseHash && (nbody.BodyID == -1 || body.BodyID == -1 || nbody.BodyID == body.BodyID))
                {
                    return;
                }
            }

            foreach (var nbody in bodies)
            {
                if (body.ScanBaseHash == 0 && body.Id == 0 && nbody.BodyID == body.BodyID)
                {
                    return;
                }
            }

            bodies.Add(body);
        }
    }
}
