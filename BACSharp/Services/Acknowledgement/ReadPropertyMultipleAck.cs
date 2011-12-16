using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    class ReadPropertyMultipleAck : IBacNetApdu
    {
        public List<BacNetObject> ObjectList { get; set; }
        public int InvokeId { get; set; }

        public ReadPropertyMultipleAck(byte[] apdu)
        {
            ObjectList = new List<BacNetObject>();
            InvokeId = apdu[1];
            int len = 3;
            if (apdu.Length < 9)
                throw new Exception("Reject.Missing_required_paramter");
            while (ReadObject(apdu, len, ref len))
            {
                var obj = ObjectList[ObjectList.Count - 1];
                if (obj == null) continue;
                obj.Properties = ReadProperties(apdu, len, obj.ObjectId, ref len);
            }
        }

        public ReadPropertyMultipleAck(List<BacNetObject> objectList)
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
                    //Object tag
                    BacNetTag objectIdTag = new BacNetTag(apdu, len, ref len);
                    if (objectIdTag.Class == false)
                        throw new Exception("Reject.Invalid_tag");
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

        private List<BacNetProperty> ReadProperties(byte[] apdu, int startIndex, uint objectId, ref int len)
        {
            List<BacNetProperty> res = new List<BacNetProperty>();
            var openingTag = new BacNetTag(apdu, len, ref len);
            if (openingTag.Length == 6 && openingTag.Number == 1)
            {
                BacNetTag metaTag = new BacNetTag(apdu, len, ref len);                
                while (metaTag.Length != 7 && metaTag.Number != 1)
                {
                    var propertyId = new BacNetUInt(apdu, len, metaTag.Length, ref len);
                    BacNetProperty prop = new BacNetProperty { PropertyId = propertyId};
                    prop.Values = ReadValues(apdu, len, ref len);
                    res.Add(prop);                    
                    metaTag = new BacNetTag(apdu, len, ref len);
                }
            }
            return res;
        }

        private ArrayList ReadValues(byte[] apdu, int startIndex, ref int len)
        {
            var valueList = new ArrayList();
            var openingTag = new BacNetTag(apdu, len, ref len);
            if (openingTag.Length == 6 && openingTag.Number == 4)
            {
                BacNetTag metaTag = new BacNetTag(apdu, len, ref len);
                while (metaTag.Length != 7)
                {
                    object value = ByteConverter.GetAppTagValue(apdu, len, metaTag, ref len);
                    valueList.Add(value);
                    metaTag = new BacNetTag(apdu, len, ref len);
                }
            }
            return valueList;
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
