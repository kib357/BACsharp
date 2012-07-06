using BACSharp.NPDU;
using BACSharp.Services;

namespace BACSharp.Types
{
    public class BacNetMessage
    {
        public IBacNetApdu Apdu { get; set; }
        public IBacNetNpdu Npdu { get; set; }
    }
}
