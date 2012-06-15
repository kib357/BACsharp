using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetTimeStamp
    {
        public int Value { get; set; }

        public BacNetTimeStamp()
        {}

        public BacNetTimeStamp(int value)
        {
            Value = value;
        }

        public BacNetTimeStamp(byte[] apdu, int startIndex, ref int len)
        {
            len += 10;
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
