﻿using System;
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
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<int, ReadPropertyMultiple> _rpmPool;
        private Dictionary<int, WriteProperty> _writePropertyPool;

        public ConfirmedServices()
        {
            _rpmPool = new Dictionary<int, ReadPropertyMultiple>();
            _writePropertyPool = new Dictionary<int, WriteProperty>();
        }

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
            {
                _logger.Warn("Wrong address: " + address);
                return null;
            }

            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(addrArray[0]));
            if (remote == null)
            {
                return null;
            }

            BacNetObject tmpObj;
            try
            {
                tmpObj = new BacNetObject(addrArray[1]);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return new BacNetProperty
                                         {
                                             PropertyId = new BacNetUInt{ Value = (uint)propId},
                                             Values = new ArrayList {new BacNetString("Error")}
                                         };
                //return null;
            }

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
                property.Values = valueList ?? new ArrayList();
            else
            {
                property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)propId }, Values = valueList ?? new ArrayList() };
                obj.Properties.Add(property);
            }                                      
            return property;
        }

        public bool? SubscribeCOV(string address, BacNetEnums.BACNET_PROPERTY_ID propId = BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)
        {
            string[] addrArray = address.Split('.');
            if (addrArray.Length != 2)
            {
                _logger.Warn("Wrong address: " + address);
                return null;
            }

            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(addrArray[0]));
            if (remote == null)
            {
                _logger.Warn("No such device in network. Device number: " + addrArray[0]);
                return null;
            }

            BacNetObject tmpObj;
            try
            {
                tmpObj = new BacNetObject(addrArray[1]);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return null;
            }

            BacNetObject obj = remote.Objects.FirstOrDefault(s => s.ObjectId == tmpObj.ObjectId && s.ObjectType == tmpObj.ObjectType);
            if (obj == null)
            {
                remote.Objects.Add(tmpObj);
                obj = tmpObj;
            }
            var apdu = new SubscribeCOV(obj) { ProccessId = new BacNetUInt(5556) };
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            BacNetDevice.Instance.Waiter = apdu.InvokeId;
            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
            ArrayList valueList = WaitForResponce(apdu.InvokeId) as ArrayList;

            /*BacNetProperty property = obj.Properties.FirstOrDefault(s => s.PropertyId.Value == (uint)propId);
            if (property != null)
                property.Values = valueList ?? new ArrayList();
            else
            {
                property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)propId }, Values = valueList ?? new ArrayList() };
                obj.Properties.Add(property);
            }
            return property;*/
            return true;
        }

        public List<BacNetObject> Rpm(uint instanceId, List<BacNetObject> objectList)
        {
            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString()));
            if (remote == null)
            {
                return new List<BacNetObject>();
            }          

            var apdu = new ReadPropertyMultiple(objectList);
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            BacNetDevice.Instance.Waiter = apdu.InvokeId;
            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);            
            objectList = WaitForResponce(apdu.InvokeId) as List<BacNetObject>;
            return objectList ?? new List<BacNetObject>();
        }

        public delegate void RpmEDelegate(uint deviceId, List<BacNetObject> objects);
        public void RpmE(uint instanceId, List<BacNetObject> objectList, RpmEDelegate callBack)
        {
            BacNetRemoteDevice remote = BacNetDevice.Instance.SearchRemote(BacNetRemoteDevice.Get(instanceId.ToString()));
            if (remote == null)
            {
                return;
            }

            var apdu = new ReadPropertyMultiple(objectList);
            apdu.CallBack = callBack;
            apdu.InstanceId = instanceId;
            var npdu = new BacNetIpNpdu { ExpectingReply = true, Destination = remote.BacAddress };

            lock (_rpmPool)
            {
                if (_rpmPool.ContainsKey(apdu.InvokeId))
                {
                    _rpmPool[apdu.InvokeId].CallBack(_rpmPool[apdu.InvokeId].InstanceId, null);
                    _rpmPool.Remove(apdu.InvokeId);
                }
                _rpmPool.Add(apdu.InvokeId, apdu);
            }
            
            BacNetDevice.Instance.Services.Execute(npdu, apdu, remote.EndPoint);
        }

        public void RpmCallBack(int invokeID, List<BacNetObject> objects)
        {
            lock (_rpmPool)
            {
                if (_rpmPool.ContainsKey(invokeID))
                {
                    _rpmPool[invokeID].CallBack(_rpmPool[invokeID].InstanceId, objects);
                    _rpmPool.Remove(invokeID);
                }
            }
        }

        public delegate void WritePropertyDelegate(uint deviceId, BacNetObject objectId, string status);
        public void WriteProperty(uint instanceId, BacNetObject bacNetObject, BacNetEnums.BACNET_PROPERTY_ID propertyId, ArrayList valueList, WritePropertyDelegate callBack = null)
        {
            var apdu = new WriteProperty(bacNetObject, propertyId, valueList);
            apdu.InstanceId = instanceId;
            apdu.CallBack = callBack;         
            var npdu = new BacNetIpNpdu();
            npdu.ExpectingReply = true;
            IPEndPoint endPoint = null;
            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.Remote)
            {
                if (remoteDevice.InstanceNumber == instanceId)
                {
                    npdu.Destination = remoteDevice.BacAddress;
                    endPoint = remoteDevice.EndPoint;
                }
            }

            lock (_writePropertyPool)
            {
                if (_writePropertyPool.ContainsKey(apdu.InvokeId))
                {
                    _writePropertyPool[apdu.InvokeId].CallBack(_writePropertyPool[apdu.InvokeId].InstanceId, null, null);
                    _writePropertyPool.Remove(apdu.InvokeId);
                }
                _writePropertyPool.Add(apdu.InvokeId, apdu);
            }

            BacNetDevice.Instance.Services.Execute(npdu, apdu, endPoint);
        }

        public void WritePropertyCallBack(int invokeID, string status)
        {
            lock (_writePropertyPool)
            {
                if (_writePropertyPool.ContainsKey(invokeID))
                {
                    if (_writePropertyPool[invokeID].CallBack != null)
                        _writePropertyPool[invokeID].CallBack(_writePropertyPool[invokeID].InstanceId, _writePropertyPool[invokeID].ObjectId, status);
                    _writePropertyPool.Remove(invokeID);
                }
            }
        }

        public object WaitForResponce(int invokeId, int timeOut = 1000)
        {            
            BacNetDevice.Instance.Waiter = invokeId;
            int sleep = 5, time = 0;
            while (BacNetDevice.Instance.Waiter is int)
            {
                try
                {
                    if (invokeId == Convert.ToInt32(BacNetDevice.Instance.Waiter))
                    {
                        Thread.Sleep(sleep);
                        time += sleep;
                        if (time >= timeOut) break;
                    }
                }
                catch
                {
                    break;
                }                
            }
            return BacNetDevice.Instance.Waiter;           
        }

        public BacNetObject CreateObject(uint instanceId, BacNetObject bacNetObject) 
        {
            var npdu = new BacNetIpNpdu();
            var apdu = new CreateObject(bacNetObject);
            IPEndPoint endPoint = null;

            foreach (BacNetRemoteDevice remoteDevice in BacNetDevice.Instance.Remote)
            {
                if (remoteDevice.InstanceNumber == instanceId)
                {
                    npdu.Destination = remoteDevice.BacAddress;
                    endPoint = remoteDevice.EndPoint;
                    break;
                }
            }

            BacNetDevice.Instance.Services.Execute(npdu, apdu, endPoint);

            return apdu.NewObject;
        }

        public void DeletObject(uint instanceId, uint objectId) 
        {
            foreach (BacNetRemoteDevice remoteDivice in BacNetDevice.Instance.Remote) 
            {
                if (remoteDivice.InstanceNumber == instanceId) 
                {
                    foreach (var bacNetObject in remoteDivice.Objects) 
                    {
                        if (bacNetObject.ObjectId == objectId) 
                        {
                            var npdu = new BacNetIpNpdu();
                            npdu.Destination = remoteDivice.BacAddress;

                            var apdu = new DeleteObject(bacNetObject);

                            BacNetDevice.Instance.Services.Execute(npdu, apdu, remoteDivice.EndPoint);

                            return;
                        }
                    }
                }
            }
        }
      
    }
}
