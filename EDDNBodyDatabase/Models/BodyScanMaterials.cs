using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class BodyScanMaterials
    {
        public int Id { get; set; }
        public float MaterialCarbon { get; set; }
        public float MaterialIron { get; set; }
        public float MaterialNickel { get; set; }
        public float MaterialPhosphorus { get; set; }
        public float MaterialSulphur { get; set; }
        public float Material1Amt { get; set; }
        public float Material2Amt { get; set; }
        public float Material3Amt { get; set; }
        public float Material4Amt { get; set; }
        public float Material5Amt { get; set; }
        public float Material6Amt { get; set; }
        public byte Material1Id { get; set; }
        public byte Material2Id { get; set; }
        public byte Material3Id { get; set; } 
        public byte Material4Id { get; set; }
        public byte Material5Id { get; set; }
        public byte Material6Id { get; set; }
        public string Material1 { get { return BodyDatabase.MaterialName.GetName(Material1Id); } set { Material1Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
        public string Material2 { get { return BodyDatabase.MaterialName.GetName(Material2Id); } set { Material2Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
        public string Material3 { get { return BodyDatabase.MaterialName.GetName(Material3Id); } set { Material3Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
        public string Material4 { get { return BodyDatabase.MaterialName.GetName(Material4Id); } set { Material4Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
        public string Material5 { get { return BodyDatabase.MaterialName.GetName(Material5Id); } set { Material5Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
        public string Material6 { get { return BodyDatabase.MaterialName.GetName(Material6Id); } set { Material6Id = (byte)BodyDatabase.MaterialName.GetId(value); } }

        public virtual MaterialName Material1Ref { get; set; }
        public virtual MaterialName Material2Ref { get; set; }
        public virtual MaterialName Material3Ref { get; set; }
        public virtual MaterialName Material4Ref { get; set; }
        public virtual MaterialName Material5Ref { get; set; }
        public virtual MaterialName Material6Ref { get; set; }

        public bool Equals(BodyScanMaterials scan)
        {
            return this.MaterialCarbon == scan.MaterialCarbon &&
                   this.MaterialIron == scan.MaterialIron &&
                   this.MaterialNickel == scan.MaterialNickel &&
                   this.MaterialPhosphorus == scan.MaterialPhosphorus &&
                   this.MaterialSulphur == scan.MaterialSulphur &&
                   this.Material1Id == scan.Material1Id &&
                   this.Material1Amt == scan.Material1Amt &&
                   this.Material2Id == scan.Material2Id &&
                   this.Material2Amt == scan.Material2Amt &&
                   this.Material3Id == scan.Material3Id &&
                   this.Material3Amt == scan.Material3Amt &&
                   this.Material4Id == scan.Material4Id &&
                   this.Material4Amt == scan.Material4Amt &&
                   this.Material5Id == scan.Material5Id &&
                   this.Material5Amt == scan.Material5Amt &&
                   this.Material6Id == scan.Material6Id &&
                   this.Material6Amt == scan.Material6Amt;
        }

        public bool Equals(XScanMaterials scan)
        {
            return this.MaterialCarbon == scan.MaterialCarbon &&
                   this.MaterialIron == scan.MaterialIron &&
                   this.MaterialNickel == scan.MaterialNickel &&
                   this.MaterialPhosphorus == scan.MaterialPhosphorus &&
                   this.MaterialSulphur == scan.MaterialSulphur &&
                   this.Material1Id == scan.Material1Id &&
                   this.Material1Amt == scan.Material1Amt &&
                   this.Material2Id == scan.Material2Id &&
                   this.Material2Amt == scan.Material2Amt &&
                   this.Material3Id == scan.Material3Id &&
                   this.Material3Amt == scan.Material3Amt &&
                   this.Material4Id == scan.Material4Id &&
                   this.Material4Amt == scan.Material4Amt &&
                   this.Material5Id == scan.Material5Id &&
                   this.Material5Amt == scan.Material5Amt &&
                   this.Material6Id == scan.Material6Id &&
                   this.Material6Amt == scan.Material6Amt;
        }

        public void GetDifferingProps(XScanMaterials scan, List<BodyScan.CompareResult> diffs, BodyScan bodyscan)
        {
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "MaterialCarbon", this.MaterialCarbon, scan.MaterialCarbon);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "MaterialIron", this.MaterialIron, scan.MaterialIron);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "MaterialNickel", this.MaterialNickel, scan.MaterialNickel);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "MaterialPhosphorus", this.MaterialPhosphorus, scan.MaterialPhosphorus);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "MaterialSulphur", this.MaterialSulphur, scan.MaterialSulphur);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material1Id", this.Material1Id, scan.Material1Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material1Amt", this.Material1Amt, scan.Material1Amt);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material2Id", this.Material2Id, scan.Material2Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material2Amt", this.Material2Amt, scan.Material2Amt);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material3Id", this.Material3Id, scan.Material3Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material3Amt", this.Material3Amt, scan.Material3Amt);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material4Id", this.Material4Id, scan.Material4Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material4Amt", this.Material4Amt, scan.Material4Amt);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material5Id", this.Material5Id, scan.Material5Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material5Amt", this.Material5Amt, scan.Material5Amt);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material6Id", this.Material6Id, scan.Material6Id);
            BodyScan.CompareResult.AddIfUnequal(diffs, bodyscan, "Material6Amt", this.Material6Amt, scan.Material6Amt);
        }

        public bool Equals(XScanMaterials scan, float epsilon)
        {
            return Math.Abs(this.MaterialCarbon - scan.MaterialCarbon) < epsilon &&
                   Math.Abs(this.MaterialIron - scan.MaterialIron) < epsilon &&
                   Math.Abs(this.MaterialNickel - scan.MaterialNickel) < epsilon &&
                   Math.Abs(this.MaterialPhosphorus - scan.MaterialPhosphorus) < epsilon &&
                   Math.Abs(this.MaterialSulphur - scan.MaterialSulphur) < epsilon &&
                   this.Material1Id == scan.Material1Id &&
                   Math.Abs(this.Material1Amt - scan.Material1Amt) < epsilon &&
                   this.Material2Id == scan.Material2Id &&
                   Math.Abs(this.Material2Amt - scan.Material2Amt) < epsilon &&
                   this.Material3Id == scan.Material3Id &&
                   Math.Abs(this.Material3Amt - scan.Material3Amt) < epsilon &&
                   this.Material4Id == scan.Material4Id &&
                   Math.Abs(this.Material4Amt - scan.Material4Amt) < epsilon &&
                   this.Material5Id == scan.Material5Id &&
                   Math.Abs(this.Material5Amt - scan.Material5Amt) < epsilon &&
                   this.Material6Id == scan.Material6Id &&
                   Math.Abs(this.Material6Amt - scan.Material6Amt) < epsilon;
        }

        public BodyScanMaterials()
        {
        }

        public BodyScanMaterials(XScanMaterials scan, int id = 0)
        {
            Id = id;
            MaterialCarbon = scan.MaterialCarbon;
            MaterialIron = scan.MaterialIron;
            MaterialNickel = scan.MaterialNickel;
            MaterialPhosphorus = scan.MaterialPhosphorus;
            MaterialSulphur = scan.MaterialSulphur;
            Material1Id = scan.Material1Id;
            Material1Amt = scan.Material1Amt;
            Material2Id = scan.Material2Id;
            Material2Amt = scan.Material2Amt;
            Material3Id = scan.Material3Id;
            Material3Amt = scan.Material3Amt;
            Material4Id = scan.Material4Id;
            Material4Amt = scan.Material4Amt;
            Material5Id = scan.Material5Id;
            Material5Amt = scan.Material5Amt;
            Material6Id = scan.Material6Id;
            Material6Amt = scan.Material6Amt;
        }
    }
}
