using System.Collections;
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
        public ArrayList Objects { get; set; }        
    }
}
