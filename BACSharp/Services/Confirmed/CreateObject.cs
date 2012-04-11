using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    class CreateObject : IBacNetApdu
    {
        public BacNetObject NewObject { get; private set; }

        public CreateObject(BacNetObject bacNetObject) 
        {
            NewObject = bacNetObject;
        }

        public CreateObject(byte[] apdu) { }

        public byte[] GetBytes() 
        {
            ArrayList res = new ArrayList();
            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST);
            res.Add((byte)84);
            res.Add(BacNetDevice.Instance.InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_CREATE_OBJECT);

            BacNetTag openingTag = new BacNetTag { Class = true, Length = 6, Number = 0 };
            res.AddRange(openingTag.GetBytes());

            BacNetTag objectTag = new BacNetTag { Class = true, Length = 4, Number = 1 };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(NewObject.GetObjectBytes());

            BacNetTag closingTag = new BacNetTag { Class = true, Length = 7, Number = 0 };
            res.AddRange(closingTag.GetBytes());


            objectTag.Class = false;
            objectTag.Number = 0;

            openingTag.Number = 1;
            res.AddRange(openingTag.GetBytes());

            openingTag.Number = 2;
            closingTag.Number = 2;

            foreach (BacNetProperty property in NewObject.Properties) 
            {
                BacNetTag propertyIdTag = new BacNetTag { Class = true, Length = (byte)property.PropertyId.GetLength(), Number = 0 };
                res.AddRange(propertyIdTag.GetBytes());
                res.AddRange(property.PropertyId.GetBytes());

                res.AddRange(openingTag.GetBytes());

                BacNetTag metaTag = new BacNetTag();
                foreach (var value in property.Values)
                {
                    int type;
                    byte[] valueBytes = ByteConverter.GetPropertyValueBytes(value, out type);
                    metaTag = new BacNetTag { Class = false, Length = (byte)valueBytes.Length, Number = (byte)type, LongTag = true };
                    res.AddRange(metaTag.GetBytes());
                    res.AddRange(valueBytes);
                }

                res.AddRange(closingTag.GetBytes());
            }

            closingTag.Number = 1;
            res.AddRange(closingTag.GetBytes());

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
