using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public interface IWithId
    {
        int Id { get; set; }
    }

    public interface IWithParentId : IWithId
    {
        int ParentId { get; set; }
    }

    public interface IWithNextId
    {
        int NextId { get; set; }
    }

    public interface IStructEquatable<T>
    {
        bool Equals(in T other);
    }

    public interface IStructEqualityComparer<T>
    {
        bool Equals(in T x, in T y);
    }

    public delegate bool StructEqualityComparison<T>(in T x, in T y);
    public delegate bool StructUpdater<T>(ref T x, in T b);
    public delegate int StructGetHashcode<T>(in T a);

}
