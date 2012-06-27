using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetTime
    {
        public DateTime Value { get; set; }

        public override string ToString()
        {
            return Value.Hour + ":" + Value.Minute + ":" + Value.Second + "." + Value.Millisecond;
        }

        public BacNetTime()
        {
        }

        public BacNetTime(int hour, int minute, int second, int millisecond = 0)
        {
            Value = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, hour, minute, second, millisecond);
        }

        public BacNetTime(DateTime value)
        {
            Value = value;
        }

        public BacNetTime(byte[] apdu, int startIndex, ref int len)
        {
            var value = new byte[4];
            for (int i = 0; i < 4;i++ )
            {
                value[i] = apdu[startIndex + i];
            }
            Value = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, value[0], value[1], value[2], value[3] * 10);
            len += 4;
        }

        public byte[] GetBytes()
        {
            int ms = (int)(Value.Millisecond * 0.1);//.ToString(CultureInfo.InvariantCulture);
            var res = new[]
                {
                    (byte)Value.Hour,
                    (byte)Value.Minute,
                    (byte)Value.Second,
                    Convert.ToByte(ms)
                };
            return res;
        }
    }
}
