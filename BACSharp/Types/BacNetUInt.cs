using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetUInt
    {
        public uint Value { get; set; }

        public BacNetUInt()
        {}

        public BacNetUInt(uint value)
        {
            Value = value;
        }

        public BacNetUInt(byte[] apdu, int startIndex, int length, ref int len)
        {
            if (length > 4)
                throw new Exception("Length must be not greater then 4.");            
            if (apdu.Length - startIndex < length)
                throw new Exception("Byte array length must be greater then " + (startIndex + length).ToString() + ".");            
            for (int i = 0; i < length;i++)
            {
                Value |= (uint)(apdu[startIndex + i] << 8 * (length - 1 - i));
            }
            len += length;
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
            for (int i = 0;i < res.Length;i++)
            {
                res[res.Length - 1 - i] = (byte) ((Value << (3 - i)*8) >> 24);
            }
            return res;
        }
    }
}
