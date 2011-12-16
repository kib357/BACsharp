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
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.Remote)
                if (remoteDevice.InstanceNumber == destinationAddress)
                    remote = remoteDevice;
            if (remote != null)
            {
                npdu.Destination = remote.BacAddress;
                BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
            BacNetDevice.Instance.Waiter = apdu.InvokeId;
                return WaitForResponce(apdu.InvokeId) as ArrayList;
            }
            return null;
        }

        public BacNetProperty ReadProperty(string address, BacNetEnums.BACNET_PROPERTY_ID propId = BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)
        {
            string[] addrArray = address.Split('.');
            if (addrArray.Length != 2)
                throw new Exception("Wrong address");

            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(addrArray[0]));
            if (remote == null)
                throw new Exception("No such device in network");
            BacNetObject tmpObj = BacNetObject.Get(addrArray[1]);
            BacNetObject obj = remote.Objects.FirstOrDefault(s => s.ObjectId == tmpObj.ObjectId && s.ObjectType == tmpObj.ObjectType);
            if (obj == null)
            {
                remote.Objects.Add(tmpObj);
                obj = tmpObj;
            }
            var apdu = new ReadProperty(obj, propId);
            var npdu = new BacNetIpNpdu {ExpectingReply = true, Destination = remote.BacAddress};

            BacNetDevice.Instance.Waiter = apdu.InvokeId;
            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);            
            ArrayList valueList = WaitForResponce(apdu.InvokeId) as ArrayList;

            BacNetProperty property = obj.Properties.FirstOrDefault(s => s.PropertyId.Value == (uint) propId);
            if (property != null)
                property.Values = valueList;
            else
            {
                property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)propId }, Values = valueList };
                obj.Properties.Add(property);
            }                                      
            return property;
        }

        public List<BacNetObject> Rpm(uint instanceId, List<BacNetObject> objectList)
        {
            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString()));
            if (remote == null)
                throw new Exception("No such device in network");

            /*foreach (var tmpObj in objectList)
            {
                BacNetObject obj = remote.Objects.FirstOrDefault(s => s.ObjectId == tmpObj.ObjectId && s.ObjectType == tmpObj.ObjectType);
                if (obj == null)
                {
                    remote.Objects.Add(tmpObj);
                }
            }*/

            var apdu = new ReadPropertyMultiple(objectList);
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            BacNetDevice.Instance.Waiter = apdu.InvokeId;
            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);            
            objectList = WaitForResponce(apdu.InvokeId) as List<BacNetObject>;
            return objectList;
        }

        public void WriteProperty(UInt16 destinationAddress, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId, ArrayList valueList)
        {
            var apdu = new WriteProperty(bacNetObject, propertyId, valueList);
            var npdu = new BacNetIpNpdu();
            npdu.ExpectingReply = true;
            IPEndPoint endPoint = null;
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.Remote)
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

        public object WaitForResponce(int invokeId, int timeOut = 1000)
        {            
            BacNetDevice.Instance.Waiter = invokeId;
            int sleep = 5, time = 0;
            while (BacNetDevice.Instance.Waiter is int && Convert.ToInt32(BacNetDevice.Instance.Waiter) == invokeId)
            {
                Thread.Sleep(sleep);
                time += sleep;
                if (time >= timeOut) break;
            }
            return BacNetDevice.Instance.Waiter;           
        }
    }
}
