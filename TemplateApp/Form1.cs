using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BACSharp;
using BACSharp.Network;
using BACSharp.Services.Confirmed;
using BACSharp.Types;

namespace TemplateApp
{
    public partial class Form1 : Form
    {
        private BacNetDevice _device;

        public Form1()
        {
            InitializeComponent();
            _device = BacNetDevice.Instance;
            _device.DeviceId = 357;
            ArrayList adresses = new ArrayList();
            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties p = f.GetIPProperties();
                    foreach (var s in p.UnicastAddresses)
                    {
                        if (s.IPv4Mask != null && s.IPv4Mask.ToString() != "0.0.0.0")
                        {
                            comboBox1.Items.Add(s.Address + " " + s.IPv4Mask);
                            adresses.Add(s.Address + " " + s.IPv4Mask);
                        }
                    }
                }
            comboBox1.SelectedItem = adresses[0];
        }

        private void whoIsButton_Click(object sender, EventArgs e)
        {
            _device.Services.Unconfirmed.WhoIs();
            Thread.Sleep(1000);
            foreach (var remoteDevice in _device.Remote)
            {
                BacNetRemoteDevice rm = remoteDevice as BacNetRemoteDevice;
                if (rm == null) continue;
                listBox1.Items.Add(rm.InstanceNumber);
            }
        }

        private void readPropertyButton_Click(object sender, EventArgs e)
        {
            /*ArrayList objectList = new ArrayList();
            ArrayList propertyList = new ArrayList();
            BacNetObject obj = new BacNetObject { ObjectId = 2, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE };
            objectList.Add(obj);
            BacNetProperty property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE }, Values = new ArrayList() };
            propertyList.Add(property);
            _device.Services.Confirmed.Rpm(502, objectList, propertyList);*/
            var property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE } };
            var obj = new BacNetObject { ObjectId = 112, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT };
            var objList = new List<BacNetObject> {obj};
            objList[0].Properties.Add(property);
            objList.Add(new BacNetObject { ObjectId = 212, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 1212, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 1, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 999, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 1000, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 104, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 108, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 109, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 111, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 204, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 208, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 101, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 103, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT, Properties = new List<BacNetProperty> { property } });
            objList.Add(new BacNetObject { ObjectId = 104, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT, Properties = new List<BacNetProperty> { property } });
            _device.Services.Confirmed.Rpm(100, objList);
        }

        private void writePropertyButton_Click(object sender, EventArgs e)
        {
            ArrayList values = new ArrayList();
            values.Add(new BacNetReal {Value = (float)10});
            _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = 276, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedIndex > 0 && listBox1.SelectedItem is uint)
            {
                ushort instance = Convert.ToUInt16(listBox1.SelectedItem);
                BacNetProperty property = _device.Services.Confirmed.ReadProperty(instance + ".DEV" + instance, BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_LIST);
                if (property != null)
                {
                    foreach (var value in property.Values)
                    {
                        BacNetObject obj = value as BacNetObject;
                        if (obj == null) continue;
                        listBox2.Items.Add(obj);
                    }
                }
                var objList = new List<BacNetObject>();
                var nameProperty = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_NAME } };
                var valueProperty = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE } };
                for (int i = 0; i < listBox2.Items.Count && i < 30;i++)
                {
                    objList.Add(listBox2.Items[i] as BacNetObject);
                    objList[objList.Count - 1].Properties = new List<BacNetProperty> {nameProperty, valueProperty};
                }
                List<BacNetObject> objectsWithValues = _device.Services.Confirmed.Rpm(instance, objList);
                listBox3.Items.Clear();
                foreach (var objectWithValues in objectsWithValues)
                {
                    if (objectWithValues != null)
                        listBox3.Items.Add(objectWithValues);
                }
            }            
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            if ((listBox1.SelectedIndex >= 0 && listBox1.SelectedItem is uint) && (listBox2.SelectedIndex >= 0 && listBox2.SelectedItem is BacNetObject))
            {
                BacNetProperty property;
                property = _device.Services.Confirmed.ReadProperty(listBox1.SelectedItem + "." + (listBox2.SelectedItem as BacNetObject).GetStringId());
                if (property != null)
                {
                    foreach (var value in property.Values)
                    {
                        textBox1.Text = value.ToString();
                    }
                }
                property = _device.Services.Confirmed.ReadProperty(listBox1.SelectedItem + "." + (listBox2.SelectedItem as BacNetObject).GetStringId(), BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_NAME);
                if (property != null)
                {
                    foreach (var value in property.Values)
                    {
                        textBox2.Text = value.ToString();
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IPAddress address = IPAddress.Parse(comboBox1.SelectedItem.ToString().Split(' ')[0]);
            IPAddress mask = IPAddress.Parse(comboBox1.SelectedItem.ToString().Split(' ')[1]);
            _device.Network = new BacNetIpNetwork(address, mask);
        }
    }
}
