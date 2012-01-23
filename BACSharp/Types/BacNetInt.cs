using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetInt
    {
        public int Value { get; set; }

        public BacNetInt()
        {}

        public BacNetInt(byte[] apdu, int startIndex, int length, ref int len)
        {
            if (length > 4)
                throw new Exception("Length must be not greater then 4.");            
            if (apdu.Length - startIndex < length)
                throw new Exception("Byte array length must be greater then " + (startIndex + length).ToString() + ".");            
            for (int i = 0; i < length;i++)
            {
                Value |= (apdu[startIndex + i] << 8 * (length - 1 - i));
            }
            len += length;
        }

        public int GetLength()
        {
            if (Value < 256) return 1;
            if (Value < 65536) return 2;
            if (Value < 16777216) return 3;
            if (Value <= 2147483647) return 4;
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
