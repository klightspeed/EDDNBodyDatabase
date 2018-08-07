using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public interface INameIdMap<T>
        where T : struct
    {
        T Id { get; set; }
        string Name { get; set; }
    }

    public interface INameIdMapTinyId : INameIdMap<byte>
    {
    }

    public class Atmosphere : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class AtmosphereComponent : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class AtmosphereType : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class BodyType : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class Luminosity : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class MaterialName : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class PlanetClass : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class ReserveLevel : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class RingClass : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class ScanType : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class Software : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class StarType : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class TerraformState : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
    public class Volcanism : INameIdMapTinyId { public byte Id { get; set; } public string Name { get; set; } }
}
