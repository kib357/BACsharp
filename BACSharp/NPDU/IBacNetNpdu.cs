using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.NPDU
{
    public interface IBacNetNpdu
    {
        byte[] GetBytes();
    }
}
