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

        public BacNetBool(BacNetTag boolTag)
        {
            Value = (boolTag.Length & 1) == 1;
        }

        public override string ToString()
        {
            return Value.ToString();
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
