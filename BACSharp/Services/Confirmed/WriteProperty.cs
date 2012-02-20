using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    class WriteProperty : IBacNetApdu
    {
        public BacNetObject ObjectId { get; set; }
        public BacNetUInt PropertyId { get; set; }
        public BacNetUInt ArrayIndex { get; set; }
        public ArrayList ValueList { get; set; }
        public int InvokeId { get; set; }

        public WriteProperty(byte[] apdu)
        {
            /*int len = 4;
            if (apdu.Length < 7)
                throw new Exception("Reject.Missing_required_paramter");
            //Object tag
            BacNetTag objectIdTag = new BacNetTag(apdu, len, ref len);
            if (objectIdTag.Class == false)
                throw new Exception("Reject.Invalid_tag");
            ObjectId = new BacNetObject(apdu, len, ref len);
            //Property Id
            BacNetTag propertyIdTag = new BacNetTag(apdu, len, ref len);
            if(propertyIdTag.Number != 1)
                throw new Exception("Reject.Invalid_tag");
            PropertyId = new BacNetUInt(apdu, len, propertyIdTag.Length, ref len);
            if(len < apdu.Length)
            {
                BacNetTag arrayIndexTag = new BacNetTag(apdu, len, ref len);
                if(arrayIndexTag.Number == 2 && len < apdu.Length)
                {
                    ArrayIndex = new BacNetUInt(apdu, len, arrayIndexTag.Length, ref len);
                }
                else
                {
                    throw new Exception("Reject.InvalidTag");
                }
            }
            if(len<apdu.Length)
                throw new Exception("Reject.TooManyArguments");*/
        }

        public WriteProperty(BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId, ArrayList valueList)
        {
            ObjectId = bacNetObject;
            PropertyId = new BacNetUInt();
            PropertyId.Value = (uint) propertyId;
            ValueList = valueList;
            InvokeId = BacNetDevice.Instance.InvokeId;
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST);
            res.Add((byte)3);

            res.Add((byte)InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_WRITE_PROPERTY);

            //Object ID
            BacNetTag objectTag = new BacNetTag { Class = true, Length = 4, Number = 0 };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(ObjectId.GetObjectBytes());

            //Property ID
            BacNetTag propertyIdTag = new BacNetTag { Class = true, Length = (byte)PropertyId.GetLength(), Number = 1 };
            res.AddRange(propertyIdTag.GetBytes());
            res.AddRange(PropertyId.GetBytes());

            if (ArrayIndex != null)
            {
                BacNetTag arrayIndexTag = new BacNetTag { Class = true, Length = (byte)ArrayIndex.GetLength(), Number = 2 };
                res.AddRange(arrayIndexTag.GetBytes());
                res.AddRange(ArrayIndex.GetBytes());
            }

            //Opening Tag
            BacNetTag openingTag = new BacNetTag { Class = true, Length = 6, Number = 3 };
            res.AddRange(openingTag.GetBytes());

            BacNetTag metaTag = new BacNetTag();
            foreach (var value in ValueList)
            {
                int type;
                byte[] valueBytes = ByteConverter.GetPropertyValueBytes(value, out type);
                metaTag = new BacNetTag { Class = false, Length = (byte)valueBytes.Length, Number = (byte)type };
                res.AddRange(metaTag.GetBytes());
                res.AddRange(valueBytes);
            }

            //Closing Tag
            BacNetTag closingTag = new BacNetTag { Class = true, Length = 7, Number = 3 };
            res.AddRange(closingTag.GetBytes());

            BacNetTag priorityTag = new BacNetTag {Class = true, Number = 4, Length = 1};
            res.AddRange(priorityTag.GetBytes());
            res.Add((byte)0xA);
            

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
