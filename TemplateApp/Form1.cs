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
using System.Threading.Tasks;
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
            Thread.Sleep(10000);
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
            var property = new BacNetProperty { PropertyId = new BacNetUInt { Value = (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_LIST } };
            var obj = new BacNetObject { ObjectId = 500, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE };
            var objList = new List<BacNetObject> {obj};
            _device.Services.Confirmed.ReadProperty("500.DEV500", BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_LIST);
            _device.Services.Confirmed.ReadProperty("500.DEV500", BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_LIST);
            //objList[0].Properties.Add(property);
            /*objList.Add(new BacNetObject { ObjectId = 212, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT, Properties = new List<BacNetProperty> { property } });
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
            objList.Add(new BacNetObject { ObjectId = 104, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT, Properties = new List<BacNetProperty> { property } });*/
            //_device.Services.Confirmed.RpmE(17811, objList, RpmECallBack);
        }

        private void RpmECallBack(uint deviceId, List<BacNetObject> objects)
        {
            throw new NotImplementedException();
        }

        private void writePropertyButton_Click(object sender, EventArgs e)
        {
            ArrayList values = new ArrayList {new BacNetEnumeration {Value = 1}};
            _device.Services.Confirmed.WriteProperty(501, new BacNetObject { ObjectId = (uint)1999, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_VALUE }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values, OnPropertyWritten);
            /*List<int> objects = new List<int> { 276, 265, 268, 267, 258, 271 };

            List<int> col1 = new List<int> { 276, 265, 268 };
            List<int> col2 = new List<int> { 267, 258, 271 };
            ArrayList values = new ArrayList();
            values.Add(new BacNetReal {Value = (float)1});
            //Выключаем лампы
            foreach (var i in objects)
            {
                _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)i, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
            }

            //values[0] = new BacNetReal {Value = 100};
            //_device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)col1[0], ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
            while (true)
            {
                values[0] = new BacNetReal { Value = 100 };
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(400);
                    _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)col1[i], ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
                values[0] = new BacNetReal { Value = 1 };
                for (int i = 2; i >= 0; i--)
                {
                    Thread.Sleep(400);
                    _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)col1[i], ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
            }
            /*foreach (var i in col1)
            {
                _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)i, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
            }

            int k = 1;
            for (int i = 0; i < 3;i++ )
            {
                _device.Services.Confirmed.WriteProperty(17811,
                                                                     new BacNetObject
                                                                     {
                                                                         ObjectId = (uint)i,
                                                                         ObjectType =
                                                                             BacNetEnums.BACNET_OBJECT_TYPE.
                                                                             OBJECT_ANALOG_OUTPUT
                                                                     },
                                                                     BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE,
                                                                     values);
            }
                for (int j = 0; j < 1; j++)
                {
                    //Thread.Sleep(500);
                    foreach (var i in objects)
                    {
                        values[0] = new BacNetReal { Value = k };
                        if (k <= 100)
                            _device.Services.Confirmed.WriteProperty(17811,
                                                                     new BacNetObject
                                                                         {
                                                                             ObjectId = (uint)i,
                                                                             ObjectType =
                                                                                 BacNetEnums.BACNET_OBJECT_TYPE.
                                                                                 OBJECT_ANALOG_OUTPUT
                                                                         },
                                                                     BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE,
                                                                     values);
                        k += 18;
                    }

                }*/
            /*while (k < 20)
            {
                foreach (var i in objects)
                {
                    Thread.Sleep(500);
                    values[0] = new BacNetReal { Value = new Random().Next(0, 100) };
                    _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)i, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
                k++;
            }*/
        }

        private void OnPropertyWritten(uint deviceid, BacNetObject objectid, string status)
        {
            MessageBox.Show(deviceid + " " + objectid.ObjectId + " " + status);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedItem is uint)
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

        private void buttonLightOn_Click(object sender, EventArgs e)
        {
            /*List<int> objects = new List<int> { 276, 265, 268, 267, 258, 271 };

            ArrayList values = new ArrayList();
            values.Add(new BacNetReal { Value = (float)100 });
            //Выключаем лампы
            foreach (var i in objects)
            {
                _device.Services.Confirmed.WriteProperty(17811, new BacNetObject { ObjectId = (uint)i, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
            }*/
            List<string> objects = new List<string> { "17811.65551", "17811.65807",
                                                      "17812.68111", "17812.68367", "17812.68623",
                                                      "17821.1015", "17821.1115", "17821.1215",
                                                      "17822.65551", "17822.65807",
                                                      "17831.68111", "17831.86367", "17831.68623", "17831.277",
                                                      "17832.68111", "17832.68367",
                                                      "17841.68111", "17841.68367",
                                                      "17842.68111", "17842.68367", "17842.68623"};

            ArrayList values = new ArrayList();
            values.Add(new BacNetReal { Value = 100 });
            //Выключаем лампы
            foreach (var i in objects)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
            }
        }

        private void buttonLightOff_Click(object sender, EventArgs e)
        {
            List<string> objects = new List<string> { "17811.65551", "17811.65807",
                                                      "17812.68111", "17812.68367", "17812.68623",
                                                      "17821.1015", "17821.1115", "17821.1215",
                                                      "17822.65551", "17822.65807",
                                                      "17831.68111", "17831.86367", "17831.68623", "17831.277",
                                                      "17832.68111", "17832.68367",
                                                      "17841.68111", "17841.68367",
                                                      "17842.68111", "17842.68367", "17842.68623"};

            ArrayList values = new ArrayList();
            values.Add(new BacNetReal { Value = 0 });
            //Выключаем лампы
            foreach (var i in objects)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
            }
        }

        private void buttonLightStart_Click(object sender, EventArgs e)
        {
            List<string> objects = new List<string> { "17811.65550", "17811.65806",
                                                      "17812.68110", "17812.68366", "17812.68622",
                                                      "17821.1014", "17821.1114", "17821.1214",
                                                      "17822.65550", "17822.65806",
                                                      "17831.68110", "17831.86366", "17831.68622", "17831.276",
                                                      "17832.68110", "17832.68366",
                                                      "17841.68110", "17841.68366",
                                                      "17842.68110", "17842.68366", "17842.68622"};

            ArrayList values = new ArrayList();
            values.Add(new BacNetReal { Value = 0 });
            //Выключаем лампы
            foreach (var i in objects)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, values);
                }
            }

            
            lpList = new List<LampGroup>();
            //FASAD
            
            List<string> addresses = new List<string> {"17811.257", "17821.119", "17831.533", "17841.273"};
            LampGroup lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.271", "17821.123", "17831.530", "17841.281" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.268", "17821.118", "17831.536", "17841.282" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.269", "17822.34", "17832.27", "17842.520" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.270", "17822.37", "17832.33", "17842.527" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.276", "17822.267", "17832.275", "17842.267" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.277", "17822.257", "17832.276", "17842.265" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.271", "17822.256", "17832.277", "17842.271" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.316", "17822.260", "17832.273", "17842.273" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.548", "17822.271", "17832.270", "17842.262" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.532", "17822.274", "17832.266", "17842.263" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.540", "17822.276", "17832.268", "17842.270" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            //PRAVO
            addresses = new List<string> { "17812.535", "17822.272", "17832.264", "17842.266" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.534", "17822.9", "17832.17", "17842.519" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.512", "17822.21", "17832.7", "17842.531" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.524", "17822.23", "17832.13", "17842.534" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.19", "17822.19", "17832.32", "17842.535" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.16", "17822.20", "17832.12", "17842.533" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.15", "17822.205", "17831.6", "17841.2" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.24", "17822.25", "17832.16", "17842.544" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            //BACK
            addresses = new List<string> { "17812.18", "17822.29", "17832.20", "17842.540" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.5", "17822.28", "17832.15", "17842.542" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.29", "17822.32", "17832.29", "17842.553" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17812.2", "17821.222", "17831.21", "17842.545" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.60", "17821.221", "17831.20", "17842.541" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.12", "17821.217", "17831.13", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.8", "17821.209", "17831.25", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.10", "17821.216", "17831.23", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.7", "17821.227", "17831.26", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.60", "17821.234", "17831.28", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.1", "17821.238", "17831.36", "17841.24" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.3", "17821.244", "17831.32", "17841.21" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.5", "17821.242", "17831.39", "17841.23" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            //LEVO
            addresses = new List<string> { "17811.2", "17821.231", "17831.38", "17841.19" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.269", "17821.202", "17831.7", "17841.0" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.319", "17821.103", "17831.524", "17841.258" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.319", "17821.107", "17831.518", "17841.264" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.319", "17821.110", "17831.516", "17841.262" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.262", "17821.124", "17831.528", "17841.268" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.260", "17822.40", "17832.0", "17842.60" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
            addresses = new List<string> { "17811.261", "17821.115", "17831.537", "17841.284" };
            lp = new LampGroup(addresses, _device);
            lpList.Add(lp);
             
            foreach (var lampGroup in lpList)
            {
                lampGroup.Start();
                Thread.Sleep(1000);
            }
        }

        private List<LampGroup> lpList;

        private Task _sg1;
        private Task _sg2;
        private Task _sg3;

        private volatile bool go;

        private Task _sg;

        private void buttonLightStop_Click(object sender, EventArgs e)
        {
            foreach (var lampGroup in lpList)
            {
                lampGroup.Stop();
            }
        }

        private void buttonVertLight_Click(object sender, EventArgs e)
        {
            Thread s1 = new Thread(f1S) { IsBackground = true };
            Thread s2 = new Thread(f2S) { IsBackground = true };
            Thread s3 = new Thread(f3S) { IsBackground = true };
            Thread s4 = new Thread(f4S) { IsBackground = true };
            while (true)
            {
                s1 = new Thread(f1S) { IsBackground = true };
                s1.Start();
                Thread.Sleep(1250);
                s2 = new Thread(f2S) { IsBackground = true };
                s2.Start();
                Thread.Sleep(1250);
                s3 = new Thread(f3S) { IsBackground = true };
                s3.Start();
                Thread.Sleep(1250);
                s4 = new Thread(f4S) { IsBackground = true };
                s4.Start();
                Thread.Sleep(1250);
            }
            
        }

        private void f1S()
        {
            List<string> floor1 = new List<string> {  "17811.65551", "17811.65807",
                                                      "17812.68111", "17812.68367", "17812.68623"};
            ArrayList offValues = new ArrayList();
            offValues.Add(new BacNetReal { Value = 1 });
            ArrayList onValues = new ArrayList();
            onValues.Add(new BacNetReal { Value = 100 });

            foreach (var i in floor1)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, onValues);
                }
            }
            Thread.Sleep(1000);
            foreach (var i in floor1)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, offValues);
                }
            }
        }

        private void f2S()
        {
            List<string> floor2 = new List<string> {  "17821.1015", "17821.1115", "17821.1215",
                                                      "17822.65551", "17822.65807"};
            ArrayList offValues = new ArrayList();
            offValues.Add(new BacNetReal { Value = 1 });
            ArrayList onValues = new ArrayList();
            onValues.Add(new BacNetReal { Value = 100 });
            foreach (var i in floor2)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, onValues);
                }
            }
            Thread.Sleep(1000);
            foreach (var i in floor2)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, offValues);
                }
            }
        }

        private void f3S()
        {
            List<string> floor3 = new List<string> {  "17831.68111", "17831.86367", "17831.68623", "17831.277",
                                                      "17832.68111", "17832.68367"};
            ArrayList offValues = new ArrayList();
            offValues.Add(new BacNetReal { Value = 1 });
            ArrayList onValues = new ArrayList();
            onValues.Add(new BacNetReal { Value = 100 });

            foreach (var i in floor3)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, onValues);
                }
            }
            Thread.Sleep(1000);
            foreach (var i in floor3)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, offValues);
                }
            }
        }

        private void f4S()
        {
            List<string> floor4 = new List<string> {  "17841.68111", "17841.68367",
                                                      "17842.68111", "17842.68367", "17842.68623"};
            ArrayList offValues = new ArrayList();
            offValues.Add(new BacNetReal { Value = 1 });
            ArrayList onValues = new ArrayList();
            onValues.Add(new BacNetReal { Value = 100 });

            foreach (var i in floor4)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, onValues);
                }
            }
            Thread.Sleep(1000);
            foreach (var i in floor4)
            {
                string[] tmpAddr = i.Split('.');
                if (tmpAddr.Length == 2)
                {
                    ushort dev = Convert.ToUInt16(tmpAddr[0]);
                    uint num = Convert.ToUInt32(tmpAddr[1]);
                    _device.Services.Confirmed.WriteProperty(dev, new BacNetObject { ObjectId = num, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT }, BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE, offValues);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                uint dev = UInt32.Parse(listBox1.SelectedItem.ToString());

                _device.Services.Confirmed.CreateObject(dev, new BacNetObject()
                {
                    ObjectId = 8006,
                    ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_VALUE,
                    Properties = new List<BacNetProperty>() 
                    {
                        new BacNetProperty() 
                        { 
                            PropertyId = new BacNetUInt(77), 
                            Values = new ArrayList() 
                            {
                                //new BacNetInt(1),
                                new BacNetString("abc")
                            }
                        } 
                    }
                });
            }
            catch{}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                uint dev = UInt32.Parse(listBox1.SelectedItem.ToString());
                uint obj = UInt32.Parse(listBox2.SelectedItem.ToString().Substring(listBox2.SelectedItem.ToString().IndexOf('.') + 1));

                _device.Services.Confirmed.DeletObject(dev, obj);
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                /*uint dev = UInt32.Parse(listBox1.SelectedItem.ToString());
                uint obj = UInt32.Parse(listBox2.SelectedItem.ToString().Substring(listBox2.SelectedItem.ToString().IndexOf('.') + 1));
                string address = dev + "." + obj;*/
                _device.Services.Confirmed.SubscribeCOV("100.AV1");                
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*var address = textBox3.Text.Split('-');
            var propertyId = BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE;
            Enum.TryParse(address[1], out propertyId);
            var property = _device.Services.Confirmed.ReadProperty(address[0], propertyId);
            textBox4.Text = property.Value;*/


            var sch = new BacNetWeeklySchedule();
            sch.Monday.Add(new BacNetTime(12,14,2), true);
            sch.Monday.Add(new BacNetTime(14, 14, 2), null);
            _device.Services.Confirmed.WriteProperty((uint)200, new BacNetObject { ObjectId = 4, ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_SCHEDULE }, BacNetEnums.BACNET_PROPERTY_ID.PROP_WEEKLY_SCHEDULE, sch.ValueList);
            Thread.Sleep(500);
            var sch1 = new BacNetWeeklySchedule();
            sch1.ValueList = _device.Services.Confirmed.ReadProperty("200.SCH4", BacNetEnums.BACNET_PROPERTY_ID.PROP_WEEKLY_SCHEDULE).Values;
        }
    }
}
