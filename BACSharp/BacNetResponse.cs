using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BACSharp.NPDU;
using BACSharp.Services.Acknowledgement;
using BACSharp.Services.Confirmed;
using BACSharp.Services.Unconfirmed;
using BACSharp.Types;

namespace BACSharp
{
    //UnConfirmed
    public delegate void ReceivedIAmEventHandler(BacNetMessage message);
    public delegate void ReceivedCovNotificationEventHandler(BacNetMessage message);
    public delegate void ReceivedEventNotificationEventHandler(BacNetMessage message);
    public delegate void ReceivedWhoIsEventHandler(BacNetMessage message);

    //Confirmed
    public delegate void ReceivedReadPropertyEventHandler(BacNetMessage message);

    //Acknowledgement
    public delegate void ReceivedReadPropertyAckEventHandler(BacNetMessage message);
    public delegate void ReceivedReadPropertyMultipleAckEventHandler(BacNetMessage message);
    public delegate void ReceivedErrorAckEventHandler(BacNetMessage message);
    public delegate void ReceivedSimpleAckEventHandler(BacNetMessage message);

    public class BacNetResponse
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #region Unconfirmed

        public event ReceivedIAmEventHandler ReceivedIAmEvent;
        public void OnReceivedIAmEvent(BacNetMessage message)
        {
            ReceivedIAmEventHandler handler = ReceivedIAmEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedIAm(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            var newDevice = new BacNetRemoteDevice {EndPoint = endPoint};
            var npdu = new BacNetIpNpdu();
            var apdu = new IAm();
            try
            {
                npdu = new BacNetIpNpdu(msg.Npdu);
                apdu = new IAm(msg.Apdu);
                OnReceivedIAmEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed I-am: ", ex);
                return;
            }
            
            if (npdu.Source != null)
                newDevice.BacAddress = npdu.Source;
            newDevice.MaxApduLength = apdu.MaxApduLength;
            newDevice.InstanceNumber = apdu.deviceObject.ObjectId;            
            newDevice.Segmentation = apdu.SegmentationSupported;
            newDevice.VendorId = apdu.VendorId;

            if (newDevice.InstanceNumber == BacNetDevice.Instance.DeviceId)
                return;

            var rem =
                BacNetDevice.Instance.Remote.FirstOrDefault(s => s.InstanceNumber == newDevice.InstanceNumber);
            if (rem != null)
                BacNetDevice.Instance.Remote.Remove(rem);

            BacNetDevice.Instance.Remote.Add(newDevice);
        }

        public void ReceivedIHave(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        public event ReceivedCovNotificationEventHandler ReceivedCovNotificationEvent;
        public void OnReceivedCovNotificationEvent(BacNetMessage message)
        {
            var handler = ReceivedCovNotificationEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedCovNotification(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new UnconfirmedCOVnotification(msg.Apdu);
                OnReceivedCovNotificationEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed UnconfirmedCOVnotification: ", ex);
            }            
        }

        public event ReceivedEventNotificationEventHandler ReceivedEventNotificationEvent;
        public void OnReceivedEventNotificationEvent(BacNetMessage message)
        {
            var handler = ReceivedEventNotificationEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedEventNotification(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new UnconfirmedEventNotification(msg.Apdu);
                OnReceivedEventNotificationEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed UnconfirmedEventNotification: ", ex);
            }            
        }

        public void ReceivedPrivateTransfer(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        public void ReceivedTextMessage(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        public void ReceivedTimeSynchronization(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        public void ReceivedWhoHas(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        public void ReceivedWhoIs(BacNetRawMessage msg)
        {
            var apdu = new WhoIs(msg.Apdu);
            uint devId = BacNetDevice.Instance.DeviceId;
            if ((apdu.LowLimit != null && apdu.HighLimit != null && apdu.LowLimit.Value < devId && apdu.HighLimit.Value > devId) || (apdu.LowLimit == null || apdu.HighLimit == null))
                BacNetDevice.Instance.Services.Unconfirmed.IAm();
        }

        public void ReceivedUtcTimeSynchronization(BacNetRawMessage msg)
        {
            //todo: implement method
        }

        #endregion

        #region Confirmed

        public void ReceivedReadProperty(BacNetRawMessage msg)
        {
            try
            {
                var apdu = new ReadProperty(msg.Apdu);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Reject.Missing_required_paramter")
                {
                    //Отправляем сообщение об ошибке
                }
            }            
        }

        #endregion

        #region Acknowledgement

        public event ReceivedReadPropertyAckEventHandler ReceivedReadPropertyAckEvent;
        public void OnReceivedReadPropertyAckEvent(BacNetMessage message)
        {
            var handler = ReceivedReadPropertyAckEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedReadPropertyAck(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new ReadPropertyAck(msg.Apdu);              
                OnReceivedReadPropertyAckEvent(new BacNetMessage {Npdu = npdu, Apdu = apdu});
            }
            catch(Exception ex)
            {
                _logger.WarnException("Malformed ReadPropertyAck: ", ex);
            }
        }

        public event ReceivedReadPropertyMultipleAckEventHandler ReceivedReadPropertyMultipleAckEvent;
        public void OnReceivedReadPropertyMultipleAckEvent(BacNetMessage message)
        {
            var handler = ReceivedReadPropertyMultipleAckEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedReadPropertyMultipleAck(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new ReadPropertyMultipleAck(msg.Apdu);
                OnReceivedReadPropertyAckEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed ReadPropertyMultipleAck: ", ex);
            }            
        }

        public event ReceivedErrorAckEventHandler ReceivedErrorAckEvent;
        public void OnReceivederrorAckEvent(BacNetMessage message)
        {
            var handler = ReceivedErrorAckEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedErrorAck(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new ErrorAck(msg.Apdu);
                OnReceivedReadPropertyAckEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed ErrorAck: ", ex);
            }
        }

        public event ReceivedSimpleAckEventHandler ReceivedSimpleAckEvent;
        public void OnReceivedSimpleAckEvent(BacNetMessage message)
        {
            var handler = ReceivedSimpleAckEvent;
            if (handler != null) handler(message);
        }
        public void ReceivedSimpleAck(BacNetRawMessage msg)
        {
            try
            {
                var npdu = new BacNetIpNpdu(msg.Npdu);
                var apdu = new SimpleAck(msg.Apdu);
                OnReceivedReadPropertyAckEvent(new BacNetMessage { Npdu = npdu, Apdu = apdu });
            }
            catch (Exception ex)
            {
                _logger.WarnException("Malformed SimpleAck: ", ex);
            }
        }

        #endregion
    }
}
