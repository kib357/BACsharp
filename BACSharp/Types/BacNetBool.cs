using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetBool
    {
        public bool Value { get; set; }

        public BacNetBool()
        {}

        public BacNetBool(byte[] apdu, int startIndex, int length, ref int len)
        {
            Value = (apdu[startIndex] & 1) == 1;
            len += length;
        }

        public int GetLength()
        {
            return 1;
        }

        public byte[] GetBytes()
        {
            byte[] res = new byte[1];
            res[0] = Convert.ToByte(Value);
            return res;
        }
    }
}
