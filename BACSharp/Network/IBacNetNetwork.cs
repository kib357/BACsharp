using System.Net;

namespace BACSharp.Network
{
    public interface IBacNetNetwork
    {
        void Send(byte[] message, IPEndPoint endPoint);
    }
}
