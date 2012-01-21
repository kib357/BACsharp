using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.APDU
{
    /*public class ConfirmedRequest : IBacNetApdu
    {
        protected BacNetEnums.BACNET_CONFIRMED_SERVICE ServiceNumber { get; set; }

        protected byte _maxApdu = 0x03;

        protected List<byte> _params = new List<byte>();

        public virtual byte[] GetBytes()
        {
            List<byte> res = new List<byte>();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST);
            res.Add(_maxApdu);
            res.Add(BacNetDevice.Instance.DeviceId);
            res.Add((byte)ServiceNumber);
            res.AddRange(_params);

            return res.ToArray();
        }        
    }*/
}
