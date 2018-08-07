using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public class ParentSetList<T> : IReadOnlyList<T>
        where T : struct, IWithParentId, IStructEquatable<T>, IWithNextId
    {
        private readonly StructEqualityComparison<T> EqualsFunc;
        private readonly StructUpdater<T> UpdateFunc;
        private long EqualTestsRun = 0;
        private int DeepestRun = 0;
        private int DeepestRunIndex = 0;

        public ParentSetList(StructEqualityComparison<T> equalsFunc, StructUpdater<T> updateFunc = null)
        {
            EqualsFunc = equalsFunc;
            UpdateFunc = updateFunc;
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

                if (pos / 1024 >= Nodes.Length)
                {
                    Array.Resize(ref Nodes, Nodes.Length / 2);
                }

                if (Nodes[pos / 1024] == null)
                {
                    Nodes[pos / 1024] = new T[1024];
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

        private T[][] Nodes = new T[16384][];
        private Dictionary<int, int> ParentPointers = new Dictionary<int, int>();

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

        protected int IndexOf(in T val, out int pos)
        {
            int parent = val.ParentId;
            if (ParentPointers.ContainsKey(parent))
            {
                pos = ParentPointers[parent];

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
            return IndexOf(in val, out _);
        }

        public bool Contains(in T val)
        {
            return IndexOf(in val) > 0;
        }

        public int Add(in T val)
        {
            int pos = -1;

            int parent = val.ParentId;
            lock (ParentPointers)
            {
                if (!ParentPointers.ContainsKey(parent))
                {
                    pos = AddNode(in val);
                    ParentPointers[parent] = pos;
                    return pos;
                }
                else
                {
                    pos = ParentPointers[parent];
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

        public int GetOrAdd(in T val)
        {
            lock (Nodes)
            {
                int pos = IndexOf(in val, out int next);

                if (pos > 0)
                {
                    return pos;
                }
                else if (next == 0)
                {
                    pos = AddNode(in val);
                    ParentPointers[val.ParentId] = pos;
                    return pos;
                }
                else
                {
                    pos = ParentPointers[val.ParentId];
                    lock (Nodes)
                    {
                        int depth = 0;
                        int head = pos;
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
