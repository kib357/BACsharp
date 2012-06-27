using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp
{
    public class ByteConverter
    {
        public static byte[] GetBytes(UInt32 source)
        {
            ArrayList bytes = new ArrayList();
            for (int i = 0; i < 32; i++)
            {
                if ((source >> i & 1) == 1)
                {
                    if (bytes.Count == 0)
                        bytes.AddRange(new byte[4 - i/8]);

                    bytes[3 - i/8] = (byte) ((byte) bytes[3 - i/8] | (1 << (i%8)));
                }
            }
            if (bytes.Count == 0)
                bytes.Add((byte) 0);
            return (byte[])bytes.ToArray(typeof( byte ));
            
        }

        public static byte[] GetBytes(UInt16 source)
        {
            ArrayList bytes = new ArrayList();
            for (int i = 0; i < 16; i++)
            {
                if ((source >> i & 1) == 1)
                {
                    if (bytes.Count == 0)
                        bytes.AddRange(new byte[2 - i/8]);

                    bytes[1 - i/8] = (byte) ((byte) bytes[1 - i/8] | (1 << (i%8)));
                }
            }
            if (bytes.Count == 0)
                bytes.Add((byte)0);
            return (byte[])bytes.ToArray(typeof(byte));
        }

        public static object GetAppTagValue(byte[] apdu, int startIndex, BacNetTag metaTag, ref int len)
        {
            object res = null;
            switch (metaTag.Number)
            {
                case 1: //UNSIGNED_INT
                    var boolValue = new BacNetBool(metaTag);
                    res = boolValue;
                    break;
                case 2: //UNSIGNED_INT
                    var uIntValue = new BacNetUInt(apdu, len, metaTag.Length, ref len);
                    res = uIntValue;
                    break;
                case 3: //SIGNED_INT
                    var intValue = new BacNetInt(apdu, len, metaTag.Length, ref len);
                    res = intValue;
                    break;
                case 4: //REAL
                    var realValue = new BacNetReal(apdu, len, metaTag.Length, ref len);
                    res = realValue;
                    break;
                case 5: //DOUBLE
                    var doubleValue = new BacNetDouble(apdu, len, metaTag.Length, ref len);
                    res = doubleValue;
                    break;
                case 7: //CHARACTER STRING
                    var str = new BacNetString(apdu, len, metaTag.Length, ref len);
                    res = str;
                    break;
                case 9: //ENUMERATION
                    var enumValue = new BacNetEnumeration(apdu, len, metaTag.Length, ref len);
                    res = enumValue;
                    break;
                case 11: //TIME
                    var time = new BacNetTime(apdu, len, ref len);
                    res = time;
                    break;
                case 12: //OBJECT IDENTIFIER
                    var obj = new BacNetObject(apdu, len, ref len);
                    res = obj;
                    break;
            }            
            return res;
        }

        public static byte[] GetPropertyValueBytes(object value, out int type)
        {
            var res = new ArrayList();
            var boolValue = value as BacNetBool;
            if (boolValue != null)
            {
                type = 1;
                return boolValue.GetBytes();
            }
            var uIntValue = value as BacNetUInt;
            if (uIntValue != null)
            {
                type = 2;
                return uIntValue.GetBytes();
            }
            var intValue = value as BacNetInt;
            if (intValue != null)
            {
                type = 3;
                return intValue.GetBytes();
            }
            var realValue = value as BacNetReal;
            if (realValue != null)
            {
                type = 4;
                return realValue.GetBytes();
            }
            var doubleValue = value as BacNetDouble;
            if (doubleValue != null)
            {
                type = 5;
                return doubleValue.GetBytes();
            }
            var str = value as BacNetString;
            if (str != null)
            {
                type = 7;
                return str.GetBytes();
            }
            var enumValue = value as BacNetEnumeration;
            if (enumValue != null)
            {
                type = 9;
                return enumValue.GetBytes();
            }
            var obj = value as BacNetObject;
            if (obj != null)
            {
                type = 12;
                return obj.GetObjectBytes();
            }
            var schDayDictionary = value as Dictionary<BacNetTime, bool?>;
            if (schDayDictionary != null)
            {
                var timeTag = new BacNetTag() { Class = false, Length = 4, Number = 11 };
                var schRes = new ArrayList {(byte) 0x0E};
                foreach (var timeValuePair in schDayDictionary)
                {
                    schRes.AddRange(timeTag.GetBytes());
                    schRes.AddRange(timeValuePair.Key.GetBytes());

                    if (timeValuePair.Value != null)
                    {
                        schRes.AddRange(new BacNetTag {Class = false, Length = 1, Number = 9}.GetBytes());
                        schRes.Add((byte) (timeValuePair.Value == true ? 1 : 0));
                    }
                    else
                    {
                        schRes.Add((byte)(0x00));
                    }
                }
                schRes.Add((byte)0x0F);
                type = 0;
                return (byte[])schRes.ToArray(typeof(byte));
            }

            type = 0;// убрать!!!!!!!!
            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
