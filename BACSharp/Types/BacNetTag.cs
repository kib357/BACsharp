using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetTag
    {
        private byte _number;
        private bool _longTag;

        public bool LongTag 
        { 
            get { return _longTag; }
            set { _longTag = value; }
        }

        public byte Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    _number = value < 16 ? value : (byte) 15;
                }
            }
        }

        public bool Class { get; set; }

        private byte _length;

        public byte Length
        {
            get { return _length; }
            set
            {
                if (_length != value)
                {
                    _length = value;
                }
            }
        }

        public BacNetTag()
        {
            _longTag = false;
        }

        public BacNetTag(byte[] tagBytes, int startIndex, ref int len)
        {
            Number = (byte)(tagBytes[startIndex] >> 4);
            Class = ((tagBytes[startIndex] >> 3) & 1) == 1;
            byte tmpLength = (byte)(tagBytes[startIndex] & 7);
            if (tmpLength == 5)
            {
                Length = tagBytes[startIndex + 1];
                _longTag = true;
                len += 2;
            }
            else
            {
                Length = tmpLength;
                _longTag = false;
                len += 1;
            }
            
        }

        public byte[] GetBytes()
        {
            byte[] res = _longTag ? new byte[2] : new byte[1]; 
            
            for (int i = 0; i < 8; i++)
            {
                if (i < 3)
                {
                    if (((_longTag ? 5 : Length) >> i & 1) == 1)
                        res[0] = (byte) (res[0] | (1 << i));
                }
                if (i > 2 & i < 4) 
                {
                    if (Class)
                        res[0] = (byte)(res[0] | (1 << i));
                }
                if (i > 3)
                {
                    if ((Number >> (i - 4) & 1) == 1)
                        res[0] = (byte)(res[0] | (1 << i));
                }
            }
            if (_longTag)
                res[1] = Length;
            return res;
        }
    }
}
