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

        public BacNetReal(float value)
        {
            Value = value;
        }

        public BacNetReal(byte[] apdu, int startIndex, int length, ref int len)
        {
            byte[] value = new byte[length];
            for (int i = 0; i < length;i++ )
            {
                value[i] = apdu[startIndex + length - 1 - i];
            }
            Value = BitConverter.ToSingle(value, 0);
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
