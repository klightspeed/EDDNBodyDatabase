using EDDNBodyDatabase.XModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class ParentSet
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public short Parent1BodyId { get; set; }
        public short Parent2BodyId { get; set; }
        public short Parent3BodyId { get; set; }
        public short Parent4BodyId { get; set; }
        public short Parent5BodyId { get; set; }
        public short Parent6BodyId { get; set; }
        public short Parent7BodyId { get; set; }
        public byte Parent0TypeId { get; set; }
        public byte? Parent1TypeId { get; set; }
        public byte? Parent2TypeId { get; set; }
        public byte? Parent3TypeId { get; set; }
        public byte? Parent4TypeId { get; set; }
        public byte? Parent5TypeId { get; set; }
        public byte? Parent6TypeId { get; set; }
        public byte? Parent7TypeId { get; set; }
        public string Parent0Type { get { return BodyDatabase.BodyType.GetName(Parent0TypeId); } set { Parent0TypeId = (byte)BodyDatabase.BodyType.GetId(value); } }
        public string Parent1Type { get { return BodyDatabase.BodyType.GetName(Parent1TypeId); } set { Parent1TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent2Type { get { return BodyDatabase.BodyType.GetName(Parent2TypeId); } set { Parent2TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent3Type { get { return BodyDatabase.BodyType.GetName(Parent3TypeId); } set { Parent3TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent4Type { get { return BodyDatabase.BodyType.GetName(Parent4TypeId); } set { Parent4TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent5Type { get { return BodyDatabase.BodyType.GetName(Parent5TypeId); } set { Parent5TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent6Type { get { return BodyDatabase.BodyType.GetName(Parent6TypeId); } set { Parent6TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }
        public string Parent7Type { get { return BodyDatabase.BodyType.GetName(Parent7TypeId); } set { Parent7TypeId = (byte?)BodyDatabase.BodyType.GetId(value); } }

        public virtual ParentSet ParentRef { get; set; }
        public virtual BodyType Parent0TypeRef { get; set; }
        public virtual BodyType Parent1TypeRef { get; set; }
        public virtual BodyType Parent2TypeRef { get; set; }
        public virtual BodyType Parent3TypeRef { get; set; }
        public virtual BodyType Parent4TypeRef { get; set; }
        public virtual BodyType Parent5TypeRef { get; set; }
        public virtual BodyType Parent6TypeRef { get; set; }
        public virtual BodyType Parent7TypeRef { get; set; }

        public bool Equals(XParentSet scan)
        {
            return scan.Parent0BodyID == 0 &&
                   this.Parent0Type == scan.Parent0Type &&
                   this.Parent1BodyId == scan.Parent1BodyID &&
                   this.Parent1Type == scan.Parent1Type &&
                   this.Parent2BodyId == scan.Parent2BodyID &&
                   this.Parent2Type == scan.Parent2Type &&
                   this.Parent3BodyId == scan.Parent3BodyID &&
                   this.Parent3Type == scan.Parent3Type &&
                   this.Parent4BodyId == scan.Parent4BodyID &&
                   this.Parent4Type == scan.Parent4Type &&
                   this.Parent5BodyId == scan.Parent5BodyID &&
                   this.Parent5Type == scan.Parent5Type &&
                   this.Parent6BodyId == scan.Parent6BodyID &&
                   this.Parent6Type == scan.Parent6Type &&
                   this.Parent7BodyId == scan.Parent7BodyID &&
                   this.Parent7Type == scan.Parent7Type;
        }

        public ParentSet()
        {
        }

        public ParentSet(XParentSet scan, int id = 0, int? parentid = null)
        {
            Id = id;
            ParentId = parentid;
            Parent0Type = scan.Parent0Type;
            Parent1BodyId = scan.Parent1BodyID;
            Parent1Type = scan.Parent1Type;
            Parent2BodyId = scan.Parent2BodyID;
            Parent2Type = scan.Parent2Type;
            Parent3BodyId = scan.Parent3BodyID;
            Parent3Type = scan.Parent3Type;
            Parent4BodyId = scan.Parent4BodyID;
            Parent4Type = scan.Parent4Type;
            Parent5BodyId = scan.Parent5BodyID;
            Parent5Type = scan.Parent5Type;
            Parent6BodyId = scan.Parent6BodyID;
            Parent6Type = scan.Parent6Type;
            Parent7BodyId = scan.Parent7BodyID;
            Parent7Type = scan.Parent7Type;
        }
    }
}
