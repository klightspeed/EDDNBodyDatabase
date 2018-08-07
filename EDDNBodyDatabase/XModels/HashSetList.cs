using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public class HashSetList<T> : IReadOnlyList<T>
        where T : struct, IWithId, IStructEquatable<T>, IWithNextId
    {
        private readonly StructEqualityComparison<T> EqualsFunc;
        private readonly StructUpdater<T> UpdateFunc;
        private readonly StructGetHashcode<T> HashCodeFunc;
        private long EqualTestsRun = 0;
        private int DeepestRun = 0;
        private int DeepestRunIndex = 0;

        public HashSetList(StructEqualityComparison<T> equalsFunc, StructGetHashcode<T> hashCodeFunc, StructUpdater<T> updaterFunc = null)
        {
            EqualsFunc = equalsFunc;
            HashCodeFunc = hashCodeFunc;
            UpdateFunc = updaterFunc;
        }

        protected void SetNode(ref T node, in T val, int pos)
        {
            node = val;
            node.Id = pos;
        }

        protected int AddNode(in T val)
        {
            lock (Nodes)
            {
                int pos = Count;

                if (pos / 1024 >= Nodes.Count)
                {
                    Nodes.Add(new T[1024]);
                }

                SetNode(ref Nodes[pos / 1024][pos % 1024], in val, pos);

                Count++;

                return pos;
            }
        }

        protected IEnumerable<T> DeepestRunEnts
        {
            get
            {
                int pos = DeepestRunIndex;
                while (pos != 0)
                {
                    yield return this[pos];
                    pos = this[pos].NextId;
                }
            }
        }

        private List<T[]> Nodes = new List<T[]> { new T[1024] };
        private Dictionary<int, int> HashPointers = new Dictionary<int, int>();

        public T this[int index]
        {
            get
            {
                return Nodes[index / 1024][index % 1024];
            }
        }

        public int Count { get; private set; } = 1;

        public IEnumerator<T> GetEnumerator()
        {
            return Nodes.SelectMany(n => n).Skip(1).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected bool TestEqual(in T val, in T cmp, out int next)
        {
            next = cmp.NextId;
            return EqualsFunc(in val, in cmp);
        }

        protected bool UpdateIfEqual(in T val, ref T cmp, out int next)
        {
            next = cmp.NextId;
            return UpdateFunc(ref cmp, in val);
        }

        protected bool TestEqual(in T val, int pos, out int next)
        {
            int div = Math.DivRem(pos, 1024, out int rem);
            EqualTestsRun++;
            if (UpdateFunc != null)
            {
                return UpdateIfEqual(in val, ref Nodes[div][rem], out next);
            }
            else
            {
                return TestEqual(in val, in Nodes[div][rem], out next);
            }
        }

        protected int IndexOf(in T val, out int pos, int hash)
        {
            if (HashPointers.ContainsKey(hash))
            {
                pos = HashPointers[hash];

                do
                {
                    if (TestEqual(in val, pos, out int next))
                    {
                        return pos;
                    }

                    if (next == 0)
                    {
                        return -1;
                    }

                    pos = next;
                }
                while (pos != 0);
            }

            pos = 0;
            return -1;
        }

        public int IndexOf(in T val)
        {
            return IndexOf(in val, out _, val.GetHashCode());
        }

        public bool Contains(in T val)
        {
            return IndexOf(in val) > 0;
        }

        public int Add(in T val, bool addhash = true)
        {
            int pos = -1;

            if (addhash)
            {
                int hash = HashCodeFunc(in val);
                lock (HashPointers)
                {
                    if (!HashPointers.ContainsKey(hash))
                    {
                        lock (Nodes)
                        {
                            pos = AddNode(in val);
                            HashPointers[hash] = pos;
                            return pos;
                        }
                    }
                    else
                    {
                        pos = HashPointers[hash];
                    }
                }

                lock (Nodes)
                {
                    int next;
                    int head = pos;
                    int depth = 0;
                    while ((next = Nodes[pos / 1024][pos % 1024].NextId) != 0)
                    {
                        pos = next;
                        depth++;
                    }

                    if (depth > DeepestRun)
                    {
                        DeepestRun = depth;
                        DeepestRunIndex = head;
                    }

                    int ptr = AddNode(in val);
                    Nodes[pos / 1024][pos % 1024].NextId = ptr;
                    return ptr;
                }
            }
            else
            {
                lock (Nodes)
                {
                    return AddNode(in val);
                }
            }
        }

        public int GetOrAdd(in T val)
        {
            int hash = HashCodeFunc(in val);

            lock (Nodes)
            {
                int pos = IndexOf(in val, out int next, hash);

                if (pos > 0)
                {
                    return pos;
                }
                else if (next == 0)
                {
                    pos = AddNode(in val);
                    HashPointers[hash] = pos;
                    return pos;
                }
                else
                {
                    pos = HashPointers[hash];
                    lock (Nodes)
                    {
                        int head = pos;
                        int depth = 0;
                        do
                        {
                            if (TestEqual(in val, pos, out next))
                            {
                                return pos;
                            }

                            if (next != 0)
                            {
                                pos = next;
                                depth++;
                            }
                        }
                        while (next != 0);

                        if (depth > DeepestRun)
                        {
                            DeepestRun = depth;
                            DeepestRunIndex = head;
                        }

                        int ptr = AddNode(in val);
                        Nodes[pos / 1024][pos % 1024].NextId = ptr;
                        return ptr;
                    }
                }
            }
        }
    }
}
