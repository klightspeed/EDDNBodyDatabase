﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class System
    {
        private static readonly DateTime EdsmTimeZero = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public int Id { get; set; }
        public long ModSystemAddress { get; set; }
        public long SystemAddress { get { return ModSystemAddressToSystemAddress(ModSystemAddress); } set { ModSystemAddress = SystemAddressToModSystemAddress(value); } }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public short RegionId { get; set; }
        public string Region { get { return BodyDatabase.Region.GetName(RegionId); } set { RegionId = BodyDatabase.Region.GetId(value) ?? 0; } }
        public byte Mid1a { get; set; }
        public byte Mid1b { get; set; }
        public byte Mid2 { get; set; }
        public byte SizeClass { get; set; }
        public byte Mid3 { get; set; }
        public short Sequence { get; set; }
        public int EdsmId { get; set; }
        public int EdsmLastModifiedSeconds { get; set; }

        public SystemCustomName SystemCustomName { get; set; }
        public virtual Region RegionRef { get; set; }

        public DateTime EdsmLastModified { get { return EdsmTimeZero.AddSeconds(EdsmLastModifiedSeconds); } set { EdsmLastModifiedSeconds = (int)value.Subtract(EdsmTimeZero).TotalSeconds; } }

        public string Name
        {
            get
            {
                if (SystemCustomName != null)
                {
                    return SystemCustomName.CustomName;
                }
                else
                {
                    return $"{Region} {(char)(Mid1a + 'A')}{(char)(Mid1b + 'A')}-{(char)(Mid2 + 'A')} " +
                           $"{(char)(SizeClass + 'a')}{(Mid3 == 0 ? "" : $"{Mid3}-")}{Sequence}";
                }
            }
        }

        public static long ModSystemAddressToSystemAddress(long msysaddr)
        {
            ulong z2 = (ulong)((msysaddr >> 53) & 0x7F);
            ulong y2 = (ulong)((msysaddr >> 47) & 0x3F);
            ulong x2 = (ulong)((msysaddr >> 40) & 0x7F);
            byte sizeclass = (byte)((msysaddr >> 37) & 0x07);
            ulong z1 = (ulong)((msysaddr >> 30) & 0x7F);
            ulong y1 = (ulong)((msysaddr >> 23) & 0x7F);
            ulong x1 = (ulong)((msysaddr >> 16) & 0x7F);
            ulong seq = (ulong)(msysaddr & 0xFFFF);
            ulong x0 = x1 | (x2 << 7);
            ulong y0 = y1 | (y2 << 7);
            ulong z0 = z1 | (z2 << 7);

            return (long)(sizeclass | (z0 << 3) | (y0 << (17 - sizeclass)) | (x0 << (30 - sizeclass * 2)) | (seq << (44 - sizeclass * 3)));
        }

        public static long SystemAddressToModSystemAddress(long systemaddress)
        {
            byte sizeclass = (byte)(systemaddress & 7);
            ulong z0 = (ulong)((systemaddress >> 3) & (0x3FFF >> sizeclass));
            ulong y0 = (ulong)((systemaddress >> (17 - sizeclass)) & (0x1FFF >> sizeclass));
            ulong x0 = (ulong)((systemaddress >> (30 - sizeclass * 2)) & (0x3FFF >> sizeclass));
            ulong z1 = (z0 & (0x7FUL >> sizeclass));
            ulong z2 = ((z0 >> (7 - sizeclass)) & 0x7FUL);
            ulong y1 = (y0 & (0x7FUL >> sizeclass));
            ulong y2 = ((y0 >> (7 - sizeclass)) & 0x3FUL);
            ulong x1 = (x0 & (0x7FUL >> sizeclass));
            ulong x2 = ((x0 >> (7 - sizeclass)) & 0x7FUL);
            ulong seq = (ulong)((systemaddress >> (44 - sizeclass * 3)) & 0x7FFF);
            return (long)(seq | (x1 << 16) | (y1 << 23) | (z1 << 30) | ((ulong)sizeclass << 37) | (x2 << 40) | (y2 << 47) | (z2 << 53));
        }

        public static long ModSystemAddressFromCoords(int x, int y, int z, byte sizeclass, ushort seq = 0)
        {
            ulong x0 = (ulong)(x / (320 << sizeclass));
            ulong y0 = (ulong)(y / (320 << sizeclass));
            ulong z0 = (ulong)(z / (320 << sizeclass));
            ulong z1 = (z0 & (0x7FUL >> sizeclass));
            ulong z2 = ((z0 >> (7 - sizeclass)) & 0x7FUL);
            ulong y1 = (y0 & (0x7FUL >> sizeclass));
            ulong y2 = ((y0 >> (7 - sizeclass)) & 0x3FUL);
            ulong x1 = (x0 & (0x7FUL >> sizeclass));
            ulong x2 = ((x0 >> (7 - sizeclass)) & 0x7FUL);
            return (long)(seq | (x1 << 16) | (y1 << 23) | (z1 << 30) | ((ulong)sizeclass << 37) | (x2 << 40) | (y2 << 47) | (z2 << 53));
        }

        public static System From(XModels.XSystem sys, string customname)
        {
            return new System
            {
                Id = sys.DbId,
                SystemAddress = sys.SystemAddress,
                X = (int)Math.Floor((sys.StarPosX + 49985) * 32.0 + 0.5),
                Y = (int)Math.Floor((sys.StarPosY + 40985) * 32.0 + 0.5),
                Z = (int)Math.Floor((sys.StarPosZ + 24105) * 32.0 + 0.5),
                RegionId = sys.RegionId,
                Mid1a = sys.Mid1a,
                Mid1b = sys.Mid1b,
                Mid2 = sys.Mid2,
                Mid3 = sys.Mid3,
                SizeClass = sys.SizeClass,
                Sequence = sys.Sequence,
                SystemCustomName = customname == null ? null : new SystemCustomName
                {
                    Id = sys.DbId,
                    SystemAddress = sys.SystemAddress,
                    CustomName = customname
                }
            };
        }
    }
}
