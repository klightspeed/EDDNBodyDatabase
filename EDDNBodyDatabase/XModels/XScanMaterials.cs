using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 64 bytes per entry
    public struct XScanMaterials : IEquatable<XScanMaterials>, IWithParentId, IStructEquatable<XScanMaterials>, IWithNextId
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int NextId { get; set; }
        public float MaterialCarbon;
        public float MaterialIron;
        public float MaterialNickel;
        public float MaterialPhosphorus;
        public float MaterialSulphur;
        public float Material1Amt;
        public float Material2Amt;
        public float Material3Amt;
        public float Material4Amt;
        public float Material5Amt;
        public float Material6Amt;
        public byte Material1Id;
        public byte Material2Id;
        public byte Material3Id;
        public byte Material4Id;
        public byte Material5Id;
        public byte Material6Id;

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XScanMaterials other &&
                   Equals(in this, in other);
        }

        public override int GetHashCode()
        {
            return MaterialCarbon.GetHashCode() ^
                   MaterialIron.GetHashCode() ^
                   MaterialNickel.GetHashCode() ^
                   MaterialSulphur.GetHashCode() ^
                   MaterialPhosphorus.GetHashCode() ^
                   Material1Amt.GetHashCode() ^
                   Material2Amt.GetHashCode() ^
                   Material3Amt.GetHashCode() ^
                   Material4Amt.GetHashCode() ^
                   Material5Amt.GetHashCode() ^
                   Material6Amt.GetHashCode();
        }

        public bool Equals(XScanMaterials other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XScanMaterials other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XScanMaterials x, in XScanMaterials y)
        {
            return x.ParentId == y.ParentId &&
                   x.MaterialCarbon == y.MaterialCarbon &&
                   x.MaterialIron == y.MaterialIron &&
                   x.MaterialNickel == y.MaterialNickel &&
                   x.MaterialPhosphorus == y.MaterialPhosphorus &&
                   x.MaterialSulphur == y.MaterialSulphur &&
                   x.Material1Id == y.Material1Id &&
                   x.Material1Amt == y.Material1Amt &&
                   x.Material2Id == y.Material2Id &&
                   x.Material2Amt == y.Material2Amt &&
                   x.Material3Id == y.Material3Id &&
                   x.Material3Amt == y.Material3Amt &&
                   x.Material4Id == y.Material4Id &&
                   x.Material4Amt == y.Material4Amt &&
                   x.Material5Id == y.Material5Id &&
                   x.Material5Amt == y.Material5Amt &&
                   x.Material6Id == y.Material6Id &&
                   x.Material6Amt == y.Material6Amt;
        }

        public static XScanMaterials From(in XScanData scan)
        {
            return new XScanMaterials
            {
                MaterialCarbon = scan.MaterialCarbon,
                MaterialIron = scan.MaterialIron,
                MaterialNickel = scan.MaterialNickel,
                MaterialPhosphorus = scan.MaterialPhosphorus,
                MaterialSulphur = scan.MaterialSulphur,
                Material1Amt = scan.Material1Amt,
                Material2Amt = scan.Material2Amt,
                Material3Amt = scan.Material3Amt,
                Material4Amt = scan.Material4Amt,
                Material5Amt = scan.Material5Amt,
                Material6Amt = scan.Material6Amt,
                Material1Id = scan.Material1Id,
                Material2Id = scan.Material2Id,
                Material3Id = scan.Material3Id,
                Material4Id = scan.Material4Id,
                Material5Id = scan.Material5Id,
                Material6Id = scan.Material6Id,
            };
        }
    }
}
