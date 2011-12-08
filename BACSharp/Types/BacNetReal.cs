using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetReal
    {
        public float Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public BacNetReal()
        {}

        public BacNetReal(byte[] apdu, int startIndex, int length, ref int len)
        {
            Value = BitConverter.ToSingle(apdu, startIndex);
            len += 4;
        }

        public byte[] GetBytes()
        {
            byte[] res = BitConverter.GetBytes(Value);
            for (int i = 0; i < res.Length / 2;i++ )
            {
                byte tmp = res[i];
                res[i] = res[res.Length - 1 - i];
                res[res.Length - 1 - i] = tmp;
            }
            return res;
        }
    }
}
