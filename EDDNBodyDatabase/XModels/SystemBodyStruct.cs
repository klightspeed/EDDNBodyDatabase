using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.XModels
{
    public struct SystemBodyStruct : IEquatable<SystemBodyStruct>, IStructEquatable<SystemBodyStruct>
    {
        public byte Stars;
        public byte Planet;
        public byte Moon1;
        public byte Moon2;
        public byte Moon3;
        public bool IsBelt;

        public string GetName(string sysname)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sysname);
            if (Stars != 0)
            {
                sb.Append(" ");
                for (int i = 0; i < 8; i++)
                {
                    if (((Stars >> i) & 1) == 1)
                    {
                        sb.Append((char)('A' + i));
                    }
                }
            }

            if (Planet != 0)
            {
                sb.Append(" ");
                if (IsBelt)
                {
                    sb.Append((char)('A' + Planet - 1));
                    sb.Append(" Belt");

                    if (Moon1 != 0)
                    {
                        sb.Append($" Cluster {Moon1}");
                    }
                }
                else
                {
                    sb.Append(Planet.ToString());

                    if (Moon1 != 0)
                    {
                        sb.Append(" ");
                        sb.Append((char)('a' + Moon1 - 1));

                        if (Moon2 != 0)
                        {
                            sb.Append(" ");
                            sb.Append((char)('a' + Moon2 - 1));

                            if (Moon3 != 0)
                            {
                                sb.Append(" ");
                                sb.Append((char)('a' + Moon3 - 1));
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        public static bool TryParse(string bodyname, string sysname, out SystemBodyStruct body)
        {
            string pgname = bodyname.ToLowerInvariant();
            body = new SystemBodyStruct();

            if (pgname.Equals(sysname, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else if (!pgname.StartsWith(sysname, StringComparison.InvariantCultureIgnoreCase) || pgname[sysname.Length] != ' ')
            {
                return false;
            }

            int pos = sysname.Length + 1;

            if (pgname.Length < pos + 6 || pgname.Substring(pos + 1, 5) != " belt")
            {
                for (int s = 0; s < 8 && pos < pgname.Length; s++)
                {
                    if (pgname[pos] == 'a' + s)
                    {
                        body.Stars |= (byte)(1 << s);
                        pos++;
                    }
                }
            }

            if (body.Stars != 0)
            {
                if (pos == pgname.Length)
                    return true;
                if (pgname[pos] != ' ')
                    return false;

                pos++;
            }

            if (pgname[pos] >= '1' && pgname[pos] <= '9')
            {
                int pnumstart = pos;

                while (pos < pgname.Length && pgname[pos] >= '0' && pgname[pos] <= '9') pos++;

                if (!Byte.TryParse(pgname.Substring(pnumstart, pos - pnumstart), out body.Planet)) return false;
                if (pos == pgname.Length) return true;
                if (pgname[pos] != ' ') return false;

                pos++;

                if (pos == pgname.Length || pgname[pos] < 'a' || pgname[pos] > 'z') return false;

                body.Moon1 = (byte)(pgname[pos] - 'a' + 1);
                pos++;

                if (pos == pgname.Length) return true;
                if (pgname[pos] != ' ') return false;

                pos++;

                if (pos == pgname.Length || pgname[pos] < 'a' || pgname[pos] > 'z') return false;

                body.Moon2 = (byte)(pgname[pos] - 'a' + 1);
                pos++;
                if (pos == pgname.Length) return true;
                if (pgname[pos] != ' ') return false;

                pos++;

                if (pos == pgname.Length || pgname[pos] < 'a' || pgname[pos] > 'z') return false;

                body.Moon3 = (byte)(pgname[pos] - 'a' + 1);
                pos++;
            }
            else if (pgname.Length >= pos + 6 && pgname[pos] >= 'a' && pgname[pos] <= 'z' && pgname.Substring(pos + 1, 5) == " belt")
            {
                body.IsBelt = true;
                body.Planet = (byte)(pgname[pos] - 'a' + 1);
                pos += 6;

                if (pgname.Length >= pos + 10 && pgname.Substring(pos, 9) == " cluster " && pgname[pos + 9] >= '1' && pgname[pos + 9] <= '9')
                {
                    pos += 9;
                    int cnumstart = pos;

                    while (pos < pgname.Length && pgname[pos] >= '0' && pgname[pos] <= '9') pos++;

                    if (!Byte.TryParse(pgname.Substring(cnumstart, pos - cnumstart), out body.Moon1))
                    {
                        return false;
                    }
                }
            }

            if (pos != pgname.Length)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj is SystemBodyStruct other &&
                   this.Equals(other);
        }

        public bool Equals(SystemBodyStruct other)
        {
            return Equals(in this, in other);
        }

        public bool Equals(in SystemBodyStruct other)
        {
            return Equals(in this, in other);
        }

        public static bool Equals(in SystemBodyStruct x, in SystemBodyStruct y)
        {
            return x.Stars == y.Stars &&
                   x.Planet == y.Planet &&
                   x.Moon1 == y.Moon1 &&
                   x.Moon2 == y.Moon2 &&
                   x.Moon3 == y.Moon3 &&
                   x.IsBelt == y.IsBelt;
        }

        public static int GetHashCode(in SystemBodyStruct x)
        {
            return (x.IsBelt ? 13 : 41) * ((((x.Stars * 19 + x.Planet) * 17 + x.Moon1) * 11 + x.Moon2) * 5 + x.Moon3);
        }

        public override int GetHashCode()
        {
            return (IsBelt ? 13 : 41) * ((((Stars * 19 + Planet) * 17 + Moon1) * 11 + Moon2) * 5 + Moon3);
        }
    }
}
