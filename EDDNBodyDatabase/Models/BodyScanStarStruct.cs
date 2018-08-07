using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public struct BodyScanStarStruct : IEquatable<BodyScanStarStruct>, IStructEquatable<BodyScanStarStruct>
    {
        public float AbsoluteMagnitude;
        public float StellarMass;
        public short Age_MY;
        public byte StarTypeId;
        public byte LuminosityId;
        public string StarType { get { return BodyDatabase.StarType.GetName(StarTypeId); } set { StarTypeId = BodyDatabase.StarType.GetId(value) ?? 0; } }
        public string Luminosity { get { return BodyDatabase.Luminosity.GetName(LuminosityId); } set { LuminosityId = BodyDatabase.Luminosity.GetId(value) ?? 0; } }

        public bool Equals(BodyScanStarStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in BodyScanStarStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in BodyScanStarStruct x, in BodyScanStarStruct y)
        {
            return x.AbsoluteMagnitude == y.AbsoluteMagnitude &&
                   x.StellarMass == y.StellarMass &&
                   x.Age_MY == y.Age_MY &&
                   x.StarTypeId == y.StarTypeId &&
                   x.LuminosityId == y.LuminosityId;
        }

        public static BodyScanStarStruct FromJSON(JObject jo)
        {
            return new BodyScanStarStruct
            {
                StarType = jo.Value<string>("StarType"),
                AbsoluteMagnitude = jo.Value<float?>("AbsoluteMagnitude") ?? 0,
                Age_MY = jo.Value<short?>("Age_MY") ?? 0,
                StellarMass = jo.Value<float?>("StellarMass") ?? 0,
                Luminosity = jo.Value<string>("Luminosity"),
            };
        }
    }
}
