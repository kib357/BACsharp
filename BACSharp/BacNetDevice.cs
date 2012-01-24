using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BACSharp.Network;
using BACSharp.Types;

namespace BACSharp
{
    public sealed class BacNetDevice
    {
        #region Fields

        private static volatile BacNetDevice _instance;
        internal volatile bool Listen;
        internal volatile object Waiter;
        private static readonly object SyncRoot = new Object();
                    
        #endregion

        #region Properties

        public const int VendorId = 500;
        private byte _invokeId = 0;
        public byte InvokeId
        {
            get 
            {               
                return _invokeId++;
            }           
        }
        public uint DeviceId { get; set; }
        public int MaxAPDULength { get; set; }
        public bool SegmentationSupported { get; set; }

        public IBacNetNetwork Network;
        public BacNetServices Services;
        public BacNetListener Listener;
        public BacNetResponse Response;
        public List<BacNetRemoteDevice> Remote { get; internal set; } 

        #endregion

        private BacNetDevice()
        {
            Remote = new List<BacNetRemoteDevice>();
            Services = new BacNetServices();
            Response = new BacNetResponse();
        }

        public static BacNetDevice Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new BacNetDevice();
                    }
                }

                return _instance;
            }
        }

        public static void Reset()
        {
            if (_instance != null)
            {
                lock (SyncRoot)
                {
                    _instance = null;
                }
            }
        }

        internal BacNetRemoteDevice SearchRemote(BacNetRemoteDevice device)
        {
            BacNetRemoteDevice rem =
               Instance.Remote.FirstOrDefault(s => s.InstanceNumber == device.InstanceNumber);
            if (rem == null)
            {
                Instance.Services.Unconfirmed.WhoIs((ushort)device.InstanceNumber, (ushort)device.InstanceNumber, 500);
                Thread.Sleep(500);
                rem = Instance.Remote.FirstOrDefault(s => s.InstanceNumber == device.InstanceNumber);
            }
            return rem;
        }
    }
}
