using System;
using System.Collections;
using System.Net;
using BACSharp.NPDU;
using BACSharp.Services.Acknowledgement;
using BACSharp.Services.Confirmed;
using BACSharp.Services.Unconfirmed;
using BACSharp.Types;

namespace BACSharp
{
    public class BacNetResponse
    {
        public BacNetResponse()
        {    
        }

        #region Unconfirmed

        public void ReceivedIAm(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            BacNetRemoteDevice device = new BacNetRemoteDevice();
            device.EndPoint = endPoint;
            BacNetIpNpdu npdu = new BacNetIpNpdu(msg.Npdu);
            IAm apdu = new IAm(msg.Apdu);
            if (npdu.Source != null)
                device.BacAddress = npdu.Source;
            device.MaxApduLength = apdu.MaxApduLength;
            device.InstanceNumber = apdu.deviceObject.ObjectId;            
            device.Segmentation = apdu.SegmentationSupported;
            device.VendorId = apdu.VendorId;

            if (device.InstanceNumber == BacNetDevice.Instance.DeviceId)
                return;

            for (int i = 0; i < BacNetDevice.Instance.RemoteDevices.Count;i++ )
            {
                BacNetRemoteDevice remoteDevice = BacNetDevice.Instance.RemoteDevices[i] as BacNetRemoteDevice;
                if (remoteDevice == null) continue;
                if (remoteDevice.InstanceNumber == device.InstanceNumber)
                {
                    BacNetDevice.Instance.RemoteDevices[i] = device;
                    return;
                }
            }
            BacNetDevice.Instance.RemoteDevices.Add(device);
        }

        public void ReceivedIHave(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }

        public void ReceivedCovNotification(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }

        public void ReceivedEventNotification(BacNetRawMessage msg)
        {
            //throw new NotImplementedException();
        }

        public void ReceivedPrivateTransfer(BacNetRawMessage msg)
        {
            //throw new NotImplementedException();
        }

        public void ReceivedTextMessage(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }

        public void ReceivedTimeSynchronization(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }


        public void ReceivedWhoHas(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }

        public void ReceivedWhoIs(BacNetRawMessage msg)
        {
            WhoIs apdu = new WhoIs(msg.Apdu);
            uint devId = BacNetDevice.Instance.DeviceId;
            if ((apdu.LowLimit != null && apdu.HighLimit != null && apdu.LowLimit.Value < devId && apdu.HighLimit.Value > devId) || (apdu.LowLimit == null || apdu.HighLimit == null))
                BacNetDevice.Instance.Services.Unconfirmed.IAm();
        }

        public void ReceivedUtcTimeSynchronization(BacNetRawMessage msg)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Confirmed

        public void ReceivedReadProperty(BacNetRawMessage msg)
        {
            try
            {
                ReadProperty apdu = new ReadProperty(msg.Apdu);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Reject.Missing_required_paramter")
                {
                    //Отправляем сообщение об ошибке
                }
                throw;
            }            
        }

        #endregion

        #region Acknowledgement

        public void ReceivedReadPropertyAck(BacNetRawMessage msg)
        {
            ReadPropertyAck apdu = new ReadPropertyAck(msg.Apdu);
            if (BacNetDevice.Instance.Waiter is int && Convert.ToInt32(BacNetDevice.Instance.Waiter) == apdu.InvokeId)
                BacNetDevice.Instance.Waiter = apdu.Obj.Properties[0].Values;
        }

        public void ReceivedErrorAck(BacNetRawMessage msg)
        {
            ErrorAck apdu = new ErrorAck(msg.Apdu);
            ArrayList res = new ArrayList();
            res.Add(apdu.ErrorCode);
            if (BacNetDevice.Instance.Waiter is int && Convert.ToInt32(BacNetDevice.Instance.Waiter) == apdu.InvokeId)
                BacNetDevice.Instance.Waiter = res;
        }

        #endregion
    }
}
