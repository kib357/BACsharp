using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Services.Acknowledgement
{
    public class SegmentAck : IBacNetApdu
    {
        public byte InvokeId { get; set; }
        public byte SequenceNumber { get; set; }
        public byte PropWindowSize { get; set; }

        public SegmentAck()
        {}

        public SegmentAck(byte[] apdu)
        {
            if (apdu.Length > 1)
                InvokeId = apdu[1];
        }

        public byte[] GetBytes()
        {
            ArrayList res = new ArrayList();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_SEGMENT_ACK);
            res.Add(InvokeId);
            res.Add(SequenceNumber);
            res.Add(PropWindowSize);

            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
