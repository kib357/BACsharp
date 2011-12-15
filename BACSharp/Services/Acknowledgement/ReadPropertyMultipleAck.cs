using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BACSharp.APDU;
using BACSharp.Types;

namespace BACSharp.Services.Acknowledgement
{
    class ReadPropertyMultipleAck : IBacNetApdu
    {
        public BacNetObject[] ObjectList { get; set; }
        public byte InvokeId { get; set; }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }
}
