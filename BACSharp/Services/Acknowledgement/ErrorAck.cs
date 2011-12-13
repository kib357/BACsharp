using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    class ErrorAck : IBacNetApdu
    {
        public BacNetEnums.BACNET_ERROR_CLASS ErrorClass { get; set; }
        public BacNetEnums.BACNET_ERROR_CODE ErrorCode { get; set; }
        public byte InvokeId { get; set; }

        public ErrorAck(byte[] apdu)
        {
            InvokeId = apdu[1];
            int len = 3;
            //Error class
            BacNetTag errorClassTag = new BacNetTag(apdu, len, ref len);
            if (errorClassTag.Class)
                throw new Exception("Reject.Invalid_tag");
            BacNetEnumeration errorClass = new BacNetEnumeration(apdu, len, errorClassTag.Length, ref len);
            if (errorClass.Value > (int)BacNetEnums.BACNET_ERROR_CLASS.MAX_BACNET_ERROR_CLASS)
                throw new Exception("Error class error.");
            BacNetEnums.BACNET_ERROR_CLASS tErrorClass;
            if (BacNetEnums.BACNET_ERROR_CLASS.TryParse(errorClass.Value.ToString(), out tErrorClass))
                ErrorClass = tErrorClass;
            //Error code
            BacNetTag errorCodeTag = new BacNetTag(apdu, len, ref len);
            if (errorCodeTag.Class)
                throw new Exception("Reject.Invalid_tag");
            BacNetEnumeration errorCode = new BacNetEnumeration(apdu, len, errorCodeTag.Length, ref len);
            if (errorCode.Value > (int)BacNetEnums.BACNET_ERROR_CODE.MAX_BACNET_ERROR_CODE)
                throw new Exception("Error class error.");
            BacNetEnums.BACNET_ERROR_CODE tErrorCode;
            if (BacNetEnums.BACNET_ERROR_CODE.TryParse(errorCode.Value.ToString(), out tErrorCode))
                ErrorCode = tErrorCode;
        }

        public byte[] GetBytes()
        {
            /*ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_COMPLEX_ACK);
            res.Add(InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_READ_PROPERTY);

            //Object ID
            BacNetTag objectTag = new BacNetTag { Class = true, Length = 4, Number = 0 };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(ObjectId.GetObjectBytes());

            //Max APDU
            BacNetTag propertyIdTag = new BacNetTag { Class = true, Length = (byte)PropertyId.GetLength(), Number = 1 };
            res.AddRange(propertyIdTag.GetBytes());
            res.AddRange(PropertyId.GetBytes());

            /*if (ArrayIndex != null)
            {
                BacNetTag arrayIndexTag = new BacNetTag { Class = true, Length = (byte)ArrayIndex.GetLength(), Number = 2 };
                res.AddRange(arrayIndexTag.GetBytes());
                res.AddRange(ArrayIndex.GetBytes());
            }*/

            //return (byte[])res.ToArray(typeof(byte));
            return null;
        }
    }
}
