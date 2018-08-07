using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public class XScanClass
    {
        public XScanData ScanData;
        public XScanHeader Header;
        public XScan Scan;
        public XScanStar Star;
        public XScanPlanet Planet;
        public XScanAtmosphere Atmosphere;
        public XScanMaterials Materials;
        public XParentSet Parents;
        public XScanBase ScanBase;
        public XScanRing RingA;
        public string RingAName;
        public XScanRing RingB;
        public string RingBName;
        public XScanRing RingC;
        public string RingCName;
        public XScanRing RingD;
        public string RingDName;
        public XBody Body;
        public string CustomBodyName;
        public XSystem System;
        public string CustomSystemName;
        public string JsonExtra;

        public Models.System DbSystem;
        public Models.SystemBody DbBody;
        public Models.ParentSet DbParents;
        public Models.BodyScan DbScan;
        public Models.EDDNJournalScan DbScanHeader;

        public static XScanClass From(in XScanData ent)
        {
            XScanRing[] rings = XScanRing.From(ent, out string[] ringnames);

            XScanClass scandata = new XScanClass
            {
                ScanData = ent,
                System = XSystem.From(in ent),
                CustomSystemName = ent.CustomSystemName,
                Body = XBody.From(in ent),
                CustomBodyName = ent.CustomBodyName,
                ScanBase = XScanBase.From(ent),
                Parents = ent.HasParents ? XParentSet.From(ent) : default,
                Atmosphere = ent.HasAtmosphere ? XScanAtmosphere.From(ent) : default,
                Materials = ent.HasMaterials ? XScanMaterials.From(ent) : default,
                Planet = ent.IsPlanet ? XScanPlanet.From(ent) : default,
                Star = ent.IsStar ? XScanStar.From(ent) : default,
                Scan = XScan.From(ent),
                Header = XScanHeader.From(ent),
                RingA = rings[0],
                RingB = rings[1],
                RingC = rings[2],
                RingD = rings[3],
                RingAName = ringnames[0],
                RingBName = ringnames[1],
                RingCName = ringnames[2],
                RingDName = ringnames[3],
                JsonExtra = ent.JsonExtra,
            };

            return scandata;
        }
    }
}
