using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public struct SystemStruct : IEquatable<SystemStruct>, IStructEquatable<SystemStruct>
    {
        public short RegionId { get; set; }
        public short Sequence { get; set; }
        public byte Mid1a { get; set; }
        public byte Mid1b { get; set; }
        public byte Mid2 { get; set; }
        public byte SizeClass { get; set; }
        public byte Mid3 { get; set; }

        public Models.Region Region { get { return RegionId == 0 ? null : BodyDatabase.Region[RegionId]; } set { RegionId = value?.Id ?? 0; } }

        public string Name
        {
            get
            {
                return $"{Region.Name} {(char)(Mid1a + 'A')}{(char)(Mid1b + 'A')}-{(char)(Mid2 + 'A')} " +
                       $"{(char)(SizeClass + 'a')}{(Mid3 == 0 ? "" : $"{Mid3}-")}{Sequence}";
            }
        }

        public long ToSystemAddress()
        {
            Models.Region reg = Region;

            if (reg.X0 == null)
            {
                var regpos = PGSectors.GetSectorPos(reg.Name);
                reg.X0 = regpos.X * 40960;
                reg.Y0 = regpos.Y * 40960;
                reg.Z0 = regpos.Z * 40960;
            }

            int mid = ((Mid3 * 26 + Mid2) * 26 + Mid1b) * 26 + Mid1a;
            ulong x = (uint)(mid & 0x7F) + (uint)Math.Floor((float)reg.X0 / (320 << SizeClass));
            ulong y = (uint)((mid >> 7) & 0x7F) + (uint)Math.Floor((float)reg.Y0 / (320 << SizeClass));
            ulong z = (uint)((mid >> 14) & 0x7F) + (uint)Math.Floor((float)reg.Z0 / (320 << SizeClass));
            ulong seq = (ulong)Sequence;

            return (long)(SizeClass | (z << 3) | (y << (17 - SizeClass)) | (x << (30 - SizeClass * 2)) | (seq << (44 - SizeClass * 3)));
        }

        public long ToModSystemAddress()
        {
            Models.Region reg = Region;

            if (reg.X0 == null)
            {
                var regpos = PGSectors.GetSectorPos(reg.Name);
                reg.X0 = regpos.X * 40960;
                reg.Y0 = regpos.Y * 40960;
                reg.Z0 = regpos.Z * 40960;
            }

            int mid = ((Mid3 * 26 + Mid2) * 26 + Mid1b) * 26 + Mid1a;
            ulong x = (uint)(mid & 0x7F) + (uint)Math.Floor((float)reg.X0 / (320 << SizeClass));
            ulong x1 = x & 0x7F;
            ulong x2 = (x >> 7) & 0x7F;
            ulong y = (uint)((mid >> 7) & 0x7F) + (uint)Math.Floor((float)reg.Y0 / (320 << SizeClass));
            ulong y1 = y & 0x7F;
            ulong y2 = (y >> 7) & 0x3F;
            ulong z = (uint)((mid >> 14) & 0x7F) + (uint)Math.Floor((float)reg.Z0 / (320 << SizeClass));
            ulong z1 = z & 0x7F;
            ulong z2 = (z >> 7) & 0x7F;
            ulong seq = (ulong)Sequence;
            ulong szclass = SizeClass;

            return (long)(seq | (x1 << 16) | (y1 << 23) | (z1 << 30) | (szclass << 37) | (x2 << 40) | (y2 << 47) | (z2 << 53));
        }

        public static bool TryParse(string s, out SystemStruct sys)
        {
            sys = new SystemStruct();
            if (s == null) return false;

            int i = s.Length - 1;
            byte mid3 = 0;

            string _s = s.ToLowerInvariant();

            if (i < 9) return false;                                   // a bc-d e0
            if (_s[i] < '0' || _s[i] > '9') return false;              // cepheus dark region a sector xy-z a1-[0]
            while (i > 8 && _s[i] >= '0' && _s[i] <= '9') i--;
            if (!Int16.TryParse(_s.Substring(i + 1), out short seq)) return false;
            sys.Sequence = seq;
            if (_s[i] == '-')                                          // cepheus dark region a sector xy-z a1[-]0
            {
                i--;
                int vend = i;
                while (i > 8 && _s[i] >= '0' && _s[i] <= '9') i--;     // cepheus dark region a sector xy-z a[1]-0
                if (i == vend) return false;
                if (!Byte.TryParse(_s.Substring(i + 1, vend - i), out mid3)) return false;
                sys.Mid3 = mid3;
            }
            if (_s[i] < 'a' || _s[i] > 'h') return false;              // cepheus dark region a sector xy-z [a]1-0
            sys.SizeClass = (byte)(_s[i] - 'a');
            i--;
            if (_s[i] != ' ') return false;                            // cepheus dark region a sector xy-z[ ]a1-0
            i--;
            if (_s[i] < 'a' || _s[i] > 'z') return false;              // cepheus dark region a sector xy-[z] a1-0
            sys.Mid2 = (byte)(_s[i] - 'a');
            i--;
            if (_s[i] != '-') return false;                            // cepheus dark region a sector xy[-]z a1-0
            i--;
            if (_s[i] < 'a' || _s[i] > 'z') return false;              // cepheus dark region a sector x[y]-z a1-0
            sys.Mid1b = (byte)(_s[i] - 'a');
            i--;
            if (_s[i] < 'a' || _s[i] > 'z') return false;              // cepheus dark region a sector [x]y-z a1-0
            sys.Mid1a = (byte)(_s[i] - 'a');
            i--;
            if (_s[i] != ' ') return false;                            // cepheus dark region a sector[ ]xy-z a1-0
            i--;
            var regname = s.Substring(0, i + 1);                       // [cepheus dark region a sector] xy-z a1-0
            sys.Region = BodyDatabase.Region[regname];
            return true;
        }

        public static SystemStruct FromSystemAddress(long systemaddress)
        {
            byte sizeclass = (byte)(systemaddress & 7);
            short z0 = (short)((systemaddress >> 3) & (0x3FFF >> sizeclass));
            byte z1 = (byte)((systemaddress >> 3) & (0x7F >> sizeclass));
            sbyte z2 = (sbyte)((systemaddress >> (10 - sizeclass)) & 0x7F);
            short y0 = (short)((systemaddress >> (17 - sizeclass)) & (0x1FFF >> sizeclass));
            byte y1 = (byte)((systemaddress >> (17 - sizeclass)) & (0x7F >> sizeclass));
            sbyte y2 = (sbyte)((systemaddress >> (24 - sizeclass * 2)) & 0x3F);
            short x0 = (short)((systemaddress >> (30 - sizeclass * 2)) & (0x3FFF >> sizeclass));
            byte x1 = (byte)((systemaddress >> (30 - sizeclass * 2)) & (0x7F >> sizeclass));
            sbyte x2 = (sbyte)((systemaddress >> (37 - sizeclass * 3)) & 0x7F);
            short seq = (short)((systemaddress >> (44 - sizeclass * 3)) & 0x7FFF);
            var region = BodyDatabase.Region[PGSectors.GetSectorName(new PGSectors.ByteXYZ(x2, y2, z2))];
            int mid = x1 | (y1 << 7) | (z1 << 14);
            byte mid1a = (byte)(mid % 26);
            byte mid1b = (byte)((mid / 26) % 26);
            byte mid2 = (byte)((mid / (26 * 26)) % 26);
            byte mid3 = (byte)((mid / (26 * 26 * 26)));

            return new SystemStruct
            {
                Region = region,
                Mid1a = mid1a,
                Mid1b = mid1b,
                Mid2 = mid2,
                Mid3 = mid3,
                SizeClass = sizeclass,
                Sequence = seq
            };
        }

        public static SystemStruct FromModSystemAddress(long systemaddress)
        {
            short seq = (short)(systemaddress & 0x7FFF);
            int mid = (int)((systemaddress >> 16) & 0x1FFFFF);
            byte sizeclass = (byte)((systemaddress >> 37) & 7);
            sbyte x2 = (sbyte)((systemaddress >> 40) & 0x7F);
            sbyte y2 = (sbyte)((systemaddress >> 47) & 0x3F);
            sbyte z2 = (sbyte)((systemaddress >> 53) & 0x7F);
            var region = BodyDatabase.Region[PGSectors.GetSectorName(new PGSectors.ByteXYZ(x2, y2, z2))];
            byte mid1a = (byte)(mid % 26);
            byte mid1b = (byte)((mid / 26) % 26);
            byte mid2 = (byte)((mid / (26 * 26)) % 26);
            byte mid3 = (byte)((mid / (26 * 26 * 26)));

            return new SystemStruct
            {
                Region = region,
                Mid1a = mid1a,
                Mid1b = mid1b,
                Mid2 = mid2,
                Mid3 = mid3,
                SizeClass = sizeclass,
                Sequence = seq
            };
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is SystemStruct sys &&
                   this.Equals(sys);
        }

        public override int GetHashCode()
        {
            return GetHashCode(in this);
        }

        public bool Equals(SystemStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in SystemStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in SystemStruct x, in SystemStruct y)
        {
            return x.RegionId == y.RegionId &&
                   x.Mid1a == y.Mid1a &&
                   x.Mid1b == y.Mid1b &&
                   x.Mid2 == y.Mid2 &&
                   x.Mid3 == y.Mid3 &&
                   x.SizeClass == y.SizeClass &&
                   x.Sequence == y.Sequence;
        }

        public static int GetHashCode(in SystemStruct x)
        {
            return (x.RegionId * 52711) ^
                   (((((x.Mid3 * 26 + x.Mid2) * 26 + x.Mid1b) * 26 + x.Mid1a) | (x.SizeClass << 21)) * 127) ^
                   (x.Sequence * 3);
        }
    }
}
