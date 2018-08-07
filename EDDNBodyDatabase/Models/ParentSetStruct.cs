using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace EDDNBodyDatabase.Models
{
    public struct ParentSetStruct
    {
        public struct ParentEntry
        {
            public byte TypeId;
            public string Type { get { return BodyDatabase.BodyType.GetName(TypeId); } set { TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
            public short BodyID;
        }

        public ParentEntry Parent0;
        public ParentEntry Parent1;
        public ParentEntry Parent2;
        public ParentEntry Parent3;
        public ParentEntry Parent4;
        public ParentEntry Parent5;
        public ParentEntry Parent6;
        public ParentEntry Parent7;

        public bool Equals(ParentSetStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in ParentSetStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in ParentSetStruct x, in ParentSetStruct y)
        {
            return x.Parent0.Type == y.Parent0.Type &&
                   x.Parent0.BodyID == y.Parent0.BodyID &&
                   x.Parent1.Type == y.Parent1.Type &&
                   x.Parent1.BodyID == y.Parent1.BodyID &&
                   x.Parent2.Type == y.Parent2.Type &&
                   x.Parent2.BodyID == y.Parent2.BodyID &&
                   x.Parent3.Type == y.Parent3.Type &&
                   x.Parent3.BodyID == y.Parent3.BodyID &&
                   x.Parent4.Type == y.Parent4.Type &&
                   x.Parent4.BodyID == y.Parent4.BodyID &&
                   x.Parent5.Type == y.Parent5.Type &&
                   x.Parent5.BodyID == y.Parent5.BodyID &&
                   x.Parent6.Type == y.Parent6.Type &&
                   x.Parent6.BodyID == y.Parent6.BodyID &&
                   x.Parent7.Type == y.Parent7.Type &&
                   x.Parent7.BodyID == y.Parent7.BodyID;
        }

        public static int GetHashCode(in ParentSetStruct x)
        {
            return unchecked(
                x.Parent1.BodyID * 0x01000000 +
                x.Parent2.BodyID * 0x00100000 +
                x.Parent3.BodyID * 0x00010000 +
                x.Parent4.BodyID * 0x00001000 +
                x.Parent5.BodyID * 0x00000100 +
                x.Parent6.BodyID * 0x00000010 +
                x.Parent7.BodyID * 0x00000001);
        }

        public static ParentSetStruct FromJSON(JArray ja)
        {
            ParentEntry[] parents = new ParentEntry[8];

            if (ja != null)
            {
                List<ParentEntry> parentlist =
                    ja.OfType<JObject>()
                        .SelectMany(e =>
                            e.Properties()
                                .Select(p => new ParentEntry
                                {
                                    Type = BodyDatabase.BodyType.Intern(p.Name),
                                    BodyID = p.Value.Value<short>()
                                })
                        )
                        .Reverse()
                        .ToList();
                parentlist.CopyTo(0, parents, 0, parentlist.Count < 8 ? parentlist.Count : 8);
            }

            return new ParentSetStruct
            {
                Parent0 = parents[0],
                Parent1 = parents[1],
                Parent2 = parents[2],
                Parent3 = parents[3],
                Parent4 = parents[4],
                Parent5 = parents[5],
                Parent6 = parents[6],
                Parent7 = parents[7],
            };
        }
    }
}
