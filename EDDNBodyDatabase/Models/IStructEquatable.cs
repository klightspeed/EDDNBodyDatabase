using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public interface IStructEquatable<T>
    {
        bool Equals(in T other);
    }

    public interface IStructEqualityComparer<T>
    {
        bool Equals(in T x, in T y);
    }
}
