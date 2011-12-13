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
        private byte[] _bvlc;
        private byte[] _npdu;
        private byte[] _apdu;

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
                        ReadBvlc(_all);
                        ReadNpdu(_all);
                        ReadApdu(_all);

                        if (Bvlc.Length < 4 || Npdu.Length < 2 || Apdu.Length == 0)
                            _all = null;
                    }
                    else
                        _all = null;
                }
            }
        }

#region BVLC

        public byte[] Bvlc
        {
            get { return _bvlc; }
        }

        private void ReadBvlc(byte[] message)
        {
            _bvlc = new byte[BvlcLength(message)];
            for (int i = 0; i < _bvlc.Length; i++)
                _bvlc[i] = message[i];
        }

        private int BvlcLength(byte[] message)
        {
            int bvlcLength = 0;
            if (message != null)
            {
                if (message[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_ORIGINAL_UNICAST_NPDU)
                    bvlcLength = 4;
                if (message[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_ORIGINAL_BROADCAST_NPDU)
                    bvlcLength = 4;
                if (message[1] == (byte)BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_FORWARDED_NPDU)
                    bvlcLength = 10;
            }
            return bvlcLength;
        }

#endregion BVLC

#region NPDU

        public byte[] Npdu
        {
            get { return _npdu; }
        }

        private void ReadNpdu(byte[] message)
        {
            _npdu = new byte[NpduLength(message)];
            for (int i = 0; i < _npdu.Length; i++)
                _npdu[i] = All[_bvlc.Length + i];
        }

        private int NpduLength(byte[] message)
        {
            if (_bvlc.Length + 2 > message.Length)
                return 0;
            int npduLength = 2, npduStart = _bvlc.Length;
            if (npduStart > 3)
            {
                byte version = message[npduStart];
                byte npci = message[npduStart + 1];                
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
                    destAddrLen = message[npduStart + 4];
                    npduLength = 6 + destAddrLen;
                }
                if (sourceSpecified)
                {
                    sourceAddrLen = destSpecified ? message[npduStart + 7 + destAddrLen] : message[npduStart + 4];
                    npduLength = destSpecified ? 9 + destAddrLen + sourceAddrLen : 5 + sourceAddrLen;
                }
                
            }
            return npduLength;
        }

#endregion NPDU

#region APDU

        public byte[] Apdu
        {
            get { return _apdu; }
        }

        private void ReadApdu(byte[] message)
        {
            _apdu = new byte[ApduLength(message)];
            for (int i = 0; i < _apdu.Length; i++)
                _apdu[i] = All[_bvlc.Length + _npdu.Length + i];
        }

        private int ApduLength(byte[] message)
        {
            int apduLength = 0;
            if (message != null)
            {
                apduLength = message.Length - Npdu.Length - Bvlc.Length;
            }
            return apduLength;
        }

#endregion APDU

    }
}
