using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetString
    {
        private string Value { get; set; }

        private BacNetEnums.BACNET_CHARACTER_STRING_ENCODING TextEncoding { get; set; }

        public BacNetString()
        {}

        public override string ToString()
        {
            return Value;
        }

        public BacNetString(byte[] apdu, int startIndex, int length, ref int len)
        {
            if (apdu[startIndex] == 0)
            {
                Value = Encoding.UTF8.GetString(apdu, startIndex + 1, length - 1);
                TextEncoding = BacNetEnums.BACNET_CHARACTER_STRING_ENCODING.CHARACTER_ANSI_X34;
            }
            len += length;
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();
            if (TextEncoding == BacNetEnums.BACNET_CHARACTER_STRING_ENCODING.CHARACTER_ANSI_X34)
            {
                res.Add((byte) 0);
                res.AddRange(Encoding.UTF8.GetBytes(Value));
            }
            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
