using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using BACSharp.NPDU;
using BACSharp.Types;

namespace BACSharp.Services.Confirmed
{
    public class ConfirmedServices
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void ReadProperty(UInt16 instanceId, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId)
        {
            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString(CultureInfo.InvariantCulture)));
            if (remote == null) return;

            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };
            var apdu = new ReadProperty(bacNetObject, propertyId);

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }

        public void ReadProperty(string address, BacNetEnums.BACNET_PROPERTY_ID propId = BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)
        {
            string[] addrArray = address.Split('.');
            if (addrArray.Length != 2)
            {
                _logger.Warn("Wrong address: " + address);
                return;
            }

            BacNetObject tmpObj;
            try
            {
                tmpObj = new BacNetObject(addrArray[1]);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return;
            }

            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(addrArray[0]));
            if (remote == null) return;

            var apdu = new ReadProperty(tmpObj, propId);
            var npdu = new BacNetIpNpdu {ExpectingReply = true, Destination = remote.BacAddress};

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);                        
        }

        public void SubscribeCOV(string address, BacNetEnums.BACNET_PROPERTY_ID propId = BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)
        {
            string[] addrArray = address.Split('.');
            if (addrArray.Length != 2)
            {
                _logger.Warn("Wrong address: " + address);
                return;
            }

            BacNetObject tmpObj;
            try
            {
                tmpObj = new BacNetObject(addrArray[1]);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return;
            }

            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(addrArray[0]));
            if (remote == null) return;

            var apdu = new SubscribeCOV(tmpObj) { ProccessId = new BacNetUInt(5556) };
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }

        public void ReadPropertyMultiple(uint instanceId, List<BacNetObject> objectList)
        {
            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString(CultureInfo.InvariantCulture)));
            if (remote == null) return;

            var apdu = new ReadPropertyMultiple(objectList);
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);            
        }       

        public void WriteProperty(uint instanceId, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId, ArrayList valueList)
        {
            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString(CultureInfo.InvariantCulture)));
            if (remote == null) return;

            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };
            var apdu = new WriteProperty(bacNetObject, propertyId, valueList);
            apdu.InstanceId = instanceId;

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }

        public void CreateObject(uint instanceId, BacNetObject bacNetObject) 
        {
            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString(CultureInfo.InvariantCulture)));
            if (remote == null) return;

            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };
            var apdu = new CreateObject(bacNetObject);

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }

        public void DeleteObject(uint instanceId, BacNetObject bacNetObject) 
        {
            var remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString(CultureInfo.InvariantCulture)));
            if (remote == null) return;            

            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };
            var apdu = new DeleteObject(bacNetObject);

            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }
      
    }
}
