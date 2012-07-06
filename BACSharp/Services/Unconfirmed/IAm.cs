using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Unconfirmed
{
    public class IAm : IBacNetApdu 
    {
        public BacNetObject deviceObject { get; set; }
        public BacNetUInt MaxApduLength { get; set; }
        public BacNetEnums.BACNET_SEGMENTATION SegmentationSupported { get; set; }
        public BacNetUInt VendorId { get; set; }

        public IAm()
        {
            deviceObject = new BacNetObject { ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE };
        }

        public IAm(byte[] apdu)
        {
            int len = 2;

            /* OBJECT ID - BacNetObject */
            BacNetTag deviceTag = new BacNetTag(apdu, len, ref len);
            if (deviceTag.Number != (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OBJECT_ID)
                throw new Exception("Device Id tag is missed.");
            deviceObject = new BacNetObject(apdu, len, ref len);
            if (deviceObject.ObjectType != BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE)
                throw new Exception("Device object is missed.");

            /* MAX APDU - uint */
            BacNetTag maxLengthTag = new BacNetTag(apdu, len, ref len);
            if (maxLengthTag.Number != (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT)
                throw new Exception("Max APDU length tag is missed.");
            MaxApduLength = new BacNetUInt(apdu, len, maxLengthTag.Length, ref len);

            /* Segmentation - enum */
            BacNetTag segmentationTag = new BacNetTag(apdu, len, ref len);
            if (segmentationTag.Number != (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED)
                throw new Exception("Segmentation APDU tag is missed.");
            BacNetEnumeration segmentation = new BacNetEnumeration(apdu, len, segmentationTag.Length, ref len);
            if (segmentation.Value > (int)BacNetEnums.BACNET_SEGMENTATION.MAX_BACNET_SEGMENTATION)
                throw new Exception("Segmentation error.");
            BacNetEnums.BACNET_SEGMENTATION tSegmentation;
            if (BacNetEnums.BACNET_SEGMENTATION.TryParse(segmentation.Value.ToString(), out tSegmentation))
                SegmentationSupported = tSegmentation;

            /* Vendor ID - UShort */
            BacNetTag vendorTag = new BacNetTag(apdu, len, ref len);
            if (vendorTag.Number != (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT)
                throw new Exception("Vendor Id tag is missed.");
            VendorId = new BacNetUInt(apdu, len, vendorTag.Length, ref len);
        }       

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte) BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST);
            res.Add((byte) BacNetEnums.BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_I_AM);

            //Object ID
            BacNetTag deviceTag = new BacNetTag { Class = false, Length = 4, Number = (byte)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OBJECT_ID };
            res.AddRange(deviceTag.GetBytes());
            BacNetObject device = new BacNetObject { ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE, ObjectId = deviceObject.ObjectId};
            res.AddRange(device.GetObjectBytes());

            //Max APDU
            BacNetTag maxApduTag = new BacNetTag { Class = false, Length = (byte)MaxApduLength.GetLength(), Number = (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT };
            res.AddRange(maxApduTag.GetBytes());
            res.AddRange(MaxApduLength.GetBytes());

            //Segmentation
            BacNetTag segmentationTag = new BacNetTag { Class = false, Length = 1, Number = (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_ENUMERATED };
            res.AddRange(segmentationTag.GetBytes());
            res.Add((byte) SegmentationSupported);

            //Vendor Id
            BacNetTag vendorIdTag = new BacNetTag { Class = false, Length = (byte)VendorId.GetLength(), Number = (int)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT };
            res.AddRange(vendorIdTag.GetBytes());
            res.AddRange(VendorId.GetBytes());

            return (byte[]) res.ToArray(typeof (byte));
        }
    }
}
