using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BACSharp.Types;

namespace BACSharp.Network
{
    public class BacNetIpNetwork : IBacNetNetwork
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly object SyncRoot = new Object();

        public BacNetIpNetwork(IPAddress address, IPAddress mask,  int udpport = 47808)
        {
            byte[] addressBytes = address.GetAddressBytes();
            byte[] maskBytes = mask.GetAddressBytes();
            byte[] broadcastBytes = new byte[4];
            for (int i = 0; i < addressBytes.Length; i++)
            {
                broadcastBytes[i] = (byte)(addressBytes[i] | (255 - maskBytes[i]));
            }
            Address = address;
            UdpPort = udpport;
            Broadcast = new IPAddress(broadcastBytes);

            if (BacNetDevice.Instance.Listener != null)
            {
                BacNetDevice.Instance.Remote.Clear();
                BacNetDevice.Instance.Listen = false;
                Send(new byte[1], new IPEndPoint(Broadcast, UdpPort));
                Thread.Sleep(100);
            }

            BacNetDevice.Instance.Listener = new BacNetListener(Address);
        }

        private IPAddress Address { get; set; }
        private IPAddress Broadcast { get; set; }

        private int UdpPort { get; set; }

        public void Send(byte[] message, IPEndPoint endPoint = null)
        {
            lock (SyncRoot)
            {
                var udpSendClient = new UdpClient() {EnableBroadcast = true};
                udpSendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                try
                {
                    udpSendClient.Client.Bind(new IPEndPoint(Address, UdpPort));
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Bind error", ex);
                    return;
                }


                if (endPoint == null)
                    udpSendClient.Connect(Broadcast, UdpPort);
                else
                    udpSendClient.Connect(endPoint.Address, endPoint.Port);


                udpSendClient.Send(message, message.Length);

                udpSendClient.Close();

                /*var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var ep = new IPEndPoint(Broadcast, 47808);

                s.SendTo(message, ep);
                s.Close();*/
            }
        }

        private void Completed(IAsyncResult ar)
        {
            //throw new NotImplementedException();
        }
    }
}
