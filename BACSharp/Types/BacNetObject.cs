using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetObject
    {
        public BacNetEnums.BACNET_OBJECT_TYPE ObjectType { get; set; }
        public uint ObjectId { get; set; }

        public BacNetProperty[] Properties { get; set; }

        public BacNetObject()
        {}

        public BacNetObject(byte[] apdu, int startIndex, ref int len)
        {
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
            return ObjectType.ToString() + "." + ObjectId.ToString();
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
    }
}
