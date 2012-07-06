using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    class SubscribeCOV : IBacNetApdu
    {
        public BacNetObject ObjectId { get; set; }
        public int InvokeId { get; set; }
        public BacNetUInt ProccessId { get; set; }

        public SubscribeCOV(byte[] apdu)
        {
            throw new NotImplementedException();
        }

        public SubscribeCOV(BacNetObject bacNetObject)
        {
            ObjectId = bacNetObject;
            InvokeId = BacNetDevice.Instance.InvokeId;
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST | (1 << 1)));
            res.Add((byte)84);

            res.Add((byte)InvokeId);
            res.Add((byte)BacNetEnums.BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_SUBSCRIBE_COV);


            //Process ID
            var processIdTag = new BacNetTag { Class = true, Length = (byte)ProccessId.GetLength(), Number = 0 };
            res.AddRange(processIdTag.GetBytes());
            res.AddRange(ProccessId.GetBytes());

            //Object ID
            var objectTag = new BacNetTag { Class = true, Length = 4, Number = 1 };
            res.AddRange(objectTag.GetBytes());
            res.AddRange(ObjectId.GetObjectBytes());

            //Issue Confirmed Notifications
            var icn = new BacNetBool();
            icn.Value = false;
            var icnTag = new BacNetTag { Class = true, Length = (byte)icn.GetLength(), Number = 2 };
            res.AddRange(icnTag.GetBytes());
            res.AddRange(icn.GetBytes());

            //Lifetime
            var lifeTime = new BacNetUInt();
            lifeTime.Value = 360;
            var lifeTimeTag = new BacNetTag { Class = true, Length = (byte)lifeTime.GetLength(), Number = 3 };
            res.AddRange(lifeTimeTag.GetBytes());
            res.AddRange(lifeTime.GetBytes());

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
