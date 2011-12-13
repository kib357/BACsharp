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
            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties p = f.GetIPProperties();
                    foreach (var s in p.UnicastAddresses)
                    {
                        if (s.IPv4Mask != null && s.IPv4Mask.ToString() != "0.0.0.0")
                        {
                            comboBox1.Items.Add(s.Address + " " + s.IPv4Mask);
                        }
                    }
                }
        }

        private void whoIsButton_Click(object sender, EventArgs e)
        {
            _device.Services.Unconfirmed.WhoIs(0,1000);
            Thread.Sleep(1000);
            foreach (var remoteDevice in _device.RemoteDevices)
            {
                BacNetRemoteDevice rm = remoteDevice as BacNetRemoteDevice;
                if (rm == null) continue;
                listBox1.Items.Add(rm.InstanceNumber);
            }
        }

        private void readPropertyButton_Click(object sender, EventArgs e)
        {
            _device.Services.Confirmed.ReadProperty(502, new BacNetObject { ObjectId = 2, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE);
        }

        private void writePropertyButton_Click(object sender, EventArgs e)
        {
            ArrayList values = new ArrayList();
            values.Add(new BacNetReal {Value = (float)22.5});
            _device.Services.Confirmed.WriteProperty(1769, new BacNetObject { ObjectId = 200, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedIndex > 0 && listBox1.SelectedItem is uint)
            {
                ArrayList values;
                ushort instance = Convert.ToUInt16(listBox1.SelectedItem);
                values = _device.Services.Confirmed.ReadProperty(instance, new BacNetObject { ObjectId = instance, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE }, BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_LIST);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        BacNetObject obj = value as BacNetObject;
                        if (obj == null) continue;
                        listBox2.Items.Add(obj);
                    }
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            if ((listBox1.SelectedIndex >= 0 && listBox1.SelectedItem is uint) && (listBox2.SelectedIndex >= 0 && listBox2.SelectedItem is BacNetObject))
            {
                ArrayList values;
                values = _device.Services.Confirmed.ReadProperty(Convert.ToUInt16(listBox1.SelectedItem), listBox2.SelectedItem as BacNetObject, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        textBox1.Text = value.ToString();
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
