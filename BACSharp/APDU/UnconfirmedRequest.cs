using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.APDU
{
    /*public class UnconfirmedRequest : IBacNetApdu
    {
        public byte ServiceNumber { get; set; }

        protected List<byte> _params = new List<byte>();

        public virtual byte[] GetBytes()
        {
            List<byte> res = new List<byte>();

            res.Add((byte)BacNetEnums.BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST);
            res.Add(ServiceNumber);
            res.AddRange(_params);

            return res.ToArray();
        }
    }*/
}
