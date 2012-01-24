using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BACSharp.NPDU;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    public class AckServices
    {
        public AckServices()
        {
        }

        public void SimpleAck(byte invokeId, byte serviceChoice)
        {
            var apdu = new SimpleAck() { InvokeId = invokeId, ServiceChoise = serviceChoice };
            var npdu = new BacNetIpNpdu();
            //npdu.Destination = new BacNetAddress();
            //npdu.Destination.Network = ByteConverter.GetBytes((ushort)65535);

            BacNetDevice.Instance.Services.Execute(npdu, apdu);
            //WaitForResponce();
        }

        public void SegmentAck(IPEndPoint endPoint, BacNetAddress source, byte invokeId, byte sequenceNumber, byte propWindowSize)
        {
            var apdu = new SegmentAck() { InvokeId = invokeId, SequenceNumber = sequenceNumber, PropWindowSize = propWindowSize };
            var npdu = new BacNetIpNpdu();
            npdu.Destination = source;
            //npdu.Destination = new BacNetAddress();
            //npdu.Destination.Network = ByteConverter.GetBytes((ushort)65535);

            BacNetDevice.Instance.Services.Execute(npdu, apdu, endPoint);
            //WaitForResponce();
        }
    }
}
