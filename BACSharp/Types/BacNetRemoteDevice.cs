using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
        public BacNetObject[] Objects { get; set; }        
    }
}
