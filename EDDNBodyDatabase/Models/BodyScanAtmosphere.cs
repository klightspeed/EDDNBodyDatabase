using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanAtmosphere
    {
        public int Id { get; set; }
        public float SurfacePressure { get; set; }
        public float AtmosphereComponent1Amt { get; set; }
        public float AtmosphereComponent2Amt { get; set; }
        public float AtmosphereComponent3Amt { get; set; }
        public byte? AtmosphereComponent1Id { get; set; }
        public byte? AtmosphereComponent2Id { get; set; }
        public byte? AtmosphereComponent3Id { get; set; }
        public byte AtmosphereId { get; set; }
        public byte? AtmosphereTypeId { get; set; }
        public bool AtmosphereHot { get; set; }
        public bool AtmosphereThin { get; set; }
        public bool AtmosphereThick { get; set; }
        public string AtmosphereComponent1 { get { return BodyDatabase.AtmosphereComponent.GetName(AtmosphereComponent1Id); } set { AtmosphereComponent1Id = (byte?)BodyDatabase.AtmosphereComponent.GetId(value); } }
        public string AtmosphereComponent2 { get { return BodyDatabase.AtmosphereComponent.GetName(AtmosphereComponent2Id); } set { AtmosphereComponent2Id = (byte?)BodyDatabase.AtmosphereComponent.GetId(value); } }
        public string AtmosphereComponent3 { get { return BodyDatabase.AtmosphereComponent.GetName(AtmosphereComponent3Id); } set { AtmosphereComponent3Id = (byte?)BodyDatabase.AtmosphereComponent.GetId(value); } }
        public string Atmosphere { get { return BodyDatabase.Atmosphere.GetName(AtmosphereId); } set { AtmosphereId = (byte)BodyDatabase.Atmosphere.GetId(value); } }
        public string AtmosphereType { get { return BodyDatabase.AtmosphereType.GetName(AtmosphereTypeId); } set { AtmosphereTypeId = (byte?)BodyDatabase.AtmosphereType.GetId(value); } }

        public virtual AtmosphereComponent AtmosphereComponent1Ref { get; set; }
        public virtual AtmosphereComponent AtmosphereComponent2Ref { get; set; }
        public virtual AtmosphereComponent AtmosphereComponent3Ref { get; set; }
        public virtual Atmosphere AtmosphereRef { get; set; }
        public virtual AtmosphereType AtmosphereTypeRef { get; set; }

        public bool Equals(XScanAtmosphere scan)
        {
            return this.SurfacePressure == scan.SurfacePressure &&
                   this.Atmosphere == scan.Atmosphere &&
                   this.AtmosphereHot == scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereHot) &&
                   this.AtmosphereThin == scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThin) &&
                   this.AtmosphereThick == scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThick) &&
                   (this.AtmosphereTypeId == null || scan.AtmosphereType == null || this.AtmosphereType == scan.AtmosphereType) &&
                   (this.AtmosphereComponent1Id == null || scan.Component1Id == 0 || (
                       this.AtmosphereComponent1 == scan.Component1Name &&
                       this.AtmosphereComponent1Amt == scan.Component1Amt &&
                       this.AtmosphereComponent2 == scan.Component2Name &&
                       this.AtmosphereComponent2Amt == scan.Component2Amt &&
                       this.AtmosphereComponent3 == scan.Component3Name &&
                       this.AtmosphereComponent3Amt == scan.Component3Amt
                   ));
        }

        public void GetDifferingProps(XModels.XScanAtmosphere scan, List<BodyScan.CompareResult> diffs, BodyScan bodyscan)
        {
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "SurfacePressure", this.SurfacePressure, scan.SurfacePressure);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Atmosphere", this.Atmosphere, scan.Atmosphere);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereHot", this.AtmosphereHot, scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereHot));
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereThin", this.AtmosphereThin, scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThin));
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereThick", this.AtmosphereThick, scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThick));
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereTypeId", this.AtmosphereTypeId, scan.AtmosphereTypeId == 0 ? null : (byte?)scan.AtmosphereTypeId);
            if (this.AtmosphereComponent1Id != null && scan.Component1Id != 0)
            {
                BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereComponent1", false, this.AtmosphereComponent1Id, scan.Component1Id == 0 ? null : (byte?)scan.Component1Id);
                BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereComponent2", false, this.AtmosphereComponent2Id, scan.Component2Id == 0 ? null : (byte?)scan.Component2Id);
                BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "AtmosphereComponent3", false, this.AtmosphereComponent3Id, scan.Component3Id == 0 ? null : (byte?)scan.Component3Id);
            }
        }

        public BodyScanAtmosphere()
        {
        }

        public BodyScanAtmosphere(XScanAtmosphere scan, int id = 0)
        {
            Id = id;
            SurfacePressure = scan.SurfacePressure;
            AtmosphereId = scan.AtmosphereId;
            AtmosphereTypeId = scan.AtmosphereTypeId == 0 ? (byte?)null : scan.AtmosphereTypeId;
            AtmosphereHot = scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereHot);
            AtmosphereThin = scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThin);
            AtmosphereThick = scan.Flags.HasFlag(XScanAtmosphere.ScanFlags.AtmosphereThick);
            AtmosphereComponent1Id = scan.Component1Id == 0 ? (byte?)null : scan.Component1Id;
            AtmosphereComponent1Amt = scan.Component1Amt;
            AtmosphereComponent2Id = scan.Component2Id == 0 ? (byte?)null : scan.Component2Id;
            AtmosphereComponent2Amt = scan.Component2Amt;
            AtmosphereComponent3Id = scan.Component3Id == 0 ? (byte?)null : scan.Component3Id;
            AtmosphereComponent3Amt = scan.Component3Amt;
        }
    }
}
