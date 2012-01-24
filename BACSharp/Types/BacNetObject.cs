using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BACSharp.Types
{
    public class BacNetObject
    {
        public BacNetEnums.BACNET_OBJECT_TYPE ObjectType { get; set; }
        public uint ObjectId { get; set; }

        public List<BacNetProperty> Properties { get; set; }

        private void InitializeFields()
        {
            Properties = new List<BacNetProperty>();
        }

        public BacNetObject()
        {
            InitializeFields();
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
                        ObjectId = (ushort)(ObjectId | (1 << i));
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

        public static BacNetObject Get(string id)
        {
            id = id.ToUpper();
            string objType = new Regex(@"[a-z\-A-Z]+").Match(id).Value;
            uint objId;
            if (!uint.TryParse(new Regex(@"[0-9]+").Match(id).Value, out objId))
                throw new Exception("Not valid string id - wrong object number");
            if (objType == string.Empty)
                throw new Exception("Not valid string id - empty object type");
            var obj = new BacNetObject { ObjectId = objId};
            switch (objType)
            {
                case "AC":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ACCUMULATOR;
                    break;
                case "AI":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_INPUT;
                    break;
                case "AO":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_OUTPUT;
                    break;
                case "AV":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_ANALOG_VALUE;
                    break;
                case "AR":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_AVERAGING;
                    break;
                case "BI":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_INPUT;
                    break;
                case "BO":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_OUTPUT;
                    break;
                case "BV":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_BINARY_VALUE;
                    break;
                case "CAL":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_CALENDAR;
                    break;
                case "CMD":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_COMMAND;
                    break;
                case "DEV":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_DEVICE;
                    break;
                case "EE":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_ENROLLMENT;
                    break;
                case "EL":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_EVENT_LOG;
                    break;
                case "FILE":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_FILE;
                    break;
                case "GG":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GLOBAL_GROUP;
                    break;
                case "GR":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_GROUP;
                    break;
                case "LSP":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_POINT;
                    break;
                case "LSZ":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIFE_SAFETY_ZONE;
                    break;
                case "LO":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LIGHTING_OUTPUT;
                    break;
                case "LC":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOAD_CONTROL;
                    break;
                case "LOOP":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_LOOP;
                    break;
                case "MI":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_INPUT;
                    break;
                case "MO":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_OUTPUT;
                    break;
                case "MV":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_MULTI_STATE_VALUE;
                    break;
                case "NC":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_NOTIFICATION_CLASS;
                    break;
                case "PROG":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PROGRAM;
                    break;
                case "PC":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_PULSE_CONVERTER;
                    break;
                case "SCH":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_SCHEDULE;
                    break;
                case "SV":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_STRUCTURED_VIEW;
                    break;
                case "TL":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TRENDLOG;
                    break;
                case "TLM":
                    obj.ObjectType = BacNetEnums.BACNET_OBJECT_TYPE.OBJECT_TREND_LOG_MULTIPLE;
                    break;
                default:
                    throw new Exception("Not valid string id - unknown object type");
            }
            return obj;
        }
    }
}
