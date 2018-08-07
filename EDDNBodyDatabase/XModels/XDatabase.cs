using Ionic.BZip2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public class XDatabase : IDisposable
    {
        public static readonly DateTime ed304 = new DateTime(2018, 3, 27, 16, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime ed303 = new DateTime(2018, 3, 19, 10, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime ed300 = new DateTime(2018, 2, 27, 16, 0, 0, DateTimeKind.Utc);

        public HashSetList<XSystem> Systems { get; } = new HashSetList<XSystem>(XSystem.Equals, XSystem.GetHashCode);
        public Dictionary<string, Dictionary<XSystem, int>> SystemCustomNameMap { get; } = new Dictionary<string, Dictionary<XSystem, int>>();
        public Dictionary<int, string> SystemCustomNames { get; } = new Dictionary<int, string>();
        public HashSetList<XBody> Bodies { get; } = new HashSetList<XBody>(XBody.Equals, XBody.GetHashCode);
        public Dictionary<string, Dictionary<XBody, int>> BodyCustomNameMap { get; } = new Dictionary<string, Dictionary<XBody, int>>();
        public Dictionary<int, string> BodyCustomNames { get; } = new Dictionary<int, string>();
        public HashSetList<XParentSet> ParentSets { get; } = new HashSetList<XParentSet>(XParentSet.Equals, XParentSet.GetHashCode);
        public HashSetList<XScanBase> ScanBases { get; } = new HashSetList<XScanBase>(XScanBase.Equals, XScanBase.GetHashCode);
        public ParentSetList<XScanAtmosphere> Atmospheres { get; } = new ParentSetList<XScanAtmosphere>(XScanAtmosphere.Equals, XScanAtmosphere.UpdateIfEqual);
        public ParentSetList<XScanMaterials> Materials { get; } = new ParentSetList<XScanMaterials>(XScanMaterials.Equals);
        public ParentSetList<XScanPlanet> Planets { get; } = new ParentSetList<XScanPlanet>(XScanPlanet.Equals, XScanPlanet.UpdateIfEqual);
        public ParentSetList<XScanStar> Stars { get; } = new ParentSetList<XScanStar>(XScanStar.Equals, XScanStar.UpdateIfEqual);
        public ParentSetList<XScanRing> Rings { get; } = new ParentSetList<XScanRing>(XScanRing.Equals);
        public Dictionary<int, string> RingNames { get; } = new Dictionary<int, string>();
        public HashSetList<XScan> Scans { get; } = new HashSetList<XScan>(XScan.Equals, XScan.GetHashCode, XScan.UpdateIfEqual);
        public HashSetList<XScanHeader> ScanHeaders { get; } = new HashSetList<XScanHeader>(XScanHeader.Equals, XScanHeader.GetHashCode);
        public Dictionary<int, string> JsonExtra { get; } = new Dictionary<int, string>();
        public int NumRinged { get; private set; }

        private Models.DbSystemInserter _SystemInserter;
        private Models.DbBodyInserter _BodyInserter;
        private Models.DbScanInserter _ScanInserter;
        private Models.DbScanHeaderInserter _ScanHeaderInserter;
        private Models.DbScanUpdater _ScanUpdater;
        private Models.BodyDbContext _Context;

        private Models.DbSystemInserter SystemInserter { get { if (_SystemInserter == null) { SetContext(new Models.BodyDbContext()); } return _SystemInserter; } }
        private Models.DbBodyInserter BodyInserter { get { if (_BodyInserter == null) { SetContext(new Models.BodyDbContext()); } return _BodyInserter; } }
        private Models.DbScanInserter ScanInserter { get { if (_ScanInserter == null) { SetContext(new Models.BodyDbContext()); } return _ScanInserter; } }
        private Models.DbScanHeaderInserter ScanHeaderInserter { get { if (_ScanHeaderInserter == null) { SetContext(new Models.BodyDbContext()); } return _ScanHeaderInserter; } }
        private Models.DbScanUpdater ScanUpdater { get { if (_ScanUpdater == null) { SetContext(new Models.BodyDbContext()); } return _ScanUpdater; } }
        private Models.BodyDbContext Context { get { if (_Context == null) { SetContext(new Models.BodyDbContext()); } return _Context; } }

        public XDatabase SetContext(Models.BodyDbContext ctx)
        {
            _Context = ctx;
            _SystemInserter = new Models.DbSystemInserter(ctx);
            _BodyInserter = new Models.DbBodyInserter(ctx);
            _ScanInserter = new Models.DbScanInserter(ctx);
            _ScanHeaderInserter = new Models.DbScanHeaderInserter(ctx);
            _ScanUpdater = new Models.DbScanUpdater(ctx);

            return this;
        }

        public void Dispose()
        {
            if (_SystemInserter != null)
            {
                _SystemInserter.Dispose();
                _SystemInserter = null;
            }

            if (_BodyInserter != null)
            {
                _BodyInserter.Dispose();
                _BodyInserter = null;
            }

            if (_ScanInserter != null)
            {
                _ScanInserter.Dispose();
                _ScanInserter = null;
            }

            if (_ScanUpdater != null)
            {
                _ScanUpdater.Dispose();
                _ScanUpdater = null;
            }

            if (_ScanHeaderInserter != null)
            {
                _ScanHeaderInserter.Dispose();
                _ScanHeaderInserter = null;
            }
        }

        public IEnumerable<XScanClass> GetScans()
        {
            foreach (var hdr in ScanHeaders)
            {
                XScanClass scancls = new XScanClass();
                scancls.Scan = Scans[hdr.ParentId];
                scancls.Planet = scancls.Scan.Flags.HasFlag(XScan.ScanFlags.IsPlanet) ? Planets[scancls.Scan.PlanetScanId] : default;
                scancls.Atmosphere = scancls.Planet.HasAtmosphere ? Atmospheres[scancls.Planet.AtmosphereId] : default;
                scancls.Materials = scancls.Planet.HasMaterials ? Materials[scancls.Planet.MaterialsId] : default;
                scancls.Star = scancls.Scan.Flags.HasFlag(XScan.ScanFlags.IsStar) ? Stars[scancls.Scan.StarScanId] : default;
                scancls.Parents = scancls.Scan.Flags.HasFlag(XScan.ScanFlags.HasParents) ? ParentSets[scancls.Scan.ParentSetId] : default;
                scancls.ScanBase = ScanBases[scancls.Scan.ParentId];
                scancls.Body = Bodies[scancls.Scan.BodyId];
                scancls.System = Systems[scancls.Body.ParentId];
                scancls.CustomBodyName = BodyCustomNames.ContainsKey(scancls.Body.Id) ? BodyCustomNames[scancls.Body.Id] : null;
                scancls.CustomSystemName = SystemCustomNames.ContainsKey(scancls.System.Id) ? SystemCustomNames[scancls.System.Id] : null;

                if (scancls.Scan.Flags.HasFlag(XScan.ScanFlags.HasRings))
                {
                    scancls.RingA = Rings[scancls.Scan.RingScanId];
                    scancls.RingB = Rings[scancls.RingA.NextRingId];
                    scancls.RingC = Rings[scancls.RingB.NextRingId];
                    scancls.RingD = Rings[scancls.RingC.NextRingId];
                    scancls.RingAName = RingNames.ContainsKey(scancls.RingA.Id) ? RingNames[scancls.RingA.Id] : null;
                    scancls.RingBName = RingNames.ContainsKey(scancls.RingB.Id) ? RingNames[scancls.RingB.Id] : null;
                    scancls.RingCName = RingNames.ContainsKey(scancls.RingC.Id) ? RingNames[scancls.RingC.Id] : null;
                    scancls.RingDName = RingNames.ContainsKey(scancls.RingD.Id) ? RingNames[scancls.RingD.Id] : null;
                }

                scancls.JsonExtra = JsonExtra.ContainsKey(hdr.Id) ? JsonExtra[hdr.Id] : null;

                yield return scancls;
            }
        }

        private XSystem AddSystem(in XScanClass ent, out Models.System dbsys)
        {
            XSystem sys = ent.System;
            string syscustomname = ent.CustomSystemName;

            dbsys = Context.GetOrAddSystem(sys, syscustomname, true, true);

            if (dbsys.Id == 0)
            {
                SystemInserter.Insert(dbsys);
            }

            sys.DbId = dbsys.Id;

            if (syscustomname != null)
            {
                if (!SystemCustomNameMap.ContainsKey(syscustomname))
                {
                    SystemCustomNameMap[syscustomname] = new Dictionary<XSystem, int>();
                }

                if (SystemCustomNameMap[syscustomname].ContainsKey(sys))
                {
                    sys.Id = SystemCustomNameMap[syscustomname][sys];
                }
                else
                {
                    sys.Id = Systems.Add(in sys, false);
                    SystemCustomNameMap[syscustomname][sys] = sys.Id;
                    SystemCustomNames[sys.Id] = syscustomname;
                }
            }
            else
            {
                sys.Id = Systems.GetOrAdd(in sys);
            }

            return sys;
        }

        private XBody AddBody(in XScanClass ent, int sysid, int dbsysid, out Models.SystemBody dbbody)
        {
            XBody body = ent.Body;
            string bodycustomname = ent.CustomBodyName;

            body.ParentId = sysid;

            dbbody = Context.GetOrAddBody(body, dbsysid, scanbasehash: ent.ScanBase.GetHashCode(), customname: bodycustomname, noadd: true, notrack: true, scandata: ent);

            if (dbbody.Id == 0)
            {
                BodyInserter.Insert(dbbody);
            }

            body.DbId = dbbody.Id;

            if (bodycustomname != null)
            {
                if (!BodyCustomNameMap.ContainsKey(bodycustomname))
                {
                    BodyCustomNameMap[bodycustomname] = new Dictionary<XBody, int>();
                }

                if (BodyCustomNameMap[bodycustomname].ContainsKey(body))
                {
                    body.Id = BodyCustomNameMap[bodycustomname][body];
                }
                else
                {
                    body.Id = Bodies.Add(in body, false);
                    BodyCustomNameMap[bodycustomname][body] = body.Id;
                    BodyCustomNames[body.Id] = bodycustomname;
                }
            }
            else
            {
                body.Id = Bodies.GetOrAdd(in body);
            }

            return body;
        }

        private XParentSet AddParentSet(in XScanClass ent, out Models.ParentSet dbparentset)
        {
            XParentSet parentset = ent.Parents;
            dbparentset = null;

            if (ent.Header.Flags.HasFlag(XScanHeader.ScanFlags.HasParents))
            {
                if (!ParentSets.Contains(in parentset))
                {
                    dbparentset = BodyDatabase.GetOrAddParentSet(parentset);
                    parentset.DbId = dbparentset.Id;
                }

                parentset.Id = ParentSets.GetOrAdd(in parentset);
                parentset.DbId = ParentSets[parentset.Id].DbId;
            }

            return parentset;
        }

        private XScanBase AddScanBase(in XScanClass ent)
        {
            XScanBase scanbase = ent.ScanBase;
            scanbase.Id = ScanBases.GetOrAdd(in scanbase);
            return scanbase;
        }

        private XScanAtmosphere AddScanAtmosphere(in XScanClass ent, int scanbaseId)
        {
            XScanAtmosphere atmosphere = ent.Atmosphere;

            if (ent.Planet.HasAtmosphere)
            {
                atmosphere.ParentId = scanbaseId;
                atmosphere.Id = Atmospheres.GetOrAdd(in atmosphere);
            }

            return atmosphere;
        }

        private XScanMaterials AddScanMaterials(in XScanClass ent, int scanbaseId)
        {
            XScanMaterials materials = ent.Materials;

            if (ent.Planet.HasMaterials)
            {
                materials.ParentId = scanbaseId;
                materials.Id = Materials.GetOrAdd(in materials);
            }

            return materials;
        }

        private XScanPlanet AddScanPlanet(in XScanClass ent, int scanbaseId, int atmosphereId, int materialsId)
        {
            XScanPlanet planet = ent.Planet;

            if (ent.Scan.Flags.HasFlag(XScan.ScanFlags.IsPlanet))
            {
                planet.ParentId = scanbaseId;
                planet.AtmosphereId = atmosphereId;
                planet.MaterialsId = materialsId;
                planet.Id = Planets.GetOrAdd(in planet);
            }

            return planet;
        }

        private XScanStar AddScanStar(in XScanClass ent, int scanbaseId)
        {
            XScanStar star = ent.Star;

            if (ent.Scan.Flags.HasFlag(XScan.ScanFlags.IsStar))
            {
                star.ParentId = scanbaseId;
                star.Id = Stars.GetOrAdd(in star);
            }

            return star;
        }

        private XScanRing[] AddScanRings(in XScanClass ent, int scanbaseId, out string[] ringnames)
        {
            XScanRing[] rings = new XScanRing[]
            {
                ent.RingA,
                ent.RingB,
                ent.RingC,
                ent.RingD
            };
            ringnames = new string[]
            {
                ent.RingAName,
                ent.RingBName,
                ent.RingCName,
                ent.RingDName
            };

            if (ent.Scan.Flags.HasFlag(XScan.ScanFlags.HasRings))
            {
                rings[0].ParentId = scanbaseId;
                rings[1].ParentId = scanbaseId;
                rings[2].ParentId = scanbaseId;
                rings[3].ParentId = scanbaseId;
                int lastring = rings.LastOrDefault(r => r.ClassId != 0).RingNumber;
                int nextring = 0;

                if (!Rings.Contains(in rings[0]))
                {
                    NumRinged++;
                }

                for (int rn = lastring; rn >= 0; rn--)
                {
                    if (rings[rn].ClassId != 0)
                    {
                        rings[rn].NextRingId = nextring;
                        rings[rn].Id = Rings.GetOrAdd(in rings[rn]);
                        nextring = rings[rn].Id;

                        if (ringnames[rn] != null)
                        {
                            RingNames[nextring] = ringnames[rn];
                        }
                    }
                }
            }

            return rings;
        }

        private XScan AddScan(in XScanClass ent, Models.SystemBody dbbody, out Models.BodyScan dbscan)
        {
            XScan scan = ent.Scan;
            scan.ParentSetId = ent.Parents.Id;
            scan.ParentId = ent.ScanBase.Id;
            scan.PlanetScanId = ent.Planet.Id;
            scan.StarScanId = ent.Star.Id;
            scan.RingScanId = ent.RingA.Id;
            scan.BodyId = ent.Body.Id;

            dbscan = Context.GetOrAddScan(dbbody, ent, true, true);

            if (dbscan.Id == 0)
            {
                ScanInserter.Insert(dbscan);
            }
            else
            {
                ScanUpdater.Update(ent, dbbody, dbscan);
            }

            scan.DbId = dbscan.Id;

            scan.Id = Scans.GetOrAdd(in scan);
            scan.DbId = Scans[scan.Id].DbId;
            return scan;
        }

        private XScanHeader AddScanHeader(in XScanClass ent, int scanId, int dbscanid, out Models.EDDNJournalScan dbhdr)
        {
            XScanHeader hdr = ent.Header;
            hdr.ParentId = scanId;

            dbhdr = Context.GetOrAddEDDNScan(hdr, ent.JsonExtra, dbscanid, null, true, true);

            if (dbhdr.Id == 0)
            {
                ScanHeaderInserter.Insert(dbhdr);
            }

            hdr.DbId = dbhdr.Id;

            hdr.Id = ScanHeaders.GetOrAdd(in hdr);

            if (ent.JsonExtra != null)
            {
                JsonExtra[hdr.Id] = ent.JsonExtra;
            }

            return hdr;
        }

        public XScanClass Add(in XScanClass ent)
        {
            XSystem sys = AddSystem(in ent, out Models.System dbsys);
            XBody body = AddBody(in ent, sys.Id, dbsys.Id, out Models.SystemBody dbbody);
            XParentSet parentset = AddParentSet(in ent, out Models.ParentSet dbparentset);
            XScanBase scanbase = AddScanBase(in ent);
            XScanAtmosphere atmosphere = AddScanAtmosphere(in ent, scanbase.Id);
            XScanMaterials materials = AddScanMaterials(in ent, scanbase.Id);
            XScanPlanet planet = AddScanPlanet(in ent, scanbase.Id, atmosphere.Id, materials.Id);
            XScanStar star = AddScanStar(in ent, scanbase.Id);
            XScanRing[] rings = AddScanRings(in ent, scanbase.Id, out string[] ringnames);

            XScanClass scancls = new XScanClass
            {
                ScanData = ent.ScanData,
                System = sys,
                Body = body,
                Parents = parentset,
                Atmosphere = atmosphere,
                Materials = materials,
                Planet = planet,
                Star = star,
                Scan = ent.Scan,
                Header = ent.Header,
                RingA = rings[0],
                RingB = rings[1],
                RingC = rings[2],
                RingD = rings[3],
                RingAName = ringnames[0],
                RingBName = ringnames[1],
                RingCName = ringnames[2],
                RingDName = ringnames[3],
                ScanBase = scanbase,
                DbSystem = dbsys,
                DbBody = dbbody,
                DbParents = dbparentset,
            };

            scancls.Scan = AddScan(in scancls, dbbody, out scancls.DbScan);
            scancls.Header = AddScanHeader(in scancls, scancls.Scan.Id, scancls.DbScan.Id, out scancls.DbScanHeader);

            return scancls;
        }

        public static void ProcessScans(IEnumerable<XScanData> scanenum, bool excludeNoAxialTilt = false, bool excludeNoComposition = false, bool excludeNoLuminosity = false, bool excludeNoBodyId = false, bool excludeed303 = true, bool excludePartial = true)
        {
            int i = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            long lastelapsed = 0;
            long lastalloc = GC.GetTotalMemory(true);
            var enumerator = scanenum.GetEnumerator();
            bool enumend = false;

            HashSet<DateTime> gwtimestamps;

            Console.WriteLine("Loading gateway timestamps");

            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                gwtimestamps = new HashSet<DateTime>(ctx.Set<Models.EDDNJournalScan>().Select(s => s.GatewayTimestamp).AsEnumerable().Select(t => DateTime.SpecifyKind(t, DateTimeKind.Utc)));
            }

            Console.WriteLine("Processing scans");

            var scans = new List<XScanClass>();
            var db = new XDatabase();

            while (!enumend)
            {
                scans.Clear();

                using (var ctx = new Models.BodyDbContext())
                {
                    while (scans.Count < 1000)
                    {
                        if (!enumerator.MoveNext())
                        {
                            enumend = true;
                            break;
                        }

                        var scandata = enumerator.Current;

                        if (scandata.GatewayTimestamp == default ||
                            scandata.ScanTimestamp == default ||
                            (scandata.BodyID != -1 && scandata.ScanTimestamp < XModels.XDatabase.ed300) ||
                            (excludeed303 && scandata.ScanTimestamp >= XModels.XDatabase.ed303 && scandata.ScanTimestamp < XModels.XDatabase.ed304) ||
                            (excludeNoAxialTilt && !scandata.HasAxialTilt) ||
                            (excludeNoComposition && (scandata.IsPlanet && !scandata.HasComposition)) ||
                            (excludeNoLuminosity && (scandata.IsStar && scandata.LuminosityId == 0)) ||
                            (excludeNoBodyId && scandata.BodyID == -1) ||
                            (excludePartial && scandata.IsPartial))
                        {
                            if (scandata.IsPartial)
                            {
                                //Debugger.Break();
                            }

                            continue;
                        }

                        var scan = XScanClass.From(scandata);

                        if (false)
                        {
                            var system = ctx.GetOrAddSystem(scan.System, scan.CustomSystemName, true, true);
                            if (system.Id != 0)
                            {
                                var body = ctx.GetOrAddBody(scan.Body, system.Id, scan.ScanBase.GetHashCode(), scan.CustomBodyName, true, true, false, scan);
                            }
                        }

                        if (!gwtimestamps.Contains(scandata.GatewayTimestamp))
                        {
                            scans.Add(scan);
                        }
                    }
                }

                if (scans.Count != 0)
                {
                    using (var ctx = new Models.BodyDbContext())
                    {
                        using (db.SetContext(ctx))
                        {
                            foreach (var scan in scans)
                            {
                                db.Add(scan);
                                gwtimestamps.Add(scan.Header.GatewayTimestamp);
                                i++;
                            }
                        }
                    }

                    long elapsed = stopwatch.ElapsedMilliseconds;
                    long allocsize = GC.GetTotalMemory(true);
                    long allocdelta = allocsize - lastalloc;
                    System.Diagnostics.Trace.WriteLine($"==================================================");
                    System.Diagnostics.Trace.WriteLine($"{i} scans processed");
                    System.Diagnostics.Trace.WriteLine($"{allocsize} bytes allocated (avg {(allocsize / i)} bytes per scan)");
                    System.Diagnostics.Trace.WriteLine($"{allocdelta} bytes delta (avg {(allocdelta / scans.Count)} bytes per scan");
                    System.Diagnostics.Trace.WriteLine($"{stopwatch.Elapsed} elapsed ({(scans.Count * 1000.0 / (elapsed - lastelapsed))}/second)");
                    System.Diagnostics.Trace.WriteLine($"{db.Systems.Count} systems, {db.Bodies.Count} bodies");
                    System.Diagnostics.Trace.WriteLine($"{db.ScanBases.Count} distinct body bases, {db.Scans.Count} distinct scans");
                    System.Diagnostics.Trace.WriteLine($"{db.Planets.Count} planets, {db.Stars.Count} stars");
                    System.Diagnostics.Trace.WriteLine($"{db.Atmospheres.Count} atmosphereic planets, {db.Materials.Count} landable planets");
                    System.Diagnostics.Trace.WriteLine($"{db.NumRinged} ringed bodies, {db.Rings.Count} rings");
                    System.Diagnostics.Trace.WriteLine($"--------------------------------------------------");
                    lastelapsed = elapsed;
                    lastalloc = allocsize;
                }
            }

        }

        public static void CheckScanBaseHashes()
        {
            Console.WriteLine("Updating scan base hashes where necesary");

            int offset = 0;
            int numproc = 0;
            int numupdated = 0;
            int lastupdated = 0;
            int fetched = 0;
            int lastid = 0;
            int numrows = 0;

            using (var ctx = new Models.BodyDbContext())
            {
                numrows = ctx.Set<Models.BodyScan>().Count();
            }

            do
            {
                fetched = 0;

                using (var ctx = new Models.BodyDbContext())
                {
                    System.Data.Common.DbCommand scancmd = null;
                    System.Data.Common.DbCommand bodycmd = null;

                    try
                    {
                        var bodies = ctx.GetAllBodyScans(offset, 64000).ToList();

                        var conn = ctx.Database.Connection;
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        bodycmd = conn.CreateCommand();
                        bodycmd.CommandText = "UPDATE SystemBodies SET ScanBaseHash = @ScanBaseHash WHERE Id = @Id";
                        bodycmd.CommandType = System.Data.CommandType.Text;
                        var parambodyid = Models.DbExtensions.AddParameter(bodycmd, "@Id", System.Data.DbType.Int32);
                        var parambodyhash = Models.DbExtensions.AddParameter(bodycmd, "@ScanBaseHash", System.Data.DbType.Int32);

                        scancmd = conn.CreateCommand();
                        scancmd.CommandText = "UPDATE BodyScans SET ScanBaseHash = @ScanBaseHash WHERE Id = @Id";
                        scancmd.CommandType = System.Data.CommandType.Text;
                        var paramscanid = Models.DbExtensions.AddParameter(scancmd, "@Id", System.Data.DbType.Int32);
                        var paramscanhash = Models.DbExtensions.AddParameter(scancmd, "@ScanBaseHash", System.Data.DbType.Int32);

                        foreach (var body in bodies)
                        {
                            var scanbase = XScanBase.From(body);
                            var scanbasehash = scanbase.GetHashCode();

                            if (scanbasehash != body.ScanBaseHash)
                            {
                                parambodyid.Value = body.SystemBody.Id;
                                paramscanid.Value = body.Id;
                                parambodyhash.Value = scanbasehash;
                                paramscanhash.Value = scanbasehash;
                                bodycmd.ExecuteNonQuery();
                                scancmd.ExecuteNonQuery();

                                body.ScanBaseHash = scanbasehash;
                                body.SystemBody.ScanBaseHash = scanbasehash;

                                numupdated++;
                            }

                            numproc++;
                            fetched++;
                            offset = body.Id;
                            lastid = body.Id;

                            if ((numproc % 1000) == 0)
                            {
                                int updated = numupdated - lastupdated;

                                if (updated == 0)
                                {
                                    Console.Write(".");
                                }
                                else if (updated < 100)
                                {
                                    Console.Write(",");
                                }
                                else if (updated < 500)
                                {
                                    Console.Write("+");
                                }
                                else if (updated < 900)
                                {
                                    Console.Write("*");
                                }
                                else
                                {
                                    Console.Write("#");
                                }

                                lastupdated = numupdated;
                            }
                        }
                    }
                    finally
                    {
                        if (bodycmd != null)
                        {
                            bodycmd.Dispose();
                            bodycmd = null;
                        }

                        if (scancmd != null)
                        {
                            scancmd.Dispose();
                            scancmd = null;
                        }
                    }
                }

                if (fetched != 0)
                {
                    Console.WriteLine(" {0} / {1} updated (id = {2}) of {3} scans", numupdated, numproc, lastid, numrows);
                }
            }
            while (fetched != 0);

            Console.WriteLine("Done");
        }

        public static IEnumerable<XModels.XScanData> ReadEDDNBodies(JsonReader jr, string name)
        {
            Trace.WriteLine($"Processing EDDN bodies {name}");
            Console.WriteLine($"Processing EDDN bodies {name}");

            int i = 0;
            while (jr.Read())
            {
                if (jr.TokenType == JsonToken.StartObject)
                {
                    yield return XModels.XScanData.Read(jr);
                    //JObject jo = JObject.Load(jr);
                    //yield return Models.EDDNJournalScanStruct.FromJSON(jo);
                    i++;

                    if ((i % 200) == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Trace.WriteLine($"{i} scans processed from {name}");
            Console.WriteLine(" Done");
        }

        public static IEnumerable<XModels.XScanData> ReadEDDNBodiesBzip2(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                using (BZip2InputStream bzstream = new BZip2InputStream(stream))
                {
                    using (TextReader tr = new StreamReader(bzstream, Encoding.UTF8))
                    {
                        using (JsonReader jr = new JsonTextReader(tr))
                        {
                            jr.SupportMultipleContent = true;

                            foreach (var ent in ReadEDDNBodies(jr, Path.GetFileName(filename)))
                            {
                                yield return ent;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<XModels.XScanData> ReadEDDNBodiesJsonl(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                using (TextReader tr = new StreamReader(stream, Encoding.UTF8))
                {
                    using (JsonReader jr = new JsonTextReader(tr))
                    {
                        jr.SupportMultipleContent = true;

                        foreach (var ent in ReadEDDNBodies(jr, Path.GetFileName(filename)))
                        {
                            yield return ent;
                        }
                    }
                }
            }
        }

        public static IEnumerable<XModels.XScanData> ReadEDDNBodiesFromDir(string dir, params string[] patterns)
        {
            if (patterns == null || patterns.Length == 0)
            {
                patterns = new string[] { "Journal.Scan-20*.jsonl.bz2" };
            }

            foreach (string pattern in patterns)
            {
                foreach (string filename in Directory.EnumerateFiles(dir, pattern, SearchOption.TopDirectoryOnly))
                {
                    if (filename.EndsWith(".bz2"))
                    {
                        foreach (var ent in ReadEDDNBodiesBzip2(filename))
                        {
                            yield return ent;
                        }
                    }
                    else
                    {
                        foreach (var ent in ReadEDDNBodiesJsonl(filename))
                        {
                            yield return ent;
                        }
                    }
                }
            }
        }
    }
}
