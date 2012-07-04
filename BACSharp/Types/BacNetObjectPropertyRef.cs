using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetObjectPropertyRef
    {
        public BacNetObject ObjectId { get; set; }
        public BacNetUInt PropertyId { get; set; }
    }
}

