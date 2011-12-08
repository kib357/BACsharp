using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetRawMessage
    {
        private byte[] _all;
        public byte[] All 
        {
            get { return _all; }
            set
            {
                if (_all != value)
                {
                    int tmp = 0;
                    int len = new BacNetInt(value, 2, 2, ref tmp).Value;
                    if (value.Length > 4 && len == value.Length)
                    {
                        _all = new byte[value.Length];
                        value.CopyTo(_all, 0);
                    }
                    else
                        _all = null;
                }
            }
        }
        public byte[] Bvlc
        {
            get
            {
                if (All == null)
                    return null;
                byte[] bvlc = new byte[BvlcLength()];
                for (int i = 0; i < bvlc.Length; i++)
                    bvlc[i] = All[i];
                return bvlc;
            }
        }

        public int BvlcLength()
        {
            int bvlcLength = 0;
            if (All != null)
            {
                if (All[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_ORIGINAL_UNICAST_NPDU)
                    bvlcLength = 4;
                if (All[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_ORIGINAL_BROADCAST_NPDU)
                    bvlcLength = 4;
                if (All[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_FORWARDED_NPDU)
                    bvlcLength = 10;
            }
            return bvlcLength;
        }

        public byte[] Npdu
        {
            get
            {
                if (All == null)
                    return null;
                int bvlcLength = BvlcLength();
                byte[] npdu = new byte[NpduLength()];
                for (int i = 0; i < npdu.Length; i++)
                    npdu[i] = All[bvlcLength + i];
                return npdu;
            }
        }

        public int NpduLength()
        {
            int npduLength = 2, npduStart = BvlcLength();
            if (All != null && npduStart > 3)
            {
                byte version = All[npduStart];
                byte npci = All[npduStart + 1];                
                bool destSpecified = false, sourceSpecified = false;
                for (int i = 0; i < 8; i++)
                {
                    if ((npci >> i & 1) == 1)
                    {
                        switch (i)
                        {
                            case 3:
                                sourceSpecified = true;
                                break;
                            case 5:
                                destSpecified = true;
                                break;
                        }
                    }
                }
                int destAddrLen = -1, sourceAddrLen = -1;
                if (destSpecified)
                {
                    destAddrLen = All[npduStart + 4];
                    npduLength = 6 + destAddrLen;
                }
                if (sourceSpecified)
                {
                    sourceAddrLen = destSpecified ? All[npduStart + 7 + destAddrLen] : All[npduStart + 4];
                    npduLength = destSpecified ? 9 + destAddrLen + sourceAddrLen : 5 + sourceAddrLen;
                }
                
            }
            return npduLength;
        }

        public byte[] Apdu
        {
            get
            {
                if (All == null)
                    return null;
                int bvlcLength = BvlcLength(), npduLength = NpduLength();
                byte[] apdu = new byte[ApduLength()];
                for (int i = 0; i < apdu.Length; i++)
                    apdu[i] = All[bvlcLength + npduLength + i];
                return apdu;
            }
        }

        public int ApduLength()
        {
            int apduLength = 0;
            if (All != null)
            {
                apduLength = All.Length - NpduLength() - BvlcLength();
            }
            return apduLength;
        }
    }
}
