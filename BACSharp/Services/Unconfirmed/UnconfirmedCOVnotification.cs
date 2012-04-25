using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Unconfirmed
{
    class UnconfirmedCOVnotification
    {
        public BacNetUInt ProccessId { get; private set; }
        public byte InvokeId { get; private set; }
        public BacNetUInt TimeRemaining { get; private set; }
        public BacNetObject Device { get; private set; }
        public BacNetObject Object { get; private set; }


        public UnconfirmedCOVnotification(byte[] apdu) 
        {
            int len = 2;

            BacNetTag tag = new BacNetTag(apdu, len, ref len);
            ProccessId = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Device = new BacNetObject(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Object = new BacNetObject(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            TimeRemaining = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            BacNetUInt value = new BacNetUInt(apdu, len, tag.Length, ref len);

            //{
            tag = new BacNetTag(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            BacNetReal analogvalue = new BacNetReal(apdu, len, tag.Length, ref len);

            //}
            tag = new BacNetTag(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);

        }



    }
}
