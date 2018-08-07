using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanRing
    {
        public int ScanId { get; set; }
        public byte RingNum { get; set; }
        public byte ClassId { get; set; }
        public float InnerRad { get; set; }
        public float OuterRad { get; set; }
        public float MassMT { get; set; }
        public bool IsBelt { get; set; }
        public string Class { get { return BodyDatabase.RingClass.GetName(ClassId); } set { ClassId = (byte)BodyDatabase.RingClass.GetId(value); } }
        public string Name {
            get { return CustomName?.Name; }
            set { CustomName = value == null ? null : new BodyScanRingCustomName { ScanId = this.ScanId, RingNum = this.RingNum, Name = value }; }
        }

        public virtual RingClass ClassRef { get; set; }
        public virtual BodyScan ScanRef { get; set; }
        public BodyScanRingCustomName CustomName { get; set; }

        public bool Equals(XModels.XScanRing scan)
        {
            return this.ClassId == scan.ClassId &&
                   this.InnerRad == scan.InnerRad &&
                   this.OuterRad == scan.OuterRad &&
                   this.MassMT == scan.MassMT &&
                   this.IsBelt == scan.IsBelt;
        }

        public void GetDifferingProps(XModels.XScanRing scan, List<BodyScan.CompareResult> diffs, BodyScan bscan)
        {
            BodyScan.CompareResult.AddIfUnequal(diffs, bscan, $"Rings[{RingNum}].ClassId", this.ClassId, scan.ClassId);
            BodyScan.CompareResult.AddIfUnequal(diffs, bscan, $"Rings[{RingNum}].InnerRad", this.InnerRad, scan.InnerRad);
            BodyScan.CompareResult.AddIfUnequal(diffs, bscan, $"Rings[{RingNum}].OuterRad", this.OuterRad, scan.OuterRad);
            BodyScan.CompareResult.AddIfUnequal(diffs, bscan, $"Rings[{RingNum}].MassMT", this.MassMT, scan.MassMT);
            BodyScan.CompareResult.AddIfUnequal(diffs, bscan, $"Rings[{RingNum}].IsBelt", this.IsBelt, scan.IsBelt);
        }

        public BodyScanRing()
        {
        }

        public BodyScanRing(XModels.XScanRing scan, byte ringnum, int scanid = 0, string name = null)
        {
            ScanId = scanid;
            RingNum = ringnum;
            ClassId = scan.ClassId;
            CustomName = new BodyScanRingCustomName
            {
                ScanId = scanid,
                RingNum = ringnum,
                Name = name
            };
            InnerRad = scan.InnerRad;
            OuterRad = scan.OuterRad;
            MassMT = scan.MassMT;
            IsBelt = scan.IsBelt;
        }

        public static List<BodyScanRing> GetRings(XModels.XScanClass scan)
        {
            List<BodyScanRing> rings = new[]
            {
                new BodyScanRing(scan.RingA, 1, name: scan.RingAName),
                new BodyScanRing(scan.RingB, 2, name: scan.RingBName),
                new BodyScanRing(scan.RingC, 3, name: scan.RingCName),
                new BodyScanRing(scan.RingD, 4, name: scan.RingDName),
            }.TakeWhile(r => r.ClassId != 0)
             .ToList();
            return rings;
        }
    }
}
