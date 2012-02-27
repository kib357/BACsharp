using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BACSharp.Types
{
    public class BacNetObject
    {
        public BacNetEnums.BACNET_OBJECT_TYPE ObjectType { get; set; }
        public uint ObjectId { get; set; }

        public BacNetProperty PresentValue
        {
            get
            {
                BacNetProperty property = Properties.FirstOrDefault(s => s.PropertyId.Value == (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE);
                if (property == null)
                {
                    Properties.Add(new BacNetProperty((uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE));
                    property = Properties.Last();
                }
                return property;
            }
        }

        public List<BacNetProperty> Properties { get; set; }

        private void InitializeFields()
        {
            Properties = new List<BacNetProperty>();
        }

        public BacNetObject()
        {
            InitializeFields();
        }

        public BacNetObject(string id, bool initPresentValueProperty = false)
        {
            InitializeFields();

            if (initPresentValueProperty)
            {
                Properties.Add(new BacNetProperty {PropertyId = new BacNetUInt((uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)});
            }

            id = id.ToUpper();            

            uint objId;
            if (!uint.TryParse(new Regex(@"[0-9]+").Match(id).Value, out objId))
                throw new Exception("Not valid string id - wrong object number");
            ObjectId = objId;

            var objType = new Regex(@"[a-z\-A-Z]+").Match(id).Value;
            if (objType == string.Empty)
                throw new Exception("Not valid string id - empty object type");

            ObjectType = GetObjectType(objType);
        }

        public static BacNetEnums.BACNET_OBJECT_TYPE GetObjectType(string objType)
        {
            switch (objType)
            {
                case "AC":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ACCUMULATOR;
                case "AI":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT;
                case "AO":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT;
                case "AV":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE;
                case "AR":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_AVERAGING;
                case "BI":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT;
                case "BO":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT;
                case "BV":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_VALUE;
                case "CAL":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_CALENDAR;
                case "CMD":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_COMMAND;
                case "DEV":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE;
                case "EE":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_ENROLLMENT;
                case "EL":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_LOG;
                case "FILE":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_FILE;
                case "GG":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GLOBAL_GROUP;
                case "GR":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GROUP;
                case "LSP":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_POINT;
                case "LSZ":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_ZONE;
                case "LO":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIGHTING_OUTPUT;
                case "LC":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOAD_CONTROL;
                case "LOOP":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOOP;
                case "MI":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_INPUT;
                case "MO":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_OUTPUT;
                case "MV":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_VALUE;
                case "NC":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_NOTIFICATION_CLASS;
                case "PROG":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PROGRAM;
                case "PC":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PULSE_CONVERTER;
                case "SCH":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_SCHEDULE;
                case "SV":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_STRUCTURED_VIEW;
                case "TL":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TRENDLOG;
                case "TLM":
                    return BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TREND_LOG_MULTIPLE;
                default:
                    throw new Exception("Not valid string id - unknown object type");
            }
        }

        public BacNetObject(byte[] apdu, int startIndex, ref int len)
        {
            InitializeFields();

            if (apdu.Length - startIndex < 4)
                throw new Exception("Byte array length must be 4.");
            ushort objectType = 0;
            for (int i = 0; i < 32;i++)
            {
                if (i < 22)
                {
                    if (((apdu[startIndex + 3 - i / 8] >> (i % 8)) & 1) == 1)
                        //ObjectId = (ushort)(ObjectId | (1 << i));
                        ObjectId = (uint)(ObjectId | (1 << i));
                }
                else
                {
                    if (((apdu[startIndex + 3 - i / 8] >> (i % 8)) & 1) == 1)
                        objectType = (ushort)(objectType | (1 << (i - 22)));
                }
            }
            BacNetEnums.BACNET_OBJECT_TYPE tmpType;
            if (BacNetEnums.BACNET_OBJECT_TYPE.TryParse(objectType.ToString(), out tmpType))
                ObjectType = tmpType;
            len += 4;
        }

        public override string ToString()
        {
            string res = string.Empty;
            foreach (var bacNetProperty in Properties)
            {
                if (bacNetProperty.PropertyId.Value == (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_OBJECT_NAME)
                    foreach (var value in bacNetProperty.Values)
                    {
                        res += value ?? "" + " ";
                    }
                if (bacNetProperty.PropertyId.Value == (uint)BacNetEnums.BACNET_PROPERTY_ID.PROP_PRESENT_VALUE)
                    foreach (var value in bacNetProperty.Values)
                    {
                        res += value ?? "" + " ";
                    }
            }
            if (res == string.Empty)
                return ObjectType.ToString() + "." + ObjectId.ToString();
            return res;
        }

        public byte[] GetObjectBytes()
        {
            byte[] res = new byte[4];
            for (int i = 0; i < 10; i++)
            {
                if (i < 2)
                {
                    if (((int)ObjectType >> i & 1) == 1)
                        res[1] = (byte)(res[1] | (1 << (i + 6)));
                }
                else
                {
                    if (((int)ObjectType >> i & 1) == 1)
                        res[0] = (byte)(res[0] | (1 << (i - 2)));
                }
            }

            for (int i = 0; i < 22; i++)
            {
                if ((ObjectId >> i & 1) == 1)
                    res[3 - i / 8] = (byte)(res[3 - i / 8] | (1 << (i % 8)));
            }
            return res;
        }

        public string GetStringId()
        {
            string res = string.Empty;
            switch (ObjectType)
            {
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ACCUMULATOR:
                    res = "AC";break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT:
                    res = "AI"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT:
                    res = "AO"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE:
                    res = "AV"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_AVERAGING:
                    res = "AR"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT:
                    res = "BI"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT:
                    res = "BO"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_VALUE:
                    res = "BV"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_CALENDAR:
                    res = "CAL"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_COMMAND:
                    res = "CMD"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE:
                    res = "DEV"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_ENROLLMENT:
                    res = "EE"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_LOG:
                    res = "EL"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_FILE:
                    res = "FILE"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GLOBAL_GROUP:
                    res = "GG"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GROUP:
                    res = "GR"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_POINT:
                    res = "LSP"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_ZONE:
                    res = "LSZ"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIGHTING_OUTPUT:
                    res = "LO"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOAD_CONTROL:
                    res = "LC"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOOP:
                    res = "LOOP"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_INPUT:
                    res = "MI"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_OUTPUT:
                    res = "MO"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_VALUE:
                    res = "MV"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_NOTIFICATION_CLASS:
                    res = "NC"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PROGRAM:
                    res = "PROG"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PULSE_CONVERTER:
                    res = "PC"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_SCHEDULE:
                    res = "SCH"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_STRUCTURED_VIEW:
                    res = "SV"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TRENDLOG:
                    res = "TL"; break;
                case BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TREND_LOG_MULTIPLE:
                    res = "TLM"; break;
            }
            return res + ObjectId;
        }        
    }
}
