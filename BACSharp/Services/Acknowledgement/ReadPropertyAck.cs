using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    public class ReadPropertyAck : IBacNetApdu
    {
        public BacNetObject Obj { get; set; }
        public byte InvokeId { get; set; }

        public ReadPropertyAck(byte[] apdu)
        {
            InvokeId = apdu[1];
            int len = 3;
            //Object tag
            BacNetTag objectIdTag = new BacNetTag(apdu, len, ref len);
            if (objectIdTag.Class == false)
                throw new Exception("Reject.Invalid_tag");
            Obj = new BacNetObject(apdu, len, ref len);
            //Property Id
            BacNetTag propertyIdTag = new BacNetTag(apdu, len, ref len);
            if (propertyIdTag.Number != 1)
                throw new Exception("Reject.Invalid_tag");
            BacNetUInt PropertyId = new BacNetUInt(apdu, len, propertyIdTag.Length, ref len);
            BacNetTag openingTag = new BacNetTag(apdu, len, ref len);
            ArrayList ValueList = new ArrayList();
            if (openingTag.Length == 6)
            {                
                BacNetTag metaTag = new BacNetTag(apdu, len, ref len);
                while (metaTag.Length != 7)
                {                    
                    if (metaTag.Class == false)
                    {
                        object value = ByteConverter.GetAppTagValue(apdu, len, metaTag, ref len);
                        ValueList.Add(value);
                    }
                    else
                    {
                        if (metaTag.Length == 6 && PropertyId.Value == (int)BacNetEnums.BACNET_PROPERTY_ID.PROP_WEEKLY_SCHEDULE)
                        {
                            var value = BuildScheduleDay(apdu, ref len);
                            ValueList.Add(value);
                        }
                        if (metaTag.Length == 4 && metaTag.Number == 0 && PropertyId.Value == (int)BacNetEnums.BACNET_PROPERTY_ID.PROP_LIST_OF_OBJECT_PROPERTY_REFERENCES)
                        {
                            var value = BuildObjectPropertyReference(apdu, ref len);
                            ValueList.Add(value);
                        }
                    }                    
                    metaTag = new BacNetTag(apdu, len, ref len);
                }
            }
            var property = new BacNetProperty {PropertyId = PropertyId, Values = ValueList};
            Obj.Properties.Add(property);
        }

        private object BuildObjectPropertyReference(byte[] apdu, ref int len)
        {
            var objId = new BacNetObject(apdu, len, ref len);
            var metaTag = new BacNetTag(apdu, len, ref len);
            var propId = new BacNetUInt(apdu, len, metaTag.Length, ref len);
            return new BacNetObjectPropertyRef {ObjectId = objId, PropertyId = propId};
        }

        private static Dictionary<BacNetTime, object> BuildScheduleDay(byte[] apdu, ref int len)
        {
            var value = new Dictionary<BacNetTime, object>();
            var metaTag = new BacNetTag(apdu, len, ref len);
            while (metaTag.Length != 7)
            {
                var time = ByteConverter.GetAppTagValue(apdu, len, metaTag, ref len) as BacNetTime;
                metaTag = new BacNetTag(apdu, len, ref len);
                var timeValue = ByteConverter.GetAppTagValue(apdu, len, metaTag, ref len);
                if (time != null)
                {
                    value.Add(time, timeValue);
                }
                metaTag = new BacNetTag(apdu, len, ref len);
            }
            return value;
        }


        /// <summary>
        /// todo: реализовать выбор свойства для отправки
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_COMPLEX_ACK);
            res.Add(InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_READ_PROPERTY);

            //Object ID
            BacNetTag objectTag = new BacNetTag { Class = true, Length = 4, Number = 0 };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(Obj.GetObjectBytes());

            //Max APDU
            BacNetTag propertyIdTag = new BacNetTag { Class = true, Length = (byte)Obj.Properties[0].PropertyId.GetLength(), Number = 1 };
            res.AddRange(propertyIdTag.GetBytes());
            res.AddRange(Obj.Properties[0].PropertyId.GetBytes());

            /*if (ArrayIndex != null)
            {
                BacNetTag arrayIndexTag = new BacNetTag { Class = true, Length = (byte)ArrayIndex.GetLength(), Number = 2 };
                res.AddRange(arrayIndexTag.GetBytes());
                res.AddRange(ArrayIndex.GetBytes());
            }*/

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
