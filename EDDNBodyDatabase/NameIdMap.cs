using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

namespace EDDNBodyDatabase
{
    public class NameIdMap<T, U>
        where T : class, Models.INameIdMap<U>, new()
        where U : struct
    {
        private Dictionary<string, T> NameMap { get; set; } = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<U, T> IdMap { get; set; } = new Dictionary<U, T>();

        public NameIdMap(IEnumerable<T> seed)
        {
            using (Models.BodyDbContext ctx = new Models.BodyDbContext())
            {
                T[] maps = ctx.Set<T>().ToArray();

                if (maps.Length == 0)
                {
                    maps = seed.ToArray();
                    ctx.Set<T>().AddRange(maps);
                    ctx.SaveChanges();
                }

                foreach (T map in maps)
                {
                    NameMap[map.Name] = map;
                    IdMap[map.Id] = map;
                }
            }
        }

        public NameIdMap(string[] seed) : this(seed.Select(n => new T { Name = n }))
        {
        }

        public string GetName(U id)
        {
            if (id.Equals(default(U)))
            {
                return null;
            }
            return IdMap[id].Name;
        }

        public string GetName(U? id)
        {
            return id == null ? null : GetName((U)id);
        }

        public U? GetId(string name)
        {
            if (name == null)
            {
                return null;
            }

            if (!NameMap.ContainsKey(name))
            {
                using (Models.BodyDbContext ctx = new Models.BodyDbContext())
                {
                    T map = ctx.Set<T>().FirstOrDefault(v => v.Name == name);

                    if (map == null)
                    {
                        map = new T { Name = name };
                        ctx.Set<T>().Add(map);
                        ctx.SaveChanges();
                    }

                    IdMap[map.Id] = map;
                    NameMap[map.Name] = map;
                }
            }

            return NameMap[name].Id;
        }

        public T this[U id]
        {
            get
            {
                return IdMap[id];
            }
        }

        public T this[string name]
        {
            get
            {
                return IdMap[(U)GetId(name)];
            }
        }

        public string Intern(string val)
        {
            return GetName(GetId(val));
        }
    }

    public class NameIdMap<T> : NameIdMap<T, byte>
        where T : class, Models.INameIdMap<byte>, new()
    {
        public NameIdMap(IEnumerable<T> seed) : base(seed)
        {
        }

        public NameIdMap(string[] seed) : base(seed)
        {
        }
    }
}
