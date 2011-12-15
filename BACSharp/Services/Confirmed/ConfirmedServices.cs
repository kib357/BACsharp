using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using BACSharp.APDU;
using BACSharp.NPDU;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    public class ConfirmedServices : IBacNetServiceList
    {
        public ArrayList ReadProperty(UInt16 destinationAddress, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId)
        {
            var apdu = new ReadProperty(bacNetObject, propertyId);
            var npdu = new BacNetIpNpdu();
            npdu.ExpectingReply = true;
            BacNetRemoteDevice remote = null;
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.RemoteDevices)
                if (remoteDevice.InstanceNumber == destinationAddress)
                    remote = remoteDevice;
            if (remote != null)
            {
                npdu.Destination = remote.BacAddress;
                BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
            BacNetDevice.Instance.Waiter = apdu.InvokeId;
                return WaitForResponce(apdu.InvokeId);
            }
            return null;
        }

        public ArrayList Rpm(UInt16 destinationAddress, ArrayList objectList, ArrayList propertiesList)
        {
            var apdu = new ReadPropertyMultiple(objectList, propertiesList);
            var npdu = new BacNetIpNpdu();
            npdu.ExpectingReply = true;
            BacNetRemoteDevice remote = null;
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.RemoteDevices)
                if (remoteDevice.InstanceNumber == destinationAddress)
                    remote = remoteDevice;
            if (remote != null)
            {
                npdu.Destination = remote.BacAddress;
                BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
                BacNetDevice.Instance.Waiter = apdu.InvokeId;
                return WaitForResponce(apdu.InvokeId);
            }
            return null;
        }

        public void WriteProperty(UInt16 destinationAddress, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId, ArrayList valueList)
        {
            var apdu = new WriteProperty(bacNetObject, propertyId, valueList);
            var npdu = new BacNetIpNpdu();
            npdu.ExpectingReply = true;
            IPEndPoint endPoint = null;
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.RemoteDevices)
            {
                if (remoteDevice.InstanceNumber == destinationAddress)
                {
                    npdu.Destination = remoteDevice.BacAddress;
                    endPoint = remoteDevice.EndPoint;
                }
            }
            BacNetDevice.Instance.Services.Execute(npdu, apdu, endPoint);
            WaitForResponce(apdu.InvokeId);
        }

        public ArrayList WaitForResponce(int invokeId, int timeOut = 1000)
        {            
            BacNetDevice.Instance.Waiter = invokeId;
            int sleep = 5, time = 0;
            while (BacNetDevice.Instance.Waiter is int && Convert.ToInt32(BacNetDevice.Instance.Waiter) == invokeId)
            {
                Thread.Sleep(sleep);
                time += sleep;
                if (time >= timeOut) break;
            }
            if (BacNetDevice.Instance.Waiter is ArrayList)
                return BacNetDevice.Instance.Waiter as ArrayList;
            return null;
        }
    }
}
