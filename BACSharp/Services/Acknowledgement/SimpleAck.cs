using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    public class SimpleAck : IBacNetApdu
    {
        public byte InvokeId { get; set; }
        public byte ServiceChoise { get; set; }

        public SimpleAck()
        {}

        public SimpleAck(byte[] apdu)
        {
            if (apdu.Length > 2)
            {
                InvokeId = apdu[1];
                ServiceChoise = apdu[2];
            }
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_SIMPLE_ACK);
            res.Add(InvokeId);
            res.Add(ServiceChoise);

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
