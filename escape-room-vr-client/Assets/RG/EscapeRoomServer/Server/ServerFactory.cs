using System.Net.Sockets;
using System.Threading;
using RG.EscapeRoom.Model.Rooms;
using RG.EscapeRoomProtocol;

namespace RG.EscapeRoomServer.Server
{
    public class ServerFactory
    {
        private ILogger logger;
        private CancellationTokenSource cancellationTokenSource;

        public ServerFactory(ILogger logger, CancellationTokenSource cancellationTokenSource)
        {
            this.logger = logger;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        public EscapeRoomSocketServer CreateServer(int port, string pathToRoomDefinition)
        {
            var protocolSerializer = new ProtocolSerializer();
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var udpMessageSender = new UdpMessageSender(socket, protocolSerializer);
            udpMessageSender.Init();
            RoomDefinitionParser roomDefinitionParser = new RoomDefinitionParser();
            return new EscapeRoomSocketServer(port, pathToRoomDefinition, roomDefinitionParser, logger, cancellationTokenSource, protocolSerializer, udpMessageSender);

        }
    }
}