using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    class ReadPropertyMultiple : IBacNetApdu
    {
        public List<BacNetObject> ObjectList { get; set; }
        public int InvokeId { get; set; }

        public ReadPropertyMultiple(byte[] apdu)
        {
            InvokeId = apdu[2];
            int len = 4;
            if (apdu.Length < 7)
                throw new Exception("Reject.Missing_required_paramter");            
            //Object tag
            BacNetTag objectIdTag = new BacNetTag(apdu, len, ref len);
            if (objectIdTag.Class == false)
                throw new Exception("Reject.Invalid_tag");
            while (ReadObject(apdu, len, ref len))
            {
                var obj = ObjectList[ObjectList.Count - 1];
                if (obj == null) continue;
                ReadProperties(apdu, len, obj.ObjectId, ref len);
            }
        }

        public ReadPropertyMultiple(List<BacNetObject> objectList)
        {
            ObjectList = objectList;
            InvokeId = BacNetDevice.Instance.InvokeId;
        }

        private bool ReadObject(byte[] apdu, int startIndex, ref int len)
        {
            bool res = false;
            if (apdu.Length - 1 > startIndex)
            {
                try
                {
                    var objectId = new BacNetObject(apdu, len, ref len);
                    ObjectList.Add(objectId);
                    res = true;
                }
                catch
                {
                    throw new Exception("Reject.Invalid_object_tag");
                }
            }
            return res;
        }

        private void ReadProperties(byte[] apdu, int startIndex, uint objectId, ref int len)
        {
            
            /*var openingTag = new BacNetTag(apdu, len, ref len);
            if (openingTag.Length == 6)
            {
                BacNetTag metaTag = new BacNetTag(apdu, len, ref len);                                
                while (metaTag.Length != 7)
                {
                    var propertyId = new BacNetUInt(apdu, len, metaTag.Length, ref len);
                    BacNetProperty prop = new BacNetProperty { ObjectId = objectId, PropertyId = propertyId};
                    PropertiesList.Add(prop);
                    metaTag = new BacNetTag(apdu, len, ref len);
                }
            }*/
        }



        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST | (1 << 1)));
            res.Add((byte)84);

            res.Add((byte)InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_READ_PROP_MULTIPLE);

            //Object ID
            foreach (BacNetObject obj in ObjectList)
            {
                BacNetTag objectTag = new BacNetTag { Class = true, Length = 4, Number = 0 };
                res.AddRange(objectTag.GetBytes());
                res.AddRange(obj.GetObjectBytes());
                BacNetTag openingTag = new BacNetTag{Class = true, Length = 6, Number = 1};
                res.AddRange(openingTag.GetBytes());
                byte propNumber = 0;
                foreach (BacNetProperty property in obj.Properties)
                {
                    //Property ID
                    BacNetTag propertyIdTag = new BacNetTag { Class = true, Length = (byte)property.PropertyId.GetLength(), Number = propNumber };
                    res.AddRange(propertyIdTag.GetBytes());
                    res.AddRange(property.PropertyId.GetBytes());
                    propNumber++;
                }
                BacNetTag closingTag = new BacNetTag { Class = true, Length = 7, Number = 1 };
                res.AddRange(closingTag.GetBytes());
            }
            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
