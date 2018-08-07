using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    // Node: 144 bytes per entry
    public struct XParentSet : IEquatable<XParentSet>, IWithId, IStructEquatable<XParentSet>, IWithNextId
    {
        public int Id { get; set; }
        public int NextId { get; set; }
        public int DbId { get; set; }
        public short Parent0BodyID;
        public short Parent1BodyID;
        public short Parent2BodyID;
        public short Parent3BodyID;
        public short Parent4BodyID;
        public short Parent5BodyID;
        public short Parent6BodyID;
        public short Parent7BodyID;
        public byte Parent0TypeId;
        public byte Parent1TypeId;
        public byte Parent2TypeId;
        public byte Parent3TypeId;
        public byte Parent4TypeId;
        public byte Parent5TypeId;
        public byte Parent6TypeId;
        public byte Parent7TypeId;

        public string Parent0Type { get { return BodyDatabase.BodyType.GetName(Parent0TypeId); } set { Parent0TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent1Type { get { return BodyDatabase.BodyType.GetName(Parent1TypeId); } set { Parent1TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent2Type { get { return BodyDatabase.BodyType.GetName(Parent2TypeId); } set { Parent2TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent3Type { get { return BodyDatabase.BodyType.GetName(Parent3TypeId); } set { Parent3TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent4Type { get { return BodyDatabase.BodyType.GetName(Parent4TypeId); } set { Parent4TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent5Type { get { return BodyDatabase.BodyType.GetName(Parent5TypeId); } set { Parent5TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent6Type { get { return BodyDatabase.BodyType.GetName(Parent6TypeId); } set { Parent6TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public string Parent7Type { get { return BodyDatabase.BodyType.GetName(Parent7TypeId); } set { Parent7TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is XParentSet other &&
                   Equals(in this, in other);
        }

        public bool Equals(XParentSet other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in XParentSet other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in XParentSet x, in XParentSet y)
        {
            return x.Parent0Type == y.Parent0Type &&
                   x.Parent0BodyID == y.Parent0BodyID &&
                   x.Parent1Type == y.Parent1Type &&
                   x.Parent1BodyID == y.Parent1BodyID &&
                   x.Parent2Type == y.Parent2Type &&
                   x.Parent2BodyID == y.Parent2BodyID &&
                   x.Parent3Type == y.Parent3Type &&
                   x.Parent3BodyID == y.Parent3BodyID &&
                   x.Parent4Type == y.Parent4Type &&
                   x.Parent4BodyID == y.Parent4BodyID &&
                   x.Parent5Type == y.Parent5Type &&
                   x.Parent5BodyID == y.Parent5BodyID &&
                   x.Parent6Type == y.Parent6Type &&
                   x.Parent6BodyID == y.Parent6BodyID &&
                   x.Parent7Type == y.Parent7Type &&
                   x.Parent7BodyID == y.Parent7BodyID;
        }

        public static int GetHashCode(in XParentSet x)
        {
            return unchecked(
                x.Parent1BodyID * 0x01000000 +
                x.Parent2BodyID * 0x00100000 +
                x.Parent3BodyID * 0x00010000 +
                x.Parent4BodyID * 0x00001000 +
                x.Parent5BodyID * 0x00000100 +
                x.Parent6BodyID * 0x00000010 +
                x.Parent7BodyID * 0x00000001);
        }

        public static XParentSet From(in Models.ParentSetStruct scan)
        {
            return new XParentSet
            {
                Parent0BodyID = scan.Parent0.BodyID,
                Parent0TypeId = scan.Parent0.TypeId,
                Parent1BodyID = scan.Parent1.BodyID,
                Parent1TypeId = scan.Parent1.TypeId,
                Parent2BodyID = scan.Parent2.BodyID,
                Parent2TypeId = scan.Parent2.TypeId,
                Parent3BodyID = scan.Parent3.BodyID,
                Parent3TypeId = scan.Parent3.TypeId,
                Parent4BodyID = scan.Parent4.BodyID,
                Parent4TypeId = scan.Parent4.TypeId,
                Parent5BodyID = scan.Parent5.BodyID,
                Parent5TypeId = scan.Parent5.TypeId,
                Parent6BodyID = scan.Parent6.BodyID,
                Parent6TypeId = scan.Parent6.TypeId,
                Parent7BodyID = scan.Parent7.BodyID,
                Parent7TypeId = scan.Parent7.TypeId,
            };
        }

        public static XParentSet From(in XScanData scan)
        {
            return new XParentSet
            {
                Parent0BodyID = scan.Parent0.BodyID,
                Parent0TypeId = scan.Parent0.TypeId,
                Parent1BodyID = scan.Parent1.BodyID,
                Parent1TypeId = scan.Parent1.TypeId,
                Parent2BodyID = scan.Parent2.BodyID,
                Parent2TypeId = scan.Parent2.TypeId,
                Parent3BodyID = scan.Parent3.BodyID,
                Parent3TypeId = scan.Parent3.TypeId,
                Parent4BodyID = scan.Parent4.BodyID,
                Parent4TypeId = scan.Parent4.TypeId,
                Parent5BodyID = scan.Parent5.BodyID,
                Parent5TypeId = scan.Parent5.TypeId,
                Parent6BodyID = scan.Parent6.BodyID,
                Parent6TypeId = scan.Parent6.TypeId,
                Parent7BodyID = scan.Parent7.BodyID,
                Parent7TypeId = scan.Parent7.TypeId,
            };
        }
    }
}
