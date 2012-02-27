using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetEnumeration
    {
        public uint Value { get; set; }

        public BacNetEnumeration()
        {}

        public BacNetEnumeration(uint value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public BacNetEnumeration(byte[] apdu, int startIndex, int length, ref int len)
        {
            BacNetUInt res = new BacNetUInt(apdu, startIndex, length, ref len);
            Value = res.Value;
        }

        public int GetLength()
        {
            if (Value < 256) return 1;
            if (Value < 65536) return 2;
            if (Value < 16777216) return 3;
            if (Value <= 4294967295) return 4;
            return 0;
        }

        public byte[] GetBytes()
        {
            byte[] res = new byte[GetLength()];
            for (int i = 0; i < res.Length; i++)
            {
                res[res.Length - 1 - i] = (byte)((Value << (3 - i) * 8) >> 24);
            }
            return res;
        }
    }
}
