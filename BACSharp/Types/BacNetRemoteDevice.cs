using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

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

        public string GetStringIN()
        {
            return InstanceNumber.ToString();
        }

        public static BacNetRemoteDevice Get(string str)
        {
            uint instanceNumber;
            if(uint.TryParse(str, out instanceNumber))
                return new BacNetRemoteDevice{InstanceNumber = instanceNumber};
            else
            {
                throw new Exception("Wrong device number");
            }
        }
    }
}
