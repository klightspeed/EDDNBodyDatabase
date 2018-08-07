using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public struct XParentEntry
    {
        public byte TypeId;
        public string Type { get { return BodyDatabase.BodyType.GetName(TypeId); } set { TypeId = BodyDatabase.BodyType.GetId(value) ?? 0; } }
        public short BodyID;
    }
}
