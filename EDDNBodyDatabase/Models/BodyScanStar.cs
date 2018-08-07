using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanStar : BodyScan
    {
        public float AbsoluteMagnitude { get; set; }
        public float StellarMass { get; set; }
        public short Age_MY { get; set; }
        public byte StarTypeId { get; set; }
        public string StarType { get { return BodyDatabase.StarType.GetName(StarTypeId); } set { StarTypeId = (byte)BodyDatabase.StarType.GetId(value); } }
        public byte? LuminosityId { get; set; }
        public string Luminosity { get { return BodyDatabase.Luminosity.GetName(LuminosityId); } set { LuminosityId = BodyDatabase.Luminosity.GetId(value); } }

        public StarType StarTypeRef { get; set; }
        public Luminosity LuminosityRef { get; set; }

        public override bool Equals(XModels.XScanClass scan, bool ignoremats = false)
        {
            XModels.XScanStar sscan = scan.Star;

            return base.Equals(scan) &&
                   this.AbsoluteMagnitude == sscan.AbsoluteMagnitude &&
                   this.StellarMass == sscan.StellarMass &&
                   this.Age_MY == sscan.Age_MY &&
                   this.StarTypeId == sscan.StarTypeId &&
                   (sscan.LuminosityId == 0 || this.LuminosityId == null || this.LuminosityId == sscan.LuminosityId);
        }

        public override List<CompareResult> GetDifferingProps(XScanClass scandata, List<CompareResult> diffs)
        {
            XModels.XScanStar sscan = scandata.Star;
            base.GetDifferingProps(scandata, diffs);
            CompareResult.AddIfUnequal(diffs, this, "AbsoluteMagnitude", this.AbsoluteMagnitude, sscan.AbsoluteMagnitude);
            CompareResult.AddIfUnequal(diffs, this, "StellarMass", this.StellarMass, sscan.StellarMass);
            CompareResult.AddIfUnequal(diffs, this, "Age_MY", this.Age_MY, sscan.Age_MY);
            CompareResult.AddIfUnequal(diffs, this, "StarTypeId", this.StarTypeId, sscan.StarTypeId);
            CompareResult.AddIfUnequal(diffs, this, "LuminosityId", this.LuminosityId, sscan.LuminosityId == 0 ? null : (byte?)sscan.LuminosityId);

            return diffs;
        }

        public override bool Equals(XModels.XScanClass scan, float epsilon)
        {
            XModels.XScanStar sscan = scan.Star;

            return base.Equals(scan) &&
                   this.AbsoluteMagnitude == sscan.AbsoluteMagnitude &&
                   this.StellarMass == sscan.StellarMass &&
                   this.Age_MY == sscan.Age_MY &&
                   this.StarTypeId == sscan.StarTypeId &&
                   (sscan.LuminosityId == 0 || this.LuminosityId == null || this.LuminosityId == sscan.LuminosityId);
        }

        public BodyScanStar()
        {
        }

        public BodyScanStar(XModels.XScanClass scan, SystemBody sysbody, int sysbodyid, int id = 0) : base(scan, sysbody, sysbodyid, id)
        {
            XScanStar sscan = scan.Star;
            AbsoluteMagnitude = sscan.AbsoluteMagnitude;
            StellarMass = sscan.StellarMass;
            Age_MY = sscan.Age_MY;
            StarTypeId = sscan.StarTypeId;
            LuminosityId = sscan.LuminosityId == 0 ? (byte?)null : sscan.LuminosityId;
        }
    }
}
