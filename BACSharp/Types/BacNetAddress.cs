using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetAddress
    {
        public BacNetAddress()
        {
            Network = new byte[2];
            Address = new byte[0];
            HopCount = 255;
        }

        private byte[] _network;
        public byte[] Network
        {
            get { return _network; } 
            set
            {
                if (_network != value && value != null && value.Length == 2)
                    _network = value;
            } 
        }

        public byte AddressLength { get { return (byte)Address.Length; } }
        public byte[] Address { get; set; }
        public byte HopCount { get; set; }
    }
}
