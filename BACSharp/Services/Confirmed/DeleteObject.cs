using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    class DeleteObject : IBacNetApdu
    {
        public DeleteObject(BacNetObject bacNetObject) 
        {
            _bacNetObject = bacNetObject;
        }

        public DeleteObject(byte[] apdu) { }

        public byte[] GetBytes() 
        {
            ArrayList res = new ArrayList();
            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST);
            res.Add((byte)84);
            res.Add(BacNetDevice.Instance.InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_DELETE_OBJECT);

            BacNetTag objectTag = new BacNetTag { Class = false, Length = 4, Number = (byte)BacNetEnums.BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_OBJECT_ID };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(_bacNetObject.GetObjectBytes());

            return (byte[])res.ToArray(typeof(byte));
        }


        private BacNetObject _bacNetObject;
    }
}
