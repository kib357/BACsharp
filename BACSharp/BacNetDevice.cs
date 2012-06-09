using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BACSharp.Network;
using BACSharp.Services.Unconfirmed;
using BACSharp.Types;

namespace BACSharp
{
    public delegate void NotificationEventHandler(UnconfirmedEventNotification notification);

    public sealed class BacNetDevice
    {
        #region Fields

        private static volatile BacNetDevice _instance;
        internal volatile bool Listen;
        internal volatile object Waiter;
        internal volatile Dictionary<int, object> WaitList; 
        private static readonly object SyncRoot = new Object();
                    
        #endregion

        #region Properties

        public event NotificationEventHandler NotificationEvent;

        public void OnNotificationEvent(UnconfirmedEventNotification notification)
        {
            NotificationEventHandler handler = NotificationEvent;
            if (handler != null) handler(notification);
        }

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
        internal BacNetListener Listener;
        internal BacNetResponse Response;
        public List<BacNetRemoteDevice> Remote { get; internal set; }
        private readonly Dictionary<uint, DateTime> LostDevices;

        #endregion

        private BacNetDevice()
        {
            Remote = new List<BacNetRemoteDevice>();
            Services = new BacNetServices();
            Response = new BacNetResponse();
            WaitList = new Dictionary<int, object>();
            LostDevices = new Dictionary<uint, DateTime>();
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
            if (LostDevices.ContainsKey(device.InstanceNumber))
            {
                if (DateTime.Now - LostDevices[device.InstanceNumber] < new TimeSpan(0, 0, 10))
                    return null;
                LostDevices.Remove(device.InstanceNumber);
            }
                
            BacNetRemoteDevice rem = Instance.Remote.FirstOrDefault(s => s.InstanceNumber == device.InstanceNumber);
            if (rem == null)
            {
                Instance.Services.Unconfirmed.WhoIs((ushort)device.InstanceNumber, (ushort)device.InstanceNumber, 500);
                Thread.Sleep(500);
                rem = Instance.Remote.FirstOrDefault(s => s.InstanceNumber == device.InstanceNumber);
            }
            if (rem == null && !LostDevices.ContainsKey(device.InstanceNumber))
                LostDevices.Add(device.InstanceNumber, DateTime.Now);
            return rem;
        }
    }
}
