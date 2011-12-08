using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetDouble
    {
        public double Value { get; set; }

        public BacNetDouble()
        {}

        public BacNetDouble(byte[] apdu, int startIndex, int length, ref int len)
        {
            Value = BitConverter.ToDouble(apdu, startIndex);
            len += 8;
        }

        public byte[] GetBytes()
        {
            byte[] res = BitConverter.GetBytes(Value);
            for (int i = 0; i < res.Length / 2; i++)
            {
                byte tmp = res[i];
                res[i] = res[res.Length - 1 - i];
                res[res.Length - 1 - i] = tmp;
            }
            return res;
        }
    }
}
