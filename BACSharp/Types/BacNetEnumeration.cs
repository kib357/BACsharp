using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetEnumeration :BacNetUInt
    {
        public BacNetEnumeration()
        {}

        public override string ToString()
        {
            return Value.ToString();
        }

        public BacNetEnumeration(byte[] apdu, int startIndex, int length, ref int len)
        {
            //!!!!Возможна ошибка с параметром len!!!!
            BacNetUInt res = new BacNetUInt(apdu, startIndex, length, ref len);
            Value = res.Value;
        }       
    }
}
