using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Unconfirmed
{
    public class WhoIs : IBacNetApdu
    {
        public BacNetUInt LowLimit { get; set; }
        public BacNetUInt HighLimit { get; set; }

        public WhoIs(UInt16 startAddress = UInt16.MinValue, UInt16 endAddress = UInt16.MaxValue)
        {
            if (startAddress <= endAddress && endAddress != UInt16.MaxValue)
            {
                LowLimit = new BacNetUInt { Value = startAddress };
                HighLimit = new BacNetUInt { Value = endAddress };
            }
            /*ServiceNumber = (byte) BacNetEnums.BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_WHO_IS;
            if (startAddress != UInt16.MinValue && endAddress != UInt16.MaxValue)
            {
                byte[] start = ByteConverter.GetBytes(startAddress);
                _params.AddRange((new BacNetTag { Class = true, Length = (byte)start.Length, Number = 0 }).GetBytes());
                _params.AddRange(start);
                byte[] end = ByteConverter.GetBytes(endAddress);
                _params.AddRange((new BacNetTag { Class = true, Length = (byte)end.Length, Number = 1 }).GetBytes());
                _params.AddRange(end);
            }*/
        }

        public WhoIs(byte[] apdu)
        {
            if(apdu.Length < 2)
                throw new Exception("Malformed APDU byte array");
            if (apdu.Length == 2)
                return;
            int len = 2;
            //Low limit
            BacNetTag lowLimitTag = new BacNetTag(apdu, len, ref len);
            LowLimit = new BacNetUInt(apdu, len, lowLimitTag.Length, ref len);
            //High limit

            BacNetTag highLimitTag = new BacNetTag(apdu, len, ref len);
            HighLimit = new BacNetUInt(apdu, len, highLimitTag.Length, ref len);
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST);
            res.Add((byte)BacNetEnums.BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_WHO_IS);

            if (LowLimit != null && HighLimit != null)
            {
                //Low limit
                BacNetTag lowLimitTag = new BacNetTag { Class = true, Length = (byte)LowLimit.GetLength(), Number = 0 };
                res.AddRange(lowLimitTag.GetBytes());
                res.AddRange(LowLimit.GetBytes());

                //High limit
                BacNetTag highLimitTag = new BacNetTag { Class = true, Length = (byte)HighLimit.GetLength(), Number = 1 };
                res.AddRange(highLimitTag.GetBytes());
                res.AddRange(HighLimit.GetBytes());
            }

            return (byte[]) res.ToArray(typeof (byte));
        }
    }
}
