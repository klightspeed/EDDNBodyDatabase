using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanMaterialsStruct : IEquatable<BodyScanMaterialsStruct>, IStructEquatable<BodyScanMaterialsStruct>
    {
        public struct Material
        {
            public string Name { get { return BodyDatabase.MaterialName.GetName(Id); } set { Id = (byte)BodyDatabase.MaterialName.GetId(value); } }
            public float Amt;
            public byte Id;
        }

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
        public string Material1Name { get { return BodyDatabase.MaterialName.GetName(Material1Id); } set { Material1Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }
        public string Material2Name { get { return BodyDatabase.MaterialName.GetName(Material2Id); } set { Material2Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }
        public string Material3Name { get { return BodyDatabase.MaterialName.GetName(Material3Id); } set { Material3Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }
        public string Material4Name { get { return BodyDatabase.MaterialName.GetName(Material4Id); } set { Material4Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }
        public string Material5Name { get { return BodyDatabase.MaterialName.GetName(Material5Id); } set { Material5Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }
        public string Material6Name { get { return BodyDatabase.MaterialName.GetName(Material6Id); } set { Material6Id = BodyDatabase.MaterialName.GetId(value) ?? 0; } }

        public bool IsValid
        {
            get
            {
                return Material1Id != 0 &&
                       Material2Id != 0 &&
                       Material3Id != 0 &&
                       Material4Id != 0 &&
                       Material5Id != 0 &&
                       Material6Id != 0;
            }
        }

        public bool Equals(BodyScanMaterialsStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanMaterialsStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanMaterialsStruct x, in BodyScanMaterialsStruct y)
        {
            return x.MaterialCarbon == y.MaterialCarbon &&
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

        private static HashSet<string> MaterialsVeryCommon = new HashSet<string>(new string[] { "carbon", "iron", "nickel", "phosphorus", "sulphur" });

        public static BodyScanMaterialsStruct FromJSON(JToken jt)
        {
            float materialCarbon = 0;
            float materialIron = 0;
            float materialNickel = 0;
            float materialPhosphorus = 0;
            float materialSulphur = 0;
            Material[] materials = new Material[6];

            if (jt != null)
            {
                if (jt is JArray)
                {
                    JArray mats = (JArray)jt;
                    JObject matobj = new JObject();

                    foreach (JObject mat in mats)
                    {
                        matobj[mat.Value<string>("Name")] = mat.Value<float>("Percent");
                    }

                    jt = matobj;
                }

                if (jt is JObject)
                {
                    JObject mats = (JObject)jt;
                    materialCarbon = mats.Value<float?>("carbon") ?? 0;
                    materialIron = mats.Value<float?>("iron") ?? 0;
                    materialNickel = mats.Value<float?>("nickel") ?? 0;
                    materialPhosphorus = mats.Value<float?>("phosphorus") ?? 0;
                    materialSulphur = mats.Value<float?>("sulphur") ?? 0;
                    List<Material> matlist = mats
                        .Properties()
                        .Where(p => !MaterialsVeryCommon.Contains(p.Name))
                        .Select(p => new Material {
                            Name = BodyDatabase.MaterialName.Intern(p.Name),
                            Amt = p.Value.Value<float>()
                        }).ToList();

                    matlist.CopyTo(0, materials, 0, matlist.Count < 6 ? matlist.Count : 6);
                }
            }

            return new BodyScanMaterialsStruct
            {
                MaterialCarbon = materialCarbon,
                MaterialIron = materialIron,
                MaterialNickel = materialNickel,
                MaterialPhosphorus = materialPhosphorus,
                MaterialSulphur = materialSulphur,
                Material1Id = materials[0].Id,
                Material1Amt = materials[0].Amt,
                Material2Id = materials[1].Id,
                Material2Amt = materials[1].Amt,
                Material3Id = materials[2].Id,
                Material3Amt = materials[2].Amt,
                Material4Id = materials[3].Id,
                Material4Amt = materials[3].Amt,
                Material5Id = materials[4].Id,
                Material5Amt = materials[4].Amt,
                Material6Id = materials[5].Id,
                Material6Amt = materials[5].Amt,
            };
        }
    }
}
