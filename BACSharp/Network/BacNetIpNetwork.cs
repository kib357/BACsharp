using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BACSharp.Network
{
    public class BacNetIpNetwork : IBacNetNetwork
    {
        private UdpClient _udpSendClient;

        public BacNetIpNetwork(IPAddress adress, int udpport = 47808)
        {
            Address = adress;
            UdpPort = udpport;
            
            BacNetDevice.Instance.Listener = new BacNetListener(udpport);
        }

        public IPAddress Address { get; set; }

        public int UdpPort { get; set; }

        public void Send(byte[] message, IPEndPoint endPoint = null)
        {
            _udpSendClient = new UdpClient() { EnableBroadcast = true };
            _udpSendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpSendClient.Client.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.106"), UdpPort));

            if (endPoint == null)
                _udpSendClient.Connect(Address, UdpPort);
            else 
                _udpSendClient.Connect(endPoint.Address, endPoint.Port);

            _udpSendClient.Send(message, message.Length);
            _udpSendClient.Close();
        }
    }
}
