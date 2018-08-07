using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanAtmosphereStruct : IEquatable<BodyScanAtmosphereStruct>, IStructEquatable<BodyScanAtmosphereStruct>
    {
        public struct AtmosphereComponent : IComparable<AtmosphereComponent>, IEquatable<AtmosphereComponent>, IComparable, IStructEquatable<AtmosphereComponent>
        {
            public string Name { get { return BodyDatabase.AtmosphereComponent.GetName(Id); } set { Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }
            public float Amt;
            public byte Id;

            public int CompareTo(object obj)
            {
                if (obj is null)
                {
                    return -1;
                }
                else if (obj is AtmosphereComponent atm)
                {
                    return this.CompareTo(atm);
                }
                else
                {
                    throw new ArgumentException("Bad parameter type", "obj");
                }
            }

            public int CompareTo(AtmosphereComponent other)
            {
                if (this.Name != other.Name)
                {
                    return String.CompareOrdinal(this.Name, other.Name);
                }
                else
                {
                    return this.Amt.CompareTo(other.Amt);
                }
            }

            public override bool Equals(object obj)
            {
                return
                    obj != null &&
                    obj is AtmosphereComponent atm &&
                    this.Equals(atm);
            }

            public bool Equals(AtmosphereComponent other)
            {
                return Equals(in this, in other);
            }

            public bool Equals(in AtmosphereComponent other)
            {
                return Equals(in this, in other);
            }

            public static bool Equals(in AtmosphereComponent x, in AtmosphereComponent y)
            {
                return
                    String.Equals(x.Name, y.Name) &&
                    x.Amt == y.Amt;
            }

            public override int GetHashCode()
            {
                return
                    (Name?.GetHashCode() ?? 0) ^
                    Amt.GetHashCode();
            }
        }

        public float SurfacePressure;
        public float Component1Amt;
        public float Component2Amt;
        public float Component3Amt;
        public byte Component1Id;
        public byte Component2Id;
        public byte Component3Id;
        public byte AtmosphereId;
        public byte AtmosphereTypeId;
        public bool AtmosphereHot;
        public bool AtmosphereThin;
        public bool AtmosphereThick;
        public string Atmosphere { get { return BodyDatabase.Atmosphere.GetName(AtmosphereId); } set { AtmosphereId = BodyDatabase.Atmosphere.GetId(value) ?? 0; } }
        public string AtmosphereType { get { return BodyDatabase.AtmosphereType.GetName(AtmosphereTypeId); } set { AtmosphereTypeId = BodyDatabase.AtmosphereType.GetId(value) ?? 0; } }
        public string Component1Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component1Id); } set { Component1Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }
        public string Component2Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component2Id); } set { Component2Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }
        public string Component3Name { get { return BodyDatabase.AtmosphereComponent.GetName(Component3Id); } set { Component3Id = BodyDatabase.AtmosphereComponent.GetId(value) ?? 0; } }

        public bool Equals(BodyScanAtmosphereStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanAtmosphereStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanAtmosphereStruct x, in BodyScanAtmosphereStruct y)
        {
            return x.AtmosphereId == y.AtmosphereId &&
                   x.AtmosphereHot == y.AtmosphereHot &&
                   x.AtmosphereThin == y.AtmosphereThin &&
                   x.AtmosphereThick == y.AtmosphereThick &&
                   x.AtmosphereTypeId == y.AtmosphereTypeId &&
                   x.Component1Id == y.Component1Id &&
                   x.Component1Amt == y.Component1Amt &&
                   x.Component2Id == y.Component2Id &&
                   x.Component2Amt == y.Component2Amt &&
                   x.Component3Id == y.Component3Id &&
                   x.Component3Amt == y.Component3Amt;
        }

        public static BodyScanAtmosphereStruct FromJSON(JObject jo)
        {
            string atmosphere = jo.Value<string>("Atmosphere");
            bool atmosphereHot = false;
            bool atmosphereThin = false;
            bool atmosphereThick = false;

            if (atmosphere != null)
            {
                if (atmosphere.StartsWith("hot "))
                {
                    atmosphereHot = true;
                    atmosphere = atmosphere.Substring(4);
                }
                if (atmosphere.StartsWith("thin "))
                {
                    atmosphereThin = true;
                    atmosphere = atmosphere.Substring(5);
                }
                if (atmosphere.StartsWith("thick "))
                {
                    atmosphereThick = true;
                    atmosphere = atmosphere.Substring(6);
                }
            }

            AtmosphereComponent[] components = new AtmosphereComponent[3];
            var compjo = jo["AtmosphereComposition"];

            if (compjo != null)
            {
                var comps = compjo
                    .OfType<JObject>()
                    .Select(e => new AtmosphereComponent {
                        Name = BodyDatabase.AtmosphereComponent.Intern(e.Value<string>("Name")),
                        Amt = e.Value<float>("Percent")
                    }).ToList();

                comps.CopyTo(0, components, 0, comps.Count < 3 ? comps.Count : 3);
            }

            return new BodyScanAtmosphereStruct
            {
                SurfacePressure = jo.Value<float?>("SurfacePressure") ?? 0,
                Atmosphere = BodyDatabase.Atmosphere.Intern(atmosphere),
                AtmosphereHot = atmosphereHot,
                AtmosphereThin = atmosphereThin,
                AtmosphereThick = atmosphereThick,
                AtmosphereType = BodyDatabase.AtmosphereType.Intern(jo.Value<string>("AtmosphereType")),
                Component1Id = components[0].Id,
                Component1Amt = components[0].Amt,
                Component2Id = components[1].Id,
                Component2Amt = components[1].Amt,
                Component3Id = components[2].Id,
                Component3Amt = components[2].Amt,
            };
        }
    }
}
