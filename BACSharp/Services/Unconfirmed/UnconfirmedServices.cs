using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.NPDU;
using BACSharp.Types;

namespace BACSharp.Services.Unconfirmed
{
    public class UnconfirmedServices
    {
        public void WhoIs(UInt16 startAddress = UInt16.MinValue, UInt16 endAddress = UInt16.MaxValue, int timeOut = 1000)
        {
            var apdu = new WhoIs(startAddress, endAddress);
            var npdu = new BacNetIpNpdu();
            npdu.Destination = new BacNetAddress();
            npdu.Destination.Network = ByteConverter.GetBytes((ushort)65535);

            BacNetDevice.Instance.Services.Execute(npdu, apdu);
            //WaitForResponce();
        }

        public void IAm()
        {
            var apdu = new IAm();
            apdu.deviceObject = new BacNetObject {ObjectId = BacNetDevice.Instance.DeviceId, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE};
            apdu.MaxApduLength = new BacNetUInt {Value = 480};
            apdu.SegmentationSupported = BacNetEnums.BACNET_SEGMENTATION.SEGMENTATION_NONE;
            apdu.VendorId = new BacNetUInt { Value = 500 };

            var npdu = new BacNetIpNpdu();
            npdu.Source = new BacNetAddress { Address = ByteConverter.GetBytes(BacNetDevice.Instance.DeviceId), Network = ByteConverter.GetBytes((ushort)60001), HopCount = 255};

            BacNetDevice.Instance.Services.Execute(npdu, apdu);
        }
    }
}
