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
                    BacNetBool boolValue = new BacNetBool(apdu, len, metaTag.Length, ref len);
                    res = boolValue;
                    break;
                case 2: //UNSIGNED_INT
                    BacNetUInt uIntValue = new BacNetUInt(apdu, len, metaTag.Length, ref len);
                    res = uIntValue;
                    break;
                case 3: //SIGNED_INT
                    BacNetInt intValue = new BacNetInt(apdu, len, metaTag.Length, ref len);
                    res = intValue;
                    break;
                case 4: //REAL
                    BacNetReal realValue = new BacNetReal(apdu, len, metaTag.Length, ref len);
                    res = realValue;
                    break;
                case 5: //DOUBLE
                    BacNetDouble doubleValue = new BacNetDouble(apdu, len, metaTag.Length, ref len);
                    res = doubleValue;
                    break;
                case 7: //CHARACTER STRING
                    BacNetString str = new BacNetString(apdu, len, metaTag.Length, ref len);
                    res = str;
                    break;
                case 9: //ENUMERATION
                    BacNetEnumeration enumValue = new BacNetEnumeration(apdu, len, metaTag.Length, ref len);
                    res = enumValue;
                    break;
                case 12: //OBJECT IDENTIFIER
                    BacNetObject obj = new BacNetObject(apdu, len, ref len);
                    res = obj;
                    break;
            }            
            return res;
        }

        public static byte[] GetPropertyValueBytes(object value, out int type)
        {
            ArrayList res = new ArrayList();
            BacNetBool boolValue = value as BacNetBool;
            if (boolValue != null)
            {
                type = 1;
                return boolValue.GetBytes();
            }
            BacNetUInt uIntValue = value as BacNetUInt;
            if (uIntValue != null)
            {
                type = 2;
                return uIntValue.GetBytes();
            }
            BacNetInt intValue = value as BacNetInt;
            if (intValue != null)
            {
                type = 3;
                return intValue.GetBytes();
            }
            BacNetReal realValue = value as BacNetReal;
            if (realValue != null)
            {
                type = 4;
                return realValue.GetBytes();
            }
            BacNetDouble doubleValue = value as BacNetDouble;
            if (doubleValue != null)
            {
                type = 5;
                return doubleValue.GetBytes();
            }
            BacNetString str = value as BacNetString;
            if (str != null)
            {
                type = 7;
                return str.GetBytes();
            }
            BacNetEnumeration enumValue = value as BacNetEnumeration;
            if (enumValue != null)
            {
                type = 9;
                return enumValue.GetBytes();
            }
            BacNetObject obj = value as BacNetObject;
            if (obj != null)
            {
                type = 12;
                return obj.GetObjectBytes();
            }

            type = 0;// убрать!!!!!!!!
            return (byte[])res.ToArray(typeof(byte));
        }
    }
}
