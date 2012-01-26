using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BACSharp;
using BACSharp.Types;

namespace TemplateApp
{
    public class LampGroup
    {
        private Task _sg;
        private List<string> _addresses;
        private BacNetDevice _device;
        private volatile bool go;

        public LampGroup(List<string> addresses, BacNetDevice device)
        {
            go = true;
            _addresses = addresses;
            _device = device;
            _sg = new Task(StartGroup);
        }

        public void Start()
        {
            _sg.Start();
        }

        public void Stop()
        {
            go = false;
        }

        private void StartGroup()
        {
            ArrayList values = new ArrayList();
            values.Add(new BacNetReal { Value = 1 });

            while (true)
            {
                if (!go) return;

                values[0] = new BacNetReal { Value = 100 };
                for (int i = 0; i < _addresses.Count; i++)
                {
                    Thread.Sleep(1000);
                    string[] tmpAddr = _addresses[i].Split('.');
                    if (tmpAddr.Length == 2)
                    {
                        ushort dev = Convert.ToUInt16(tmpAddr[0]);
                        uint num = Convert.ToUInt32(tmpAddr[1]);
                        _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                    }
                }
                values[0] = new BacNetReal { Value = 1 };
                Thread.Sleep(1000);
                for (int i = (_addresses.Count - 1); i >= 0; i--)
                {
                    Thread.Sleep(1000);
                    string[] tmpAddr = _addresses[i].Split('.');
                    if (tmpAddr.Length == 2)
                    {
                        ushort dev = Convert.ToUInt16(tmpAddr[0]);
                        uint num = Convert.ToUInt32(tmpAddr[1]);
                        _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
