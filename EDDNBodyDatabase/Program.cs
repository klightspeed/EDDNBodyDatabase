using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDNBodyDatabase.XModels;

namespace EDDNBodyDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            string eddnbasedir = args[0];

            BodyDatabase.Init();

            if (BodyDatabase.SystemCount() < 154700)
            {
                BodyDatabase.LoadNamedSystems("NamedSystems.json");
            }

            BodyDatabase.LoadEDSMSystemsLocal("systemsWithCoordinates.jsonl");

            XDatabase.CheckScanBaseHashes();

            BodyDatabase.LoadNamedBodiesLocal("NamedBodies.tsv");

            // Process 3.x entries, excluding 3.0.3
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, 
                    "Journal.Scan-2018-02-2?.jsonl.bz2",
                    "Journal.Scan-2018-03-*.jsonl.bz2",
                    "Journal.Scan-2018-04-*.jsonl.bz2",
                    "Journal.Scan-2018-05-*.jsonl.bz2",
                    "Journal.Scan-2018-06-*.jsonl.bz2",
                    "Journal.Scan-2018-07-*.jsonl.bz2",
                    "Journal.Scan-2018-08-*.jsonl.bz2",
                    "Journal.Scan-2018-09-*.jsonl.bz2",
                    "Journal.Scan-2018-10-*.jsonl.bz2",
                    "Journal.Scan-2018-11-*.jsonl.bz2",
                    "Journal.Scan-2018-12-*.jsonl.bz2"
                ), true, true, true, true, true);

            // Process entries without BodyID / SystemAddress
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, "Journal.Scan-2018-*.jsonl.bz2"), true, true, true, false, true);
            // Process entries without BodyID / SystemAddress / Luminosity / Composition
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, "Journal.Scan-*.jsonl.bz2"), true, false, false, false, true);
            // Process entries without BodyID / SystemAddress / Luminosity / Composition / AxialTilt
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, "Journal.Scan-*.jsonl.bz2"), false, false, false, false, true);
            // Process 3.0.3 entries
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, "Journal.Scan-2018-*.jsonl.bz2"), true, true, true, true, false);
            // Process everything else
            XDatabase.ProcessScans(XDatabase.ReadEDDNBodiesFromDir(eddnbasedir, "Journal.Scan-*.jsonl.bz2"), false, false, false, false, false);
        }
    }
}
