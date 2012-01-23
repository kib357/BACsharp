using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BACSharp.NPDU;
using BACSharp.Network;
using BACSharp.Types;

namespace BACSharp
{
    public class BacNetListener
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private UdpClient _udpReceiveClient;        
        private readonly int _udpPort;
        private ArrayList messagePool;
        //private object _receiveState;

        public BacNetListener(int udpport = 47808)
        {
            _udpPort = udpport;
            BacNetDevice.Instance.Listen = true;
            messagePool = new ArrayList();
            Thread listener = new Thread(DoListen) {IsBackground = true};
            listener.Start();
        }

        private void DoListen()
        {
            _udpReceiveClient = new UdpClient(_udpPort, AddressFamily.InterNetwork);
            IPAddress mcAddress = IPAddress.Parse("224.0.0.1");
            _udpReceiveClient.JoinMulticastGroup(mcAddress);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, _udpPort);
            while (BacNetDevice.Instance.Listen)
            {
                byte[] bytes = new byte[0];
                try
                {
                    bytes = _udpReceiveClient.Receive(ref groupEP);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                ParseBacNetMessage(bytes, groupEP);
            }
            _udpReceiveClient.Close();
        }        

        private void ParseBacNetMessage(byte[] bytes, IPEndPoint endPoint)
        {
            if (bytes[0] != BacNetEnums.BACNET_BVLC_TYPE_BIP || bytes[4] != BacNetEnums.BACNET_PROTOCOL_VERSION)
                return;

            BacNetRawMessage msg = new BacNetRawMessage();
            msg.All = bytes;
            if (msg.All == null)
            {
                _logger.Info("Malformed packet received.");
                return;
            }

            byte type = (byte)(msg.Apdu[0] >> 4);
            switch (type)
            {
                case 0:
                    ParseConfirmed(msg, endPoint);
                    break;
                case 1:
                    ParseUncofirmed(msg, endPoint);
                    break;
                case 2:
                    ParseSimpleAck(msg, endPoint);
                    break;
                case 3:
                    ParseComplexAck(msg, endPoint);
                    break;
                case 5:
                    ParseErrorAck(msg, endPoint);
                    break;
            }
        }

        //todo
        private void ParseErrorAck(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            if(msg.Apdu.Length > 2)
                switch (msg.Apdu[2])
                {
                    case 12:
                        BacNetDevice.Instance.Response.ReceivedErrorAck(msg);
                        break;
                }
        }

        private void ParseUncofirmed(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            switch (msg.Apdu[1])
            {
                case 0:
                    BacNetDevice.Instance.Response.ReceivedIAm(msg, endPoint);
                    break;
                case 1:
                    BacNetDevice.Instance.Response.ReceivedIHave(msg);
                    break;
                case 2:
                    BacNetDevice.Instance.Response.ReceivedCovNotification(msg);
                    break;
                case 3:
                    BacNetDevice.Instance.Response.ReceivedEventNotification(msg);
                    break;
                case 4:
                    BacNetDevice.Instance.Response.ReceivedPrivateTransfer(msg);
                    break;
                case 5:
                    BacNetDevice.Instance.Response.ReceivedTextMessage(msg);
                    break;
                case 6:
                    BacNetDevice.Instance.Response.ReceivedTimeSynchronization(msg);
                    break;
                case 7:
                    BacNetDevice.Instance.Response.ReceivedWhoHas(msg);
                    break;
                case 8:
                    BacNetDevice.Instance.Response.ReceivedWhoIs(msg);
                    break;
                case 9:
                    BacNetDevice.Instance.Response.ReceivedUtcTimeSynchronization(msg);
                    break;
            }
        }

        private void ParseConfirmed(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            if (msg.Apdu.Length > 3)
                switch (msg.Apdu[3])
                {
                    case 12:
                        BacNetDevice.Instance.Response.ReceivedReadProperty(msg);
                        break;
                }
        }

        private void ParseSimpleAck(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            
        }

        private void ParseComplexAck(BacNetRawMessage msg, IPEndPoint endPoint)
        {
            if (msg.Apdu.Length > 2)
            {
                //Сегментированный ответ
                if ((msg.Apdu[0] >> 3 & 1) == 1)
                {
                    /*BacNetRawMessage segmentedMsg = null;
                    foreach (object m in messagePool)
                    {
                        BacNetRawMessage ms = m as BacNetRawMessage;
                        //Если в пуле есть сообщение с таким же InvokeId как у поступившего используем его
                        if (ms != null && ms.Apdu[1] == msg.Apdu[1])
                            segmentedMsg = ms;
                    }
                    if (segmentedMsg == null) segmentedMsg = msg;*/

                    messagePool.Add(msg);
                    BacNetIpNpdu npdu = new BacNetIpNpdu(msg.Npdu);
                    BacNetDevice.Instance.Services.Acknowledgement.SegmentAck(endPoint, npdu.Source, msg.Apdu[1], msg.Apdu[2], msg.Apdu[3]);
                    //Если есть ещё сегменты
                    if ((msg.Apdu[0] >> 2 & 1) == 1)
                    {
                        return;   
                    }
                    msg = ReassembleMessage(messagePool, msg.Apdu[1]);
                }
                switch (msg.Apdu[2])
                {
                    case 12:
                        BacNetDevice.Instance.Response.ReceivedReadPropertyAck(msg);
                        break;
                    case 14:
                        BacNetDevice.Instance.Response.ReceivedReadPropertyMultipleAck(msg);
                        break;
                }
            }
        }

        private BacNetRawMessage ReassembleMessage(ArrayList messageList, byte invokeId)
        {
            bool firstMsg = true;
            ArrayList resMsg = new ArrayList();
            foreach (var message in messageList)
            {
                BacNetRawMessage msg = message as BacNetRawMessage;
                if (msg == null || msg.Apdu[1] != invokeId) continue;

                if (firstMsg)
                {
                    resMsg.AddRange(msg.Bvlc);
                    resMsg.AddRange(msg.Npdu);
                    byte[] firstApdu = new byte[msg.Apdu.Length - 2];
                    firstApdu[0] = msg.Apdu[0];
                    firstApdu[1] = msg.Apdu[1];
                    for (int i = 4;i< msg.Apdu.Length; i++)
                        firstApdu[i - 2] = msg.Apdu[i];
                    resMsg.AddRange(firstApdu);
                    firstMsg = false;
                }
                else
                {
                    byte[] anyApdu = new byte[msg.Apdu.Length - 5];
                    Array.Copy(msg.Apdu, 5, anyApdu, 0, anyApdu.Length);
                    resMsg.AddRange(anyApdu);
                }
            }
            BacNetRawMessage res = new BacNetRawMessage();
            byte[] length = ByteConverter.GetBytes((ushort)resMsg.Count);
            resMsg[2] = length[0];
            resMsg[3] = length[1];
            res.All = (byte[]) resMsg.ToArray(typeof (byte));
            return res;
        }

        private BacNetEnums.BACNET_PDU_TYPE GetPduType(byte firstByte)
        {
            byte type = (byte)(firstByte >> 4);
            switch (type)
            {
                case 0:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST;
                case 1:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST;
                case 2:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_SIMPLE_ACK;
                case 3:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_COMPLEX_ACK;
                case 4:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_SEGMENT_ACK;
                case 5:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_ERROR;
                case 6:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_REJECT;
                case 7:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_ABORT;
                default:
                    return BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_ERROR;//!!!!!!!!!!!!!!!
            }
        }
    }
}
