using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BACSharp.NPDU;
using BACSharp.Network;
using BACSharp.Services;
using BACSharp.Services.Acknowledgement;
using BACSharp.Services.Confirmed;
using BACSharp.Services.Unconfirmed;

namespace BACSharp
{
    public class BacNetServices
    {
        public ConfirmedServices Confirmed { get; private set; }
        public UnconfirmedServices Unconfirmed { get; private set; }
        public AckServices Acknowledgement { get; private set; }

        public BacNetServices()
        {
            Initialize();
        }

        private void Initialize()
        {
            Confirmed = new ConfirmedServices();
            Unconfirmed = new UnconfirmedServices();
            Acknowledgement = new AckServices();
        }       

        internal void Execute(IBacNetNpdu npdu, IBacNetApdu apdu, IPEndPoint endPoint = null, int timeOut = 0)
        {
            byte[] _npdu = npdu.GetBytes();
            byte[] _apdu = apdu.GetBytes();

            List<byte> bacNetMessage = new List<byte>();
            bacNetMessage.AddRange(_npdu);
            bacNetMessage.AddRange(_apdu);
            bacNetMessage[3] = (byte)bacNetMessage.Count;

            BacNetDevice.Instance.Network.Send(bacNetMessage.ToArray(), endPoint);
        }
    }
}
