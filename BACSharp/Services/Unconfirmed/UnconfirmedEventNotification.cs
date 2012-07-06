using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.Types;

namespace BACSharp.Services.Unconfirmed
{
    public class UnconfirmedEventNotification : IBacNetApdu
    {
        public BacNetUInt ProccessId { get; private set; }
        public BacNetObject Device { get; private set; }
        public BacNetObject Object { get; private set; }
        public BacNetTimeStamp TimeStamp { get; private set; }
        public BacNetUInt NotificationClass { get; private set; }
        public BacNetUInt Priority { get; private set; }
        public BacNetUInt EventType { get; private set; }  
        public BacNetString Message { get; private set; }
        public BacNetUInt NotifyType { get; private set; }
        public BacNetUInt AckRequired { get; private set; }
        public BacNetUInt FromState { get; private set; }
        public BacNetUInt ToState { get; private set; }


        public UnconfirmedEventNotification(byte[] apdu) 
        {
            int len = 2;

            BacNetTag tag = new BacNetTag(apdu, len, ref len);
            ProccessId = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Device = new BacNetObject(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Object = new BacNetObject(apdu, len, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            if (tag.Length == 6 && tag.Number == 3)
            {
                tag = new BacNetTag(apdu, len, ref len);    
                if (tag.Length == 6 && tag.Number == 2)
                {
                    TimeStamp = new BacNetTimeStamp(apdu, len, ref len);
                }
                tag = new BacNetTag(apdu, len, ref len);
                tag = new BacNetTag(apdu, len, ref len);
                if (tag.Length != 7 && tag.Number != 3)
                    throw new Exception("Invalid TimeStamp");
            }

            tag = new BacNetTag(apdu, len, ref len);
            NotificationClass = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Priority = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            EventType = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            Message = new BacNetString(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            NotifyType = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            AckRequired = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            FromState = new BacNetUInt(apdu, len, tag.Length, ref len);

            tag = new BacNetTag(apdu, len, ref len);
            ToState = new BacNetUInt(apdu, len, tag.Length, ref len);
        }


        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
