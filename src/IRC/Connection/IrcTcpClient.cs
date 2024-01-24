using System.Net;
using System.Net.Sockets;

namespace Meebey.SmartIrc4net
{
    internal class IrcTcpClient: TcpClient
    {
        public IrcTcpClient() : base() { }
        public IrcTcpClient(AddressFamily family) : base(family) { }
        public IrcTcpClient(IPEndPoint localEP)   : base(localEP) { }
        public IrcTcpClient(string hostname, int port) : base(hostname, port) { }

        public Socket Socket {
            get {
                return Client;
            }
        }
    }
}
