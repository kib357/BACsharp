using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace BACSharp.Types
{
    public class BacNetRemoteDevice
    {
        public uint InstanceNumber { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public BacNetAddress BacAddress { get; set; }
        public BacNetUInt MaxApduLength { get; set; }
        public BacNetEnums.BACNET_SEGMENTATION Segmentation { get; set; }
        public BacNetUInt VendorId { get; set; }
        public List<BacNetObject> Objects { get; set; }
     
        public BacNetRemoteDevice()
        {
            Objects = new List<BacNetObject>();
        }

        public BacNetObject GetObject(string id)
        {
            uint objId;
            if (!uint.TryParse(new Regex(@"[0-9]+").Match(id).Value, out objId))
                throw new Exception("Not valid string id - wrong object number");            

            var objType = new Regex(@"[a-z\-A-Z]+").Match(id).Value;
            if (objType == string.Empty)
                throw new Exception("Not valid string id - empty object type");

            return Objects.FirstOrDefault(s => s.ObjectId == objId && s.ObjectType == BacNetObject.GetObjectType(objType));
        }

        public static BacNetRemoteDevice Get(string str)
        {
            uint instanceNumber;
            if(uint.TryParse(str, out instanceNumber))
                return new BacNetRemoteDevice{InstanceNumber = instanceNumber};
            throw new Exception("Wrong device number");
        }
    }
}
