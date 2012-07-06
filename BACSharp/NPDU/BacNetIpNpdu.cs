using System.Collections.Generic;
using BACSharp.Types;

namespace BACSharp.NPDU
{
    public class BacNetIpNpdu : IBacNetNpdu
    {
        public BacNetIpNpdu()
        {}

        public BacNetIpNpdu(byte[] npduBytes)
        {
            if (npduBytes.Length < 2) return;
            byte npci = npduBytes[1];
            bool destSpecified = false, sourceSpecified = false;
            for (int i = 0; i < 8; i++)
            {
                if ((npci >> i & 1) == 1)
                {
                    switch (i)
                    {
                        case 2:
                            ExpectingReply = true;
                            break;
                        case 3:
                            sourceSpecified = true;
                            break;
                        case 5:
                            destSpecified = true;
                            break;
                    }
                }
            }
            if (destSpecified)
            {
                Destination = new BacNetAddress();
                Destination.Network[0] = npduBytes[2];
                Destination.Network[1] = npduBytes[3];
                Destination.Address = new byte[npduBytes[4]];
                for (int i = 0; i < Destination.AddressLength; i++)
                    Destination.Address[i] = npduBytes[5 + i];
            }
            if (sourceSpecified)
            {
                int sourceStart = Destination == null ? 0 : 5 + Destination.AddressLength;
                Source = new BacNetAddress();
                Source.Network[0] = npduBytes[sourceStart + 2];
                Source.Network[1] = npduBytes[sourceStart + 3];
                Source.Address = new byte[npduBytes[sourceStart + 4]];
                for (int i = 0; i < Source.AddressLength; i++)
                    Source.Address[i] = npduBytes[sourceStart + 5 + i];
                if (destSpecified)
                    Destination.HopCount = npduBytes[sourceStart + 5 + Source.AddressLength];
            }
            if (destSpecified && !sourceSpecified)
                Destination.HopCount = npduBytes[5 + Destination.AddressLength];
        }

        public BacNetAddress Source { get; set; }

        public BacNetAddress Destination { get; set; }

        public bool ExpectingReply { get; set; }

        public BacNetEnums.BACNET_BVLC_FUNCTION Function = BacNetEnums.BACNET_BVLC_FUNCTION.BVLC_ORIGINAL_UNICAST_NPDU;

        public byte[] GetBytes()
        {
            List<byte> res = new List<byte>();

            res.Add(BacNetEnums.BACNET_BVLC_TYPE_BIP);
            res.Add((byte)Function);
            //Length
            res.Add(0);
            res.Add(0);
            res.Add(BacNetEnums.BACNET_PROTOCOL_VERSION);

            res.Add(0);
            if (ExpectingReply)
            {
                res[5] = (byte)(res[5] | (1 << 2));
            }
            if (Destination != null)
            {
                res[5] = (byte)(res[5] | (1 << 5));
                res.Add(Destination.Network[0]);
                res.Add(Destination.Network[1]);
                res.Add(Destination.AddressLength);
                for (int i = 0; i < Destination.AddressLength; i ++ )
                    res.Add(Destination.Address[i]);

                    if (Source != null)
                    {
                        res[5] = (byte)(res[5] | (1 << 3));
                        res.Add(Source.Network[0]);
                        res.Add(Source.Network[1]);
                        res.Add(Source.AddressLength);
                        for (int i = 0; i < Source.AddressLength; i++)
                            res.Add(Source.Address[i]);
                    }
                res.Add(Destination.HopCount);
            }
            if (Source != null)
            {
                res[5] = (byte)(res[5] | (1 << 3));
                res.Add(Source.Network[0]);
                res.Add(Source.Network[1]);
                res.Add(Source.AddressLength);
                for (int i = 0; i < Source.AddressLength; i++)
                    res.Add(Source.Address[i]);
            }

            return res.ToArray();
        }
    }
}
